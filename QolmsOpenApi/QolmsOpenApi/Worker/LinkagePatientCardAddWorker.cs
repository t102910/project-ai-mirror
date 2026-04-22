using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 診察券追加処理
    /// このクラスを修正する場合は必ず対応するテストも修正し全てパスさせるようにしてください。
    /// </summary>
    public class LinkagePatientCardAddWorker: LinkagePatientCardWorkerBase
    {
        ILinkageRepository _linkageRepository;
        IPatientCardRepository _cardRepository;
        IAccountRepository _accountRepository;
        UserFamilyAddWorker _familyAddWorker;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkageRepository"></param>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        /// <param name="patientCardRepository"></param>
        /// <param name="storageRepository"></param>
        public LinkagePatientCardAddWorker(
            ILinkageRepository linkageRepository, 
            IAccountRepository accountRepository, 
            IFamilyRepository familyRepository,
            IPatientCardRepository patientCardRepository,
            IStorageRepository storageRepository
            ):base(linkageRepository,accountRepository)
        {
            _linkageRepository = linkageRepository;
            _cardRepository = patientCardRepository;
            _accountRepository = accountRepository;

            _familyAddWorker = new UserFamilyAddWorker(accountRepository, familyRepository, storageRepository);
        }

        /// <summary>
        /// 診察券追加実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoLinkagePatientCardAddApiResults Add(QoLinkagePatientCardAddApiArgs args)
        {
            var results = new QoLinkagePatientCardAddApiResults
            {
                IsSuccess = bool.FalseString
            };

            // ActorKey, Birthday変換チェック
            if(!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey),Guid.Empty,results, out var accountKey) ||               
               !args.Birthday.CheckArgsConvert(nameof(args.Birthday),DateTime.MinValue, results, out var birthDate))
            {
                return results;
            }

            // 家族アカウント作成フラグ変換
            var withFamilyAccountRegistration = args.WithFamilyAccountRegistration.TryToValueType(false);

            // 連携システム番号変換（未設定でもエラーにしない。互換性のため）
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);

            // 施設キー変換チェック
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (facilityKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.FacilityKeyReference)}が不正です。");
                return results;
            }

            // 性別変換チェック
            var sexType = (QsDbSexTypeEnum)args.SexType.TryToValueType((byte)QsDbSexTypeEnum.None);
            if (sexType == QsDbSexTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.SexType)}が不正です。");
                return results;
            }

            // 必須チェック
            if(!args.FamilyName.CheckArgsRequired(nameof(args.FamilyName),results) ||
               !args.GivenName.CheckArgsRequired(nameof(args.GivenName), results) ||
               !args.FamilyKanaName.CheckArgsRequired(nameof(args.FamilyKanaName), results) ||
               !args.GivenKanaName.CheckArgsRequired(nameof(args.GivenKanaName), results) ||
               !args.LinkUserId.CheckArgsRequired(nameof(args.LinkUserId), results))
            {
                return results;
            }            

            // 診察券の連携システム番号を取得
            var cardLinkageNo = _linkageRepository.GetLinkageNo(facilityKey);
            if (cardLinkageNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "施設キーを連携システム番号に変換できませんでした。");
                return results;
            }

            // 利用者カード重複チェック
            if (!CheckCardNo(facilityKey, args.LinkUserId, results))
            {
                return results;
            }

            // ユーザ情報 照合（家族登録なしの場合のみチェック）
            if (!withFamilyAccountRegistration && !CheckUserInfo(accountKey, birthDate, sexType, results))
            {
                return results;
            }

            try
            {
                // トランザクション開始
                using (var tran = new QoTransaction())
                {
                    var targetAccountKey = accountKey;
                    // 家族アカウント作成フラグがあれば先に家族を追加する
                    if (withFamilyAccountRegistration)
                    {
                        if (!AddFamilyUser(accountKey, args.ExecutorName, args, results, out var familyUser))
                        {
                            return results;
                        }
                        // ターゲットを家族アカウントに
                        targetAccountKey = familyUser.AccountKeyReference.ToDecrypedReference<Guid>();
                        // 戻り値のアカウント情報をセット
                        results.User = familyUser;
                        // 互換用 将来的に削除する
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
                        results.Account = ConvetApiAccountItem(familyUser);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
                    }

                    

                    // 連携システム番号が指定されているときのみ親施設処理を行う
                    if (linkageSystemNo != int.MinValue)
                    {
                        // 親施設の取得と連携登録
                        if (!AddParentLinkage(targetAccountKey, facilityKey, linkageSystemNo, results))
                        {
                            return results;
                        }
                    }

                    // 診察券登録処理
                    if (!AddPatientCard(args.AuthorKey.TryToValueType(Guid.Empty), targetAccountKey, cardLinkageNo, facilityKey, args.LinkUserId, results, out var seq))
                    {
                        return results;
                    }

                    // 追加した診察券情報を取得
                    if (!TryReadPatientCard(targetAccountKey, cardLinkageNo, seq, results, out var cardItem))
                    {
                        return results;
                    }

                    results.PatientCardItem = cardItem;

                    // コミット
                    tran.Commit();
                }
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "その他のエラーにより診察券追加処理に失敗しました。");
                return results;
            }
            
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryReadPatientCard(Guid accountKey, int cardLinkageNo, int seq, QoApiResultsBase results, out QoApiPatientCardItem cardItem)
        {
            cardItem = null;
            try
            {
                var entity = _cardRepository.ReadPatientCard(accountKey, cardLinkageNo, seq);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "追加した診察券情報が取得できませんでした。");
                    return false;
                }

                cardItem = entity.ToApiPatientCardItem(QoPatientCardBarcodeHelper.CreateCustomBarcode);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "診察券情報の取得に失敗しました。");
                return false;
            }
        }        

        // 家族追加処理
        bool AddFamilyUser(Guid accountKey, string executorName, QoLinkagePatientCardAddApiArgs args, QoApiResultsBase results, out QoApiUserItem familyUser)
        {
            familyUser = null;
            try
            {
                var user = new QoApiUserItem
                {
                    FamilyName = args.FamilyName,
                    GivenName = args.GivenName,
                    FamilyNameKana = args.FamilyKanaName,
                    GivenNameKana = args.GivenKanaName,
                    NickName = args.NickName,
                    Birthday = args.Birthday,
                    Sex = args.SexType.TryToValueType((byte)0),
                };
                var familyArgs = new QoUserFamilyAddApiArgs
                {
                    User = user,
                    ActorKey = args.ActorKey,
                    ExecuteSystemType = args.ExecuteSystemType,
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName,                    
                };

                var familyResult = _familyAddWorker.Add(familyArgs);
                if(familyResult.IsSuccess != bool.TrueString)
                {
                    results.Result = familyResult.Result;
                    return false;
                }

                familyUser = familyResult.User;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "家族アカウントの追加に失敗しました。");
                return false;
            }           
        }        

        QoApiAccountFamilyViewItem ConvetApiAccountItem(QoApiUserItem user)
        {
            return new QoApiAccountFamilyViewItem
            {
                AccountKeyReference = user.AccountKeyReference,
                FamilyName = user.FamilyName,
                GivenName = user.GivenName,
                FamilyNameKana = user.FamilyNameKana,
                GivenNameKana = user.GivenNameKana,
                AccessKey = user.AccessKey,
                Birthday = user.Birthday,
                Sex = $"{(int)user.Sex}",
            };            
        }
    }
}
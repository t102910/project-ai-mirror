using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 連携ユーザー登録処理
    /// このクラスを修正する場合は必ず対応するテストも修正し全てパスさせるようにしてください。
    /// </summary>
    public class LinkageRegisterWorker: LinkageRegisterWorkerBase
    {        
        ISignUpRepository _signUpRepository;
        INoticeApiRepository _noticeApi;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="linkageRepository"></param>
        /// <param name="identityApiRepository"></param>
        /// <param name="signUpRepository"></param>
        /// <param name="noticeApiRepository"></param>
        /// <param name="accountRepository"></param>
        public LinkageRegisterWorker(
            ILinkageRepository linkageRepository, 
            IIdentityApiRepository identityApiRepository, 
            ISignUpRepository signUpRepository, 
            INoticeApiRepository noticeApiRepository, 
            IAccountRepository accountRepository):base(linkageRepository,identityApiRepository,accountRepository)
        {            
            _signUpRepository = signUpRepository;
            _noticeApi = noticeApiRepository;
        }

        /// <summary>
        /// 連携ユーザーとして本登録を行う
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoLinkageNewUserRegisterApiResults RegisterNewUser(QoLinkageNewUserRegisterApiArgs args)
        {
            var results = new QoLinkageNewUserRegisterApiResults
            {
                IsSuccess = bool.FalseString
            };
            
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);
            if (linkageSystemNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError,$"{nameof(args.LinkageSystemNo)}情報が不正です。");
                return results;
            }

            // 仮アカウントキーのデコード
            var preAccountKey = args.AccountKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if(preAccountKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.AccountKeyReference)}が不正です。");
                return results;
            }

            // 施設キー変換チェック
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (facilityKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "FacilityKeyが不明です");
                return results;
            }

            // 生年月日変換チェック
            var birthDate = args.Birthday.TryToValueType(DateTime.MinValue);
            if(birthDate == DateTime.MinValue)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Birthday)}が不正です。");
                return results;
            }

            // 性別変換チェック
            var sexType = (QsDbSexTypeEnum)args.Sex.TryToValueType((byte)QsDbSexTypeEnum.None);
            if(sexType == QsDbSexTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.Sex)}が不正です。");
                return results;
            }

            // 必須チェック
            if (!ArgsValidationHelper.CheckArgsRequired(args.FamilyName, nameof(args.FamilyName), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.GivenName, nameof(args.GivenName), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.GivenNameKana, nameof(args.GivenNameKana), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.FamilyNameKana, nameof(args.FamilyNameKana), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.Password, nameof(args.Password), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.Sex, nameof(args.Sex), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.Birthday, nameof(args.Birthday), results) ||
                !ArgsValidationHelper.CheckArgsRequired(args.LinkageSystemId, nameof(args.LinkageSystemId), results))
            {
                return results;
            }

            // 規約同意チェック
            if (!args.IsAgreePrivacyPolicy.TryToValueType(false))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "プライバシーポリシーの同意がありません。");
                return results;
            }
            if(!args.IsAgreeTermsOfService.TryToValueType(false))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "利用規約の同意がありません。");
                return results;
            }
            
            // 家族情報がある場合は家族情報引数チェック
            if (args.FamilyAccountInfo != null)
            {
                var familyinfoErrors = new List<string>();
                if(!args.FamilyAccountInfo.Validate(ref familyinfoErrors))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"家族情報が不正です。{string.Join(",",familyinfoErrors)}");
                    return results;
                }
            }

            // 診察券の連携システム番号を取得
            var cardLinkageNo = _linkageRepository.GetLinkageNo(facilityKey);
            if (cardLinkageNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "施設キーを連携システム番号に変換できませんでした。");
                return results;
            }

            // 仮登録データチェック
            if(!CheckPreRegister(preAccountKey, results, out var mailAddr))
            {
                return results;
            }

            // 退会後既定時間内の新規登録の場合エラー返却
            if (!CheckRegisterInterval(args.LinkageSystemId,facilityKey, results))
            {
                return results;
            }

            // 利用者カード重複チェック
            if (!CheckCardNo(facilityKey, args.LinkageSystemId, results))
            {
                return results;
            }

            // 同一情報のユーザーの存在チェック
            if(!CheckDuplicateUser(mailAddr,args.FamilyName,args.GivenName,birthDate,sexType,results))
            {
                return results;
            }

            // 仮連携ID生成
            var newLinkageSystemID = Guid.NewGuid();

            // ユーザー登録処理
            var registerResult = TryRegisterUser(linkageSystemNo, newLinkageSystemID.ToString("N") , args.Password, args.FamilyName, args.GivenName, args.FamilyNameKana, args.GivenNameKana, args.Sex, args.Birthday, mailAddr);

            if (!registerResult.isSuccess)
            {
                // エラー
                results.Result = registerResult.apiResult;
                return results;
            }

            // 仮で設定していたPush通知IDを更新する
            if (!TryUpdateNotificationId(registerResult.accountKey, linkageSystemNo, results))
            {
                // 失敗時アカウントも削除
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // 診察券対象アカウントキー設定
            var cardAccountKey = registerResult.accountKey;
            // 家族ユーザー登録
            if(args.FamilyAccountInfo != null)
            {
                if(!AddFamilyUser(registerResult.accountKey, args.ExecutorName,args.FamilyAccountInfo, results, out var familyAccountKey))
                {
                    // 失敗時アカウントも削除
                    WithdrawAccount(args,linkageSystemNo, registerResult.accountKey);
                    return results;
                }
                // 診察券対象アカウントキーを家族に
                cardAccountKey = familyAccountKey;
            }

            // 親施設の取得と連携登録
            if(!AddParentLinkage(cardAccountKey, facilityKey, linkageSystemNo, results))
            {
                // 失敗時アカウントも削除
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // 診察券登録処理
            if (!AddPatientCard(args.AuthorKey.TryToValueType(Guid.Empty), cardAccountKey, cardLinkageNo, facilityKey, args.LinkageSystemId, results, out var _))
            {
                // 失敗時アカウントも削除（子も削除されるはず）
                WithdrawAccount(args, linkageSystemNo, registerResult.accountKey);
                return results;
            }

            // 仮登録データ削除（失敗しても続行する）
            _signUpRepository.DeleteSignUpData(preAccountKey);

            // 本登録完了 ID案内メール送信
            _ = _noticeApi.SendIdNotificationMail(mailAddr, registerResult.userId, args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.UserId = registerResult.userId;
            results.LinkageIdReference = registerResult.accountKey.ToEncrypedReference();
            results.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(GetEncryptExecutor(args.Executor), registerResult.accountKey);

            return results;
        }

        /// <summary>
        /// 仮登録チェック
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        bool CheckPreRegister(Guid accountKey, QoApiResultsBase results, out string mail)
        {
            mail = string.Empty;
            try
            {
                var idResults = _identityApi.ExecuteRegisterReadApi(accountKey);
                if(idResults.IsSuccess != bool.TrueString)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.InternalServerError, "仮登録データの取得に失敗しました。");
                    return false;
                }

                mail = idResults.MailAddress;

                if(string.IsNullOrEmpty(idResults.MailAddress))
                {
                    // 仮登録データが見つからなかった場合はSuccess Trueでメールアドレスに空がセットされる
                    // V1/QolmsIdentityApiOnAuzre/QolmsIdentityApi/Workers/Qolms/QhRegisterWorker
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SignUpInfoNotFound, "仮登録データが見つかりませんでした。");
                    return false;
                }

                var expires = idResults.Expires.TryToValueType(DateTime.MinValue);
                if(expires < DateTime.Now)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountRegisterExpired, "仮登録の期限が切れています。");
                    return false;
                }

                return true;

            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex);
                return false;
            }
        }
    }
}
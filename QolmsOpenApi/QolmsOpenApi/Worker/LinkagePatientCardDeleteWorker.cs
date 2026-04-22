using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 診察券削除処理
    /// このクラスを修正する場合は必ず対応するテストも修正し全てパスさせるようにしてください。
    /// </summary>
    public class LinkagePatientCardDeleteWorker : LinkagePatientCardWorkerBase
    {
        ILinkageRepository _linkageRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkageRepository"></param>
        /// <param name="accountRepository"></param>
        public LinkagePatientCardDeleteWorker(ILinkageRepository linkageRepository, IAccountRepository accountRepository) : base(linkageRepository, accountRepository)
        {
            _linkageRepository = linkageRepository;
        }

        /// <summary>
        /// 診察券削除実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoLinkagePatientCardDeleteApiResults Delete(QoLinkagePatientCardDeleteApiArgs args)
        {
            var results = new QoLinkagePatientCardDeleteApiResults
            {
                IsSuccess = bool.FalseString
            };

            if(!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey) ||
               !args.AuthorKey.CheckArgsConvert(nameof(args.AuthorKey), Guid.Empty, results, out var authorKey))
            {
                return results;
            }

            // 連携システム番号変換（未設定でもエラーにしない。互換性のため）
            var linkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue);

            // 施設キー変換チェック
            var facilityKey = args.FacilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (facilityKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.FacilityKeyReference)}が不正です。");
                return results;
            }

            // 診察券の連携システム番号を取得
            var cardLinkageNo = _linkageRepository.GetLinkageNo(facilityKey);
            if (cardLinkageNo < 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "施設キーを連携システム番号に変換できませんでした。");
                return results;
            }

            // トランザクション開始
            using (var tran = new QoTransaction())
            {
                // 診察券削除
                if(!DeletePatientCard(authorKey, accountKey, cardLinkageNo, facilityKey, results))
                {
                    return results;
                }
                // 親連携解除（関連診察券がゼロになれば）
                if(!DeleteParentLinkage(accountKey, facilityKey, linkageSystemNo, results))
                {
                    return results;
                }

                // トランザクション確定
                tran.Commit();
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool DeletePatientCard(Guid authorKey, Guid accountKey, int cardLinkageNo, Guid facilityKey, QoApiResultsBase apiResult)
        {
            try
            {
                // 削除フラグを立てて更新
                (var errors, var entity) = _linkageRepository.WriteLinkagePatientCard(authorKey, accountKey, cardLinkageNo, facilityKey, string.Empty, 1, true);

                if (string.IsNullOrWhiteSpace(errors))
                {
                    return true;
                }

                apiResult.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"診察券の登録に失敗しました。{errors}");
                return false;
            }
            catch (Exception ex)
            {
                apiResult.Result = QoApiResult.Build(ex,"診察券削除処理でエラーが発生しました。");
                return false;
            }
        }

        bool DeleteParentLinkage(Guid accountKey, Guid facilityKey, int rootLinkageSystemNo, QoApiResultsBase apiResult)
        {
            try
            {
                if(rootLinkageSystemNo == int.MinValue)
                {
                    // 連携システム番号が指定されていなければスキップする
                    return true;
                }

                var parentLinkage = _linkageRepository.GetParentLinkageMst(facilityKey);
                if (parentLinkage == null)
                {
                    // 親施設がない場合は処理する必要がないので正常で返す
                    return true;
                }

                var parentLinkageNo = parentLinkage.LINKAGESYSTEMNO;
                if (parentLinkageNo == rootLinkageSystemNo)
                {
                    // 親連携番号がルート連携番号と同一の場合は追加処理はスキップ
                    // Api引数のLinkageSystemNoがルート連携システム番号だが
                    // これはユーザー登録とセットで連携されるのでここでの追加は不要
                    return true;
                }

                // 子の連携情報を取得
                var children = _linkageRepository.ReadChildLinkageList(accountKey, parentLinkage.FACILITYKEY);

                if (children.Any())
                {
                    // 子の連携がまだ存在する場合は親連携は解除しない
                    return true;
                }

                if (_linkageRepository.ReadEntity(accountKey, parentLinkageNo) == null)
                {
                    // 親連携レコードが存在しなければ何もせず正常で返す
                    return true;
                }

                var entity = new QH_LINKAGE_DAT
                {
                    ACCOUNTKEY = accountKey,
                    LINKAGESYSTEMNO = parentLinkageNo,
                    LINKAGESYSTEMID = "",
                    DATASET = "",
                    STATUSTYPE = 2,               
                };

                // 親施設に関連する子施設の連携を一つも持たなくなれば親連携も解除する
                _linkageRepository.DeleteEntity(entity);

                return true;
            }
            catch (Exception ex)
            {
                apiResult.Result = QoApiResult.Build(ex, "上位施設の連携解除処理でエラーが発生しました。");
                return false;
            }
        }
    }
}
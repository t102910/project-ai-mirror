using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 診察券処理の基底クラス
    /// このクラスを修正する場合は必ず対応するテストも修正し全てパスさせるようにしてください。
    /// </summary>
    public class LinkagePatientCardWorkerBase
    {
        ILinkageRepository _linkageRepository;
        IAccountRepository _accountRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkageRepository"></param>
        /// <param name="accountRepository"></param>
        public LinkagePatientCardWorkerBase(ILinkageRepository linkageRepository,IAccountRepository accountRepository)
        {
            _linkageRepository = linkageRepository;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// カードが利用可能かどうかチェック
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="cardNo"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool CheckCardNo(Guid facilityKey, string cardNo, QoApiResultsBase results)
        {
            try
            {
                if (!_linkageRepository.IsAvailableCard(facilityKey, cardNo))
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.PatientCardDuplicate, "この診察券番号は既に使われています。");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex);
                return false;
            }
        }

        /// <summary>
        /// 親施設との連携処理
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="facilityKey"></param>
        /// <param name="rootLinkageSystemNo"></param>
        /// <param name="apiResult"></param>
        /// <returns></returns>
        protected bool AddParentLinkage(Guid accountKey, Guid facilityKey, int rootLinkageSystemNo, QoApiResultsBase apiResult)
        {
            try
            {
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

                if(_linkageRepository.ReadEntity(accountKey, parentLinkageNo) != null)
                {
                    // 既に連携レコードが存在すれば何もせず正常で返す
                    return true;
                }

                var entity = new QH_LINKAGE_DAT
                {
                    ACCOUNTKEY = accountKey,
                    LINKAGESYSTEMNO = parentLinkageNo,
                    LINKAGESYSTEMID = "",
                    DATASET = "",
                    STATUSTYPE = 2, // 承認済みとして登録                    
                };

                _linkageRepository.InsertEntity(entity);

                return true;
            }
            catch (Exception ex)
            {
                apiResult.Result = QoApiResult.Build(ex, "上位施設の連携処理でエラーが発生しました。");
                return false;
            }
        }

        /// <summary>
        /// 診察券追加処理
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="accountKey"></param>
        /// <param name="cardLinkageNo"></param>
        /// <param name="facilityKey"></param>
        /// <param name="cardLinkageId"></param>
        /// <param name="apiResult"></param>
        /// <param name="sequence">追加した診察券のSEQUENCE番号</param>
        /// <returns></returns>
        protected bool AddPatientCard(Guid authorKey, Guid accountKey, int cardLinkageNo, Guid facilityKey, string cardLinkageId, QoApiResultsBase apiResult, out int sequence)
        {
            sequence = int.MinValue;
            try
            {               

                (var errors, var entity) = _linkageRepository.WriteLinkagePatientCard(authorKey, accountKey, cardLinkageNo, facilityKey, cardLinkageId, int.MinValue, false);

                if (string.IsNullOrWhiteSpace(errors))
                {
                    sequence = entity.SEQUENCE;
                    return true;
                }

                if (errors.Contains("指定された種類のカードは、すでに登録があります"))
                {
                    apiResult.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.PatientCardDuplicate, errors);
                    return false;
                }

                apiResult.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"診察券の登録に失敗しました。{errors}");
                return false;
            }
            catch (Exception ex)
            {
                apiResult.Result = QoApiResult.Build(ex);
                return false;
            }
        }

        /// <summary>
        /// システムに登録されているユーザー情報と一致しているかを確認する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="birthDate"></param>
        /// <param name="sexType"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool CheckUserInfo(Guid accountKey, DateTime birthDate, QsDbSexTypeEnum sexType, QoApiResultsBase results)
        {
            try
            {
                var entity = _accountRepository.ReadAccountIndexDat(accountKey);

                // 生年月日と性別で照合
                // 姓名カナはユーザーが訂正して一致しない可能性があるため照合しない
                if(entity.BIRTHDAY != birthDate)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.PatientCardUserInfoInvalid, "生年月日が登録情報と一致しません。");
                    return false;
                }
                if(entity.SEXTYPE != (byte)sexType)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.PatientCardUserInfoInvalid, "性別が登録情報と一致しません。");
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ユーザー情報照合処理でエラーが発生しました。");
                return false;
            }
        }
    }
}
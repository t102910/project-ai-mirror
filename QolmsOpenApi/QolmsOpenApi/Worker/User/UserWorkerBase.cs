using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsCryptV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 
    /// </summary>
    public class UserWorkerBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected IAccountRepository _accountRepo;

        /// <summary>
        /// 
        /// </summary>
        protected IFamilyRepository _familyRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="familyRepository"></param>
        public UserWorkerBase(IAccountRepository accountRepository, IFamilyRepository familyRepository)
        {
            _accountRepo = accountRepository;
            _familyRepo = familyRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool CheckParentAccount(Guid accountKey, QoApiResultsBase results)
        {
            try
            {
                var entity = _accountRepo.ReadMasterEntity(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "このアカウントは存在しません。");
                    return false;
                }

                if (entity.PRIVATEACCOUNTFLAG)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "このアカウントに子を追加することはできません。");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント情報取得処理でエラーが発生しました。");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parenteAccountKey"></param>
        /// <param name="childAccountKey"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool TryCheckRelation(Guid parenteAccountKey, Guid childAccountKey, QoApiResultsBase results)
        {
            try
            {
                var isOK = _familyRepo.IsParentChildRelation(parenteAccountKey, childAccountKey);
                if (!isOK)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "親子関係が成立していません。");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント関連情報の取得に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="results"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected bool TryReadAccountIndex(Guid accountKey, QoApiResultsBase results, out QH_ACCOUNTINDEX_DAT entity)
        {
            entity = null;
            try
            {
                entity = _accountRepo.ReadAccountIndexDat(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "対象アカウントが存在しません。");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント情報の取得に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool TryWriteAccoutIndex(QH_ACCOUNTINDEX_DAT entity, QoApiResultsBase results)
        {
            try
            {
                _accountRepo.UpdateIndexEntity(entity);
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント情報の更新に失敗しました。");
                return false;
            }
        }
    }
}
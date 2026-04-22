
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 子アカウント関連のReader
    /// </summary>
    internal sealed class AccountFamilyReader : QsDbReaderBase, IQsDbDistributedReader<QH_ACCOUNTINDEX_DAT, AccountFamilyReaderArgs, AccountFamilyReaderResults>
    {



        /// <summary>
        /// <see cref="AccountFamilyReader" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AccountFamilyReader() : base()
        {
        }



        public AccountFamilyReaderResults ExecuteByDistributed(AccountFamilyReaderArgs args)
        {
            AccountFamilyReaderResults result = new AccountFamilyReaderResults() { IsSuccess = false };

            result.Result = new DbAccountFamilyReaderCore(args.AccountKey).ReadAccountFamilyList(args.IsIncludeMine);
            result.IsSuccess = true;
           

            return result;
        }
    }


}
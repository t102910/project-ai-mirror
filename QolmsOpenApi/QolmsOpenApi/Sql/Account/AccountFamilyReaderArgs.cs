using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 
    /// </summary>
    internal sealed class AccountFamilyReaderArgs : QsDbReaderArgsBase<QH_ACCOUNTINDEX_DAT>
    {
        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 自分自身（親アカウント）を含めるかどうかを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsIncludeMine { get; set; } = true;



        /// <summary>
        /// <see cref="AccountFamilyReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AccountFamilyReaderArgs() : base()
        {
        }
    }


}
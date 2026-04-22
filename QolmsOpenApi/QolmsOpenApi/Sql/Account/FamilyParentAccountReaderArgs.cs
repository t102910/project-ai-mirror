using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 
    /// </summary>
    internal sealed class FamilyParentAccountReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// アカウントキー(子か親か不明）を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;


        /// <summary>
        /// <see cref="FamilyParentAccountReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FamilyParentAccountReaderArgs() : base()
        {
        }
    }


}
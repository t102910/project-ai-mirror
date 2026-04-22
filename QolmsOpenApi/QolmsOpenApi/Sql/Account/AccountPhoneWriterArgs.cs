using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class AccountPhoneWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 対象のEntityを設定する
        /// </summary>
        public QH_ACCOUNTPHONE_MST Entity { get; set; }

        /// <summary>
        /// 物理削除するかどうかを設定する
        /// </summary>
        public bool IsPhysicalDelete { get; set; } = false;
    }
}
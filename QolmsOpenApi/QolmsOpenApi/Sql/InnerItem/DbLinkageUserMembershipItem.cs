using System;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// DB用の利用規約情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbLinkageUserMembershipItem : QsDbEntityBase
    {
        
        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        /// <remarks></remarks>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 共通IDを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageId { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>

        public byte MembershipType { get; set; } = byte.MinValue;


        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                this.AccountKey = reader.GetGuid(0);
                this.LinkageId = reader.GetString(1);
                this.MembershipType = reader.IsDBNull(2) ? (byte)QsDbMemberShipTypeEnum.Free:reader.GetByte(2);

                this.KeyGuid = Guid.NewGuid();
                this.DataState = QsDbEntityStateTypeEnum.Unchanged;
                this.IsEmpty = false;
            }
            catch (Exception)
            {

                throw;
            }

            return this;
        }

        public override bool IsKeysValid()
        {
            return true;
        }

        /// <summary>
        /// <see cref="DbLinkageUserMembershipItem" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbLinkageUserMembershipItem()
        {
        }

    }

}
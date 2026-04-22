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
    internal sealed class DbTermsItem : QsDbEntityBase
    {
        /// <summary>
        /// 利用規約番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public long TermsNo { get; set; } = long.MinValue;

        /// <summary>
        /// 利用規約の更新日を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime TermsUpdatedDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 内容を取得または設定します。
        /// </summary>
        public string Contents { get; set; } = string.Empty;

        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                this.TermsNo = reader.GetInt64(0);
                this.TermsUpdatedDate = reader.GetDateTime(1);
                this.Contents = reader.GetString(2);

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
        /// <see cref="DbTermsItem" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbTermsItem()
        {
        }
        
    }

}
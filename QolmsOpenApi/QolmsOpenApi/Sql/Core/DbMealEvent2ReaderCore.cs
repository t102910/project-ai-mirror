using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// 食事イベント情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbMealEvent2ReaderCore : QsDbReaderBase
    {
        #region "Private Property"
        /// <summary>
        /// アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _accountKey = Guid.Empty;
        #endregion

        #region "Public Property"
        #endregion

        #region "Constructor"
        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbMealEvent2ReaderCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbMealEvent2ReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <remarks></remarks>
        public DbMealEvent2ReaderCore(Guid accountKey) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }
        #endregion

        #region "Private Method"
        /// <summary>
        /// 食事イベント情報のリストを取得します。
        /// </summary>
        /// <param name="recordDate">記録日</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<QH_MEALEVENT2_DAT> ReadMealEventEntityList(DateTime recordDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEALEVENT2_DAT>())
            {
                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", this._accountKey),
                    this.CreateParameter(connection, "@P2", recordDate),
                };

                // クエリを作成
                StringBuilder query = new StringBuilder()
                    .Append(" select *")
                    .Append(" from qh_mealevent2_dat")
                    .Append(" where accountkey = @P1")
                    .Append(" and recorddate = @P2")
                    .Append(" and deleteflag = 0")
                    .Append(" order by recorddate desc, sequence desc, mealtype asc");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_MEALEVENT2_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }
        #endregion

        #region "Public Method"
        /// <summary>
        /// 食事イベント情報のリストを取得します。
        /// </summary>
        /// <param name="recordDate">記録日</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_MEALEVENT2_DAT> ReadMealEventList(DateTime recordDate)
        {
            return this.ReadMealEventEntityList(recordDate);
        }
        #endregion
    }
}

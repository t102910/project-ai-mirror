using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// イベント情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbEventReaderCore : QsDbReaderBase
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
        private DbEventReaderCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbEventReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <remarks></remarks>
        public DbEventReaderCore(Guid accountKey) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }
        #endregion

        #region "Private Method"
        /// <summary>
        /// イベント情報のリストを取得します。
        /// </summary>
        /// <param name="eventDate">イベント日</param>
        /// <param name="linkageSystemNo">連携システム番号</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<QH_EVENT_DAT> ReadMentalEventEntityList(DateTime eventDate, int linkageSystemNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_EVENT_DAT>())
            {
                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", this._accountKey),
                    this.CreateParameter(connection, "@P2", DateTime.ParseExact(eventDate.ToString("yyyyMMdd0000000000000"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None)),
                    this.CreateParameter(connection, "@P3", DateTime.ParseExact(eventDate.ToString("yyyyMMdd2359599999999"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None)),
                    this.CreateParameter(connection, "@P4", linkageSystemNo),
                    this.CreateParameter(connection, "@P5", nameof(QhQolmsDiaryEventSetOfJson)),
                };

                // クエリを作成
                StringBuilder query = new StringBuilder()
                    .Append(" select *")
                    .Append(" from qh_event_dat")
                    .Append(" where accountkey = @P1")
                    .Append(" and (eventdate between @P2 and @P3)")
                    .Append(" and linkagesystemno = @P4")
                    .Append(" and eventsettypename = @P5")
                    .Append(" and deleteflag = 0")
                    .Append(" order by eventdate desc, eventsequence desc");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_EVENT_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }
        #endregion

        #region "Public Method"
        /// <summary>
        /// メンタルイベント情報のリストを取得します。
        /// </summary>
        /// <param name="eventDate">イベント日</param>
        /// <param name="linkageSystemNo">連携システム番号</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_EVENT_DAT> ReadMentalEventList(DateTime eventDate, int linkageSystemNo)
        {
            return this.ReadMentalEventEntityList(eventDate, linkageSystemNo);
        }
        #endregion
    }
}
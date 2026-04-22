using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// バイタル情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbHealthRecordReaderCore : QsDbReaderBase
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
        private DbHealthRecordReaderCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbHealthRecordReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <remarks></remarks>
        public DbHealthRecordReaderCore(Guid accountKey) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }
        #endregion

        #region "Private Method"
        /// <summary>
        /// バイタル情報のリストを取得します。
        /// </summary>
        /// <param name="recordDate">測定日付</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<QH_HEALTHRECORD_DAT> ReadHealthRecordEntityList(DateTime recordDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", this._accountKey),
                    this.CreateParameter(connection, "@P2", DateTime.ParseExact(recordDate.ToString("yyyyMMdd0000000000000"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None)),
                    this.CreateParameter(connection, "@P3", DateTime.ParseExact(recordDate.ToString("yyyyMMdd2359599999999"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None))
                };

                // クエリを作成
                StringBuilder query = new StringBuilder()
                    .Append(" select *")
                    .Append(" from qh_healthrecord_dat")
                    .Append(" where accountkey = @P1")
                    .Append(" and (recorddate between @P2 and @P3)")
                    .Append(" and deleteflag = 0")
                    .Append(" order by vitaltype asc, recorddate desc");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_HEALTHRECORD_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// 血糖値情報の前回と最新を取得します。
        /// </summary>
        /// <param name="targetDate">対象日</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<QH_HEALTHRECORD_DAT> ReadPreviousAndLatestBloodSugarEntityList(DateTime targetDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", this._accountKey),
                    this.CreateParameter(connection, "@P2", (byte)QsDbVitalTypeEnum.BloodSugar),
                    this.CreateParameter(connection, "@P3", DateTime.ParseExact(targetDate.ToString("yyyyMMdd2359599999999"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None))
                };

                // クエリを作成
                StringBuilder query = new StringBuilder()
                    .Append(" select top (2) *")
                    .Append(" from qh_healthrecord_dat")
                    .Append(" where accountkey = @P1")
                    .Append(" and vitaltype = @P2")
                    .Append(" and recorddate <= @P3")
                    .Append(" and deleteflag = 0")
                    .Append(" order by recorddate desc");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_HEALTHRECORD_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// 身長情報の最新1件を取得します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private QH_HEALTHRECORD_DAT ReadLatestHeightEntity()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", this._accountKey),
                    this.CreateParameter(connection, "@P2", (byte)QsDbVitalTypeEnum.BodyHeight)
                };

                // クエリを作成
                StringBuilder query = new StringBuilder()
                    .Append(" select top (1) *")
                    .Append(" from qh_healthrecord_dat")
                    .Append(" where accountkey = @P1")
                    .Append(" and vitaltype = @P2")
                    .Append(" and deleteflag = 0")
                    .Append(" order by recorddate desc");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                List<QH_HEALTHRECORD_DAT> result = this.ExecuteReader<QH_HEALTHRECORD_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
                
                return (result != null && result.Count > 0) ? result[0] : null;
            }
        }
        #endregion

        #region "Public Method"
        /// <summary>
        /// バイタル情報のリストを取得します。
        /// </summary>
        /// <param name="recordDate">測定日付</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_HEALTHRECORD_DAT> ReadHealthRecordList(DateTime recordDate)
        {
            return this.ReadHealthRecordEntityList(recordDate);
        }

        /// <summary>
        /// 血糖値情報の前回と最新を取得します。
        /// </summary>
        /// <param name="targetDate">対象日</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_HEALTHRECORD_DAT> ReadPreviousAndLatestBloodSugarList(DateTime targetDate)
        {
            return this.ReadPreviousAndLatestBloodSugarEntityList(targetDate);
        }

        /// <summary>
        /// 身長情報の最新1件を取得します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public QH_HEALTHRECORD_DAT ReadLatestHeight()
        {
            return this.ReadLatestHeightEntity();
        }
        #endregion
    }
}

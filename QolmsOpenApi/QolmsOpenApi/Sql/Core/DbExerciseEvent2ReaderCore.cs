using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// 運動イベント情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbExerciseEvent2ReaderCore : QsDbReaderBase
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
        private DbExerciseEvent2ReaderCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbExerciseEvent2ReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <remarks></remarks>
        public DbExerciseEvent2ReaderCore(Guid accountKey) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }
        #endregion

        #region "Private Method"
        /// <summary>
        /// 運動イベント情報のリストを取得します。
        /// </summary>
        /// <param name="recordDate">記録日</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<QH_EXERCISEEVENT2_DAT> ReadExerciseEventEntityList(DateTime recordDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_EXERCISEEVENT2_DAT>())
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
                    .Append(" from qh_exerciseevent2_dat")
                    .Append(" where accountkey = @P1")
                    .Append(" and (recorddate between @P2 and @P3)")
                    .Append(" and deleteflag = 0")
                    .Append(" order by recorddate desc, sequence desc");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_EXERCISEEVENT2_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }
        #endregion

        #region "Public Method"
        /// <summary>
        /// 運動イベント情報のリストを取得します。
        /// </summary>
        /// <param name="recordDate">記録日</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_EXERCISEEVENT2_DAT> ReadExerciseEventList(DateTime recordDate)
        {
            return this.ReadExerciseEventEntityList(recordDate);
        }
        #endregion
    }
}

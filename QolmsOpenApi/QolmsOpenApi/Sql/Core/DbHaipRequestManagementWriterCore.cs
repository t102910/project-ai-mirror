using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// HAIPデータ取得依頼情報 を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbHaipRequestManagementWriterCore : QsDbWriterBase
    {
        /// <summary>
        /// RequestId を保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _requestId = Guid.Empty;

        /// <summary>
        /// ResponseStatus を保持します。
        /// </summary>
        /// <remarks></remarks>
        private string _responseStatus = string.Empty;

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbHaipRequestManagementWriterCore()
        {
        }

        /// <summary>
        /// RequestId、ResponseStatus を指定して、
        /// <see cref="DbHaipRequestManagementWriterCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="requestId">RequestId</param>
        /// <param name="responseStatus">ResponseStatus</param>
        /// <remarks></remarks>
        public DbHaipRequestManagementWriterCore(Guid requestId, string responseStatus) : base()
        {
            this._requestId = requestId;

            if (this._requestId == Guid.Empty)
                throw new ArgumentOutOfRangeException("RequestId", "RequestIdが不正です。");

            this._responseStatus = responseStatus;

            if (this._responseStatus == string.Empty)
                throw new ArgumentOutOfRangeException("ResponseStatus", "ResponseStatusが不正です。");

        }

        #region "Private Method"

        #endregion

        #region "Public Method"
        /// <summary>
        /// HAIPデータ取得依頼情報を更新します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool UpsertHaipRequestManagement()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_HAIPREQUESTMANAGEMENT_DAT>())
            {
                
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", this._requestId),
                    this.CreateParameter(connection, "@P2", this._responseStatus),
                    this.CreateParameter(connection, "@P3", DateTime.Now),

                };
                var query = new StringBuilder()
                    .Append("BEGIN ")
                    .Append(" IF (SELECT COUNT(*) FROM QH_HAIPREQUESTMANAGEMENT_DAT WHERE REQUESTID = @P1) <> 0 ")
                    .Append(" BEGIN ")
                    .Append("  UPDATE QH_HAIPREQUESTMANAGEMENT_DAT SET ")
                    .Append("  RESPONSESTATUS = @P2, PROCESSFLAG = 1, UPDATEDDATE = @P3 ")
                    .Append("  WHERE REQUESTID = @P1")
                    .Append(" END ")
                    .Append("END ");

                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params) == 1;
            }
        }
        #endregion
    }
}
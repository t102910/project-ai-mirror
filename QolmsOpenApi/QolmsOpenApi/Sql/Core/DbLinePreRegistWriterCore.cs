using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// Line事前登録情報 を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbLinePreRegistWriterCore : QsDbWriterBase
    {
        /// <summary>
        /// LineUserId を保持します。
        /// </summary>
        /// <remarks></remarks>
        private string _userId = string.Empty;

        /// <summary>
        /// LinkageSystemId を保持します。
        /// </summary>
        /// <remarks></remarks>
        private string _linkageSystemId = string.Empty;

        /// <summary>
        /// 生年月日 を保持します。
        /// </summary>
        /// <remarks></remarks>
        private DateTime _birthday = DateTime.MinValue;

        /// <summary>
        /// DeleteFlag を保持します。
        /// </summary>
        /// <remarks></remarks>
        private bool _deleteFlag = false;

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbLinePreRegistWriterCore()
        {
        }

        /// <summary>
        /// LineUserId、LinkageSystemId、生年月日 を指定して、
        /// <see cref="DbLinePreRegistWriterCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="userId">LineUserId</param>
        /// <param name="linkageSystemId">LinkageSystemId</param>
        /// <param name="birthday">生年月日</param>
        /// <remarks></remarks>
        public DbLinePreRegistWriterCore(string userId, string linkageSystemId, DateTime birthday, bool deleteFlag) : base()
        {
            this._userId = userId;

            if (this._userId == string.Empty)
                throw new ArgumentOutOfRangeException("userId", "LineUserIdが不正です。");

            this._linkageSystemId = linkageSystemId;

            //if (this._linkageSystemId == string.Empty)
            //    throw new ArgumentOutOfRangeException("linkageSystemId", "linkageSystemIdが不正です。");

            this._birthday = birthday;

            //if (this._birthday == DateTime.MinValue)
            //    throw new ArgumentOutOfRangeException("birthday", "生年月日が不正です。");

            this._deleteFlag = deleteFlag; 
        }

        #region "Private Method"

        #endregion

        #region "Public Method"
        /// <summary>
        /// Line事前登録情報を更新します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool UpsertLinePreRegist()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINEPREREGIST_DAT>())
            {
                
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", this._userId),
                    this.CreateParameter(connection, "@P2", this._linkageSystemId),
                    this.CreateParameter(connection, "@P3", this._birthday),
                    this.CreateParameter(connection, "@P4", DateTime.Now),
                    this.CreateParameter(connection, "@P5", this._deleteFlag),

                };
                var query = new StringBuilder()
                    .Append("BEGIN")
                    .Append(" IF (SELECT COUNT(*) FROM QH_LINEPREREGIST_DAT WHERE USERID = @P1) <> 0")
                    .Append(" BEGIN")
                    .Append("  UPDATE QH_LINEPREREGIST_DAT SET")
                    .Append("  UPDATEDDATE = @P4, DELETEFLAG = @P5");
                if (!string.IsNullOrWhiteSpace(_linkageSystemId))
                {
                    query.Append("  ,LINKAGESYSTEMID = @P2");
                }
                if (this._birthday != DateTime.MinValue)
                {
                    query.Append("  ,BIRTHDAY = @P3");
                }
                query.Append("  WHERE USERID = @P1");
                query.Append(" END");
                query.Append(" ELSE");
                query.Append(" BEGIN");
                query.Append("  INSERT INTO QH_LINEPREREGIST_DAT");
                query.Append("  (USERID, LINKAGESYSTEMID, BIRTHDAY, DELETEFLAG, CREATEDDATE, UPDATEDDATE)");
                query.Append("  VALUES");
                query.Append("  (@P1, @P2, @P3, 0, @P4, @P4)");
                query.Append(" END ");
                query.Append("END");

                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params) == 1;
            }
        }
        #endregion
    }
}
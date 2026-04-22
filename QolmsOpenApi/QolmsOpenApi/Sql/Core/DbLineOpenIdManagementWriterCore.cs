using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// OpenIdManagement情報 を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbLineOpenIdManagementWriterCore : QsDbWriterBase
    {
        /// <summary>
        /// LineUserId を保持します。
        /// </summary>
        /// <remarks></remarks>
        private string _userId = string.Empty;

        /// <summary>
        /// LinkageSystemNo を保持します。
        /// </summary>
        /// <remarks></remarks>
        private int _linkageSystemNo = int.MinValue;

        /// <summary>
        /// DeleteFlag を保持します。
        /// </summary>
        /// <remarks></remarks>
        private bool _deleteFlag = false;

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbLineOpenIdManagementWriterCore()
        {
        }

        /// <summary>
        /// LineUserId、LinkageSystemNo を指定して、
        /// <see cref="DbLinePreRegistWriterCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="userId">LineUserId</param>
        /// <param name="linkageSystemNo">LinkageSystemNo</param>
        /// <remarks></remarks>
        public DbLineOpenIdManagementWriterCore(string userId, int linkageSystemNo, bool deleteFlag) : base()
        {
            this._userId = userId;

            if (this._userId == string.Empty)
                throw new ArgumentOutOfRangeException("userId", "LineUserIdが不正です。");

            this._linkageSystemNo = linkageSystemNo;

            if (this._linkageSystemNo == int.MinValue)
                throw new ArgumentOutOfRangeException("linkageSystemNo", "LinkageSystemNoが不正です。");

            this._deleteFlag = deleteFlag;
        }

        #region "Private Method"

        #endregion

        #region "Public Method"
        /// <summary>
        /// OpenIdManagement情報を登録します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool InsertLineOpenIdManagement()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LOGINMANAGEMENT_DAT>())
            {
                
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", this._userId),
                    this.CreateParameter(connection, "@P2", this._linkageSystemNo),
                    this.CreateParameter(connection, "@P3", (byte)QsDbOpenIdTypeEnum.LineID),
                    this.CreateParameter(connection, "@P4", string.Empty),
                    this.CreateParameter(connection, "@P5", DateTime.Now),
                    this.CreateParameter(connection, "@P6", this._deleteFlag),
                };
                var query = new StringBuilder()
                    .Append(" BEGIN")
                    .Append("  IF (SELECT COUNT(*) FROM QH_OPENIDMANAGEMENT_DAT WHERE OPENID = @P1 AND IDTYPE = @P3 AND ACCOUNTKEY = ")
                    .Append("   (SELECT T1.ACCOUNTKEY")
                    .Append("    FROM QH_ACCOUNTINDEX_DAT AS T1, QH_LINKAGE_DAT AS T2, QH_LINEPREREGIST_DAT AS T3")
                    .Append("    WHERE T1.ACCOUNTKEY = T2.ACCOUNTKEY")
                    .Append("    AND T1.BIRTHDAY = T3.BIRTHDAY")
                    .Append("    AND T2.LINKAGESYSTEMID = T3.LINKAGESYSTEMID")
                    .Append("    AND T2.LINKAGESYSTEMNO = @P2")
                    .Append("    AND T3.USERID = @P1")
                    .Append("    AND T1.DELETEFLAG = 0")
                    .Append("    AND T2.DELETEFLAG = 0")
                    .Append("    AND T3.DELETEFLAG = 0)")
                    .Append("  ) = 0")
                    .Append("  BEGIN")
                    .Append("   INSERT INTO QH_OPENIDMANAGEMENT_DAT")
                    .Append("   (OPENID, IDTYPE, ACCOUNTKEY, MAILADDRESS, DELETEFLAG, CREATEDDATE, UPDATEDDATE)")
                    .Append("   VALUES")
                    .Append("   (@P1, @P3, ")
                    .Append("   (SELECT T1.ACCOUNTKEY")
                    .Append("    FROM QH_ACCOUNTINDEX_DAT AS T1, QH_LINKAGE_DAT AS T2, QH_LINEPREREGIST_DAT AS T3")
                    .Append("    WHERE T1.ACCOUNTKEY = T2.ACCOUNTKEY")
                    .Append("    AND T1.BIRTHDAY = T3.BIRTHDAY")
                    .Append("    AND T2.LINKAGESYSTEMID = T3.LINKAGESYSTEMID")
                    .Append("    AND T2.LINKAGESYSTEMNO = @P2")
                    .Append("    AND T3.USERID = @P1")
                    .Append("    AND T1.DELETEFLAG = 0")
                    .Append("    AND T2.DELETEFLAG = 0")
                    .Append("    AND T3.DELETEFLAG = 0),")
                    .Append("    @P4, 0, @P5, @P5)")
                    .Append("  END")
                    .Append("  ELSE")
                    .Append("  BEGIN")
                    .Append("   UPDATE QH_OPENIDMANAGEMENT_DAT SET")
                    .Append("   DELETEFLAG = @P6, UPDATEDDATE = @P5")
                    .Append("   WHERE OPENID = @P1 AND IDTYPE = @P3 AND ACCOUNTKEY = ")
                    .Append("   (SELECT T1.ACCOUNTKEY")
                    .Append("    FROM QH_ACCOUNTINDEX_DAT AS T1, QH_LINKAGE_DAT AS T2, QH_LINEPREREGIST_DAT AS T3")
                    .Append("    WHERE T1.ACCOUNTKEY = T2.ACCOUNTKEY")
                    .Append("    AND T1.BIRTHDAY = T3.BIRTHDAY")
                    .Append("    AND T2.LINKAGESYSTEMID = T3.LINKAGESYSTEMID")
                    .Append("    AND T2.LINKAGESYSTEMNO = @P2")
                    .Append("    AND T3.USERID = @P1")
                    .Append("    AND T1.DELETEFLAG = 0")
                    .Append("    AND T2.DELETEFLAG = 0")
                    .Append("    AND T3.DELETEFLAG = 0)")
                    .Append("  END")
                    .Append(" END");
                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params) == 1;
            }
        }
        #endregion
    }
}
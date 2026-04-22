using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.JAHISMedicineEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// ログイン日時 を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbAccountLogInManagementWriterCore : QsDbWriterBase
    {
        /// <summary>
        /// アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _accountKey = Guid.Empty;

        /// <summary>
        /// 実行日時を保持します。
        /// </summary>
        /// <remarks></remarks>
        private DateTime _actionDate = DateTime.MinValue;

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbAccountLogInManagementWriterCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbAccountLogInManagementWriterCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">対象者アカウントキー</param>
        /// <param name="actionDate">実行日時</param>
        /// <remarks></remarks>
        public DbAccountLogInManagementWriterCore(Guid accountKey, DateTime actionDate) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");

            this._actionDate = actionDate;

            if (this._actionDate == DateTime.MinValue)
                throw new ArgumentOutOfRangeException("actionDate", "実行日時が不正です。");
        }

        #region "Private Method"

        #endregion

        #region "Public Method"
        /// <summary>
        /// ログインマネジメントデータを更新します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool UpdateAccountLogInManagement()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LOGINMANAGEMENT_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", this._accountKey),
                    this.CreateParameter(connection, "@P2", this._actionDate),
                };

                query.Append(" UPDATE QH_LOGINMANAGEMENT_DAT");
                query.Append(" SET LASTLOGINDATE = @P2, UPDATEDDATE = @P2 ");
                query.Append(" WHERE ACCOUNTKEY = @P1 AND DELETEFLAG = 0;");

                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params) == 1;
            }
        }
        #endregion
    }
}
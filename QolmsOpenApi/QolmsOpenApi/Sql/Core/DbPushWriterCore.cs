using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// ReproPush通知登録情報 を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbPushWriterCore : QsDbWriterBase
    {
        /// <summary>
        /// QH_PUSHSEND_LOGエンティティ を保持します。
        /// </summary>
        /// <remarks></remarks>
        private List<QH_PUSHSEND_LOG> _entities = new List<QH_PUSHSEND_LOG>();

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbPushWriterCore()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public DbPushWriterCore(List<QH_PUSHSEND_LOG> entities) : base()
        {
            this._entities = entities;

            if (this._entities.Count <= 0)
                throw new ArgumentOutOfRangeException("entities", "登録対象エンティティが不正です。");

        }

        #region "Private Method"

        #endregion

        #region "Public Method"

        /// <summary>
        /// Push通知実行情報を登録します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool InsertPushSendLog()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PUSHSEND_LOG>())
            {

                int retCount = 0;

                foreach (QH_PUSHSEND_LOG entity in this._entities)
                {
                    // パラメータ を準備
                    var paramN = new List<DbParameter>()
                    {
                        this.CreateParameter(connection, "@P1", entity.ACCOUNTKEY),
                        this.CreateParameter(connection, "@P2", entity.PUSHDATE),
                        this.CreateParameter(connection, "@P3", entity.PUSHTYPE),
                        this.CreateParameter(connection, "@P4", entity.CALLERSYSTEMNAME),
                        this.CreateParameter(connection, "@P5", entity.FOREIGNKEY)

                    };

                    // クエリ を作成
                    var query = new StringBuilder()
                        .Append("INSERT INTO QH_PUSHSEND_LOG")
                        .Append(" (ACCOUNTKEY")
                        .Append(" ,PUSHDATE")
                        .Append(" ,PUSHTYPE")
                        .Append(" ,CALLERSYSTEMNAME")
                        .Append(" ,FOREIGNKEY)")
                        .Append(" VALUES")
                        .Append(" (@P1")
                        .Append(" ,@P2")
                        .Append(" ,@P3")
                        .Append(" ,@P4")
                        .Append(" ,@P5)");

                    connection.Open();
                    // クエリ を実行
                    retCount += this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), paramN);
                }

                return retCount == _entities.Count;
            }
        }
        #endregion
    }
}
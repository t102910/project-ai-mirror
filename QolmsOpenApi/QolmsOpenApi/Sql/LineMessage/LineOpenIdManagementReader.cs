using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// LineOpenId を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class LineOpenIdManagementReader : QsDbReaderBase, IQsDbDistributedReader<QH_OPENIDMANAGEMENT_DAT, LineOpenIdManagementReaderArgs, LineOpenIdManagementReaderResults>
    {
        /// <summary>
        /// <see cref="LineOpenIdManagementReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LineOpenIdManagementReader() : base()
        {
        }


        /// <summary>
        /// linkageSystemNo と、lineUserId から QH_OPENIDMANAGEMENT_DAT を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        private List<QH_OPENIDMANAGEMENT_DAT> SelectLineOpenId(int linkageSystemNo, string lineUserId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_OPENIDMANAGEMENT_DAT>())
            {

                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", (byte)QsDbOpenIdTypeEnum.LineID),
                    this.CreateParameter(connection, "@P2", linkageSystemNo),
                    this.CreateParameter(connection, "@P3", lineUserId),
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                    .Append(" SELECT * FROM QH_OPENIDMANAGEMENT_DAT WHERE DELETEFLAG = 0 AND OPENID = @P3 AND IDTYPE = @P1 AND ACCOUNTKEY = ")
                    .Append("  (SELECT T1.ACCOUNTKEY")
                    .Append("   FROM QH_ACCOUNTINDEX_DAT AS T1, QH_LINKAGE_DAT AS T2, QH_LINEPREREGIST_DAT AS T3")
                    .Append("   WHERE T1.ACCOUNTKEY = T2.ACCOUNTKEY")
                    .Append("   AND T1.BIRTHDAY = T3.BIRTHDAY")
                    .Append("   AND T2.LINKAGESYSTEMID = T3.LINKAGESYSTEMID")
                    .Append("   AND T2.LINKAGESYSTEMNO = @P2")
                    .Append("   AND T3.USERID = @P3")
                    .Append("   AND T1.DELETEFLAG = 0")
                    .Append("   AND T2.DELETEFLAG = 0")
                    .Append("   AND T3.DELETEFLAG = 0)");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_OPENIDMANAGEMENT_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        
        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public LineOpenIdManagementReaderResults ExecuteByDistributed(LineOpenIdManagementReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LineOpenIdManagementReaderResults result = new LineOpenIdManagementReaderResults() { IsSuccess = false };
            
            result.Result = this.SelectLineOpenId(args.LinkageSystemNo, args.LineUserId);
            
            if(result.Result !=null && result.Result.Count == 1)
                result.IsSuccess = true;
            return result;
        }
    }


}

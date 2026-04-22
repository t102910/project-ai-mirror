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
    /// LineUserId を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class LineUserIdReader : QsDbReaderBase, IQsDbDistributedReader<QH_OPENIDMANAGEMENT_DAT, LineUserIdReaderArgs, LineUserIdReaderResults>
    {
        /// <summary>
        /// <see cref="LineUserIdReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LineUserIdReader() : base()
        {
        }


        /// <summary>
        /// linkageSystemNo と、linkageSystemId から QH_OPENIDMANAGEMENT_DAT を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        private List<QH_OPENIDMANAGEMENT_DAT> SelectLineUserId(int linkageSystemNo, string linkageSystemId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_OPENIDMANAGEMENT_DAT>())
            {

                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", (byte)QsDbOpenIdTypeEnum.LineID),
                    this.CreateParameter(connection, "@P2", linkageSystemNo),
                    this.CreateParameter(connection, "@P3", linkageSystemId),
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                .Append(" SELECT T1.*")
                .Append(" FROM   QH_OPENIDMANAGEMENT_DAT AS T1, QH_LINKAGE_DAT AS T2")
                .Append(" WHERE  T1.IDTYPE = @P1")
                .Append(" AND    T1.ACCOUNTKEY = T2.ACCOUNTKEY")
                .Append(" AND    T1.DELETEFLAG = 0")
                .Append(" AND    T2.LINKAGESYSTEMNO = @P2")
                .Append(" AND    T2.DELETEFLAG = 0");

                if (!string.IsNullOrWhiteSpace(linkageSystemId))
                {
                    query.Append(" AND    T2.LINKAGESYSTEMID = @P3");
                }



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
        public LineUserIdReaderResults ExecuteByDistributed(LineUserIdReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LineUserIdReaderResults result = new LineUserIdReaderResults() { IsSuccess = false };
            
            result.Result = this.SelectLineUserId(args.LinkageSystemNo, args.LinkageSystemId);
            
            if(result.Result != null && result.Result.Count > 0)
                result.IsSuccess = true;
            return result;
        }
    }


}

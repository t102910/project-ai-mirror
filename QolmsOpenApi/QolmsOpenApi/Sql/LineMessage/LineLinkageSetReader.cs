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
    /// Line関連の QH_LINKAGESYSTEM_MST の内容を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class LineLinkageSetReader : QsDbReaderBase, IQsDbDistributedReader<QH_LINKAGESYSTEM_MST, LineLinkageSetReaderArgs, LineLinkageSetReaderResults>
    {
        /// <summary>
        /// <see cref="LineLinkageSetReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LineLinkageSetReader() : base()
        {
        }


        /// <summary>
        /// linkageSystemNoまたは、xLineSignature(LINKAGESET) から QH_LINKAGESYSTEM_MST を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        private List<QH_LINKAGESYSTEM_MST> SelectLinkageSystemMst(int linkageSystemNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGESYSTEM_MST>())
            {
                StringBuilder query = new StringBuilder();

                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", "LINEMessageAPI"),
                };

                // クエリを作成                
                query.Append(" SELECT *");
                query.Append(" FROM QH_LINKAGESYSTEM_MST");
                query.Append(" WHERE DELETEFLAG = 0");
                query.Append(" AND LINKAGENOTE = @P2");
                query.Append(" AND LINKAGESYSTEMNO = @P1");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_LINKAGESYSTEM_MST>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public LineLinkageSetReaderResults ExecuteByDistributed(LineLinkageSetReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LineLinkageSetReaderResults result = new LineLinkageSetReaderResults() { IsSuccess = false };
            
            result.Result = this.SelectLinkageSystemMst(args.LinkageSystemNo);
            
            if(result.Result !=null && result.Result.Count == 1)
                result.IsSuccess = true;
            return result;
        }
    }


}

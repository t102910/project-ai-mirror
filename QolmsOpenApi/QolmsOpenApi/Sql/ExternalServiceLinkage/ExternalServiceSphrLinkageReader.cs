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
    /// SPHR連携対象者情報 を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class ExternalServiceSphrLinkageReader : QsDbReaderBase, IQsDbDistributedReader<QH_LINKAGE_DAT, ExternalServiceSphrLinkageReaderArgs, ExternalServiceSphrLinkageReaderResults>
    {
        /// <summary>
        /// <see cref="ExternalServiceSphrLinkageReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExternalServiceSphrLinkageReader() : base()
        {
        }


        /// <summary>
        /// LinkageSystemNo から QH_LINKAGE_DAT を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="communitySystemNo"></param>
        /// <returns></returns>
        private List<QH_LINKAGE_DAT> SelectExternalServiceSphrLinkage(Guid accountKey, int linkageSystemNo, int communitySystemNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {

                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", communitySystemNo),
                    this.CreateParameter(connection, "@P3", accountKey),
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                .Append(" SELECT T1.ACCOUNTKEY, T1.LINKAGESYSTEMNO, T1.LINKAGESYSTEMID,")
                .Append("        T1.DATASET, T1.STATUSTYPE, T2.DELETEFLAG, T1.CREATEDDATE, T1.UPDATEDDATE")
                .Append(" FROM QH_LINKAGE_DAT AS T1, QH_LINKAGE_DAT AS T2")
                .Append(" WHERE T1.ACCOUNTKEY = T2.ACCOUNTKEY")
                .Append(" AND T1.LINKAGESYSTEMNO = @P1")
                .Append(" AND T1.DELETEFLAG = 0")
                .Append(" AND T2.LINKAGESYSTEMNO = @P2");

                if (accountKey != Guid.Empty)
                {
                    query.Append(" AND T1.ACCOUNTKEY = @P3");
                }

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_LINKAGE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public ExternalServiceSphrLinkageReaderResults ExecuteByDistributed(ExternalServiceSphrLinkageReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            ExternalServiceSphrLinkageReaderResults result = new ExternalServiceSphrLinkageReaderResults() { IsSuccess = false };
            
            result.Result = this.SelectExternalServiceSphrLinkage(args.AccountKey, args.LinkageSystemNo, args.CommunitySystemNo);
            
            if(result.Result != null)
                result.IsSuccess = true;
            return result;
        }
    }


}

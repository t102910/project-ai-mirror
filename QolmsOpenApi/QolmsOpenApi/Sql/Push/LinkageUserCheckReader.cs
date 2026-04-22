using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;
using MGF.QOLMS.QolmsCryptV1;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class LinkageUserCheckReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageUserCheckReaderArgs, LinkageUserCheckReaderResults>
    {

        #region "Private Method"

        private List<QH_LINKAGE_DAT> GetAccountKeys(int linkageSystemNo, string linkageIds)
        {

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                
                var param = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                };
                var query = new StringBuilder()
                    .Append($" SELECT *")
                    .Append($" FROM QH_LINKAGE_DAT")
                    .Append($" WHERE DELETEFLAG = 0")
                    .Append($" AND LINKAGESYSTEMNO = @P1")
                    .Append($" AND LINKAGESYSTEMID IN ({linkageIds})");

                //コネクションオープン
                connection.Open();

                var resultList = this.ExecuteReader<QH_LINKAGE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), param);
                return resultList;
            }
        }

        #endregion


        #region "Public Method"

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public LinkageUserCheckReaderResults ExecuteByDistributed(LinkageUserCheckReaderArgs args)
        {

            LinkageUserCheckReaderResults result = new LinkageUserCheckReaderResults() { IsSuccess = false };
            
            var entities = GetAccountKeys(args.LinkageSystemNo, args.LinkageIds);

            result.AccountKeys = new List<Guid>();

            foreach (var entity in entities) result.AccountKeys.Add(entity.ACCOUNTKEY);

            if (result.AccountKeys != null && result.AccountKeys.Count > 0)
            {
                result.IsSuccess = true;
            }

            return result;
        }

        #endregion
    }
}

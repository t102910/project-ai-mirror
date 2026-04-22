using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 施設画像の内容を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class FacilityFileReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, FacilityFileReaderArgs, FacilityFileReaderResults>
    {
        /// <summary>
        /// <see cref="FacilityFileReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityFileReader() : base()
        {
        }



        private List<QH_FACILITYFILE_MST> SelectFacilityFileMst(Guid facilityKey)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_FACILITYFILE_MST>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() { this.CreateParameter(connection, "@p1", facilityKey) };

                // クエリを作成                
                query.Append("select * from qh_facilityfile_mst");
                query.Append(" where facilitykey = @p1");
                query.Append(" and deleteflag = 0");
                query.Append(";");
               
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_FACILITYFILE_MST>(connection, null, query.ToString(), @params);
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
        public FacilityFileReaderResults ExecuteByDistributed(FacilityFileReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            FacilityFileReaderResults result = new FacilityFileReaderResults() { IsSuccess = false };

            result.FileKeyN = this.SelectFacilityFileMst(args.FacilityKey).ConvertAll(m =>
            {
                return m.FILEKEY;
            }    );
            result.IsSuccess = true;
            return result;
        }
    }


}

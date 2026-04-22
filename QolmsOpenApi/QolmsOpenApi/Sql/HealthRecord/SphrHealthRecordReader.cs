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
    /// SPHR用のバイタル情報 を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class SphrHealthRecordReader : QsDbReaderBase, IQsDbDistributedReader<QH_HEALTHRECORD_DAT, SphrHealthRecordReaderArgs, SphrHealthRecordReaderResults>
    {
        /// <summary>
        /// <see cref="SphrHealthRecordReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public SphrHealthRecordReader() : base()
        {
        }


        /// <summary>
        /// LinkageSystemNo、LinkageSystemId、取得対象バイタルタイプ から QH_HEALTHRECORD_DAT を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        private List<QH_HEALTHRECORD_DAT> SelectSphrHealthRecord(string linkageSystemNo, string linkageSystemId, string vitalTypes)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {

                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", linkageSystemId),
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                .Append(" SELECT T2.*")
                .Append(" FROM QH_LINKAGE_DAT AS T1, QH_HEALTHRECORD_DAT AS T2")
                .Append(" WHERE T1.ACCOUNTKEY = T2.ACCOUNTKEY")
                .Append(" AND T1.LINKAGESYSTEMNO = @P1")
                .Append(" AND T1.LINKAGESYSTEMID = @P2")
                .Append(" AND T2.VITALTYPE IN(")
                .Append(vitalTypes)
                .Append(" )")
                .Append(" ORDER BY T2.VITALTYPE, T2.RECORDDATE");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_HEALTHRECORD_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public SphrHealthRecordReaderResults ExecuteByDistributed(SphrHealthRecordReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            SphrHealthRecordReaderResults result = new SphrHealthRecordReaderResults() { IsSuccess = false };
            
            result.Result = this.SelectSphrHealthRecord(args.LinkageSystemNo, args.LinkageSystemId, args.VitalTypes);
            
            if(result.Result != null)
                result.IsSuccess = true;
            return result;
        }
    }


}

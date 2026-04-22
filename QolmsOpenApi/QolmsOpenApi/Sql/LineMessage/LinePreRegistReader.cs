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
    /// LinePreRegist を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class LinePreRegistReader : QsDbReaderBase, IQsDbDistributedReader<QH_LINEPREREGIST_DAT, LinePreRegistReaderArgs, LinePreRegistReaderResults>
    {
        /// <summary>
        /// <see cref="LinePreRegistReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinePreRegistReader() : base()
        {
        }


        /// <summary>
        /// lineUserId から QH_LINEPREREGIST_DAT を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        private List<QH_LINEPREREGIST_DAT> SelectLinePreRegist(string lineUserId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINEPREREGIST_DAT>())
            {

                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", lineUserId),
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                .Append(" SELECT *")
                .Append(" FROM   QH_LINEPREREGIST_DAT")
                .Append(" WHERE  USERID = @P1");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_LINEPREREGIST_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public LinePreRegistReaderResults ExecuteByDistributed(LinePreRegistReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinePreRegistReaderResults result = new LinePreRegistReaderResults() { IsSuccess = false };
            
            result.Result = this.SelectLinePreRegist(args.LineUserId);
            
            if(result.Result !=null && result.Result.Count == 1)
                result.IsSuccess = true;
            return result;
        }
    }


}

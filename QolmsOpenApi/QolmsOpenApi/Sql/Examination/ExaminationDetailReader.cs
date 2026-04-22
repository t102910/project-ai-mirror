using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// 施設の内容を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class ExaminationDetailReader : QsDbReaderBase, IQsDbDistributedReader<QH_EXAMINATION_DAT, ExaminationDetailReaderArgs, ExaminationDetailReaderResults>
    {
        /// <summary>
        /// <see cref="ExaminationDetailReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExaminationDetailReader() : base()
        {
        }


        /// <summary>
        /// アカウントキー、検査日、施設キーから検査結果情報を返す。
        /// </summary>
        /// <param name="medicalFacilityCode"></param>
        /// <returns></returns>
        private List<QH_EXAMINATION_DAT> SelectExaminationDat(Guid accountKey, DateTime recordDate, Guid facilityKey)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_EXAMINATION_DAT>())
            {
                StringBuilder query = new StringBuilder();
                // パラメータ設定
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", accountKey),
                    this.CreateParameter(connection, "@P2", recordDate),
                    this.CreateParameter(connection, "@P3", facilityKey),
                    this.CreateParameter(connection, "@P4", false)};

                // クエリを作成                
                query.Append("SELECT ACCOUNTKEY");
                query.Append(" ,RECORDDATE");
                query.Append(" ,SEQUENCE");
                query.Append(" ,ORDERNO");
                query.Append(" ,DATATYPE");
                query.Append(" ,OWNERTYPE");
                query.Append(" ,LINKAGESYSTEMNO");
                query.Append(" ,FACILITYKEY");
                query.Append(" ,CATEGORYID");
                query.Append(" ,DATASET");
                query.Append(" ,CONVERTEDDATASET");
                query.Append(" ,COMMENTSET");
                query.Append(" ,DELETEFLAG");
                query.Append(" ,CREATEDDATE");
                query.Append(" ,UPDATEDDATE");
                query.Append(" FROM QH_EXAMINATION_DAT");
                query.Append(" WHERE DELETEFLAG = @P4");
                query.Append(" AND ACCOUNTKEY = @P1");
                query.Append(" AND RECORDDATE = @P2");
                query.Append(" AND FACILITYKEY = @P3");
                query.Append(" ORDER BY SEQUENCE");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_EXAMINATION_DAT>(connection, null, query.ToString(), @params);
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
        public ExaminationDetailReaderResults ExecuteByDistributed(ExaminationDetailReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            ExaminationDetailReaderResults result = new ExaminationDetailReaderResults() { IsSuccess = false };
            
            result.Result = this.SelectExaminationDat(args.AccountKey, args.RecordDate, args.FacilityKey);
            
            if(result.Result !=null && result.Result.Count >= 0)
                result.IsSuccess = true;
            return result;
        }
    }


}

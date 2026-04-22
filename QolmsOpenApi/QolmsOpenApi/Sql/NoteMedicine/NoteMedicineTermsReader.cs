using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ワンタイムコードから利用者の情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class NoteMedicineTermsReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, NoteMedicineTermsReaderArgs, NoteMedicineTermsReaderResults>
    {
        /// <summary>
        /// <see cref="NoteMedicineTermsReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineTermsReader() : base()
        {
        }

        #region "Private Method"

        private DbTermsItem ReadTerms(DateTime termsDate, byte systemType)
        {
            using(DbConnection connection = QsDbManager.CreateDbConnection<QH_TERMS_DAT>())
            {
                var query = new StringBuilder();
                var @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection,"@p1",DateTime.ParseExact(termsDate.ToString("yyyyMMdd0000000000000"),"yyyyMMddHHmmssfffffff",null,System.Globalization.DateTimeStyles.None)),
                    this.CreateParameter(connection,"@p2",systemType)
                };

                // クエリ生成
                query.Append("select top 1");
                query.Append(" termsno, termsupdateddate, contents");
                query.Append(" from qh_terms_dat");
                query.Append(" where deleteflag = 0 and systemType=@p2 ");
                query.Append(" and termsupdateddate <= @p1");
                query.Append(" order by termsupdateddate desc");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                return this.ExecuteReader<DbTermsItem>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).FirstOrDefault();
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
        public NoteMedicineTermsReaderResults ExecuteByDistributed(NoteMedicineTermsReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new NoteMedicineTermsReaderResults() { IsSuccess = false };

            DbTermsItem termsItem = this.ReadTerms(DateTime.Now, args.SystemType);

            if(termsItem != null)
            {
                result.TermsItem = termsItem;
                result.IsSuccess = true;
            }           

            return result;

        }
        #endregion
    }

}
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using System.Linq;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 健診アップロードファイルシーケンス情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class CheckupFileSequenceReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, CheckupFileSequenceReaderArgs, CheckupFileSequenceReaderResults>
    {

        /// <summary>
        /// <see cref="CheckupFileSequenceReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupFileSequenceReader() : base()
        {
        }


        /// <summary>
        /// 健診アップロードファイルシーケンス情報を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="waitingNumber"></param>
        /// <param name="reserveFlag"></param>
        /// <param name="orderDate"></param>
        /// <returns></returns>
        private List<QH_CHECKUPFRAILTYFILE_DAT> SelectFrailtySequence(int linkageSystemNo, string linkageSystemId, string serviceCode, DateTime effectiveDate, string organizationId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_CHECKUPFRAILTYFILE_DAT>())
            {

                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", linkageSystemId),
                    this.CreateParameter(connection, "@P3", serviceCode),
                    this.CreateParameter(connection, "@P4", effectiveDate),
                    this.CreateParameter(connection, "@P5", organizationId)
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                    .Append(" SELECT *")
                    .Append(" FROM QH_CHECKUPFRAILTYFILE_DAT AS T1")
                    .Append(" WHERE T1.LINKAGESYSTEMNO = @P1")
                    .Append(" AND T1.LINKAGESYSTEMID = @P2")
                    .Append(" AND T1.SERVICECODE = @P3")
                    .Append(" AND T1.EFFECTIVEDATE = @P4")
                    .Append(" AND T1.ORGANIZATIONID = @P5")
                    .Append(" ORDER BY T1.SEQUENCE DESC");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_CHECKUPFRAILTYFILE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// 健診アップロードファイルシーケンス情報を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="waitingNumber"></param>
        /// <param name="reserveFlag"></param>
        /// <param name="orderDate"></param>
        /// <returns></returns>
        private List<QH_CHECKUPCAREFILE_DAT> SelectCareSequence(int linkageSystemNo, string linkageSystemId, string serviceCode, DateTime effectiveDate, string organizationId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_CHECKUPFRAILTYFILE_DAT>())
            {

                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", linkageSystemId),
                    this.CreateParameter(connection, "@P3", serviceCode),
                    this.CreateParameter(connection, "@P4", effectiveDate),
                    this.CreateParameter(connection, "@P5", organizationId)
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                    .Append(" SELECT *")
                    .Append(" FROM QH_CHECKUPCAREFILE_DAT AS T1")
                    .Append(" WHERE T1.LINKAGESYSTEMNO = @P1")
                    .Append(" AND T1.LINKAGESYSTEMID = @P2")
                    .Append(" AND T1.SERVICECODE = @P3")
                    .Append(" AND T1.EFFECTIVEDATE = @P4")
                    .Append(" AND T1.ORGANIZATIONID = @P5")
                    .Append(" ORDER BY T1.SEQUENCE DESC");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_CHECKUPCAREFILE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// 健診アップロードファイルシーケンス情報を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="waitingNumber"></param>
        /// <param name="reserveFlag"></param>
        /// <param name="orderDate"></param>
        /// <returns></returns>
        private List<QH_CHECKUPAFTERSCHOOLFILE_DAT> SelectAfterSchoolSequence(int linkageSystemNo, string linkageSystemId, string serviceCode, DateTime effectiveDate, string organizationId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_CHECKUPAFTERSCHOOLFILE_DAT>())
            {

                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", linkageSystemId),
                    this.CreateParameter(connection, "@P3", serviceCode),
                    this.CreateParameter(connection, "@P4", effectiveDate),
                    this.CreateParameter(connection, "@P5", organizationId)
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                    .Append(" SELECT *")
                    .Append(" FROM QH_CHECKUPAFTERSCHOOLFILE_DAT AS T1")
                    .Append(" WHERE T1.LINKAGESYSTEMNO = @P1")
                    .Append(" AND T1.LINKAGESYSTEMID = @P2")
                    .Append(" AND T1.SERVICECODE = @P3")
                    .Append(" AND T1.EFFECTIVEDATE = @P4")
                    .Append(" AND T1.ORGANIZATIONID = @P5")
                    .Append(" ORDER BY T1.SEQUENCE DESC");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_CHECKUPAFTERSCHOOLFILE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public CheckupFileSequenceReaderResults ExecuteByDistributed(CheckupFileSequenceReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new CheckupFileSequenceReaderResults() { IsSuccess = false };
            QoAccessLog.WriteErrorLog($"{args.LinkageSystemNo},{args.LinkageSystemId},{args.ServiceCode},{args.EffectiveDate},{args.OrganizationId}", Guid.Empty);
            switch (args.SystemType)
            {
                case QsApiSystemTypeEnum.Frailty:
                    var fret = this.SelectFrailtySequence(args.LinkageSystemNo, args.LinkageSystemId, args.ServiceCode, args.EffectiveDate, args.OrganizationId);
                    if (fret != null && fret.Count > 0)
                    {
                        QoAccessLog.WriteErrorLog($"あり", Guid.Empty);

                        result.Sequence = fret.First().SEQUENCE;
                        result.IsSuccess = true;
                        return result;
                    }
                    result.Sequence = -1;
                    result.IsSuccess = true;
                    QoAccessLog.WriteErrorLog($"無し", Guid.Empty);

                    break;
                case QsApiSystemTypeEnum.Care:
                    var cret = this.SelectCareSequence(args.LinkageSystemNo, args.LinkageSystemId, args.ServiceCode, args.EffectiveDate, args.OrganizationId);
                    if (cret != null && cret.Count > 0)
                    {
                        result.Sequence = cret.First().SEQUENCE;
                        result.IsSuccess = true;
                        return result;
                    }
                    result.Sequence = -1;
                    result.IsSuccess = true;

                    break;
                case QsApiSystemTypeEnum.AfterSchool:
                    var asret = this.SelectAfterSchoolSequence(args.LinkageSystemNo, args.LinkageSystemId, args.ServiceCode, args.EffectiveDate, args.OrganizationId);
                    if (asret != null && asret.Count > 0)
                    {
                        result.Sequence = asret.First().SEQUENCE;
                        result.IsSuccess = true;
                        return result;
                    }
                    result.Sequence = -1;
                    result.IsSuccess = true;

                    break;
                default:
                    result.IsSuccess = false;
                    break;

            }

            return result;
        }
    }


}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 診察呼び出し順番情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class WaitingOrderListReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, WaitingOrderListReaderArgs, WaitingOrderListReaderResults>
    {

        /// <summary>
        /// <see cref="WaitingOrderListReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public WaitingOrderListReader() : base()
        {
        }


        /// <summary>
        /// 対象の診療科に対する順番情報を返す。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="waitingNumber"></param>
        /// <param name="reserveFlag"></param>
        /// <param name="orderDate"></param>
        /// <param name="priorityType"></param>
        /// <param name="sameDaySequence"></param>
        /// <returns></returns>
        private List<QH_WAITINGORDERLIST_DAT> SelectOrder(int linkageSystemNo, string departmentCode, string doctorCode, int waitingNumber, bool reserveFlag, DateTime orderDate, QsDbWaitingPriorityTypeEnum priorityType, int sameDaySequence)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_WAITINGORDERLIST_DAT>())
            {

                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", departmentCode),
                    this.CreateParameter(connection, "@P3", doctorCode),
                    this.CreateParameter(connection, "@P4", waitingNumber),
                    this.CreateParameter(connection, "@P5", DateTime.Parse(orderDate.ToString("yyyy-MM-dd 00:00:00"))),
                    this.CreateParameter(connection, "@P6", DateTime.Parse(orderDate.ToString("yyyy-MM-dd 23:59:58"))),
                    this.CreateParameter(connection, "@P7", DateTime.Parse(orderDate.ToString("yyyy-MM-dd 23:59:59"))),
                    this.CreateParameter(connection, "@P8", sameDaySequence)
                };

                // クエリを作成                
                StringBuilder query = new StringBuilder()
                    .Append(" SELECT TOP(@P4) *")
                    .Append(" FROM QH_WAITINGORDERLIST_DAT AS T1")
                    .Append(" WHERE T1.LINKAGESYSTEMNO = @P1")
                    .Append(" AND T1.DEPARTMENTCODE = @P2")
                    .Append(" AND T1.DOCTORCODE = @P3")
                    .Append(" AND T1.DJKBN = @P8")
                    .Append(" AND T1.DELETEFLAG = 0");

                if (reserveFlag)
                {
                    query.Append(" AND T1.RESERVATIONDATE BETWEEN @P5 AND @P6");
                }
                else
                {
                    query.Append(" AND T1.RESERVATIONDATE BETWEEN @P5 AND @P7");
                }

                //優先度設定を見て、並べ替え順序を制御する
                if (priorityType == QsDbWaitingPriorityTypeEnum.None || priorityType == QsDbWaitingPriorityTypeEnum.PriorityByReserve)
                {
                    //未指定もしくは1の場合は予約時刻が第一優先
                    query.Append(" ORDER BY RESERVATIONDATE, RECEPTIONDATE, RECEPTIONNO");
                }
                else if (priorityType == QsDbWaitingPriorityTypeEnum.PriorityByArrival)
                {
                    //2の場合は来院時刻が第一優先、予約時刻は考慮しない
                    query.Append(" ORDER BY RECEPTIONDATE, RECEPTIONNO");
                }

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_WAITINGORDERLIST_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
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
        public WaitingOrderListReaderResults ExecuteByDistributed(WaitingOrderListReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new WaitingOrderListReaderResults() { IsSuccess = false };

            result.WaitingOrderListEntity = this.SelectOrder(args.LinkageSystemNo, args.DepartmentCode, args.DoctorCode, args.WaitingNumber, args.ReserveFlag, args.OrderDate, args.priorityType, args.SameDaySequence);
             
            result.IsSuccess = true;
            return result;
        }
    }


}

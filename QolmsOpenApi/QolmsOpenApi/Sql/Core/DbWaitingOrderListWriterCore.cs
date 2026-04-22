using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// 診察呼び出し順番情報 を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbWaitingOrderListWriterCore : QsDbWriterBase
    {

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        public DbWaitingOrderListWriterCore() : base()
        {
        }


        #region "Private Method"

        #endregion

        #region "Public Method"

        /// <summary>
        /// 診察呼び出し順番情報を登録、更新します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public int UpsertWaitingOrderList(QH_WAITINGORDERLIST_DAT entity)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_WAITINGORDERLIST_DAT>())
            {
                
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", entity.LINKAGESYSTEMNO),
                    this.CreateParameter(connection, "@P2", entity.DEPARTMENTCODE),
                    this.CreateParameter(connection, "@P3", entity.DOCTORCODE),
                    this.CreateParameter(connection, "@P4", entity.LINKAGESYSTEMID),
                    this.CreateParameter(connection, "@P5", entity.PUSHSENDFLAG),
                    this.CreateParameter(connection, "@P6", entity.RESERVATIONDATE),
                    this.CreateParameter(connection, "@P7", entity.RECEPTIONDATE),
                    this.CreateParameter(connection, "@P8", entity.RECEPTIONNO),
                    this.CreateParameter(connection, "@P9", entity.DELETEFLAG),
                    this.CreateParameter(connection, "@P10", entity.CREATEDDATE),
                    this.CreateParameter(connection, "@P11", entity.UPDATEDDATE),
                    this.CreateParameter(connection, "@P12", entity.DJKBN)
                };
                var query = new StringBuilder();
                query.Append("BEGIN");
                query.Append(" IF (SELECT COUNT(*) FROM QH_WAITINGORDERLIST_DAT WHERE LINKAGESYSTEMNO = @P1 AND DEPARTMENTCODE = @P2 AND LINKAGESYSTEMID = @P4 AND RESERVATIONDATE = @P6 AND DJKBN = @P12) <> 0");
                query.Append(" BEGIN");
                query.Append("  UPDATE QH_WAITINGORDERLIST_DAT SET");
                query.Append("  DOCTORCODE = @P3, PUSHSENDFLAG = @P5, UPDATEDDATE = @P11");
                query.Append("  WHERE LINKAGESYSTEMNO = @P1 AND DEPARTMENTCODE = @P2 AND LINKAGESYSTEMID = @P4 AND RESERVATIONDATE = @P6 AND DJKBN = @P12");
                query.Append(" END");
                query.Append(" ELSE");
                query.Append(" BEGIN");
                query.Append("  INSERT INTO QH_WAITINGORDERLIST_DAT");
                query.Append("  (LINKAGESYSTEMNO, DEPARTMENTCODE, DOCTORCODE, LINKAGESYSTEMID, PUSHSENDFLAG, RESERVATIONDATE, RECEPTIONDATE, RECEPTIONNO, DELETEFLAG, CREATEDDATE, UPDATEDDATE, DJKBN)");
                query.Append("  VALUES");
                query.Append("  (@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10, @P11, @P12)");
                query.Append(" END ");
                query.Append("END");

                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// 診察呼び出し順番情報を登録、更新します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public int UpsertWaitingOrderListNanbu(QH_WAITINGORDERLIST_DAT entity)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_WAITINGORDERLIST_DAT>())
            {

                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", entity.LINKAGESYSTEMNO),
                    this.CreateParameter(connection, "@P2", entity.DEPARTMENTCODE),
                    this.CreateParameter(connection, "@P3", entity.DOCTORCODE),
                    this.CreateParameter(connection, "@P4", entity.LINKAGESYSTEMID),
                    this.CreateParameter(connection, "@P5", entity.PUSHSENDFLAG),
                    this.CreateParameter(connection, "@P6", entity.RESERVATIONDATE),
                    this.CreateParameter(connection, "@P7", entity.RECEPTIONDATE),
                    this.CreateParameter(connection, "@P8", entity.RECEPTIONNO),
                    this.CreateParameter(connection, "@P9", entity.DELETEFLAG),
                    this.CreateParameter(connection, "@P10", entity.CREATEDDATE),
                    this.CreateParameter(connection, "@P11", entity.UPDATEDDATE),
                    this.CreateParameter(connection, "@P12", entity.DJKBN)
                };
                var query = new StringBuilder();
                query.Append("BEGIN");
                query.Append(" IF (SELECT COUNT(*) FROM QH_WAITINGORDERLIST_DAT WHERE LINKAGESYSTEMNO = @P1 AND DEPARTMENTCODE = @P2 AND LINKAGESYSTEMID = @P4 AND RESERVATIONDATE = @P6 AND DJKBN = @P12) <> 0");
                query.Append(" BEGIN");
                query.Append("  UPDATE QH_WAITINGORDERLIST_DAT SET");
                query.Append("  DOCTORCODE = @P3, UPDATEDDATE = @P11");
                query.Append("  WHERE LINKAGESYSTEMNO = @P1 AND DEPARTMENTCODE = @P2 AND LINKAGESYSTEMID = @P4 AND RESERVATIONDATE = @P6 AND DJKBN = @P12");
                query.Append(" END");
                query.Append(" ELSE");
                query.Append(" BEGIN");
                query.Append("  INSERT INTO QH_WAITINGORDERLIST_DAT");
                query.Append("  (LINKAGESYSTEMNO, DEPARTMENTCODE, DOCTORCODE, LINKAGESYSTEMID, PUSHSENDFLAG, RESERVATIONDATE, RECEPTIONDATE, RECEPTIONNO, DELETEFLAG, CREATEDDATE, UPDATEDDATE, DJKBN)");
                query.Append("  VALUES");
                query.Append("  (@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10, @P11, @P12)");
                query.Append(" END ");
                query.Append("END");

                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// 診察呼び出し順番情報を削除します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public int DeleteWaitingOrderList(int linkageSystemNo, string departmentCode, string doctorCode, string linkageSystemId, int djKbn)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_WAITINGORDERLIST_DAT>())
            {

                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", linkageSystemNo),
                    this.CreateParameter(connection, "@P2", departmentCode),
                    this.CreateParameter(connection, "@P3", doctorCode),
                    this.CreateParameter(connection, "@P4", linkageSystemId),
                    this.CreateParameter(connection, "@P5", djKbn)
                };
                var query = new StringBuilder();
                query.Append(" DELETE QH_WAITINGORDERLIST_DAT");
                query.Append(" WHERE LINKAGESYSTEMNO = @P1 AND DEPARTMENTCODE = @P2 AND DOCTORCODE = @P3 AND LINKAGESYSTEMID = @P4 AND DJKBN = @P5");
                
                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }
        #endregion
    }
}
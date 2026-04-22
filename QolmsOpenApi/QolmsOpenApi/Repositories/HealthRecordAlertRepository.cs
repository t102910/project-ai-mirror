using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// QH_HEALTHRECORDALERT_DATの入出力
    /// </summary>
    public interface IHealthRecordAlertRepository
    {
        /// <summary>
        /// 次に採番するAlertNoを返す
        /// </summary>
        /// <returns></returns>
        long GetNewAlertNo();

        /// <summary>
        /// QH_HEALTHRECORDALERT_DATのレコードを1件取得
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="recordDate"></param>
        /// <param name="vitalType"></param>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        QH_HEALTHRECORDALERT_DAT ReadEntity(Guid accountKey, DateTime recordDate, byte vitalType, int linkageSystemNo);
        /// <summary>
        /// QH_HEALTHRECORDALERT_DATのレコードを1件取得(AlertNo指定)
        /// </summary>
        /// <param name="alertNo"></param>
        /// <returns></returns>
        QH_HEALTHRECORDALERT_DAT ReadEntity(int alertNo);
        /// <summary>
        /// QH_HEALTHRECORDALERT_DATにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        void InsertEntity(QH_HEALTHRECORDALERT_DAT entity);
        /// <summary>
        /// QH_HEALTHRECORDALERT_DATのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEntity(QH_HEALTHRECORDALERT_DAT entity);
        /// <summary>
        /// QH_HEALTHRECORDALERT_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity(QH_HEALTHRECORDALERT_DAT entity);
        /// <summary>
        /// QH_HEALTHRECORDALERT_DATのレコードを物理削除します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="recordDate"></param>
        /// <param name="vitalType"></param>
        /// <param name="linkageSystemNo"></param>
        void PhysicalDeleteEntity(Guid accountKey, DateTime recordDate, byte vitalType, int linkageSystemNo);

        /// <summary>
        /// QH_HEALTHRECORDALERTとQH_SYMPTOM_DATをマージしたVIEWを返します
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        List<QH_HEALTH_ALERT_SYMPTOM_VIEW> ReadAlertSymptomView(Guid accountKey, int linkageSystemNo, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// QH_HEALTHRECORDALERT_DATのレコードを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="vitalType"></param>
        /// <param name="offset"></param>
        /// <param name="fetch"></param>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        List<QH_HEALTHRECORDALERT_DAT> ReadEntities(Guid accountKey, DateTime fromDate, DateTime toDate, byte vitalType, int offset, int fetch, int linkageSystemNo);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class HealthRecordAlertRepository: QsDbReaderBase, IHealthRecordAlertRepository
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public long GetNewAlertNo()
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                // Alertの最大値を返し、nullの場合は０を返すクエリ
                var sql = $@"
                    SELECT COALESCE(MAX({nameof(QH_HEALTHRECORDALERT_DAT.ALERTNO)}),0)
                    FROM  {nameof(QH_HEALTHRECORDALERT_DAT)}
                ";

                con.Open();

                var ret = ExecuteScalar<long>(con, null, sql, null);

                return ret + 1;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public QH_HEALTHRECORDALERT_DAT ReadEntity(Guid accountKey, DateTime recordDate, byte vitalType, int linkageSystemNo)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", recordDate),
                    CreateParameter(con, "@p3", vitalType),
                    CreateParameter(con, "@p4", linkageSystemNo)
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_HEALTHRECORDALERT_DAT)}
                    WHERE {nameof(QH_HEALTHRECORDALERT_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.RECORDDATE)} = @p2
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.VITALTYPE)} = @p3
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.LINKAGESYSTEMNO)} = @p4
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_HEALTHRECORDALERT_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();                

                return entity;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public QH_HEALTHRECORDALERT_DAT ReadEntity(int alertNo)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", alertNo),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_HEALTHRECORDALERT_DAT)}
                    WHERE {nameof(QH_HEALTHRECORDALERT_DAT.ALERTNO)} = @p1
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_HEALTHRECORDALERT_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                return entity;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        public void InsertEntity(QH_HEALTHRECORDALERT_DAT entity)        {
            

            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new HealthRecordAlertWriterArgs { Entity = entity };
            var result = new HealthRecordAlertWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_HEALTHRECORDALERT_DAT)}の挿入に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEntity(QH_HEALTHRECORDALERT_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new HealthRecordAlertWriterArgs { Entity = entity };
            var result = new HealthRecordAlertWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_HEALTHRECORDALERT_DAT)}の更新に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEntity(QH_HEALTHRECORDALERT_DAT entity)
        {            

            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new HealthRecordAlertWriterArgs { Entity = entity };
            var result = new HealthRecordAlertWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_HEALTHRECORDALERT_DAT)}の削除に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void PhysicalDeleteEntity(Guid accountKey, DateTime recordDate, byte vitalType, int linkageSystemNo)
        {
            var entity = new QH_HEALTHRECORDALERT_DAT
            {
                ACCOUNTKEY = accountKey,
                RECORDDATE = recordDate,
                VITALTYPE = vitalType,
                LINKAGESYSTEMNO = linkageSystemNo
            };

            var args = new HealthRecordAlertWriterArgs
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new HealthRecordAlertWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_HEALTHRECORDALERT_DAT)}の物理削除に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<QH_HEALTH_ALERT_SYMPTOM_VIEW> ReadAlertSymptomView(Guid accountKey, int linkageSystemNo, DateTime fromDate, DateTime toDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", linkageSystemNo),
                    CreateParameter(con, "@p3", fromDate),
                    CreateParameter(con, "@p4", toDate)
                };

                var sql = $@"
                    SELECT ALERTNO, '00000000-0000-0000-0000-000000000000' AS SYMPTOMID ,
                           RECORDDATE, 2 AS DATATYPE, VITALTYPE,
                           VALUE1, VALUE2, ABNORMALTYPE
                    FROM QH_HEALTHRECORDALERT_DAT
                    WHERE ACCOUNTKEY = @p1
                    AND LINKAGESYSTEMNO = @p2
                    AND RECORDDATE BETWEEN @p3 AND @p4
                    AND DELETEFLAG = 0

                    UNION ALL

                    SELECT 0 AS ALERTNO, ID AS SYMPTOMID, 
                           RECORDDATE, 3 As DATATYPE, 0 AS VITALTYPE,
                           0 AS VALUE1, 0 AS VALUE2, 0 AS ABNORMALTYPE
                    FROM QH_SYMPTOM_DAT
                    WHERE ACCOUNTKEY = @p1
                    AND LINKAGESYSTEMNO = @p2
                    AND RECORDDATE BETWEEN @p3 AND @p4
                    AND DELETEFLAG = 0

                    ORDER BY RECORDDATE DESC
                ";

                con.Open();

                return ExecuteReader<QH_HEALTH_ALERT_SYMPTOM_VIEW>(con, null, sql, parameters);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<QH_HEALTHRECORDALERT_DAT> ReadEntities(Guid accountKey, DateTime fromDate, DateTime toDate, byte vitalType, int offset, int fetch, int linkageSystemNo)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", fromDate),
                    CreateParameter(con, "@p3", toDate),
                    CreateParameter(con, "@p4", offset),
                    CreateParameter(con, "@p5", fetch),
                    CreateParameter(con, "@p6", linkageSystemNo),
                    CreateParameter(con, "@p7", vitalType)
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_HEALTHRECORDALERT_DAT)}
                    WHERE {nameof(QH_HEALTHRECORDALERT_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.RECORDDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.VITALTYPE)} = @p7
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.LINKAGESYSTEMNO)} = @p6
                    AND   {nameof(QH_HEALTHRECORDALERT_DAT.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_HEALTHRECORDALERT_DAT.RECORDDATE)} DESC
                    OFFSET @p4 ROWS 
                    FETCH NEXT @p5 ROWS ONLY
                ";

                con.Open();

                return ExecuteReader<QH_HEALTHRECORDALERT_DAT>(con, null, sql, paramList);
            }
        }
    }
}
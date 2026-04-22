using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using Workers = MGF.QOLMS.QolmsLibraryV1.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Common;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// Jotoネイティブアプリ向け入出力インターフェース
    /// </summary>
    public interface IJotoAppRepository
    {
        /// <summary>
        /// 運動情報範囲データを取得する。
        /// LINKAGESYSTEMNO フィルタなし（新方式:47003 / 旧方式:99999 の両方を返す）。
        /// </summary>
        List<QH_EXERCISEEVENT2_DAT> ReadExerciseRange(Guid accountKey, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// メンタル情報範囲データを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="vitalTypes"></param>
        /// <returns></returns>
        List<QH_EVENT_DAT> ReadMentalRange(Guid accountKey, DateTime fromDate, DateTime toDate, int linkageSystemNo);

        /// <summary>
        /// 運動マスタ一覧を取得する (LINKAGESYSTEMNO=47003 固定)
        /// </summary>
        List<QH_EXERCISEITEM2_MST> ReadExerciseItemList();

        /// <summary>
        /// 運動イベントの論理削除。
        /// exerciseType &gt; 0 の場合は指定種別のみ削除（個別削除）。
        /// exerciseType = 0 の場合は当日全種別を削除（一括削除）。
        /// LINKAGESYSTEMNO フィルタなし（新方式:47003 / 旧方式:99999 の両方が対象）。
        /// </summary>
        void DeleteExercise(Guid accountKey, DateTime recordDate, byte exerciseType = 0);

    }

    /// <summary>
    /// バイタル値入出力実装
    /// </summary>
    public class JotoAppRepository : QsDbReaderBase, IJotoAppRepository
    {
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="vitalTypes"></param>
        /// <returns></returns>
        /// <summary>
        /// <inheritdoc/>
        /// LINKAGESYSTEMNO フィルタなし（旧:99999 / 新:47003 両方取得）。
        /// ORDER BY RECORDDATE, EXERCISETYPE, SEQUENCE で安定したソート順を保証。
        /// </summary>
        public List<QH_EXERCISEEVENT2_DAT> ReadExerciseRange(Guid accountKey, DateTime fromDate, DateTime toDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", fromDate),
                    CreateParameter(con, "@p3", toDate),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_EXERCISEEVENT2_DAT)}
                    WHERE {nameof(QH_EXERCISEEVENT2_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_EXERCISEEVENT2_DAT.RECORDDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_EXERCISEEVENT2_DAT.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_EXERCISEEVENT2_DAT.RECORDDATE)},
                             {nameof(QH_EXERCISEEVENT2_DAT.EXERCISETYPE)},
                             {nameof(QH_EXERCISEEVENT2_DAT.SEQUENCE)}
                ";

                con.Open();
                return ExecuteReader<QH_EXERCISEEVENT2_DAT>(con, null, sql, paramList);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="vitalTypes"></param>
        /// <returns></returns>
        public List<QH_EVENT_DAT> ReadMentalRange(Guid accountKey, DateTime fromDate, DateTime toDate, int linkageSystemNo)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", fromDate),
                    CreateParameter(con, "@p3", toDate),
                    CreateParameter(con, "@p4", linkageSystemNo),
                    CreateParameter(con, "@p5", "QhQolmsDiaryEventSetOfJson"),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_EVENT_DAT)}
                    WHERE {nameof(QH_EVENT_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_EVENT_DAT.LINKAGESYSTEMNO)} = @p4
                    AND   {nameof(QH_EVENT_DAT.EVENTDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_EVENT_DAT.EVENTSETTYPENAME)} = @p5
                    AND   {nameof(QH_EVENT_DAT.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_EVENT_DAT.EVENTDATE)} DESC
                ";

                con.Open();

                return ExecuteReader<QH_EVENT_DAT>(con, null, sql, paramList);
            }
        }
        /// <summary>
        /// 運動マスタ一覧を取得します（LINKAGESYSTEMNO=47003 固定）。
        /// </summary>
        public List<QH_EXERCISEITEM2_MST> ReadExerciseItemList()
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                var paramList = new List<DbParameter>()
                {
                    CreateParameter(con, "@p1", 47003),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_EXERCISEITEM2_MST)}
                    WHERE {nameof(QH_EXERCISEITEM2_MST.LINKAGESYSTEMNO)} = @p1
                    AND   {nameof(QH_EXERCISEITEM2_MST.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_EXERCISEITEM2_MST.EXERCISETYPE)}
                ";

                con.Open();
                return ExecuteReader<QH_EXERCISEITEM2_MST>(con, null, sql, paramList);
            }
        }

        /// <summary>
        /// 運動データを論理削除します（LINKAGESYSTEMNO フィルタなし：旧99999 / 新47003 両方が対象）。
        /// exerciseType &gt; 0 : 指定種別のみ削除（個別削除）。
        /// exerciseType = 0 : 当日全種別を削除（一括削除）。
        /// </summary>
        public void DeleteExercise(Guid accountKey, DateTime recordDate, byte exerciseType = 0)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                var dayStart = new DateTime(recordDate.Year, recordDate.Month, recordDate.Day,  0,  0,  0,   0);
                var dayEnd   = new DateTime(recordDate.Year, recordDate.Month, recordDate.Day, 23, 59, 59, 999);

                // exerciseType > 0 の場合は EXERCISETYPE 条件を付与（個別削除）
                var typeFilter = exerciseType > 0
                    ? $"AND {nameof(QH_EXERCISEEVENT2_DAT.EXERCISETYPE)} = @p5"
                    : string.Empty;

                var sql = $@"
                    UPDATE {nameof(QH_EXERCISEEVENT2_DAT)}
                    SET    {nameof(QH_EXERCISEEVENT2_DAT.DELETEFLAG)}  = 1,
                           {nameof(QH_EXERCISEEVENT2_DAT.UPDATEDDATE)} = @p4
                    WHERE  {nameof(QH_EXERCISEEVENT2_DAT.ACCOUNTKEY)} = @p1
                    AND    {nameof(QH_EXERCISEEVENT2_DAT.RECORDDATE)} BETWEEN @p2 AND @p3
                    AND    {nameof(QH_EXERCISEEVENT2_DAT.DELETEFLAG)} = 0
                    {typeFilter}
                ";

                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(CreateParameter(con, "@p1", accountKey));
                    cmd.Parameters.Add(CreateParameter(con, "@p2", dayStart));
                    cmd.Parameters.Add(CreateParameter(con, "@p3", dayEnd));
                    cmd.Parameters.Add(CreateParameter(con, "@p4", DateTime.Now));
                    if (exerciseType > 0)
                        cmd.Parameters.Add(CreateParameter(con, "@p5", exerciseType));
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
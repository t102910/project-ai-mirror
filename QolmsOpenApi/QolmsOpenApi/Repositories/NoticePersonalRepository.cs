using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// PersonalRead 用お知らせ入出力インターフェース
    /// </summary>
    public interface INoticePersonalRepository
    {
        /// <summary>
        /// 個人向けお知らせ一覧を取得します。
        /// </summary>
        /// <param name="accountKeyList">対象アカウントキー一覧</param>
        /// <param name="toSystemTypeList">対象システム種別一覧</param>
        /// <param name="startDate">開始日</param>
        /// <param name="endDate">終了日</param>
        /// <param name="onlyUnread">未読のみフラグ</param>
        /// <param name="categoryNo">カテゴリ</param>
        /// <returns>総件数</returns>
        int ReadCount(
            List<Guid> accountKeyList,
            List<byte> toSystemTypeList,
            DateTime startDate,
            DateTime endDate,
            bool onlyUnread,
            byte categoryNo);

        /// <summary>
        /// 個人向けお知らせ一覧を取得します。
        /// </summary>
        /// <param name="accountKeyList">対象アカウントキー一覧</param>
        /// <param name="toSystemTypeList">対象システム種別一覧</param>
        /// <param name="startDate">開始日</param>
        /// <param name="endDate">終了日</param>
        /// <param name="onlyUnread">未読のみフラグ</param>
        /// <param name="rowStart">開始行番号</param>
        /// <param name="rowEnd">終了行番号</param>
        /// <param name="categoryNo">カテゴリ</param>
        /// <returns>一覧結果</returns>
        List<QH_NOTICEPERSONAL_READ_VIEW> ReadList(
            List<Guid> accountKeyList,
            List<byte> toSystemTypeList,
            DateTime startDate,
            DateTime endDate,
            bool onlyUnread,
            int rowStart,
            int rowEnd,
            byte categoryNo);

        /// <summary>
        /// 個人向けお知らせを 1 件取得します。
        /// </summary>
        /// <param name="noticeNo">お知らせ番号</param>
        /// <returns>お知らせ</returns>
        QH_NOTICEPERSONAL_READ_VIEW ReadById(long noticeNo);
    }

    /// <summary>
    /// PersonalRead 用お知らせ入出力実装
    /// </summary>
    public sealed class NoticePersonalRepository : QsDbReaderBase, INoticePersonalRepository
    {
        const string AlreadyReadNoticeNoColumn = "NOTICENO";
        const string AlreadyReadRecordCountColumn = "RECORDCOUNT";
        const string AlreadyReadFlagColumn = "ALREADYREADFLAG";

        /// <inheritdoc/>
        public int ReadCount(
            List<Guid> accountKeyList,
            List<byte> toSystemTypeList,
            DateTime startDate,
            DateTime endDate,
            bool onlyUnread,
            byte categoryNo)
        {
            if (accountKeyList == null || accountKeyList.Count == 0 || toSystemTypeList == null || toSystemTypeList.Count == 0)
            {
                return 0;
            }

            using (var con = QsDbManager.CreateDbConnection<QH_NOTICEPERSONAL_DAT>())
            {
                con.Open();

                // 一覧取得と同じ絞り込み条件で総件数だけを先に返す。
                string whereClause;
                List<DbParameter> countParameters;
                this.BuildListWhereParameters(con, toSystemTypeList, accountKeyList, startDate, endDate, onlyUnread, categoryNo, out whereClause, out countParameters);

                var countJoinClause = onlyUnread
                    ? $@"
LEFT OUTER JOIN {nameof(QH_NOTICEPERSONALALREADYREAD_DAT)} AS ar
    ON np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICENO)} = ar.{nameof(QH_NOTICEPERSONALALREADYREAD_DAT.NOTICENO)}
    AND ar.{nameof(QH_NOTICEPERSONALALREADYREAD_DAT.DELETEFLAG)} = 0"
                    : string.Empty;

                var countSql = $@"
SELECT COUNT(*)
FROM {nameof(QH_NOTICEPERSONAL_DAT)} AS np
{countJoinClause}
WHERE {whereClause}";

                return ExecuteScalar<int>(con, null, countSql, countParameters);
            }
        }

        /// <inheritdoc/>
        public List<QH_NOTICEPERSONAL_READ_VIEW> ReadList(
            List<Guid> accountKeyList,
            List<byte> toSystemTypeList,
            DateTime startDate,
            DateTime endDate,
            bool onlyUnread,
            int rowStart,
            int rowEnd,
            byte categoryNo)
        {
            var items = new List<QH_NOTICEPERSONAL_READ_VIEW>();
            if (accountKeyList == null || accountKeyList.Count == 0 || toSystemTypeList == null || toSystemTypeList.Count == 0)
            {
                return items;
            }

            using (var con = QsDbManager.CreateDbConnection<QH_NOTICEPERSONAL_DAT>())
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                con.Open();

                // Worker で確定した行範囲だけを返す。Repository 側では再計算しない。
                string whereClause;
                List<DbParameter> mainParameters;
                this.BuildListWhereParameters(con, toSystemTypeList, accountKeyList, startDate, endDate, onlyUnread, categoryNo, out whereClause, out mainParameters);

                var pagingParamIndex = mainParameters.Count + 1;
                mainParameters.Add(this.CreateParameter(con, $"@p{pagingParamIndex}", rowStart));
                mainParameters.Add(this.CreateParameter(con, $"@p{pagingParamIndex + 1}", rowEnd));
                var pagingCondition = $"WHERE ROW_NO BETWEEN @p{pagingParamIndex} AND @p{pagingParamIndex + 1}";

                // ROW_NUMBER で採番した結果から対象ページだけを切り出す。
                var sql = $@"
SELECT *
FROM (
    SELECT
        np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICENO)} AS NOTICENO,
        np.{nameof(QH_NOTICEPERSONAL_DAT.TITLE)} AS TITLE,
        np.{nameof(QH_NOTICEPERSONAL_DAT.CONTENTS)} AS CONTENTS,
        np.{nameof(QH_NOTICEPERSONAL_DAT.CATEGORYNO)} AS CATEGORYNO,
        np.{nameof(QH_NOTICEPERSONAL_DAT.PRIORITYNO)} AS PRIORITYNO,
        np.{nameof(QH_NOTICEPERSONAL_DAT.ACCOUNTKEY)} AS ACCOUNTKEY,
        np.{nameof(QH_NOTICEPERSONAL_DAT.FACILITYKEY)} AS FACILITYKEY,
        np.{nameof(QH_NOTICEPERSONAL_DAT.STARTDATE)} AS STARTDATE,
        np.{nameof(QH_NOTICEPERSONAL_DAT.ENDDATE)} AS ENDDATE,
        np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICEDATASET)} AS NOTICEDATASET,
        ISNULL(ar.{nameof(QH_NOTICEPERSONALALREADYREAD_DAT.ALREADYREADFLAG)}, 0) AS ALREADYREADFLAG,
        sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.FAMILYNAME)} AS SENDERFAMILYNAME,
        sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.GIVENNAME)} AS SENDERGIVENNAME,
        ROW_NUMBER() OVER (ORDER BY np.{nameof(QH_NOTICEPERSONAL_DAT.PRIORITYNO)} ASC, np.{nameof(QH_NOTICEPERSONAL_DAT.STARTDATE)} DESC) AS ROW_NO
    FROM {nameof(QH_NOTICEPERSONAL_DAT)} np
    INNER JOIN {nameof(QH_ACCOUNTINDEX_DAT)} ai
        ON ai.{nameof(QH_ACCOUNTINDEX_DAT.ACCOUNTKEY)} = np.{nameof(QH_NOTICEPERSONAL_DAT.ACCOUNTKEY)}
        AND ai.{nameof(QH_ACCOUNTINDEX_DAT.DELETEFLAG)} = 0
    LEFT OUTER JOIN {nameof(QH_NOTICEPERSONALALREADYREAD_DAT)} ar
        ON ar.{nameof(QH_NOTICEPERSONALALREADYREAD_DAT.NOTICENO)} = np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICENO)}
        AND ar.{nameof(QH_NOTICEPERSONALALREADYREAD_DAT.DELETEFLAG)} = 0
    LEFT JOIN {nameof(QH_ACCOUNTINDEX_DAT)} sender_ai
        ON sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.ACCOUNTKEY)} = np.{nameof(QH_NOTICEPERSONAL_DAT.CREATEDACCOUNTKEY)}
        AND sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.DELETEFLAG)} = 0
    WHERE {whereClause}
) AS tb
{pagingCondition}
ORDER BY ROW_NO";

                var rawItems = ExecuteReader<QH_NOTICEPERSONAL_READ_VIEW>(con, null, sql, mainParameters);
                foreach (var rawItem in rawItems)
                {
                    rawItem.SENDERFAMILYNAME = TryDecrypt(rawItem.SENDERFAMILYNAME, crypt);
                    rawItem.SENDERGIVENNAME = TryDecrypt(rawItem.SENDERGIVENNAME, crypt);
                    items.Add(rawItem);
                }

                return items;
            }
        }

        /// <inheritdoc/>
        public QH_NOTICEPERSONAL_READ_VIEW ReadById(long noticeNo)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_NOTICEPERSONAL_DAT>())
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                // 既読テーブルは noticeNo 単位で集約し、旧仕様の「1件だけ存在し、その値が true のときだけ既読」を SQL で再現する。
                var sql = $@"
SELECT TOP 1
    np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICENO)} AS NOTICENO,
    np.{nameof(QH_NOTICEPERSONAL_DAT.TITLE)} AS TITLE,
    np.{nameof(QH_NOTICEPERSONAL_DAT.CONTENTS)} AS CONTENTS,
    np.{nameof(QH_NOTICEPERSONAL_DAT.CATEGORYNO)} AS CATEGORYNO,
    np.{nameof(QH_NOTICEPERSONAL_DAT.PRIORITYNO)} AS PRIORITYNO,
    np.{nameof(QH_NOTICEPERSONAL_DAT.ACCOUNTKEY)} AS ACCOUNTKEY,
    np.{nameof(QH_NOTICEPERSONAL_DAT.FACILITYKEY)} AS FACILITYKEY,
    np.{nameof(QH_NOTICEPERSONAL_DAT.STARTDATE)} AS STARTDATE,
    np.{nameof(QH_NOTICEPERSONAL_DAT.ENDDATE)} AS ENDDATE,
    np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICEDATASET)} AS NOTICEDATASET,
    CAST(CASE
        WHEN ISNULL(ar.{AlreadyReadRecordCountColumn}, 0) = 1
            AND ISNULL(ar.{AlreadyReadFlagColumn}, 0) = 1 THEN 1
        ELSE 0
    END AS bit) AS ALREADYREADFLAG,
    sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.FAMILYNAME)} AS SENDERFAMILYNAME,
    sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.GIVENNAME)} AS SENDERGIVENNAME
FROM {nameof(QH_NOTICEPERSONAL_DAT)} np
LEFT JOIN (
    SELECT
        {nameof(QH_NOTICEPERSONALALREADYREAD_DAT.NOTICENO)} AS {AlreadyReadNoticeNoColumn},
        COUNT(*) AS {AlreadyReadRecordCountColumn},
        MAX(CASE WHEN {nameof(QH_NOTICEPERSONALALREADYREAD_DAT.ALREADYREADFLAG)} = 1 THEN 1 ELSE 0 END) AS {AlreadyReadFlagColumn}
    FROM {nameof(QH_NOTICEPERSONALALREADYREAD_DAT)}
    WHERE {nameof(QH_NOTICEPERSONALALREADYREAD_DAT.DELETEFLAG)} = 0
    GROUP BY {nameof(QH_NOTICEPERSONALALREADYREAD_DAT.NOTICENO)}
) ar
    ON ar.{AlreadyReadNoticeNoColumn} = np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICENO)}
LEFT JOIN {nameof(QH_ACCOUNTINDEX_DAT)} sender_ai
    ON sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.ACCOUNTKEY)} = np.{nameof(QH_NOTICEPERSONAL_DAT.CREATEDACCOUNTKEY)}
    AND sender_ai.{nameof(QH_ACCOUNTINDEX_DAT.DELETEFLAG)} = 0
WHERE np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICENO)} = @p1
AND np.{nameof(QH_NOTICEPERSONAL_DAT.DELETEFLAG)} = 0
ORDER BY np.{nameof(QH_NOTICEPERSONAL_DAT.NOTICENO)} DESC";

                var parameters = new List<DbParameter> { CreateParameter(con, "@p1", noticeNo) };

                con.Open();
                var item = ExecuteReader<QH_NOTICEPERSONAL_READ_VIEW>(con, null, sql, parameters).FirstOrDefault();
                if (item == null)
                {
                    return null;
                }

                item.SENDERFAMILYNAME = TryDecrypt(item.SENDERFAMILYNAME, crypt);
                item.SENDERGIVENNAME = TryDecrypt(item.SENDERGIVENNAME, crypt);
                return item;
            }
        }

        string BuildInCondition<T>(DbConnection con, ICollection<DbParameter> parameters, IEnumerable<T> values, ref int parameterIndex)
        {
            var names = new List<string>();
            foreach (var value in values)
            {
                var parameterName = $"@p{parameterIndex}";
                parameters.Add(this.CreateParameter(con, parameterName, value));
                names.Add(parameterName);
                parameterIndex++;
            }

            return string.Join(", ", names);
        }

        void BuildListWhereParameters(
            DbConnection con,
            List<byte> toSystemTypeList,
            List<Guid> accountKeyList,
            DateTime startDate,
            DateTime endDate,
            bool onlyUnread,
            byte categoryNo,
            out string whereClause,
            out List<DbParameter> parameters)
        {
            parameters = new List<DbParameter>();
            var parameterIndex = 1;

            var systemTypeCondition = this.BuildInCondition(con, parameters, toSystemTypeList, ref parameterIndex);
            var accountKeyCondition = this.BuildInCondition(con, parameters, accountKeyList, ref parameterIndex);

            var whereConditions = new List<string>
            {
                $"np.{nameof(QH_NOTICEPERSONAL_DAT.DELETEFLAG)} = 0",
                $"np.{nameof(QH_NOTICEPERSONAL_DAT.TOSYSTEMTYPE)} IN ({systemTypeCondition})",
                $"np.{nameof(QH_NOTICEPERSONAL_DAT.ACCOUNTKEY)} IN ({accountKeyCondition})"
            };

            if (startDate > DateTime.MinValue && endDate > DateTime.MinValue)
            {
                // 期間内開始・期間内終了・期間またぎの 3 パターンを全て拾う。
                whereConditions.Add(
                    $"((np.{nameof(QH_NOTICEPERSONAL_DAT.STARTDATE)} BETWEEN @p{parameterIndex} AND @p{parameterIndex + 1}) " +
                    $"OR (np.{nameof(QH_NOTICEPERSONAL_DAT.ENDDATE)} BETWEEN @p{parameterIndex} AND @p{parameterIndex + 1}) " +
                    $"OR (np.{nameof(QH_NOTICEPERSONAL_DAT.STARTDATE)} < @p{parameterIndex} AND np.{nameof(QH_NOTICEPERSONAL_DAT.ENDDATE)} > @p{parameterIndex + 1}))");
                parameters.Add(this.CreateParameter(con, $"@p{parameterIndex}", startDate));
                parameterIndex++;
                parameters.Add(this.CreateParameter(con, $"@p{parameterIndex}", endDate));
                parameterIndex++;
            }

            if (onlyUnread)
            {
                // 未読抽出時だけ LEFT JOIN 済みの既読情報を条件に使う。
                whereConditions.Add("(ar.ALREADYREADFLAG = 0 OR ar.ALREADYREADFLAG IS NULL)");
            }

            if (categoryNo != byte.MinValue)
            {
                whereConditions.Add($"np.{nameof(QH_NOTICEPERSONAL_DAT.CATEGORYNO)} = @p{parameterIndex}");
                parameters.Add(this.CreateParameter(con, $"@p{parameterIndex}", categoryNo));
            }

            whereClause = string.Join("\n    AND ", whereConditions);
        }

        string TryDecrypt(string value, QsCrypt crypt)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            try
            {
                return crypt.TryDecrypt(value);
            }
            catch
            {
                // 旧データや復号不能データが混ざっても一覧取得全体は継続する。
                return string.Empty;
            }
        }

    }
}
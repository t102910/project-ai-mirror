using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// グループへのお知らせ
    /// </summary>
    public interface INoticeGroupRepository
    {
        /// <summary>
        /// グループへのお知らせ一覧を取得します
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="targetType">対象タイプ</param>
        /// <param name="categoryNo">カテゴリ番号</param>
        /// <param name="alreadyRead">既読状態</param>
        /// <param name="pageIndex">ページ番号</param>
        /// <param name="pageSize">ページサイズ</param>
        /// <returns>お知らせ一覧</returns>
        List<DbNoticeGroupItem> ReadNoticeGroupList(Guid accountKey, byte targetType, byte categoryNo, byte alreadyRead, int pageIndex, int pageSize);

        /// <summary>
        /// グループへのお知らせ詳細を取得します
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="noticeNo">お知らせ番号</param>
        /// <returns>お知らせ詳細</returns>
        DbNoticeGroupItem ReadNoticeGroupDetail(Guid accountKey, long noticeNo);

        /// <summary>
        /// グループへのお知らせ管理 データ テーブル 登録
        /// </summary>
        /// <param name="entity">テーブルエンティティ</param>
        /// <returns>お知らせ番号</returns>
        long InsertNoticeGroupData(QH_NOTICEGROUP_DAT entity);

        /// <summary>
        /// グループへのお知らせ管理 データ テーブル 更新
        /// </summary>
        /// <param name="noticeNo">お知らせ番号</param>
        /// <param name="pushSend">プッシュ送信フラグ</param>
        /// <param name="mailSend">メール送信フラグ</param>
        /// <param name="noticeId">プッシュID</param>
        /// <returns>成功はtrue 、失敗はfalse</returns>
        bool UpdateNoticeGroupData(long noticeNo, Nullable<bool> pushSend, Nullable<bool> mailSend,List<string[]> noticeId);

        /// <summary>
        /// グループへのお知らせ対象者 管理 データ テーブル へ 対象者を Balk Insert します
        /// </summary>
        /// <param name="noticeno">お知らせ番号</param>
        /// <param name="accountkeyList">対象者アカウントキーのリスト</param>
        /// <param name="now">更新日</param>
        /// <returns>成功はtrue 、失敗はfalse</returns>
        bool WriteGroupTarget(long noticeno, List<(Guid acc, string[] pushid)> sendResult, DateTime pushDate, DateTime now);

        /// <summary>
        /// グループへのお知らせ管理 データ テーブル を取得します
        /// </summary>
        /// <param name="noticeno">お知らせ番号</param>
        /// <returns>テーブルエンティティ</returns>
        QH_NOTICEGROUP_DAT GetNoticeGroup(long noticeno);

        /// <summary>
        /// グループへのお知らせ対象者 管理 データ テーブル の既読状態を更新します
        /// </summary>
        /// <param name="noticeno">お知らせ番号</param>
        /// <param name="accountkey">対象者アカウントキー</param>
        /// <param name="alreadyReadFlag">既読フラグ</param>
        /// <param name="alreadyReadDate">既読日時</param>
        /// <param name="updatedDate">更新日</param>
        /// <returns>成功はtrue 、失敗はfalse</returns>
        bool UpdateNoticeGroupTarget(long noticeno, Guid accountkey, bool alreadyReadFlag, DateTime alreadyReadDate, DateTime updatedDate);
    }

    /// <summary>
    /// グループへのお知らせ
    /// </summary>
    public class NoticeGroupRepository : QsDbReaderBase, INoticeGroupRepository
    {
        /// <summary>
        /// アカウントに紐づく連携施設キー一覧（Guid.Empty含む）を取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <returns>施設キー一覧</returns>
        private List<Guid> ReadNoticeTargetFacilityKeys(Guid accountKey)
        {
            var facilityKeys = new List<Guid>() { Guid.Empty };

            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    this.CreateParameter(con, "@p1", accountKey),
                    this.CreateParameter(con, "@p2", (byte)QsDbLinkageStatusTypeEnum.Approved),
                };

                var sql = $@"
                    select t2.*
                    from {nameof(QH_LINKAGE_DAT)} as t1
                    inner join {nameof(QH_LINKAGESYSTEM_MST)} as t2
                        on t1.{nameof(QH_LINKAGE_DAT.LINKAGESYSTEMNO)} = t2.{nameof(QH_LINKAGESYSTEM_MST.LINKAGESYSTEMNO)}
                    where t1.{nameof(QH_LINKAGE_DAT.ACCOUNTKEY)} = @p1
                      and t1.{nameof(QH_LINKAGE_DAT.STATUSTYPE)} = @p2
                      and t1.{nameof(QH_LINKAGE_DAT.DELETEFLAG)} = 0
                      and t2.{nameof(QH_LINKAGESYSTEM_MST.DELETEFLAG)} = 0";

                con.Open();
                var rows = this.ExecuteReader<QH_LINKAGESYSTEM_MST>(con, null, sql, parameters);
                facilityKeys.AddRange(rows.Select(x => x.FACILITYKEY));
            }

            return facilityKeys
                .Where(x => x != Guid.Empty)
                .Distinct()
                .Prepend(Guid.Empty)
                .ToList();
        }

        /// <summary>
        /// グループへのお知らせ一覧を取得します
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="targetType">対象タイプ</param>
        /// <param name="categoryNo">カテゴリ番号</param>
        /// <param name="alreadyRead">既読状態</param>
        /// <param name="pageIndex">ページ番号</param>
        /// <param name="pageSize">ページサイズ</param>
        /// <returns>お知らせ一覧</returns>
        public List<DbNoticeGroupItem> ReadNoticeGroupList(Guid accountKey, byte targetType, byte categoryNo, byte alreadyRead, int pageIndex, int pageSize)
        {
            var facilityKeyN = this.ReadNoticeTargetFacilityKeys(accountKey);
            var args = new NoticeGroupReaderArgs()
            {
                AccountKey = accountKey,
                TargetType = targetType,
                CategoryNo = categoryNo,
                AlreadyRead = alreadyRead,
                PageIndex = pageIndex,
                PageSize = pageSize,
                FacilityKeyN = facilityKeyN
            };

            var result = QsDbManager.Read(new NoticeGroupReader(), args);
            if (result != null && result.IsSuccess && result.NoticeGroupItemN != null)
            {
                return result.NoticeGroupItemN;
            }

            return new List<DbNoticeGroupItem>();
        }

        /// <summary>
        /// グループへのお知らせ詳細を取得します
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="noticeNo">お知らせ番号</param>
        /// <returns>お知らせ詳細</returns>
        public DbNoticeGroupItem ReadNoticeGroupDetail(Guid accountKey, long noticeNo)
        {
            var facilityKeyN = this.ReadNoticeTargetFacilityKeys(accountKey);
            var args = new NoticeGroupDetailReaderArgs()
            {
                AccountKey = accountKey,
                NoticeNo = noticeNo,
                FacilityKeyN = facilityKeyN
            };

            var result = QsDbManager.Read(new NoticeGroupDetailReader(), args);
            if (result != null && result.IsSuccess)
            {
                return result.NoticeGroupItem;
            }

            return null;
        }

        /// <summary>
        /// グループへのお知らせ管理 データ テーブル を取得します
        /// </summary>
        /// <param name="noticeno">お知らせ番号</param>
        /// <returns>テーブルエンティティ</returns>
        public QH_NOTICEGROUP_DAT GetNoticeGroup(long noticeno)
        {
            QH_NOTICEGROUP_DAT entity = new QH_NOTICEGROUP_DAT() { NOTICENO = noticeno };
            var reader = new QhNoticeGroupEntityReader();
            var readerArgs = new QhNoticeGroupEntityReaderArgs() { Data = { entity } };

            QhNoticeGroupEntityReaderResults readerResult = QsDbManager.Read(reader, readerArgs);

            if (readerResult.IsSuccess && readerResult.Result.Any() && readerResult.Result.First().DELETEFLAG == false)
            {
                return readerResult.Result.FirstOrDefault();
            }
            else
            {
                return new QH_NOTICEGROUP_DAT();
            }
        }

        /// <summary>
        /// グループへのお知らせ管理 データ テーブル 登録
        /// </summary>
        /// <param name="entity">テーブルエンティティ</param>
        /// <returns>お知らせ番号</returns>
        public long InsertNoticeGroupData(QH_NOTICEGROUP_DAT entity)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_NOTICEGROUP_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    this.CreateParameter(con,"@p1", entity.TITLE),
                    this.CreateParameter(con,"@p2", entity.CONTENTS),
                    this.CreateParameter(con,"@p3", entity.CATEGORYNO),
                    this.CreateParameter(con,"@p4", entity.PRIORITYNO),
                    this.CreateParameter(con,"@p5", entity.FROMSYSTEMTYPE),
                    this.CreateParameter(con,"@p6", entity.TOSYSTEMTYPE),
                    this.CreateParameter(con,"@p7", entity.FACILITYKEY),
                    this.CreateParameter(con,"@p8", entity.TARGETTYPE),
                    this.CreateParameter(con,"@p9", entity.STARTDATE),
                    this.CreateParameter(con,"@p10", entity.ENDDATE),
                    this.CreateParameter(con,"@p11", entity. MAILSENDFLAG),
                    this.CreateParameter(con,"@p12", entity.PUSHSENDFLAG),
                    this.CreateParameter(con,"@p13", entity.SCHEDULENO),
                    this.CreateParameter(con,"@p14", entity.NOTICEDATASET),
                    this.CreateParameter(con,"@p15", entity.DELETEFLAG),
                    this.CreateParameter(con,"@p16", entity.CREATEDDATE),
                    this.CreateParameter(con,"@p17", entity.CREATEDACCOUNTKEY),
                    this.CreateParameter(con,"@p18", entity.UPDATEDDATE),
                    this.CreateParameter(con,"@p19", entity.UPDATEDACCOUNTKEY),
                };

                var sql = $@"
                    declare @noticeno bigint;
                    select @noticeno = ISNULL(MAX({nameof(QH_NOTICEGROUP_DAT.NOTICENO)}), 0) + 1 from {nameof(QH_NOTICEGROUP_DAT)}
                    insert into {nameof(QH_NOTICEGROUP_DAT)}
                    output inserted.{nameof(QH_NOTICEGROUP_DAT.NOTICENO)}
                    values
                    ( @noticeno
                    , @p1
                    , @p2
                    , @p3
                    , @p4
                    , @p5
                    , @p6
                    , @p7
                    , @p8
                    , @p9
                    , @p10
                    , @p11
                    , @p12
                    , @p13
                    , @p14
                    , @p15
                    , @p16
                    , @p17
                    , @p18
                    , @p19)";

                con.Open();
                return ExecuteScalar<long>(con, null, sql, parameters);
            }
        }

        /// <summary>
        /// グループへのお知らせ管理 データ テーブル 更新
        /// </summary>
        /// <param name="noticeNo">お知らせ番号</param>
        /// <param name="pushSend">プッシュ送信フラグ</param>
        /// <param name="mailSend">メール送信フラグ</param>
        /// <param name="noticeId">プッシュID</param>
        /// <returns>成功はtrue 、失敗はfalse</returns>
        public bool UpdateNoticeGroupData(long noticeNo, Nullable<bool> pushSend, Nullable<bool> mailSend, List<string[]> noticeId)
        {
           QhNoticeGroupEntityReaderArgs readerArgs = new QhNoticeGroupEntityReaderArgs() { Data = new List<QH_NOTICEGROUP_DAT>() { new QH_NOTICEGROUP_DAT { NOTICENO = noticeNo } } };
           QhNoticeGroupEntityReaderResults readerResults = QsDbManager.Read(new QhNoticeGroupEntityReader(), readerArgs);
            if (readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
            {
                QH_NOTICEGROUP_DAT entity = readerResults.Result.FirstOrDefault();
                if (pushSend.HasValue)
                    entity.PUSHSENDFLAG = (bool)pushSend;
                if (mailSend.HasValue)
                    entity.MAILSENDFLAG = (bool)mailSend;
                var serialiser = new QsJsonSerializer();
                if (noticeId != null && noticeId.Any())
                {
                    QhNoticeDataSetOfJson json = serialiser.Deserialize<QhNoticeDataSetOfJson>(entity.NOTICEDATASET);
                    if (json == null)
                        json = new QhNoticeDataSetOfJson() { AttachedFileN = new List<QhAttachedFileOfJson>(), LinkN = new List<QhNoticeLinkItemOfJson>(), PushSendN = new List<QhNoticePushSendItemOfJson>() };
                    foreach (var item in noticeId)
                    {
                        foreach (var item2 in item)
                        {
                            if (!string.IsNullOrEmpty(item2))
                                json.PushSendN.Add(new QhNoticePushSendItemOfJson() { PushDate = entity.STARTDATE.ToApiDateString(), PushId = item2 });
                        }
                    }
                    entity.NOTICEDATASET = serialiser.Serialize(json);
                }
                entity.UPDATEDDATE = DateTime.Now;
               QhNoticeGroupEntityWriterArgs writerArgs = new QhNoticeGroupEntityWriterArgs() { Data = new List<QH_NOTICEGROUP_DAT>() { entity } };
               QhNoticeGroupEntityWriterResults writerResults = QsDbManager.Write(new QhNoticeGroupEntityWriter(), writerArgs);
                return writerResults.IsSuccess;
            }

            return false;

        }

        /// <summary>
        /// グループへのお知らせ対象者 管理 データ テーブル へ 対象者を Balk Insert します
        /// </summary>
        /// <param name="noticeno">お知らせ番号</param>
        /// <param name="accountkeyList">対象者アカウントキーのリスト</param>
        /// <param name="now">更新日</param>
        /// <returns>成功はtrue 、失敗はfalse</returns>
        public bool WriteGroupTarget(long noticeno, List<(Guid acc, string[] pushid)> sendResult, DateTime pushDate, DateTime now)
        {
            List<QH_NOTICEGROUPTARGET_DAT> list = new List<QH_NOTICEGROUPTARGET_DAT>();

            foreach (var item in sendResult)
            {
                var json = new QhNoticeDataSetOfJson() { PushSendN = new List<QhNoticePushSendItemOfJson>()};
                if (item.pushid.Any())
                {
                    json.PushSendN.AddRange(item.pushid.Select(i => new QhNoticePushSendItemOfJson() { PushId = i, PushDate = pushDate.ToApiDateString()}));
                }

                var entity = new QH_NOTICEGROUPTARGET_DAT()
                {
                    ACCOUNTKEY = item.acc,
                    NOTICENO = noticeno,
                    ALREADYREADFLAG = false,
                    ALREADYREADDATE = DateTime.MinValue,
                    NOTICEDATASET = new QsJsonSerializer().Serialize(json),
                    DELETEFLAG = false,
                    CREATEDDATE = now,
                    UPDATEDDATE = now
                };

                list.Add(entity);
            }

            var write = new NoticeGroupTargetWriter();
            var writeArgs = new NoticeGroupTargetWriterArgs() { Entities = list };

            var result = QsDbManager.Write(write, writeArgs);

            return result.IsSuccess && result.Result == list.Count();
        }

        /// <summary>
        /// グループへのお知らせ対象者 管理 データ テーブル の既読状態を更新します
        /// </summary>
        /// <param name="noticeno">お知らせ番号</param>
        /// <param name="accountkey">対象者アカウントキー</param>
        /// <param name="alreadyReadFlag">既読フラグ</param>
        /// <param name="alreadyReadDate">既読日時</param>
        /// <param name="updatedDate">更新日</param>
        /// <returns>成功はtrue 、失敗はfalse</returns>
        public bool UpdateNoticeGroupTarget(long noticeno, Guid accountkey, bool alreadyReadFlag, DateTime alreadyReadDate, DateTime updatedDate)
        {
            var writer = new DbNoticeGroupTargetWriterCore();
            return writer.UpdateNoticeGroupTarget(noticeno, accountkey, alreadyReadFlag, alreadyReadDate, updatedDate);
        }

    }
}
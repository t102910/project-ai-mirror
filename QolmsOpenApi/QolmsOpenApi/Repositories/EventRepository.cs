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
    /// Event関連テーブルへの入出力インターフェース
    /// </summary>
    public interface IEventRepository
    {

        /// <summary>
        /// 日記の投稿数を取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_EVENT_DIARY_POST_VIEW GetDiaryPostCount(Guid accountKey);

        /// <summary>
        /// QH_EVENT_DATにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        void InsertEntity(QH_EVENT_DAT entity);
        /// <summary>
        /// QH_EVENT_DATのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEntity(QH_EVENT_DAT entity);
        /// <summary>
        /// QH_EVENT_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity(QH_EVENT_DAT entity);
        /// <summary>
        /// QH_EVENT_DATのレコードを物理削除します。
        /// </summary>
        /// <param name="entity"></param>
        void PhysicalDeleteEntity(QH_EVENT_DAT entity);

        /// <summary>
        /// 対象のいいね数を取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_EVENTREACTION_LIKE_VIEW GetLikeCount(Guid accountKey);

        /// <summary>
        /// QH_EVENTREACTION_DATにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        void InsertReactionEntity(QH_EVENTREACTION_DAT entity);
        /// <summary>
        /// QH_EVENTREACTION_DATのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateReactionEntity(QH_EVENTREACTION_DAT entity);
        /// <summary>
        /// QH_EVENTREACTION_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        void DeleteReactionEntity(QH_EVENTREACTION_DAT entity);
        /// <summary>
        /// QH_EVENTREACTION_DATのレコードを物理削除します。
        /// </summary>
        /// <param name="entity"></param>
        void PhysicalDeleteReactionEntity(QH_EVENTREACTION_DAT entity);
    }

    /// <summary>
    /// Event関連テーブルの入出力実装
    /// </summary>
    public class EventRepository: QsDbReaderBase, IEventRepository
    {
        /// <inheritdoc/>
        public QH_EVENT_DIARY_POST_VIEW GetDiaryPostCount(Guid accountKey)
        {
            var today = DateTime.Today;
            using (var con = QsDbManager.CreateDbConnection<QH_EVENT_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", today),
                    CreateParameter(con,"@p3", today.AddDays(-1)),
                    CreateParameter(con,"@p4", today.AddDays(-2)),
                    CreateParameter(con,"@p5", today.AddDays(-3)),
                    CreateParameter(con,"@p6", today.AddDays(-6)),
                    CreateParameter(con,"@p7", nameof(QhQolmsDiaryEventSetOfJson))
                };

                var sql = $@"
                    SELECT 
                        COALESCE(SUM(CASE WHEN CONVERT(date, EVENTDATE) = @p2 THEN 1 ELSE 0 END),0) As TODAY,
                        COALESCE(SUM(CASE WHEN CONVERT(date, EVENTDATE) BETWEEN @p4 AND @p3 THEN 1 ELSE 0 END),0) As DAY1TO2,
                        COALESCE(SUM(CASE WHEN CONVERT(date, EVENTDATE) BETWEEN @p6 AND @p5 THEN 1 ELSE 0 END),0) As DAY3TO6
                    FROM QH_EVENT_DAT
                    WHERE ACCOUNTKEY = @p1
                    AND EVENTDATE >= @p6
                    AND EVENTSETTYPENAME = @p7
                    AND DELETEFLAG = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_EVENT_DIARY_POST_VIEW>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                return entity;
            }
        }

        /// <inheritdoc/>
        public void InsertEntity(QH_EVENT_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new EventWriterArgs { Entity = entity };
            var result = new EventWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENT_DAT)}の挿入に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void UpdateEntity(QH_EVENT_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new EventWriterArgs { Entity = entity };
            var result = new EventWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENT_DAT)}の更新に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void DeleteEntity(QH_EVENT_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new EventWriterArgs { Entity = entity };
            var result = new EventWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENT_DAT)}の削除に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void PhysicalDeleteEntity(QH_EVENT_DAT entity)
        {

            var args = new EventWriterArgs
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new EventWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENT_DAT)}の物理削除に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public QH_EVENTREACTION_LIKE_VIEW GetLikeCount(Guid accountKey)
        {
            var today = DateTime.Today;
            using (var con = QsDbManager.CreateDbConnection<QH_EVENTREACTION_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", today),
                    CreateParameter(con,"@p3", QsDbReactionTypeEnum.Good)
                };

                var sql = $@"
                    SELECT 
                        COALESCE(SUM(CASE WHEN CONVERT(date, EVENTDATE) <= @p2 THEN 1 ELSE 0 END),0) As TOTAL,
                        COALESCE(SUM(CASE WHEN CONVERT(date, EVENTDATE) = @p2 THEN 1 ELSE 0 END),0) As TODAY                
                    FROM QH_EVENTREACTION_DAT
                    WHERE ACCOUNTKEY = @p1
                    AND REACTIONTYPE = @p3
                    AND DELETEFLAG = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_EVENTREACTION_LIKE_VIEW>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                return entity;
            }
        }

        /// <inheritdoc/>
        public void InsertReactionEntity(QH_EVENTREACTION_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new EventReactionWriterArgs { Entity = entity };
            var result = new EventReactionWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENTREACTION_DAT)}の挿入に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void UpdateReactionEntity(QH_EVENTREACTION_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new EventReactionWriterArgs { Entity = entity };
            var result = new EventReactionWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENTREACTION_DAT)}の更新に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void DeleteReactionEntity(QH_EVENTREACTION_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new EventReactionWriterArgs { Entity = entity };
            var result = new EventReactionWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENTREACTION_DAT)}の削除に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void PhysicalDeleteReactionEntity(QH_EVENTREACTION_DAT entity)
        {            

            var args = new EventReactionWriterArgs
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new EventReactionWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EVENTREACTION_DAT)}の物理削除に失敗しました。");
            }
        }
    }
}
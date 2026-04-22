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
    /// QH_SYMPTOM_DATの入出力
    /// </summary>
    public interface ISymptomRepository
    {
        /// <summary>
        /// 主キーを指定してQH_SYMPTOM_DATのレコードを1件取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        QH_SYMPTOM_DAT ReadEntity(Guid id);
        /// <summary>
        /// QH_SYMPTOM_DATにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        void InsertEntity(QH_SYMPTOM_DAT entity);
        /// <summary>
        /// QH_SYMPTOM_DATのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEntity(QH_SYMPTOM_DAT entity);
        /// <summary>
        /// QH_SYMPTOM_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity(QH_SYMPTOM_DAT entity);
        /// <summary>
        /// QH_SYMPTOM_DATのレコードを物理削除します。
        /// </summary>
        /// <param name="id"></param>
        void PhysicalDeleteEntity(Guid id);
        /// <summary>
        /// QH_SYMPTOM_DATのレコードを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="offset"></param>
        /// <param name="fetch"></param>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        List<QH_SYMPTOM_DAT> ReadEntities(Guid accountKey, DateTime fromDate, DateTime toDate, int offset, int fetch, int linkageSystemNo);
    }


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class SymptomRepository: QsDbReaderBase, ISymptomRepository
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public QH_SYMPTOM_DAT ReadEntity(Guid id)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_SYMPTOM_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", id),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_SYMPTOM_DAT)}
                    WHERE {nameof(QH_SYMPTOM_DAT.ID)} = @p1
                    AND   {nameof(QH_SYMPTOM_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_SYMPTOM_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                if (entity != null)
                {
                    entity.OTHERDETAIL = entity.OTHERDETAIL.TryDecrypt();
                    entity.MEMO = entity.MEMO.TryDecrypt();
                }

                return entity;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void InsertEntity(QH_SYMPTOM_DAT entity)
        {
            entity.OTHERDETAIL = entity.OTHERDETAIL.TryEncrypt();
            entity.MEMO = entity.MEMO.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new SymptomWriterArgs { Entity = entity };
            var result = new SymptomWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SYMPTOM_DAT)}の挿入に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void UpdateEntity(QH_SYMPTOM_DAT entity)
        {
            entity.OTHERDETAIL = entity.OTHERDETAIL.TryEncrypt();
            entity.MEMO = entity.MEMO.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new SymptomWriterArgs { Entity = entity };
            var result = new SymptomWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SYMPTOM_DAT)}の更新に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void DeleteEntity(QH_SYMPTOM_DAT entity)
        {
            entity.OTHERDETAIL = entity.OTHERDETAIL.TryEncrypt();
            entity.MEMO = entity.MEMO.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new SymptomWriterArgs { Entity = entity };
            var result = new SymptomWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SYMPTOM_DAT)}の削除に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void PhysicalDeleteEntity(Guid id)
        {
            var entity = new QH_SYMPTOM_DAT
            {
                ID = id
            };

            var args = new SymptomWriterArgs
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new SymptomWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SYMPTOM_DAT)}の物理削除に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<QH_SYMPTOM_DAT> ReadEntities(Guid accountKey, DateTime fromDate, DateTime toDate, int offset, int fetch, int linkageSystemNo)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_SYMPTOM_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", fromDate),
                    CreateParameter(con, "@p3", toDate),
                    CreateParameter(con, "@p4", offset),
                    CreateParameter(con, "@p5", fetch),
                    CreateParameter(con, "@p6", linkageSystemNo)
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_SYMPTOM_DAT)}
                    WHERE {nameof(QH_SYMPTOM_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_SYMPTOM_DAT.RECORDDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_SYMPTOM_DAT.LINKAGESYSTEMNO)} = @p6
                    AND   {nameof(QH_SYMPTOM_DAT.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_SYMPTOM_DAT.RECORDDATE)} DESC
                    OFFSET @p4 ROWS 
                    FETCH NEXT @p5 ROWS ONLY
                ";

                con.Open();

                return ExecuteReader<QH_SYMPTOM_DAT>(con, null, sql, paramList);
            }
        }        
    }
}
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
    /// アプリ設定テーブルの入出力インターフェース
    /// </summary>
    public interface IAppSettingsRepository
    {
        /// <summary>
        /// QH_APPSETTINGS_DATのレコードを1件取得
        /// VALUEは復号化されます。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="appType"></param>
        /// <returns></returns>
        QH_APPSETTINGS_DAT ReadEntity(Guid accountKey, int appType);
        /// <summary>
        /// QH_APPSETTINGS_DATにレコードを挿入します。
        /// VALUEは暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void InsertEntity(QH_APPSETTINGS_DAT entity);
        /// <summary>
        /// QH_APPSETTINGS_DATのレコードを更新します。
        /// VALUEは暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEntity(QH_APPSETTINGS_DAT entity);
        /// <summary>
        /// QH_APPSETTINGS_DATのレコードを削除します。
        /// VALUEは暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity(QH_APPSETTINGS_DAT entity);
        /// <summary>
        /// QH_APPSETTINGS_DATのレコードを物理削除します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="appType"></param>
        void PhysicalDeleteEntity(Guid accountKey, int appType);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class AppSettingsRepository: QsDbReaderBase, IAppSettingsRepository
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="appType"></param>
        /// <returns></returns>
        public QH_APPSETTINGS_DAT ReadEntity(Guid accountKey, int appType)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_APPSETTINGS_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", appType)
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_APPSETTINGS_DAT)}
                    WHERE {nameof(QH_APPSETTINGS_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_APPSETTINGS_DAT.APPTYPE)} = @p2
                    AND   {nameof(QH_APPSETTINGS_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_APPSETTINGS_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                if (entity != null)
                {
                    entity.VALUE = entity.VALUE.TryDecrypt();
                }

                return entity;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        public void InsertEntity(QH_APPSETTINGS_DAT entity)
        {
            entity.VALUE = entity.VALUE.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new AppSettingsWriterArgs { Entity = entity };
            var result = new AppSettingsWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_APPSETTINGS_DAT)}の挿入に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEntity(QH_APPSETTINGS_DAT entity)
        {
            entity.VALUE = entity.VALUE.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new AppSettingsWriterArgs { Entity = entity };
            var result = new AppSettingsWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_APPSETTINGS_DAT)}の更新に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEntity(QH_APPSETTINGS_DAT entity)
        {
            entity.VALUE = entity.VALUE.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new AppSettingsWriterArgs { Entity = entity };
            var result = new AppSettingsWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_APPSETTINGS_DAT)}の削除に失敗しました。");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void PhysicalDeleteEntity(Guid accountKey, int appType)
        {
            var entity = new QH_APPSETTINGS_DAT
            {
                ACCOUNTKEY = accountKey,
                APPTYPE = appType
            };

            var args = new AppSettingsWriterArgs
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new AppSettingsWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_APPSETTINGS_DAT)}の物理削除に失敗しました。");
            }
        }
    }
}
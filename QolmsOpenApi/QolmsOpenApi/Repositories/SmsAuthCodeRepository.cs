using MGF.QOLMS.QolmsCryptV1;
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
    /// SMS認証コード管理の入出力インターフェース
    /// </summary>
    public interface ISmsAuthCodeRepository
    {
        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを1件取得します。
        /// 認証コードは復号化されます。
        /// </summary>
        /// <param name="authKey"></param>
        /// <returns></returns>
        QH_SMSAUTHCODE_DAT ReadEntity(Guid authKey);
        /// <summary>
        /// QH_SMSAUTHCODE_DATにレコードを挿入します。
        /// 認証コードは暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void InsertEntity(QH_SMSAUTHCODE_DAT entity);
        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを更新します。
        /// 認証コードは暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEntity(QH_SMSAUTHCODE_DAT entity);
        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity(QH_SMSAUTHCODE_DAT entity);
        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを物理削除します。
        /// </summary>
        /// <param name="authKey"></param>
        void PhysicalDeleteEntity(Guid authKey);
    }

    /// <summary>
    /// SMS認証コード管理の入出力処理
    /// </summary>
    public class SmsAuthCodeRepository: QsDbReaderBase, ISmsAuthCodeRepository
    {
        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを1件取得します。
        /// </summary>
        /// <param name="authKey"></param>
        /// <returns></returns>
        public QH_SMSAUTHCODE_DAT ReadEntity(Guid authKey)
        {
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            using (var con = QsDbManager.CreateDbConnection<QH_SMSAUTHCODE_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", authKey),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_SMSAUTHCODE_DAT)}
                    WHERE {nameof(QH_SMSAUTHCODE_DAT.AUTHKEY)} = @p1
                    AND   {nameof(QH_SMSAUTHCODE_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_SMSAUTHCODE_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                TryDecrypt(entity);

                return entity;
            }
        }

        /// <summary>
        /// QH_SMSAUTHCODE_DATにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        public void InsertEntity(QH_SMSAUTHCODE_DAT entity)
        {
            TryEncrypt(entity);

            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new SmsAuthCodeWriterArgs { Entity = entity };
            var result = new SmsAuthCodeWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SMSAUTHCODE_DAT)}の挿入に失敗しました。");
            }
        }

        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEntity(QH_SMSAUTHCODE_DAT entity)
        {
            TryEncrypt(entity);

            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new SmsAuthCodeWriterArgs { Entity = entity };
            var result = new SmsAuthCodeWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SMSAUTHCODE_DAT)}の更新に失敗しました。");
            }
        }

        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEntity(QH_SMSAUTHCODE_DAT entity)
        {
            TryEncrypt(entity);

            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new SmsAuthCodeWriterArgs { Entity = entity };
            var result = new SmsAuthCodeWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SMSAUTHCODE_DAT)}の削除に失敗しました。");
            }
        }

        /// <summary>
        /// QH_SMSAUTHCODE_DATのレコードを物理削除します。
        /// </summary>
        /// <param name="authKey"></param>
        public void PhysicalDeleteEntity(Guid authKey)
        {
            var entity = new QH_SMSAUTHCODE_DAT
            {
                AUTHKEY = authKey
            };

            var args = new SmsAuthCodeWriterArgs 
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new SmsAuthCodeWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_SMSAUTHCODE_DAT)}の物理削除に失敗しました。");
            }
        }

        void TryEncrypt(QH_SMSAUTHCODE_DAT entity)
        {
            try
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    entity.AUTHCODE = crypt.EncryptString(entity.AUTHCODE);
                }
            }
            catch
            {
                entity.AUTHCODE = string.Empty;
            }
        }

        void TryDecrypt(QH_SMSAUTHCODE_DAT entity)
        {
            try
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    entity.AUTHCODE = crypt.DecryptString(entity.AUTHCODE);
                }
            }
            catch
            {
                entity.AUTHCODE = string.Empty;
            }
        }
    }
}
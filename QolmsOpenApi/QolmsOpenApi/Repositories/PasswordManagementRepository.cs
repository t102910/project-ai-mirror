using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// パスワード管理テーブル QH_PASSWORDMANAGEMENT_DAT への入出力インターフェース
    /// </summary>
    public interface IPasswordManagementRepository
    {
        /// <summary>
        /// QH_PASSWORDMANAGEMENT_DATのレコードを1件取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_PASSWORDMANAGEMENT_DAT ReadEntity(Guid accountKey);

        /// <summary>
        /// QH_PASSWORDMANAGEMENT_DATのレコードを1件取得します。
        /// 暗号化部分は解除されて返されます。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_PASSWORDMANAGEMENT_DAT ReadDecryptedEntity(Guid accountKey);

        /// <summary>
        /// パスワードを変更します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        void EditPassword(Guid accountKey, string newPassword);
    }

    /// <summary>
    /// パスワード管理テーブル QH_PASSWORDMANAGEMENT_DAT への入出力実装
    /// </summary>
    public class PasswordManagementRepository : QsDbReaderBase, IPasswordManagementRepository
    {
        /// <summary>
        /// QH_PASSWORDMANAGEMENT_DATのレコードを1件取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public QH_PASSWORDMANAGEMENT_DAT ReadEntity(Guid accountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_PASSWORDMANAGEMENT_DAT>())
            {                
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_PASSWORDMANAGEMENT_DAT)}
                    WHERE {nameof(QH_PASSWORDMANAGEMENT_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_PASSWORDMANAGEMENT_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_PASSWORDMANAGEMENT_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();                

                return entity;
            }
        }

        /// <summary>
        /// QH_PASSWORDMANAGEMENT_DATのレコードを1件取得します。
        /// 暗号化部分は解除されて返されます。
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public QH_PASSWORDMANAGEMENT_DAT ReadDecryptedEntity(Guid accountKey)
        {
            var entity = ReadEntity(accountKey);

            if(entity == null)
            {
                return null;
            }

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                entity.USERID = crypt.DecryptString(entity.USERID);
                entity.USERPASSWORD = crypt.DecryptString(entity.USERPASSWORD);
                if (!string.IsNullOrEmpty(entity.PASSWORDRECOVERYSET))
                {
                    entity.PASSWORDRECOVERYSET = crypt.DecryptString(entity.PASSWORDRECOVERYSET);
                }
                if (!string.IsNullOrEmpty(entity.PASSWORDRECOVERYMAILADDRESS))
                {
                    entity.PASSWORDRECOVERYMAILADDRESS = crypt.DecryptString(entity.PASSWORDRECOVERYMAILADDRESS);
                }                
            }

            return entity;
        }

        /// <summary>
        /// パスワードを変更します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        /// <remarks>QoPasswordManagementより移植</remarks>
        public void EditPassword(Guid accountKey, string newPassword)
        {
            bool result = false;
            DbAccountInformationPasswordWriter<QH_PASSWORDMANAGEMENT_DAT> writer = new DbAccountInformationPasswordWriter<QH_PASSWORDMANAGEMENT_DAT>();
            DbAccountInformationPasswordWriterArgs writerArgs = new DbAccountInformationPasswordWriterArgs()
            {
                AuthorKey = accountKey,
                Password = newPassword
            };
            DbAccountInformationPasswordWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

            if (writerResults != null)
            {
                result = writerResults.IsSuccess && writerResults.Result == 1;
            }

            if (!result)
            {
                throw new InvalidOperationException($"{nameof(QH_PASSWORDMANAGEMENT_DAT)}の更新に失敗しました。");
            }
        }
    }
}
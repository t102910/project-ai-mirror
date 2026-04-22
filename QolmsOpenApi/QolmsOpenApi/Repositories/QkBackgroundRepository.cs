using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// Kagamino背景情報入出力インターフェース
    /// </summary>
    public interface IQkBackgroundRepository
    {
        /// <summary>
        /// 背景情報リストを取得
        /// </summary>
        /// <returns></returns>
        List<QK_BACKGROUND_MST> ReadList();

        /// <summary>
        /// 背景情報を取得
        /// </summary>
        /// <param name="backgroundId">背景ID</param>
        /// <returns></returns>
        QK_BACKGROUND_MST Read(string backgroundId);

        /// <summary>
        /// 背景ファイルを取得
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        string ReadFile(Guid fileKey);
    }

    /// <summary>
    /// Kagaminoモデル入出力実装
    /// </summary>
    public class QkBackgroundRepository : QsDbReaderBase, IQkBackgroundRepository
    {
        /// <summary>
        /// 背景情報リストを取得
        /// </summary>
        /// <returns></returns>
        public List<QK_BACKGROUND_MST> ReadList()
        {
            using (var con = QsDbManager.CreateDbConnection<QK_BACKGROUND_MST>())
            {
                var builder = new StringBuilder();
                builder.AppendLine($@"
                    Select *
                    From {nameof(QK_BACKGROUND_MST)}
                    WHERE DELETEFLAG = 0");                

                builder.AppendLine(@"ORDER BY SORTORDER");

                con.Open();

                return ExecuteReader<QK_BACKGROUND_MST>(con, null, builder.ToString(), null);
            }
        }

        /// <summary>
        /// 背景情報を取得
        /// </summary>
        /// <param name="backgroundId"></param>
        /// <returns></returns>
        public QK_BACKGROUND_MST Read(string backgroundId)
        {
            using (var con = QsDbManager.CreateDbConnection<QK_BACKGROUND_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", backgroundId)
                };

                // クライアントから削除されたかどうかを判定できるように
                // 削除フラグでは絞り込まない
                var sql = $@"
                    Select *
                    From {nameof(QK_BACKGROUND_MST)}
                    WHERE {nameof(QK_BACKGROUND_MST.BACKGROUNDID)} = @p1
                ";

                con.Open();

                var result = ExecuteReader<QK_BACKGROUND_MST>(con, null, sql, paramList);
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// 背景ファイルを取得
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        public string ReadFile(Guid fileKey)
        {
            var args = new UploadOriginalFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var reader = new UploadOriginalFileBlobEntityReader<QhUploadOriginalFileBlobEntity>();
            var results = reader.Execute(args);

            if (!results.IsSuccess)
            {
                throw new Exception("ReadFile failed");
            }

            return Convert.ToBase64String(results.Result.Data);
        }
    }
}
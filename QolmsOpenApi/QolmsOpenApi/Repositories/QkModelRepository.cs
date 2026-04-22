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
    /// Kagaminoモデル情報入出力インターフェース
    /// </summary>
    public interface IQkModelRepository
    {
        /// <summary>
        /// モデル情報リストを取得
        /// </summary>
        /// <param name="containsPaid">有料アイテムを含めるかどうか</param>
        /// <returns></returns>
        List<QK_MODEL_MST> ReadList(bool containsPaid);

        /// <summary>
        /// モデル情報を取得
        /// </summary>
        /// <param name="modelId">モデルID</param>
        /// <returns></returns>
        QK_MODEL_MST Read(string modelId);

        /// <summary>
        /// モデルファイルを取得
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        string ReadFile(Guid fileKey);
    }

    /// <summary>
    /// Kagaminoモデル入出力実装
    /// </summary>
    public class QkModelRepository: QsDbReaderBase, IQkModelRepository
    {
        /// <summary>
        /// モデル情報リストを取得
        /// </summary>
        /// <param name="containsPaid"></param>
        /// <returns></returns>
        public List<QK_MODEL_MST> ReadList(bool containsPaid)
        {
            using(var con = QsDbManager.CreateDbConnection<QK_MODEL_MST>())
            {
                var builder = new StringBuilder();
                builder.AppendLine($@"
                    Select *
                    From {nameof(QK_MODEL_MST)}
                    WHERE DELETEFLAG = 0");

                if (!containsPaid)
                {
                    builder.AppendLine("And ISPAID = 0");
                }

                builder.AppendLine(@"ORDER BY SORTORDER");

                con.Open();

                return ExecuteReader<QK_MODEL_MST>(con, null, builder.ToString(), null);
            }            
        }

        /// <summary>
        /// モデル情報を取得
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public QK_MODEL_MST Read(string modelId)
        {
            using (var con = QsDbManager.CreateDbConnection<QK_MODEL_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", modelId)
                };

                // クライアントから削除されたかどうかを判定できるように
                // 削除フラグでは絞り込まない
                var sql = $@"
                    Select *
                    From {nameof(QK_MODEL_MST)}
                    WHERE MODELID = @p1
                ";

                con.Open();

                var result = ExecuteReader<QK_MODEL_MST>(con, null, sql, paramList);
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// モデルファイルを取得
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
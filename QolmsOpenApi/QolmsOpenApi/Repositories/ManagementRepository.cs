using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.AzureStorage.UploadCheckupPostingFile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{

    /// <summary>
    /// マネジメント関連入出力インターフェース
    /// </summary>
    public interface IManagementRepository
    {
        /// <summary>
        /// データ投稿（健診ファイル）を ブロブストレージへ登録します。
        /// </summary>
        /// <returns></returns>
        Guid UproadPostingFileBlob(string contentType, byte[] data, string contentEncoding, string originalFileName, int linkageSystemNo, Guid createdAccountKey);

        /// <summary>
        /// 指定したファイルキーのファイル情報を ブロブストレージから削除します。
        /// </summary>
        /// <returns></returns>
        bool DeletePostingFileBlob(Guid fileKey);

        /// <summary>
        /// データ投稿（健診ファイル）情報をDBへ新規登録します。
        /// </summary>
        /// <returns></returns>
        bool InsertPostingFileDb(int linkageSystemNo, Guid fileKey, string originalName, string contentType, byte statusType, string failureReason, bool deleteFlag, DateTime createdDate, Guid createdAccountKey);
    
    }

    /// <summary>
    /// マネジメント関連の入出力実装
    /// </summary>
    public class ManagementRepository : QsDbWriterBase, IManagementRepository
    {
        /// <summary>
        /// データ投稿（健診ファイル）を ブロブストレージへ登録します。
        /// </summary>
        /// <param name="contentType">コンテンツタイプ</param>
        /// <param name="data">ファイル情報のバイト配列</param>
        /// <param name="contentEncoding">コンテンツエンコーディング</param>
        /// <param name="originalFileName">オリジナルファイル名</param>
        /// <param name="linkageSystemNo">連携システム番号</param>
        /// <param name="createdAccountKey">作成者アカウントキー</param>
        /// <returns>
        /// 成功した場合、ファイルキーを
        /// 失敗した場合、Guid.Emptyを返します。
        /// </returns>
        /// <remarks></remarks>
        public Guid UproadPostingFileBlob(string contentType, byte[] data, string contentEncoding, string originalFileName, int linkageSystemNo, Guid createdAccountKey)
        {
            Guid result = Guid.Empty;

            try
            {
                var writer = new UploadCheckupPostingFileBlobEntityWriter();
                var writerArgs = new UploadCheckupPostingFileBlobEntityWriterArgs()
                {
                    Entity = new QmUploadCheckupPostingFileBlobEntity()
                    {
                        Name = Guid.Empty,
                        ContentType = contentType,
                        ContentEncoding = contentEncoding,
                        Data = data,
                        LinkageSystemNo = linkageSystemNo.ToString(),
                        CreatedAccountKey = createdAccountKey.ToApiGuidString(),
                        OriginalName = originalFileName
                    }
                };

                var writerResults = QsAzureStorageManager.Write(writer, writerArgs);

                if (writerResults.IsSuccess)
                    result = writerResults.Result;
            }
            catch
            {
                result = Guid.Empty;
            }

            return result;
        }

        /// <summary>
        /// 指定したファイルキーのファイル情報を ブロブストレージから削除します。
        /// </summary>
        /// <param name="fileKey">ファイルキー</param>
        /// <returns>
        /// 削除に成功した場合、Trueを
        /// 失敗した場合は Falseを返します。
        /// </returns>
        public bool DeletePostingFileBlob(Guid fileKey) 
        {
            bool result = false;

            try
            {
                var deleter = new UploadCheckupPostingFileBlobEntityDeleter();
                var deleterArgs = new UploadCheckupPostingFileBlobEntityDeleterArgs()
                {
                    Entity = new QmUploadCheckupPostingFileBlobEntity()
                    {
                        Name = fileKey
                    }
                };

                var deleteResults = QsAzureStorageManager.Write(deleter, deleterArgs);

                result = deleteResults.IsSuccess;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// データ投稿（健診ファイル）情報をDBへ新規登録します。
        /// </summary>
        /// <param name="linkageSystemNo">連携システム番号</param>
        /// <param name="fileKey">ファイルキー</param>
        /// <param name="originalName">オリジナルファイル名</param>
        /// <param name="contentType">MIMEタイプ</param>
        /// <param name="statusType">処理状態</param>
        /// <param name="failureReason">失敗理由</param>
        /// <param name="deleteFlag">削除フラグ</param>
        /// <param name="createdDate">作成日時</param>
        /// <param name="createdAccountKey">作成者アカウントキー</param>
        /// <returns>
        /// 処理結果
        /// </returns>
        public bool InsertPostingFileDb(int linkageSystemNo, Guid fileKey, string originalName, string contentType, byte statusType, string failureReason, bool deleteFlag, DateTime createdDate, Guid createdAccountKey)
        {
            bool result = false;

            using (var connection = QsDbManager.CreateDbConnection<QM_CHECKUPPOSTINGFILE_DAT>())
            {
                var query = new StringBuilder();
                var @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", linkageSystemNo),
                this.CreateParameter(connection, "@p2", fileKey),
                this.CreateParameter(connection, "@p3", originalName),
                this.CreateParameter(connection, "@p4", contentType),
                this.CreateParameter(connection, "@p5", statusType),
                this.CreateParameter(connection, "@p6", failureReason),
                this.CreateParameter(connection, "@p7", deleteFlag),
                this.CreateParameter(connection, "@p8", createdDate),
                this.CreateParameter(connection, "@p9", createdAccountKey),
                this.CreateParameter(connection, "@p10", createdDate),
                this.CreateParameter(connection, "@p11", createdAccountKey)
            };

                // クエリ作成
                query.Append("insert into qm_checkuppostingfile_dat");
                query.Append(" (linkagesystemno, fileno, filekey, originalname, contenttype, statustype,");
                query.Append("  failurereason, deleteflag, createddate, createdaccountkey, updateddate, updatedaccountkey)");
                query.Append(" values (");
                query.Append(" @p1,");
                query.Append(" (select isnull(max(fileno), 0) + 1 from qm_checkuppostingfile_dat),");
                query.Append(" @p2,");
                query.Append(" @p3,");
                query.Append(" @p4,");
                query.Append(" @p5,");
                query.Append(" @p6,");
                query.Append(" @p7,");
                query.Append(" @p8,");
                query.Append(" @p9,");
                query.Append(" @p10,");
                query.Append(" @p11");
                query.Append(" )");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリ実行
                if (this.ExecuteNonQuery(connection, default, this.CreateCommandText(connection, query.ToString()), @params) == 1)
                    result = true;
            }

            return result;
        }

    }
}
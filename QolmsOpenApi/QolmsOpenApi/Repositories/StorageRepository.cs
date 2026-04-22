using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// Storageへの入出力インターフェース
    /// </summary>
    public interface IStorageRepository
    {
        /// <summary>
        /// 画像のBLOBストレージのメタデータのアカウントキーを書き換える
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        bool UpdateImageMetaDataAccountKey(Guid fileKey, Guid accountKey);

        /// <summary>
        /// 指定ファイルをDBとStorageから削除する
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        bool DeleteFile(Guid authorKey, Guid fileKey);

        /// <summary>
        /// ファイルの書き込み
        /// </summary>
        /// <param name="actorKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="fileItem"></param>
        /// <returns></returns>
        Guid WriteFile(Guid actorKey, Guid authorKey, QoApiFileItem fileItem);

        /// <summary>
        /// 画像の書き込み
        /// </summary>
        /// <param name="actorKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="fileItem"></param>
        /// <returns></returns>
        Guid WriteImage(Guid actorKey, Guid authorKey, QoApiFileItem fileItem);

        /// <summary>
        /// ファイルの読み込み
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        QoApiFileItem ReadFile(Guid fileKey);

        /// <summary>
        /// 画像の読み込み
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        QoApiFileItem ReadImage(Guid fileKey, QsApiFileTypeEnum fileType);

        /// <summary>
        /// QH_UPLOADFILE_DATのレコードを取得する
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        QH_UPLOADFILE_DAT ReadFileEntity(Guid fileKey);
    }

    /// <summary>
    /// Storageへの入出力実装
    /// </summary>
    public class StorageRepository: QsDbReaderBase, IStorageRepository
    {
        /// <inheritdoc/>
        public bool UpdateImageMetaDataAccountKey(Guid fileKey, Guid accountKey)
        {
            return QoBlobStorage.UpdateImageMetaDataAccountKey(fileKey, accountKey);
        }

        /// <inheritdoc/>
        public QoApiFileItem ReadFile(Guid fileKey)
        {
            var entity = ReadFileEntity(fileKey);
            if (entity == null)
            {
                throw new Exception("ファイルが見つかりませんでした。");
            }
            if (entity.FILEKEY != fileKey)
            {
                throw new Exception("FileKeyが不整合です。");
            }

            var data = ReadOriginal(fileKey);

            return new QoApiFileItem
            {
                Data = data,
                ContentType = entity.CONTENTTYPE,
                OriginalName = entity.ORIGINALNAME,
            };
        }

        /// <inheritdoc/>
        public QoApiFileItem ReadImage(Guid fileKey, QsApiFileTypeEnum fileType)
        {
            var entity = ReadFileEntity(fileKey);
            if (entity == null)
            {
                throw new Exception("ファイルが見つかりませんでした。");
            }
            if (entity.FILEKEY != fileKey)
            {
                throw new Exception("FileKeyが不整合です。");
            }

            string data;
            switch (fileType)
            {
                case QsApiFileTypeEnum.Original:
                    data = ReadOriginal(fileKey);
                    break;
                case QsApiFileTypeEnum.Edited:
                    data = ReadEdited(fileKey);
                    break;
                case QsApiFileTypeEnum.Thumbnail:
                    data = ReadThumbnail(fileKey);
                    break;
                default:
                    throw new Exception("ファイルタイプが正しくありません。");
            }

            return new QoApiFileItem
            {
                Data = data,
                ContentType = entity.CONTENTTYPE,
                OriginalName = entity.ORIGINALNAME,             
            };
        }

        /// <inheritdoc/>
        public Guid WriteFile(Guid actorKey, Guid authorKey, QoApiFileItem fileItem)
        {
            // ファイルをストレージに登録
            var data = Convert.FromBase64String(fileItem.Data);
            var fileKey = WriteOriginal(actorKey, fileItem.ContentType, fileItem.OriginalName, data);

            // ファイル情報をDBに保存
            var dbWriter = new UploadFileWriter();
            var dbArgs = new UploadFileWriterArgs
            {
                Entity = new QH_UPLOADFILE_DAT
                {
                    FILEKEY = fileKey,
                    CONTENTTYPE = fileItem.ContentType,
                    ORIGINALNAME = fileItem.OriginalName,
                    CREATEDACCOUNTKEY = authorKey,
                    UPDATEDACCOUNTKEY = authorKey,
                    DataState = QsDbEntityStateTypeEnum.Added
                }
            };
            var dbResults = QsDbManager.Write(dbWriter, dbArgs);
            if (!dbResults.IsSuccess || dbResults.Result != 1)
            {
                // Storageからはゴミが残らないように削除
                DeleteOriginal(fileKey);
                throw new Exception("ファイル情報のDB登録に失敗しました。");
            }

            return fileKey;
        }

        /// <inheritdoc/>
        public Guid WriteImage(Guid actorKey, Guid authorKey, QoApiFileItem fileItem)
        {
            var data = Convert.FromBase64String(fileItem.Data);
            // オリジナル画像をストレージに保存
            var fileKey = WriteOriginal(actorKey, fileItem.ContentType, fileItem.OriginalName, data);

            (var edited, var thumbnail) = QoBlobStorage.CreateImages(data);

            try
            {
                // 編集画像をストレージに保存
                var editedKey = WriteEdited(fileKey, actorKey, fileItem.ContentType, fileItem.OriginalName, edited);
                if (editedKey != fileKey)
                {
                    throw new Exception("編集画像の保存に失敗しました。");
                }

                // サムネイル画像をストレージに保存
                var thumbKey = WriteThumbnail(fileKey, actorKey, fileItem.ContentType, fileItem.OriginalName, thumbnail);
                if (thumbKey != fileKey)
                {
                    throw new Exception("サムネイル画像の保存に失敗しました。");
                }
            }
            catch (Exception ex)
            {
                DeleteAllFiles(fileKey);
                throw new Exception(ex.Message);
            }

            // ファイル情報をDBに保存
            var dbWriter = new UploadFileWriter();
            var dbArgs = new UploadFileWriterArgs
            {
                Entity = new QH_UPLOADFILE_DAT
                {
                    FILEKEY = fileKey,
                    CONTENTTYPE = fileItem.ContentType,
                    ORIGINALNAME = fileItem.OriginalName,
                    CREATEDACCOUNTKEY = authorKey,
                    UPDATEDACCOUNTKEY = authorKey,
                    DataState = QsDbEntityStateTypeEnum.Added
                }
            };
            var dbResults = QsDbManager.Write(dbWriter, dbArgs);
            if (!dbResults.IsSuccess || dbResults.Result != 1)
            {
                // Storageからはゴミが残らないように削除
                DeleteAllFiles(fileKey);
                throw new Exception("ファイル情報のDB登録に失敗しました。");
            }

            return fileKey;
        }

        /// <inheritdoc/>
        public bool DeleteFile(Guid authorKey, Guid fileKey)
        {
            var entity = ReadFileEntity(fileKey);
            if (entity == null)
            {
                throw new Exception("ファイルが見つかりませんでした。");
            }
            if (entity.FILEKEY != fileKey)
            {
                throw new Exception("FileKeyが不整合です。");
            }

            entity.DELETEFLAG = true;
            entity.UPDATEDACCOUNTKEY = authorKey;
            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            // ファイル情報を更新
            var dbWriter = new UploadFileWriter();
            var dbArgs = new UploadFileWriterArgs
            {
                Entity = entity,
            };
            var dbResults = QsDbManager.Write(dbWriter, dbArgs);

            if (!dbResults.IsSuccess || dbResults.Result != 1)
            {
                throw new Exception("ファイル情報のDB更新に失敗しました。");
            }

            // ストレージのファイルを削除
            return DeleteAllFiles(fileKey);
        }

        /// <inheritdoc/>
        public QH_UPLOADFILE_DAT ReadFileEntity(Guid fileKey)
        {
            var reader = new QhUploadFileEntityReader();
            var args = new QhUploadFileEntityReaderArgs
            {
                Data = new List<QH_UPLOADFILE_DAT>
                {
                    new QH_UPLOADFILE_DAT
                    {
                        FILEKEY = fileKey
                    }
                }
            };

            var results = QsDbManager.Read(reader, args);

            if (!results.IsSuccess)
            {
                throw new Exception("ファイル情報の取得に失敗しました");
            }

            if (results.Result.Count == 1 && !results.Result.First().DELETEFLAG)
            {
                return results.Result.First();
            }

            return null;
        }

        string ReadOriginal(Guid fileKey)
        {
            var args = new UploadOriginalFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var reader = new UploadOriginalFileBlobEntityReader<QhUploadOriginalFileBlobEntity>();
            var results = reader.Execute(args);

            if (results.IsSuccess && results.Result != null)
            {
                return Convert.ToBase64String(results.Result.Data);
            }

            throw new Exception("オリジナルファイルの読み込みに失敗しました。");
        }

        string ReadEdited(Guid fileKey)
        {
            var args = new UploadEditedFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var reader = new UploadEditedFileBlobEntityReader<QhUploadEditedFileBlobEntity>();
            var results = reader.Execute(args);

            if (results.IsSuccess && results.Result != null)
            {
                return Convert.ToBase64String(results.Result.Data);
            }

            throw new Exception("編集画像の読み込みに失敗しました。");
        }

        string ReadThumbnail(Guid fileKey)
        {
            var args = new UploadThumbnailFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var reader = new UploadThumbnailFileBlobEntityReader<QhUploadThumbnailFileBlobEntity>();
            var results = reader.Execute(args);

            if (results.IsSuccess && results.Result != null)
            {
                return Convert.ToBase64String(results.Result.Data);
            }

            throw new Exception("サムネイル画像の読み込みに失敗しました。");
        }

        

        Guid WriteOriginal(Guid accountKey, string contentType, string originalName, byte[] data)
        {
            // ファイルをストレージに登録
            var storageArgs = new UploadOriginalFileBlobEntityWriterArgs<QhUploadOriginalFileBlobEntity>
            {
                Entity = new QhUploadOriginalFileBlobEntity
                {
                    Name = Guid.Empty,
                    AccountKey = accountKey.ToApiGuidString(),
                    ContentType = contentType,
                    Data = data,
                    OriginalName = originalName,
                    ContentEncoding = string.Empty
                }
            };
            var storageWriter = new UploadOriginalFileBlobEntityWriter<QhUploadOriginalFileBlobEntity>();
            var storageResults = storageWriter.Execute(storageArgs);

            if (!storageResults.IsSuccess)
            {
                throw new Exception("オリジナルファイルのBlobStorageへの登録に失敗しました。");
            }

            return storageResults.Result;
        }

        Guid WriteEdited(Guid fileKey, Guid accountKey, string contentType, string originalName, byte[] data)
        {
            // ファイルをストレージに登録
            var storageArgs = new UploadEditedFileBlobEntityWriterArgs<QhUploadEditedFileBlobEntity>
            {
                Entity = new QhUploadEditedFileBlobEntity
                {
                    Name = fileKey,
                    AccountKey = accountKey.ToApiGuidString(),
                    ContentType = contentType,
                    Data = data,
                    OriginalName = originalName,
                    ContentEncoding = string.Empty
                }
            };
            var storageWriter = new UploadEditedFileBlobEntityWriter<QhUploadEditedFileBlobEntity>();
            var storageResults = storageWriter.Execute(storageArgs);

            if (!storageResults.IsSuccess)
            {
                throw new Exception("編集画像のBlobStorageへの登録に失敗しました。");
            }

            return storageResults.Result;
        }

        Guid WriteThumbnail(Guid fileKey, Guid accountKey, string contentType, string originalName, byte[] data)
        {
            // ファイルをストレージに登録
            var storageArgs = new UploadThumbnailFileBlobEntityWriterArgs<QhUploadThumbnailFileBlobEntity>
            {
                Entity = new QhUploadThumbnailFileBlobEntity
                {
                    Name = fileKey,
                    AccountKey = accountKey.ToApiGuidString(),
                    ContentType = contentType,
                    Data = data,
                    OriginalName = originalName,
                    ContentEncoding = string.Empty
                }
            };
            var storageWriter = new UploadThumbnailFileBlobEntityWriter<QhUploadThumbnailFileBlobEntity>();
            var storageResults = storageWriter.Execute(storageArgs);

            if (!storageResults.IsSuccess)
            {
                throw new Exception("サムネイル画像のBlobStorageへの登録に失敗しました。");
            }

            return storageResults.Result;
        }


        bool DeleteAllFiles(Guid fileKey)
        {
            return
                DeleteOriginal(fileKey) &&
                DeleteEdited(fileKey) &&
                DeleteThumbnail(fileKey);
        }

        bool DeleteOriginal(Guid fileKey)
        {
            try
            {
                var writer = new UploadOriginalFileBlobEntityWriter<QhUploadOriginalFileBlobEntity>();
                return writer.Remove(fileKey);
            }
            catch
            {
                return false;
            }
        }

        bool DeleteEdited(Guid fileKey)
        {
            try
            {
                var writer = new UploadEditedFileBlobEntityWriter<QhUploadEditedFileBlobEntity>();
                return writer.Remove(fileKey);
            }
            catch
            {
                return false;
            }
        }

        bool DeleteThumbnail(Guid fileKey)
        {
            try
            {
                var writer = new UploadThumbnailFileBlobEntityWriter<QhUploadThumbnailFileBlobEntity>();
                return writer.Remove(fileKey);
            }
            catch
            {
                return false;
            }
        }
    }
}
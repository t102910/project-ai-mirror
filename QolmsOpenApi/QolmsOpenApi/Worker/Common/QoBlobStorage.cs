using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{

        
    internal sealed class QoBlobStorage
    {


        /// <summary>
        /// 画像の回転方向の種別を表します。
        /// </summary>
        /// <remarks></remarks>
        private enum ImageOrientationTypeEnum
        {

            /// <summary>
            ///     回転無しです。
            ///     </summary>
            ///     <remarks></remarks>
            Horizontal = 1,

            /// <summary>
            ///     180 度回転です。
            ///     </summary>
            ///     <remarks></remarks>
            Rotate180 = 3,

            /// <summary>
            ///     時計回り 90 度回転です。
            ///     </summary>
            ///     <remarks></remarks>
            Rotate90CW = 6,

            /// <summary>
            ///     時計回り 270 度回転です。
            ///     </summary>
            ///     <remarks></remarks>
            Rotate270CW = 8
        }



        /// <summary>
        /// 加工済み画像の拡張子を表します。
        /// </summary>
        /// <remarks></remarks>
        private const string EDITED_FILE_EXTENSION = ".jpg";

        /// <summary>
        /// 加工済み画像が格納されるフォルダ名を表します。
        /// </summary>
        /// <remarks></remarks>
        private const string EDITED_FOLDER_NAME = "Edited";

        /// <summary>
        /// Exif 画像方向タグ ID を表します。
        /// </summary>
        /// <remarks></remarks>
        private const int EXIF_ORIENTATION_TAGID = 0x112;

        /// <summary>
        /// 固定ファイル（画像以外のファイルを表すサムネイル画像など）が格納されるフォルダ名を表します。
        /// </summary>
        /// <remarks></remarks>
        private const string FIXED_FOLDER_NAME = "Fixed";

        /// <summary>
        /// オリジナル画像が格納されるフォルダ名を表します。
        /// </summary>
        /// <remarks></remarks>
        private const string ORIGINAL_FOLDER_NAME = "Original";

        /// <summary>
        /// テンポラリファイルの拡張子を表します。
        /// </summary>
        /// <remarks></remarks>
        private const string TEMP_FILE_EXTENSION = ".temp";

        /// <summary>
        /// サムネイル画像の拡張子を表します。
        /// </summary>
        /// <remarks></remarks>
        private const string THUMBNAIL_FILE_EXTENSION = ".jpg";

        /// <summary>
        /// サムネイル画像が格納されるフォルダ名を表します。
        /// </summary>
        /// <remarks></remarks>
        private const string THUMBNAIL_FOLDER_NAME = "Thumbnail";

        /// <summary>
        /// サムネイルの横幅（ピクセル）を表します。
        /// </summary>
        /// <remarks></remarks>
        private const int THUMBNAIL_WIDTH = 120;


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private QoBlobStorage()
        {
        }

        /// <summary>
        /// Exif タグより画像の回転方向を取得します。
        /// </summary>
        /// <param name="source">取得元イメージ。</param>
        /// <returns>
        /// 取得できれば回転方向を表す <see cref="ImageOrientationTypeEnum" />、
        /// 取得出来なければ <see cref="ImageOrientationTypeEnum.Horizontal" />。
        /// </returns>
        /// <remarks></remarks>
        private static ImageOrientationTypeEnum GetImageOrientation(Image source)
        {
            ImageOrientationTypeEnum result = ImageOrientationTypeEnum.Horizontal;

            if (source != null)
            {
                try
                {
                    PropertyItem item = source.PropertyItems.ToList().Find(i => i.Id == EXIF_ORIENTATION_TAGID && i.Type == 3 && i.Len == 2);

                    if (item != null)
                        // 回転のみサポート（反転は未サポート）
                        result = (ImageOrientationTypeEnum)Enum.ToObject(typeof(ImageOrientationTypeEnum), item.Value[0] != 0 ? item.Value[0] : item.Value[1]);
                }
                catch
                {
                }
            }

            return result;
        }



        private static TEntity ReadMetadata<TEntity, TArgs, TResults, TReader>(Guid fileKey)
            where TEntity : QsUploadFileBlobEntityBase, new()
            where TArgs : QsAzureBlobStorageMetadataReaderArgsBase, new()
            where TResults : QsAzureBlobStorageMetadataReaderResultsBase<TEntity>
            where TReader : UploadFileBlobStorageMetadataReaderBase<TEntity, TArgs, TResults>, new()
        {
            TEntity result = null;

            // ブロブ ストレージから取得
            TReader reader = new TReader();
            TArgs readerArgs = new TArgs() { Name = fileKey };
            TResults readerResults = reader.Execute(readerArgs);

            if (readerResults.IsSuccess && readerResults.Result != null)
                result = readerResults.Result;

            return result;
        }

        private static byte[] ReadFileBytes<TEntity, TArgs, TResults, TReader>(Guid fileKey, ref string refContentType, ref string refContentEncoding
    )
            where TEntity : QsUploadFileBlobEntityBase, new()
            where TArgs : QsAzureBlobStorageReaderArgsBase, new()
            where TResults : QsAzureBlobStorageReaderResultsBase<TEntity>
            where TReader : UploadFileBlobStorageReaderBase<TEntity, TArgs, TResults>, new()
        {
            byte[] result = null;

            // ブロブ ストレージから取得
            TReader reader = new TReader();
            TArgs readerArgs = new TArgs() { Name = fileKey };
            TResults readerResults = reader.Execute(readerArgs);
                     
            if (readerResults.IsSuccess && readerResults.Result != null)
            {
                refContentType = readerResults.Result.ContentType;
                refContentEncoding = readerResults.Result.ContentEncoding;

                result = readerResults.Result.Data;
            }
            return result;
        }

        private static bool WriteMetadata<TEntity, TArgs, TResults, TWriter>(TEntity entity
    )
            where TEntity : QsUploadFileBlobEntityBase, new()
            where TArgs : QsAzureBlobStorageMetadataWriterArgsBase<TEntity>, new()
            where TResults : QsAzureBlobStorageMetadataWriterResultsBase
            where TWriter : UploadFileBlobStorageMetadataWriterBase<TEntity, TArgs, TResults>, new()
        {

            // ブロブ ストレージへ保存
            TWriter writer = new TWriter();
            TArgs writerArgs = new TArgs()
            {
                Entity = entity
            };
            TResults writerResults = writer.Execute(writerArgs);

            return writerResults.IsSuccess;
        }

        private static Guid WriteFileBytes<TEntity, TArgs, TResults, TWriter>(Guid fileKey, string contentType, string contentEncoding, byte[] data, Guid accountKey, string originalName
    )
            where TEntity : QsUploadFileBlobEntityBase, new()
            where TArgs : QsAzureBlobStorageWriterArgsBase<TEntity>, new()
            where TResults : QsAzureBlobStorageWriterResultsBase
            where TWriter : UploadFileBlobStorageWriterBase<TEntity, TArgs, TResults>, new()
        {
            Guid result = Guid.Empty;

            // ブロブ ストレージへ保存
            TWriter writer = new TWriter();
            TArgs writerArgs = new TArgs()
            {
                Entity = new TEntity()
                {
                    Name = fileKey,
                    ContentType = contentType,
                    ContentEncoding = contentEncoding,
                    Data = data,
                    AccountKey = accountKey.ToApiGuidString(),
                    OriginalName = originalName
                }
            };
            TResults writerResults = writer.Execute(writerArgs);
            if (writerResults.IsSuccess)
                result = writerResults.Result;

            return result;
        }

        
        /// <summary>
        /// ファイルのバイト配列をストレージから読み込みます。
        /// </summary>
        /// <param name="fileType">ファイルの種別。</param>
        /// <param name="fileKey">ファイル キー。</param>
        /// <param name="contentType">コンテンツ タイプ。</param>
        /// <param name="accountKey">アカウント キー（未使用）。</param>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <param name="appDataFolderPath">App_Data フォルダのパス。</param>
        /// <param name="refContentType">コンテンツ タイプが格納される変数。</param>
        /// <param name="refContentEncoding">コンテンツ エンコーディングが格納される変数。</param>
        /// <returns>
        /// 成功ならファイルのバイト配列、
        /// 失敗なら Nothing。
        /// </returns>
        /// <remarks></remarks>
        public static byte[] ReadFileBytes<TEntity>(QsApiFileTypeEnum fileType, Guid fileKey, string contentType, Guid accountKey, string originalName, string appDataFolderPath, ref string refContentType, ref string refContentEncoding
    ) where TEntity : QsUploadFileDataEntityBase
        {
            refContentType = contentType;
            refContentEncoding = string.Empty;

            byte[] result = null;

            if (fileType != QsApiFileTypeEnum.None && !string.IsNullOrWhiteSpace(originalName))
            {
                bool isImage = contentType.Replace(" ", string.Empty).Trim().StartsWith("image/", StringComparison.CurrentCultureIgnoreCase);
                if(int.TryParse ("1",out int _))
                switch (typeof(TEntity).Name)
                {
                    case nameof(QH_UPLOADFILE_DAT): 
                             
                        switch (true)
                        {
                            case bool _ when fileType == QsApiFileTypeEnum.Original && fileKey != Guid.Empty:
                                // オリジナル ファイル（画像）をブロブ ストレージから取得
                                result = QoBlobStorage.ReadFileBytes<QhUploadOriginalFileBlobEntity, UploadOriginalFileBlobEntityReaderArgs, UploadOriginalFileBlobEntityReaderResults<QhUploadOriginalFileBlobEntity>, UploadOriginalFileBlobEntityReader<QhUploadOriginalFileBlobEntity>>(fileKey, ref refContentType, ref refContentEncoding);
                                break;
                            case bool _ when fileType == QsApiFileTypeEnum.Edited && fileKey != Guid.Empty:
                                // 変換された画像をブロブ ストレージから取得
                                result = QoBlobStorage.ReadFileBytes<QhUploadEditedFileBlobEntity, UploadEditedFileBlobEntityReaderArgs, UploadEditedFileBlobEntityReaderResults<QhUploadEditedFileBlobEntity>, UploadEditedFileBlobEntityReader<QhUploadEditedFileBlobEntity>>(fileKey, ref refContentType, ref refContentEncoding);
                                break;
                            case bool _ when fileType == QsApiFileTypeEnum.Thumbnail:
                                if (isImage && fileKey != Guid.Empty)
                                {
                                    // サムネイル画像をブロブ ストレージから取得
                                    result = QoBlobStorage.ReadFileBytes<QhUploadThumbnailFileBlobEntity, UploadThumbnailFileBlobEntityReaderArgs, UploadThumbnailFileBlobEntityReaderResults<QhUploadThumbnailFileBlobEntity>, UploadThumbnailFileBlobEntityReader<QhUploadThumbnailFileBlobEntity>>(fileKey, ref refContentType, ref refContentEncoding);
                                }
                                else
                                {
                                    // 固定のサムネイル画像をフォルダから取得
                                    if (!string.IsNullOrWhiteSpace(appDataFolderPath))
                                    {
                                        string filePath = System.IO.Path.Combine(appDataFolderPath, QoBlobStorage.FIXED_FOLDER_NAME, string.Format("thumbnail_{0}{1}", System.IO.Path.GetExtension(originalName).Replace(".", string.Empty), QoBlobStorage.THUMBNAIL_FILE_EXTENSION));

                                        if (System.IO.File.Exists(filePath))
                                        {
                                            // ファイルから読み込む
                                            try
                                            {
                                                using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                                                {
                                                    int length = Convert.ToInt32(stream.Length);
                                                    byte[] data = new byte[length - 1 + 1];

                                                    if (stream.Read(data, 0, length) == length)
                                                    {
                                                        refContentType = "image/jpeg";
                                                        refContentEncoding = string.Empty;
                                                        result = data;
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }

                                    }
                                 break;
                            default:
                                break;
                        }

                        break;
                   
                    case nameof(QP_UPLOADFILE_DAT):
                            switch (true)
                            {
                                case bool _ when fileType == QsApiFileTypeEnum.Original && fileKey != Guid.Empty:
                                        // オリジナル ファイル（画像）をブロブ ストレージから取得
                                        result = QoBlobStorage.ReadFileBytes<QpUploadOriginalFileBlobEntity, UploadOriginalFileBlobEntityReaderArgs, UploadOriginalFileBlobEntityReaderResults<QpUploadOriginalFileBlobEntity>, UploadOriginalFileBlobEntityReader<QpUploadOriginalFileBlobEntity>>(fileKey, ref refContentType, ref refContentEncoding);
                                        break;
                                    
                                case bool _ when fileType == QsApiFileTypeEnum.Edited && fileKey != Guid.Empty:
                                        // 変換された画像をブロブ ストレージから取得
                                        result = QoBlobStorage.ReadFileBytes<QpUploadEditedFileBlobEntity, UploadEditedFileBlobEntityReaderArgs, UploadEditedFileBlobEntityReaderResults<QpUploadEditedFileBlobEntity>, UploadEditedFileBlobEntityReader<QpUploadEditedFileBlobEntity>>(fileKey, ref refContentType, ref refContentEncoding);
                                        break;
                                case bool _ when fileType == QsApiFileTypeEnum.Thumbnail:
                                        if (isImage && fileKey != Guid.Empty)
                                            // サムネイル画像をブロブ ストレージから取得
                                            result = QoBlobStorage.ReadFileBytes<QpUploadThumbnailFileBlobEntity, UploadThumbnailFileBlobEntityReaderArgs, UploadThumbnailFileBlobEntityReaderResults<QpUploadThumbnailFileBlobEntity>, UploadThumbnailFileBlobEntityReader<QpUploadThumbnailFileBlobEntity>>(fileKey, ref refContentType, ref refContentEncoding);
                                        else
                                        {  // 固定のサムネイル画像をフォルダから取得
                                            if (!string.IsNullOrWhiteSpace(appDataFolderPath))
                                            {
                                                string filePath = System.IO.Path.Combine(appDataFolderPath, QoBlobStorage.FIXED_FOLDER_NAME, string.Format("thumbnail_{0}{1}", System.IO.Path.GetExtension(originalName).Replace(".", string.Empty), QoBlobStorage.THUMBNAIL_FILE_EXTENSION)
        );

                                                if (System.IO.File.Exists(filePath))
                                                {
                                                    // ファイルから読み込む
                                                    try
                                                    {
                                                        using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                                                        {
                                                            int length = Convert.ToInt32(stream.Length);
                                                            byte[] data = new byte[length - 1 + 1];

                                                            if (stream.Read(data, 0, length) == length)
                                                            {
                                                                refContentType = "image/jpeg";
                                                                refContentEncoding = string.Empty;
                                                                result = data;
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    
                                default:
                                    break;
                            }

                        break;
                }
            }

            return result;
        }
        
        /// <summary>
        /// ファイルをストレージから読み込みます。
        /// </summary>
        /// <param name="fileType">ファイルの種別。</param>
        /// <param name="fileKey">ファイル キー。</param>
        /// <param name="contentType">コンテンツ タイプ。</param>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <param name="appDataFolderPath">App_Data フォルダのパス。</param>
        /// <param name="refContentType">コンテンツ タイプが格納される変数。</param>
        /// <param name="refContentEncoding">コンテンツ エンコーディングが格納される変数。</param>
        /// <returns>
        /// 成功ならファイルのバイト配列、
        /// 失敗なら Nothing。
        /// </returns>
        /// <remarks></remarks>
        private static byte[] ReadFileFromStorageStorage<TEntity>(QsApiFileTypeEnum fileType, Guid fileKey, string contentType, Guid accountKey, string originalName, string appDataFolderPath, ref string refContentType, ref string refContentEncoding
    ) where TEntity : QsUploadFileDataEntityBase
        {
            refContentType = contentType;
            refContentEncoding = string.Empty;

            byte[] result ;

            try
            {
                result = ReadFileBytes<TEntity>(fileType, fileKey, contentType, accountKey, originalName, appDataFolderPath, ref refContentType, ref refContentEncoding);

                bool isValid = false;

                if (result != null)
                {
                    // 取得に成功
                    switch (true)
                    {
                        case bool _ when result.Length > 0 && fileType == QsApiFileTypeEnum.Original:
                            // オリジナル ファイル
                            isValid = !string.IsNullOrWhiteSpace(refContentType);
                            break;
                            
                        case object _ when result.Length > 0 && fileType == QsApiFileTypeEnum.Edited:
                            // 変換された画像
                            isValid = (string.Compare(refContentType.Replace(" ", ""), "image/jpeg", true) == 0);
                            break;
                            
                        case object _ when result.Length > 0 && fileType == QsApiFileTypeEnum.Thumbnail:
                            // サムネイル画像
                            isValid = (string.Compare(refContentType.Replace(" ", ""), "image/jpeg", true) == 0);
                            break;
                    }
                }

                if (!isValid)
                {
                    // フォーマットが不正
                    refContentType = string.Empty;
                    refContentEncoding = string.Empty;

                    result = null;
                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                // エラー
                refContentType = string.Empty;
                refContentEncoding = string.Empty;

                result = null;
            }

            return result;
        }

        /// <summary>
        /// ファイルをストレージから読み込み、
        /// Base64 エンコードされた文字列データとして返却します。
        /// </summary>
        /// <param name="fileType">ファイルの種別。</param>
        /// <param name="fileKey">ファイル キー。</param>
        /// <param name="contentType">コンテンツ タイプ。</param>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <param name="appDataFolderPath">App_Data フォルダのパス。</param>
        /// <param name="refContentType">コンテンツ タイプが格納される変数（"image/jpeg"）。</param>
        /// <param name="refContentEncoding">コンテンツ エンコーディングが格納される変数。</param>
        /// <returns>
        /// 成功なら Base64 エンコードされた文字列データ、
        /// 失敗なら String.Empty。
        /// </returns>
        /// <remarks></remarks>
        private static string ReadFile<TEntity>(QsApiFileTypeEnum fileType, Guid fileKey, string contentType, Guid accountKey, string originalName, string appDataFolderPath, ref string refContentType, ref string refContentEncoding
    ) where TEntity : QsUploadFileDataEntityBase
        {
            byte[] data;

            if (contentType.Replace(" ", string.Empty).Trim().StartsWith("image/", StringComparison.CurrentCultureIgnoreCase))
                // 画像
                data = QoBlobStorage.ReadFileFromStorageStorage<TEntity>(fileType, fileKey, contentType, accountKey, originalName, string.Empty, ref refContentType, ref refContentEncoding);
            else
                // 画像以外
                if (fileType == QsApiFileTypeEnum.Thumbnail)
                data = QoBlobStorage.ReadFileFromStorageStorage<TEntity>(fileType, Guid.Empty, string.Empty, Guid.Empty, originalName, appDataFolderPath, ref refContentType, ref refContentEncoding);
            else
                data = QoBlobStorage.ReadFileFromStorageStorage<TEntity>(fileType, fileKey, contentType, accountKey, originalName, string.Empty, ref refContentType, ref refContentEncoding);

            return data == null ? string.Empty : Convert.ToBase64String(data);
        }

        /// <summary>
        /// 固定のサムネイル画像をストレージから読み込み、
        /// Base64 エンコードされた文字列データとして返却します。
        /// </summary>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <param name="appDataFolderPath">App_Data フォルダのパス。</param>
        /// <param name="refContentType">コンテンツ タイプが格納される変数（"image/jpeg"）。</param>
        /// <param name="refContentEncoding">コンテンツ エンコーディングが格納される変数。</param>
        /// <returns>
        /// 成功なら Base64 エンコードされた文字列データ、
        /// 失敗なら String.Empty。
        /// </returns>
        /// <remarks></remarks>
        private static string ReadFixedThumbnailFile<TEntity>(string originalName, string appDataFolderPath, ref string refContentType, ref string refContentEncoding) where TEntity : QsUploadFileDataEntityBase
        {
            byte[] data = QoBlobStorage.ReadFileFromStorageStorage<TEntity>(QsApiFileTypeEnum.Thumbnail, Guid.Empty, string.Empty, Guid.Empty, originalName, appDataFolderPath, ref refContentType, ref refContentEncoding);

            return data == null ? string.Empty : Convert.ToBase64String(data);
        }



        /// <summary>
        /// ファイルのバイト配列をストレージへ書き込みます。
        /// </summary>
        /// <param name="fileType">ファイルの種別。</param>
        /// <param name="fileKey">
        /// ファイル キー（ブロブ名）。
        /// <paramref name="fileType" /> = <see cref="QsApiFileTypeEnum.Original" /> の場合は Guid.Empty、
        /// それ以外の場合はオリジナル ファイルの fileKey を指定。
        /// </param>
        /// <param name="contentType">コンテンツ タイプ。</param>
        /// <param name="contentEncoding">コンテンツ エンコーディング。</param>
        /// <param name="data">ファイルのバイト配列。</param>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <returns>
        /// 成功なら Guid.Empty 以外、
        /// 失敗なら Guid.Empty。
        /// </returns>
        /// <remarks></remarks>
        private static Guid WriteFileBytes<TEntity>(QsApiFileTypeEnum fileType, Guid fileKey, string contentType, string contentEncoding, byte[] data, Guid accountKey, string originalName
    ) where TEntity : QsUploadFileDataEntityBase
        {
            Guid result = Guid.Empty;

            if (fileType != QsApiFileTypeEnum.None && !string.IsNullOrWhiteSpace(contentType) && data != null && data.Length > 0 && !string.IsNullOrWhiteSpace(originalName))
            {
                bool isImage = contentType.Replace(" ", string.Empty).Trim().StartsWith("image/", StringComparison.CurrentCultureIgnoreCase);

                switch (typeof(TEntity).Name )
                {
                    case nameof(QH_UPLOADFILE_DAT):
                        switch (true)
                        {
                            case bool _ when fileType == QsApiFileTypeEnum.Original:
                                // オリジナル ファイル（画像）をブロブ ストレージへ保存
                                result = QoBlobStorage.WriteFileBytes<QhUploadOriginalFileBlobEntity, UploadOriginalFileBlobEntityWriterArgs<QhUploadOriginalFileBlobEntity>, UploadOriginalFileBlobEntityWriterResults, UploadOriginalFileBlobEntityWriter<QhUploadOriginalFileBlobEntity>>(fileKey, contentType, contentEncoding, data, accountKey, originalName);
                                break;
                            case bool _ when isImage && fileType == QsApiFileTypeEnum.Edited:
                                // 変換された画像をブロブ ストレージへ保存
                                result = QoBlobStorage.WriteFileBytes<QhUploadEditedFileBlobEntity, UploadEditedFileBlobEntityWriterArgs<QhUploadEditedFileBlobEntity>, UploadEditedFileBlobEntityWriterResults, UploadEditedFileBlobEntityWriter<QhUploadEditedFileBlobEntity>>(fileKey, contentType, contentEncoding, data, accountKey, originalName);
                                break;
                            case bool _ when isImage && fileType == QsApiFileTypeEnum.Thumbnail:
                                // サムネイル画像をブロブ ストレージへ保存
                                result = QoBlobStorage.WriteFileBytes<QhUploadThumbnailFileBlobEntity, UploadThumbnailFileBlobEntityWriterArgs<QhUploadThumbnailFileBlobEntity>, UploadThumbnailFileBlobEntityWriterResults, UploadThumbnailFileBlobEntityWriter<QhUploadThumbnailFileBlobEntity>>(fileKey, contentType, contentEncoding, data, accountKey, originalName);
                                break;
                        }
                        break;
                    case nameof(QP_UPLOADFILE_DAT):
                        switch (true)
                        {
                            case bool _ when fileType == QsApiFileTypeEnum.Original:
                                // オリジナル ファイル（画像）をブロブ ストレージへ保存
                                result = QoBlobStorage.WriteFileBytes<QpUploadOriginalFileBlobEntity, UploadOriginalFileBlobEntityWriterArgs<QpUploadOriginalFileBlobEntity>, UploadOriginalFileBlobEntityWriterResults, UploadOriginalFileBlobEntityWriter<QpUploadOriginalFileBlobEntity>>(fileKey, contentType, contentEncoding, data, accountKey, originalName);
                                break;
                            case bool _ when isImage && fileType == QsApiFileTypeEnum.Edited:
                                // 変換された画像をブロブ ストレージへ保存
                                result = QoBlobStorage.WriteFileBytes<QpUploadEditedFileBlobEntity, UploadEditedFileBlobEntityWriterArgs<QpUploadEditedFileBlobEntity>, UploadEditedFileBlobEntityWriterResults, UploadEditedFileBlobEntityWriter<QpUploadEditedFileBlobEntity>>(fileKey, contentType, contentEncoding, data, accountKey, originalName);
                                break;
                            case bool _ when isImage && fileType == QsApiFileTypeEnum.Thumbnail:
                                // サムネイル画像をブロブ ストレージへ保存
                                result = QoBlobStorage.WriteFileBytes<QpUploadThumbnailFileBlobEntity, UploadThumbnailFileBlobEntityWriterArgs<QpUploadThumbnailFileBlobEntity>, UploadThumbnailFileBlobEntityWriterResults, UploadThumbnailFileBlobEntityWriter<QpUploadThumbnailFileBlobEntity>>(fileKey, contentType, contentEncoding, data, accountKey, originalName);
                                break;
                        }
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 画像ファイルをストレージへ書き込みます。
        /// </summary>
        /// <param name="contentType">コンテンツ タイプ。</param>
        /// <param name="data">画像ファイルのバイト配列。</param>
        /// <param name="accountKey">アカウントキー。</param>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <returns>
        /// 成功なら Guid.Empty 以外、
        /// 失敗なら Guid.Empty。
        /// </returns>
        /// <remarks></remarks>
        private static Guid WriteImageFileToStorage<TEntity>(string contentType, byte[] data, Guid accountKey, string originalName) where TEntity : QsUploadFileDataEntityBase
        {
            Guid result = Guid.Empty;
            Image editidImage = null;

            try
            {
                // オリジナル画像を保存
                Guid key = QoBlobStorage.WriteFileBytes<TEntity>(QsApiFileTypeEnum.Original, Guid.Empty, contentType, string.Empty, data, accountKey, originalName);

                if (key != Guid.Empty)
                {
                    editidImage = QoBlobStorage.CreateImage(data);

                    if (editidImage != null)
                    {
                        byte[] editidBytes = QoBlobStorage.CreateJpegBytes(editidImage);
                        byte[] thumbnailBytes = QoBlobStorage.CreateJpegThumbnailBytes(editidImage, QoBlobStorage.THUMBNAIL_WIDTH, int.MaxValue, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);

                        // 変換された画像を保存
                        if (QoBlobStorage.WriteFileBytes<TEntity>(QsApiFileTypeEnum.Edited, key, "image/jpeg", string.Empty, editidBytes, accountKey, originalName) != key)
                            // TODO: コミデータの削除

                            throw new InvalidOperationException("変換された画像の保存に失敗しました。");

                        // サムネイル画像を保存
                        if (QoBlobStorage.WriteFileBytes<TEntity>(QsApiFileTypeEnum.Thumbnail, key, "image/jpeg", string.Empty, thumbnailBytes, accountKey, originalName) != key)
                            // TODO: コミデータの削除

                            throw new InvalidOperationException("サムネイル画像の保存に失敗しました。");

                        // 成功
                        result = key;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (editidImage != null)
                    editidImage.Dispose();
            }

            return result;
        }

        /// <summary>
        /// 画像以外のファイルをストレージへ書き込みます。
        /// </summary>
        /// <param name="contentType">コンテンツ タイプ。</param>
        /// <param name="contentEncoding">コンテンツ エンコーディング。</param>
        /// <param name="data">ファイルのバイト配列。</param>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <returns>
        /// 成功なら Guid.Empty 以外のファイルキー、
        /// 失敗なら Guid.Empty。
        /// </returns>
        /// <remarks></remarks>
        private static Guid WriteOtherFileToStorage<TEntity>(string contentType, string contentEncoding, byte[] data, Guid accountKey, string originalName) where TEntity : QsUploadFileDataEntityBase
        {

            // ファイルを保存（オリジナルファイルのみ）
            return QoBlobStorage.WriteFileBytes<TEntity>(QsApiFileTypeEnum.Original, Guid.Empty, contentType, contentEncoding, data, accountKey, originalName);
        }

        /// <summary>
        /// Base64 エンコードされたファイルを復号化し、
        /// ストレージへ書き込みます。
        /// </summary>
        /// <param name="contentType">コンテンツ タイプ。</param>
        /// <param name="contentEncoding">コンテンツ エンコーディング。</param>
        /// <param name="data">Base64 エンコードされたファイル。</param>
        /// <param name="accountKey">アカウント キー。</param>
        /// <param name="originalName">オリジナル ファイル名。</param>
        /// <returns>
        /// 成功なら Guid.Empty 以外のファイル キー、
        /// 失敗なら Guid.Empty。
        /// </returns>
        /// <remarks></remarks>
        private static Guid WriteFile<TEntity>(string contentType, string contentEncoding, string data, Guid accountKey, string originalName) where TEntity : QsUploadFileDataEntityBase
        {
            Guid result = Guid.Empty;

            try
            {
                if (contentType.Replace(" ", string.Empty).Trim().StartsWith("image/", StringComparison.CurrentCultureIgnoreCase))
                    // 画像
                    result = QoBlobStorage.WriteImageFileToStorage<TEntity>(contentType, Convert.FromBase64String(data), accountKey, originalName);
                else
                    // 画像以外
                    result = QoBlobStorage.WriteOtherFileToStorage<TEntity>(contentType, contentEncoding, Convert.FromBase64String(data), accountKey, originalName);
            }
            catch
            {
            }

            return result;
        }



        /// <summary>
        /// 連携状態をチェックします。
        /// </summary>
        /// <param name="authorKey">所有者アカウント キー。</param>
        /// <param name="actorKey">対象者アカウント キー。</param>
        /// <param name="fileRelationType">ファイルの連携先の種別。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private static bool IsPossible(Guid authorKey, Guid actorKey, QsApiFileRelationTypeEnum fileRelationType)
        {

            // TODO: 連携状態のチェック

            return true;
        }





        /// <summary>
        /// 画像のバイト配列からイメージを作成します。
        /// イメージは必要に応じて回転変換されます。
        /// </summary>
        /// <param name="data">画像のバイト配列。</param>
        /// <returns>
        /// 成功ならイメージ、
        /// 失敗なら Nothing。
        /// </returns>
        /// <remarks></remarks>
        public static Image CreateImage(byte[] data)
        {
            Image result = null/* TODO Change to default(_) if this is not a reference type */;

            if (data != null && data.Length > 0)
            {
                try
                {
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream(data))
                    {
                        result = Image.FromStream(stream);

                        switch (QoBlobStorage.GetImageOrientation(result))
                        {
                            case ImageOrientationTypeEnum.Rotate180:
                                {
                                    // 180 度回転
                                    result.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                }

                            case ImageOrientationTypeEnum.Rotate90CW:
                                {
                                    // 時計回りに 90 度回転
                                    result.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                }

                            case ImageOrientationTypeEnum.Rotate270CW:
                                {
                                    // 時計回りに 270 度回転
                                    result.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                }
                        }
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        /// <summary>
        /// イメージから JPEG 形式の画像を作成します。
        /// </summary>
        /// <param name="source">作成元イメージ。</param>
        /// <returns>
        /// 成功なら JPEG 形式の画像のバイト配列、
        /// 失敗なら Nothing。
        /// </returns>
        /// <remarks></remarks>
        public static byte[] CreateJpegBytes(Image source)
        {
            byte[] result = null;

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageEncoders().ToList().Find(i => i.FormatID == ImageFormat.Jpeg.Guid);

                if (jpegEncoder != null)
                {
                    System.Drawing.Imaging.Encoder category = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters @params = new EncoderParameters(1);

                    @params.Param[0] = new EncoderParameter(category, 100L);

                    using (Bitmap bmp = new Bitmap(source))
                    {
                        bmp.Save(stream, jpegEncoder, @params);

                        if (stream.Length > 0)
                        {
                            int length = Convert.ToInt32(stream.Length);
                            byte[] bytes = new byte[length - 1 + 1];

                            stream.Position = 0;
                            stream.Read(bytes, 0, length);

                            result = bytes;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// イメージから JPEG 形式のサムネイル画像を作成します。
        /// </summary>
        /// <param name="source">作成元イメージ。</param>
        /// <param name="withinWidth">サムネイルの横幅（ピクセル）。</param>
        /// <param name="withinHeight">サムネイルの縦幅（ピクセル）。</param>
        /// <param name="mode">サムネイルの品質。</param>
        /// <returns>
        /// 成功なら JPEG 形式のサムネイル画像のバイト配列、
        /// 失敗なら Nothing。
        /// サムネイル画像は、
        /// 作成元イメージの縦横比を維持して、
        /// 指定された横幅・縦幅に収まるように縮小されます。
        /// </returns>
        /// <remarks></remarks>
        public static byte[] CreateJpegThumbnailBytes(Image source, int withinWidth, int withinHeight, System.Drawing.Drawing2D.InterpolationMode mode)
        {
            byte[] result = null;
            double w;
            double h;

            if (source != null && withinWidth > 0 && withinHeight > 0)
            {
                if (source.Width >= source.Height)
                {
                    w = withinWidth;
                    h = w * source.Height / (double)source.Width;

                    if (h > withinHeight)
                    {
                        w = w * withinHeight / h;
                        h = withinHeight;
                    }
                }
                else
                {
                    h = withinHeight;
                    w = h * source.Width / (double)source.Height;

                    if (w > withinWidth)
                    {
                        h = h * withinWidth / w;
                        w = withinWidth;
                    }
                }

                using (Bitmap bmp = new Bitmap(Convert.ToInt32(Math.Floor(w)), Convert.ToInt32(Math.Floor(h))))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode = mode;
                        g.DrawRectangle(Pens.Transparent, new Rectangle(0, 0, bmp.Width, bmp.Height));
                        g.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel);

                        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                        {
                            ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageEncoders().ToList().Find(i => i.FormatID == ImageFormat.Jpeg.Guid);

                            if (jpegEncoder != null)
                            {
                                System.Drawing.Imaging.Encoder category = System.Drawing.Imaging.Encoder.Quality;
                                EncoderParameters @params = new EncoderParameters(1);

                                @params.Param[0]= new EncoderParameter(category, 100L);
                                bmp.Save(stream, jpegEncoder, @params);

                                if (stream.Length > 0)
                                {
                                    int length = Convert.ToInt32(stream.Length);
                                    byte[] bytes = new byte[length - 1 + 1];

                                    stream.Position = 0;
                                    stream.Read(bytes, 0, length);

                                    result = bytes;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        
        /// <summary>
        /// 画像のBLOBストレージのメタデータのアカウントキーを書き換えます。
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool UpdateImageMetaDataAccountKey(Guid fileKey, Guid accountKey)
        {
            bool result = true;

            // オリジナルファイルのメタ データのアカウントキーを書き換え
            QhUploadOriginalFileBlobEntity entity = QoBlobStorage.ReadMetadata<QhUploadOriginalFileBlobEntity, UploadOriginalFileBlobMetadataReaderArgs, UploadOriginalFileBlobMetadataReaderResults<QhUploadOriginalFileBlobEntity>, UploadOriginalFileBlobMetadataReader<QhUploadOriginalFileBlobEntity>>(fileKey);
            if (entity != null)
            {
                entity.AccountKey = accountKey.ToApiGuidString();
                result = result & QoBlobStorage.WriteMetadata<QhUploadOriginalFileBlobEntity, UploadOriginalFileBlobMetadataWriterArgs<QhUploadOriginalFileBlobEntity>, UploadOriginalFileBlobMetadataWriterResults, UploadOriginalFileBlobMetadataWriter<QhUploadOriginalFileBlobEntity>>(entity);
            }
            else
                result = false;
            // 変換された画像のメタデータのアカウントキーを書き換え
            QhUploadEditedFileBlobEntity editedEntity = QoBlobStorage.ReadMetadata<QhUploadEditedFileBlobEntity, UploadEditedFileBlobMetadataReaderArgs, UploadEditedFileBlobMetadataReaderResults<QhUploadEditedFileBlobEntity>, UploadEditedFileBlobMetadataReader<QhUploadEditedFileBlobEntity>>(fileKey);
            if (editedEntity != null)
            {
                editedEntity.AccountKey = accountKey.ToApiGuidString();
                result = result & QoBlobStorage.WriteMetadata<QhUploadEditedFileBlobEntity, UploadEditedFileBlobMetadataWriterArgs<QhUploadEditedFileBlobEntity>, UploadEditedFileBlobMetadataWriterResults, UploadEditedFileBlobMetadataWriter<QhUploadEditedFileBlobEntity>>(editedEntity);
            }
            else
                result = false;
            // サムネイル画像のメタデータのアカウントキーを書き換え
            QhUploadThumbnailFileBlobEntity thumnailEntity = QoBlobStorage.ReadMetadata<QhUploadThumbnailFileBlobEntity, UploadThumbnailFileBlobMetadataReaderArgs, UploadThumbnailFileBlobMetadataReaderResults<QhUploadThumbnailFileBlobEntity>, UploadThumbnailFileBlobMetadataReader<QhUploadThumbnailFileBlobEntity>>(fileKey);
            if (thumnailEntity != null)
            {
                thumnailEntity.AccountKey = accountKey.ToApiGuidString();
                result = result & QoBlobStorage.WriteMetadata<QhUploadThumbnailFileBlobEntity, UploadThumbnailFileBlobMetadataWriterArgs<QhUploadThumbnailFileBlobEntity>, UploadThumbnailFileBlobMetadataWriterResults, UploadThumbnailFileBlobMetadataWriter<QhUploadThumbnailFileBlobEntity>>(thumnailEntity);
            }
            else
                result = false;

            return result;
        }
        
        /// <summary>
        /// ファイルをストレージから取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public static QhBlobStorageReadApiResults Read<TEntity>(QhBlobStorageReadApiArgs args) where TEntity : QsUploadFileDataEntityBase
        {
            if (args == null)
                throw new ArgumentNullException("args", "Web API引数クラスがNull参照です。");

            QhBlobStorageReadApiResults result = new QhBlobStorageReadApiResults() { IsSuccess = bool.FalseString };
            Guid authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            Guid actorKey = args.ActorKey.TryToValueType(Guid.Empty);
            Guid fileKey = args.FileKey.TryToValueType(Guid.Empty);

            // 連携状態のチェック
            QoBlobStorage.IsPossible(authorKey, actorKey, args.FileRelationType.ToValueType<QsApiFileRelationTypeEnum>());

            // ゲスト処方せん時は対象者アカウントキーがGuid.Emptyとなるため、actorKeyのチェックは行わない
            if (authorKey != Guid.Empty && fileKey != Guid.Empty)
            {
                // DB から取得
                FileStorageReader<TEntity> reader = new FileStorageReader<TEntity>();
                FileStorageReaderArgs readerArgs = new FileStorageReaderArgs()
                {
                    AuthorKey = authorKey,
                    ActorKey = actorKey,
                    FileKey = fileKey
                };
                FileStorageReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

                
                if (readerResults.IsSuccess && readerResults.FileStorageItem.AccountKey != Guid.Empty && readerResults.FileStorageItem.FileKey != Guid.Empty)
                {
                    string refContentType = string.Empty;
                    string refContentEncoding = string.Empty;

                    // ファイルのバイナリ データ
                    result.Data = QoBlobStorage.ReadFile<TEntity>(args.FileType.TryToValueType(QsApiFileTypeEnum.None), readerResults.FileStorageItem.FileKey, readerResults.FileStorageItem.ContentType, actorKey, readerResults.FileStorageItem.OriginalName, args.AppDataFolderPath, ref refContentType, ref refContentEncoding); // ファイルをストレージから取得

                    // ファイルの MIME タイプ
                    result.ContentType = refContentType;

                    // オリジナル ファイル名
                    result.OriginalName = readerResults.FileStorageItem.OriginalName;

                    // 成功
                    result.IsSuccess = bool.TrueString;
                }
               
            }

            return result;
        }
        /*
        /// <summary>
        /// 固定のサムネイル画像をストレージから取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public static QhBlobStorageThumbnailReadApiResults ThumbnailRead<TEntity>(QhBlobStorageThumbnailReadApiArgs args) where TEntity : QsUploadFileDataEntityBase
        {
            if (args == null)
                throw new ArgumentNullException("args", "Web API引数クラスがNull参照です。");

            string refContentType = string.Empty;
            string refContentEncoding = string.Empty;
            string data = BlobStorageWorker.ReadFixedThumbnailFile<TEntity>(args.OriginalName, args.AppDataFolderPath, ref refContentType, ref refContentEncoding);

            return new QhBlobStorageThumbnailReadApiResults()
            {
                ContentType = refContentType,
                Data = data,
                IsSuccess = (!string.IsNullOrWhiteSpace(data)).ToString()
            };
        }
        */

        /// <summary>
        /// ファイルをストレージへ登録します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public static QhBlobStorageWriteApiResults Write<TEntity>(QhBlobStorageWriteApiArgs args) where TEntity : QsUploadFileDataEntityBase
        {
            if (args == null)
                throw new ArgumentNullException("args", "Web API引数クラスがNull参照です。");

            QhBlobStorageWriteApiResults result = new QhBlobStorageWriteApiResults() { IsSuccess = bool.FalseString };
            Guid authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            Guid actorKey = args.ActorKey.TryToValueType(Guid.Empty);

            // 連携状態のチェック（現在は未チェック）
            QoBlobStorage.IsPossible(authorKey, actorKey, args.FileRelationType.TryToValueType(QsApiFileRelationTypeEnum.None));

            if (authorKey != Guid.Empty && !string.IsNullOrWhiteSpace(args.Data))
            {
                // ファイルをストレージへ登録
                Guid fileKey = QoBlobStorage.WriteFile<TEntity>(args.ContentType, string.Empty, args.Data, actorKey, args.OriginalName);

                if (fileKey != Guid.Empty)
                {
                    // DB へ登録
                    FileStorageWriter<TEntity> writer = new FileStorageWriter<TEntity>();
                    FileStorageWriterArgs writerArgs = new FileStorageWriterArgs()
                    {
                        AuthorKey = authorKey,
                        ActorKey = actorKey,
                        FileKey = fileKey,
                        OriginalName = args.OriginalName,
                        ContentType = args.ContentType
                    };
                    FileStorageWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

                    if (writerResults.IsSuccess && writerResults.Result == 1)
                    {
                        // ファイル キー
                        result.FileKey = fileKey.ToApiGuidString();

                        // 成功
                        result.IsSuccess = bool.TrueString;
                    }
                   
                }
            }

            return result;
        }

        public static (byte[] edited, byte[] thumbnail) CreateImages(byte[] original)
        {
            var image = CreateImage(original);
            var edited = CreateJpegBytes(image);
            var thumbnail = CreateJpegThumbnailBytes(image, 360, int.MaxValue, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);

            return (edited, thumbnail);
        }
    }


}
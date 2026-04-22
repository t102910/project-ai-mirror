
using System;

using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    
    internal sealed class UploadThumbnailFileBlobEntityWriter<TEntity> : UploadFileBlobStorageWriterBase<TEntity, UploadThumbnailFileBlobEntityWriterArgs<TEntity>, UploadThumbnailFileBlobEntityWriterResults> where TEntity : QsUploadFileBlobEntityBase, new()
    {
        /// <summary>
        /// <see cref="UploadThumbnailFileBlobEntityWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadThumbnailFileBlobEntityWriter() : base()
        {
        }

        /// <summary>
        /// 実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override UploadThumbnailFileBlobEntityWriterResults Execute(UploadThumbnailFileBlobEntityWriterArgs<TEntity> args)
        {
            UploadThumbnailFileBlobEntityWriterResults result = new UploadThumbnailFileBlobEntityWriterResults()
            {
                IsSuccess = false,
                Result = Guid.Empty
            };

            result.Result = base.Write(args.Entity);
            result.IsSuccess = result.Result != Guid.Empty;

            return result;
        }

        public bool Remove(Guid fileKey)
        {
            return base.Delete(fileKey);
        }
    }


}
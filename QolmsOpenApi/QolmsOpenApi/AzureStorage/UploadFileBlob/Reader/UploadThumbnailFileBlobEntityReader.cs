using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{

    internal sealed class UploadThumbnailFileBlobEntityReader<TEntity> : UploadFileBlobStorageReaderBase<TEntity, UploadThumbnailFileBlobEntityReaderArgs, UploadThumbnailFileBlobEntityReaderResults<TEntity>> where TEntity : QsUploadFileBlobEntityBase, new()
    {
        public UploadThumbnailFileBlobEntityReader() : base(true, BlobContainerPublicAccessType.Off)
        {
        }

        public override UploadThumbnailFileBlobEntityReaderResults<TEntity> Execute(UploadThumbnailFileBlobEntityReaderArgs args)
        {
            UploadThumbnailFileBlobEntityReaderResults<TEntity> result = new UploadThumbnailFileBlobEntityReaderResults<TEntity>()
            {
                IsSuccess = false,
                Result = null
            };

            result.Result = base.Read(args.Name);
            result.IsSuccess = result.Result != null;

            return result;
        }
    }


}
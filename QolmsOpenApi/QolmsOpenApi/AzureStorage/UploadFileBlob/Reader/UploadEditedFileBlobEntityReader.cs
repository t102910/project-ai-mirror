
using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    

    internal sealed class UploadEditedFileBlobEntityReader<TEntity> : UploadFileBlobStorageReaderBase<TEntity, UploadEditedFileBlobEntityReaderArgs, UploadEditedFileBlobEntityReaderResults<TEntity>> where TEntity : QsUploadFileBlobEntityBase, new()
    {
        public UploadEditedFileBlobEntityReader() : base(true, BlobContainerPublicAccessType.Off)
        {
        }

        public override UploadEditedFileBlobEntityReaderResults<TEntity> Execute(UploadEditedFileBlobEntityReaderArgs args)
        {
            UploadEditedFileBlobEntityReaderResults<TEntity> result = new UploadEditedFileBlobEntityReaderResults<TEntity>()
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
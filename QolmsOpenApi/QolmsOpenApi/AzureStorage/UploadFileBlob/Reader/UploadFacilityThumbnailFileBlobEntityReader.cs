using MGF.QOLMS.QolmsAzureStorageCoreV1;

using Microsoft.WindowsAzure.Storage.Blob;


namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   

    internal sealed class UploadFacilityThumbnailFileBlobEntityReader : QsAzureBlobStorageReaderBase<QhUploadFacilityThumbnailFileBlobEntity>, IQsAzureBlobStorageReader<QhUploadFacilityThumbnailFileBlobEntity, UploadFacilityThumbnailFileBlobEntityReaderArgs, UploadFacilityThumbnailFileBlobEntityReaderResults>
    {
        public UploadFacilityThumbnailFileBlobEntityReader() : base(true, BlobContainerPublicAccessType.Off)
        {
        }



        public UploadFacilityThumbnailFileBlobEntityReaderResults Execute(UploadFacilityThumbnailFileBlobEntityReaderArgs args)
        {
            UploadFacilityThumbnailFileBlobEntityReaderResults result = new UploadFacilityThumbnailFileBlobEntityReaderResults()
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
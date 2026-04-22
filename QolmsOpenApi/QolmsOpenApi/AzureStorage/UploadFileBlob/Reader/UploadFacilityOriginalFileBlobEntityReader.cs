using MGF.QOLMS.QolmsAzureStorageCoreV1;

using Microsoft.WindowsAzure.Storage.Blob;


namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   

    internal sealed class UploadFacilityOriginalFileBlobEntityReader : QsAzureBlobStorageReaderBase<QhUploadFacilityOriginalFileBlobEntity>, IQsAzureBlobStorageReader<QhUploadFacilityOriginalFileBlobEntity, UploadFacilityOriginalFileBlobEntityReaderArgs, UploadFacilityOriginalFileBlobEntityReaderResults>
    {
        public UploadFacilityOriginalFileBlobEntityReader() : base(true, BlobContainerPublicAccessType.Off)
        {
        }



        public UploadFacilityOriginalFileBlobEntityReaderResults Execute(UploadFacilityOriginalFileBlobEntityReaderArgs args)
        {
            UploadFacilityOriginalFileBlobEntityReaderResults result = new UploadFacilityOriginalFileBlobEntityReaderResults()
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
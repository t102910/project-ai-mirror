using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// アプリイベントファイル取得
    /// </summary>
    internal sealed class AppEventFileBlobEntityReader : QsAzureBlobStorageReaderBase<QhAppEventFileBlobEntity>, IQsAzureBlobStorageReader<QhAppEventFileBlobEntity, AppEventFileBlobEntityReaderArgs, AppEventFileBlobEntityReaderResults>
    {
        public AppEventFileBlobEntityReader() : base(true, BlobContainerPublicAccessType.Off)
        {
        }

        public AppEventFileBlobEntityReaderResults Execute(AppEventFileBlobEntityReaderArgs args)
        {
            var result = new AppEventFileBlobEntityReaderResults()
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

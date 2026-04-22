
using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    internal sealed class UploadOriginalFileBlobEntityReader<TEntity> : UploadFileBlobStorageReaderBase<TEntity, UploadOriginalFileBlobEntityReaderArgs, UploadOriginalFileBlobEntityReaderResults<TEntity>> where TEntity : QsUploadFileBlobEntityBase, new()
    {
        public UploadOriginalFileBlobEntityReader() : base(true, BlobContainerPublicAccessType.Off)
        {
        }


        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override UploadOriginalFileBlobEntityReaderResults<TEntity> Execute(UploadOriginalFileBlobEntityReaderArgs args)
        {
            UploadOriginalFileBlobEntityReaderResults<TEntity> result = new UploadOriginalFileBlobEntityReaderResults<TEntity>()
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
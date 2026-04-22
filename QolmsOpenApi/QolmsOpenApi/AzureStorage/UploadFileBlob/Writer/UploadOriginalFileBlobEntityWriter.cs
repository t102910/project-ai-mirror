using System;

using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{


    internal sealed class UploadOriginalFileBlobEntityWriter<TEntity> : UploadFileBlobStorageWriterBase<TEntity, UploadOriginalFileBlobEntityWriterArgs<TEntity>, UploadOriginalFileBlobEntityWriterResults> where TEntity : QsUploadFileBlobEntityBase, new()
    {


        /// <summary>
        /// <see cref="UploadOriginalFileBlobEntityWriter &lt;TEntity&gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadOriginalFileBlobEntityWriter() : base()
        {
        }



        public override UploadOriginalFileBlobEntityWriterResults Execute(UploadOriginalFileBlobEntityWriterArgs<TEntity> args)
        {
            UploadOriginalFileBlobEntityWriterResults result = new UploadOriginalFileBlobEntityWriterResults()
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
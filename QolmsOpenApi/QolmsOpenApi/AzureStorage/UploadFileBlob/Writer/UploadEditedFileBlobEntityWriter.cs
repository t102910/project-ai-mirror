using System;

using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   
    internal sealed class UploadEditedFileBlobEntityWriter<TEntity> : UploadFileBlobStorageWriterBase<TEntity, UploadEditedFileBlobEntityWriterArgs<TEntity>, UploadEditedFileBlobEntityWriterResults> where TEntity : QsUploadFileBlobEntityBase, new()
    {
        /// <summary>
        /// <see cref="UploadEditedFileBlobEntityWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadEditedFileBlobEntityWriter() : base()
        {
        }
        /// <summary>
        /// 実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override UploadEditedFileBlobEntityWriterResults Execute(UploadEditedFileBlobEntityWriterArgs<TEntity> args)
        {
            UploadEditedFileBlobEntityWriterResults result = new UploadEditedFileBlobEntityWriterResults()
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
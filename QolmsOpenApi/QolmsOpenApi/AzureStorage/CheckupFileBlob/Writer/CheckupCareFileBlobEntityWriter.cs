using System;

using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   
    internal sealed class CheckupCareFileBlobEntityWriter<TEntity> : CheckupFileBlobStorageWriterBase<TEntity, CheckupCareFileBlobEntityWriterArgs<TEntity>, CheckupCareFileBlobEntityWriterResults> where TEntity : QsCheckupFileBlobEntityBase, new()
    {
        /// <summary>
        /// <see cref="CheckupCareFileBlobEntityWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupCareFileBlobEntityWriter() : base()
        {
        }
        /// <summary>
        /// 実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override CheckupCareFileBlobEntityWriterResults Execute(CheckupCareFileBlobEntityWriterArgs<TEntity> args)
        {
            CheckupCareFileBlobEntityWriterResults result = new CheckupCareFileBlobEntityWriterResults()
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
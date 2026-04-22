using System;

using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   
    internal sealed class CheckupFrailtyFileBlobEntityWriter<TEntity> : CheckupFileBlobStorageWriterBase<TEntity, CheckupFrailtyFileBlobEntityWriterArgs<TEntity>, CheckupFrailtyFileBlobEntityWriterResults> where TEntity : QsCheckupFileBlobEntityBase, new()
    {
        /// <summary>
        /// <see cref="CheckupFrailtyFileBlobEntityWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupFrailtyFileBlobEntityWriter() : base()
        {
        }
        /// <summary>
        /// 実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override CheckupFrailtyFileBlobEntityWriterResults Execute(CheckupFrailtyFileBlobEntityWriterArgs<TEntity> args)
        {
            CheckupFrailtyFileBlobEntityWriterResults result = new CheckupFrailtyFileBlobEntityWriterResults()
            {
                IsSuccess = false,
                Result = Guid.Empty
            };

            result.Result = base.Write(args.Entity);
            result.IsSuccess = result.Result != Guid.Empty;
            QoAccessLog.WriteErrorLog(result.Result.ToString(), Guid.Empty);
            return result;
        }

        public bool Remove(Guid fileKey)
        {
            return base.Delete(fileKey);
        }
    }


}
using System;

using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   
    internal sealed class CheckupAfterSchoolFileBlobEntityWriter<TEntity> : CheckupFileBlobStorageWriterBase<TEntity, CheckupAfterSchoolFileBlobEntityWriterArgs<TEntity>, CheckupAfterSchoolFileBlobEntityWriterResults> where TEntity : QsCheckupFileBlobEntityBase, new()
    {
        /// <summary>
        /// <see cref="CheckupAfterSchoolFileBlobEntityWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupAfterSchoolFileBlobEntityWriter() : base()
        {
        }
        /// <summary>
        /// 実行します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override CheckupAfterSchoolFileBlobEntityWriterResults Execute(CheckupAfterSchoolFileBlobEntityWriterArgs<TEntity> args)
        {
            CheckupAfterSchoolFileBlobEntityWriterResults result = new CheckupAfterSchoolFileBlobEntityWriterResults()
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
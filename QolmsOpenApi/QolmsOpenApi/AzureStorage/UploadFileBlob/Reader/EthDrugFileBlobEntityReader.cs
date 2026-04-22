using MGF.QOLMS.QolmsAzureStorageCoreV1;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// 医薬品ファイル取得
    /// </summary>
    public class EthDrugFileBlobEntityReader : QsAzureBlobStorageReaderBase<QhEthDrugFileBlobEntity>, IQsAzureBlobStorageReader<QhEthDrugFileBlobEntity, EthDrugFileBlobEntityReaderArgs, EthDrugFileBlobEntityReaderResults>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EthDrugFileBlobEntityReader(): base(true, BlobContainerPublicAccessType.Off)
        {
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public EthDrugFileBlobEntityReaderResults Execute(EthDrugFileBlobEntityReaderArgs args)
        {
            var result = new EthDrugFileBlobEntityReaderResults
            {
                IsSuccess = false
            };

            var entity = Read(args.Name);

            if(entity != null)
            {
                result.IsSuccess = true;
                result.Result = entity;
            }

            return result;
        }
    }
}
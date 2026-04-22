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
    public class OtcDrugFileBlobEntityReader : QsAzureBlobStorageReaderBase<QhOtcDrugFileBlobEntity>, IQsAzureBlobStorageReader<QhOtcDrugFileBlobEntity, OtcDrugFileBlobEntityReaderArgs, OtcDrugFileBlobEntityReaderResults>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OtcDrugFileBlobEntityReader(): base(true, BlobContainerPublicAccessType.Off)
        {
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public OtcDrugFileBlobEntityReaderResults Execute(OtcDrugFileBlobEntityReaderArgs args)
        {
            var result = new OtcDrugFileBlobEntityReaderResults
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
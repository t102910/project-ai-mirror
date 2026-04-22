using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 施設関連の機能を提供します。
    /// </summary>
    public sealed class FacilityWorker
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="isOriginal"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool FileStorageRead(string fileKey, bool isOriginal, ref byte[] data, ref string contentType, ref string fileName)
        {
            if (isOriginal)
            {
                UploadFacilityOriginalFileBlobEntityReader reader = new UploadFacilityOriginalFileBlobEntityReader();
                UploadFacilityOriginalFileBlobEntityReaderArgs readerArgs = new UploadFacilityOriginalFileBlobEntityReaderArgs() { Name = fileKey.TryToValueType(Guid.Empty) };
                UploadFacilityOriginalFileBlobEntityReaderResults readerResults = reader.Execute(readerArgs);

                
                if (readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Data != null)
                {
                    contentType = readerResults.Result.ContentType;
                    fileName =readerResults.Result.OriginalName;
                    data = readerResults.Result.Data;
                    return true;
                }
                
            }
            else
            {
                UploadFacilityThumbnailFileBlobEntityReader reader = new UploadFacilityThumbnailFileBlobEntityReader();
                UploadFacilityThumbnailFileBlobEntityReaderArgs readerArgs = new UploadFacilityThumbnailFileBlobEntityReaderArgs() { Name = fileKey.TryToValueType(Guid.Empty) };
                UploadFacilityThumbnailFileBlobEntityReaderResults readerResults = reader.Execute(readerArgs);

               
                if (readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Data != null)
                {
                    contentType = readerResults.Result.ContentType;
                    data = readerResults.Result.Data;
                    fileName = readerResults.Result.OriginalName;
                    return true;
                }
                
            }
            return false;
        }

       


    }

}
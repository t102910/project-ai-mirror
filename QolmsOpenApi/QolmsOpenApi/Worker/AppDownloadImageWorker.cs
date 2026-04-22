using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 画像のダウンロード処理
    /// </summary>
    public class AppDownloadImageWorker
    {
        IStorageRepository _storageRepo;
        IMedicineStorageRepository _medicineStorageRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageRepository"></param>
        public AppDownloadImageWorker(IStorageRepository storageRepository, IMedicineStorageRepository medicineStorageRepo)
        {
            _storageRepo = storageRepository;
            _medicineStorageRepo = medicineStorageRepo;
        }

        /// <summary>
        /// ダウンロード実行
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="sizeType"></param>
        /// <param name="fileCategory"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        public HttpResponseMessage DownloadImage(string fileKey, QsApiFileTypeEnum sizeType = QsApiFileTypeEnum.Original, 
            QsApiFileCategoryTyepEnum fileCategory = QsApiFileCategoryTyepEnum.Upload, int seq = 0)
        {
            try
            {
                var fileItem = new QoApiFileItem();
                switch (fileCategory)
                {
                    // 調剤薬ファイル
                    case QsApiFileCategoryTyepEnum.EthDrug:
                        fileItem = _medicineStorageRepo.ReadEthImage(fileKey, seq);
                        break;

                    // 市販薬ファイル
                    case QsApiFileCategoryTyepEnum.OtcDrug:
                        (var itemCode, var itemCodeType) = SplitOtcFileKeyCode(fileKey);
                        fileItem = _medicineStorageRepo.ReadOtcImage(itemCode, itemCodeType, seq);
                        break;

                    // アップロード画像ファイル
                    case QsApiFileCategoryTyepEnum.Upload:
                    default:
                        var decodedKey = fileKey.ToDecrypedReference();
                        fileItem = _storageRepo.ReadImage(decodedKey.TryToValueType(Guid.Empty), sizeType);
                        break;
                }

                var data = Convert.FromBase64String(fileItem.Data);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(data)
                };
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(fileItem.ContentType);
                result.Content.Headers.Add("Content-Disposition", $"attachment;filename={HttpUtility.UrlEncode(fileItem.OriginalName)}");

                return result;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 市販薬の場合、FileKeyはItemCode+ItemCodeTypeとしてセットされているので分割する
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        (string, string) SplitOtcFileKeyCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return (string.Empty, string.Empty);
            }

            var itemoCodeType = code[code.Length - 1].ToString();
            string itemCode = code.Remove(code.Length - 1);

            return (itemCode, itemoCodeType);
        }
    }
}
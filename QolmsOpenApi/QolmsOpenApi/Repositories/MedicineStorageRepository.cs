using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// Storageへの入出力インターフェース（調剤薬・市販薬用）
    /// </summary>
    public interface IMedicineStorageRepository
    {
        /// <summary>
        /// 調剤薬画像の読み込み
        /// </summary>
        /// <param name="yjCode"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        QoApiFileItem ReadEthImage(string yjCode, int seq);

        /// <summary>
        /// 市販薬画像の読み込み
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="itemCodeType"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        QoApiFileItem ReadOtcImage(string itemCode, string itemCodeType, int seq);

        /// <summary>
        /// QH_ETHDRUGFILE_MSTのレコードを取得する
        /// </summary>
        /// <param name="yjCode"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        QH_ETHDRUGFILE_MST ReadEthFileEntity(string yjCode, int seq);

        /// <summary>
        /// QH_OTCDRUGFILE_MSTのレコードを取得する
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="itemCodeType"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        QH_OTCDRUGFILE_MST ReadOtcFileEntity(string itemCode, string itemCodeType, int seq);

        /// <summary>
        /// 調剤薬ファイル実体をBlobより取得します。
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        string ReadEthBlobEntity(Guid fileKey);

        /// <summary>
        /// 市販薬ファイル実体をBlobより取得します。
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        string ReadOtcBlobEntity(Guid fileKey);
    }

    /// <summary>
    /// Storageへの入出力実装（調剤薬・市販薬用）
    /// </summary>
    public class MedicineStorageRepository : QsDbReaderBase, IMedicineStorageRepository
    {
        /// <inheritdoc/>
        public QoApiFileItem ReadEthImage(string yjCode, int seq)
        {
            // 引数チェック
            if (string.IsNullOrEmpty(yjCode))
            {
                throw new ArgumentException("YJコードが不正です。");
            }

            // DBからデータ取得
            var entity = ReadEthFileEntity(yjCode, seq);
            if (entity == null)
            {
                throw new Exception("調剤薬ファイルが見つかりませんでした。");
            }

            // 取得したデータのFileKeyからBlob取得
            var data = ReadEthBlobEntity(entity.FILEKEY);

            return new QoApiFileItem
            {
                Data = data,
                ContentType = entity.CONTENTTYPE,
                OriginalName = $"{entity.FILEKEY}.jpg",             
            };
        }

        /// <inheritdoc/>
        public QoApiFileItem ReadOtcImage(string itemCode, string itemCodeType, int seq)
        {
            // 引数チェック
            if(string.IsNullOrEmpty(itemCode))
            {
                throw new ArgumentException("ItemCodeが不正です。");
            }

            if (string.IsNullOrEmpty(itemCodeType))
            {
                throw new ArgumentException("ItemCodeTypeが不正です。");
            }

            // 0がpdf、1以上が画像
            if(seq < 1)
            {
                throw new ArgumentException("seqが不正です。");
            }

            // DBからデータを取得
            var entity = ReadOtcFileEntity(itemCode, itemCodeType, seq);
            if (entity == null)
            {
                throw new Exception("市販薬ファイルが見つかりませんでした。");
            }

            // 取得したデータのFileKeyからBlob取得
            var data = ReadOtcBlobEntity(entity.FILEKEY);
            return new QoApiFileItem
            {
                Data = data,
                ContentType = entity.CONTENTTYPE,
                OriginalName = $"{entity.FILEKEY}.jpg",
            };
        }

        /// <inheritdoc/>
        public QH_ETHDRUGFILE_MST ReadEthFileEntity(string yjCode, int seq)
        {
            var reader = new QhEthDrugFileEntityReader();
            var args = new QhEthDrugFileEntityReaderArgs
            {
                Data = new List<QH_ETHDRUGFILE_MST>
                {
                    new QH_ETHDRUGFILE_MST
                    {
                        YJCODE = yjCode,
                        SEQUENCE = seq
                    }
                }
            };

            var results = QsDbManager.Read(reader, args);

            if (!results.IsSuccess)
            {
                throw new Exception("ファイル情報の取得に失敗しました");
            }

            if (results.Result.Count == 1 && !results.Result.First().DELETEFLAG)
            {
                return results.Result.First();
            }

            return null;
        }

        /// <inheritdoc/>
        public QH_OTCDRUGFILE_MST ReadOtcFileEntity(string itemCode, string itemCodeType, int seq)
        {
            var reader = new QhOtcDrugFileEntityReader();
            var args = new QhOtcDrugFileEntityReaderArgs
            {
                Data = new List<QH_OTCDRUGFILE_MST>
                {
                    new QH_OTCDRUGFILE_MST
                    {
                        ITEMCODE = itemCode,
                        ITEMCODETYPE = itemCodeType,
                        SEQUENCE = seq,
                        CONTENTTYPE = "image/jpeg"
                    }
                }
            };

            var results = QsDbManager.Read(reader, args);

            if (!results.IsSuccess)
            {
                throw new Exception("ファイル情報の取得に失敗しました");
            }
 
            if (results.Result.Count == 1 && !results.Result.First().DELETEFLAG)
            {
                return results.Result.First();
            }

            return null;
        }

        /// <inheritdoc/>
        public string ReadEthBlobEntity(Guid fileKey)
        {
            var args = new EthDrugFileBlobEntityReaderArgs
            {
                Name = fileKey
            };

            var reader = new EthDrugFileBlobEntityReader();

            var results = reader.Execute(args);

            if (results.IsSuccess && results.Result != null)
            {
                return Convert.ToBase64String(results.Result.Data);
            }

            throw new Exception("調剤薬Blobファイルの読み込みに失敗しました。");
        }

        /// <inheritdoc/>
        public string ReadOtcBlobEntity(Guid fileKey)
        {
            var args = new OtcDrugFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var reader = new OtcDrugFileBlobEntityReader();
            var results = reader.Execute(args);

            if (results.IsSuccess && results.Result != null)
            {
                return Convert.ToBase64String(results.Result.Data);
            }

            throw new Exception("市販薬Blobファイルの読み込みに失敗しました。");
        }
    }
}
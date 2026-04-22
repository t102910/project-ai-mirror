using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// 医療用医薬品マスタ 入出力インターフェース
    /// </summary>
    public interface IMedicineRepository
    {
        /// <summary>
        /// 医薬品を検索します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="searchText"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        DbEthicalDrugSearcherResults SearchMedicine(Guid accountKey, string searchText, int pageIndex, int pageSize);

        /// <summary>
        /// 医薬品の詳細を取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="yjCode"></param>
        /// <returns></returns>
        QH_ETHDRUG_DETAIL_VIEW ReadDetail(string yjCode);

        /// <summary>
        /// 医薬品ファイル情報をYJコードからDBより取得します。
        /// </summary>
        /// <param name="yjCode"></param>
        /// <returns></returns>
        List<QH_ETHDRUGFILE_MST> ReadEthDrugFileEntity(string yjCode);

        /// <summary>
        /// 医薬品ファイル情報をDBより取得します。
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        QH_ETHDRUGFILE_MST ReadEthDrugFileEntity(Guid fileKey);

        /// <summary>
        /// 医薬品ファイル実体をBlobより取得します。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        (string base64data, string contentType, string originalName) ReadEthDrugFileBlob(QH_ETHDRUGFILE_MST entity);
    }

    /// <summary>
    /// 医療用医薬品マスタ 入出力実装
    /// </summary>
    public class MedicineRepository:QsDbReaderBase, IMedicineRepository
    {

        /// <summary>
        /// 医薬品を検索します
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="searchText"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public DbEthicalDrugSearcherResults SearchMedicine(Guid accountKey, string searchText, int pageIndex, int pageSize)
        {
            var searcher = new DbEthicalDrugSearcher();
            var args = new DbEthicalDrugSearcherArgs
            {
                AccountKey = accountKey,
                SearchText = searchText,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = QsDbManager.Read(searcher, args);

            // 結果null、失敗、医薬品リストnullは例外とする
            if(result == null || !result.IsSuccess || result.EthicalDrugN == null)
            {
                throw new Exception();
            }

            return result;
        }

        /// <summary>
        /// 医薬品の詳細を取得します。
        /// </summary>
        /// <param name="yjCode"></param>
        public QH_ETHDRUG_DETAIL_VIEW ReadDetail(string yjCode)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ETHDRUGMEIGARA_MST>())
            {
                var param = new List<DbParameter>() {
                    CreateParameter(con,"@p1", yjCode),
                };

                // 詳細情報を取得
                var sql = $@"
                    SELECT 
                        t1.YJCODE,
                        t1.PRODUCTNAME,
                        t1.COMMONNAME,
                        t1.APPROVALCOMPANYNAME,
                        t1.SALESCOMPANYNAME,
                        t1.INGREDIENTS,
                        t1.GENERALCODE,
                        t2.ACTIONA,
                        t2.ACTIONB,
                        t2.ACTIONC1,
                        t2.ACTIONC2,
                        t2.PRECAUTIONS,
                        t3.DRUGORFOOD,
                        t3.INTERACTION
                     FROM  {nameof(QH_ETHDRUGMEIGARA_MST)} t1
                     LEFT OUTER JOIN ( SELECT * FROM {nameof(QH_ETHDRUGYJHA_MST)} WHERE DELETEFLAG = 0 ) t2
                     ON t1.YJCODE = t2.YJCODE
                     LEFT OUTER JOIN ( SELECT * FROM {nameof(QH_ETHDRUGYJHS_MST)} WHERE DELETEFLAG = 0 ) t3
                     ON t1.YJCODE = t3.YJCODE
                     WHERE t1.YJCODE = @p1
                     AND t1.DELETEFLAG = 0 ";
                
                con.Open();

                var result = ExecuteReader<QH_ETHDRUG_DETAIL_VIEW>(con, null, sql, param).FirstOrDefault();

                if (result != null)
                {
                    result.FileEntityN = ReadEthDrugFileEntity(yjCode);

                }
                else
                {
                    result = new QH_ETHDRUG_DETAIL_VIEW();
                }

                return result;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<QH_ETHDRUGFILE_MST> ReadEthDrugFileEntity(string yjCode)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ETHDRUGFILE_MST>())
            {
                var param = new List<DbParameter>() {
                        CreateParameter(con,"@p1", yjCode),
                    };

                // 画像キーを取得
                var sql = $@"
                    SELECT *
                    FROM {nameof(QH_ETHDRUGFILE_MST)}
                    WHERE {nameof(QH_ETHDRUGFILE_MST.YJCODE)} = @p1
                    AND {nameof(QH_ETHDRUGFILE_MST.DELETEFLAG)} = 0 ";

                con.Open();

                var result = ExecuteReader<QH_ETHDRUGFILE_MST>(con, null, sql, param);

                return result;
            }
        }

        /// <summary>
        /// 医薬品ファイル情報をDBより取得します。
        /// </summary>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        public QH_ETHDRUGFILE_MST ReadEthDrugFileEntity(Guid fileKey)
        {
            var reader = new MasterEthDrugFileReader();
            var args = new MasterEthDrugFileReaderArgs
            {
                FileKey = fileKey
            };

            var result = QsDbManager.Read(reader, args);

            if(result == null || !result.IsSuccess || 
                result.Result == null || !result.Result.Any())
            {
                throw new Exception();
            }

            return result.Result.First();
        }

        /// <summary>
        /// 医薬品ファイル実体をBlobより取得します。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (string base64data, string contentType, string originalName) ReadEthDrugFileBlob(QH_ETHDRUGFILE_MST entity)
        {            

            var reader = new EthDrugFileBlobEntityReader();
            var args = new EthDrugFileBlobEntityReaderArgs
            {
                Name = entity.FILEKEY
            };

            var blobResult = reader.Execute(args);

            if(!blobResult.IsSuccess || blobResult.Result == null)
            {
                throw new Exception();
            }

            var base64data = blobResult.Result.Data == null ?
                string.Empty : Convert.ToBase64String(blobResult.Result.Data);

            return (base64data, blobResult.Result.ContentType, $"{entity.YJCODE}.jpg");
        }
    }
}
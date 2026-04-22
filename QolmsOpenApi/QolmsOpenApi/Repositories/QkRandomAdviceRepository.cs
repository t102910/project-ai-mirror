using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// ランダムアドバイスの入出力インターフェース
    /// </summary>
    public interface IQkRandomAdviceRepository
    {
        /// <summary>
        /// IDを指定してアドバイスを取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        QK_RANDOMADVICE_MST Read(int id);

        /// <summary>
        /// アドバイスのリストを取得
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        List<QK_RANDOMADVICE_MST> ReadList(string modelId);

        /// <summary>
        /// 対応した月のアドバイスのリストを取得
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<QK_RANDOMADVICE_MST> ReadSeasonList(string modelId, int month);

        /// <summary>
        /// ランダムアドバイスの候補のリストを取得
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="month"></param>
        /// <param name="timeType"></param>
        /// <param name="excludeIds">除外するIDリスト</param>
        /// <returns></returns>
        List<QK_RANDOMADVICE_MST> ReadRandomList(string modelId, int month, QkAdviceTimeType timeType, List<int> excludeIds = null);
    }

    /// <summary>
    /// ランダムアドバイスの入出力実装
    /// </summary>
    public class QkRandomAdviceRepository: QsDbReaderBase, IQkRandomAdviceRepository
    {
        /// <summary>
        /// アドバイスのリストを取得
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public List<QK_RANDOMADVICE_MST> ReadList(string modelId)
        {
            using (var con = QsDbManager.CreateDbConnection<QK_RANDOMADVICE_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", modelId)
                };

                var sql = $@"
                    Select *
                    From {nameof(QK_RANDOMADVICE_MST)}
                    Where {nameof(QK_RANDOMADVICE_MST.DELETEFLAG)} = 0
                    And {nameof(QK_RANDOMADVICE_MST.MODELID)} = @p1
                    Order By ID
                ";

                con.Open();

                return ExecuteReader<QK_RANDOMADVICE_MST>(con, null, sql, paramList);
            }
        }

        /// <inheritdoc/>
        public List<QK_RANDOMADVICE_MST> ReadSeasonList(string modelId, int month)
        {
            if (month < 1 || month > 12)
            {
                month = 1; // 無効な月が指定されたら1月固定とする
            }

            using (var con = QsDbManager.CreateDbConnection<QK_RANDOMADVICE_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", modelId)
                };

                var sql = $@"
                    Select *
                    From {nameof(QK_RANDOMADVICE_MST)}
                    WHERE {nameof(QK_RANDOMADVICE_MST.DELETEFLAG)} = 0
                    AND {nameof(QK_RANDOMADVICE_MST.MODELID)} = @p1
                    AND M{month} = 1
                    ORDER BY ID
                ";
               

                con.Open();

                return ExecuteReader<QK_RANDOMADVICE_MST>(con, null, sql, paramList);
            }
        }

        /// <summary>
        /// IDを指定してアドバイスを取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QK_RANDOMADVICE_MST Read(int id)
        {
            using (var con = QsDbManager.CreateDbConnection<QK_RANDOMADVICE_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", id)
                };

                var sql = $@"
                    Select *
                    From {nameof(QK_RANDOMADVICE_MST)}
                    Where {nameof(QK_RANDOMADVICE_MST.DELETEFLAG)} = 0
                    And {nameof(QK_RANDOMADVICE_MST.ID)} = @p1
                ";

                con.Open();

                var result = ExecuteReader<QK_RANDOMADVICE_MST>(con, null, sql, paramList);
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// ランダムアドバイスの候補のリストを取得
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="month"></param>
        /// <param name="timeType"></param>
        /// <param name="excludeIds"></param>
        /// <returns></returns>
        public List<QK_RANDOMADVICE_MST> ReadRandomList(string modelId, int month, QkAdviceTimeType timeType, List<int> excludeIds = null)
        {
            if(month < 1 || month > 12)
            {
                month = 1; // 無効な月が指定されたら1月固定とする
            }

            using (var con = QsDbManager.CreateDbConnection<QK_RANDOMADVICE_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", modelId),
                    CreateParameter(con,"@p2", timeType)
                };

                var sql = $@"
                    Select *
                    From {nameof(QK_RANDOMADVICE_MST)}
                    WHERE DELETEFLAG = 0
                    AND {nameof(QK_RANDOMADVICE_MST.MODELID)} = @p1
                    AND {nameof(QK_RANDOMADVICE_MST.TIMETYPE)} In (0,@p2)
                    AND M{month} = 1
                ";

                if(excludeIds != null && excludeIds.Any())
                {
                    var condition = $"AND {nameof(QK_RANDOMADVICE_MST.ID)} Not In({string.Join(",",excludeIds)})";
                    sql += condition;
                }

                con.Open();

                return ExecuteReader<QK_RANDOMADVICE_MST>(con, null, sql, paramList);
            }
        }
    }
}
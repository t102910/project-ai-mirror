using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface ILocalPointRepository
    {
        QoJotoHdrLocalPointSummaryReadApiResults ExecuteLocalPointSummaryReadApi(QolmsJotoModel mainModel, string linkageSystemId, string apikey);

        QoJotoHdrLocalPointHistoryReadApiResults ExecuteLocalPointHistoryReadApi(QolmsJotoModel mainModel, DateTime targetDate, string linkageSystemId, string apikey);

        QoJotoHdrLocalPointRedeemReadApiResults ExecuteLocalPointRedeemReadApi(QolmsJotoModel mainModel, int redeemPoint, string linkageSystemId, string apikey);
    }

    public class LocalPointRepository : ILocalPointRepository
    {
        private static void WriteLocalPointSummaryErrorLog(QoJotoHdrLocalPointSummaryReadApiArgs apiArgs, QoJotoHdrLocalPointSummaryReadApiResults apiResults)
        {
            AccessLogWorker.WriteErrorLog(
                null,
                String.Empty,
                string.Format("LocalPointSummaryRead API failed. LinkageId={0}, Result={1}", apiArgs.LinkageId, apiResults.Result)
            );
        }

        private static void WriteLocalPointHistoryErrorLog(QoJotoHdrLocalPointHistoryReadApiArgs apiArgs, QoJotoHdrLocalPointHistoryReadApiResults apiResults)
        {
            AccessLogWorker.WriteErrorLog(
                null,
                String.Empty,
                string.Format("LocalPointHistoryRead API failed. LinkageId={0}, Month={1}, Result={2}", apiArgs.LinkageId, apiArgs.Month, apiResults.Result)
            );
        }

        private static void WriteLocalPointRedeemErrorLog(QoJotoHdrLocalPointRedeemReadApiArgs apiArgs, QoJotoHdrLocalPointRedeemReadApiResults apiResults)
        {
            AccessLogWorker.WriteErrorLog(
                null,
                String.Empty,
                string.Format("LocalPointRedeemRead API failed. LinkageId={0}, Points={1}, Result={2}", apiArgs.LinkageId, apiArgs.Points, apiResults.Result)
            );
        }

        /// <summary>
        /// 「地域ポイント履歴」画面のポイント合計取得 API を実行します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        /// <returns>
        /// 成功なら Open API 戻り値 クラス、
        /// 失敗なら例外を スロー。
        /// </returns>
        public QoJotoHdrLocalPointSummaryReadApiResults ExecuteLocalPointSummaryReadApi(QolmsJotoModel mainModel, string linkageSystemId, string apikey)
        {
            var apiArgs = new QoJotoHdrLocalPointSummaryReadApiArgs(
                QoApiTypeEnum.JotoHdrLocalPointSummaryRead,
                QsApiSystemTypeEnum.JotoGinowan,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                LinkageId = linkageSystemId
            };
            QoJotoHdrLocalPointSummaryReadApiResults apiResults;
            try
            {
                apiResults = QsApiManager.ExecuteQolmsOpenApi<QoJotoHdrLocalPointSummaryReadApiResults>(
                    apiArgs, apikey
                );
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, String.Empty, ex);
                throw;
            }

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                WriteLocalPointSummaryErrorLog(apiArgs, apiResults);
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)));
            }
        }

        /// <summary>
        /// 「地域ポイント履歴」画面のポイント履歴取得 API を実行します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        /// <returns>
        /// 成功なら Open API 戻り値 クラス、
        /// 失敗なら例外を スロー。
        /// </returns>
        public QoJotoHdrLocalPointHistoryReadApiResults ExecuteLocalPointHistoryReadApi(QolmsJotoModel mainModel, DateTime targetDate, string linkageSystemId, string apikey)
        {
            var apiArgs = new QoJotoHdrLocalPointHistoryReadApiArgs(
                QoApiTypeEnum.JotoHdrLocalPointHistoryRead,
                QsApiSystemTypeEnum.JotoGinowan,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                LinkageId = linkageSystemId,
                Month = targetDate.ToApiDateString()
            };
            QoJotoHdrLocalPointHistoryReadApiResults apiResults;
            try
            {
                apiResults = QsApiManager.ExecuteQolmsOpenApi<QoJotoHdrLocalPointHistoryReadApiResults>(
                    apiArgs, apikey
                );
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, String.Empty, ex);
                throw;
            }

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                WriteLocalPointHistoryErrorLog(apiArgs, apiResults);
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)));
            }
        }

        /// <summary>
        /// 「地域ポイント履歴」画面のポイント変換 API を実行します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        /// <returns>
        /// 成功なら Open API 戻り値 クラス、
        /// 失敗なら例外を スロー。
        /// </returns>
        public QoJotoHdrLocalPointRedeemReadApiResults ExecuteLocalPointRedeemReadApi(QolmsJotoModel mainModel, int redeemPoint, string linkageSystemId, string apikey)
        {
            var apiArgs = new QoJotoHdrLocalPointRedeemReadApiArgs(
                QoApiTypeEnum.JotoHdrLocalPointRedeemRead,
                QsApiSystemTypeEnum.JotoGinowan,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                LinkageId = linkageSystemId,
                Points = (-Math.Abs(redeemPoint)).ToString(),
                IdempotencyKey = Guid.NewGuid().ToString()
            };

            QoJotoHdrLocalPointRedeemReadApiResults apiResults;
            try
            {
                apiResults = QsApiManager.ExecuteQolmsOpenApi<QoJotoHdrLocalPointRedeemReadApiResults>(
                    apiArgs, apikey
                );
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, String.Empty, ex);
                throw;
            }

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                WriteLocalPointRedeemErrorLog(apiArgs, apiResults);
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)));
            }
        }
    }
}

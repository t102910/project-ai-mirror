using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IPontaExchangeRepository
    {

        QhYappliPortalAuWalletPointExchangeItemReadApiResults ExecuteAuPointExchangeItemReadApi(QolmsJotoModel mainModel, DateTime actionDate, int id);
        QhYappliPortalAuWalletPointExchangeReadApiResults ExecuteAuWalletPointReadApi(QolmsJotoModel mainModel);

        QhYappliPortalAuWalletPointExchangeWriteApiResults ExecuteAuWalletPointWriteApi(QolmsJotoModel mainModel, DateTime actionDate, int AuWalletPointItemId, int demandPoint, string serialCode, string requestId, string actionType, string pointReceiptNo, string responseDate, string responseTime, int httpStatusCode, string result, bool deleteFlag);
    }

    public class PontaExchangeRepository: IPontaExchangeRepository
    {
        public QhYappliPortalAuWalletPointExchangeItemReadApiResults ExecuteAuPointExchangeItemReadApi(QolmsJotoModel mainModel, DateTime actionDate, int id)
        {
            QhYappliPortalAuWalletPointExchangeItemReadApiArgs apiArgs = new QhYappliPortalAuWalletPointExchangeItemReadApiArgs(
                QhApiTypeEnum.YappliPortalAuWalletPointMasterRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActionDate = actionDate.ToApiDateString(),
                AuWalletPointItemId = id.ToString()
            };

            QhYappliPortalAuWalletPointExchangeItemReadApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalAuWalletPointExchangeItemReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                if (string.IsNullOrEmpty(apiResults.AuWalletPointItemId))
                {
                    //DebugLog(string.Format("Pontaの交換マスタがありません。id:{0}", id));
                    throw new Exception(string.Format("Pontaの交換マスタがありません。id:{0}", id)); // とりあえずわかればいいので
                }
                else
                {
                    return apiResults;
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

        public QhYappliPortalAuWalletPointExchangeReadApiResults ExecuteAuWalletPointReadApi(QolmsJotoModel mainModel)
        {
            QhYappliPortalAuWalletPointExchangeReadApiArgs apiArgs = new QhYappliPortalAuWalletPointExchangeReadApiArgs(
                QhApiTypeEnum.YappliPortalAuWalletPointRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            };

            QhYappliPortalAuWalletPointExchangeReadApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalAuWalletPointExchangeReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

        public QhYappliPortalAuWalletPointExchangeWriteApiResults ExecuteAuWalletPointWriteApi(
            QolmsJotoModel mainModel,
            DateTime actionDate,
            int AuWalletPointItemId,
            int demandPoint,
            string serialCode,
            string requestId,
            string actionType,
            string pointReceiptNo,
            string responseDate,
            string responseTime,
            int httpStatusCode,
            string result,
            bool deleteFlag)
        {
            byte matchResultType = 0; // Enum
            QhYappliPortalAuWalletPointExchangeWriteApiArgs apiArgs = new QhYappliPortalAuWalletPointExchangeWriteApiArgs(
                QhApiTypeEnum.YappliPortalAuWalletPointWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                ActionDate = actionDate.ToApiDateString(),
                AuWalletPointItemId = AuWalletPointItemId.ToString(),
                SerialCode = serialCode,
                DemandPoint = demandPoint.ToString(),
                RequestId = requestId,
                ActionType = actionType,
                PointReceiptNo = pointReceiptNo,
                ResponseDate = responseDate,
                ResponseTime = responseTime,
                HttpStatusCode = httpStatusCode.ToString(),
                Result = result,
                MatchResultType = matchResultType.ToString(),
                DeleteFlag = deleteFlag.ToString()
            };

            QhYappliPortalAuWalletPointExchangeWriteApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalAuWalletPointExchangeWriteApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

    }
}
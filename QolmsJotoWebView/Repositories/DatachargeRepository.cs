using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IDatachargeRepository
    {
        QhYappliPortalDatachargeEventIdReadApiResults ExecuteDatachargeEventIdReadApi(QolmsJotoModel mainModel, DateTime actionDate, int capacity);
        QhYappliPortalDatachargeReadApiResults ExecuteDatachargeReadApi(QolmsJotoModel mainModel);
        QhYappliPortalDatachageLogWriteApiResults ExecuteDatachargeLogWriteApi(QolmsJotoModel mainModel, int actionType, string requestId, string eventId, string requestDate, string responseDate, int httpStatusCode, string result, string serialCode, string comment);
    }

    public class DatachargeRepository: IDatachargeRepository
    {
        public QhYappliPortalDatachargeEventIdReadApiResults ExecuteDatachargeEventIdReadApi(
            QolmsJotoModel mainModel, DateTime actionDate, int capacity)
        {
            QhYappliPortalDatachargeEventIdReadApiArgs apiArgs = new QhYappliPortalDatachargeEventIdReadApiArgs(
                QhApiTypeEnum.YappliPortalDatachargeEventIdRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                ActionDate = actionDate.ToApiDateString(),
                DataSize = capacity.ToString()
            };

            QhYappliPortalDatachargeEventIdReadApiResults apiResults =
                QsApiManager.ExecuteQolmsApi<QhYappliPortalDatachargeEventIdReadApiResults>(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                if (string.IsNullOrEmpty(apiResults.EventId))
                {
                    throw new Exception(string.Format("{0}MB のデータチャージのイベントIDマスタがありません。", capacity));
                }
                else
                {
                    return apiResults;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

        public QhYappliPortalDatachargeReadApiResults ExecuteDatachargeReadApi(QolmsJotoModel mainModel)
        {
            QhYappliPortalDatachargeReadApiArgs apiArgs = new QhYappliPortalDatachargeReadApiArgs(
                QhApiTypeEnum.YappliPortalDatachargeRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            };

            QhYappliPortalDatachargeReadApiResults apiResults =
                QsApiManager.ExecuteQolmsApi<QhYappliPortalDatachargeReadApiResults>(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

        public QhYappliPortalDatachageLogWriteApiResults ExecuteDatachargeLogWriteApi(
            QolmsJotoModel mainModel, int actionType, string requestId, string eventId,
            string requestDate, string responseDate, int httpStatusCode, string result,
            string serialCode, string comment)
        {
            QhYappliPortalDatachageLogWriteApiArgs apiArgs = new QhYappliPortalDatachageLogWriteApiArgs(
                QhApiTypeEnum.YappliPortalDatachargeLogWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                ActionDate = DateTime.Now.ToApiDateString(),
                ActionType = actionType.ToString(),
                RequestId = requestId,
                EventId = eventId,
                RequestDate = requestDate,
                ResponseDate = responseDate,
                HttpStatusCode = httpStatusCode.ToString(),
                Result = result,
                SerialCode = serialCode,
                Comment = comment
            };

            QhYappliPortalDatachageLogWriteApiResults apiResults =
                QsApiManager.ExecuteQolmsApi<QhYappliPortalDatachageLogWriteApiResults>(
                    apiArgs,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }
    }
}
using System;

using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface ILinkageRepository
    {
        QoLinkageLinkageSystemIdReadApiResults ExecuteLinkageSystemIdReadApi(QolmsJotoModel mainModel, string apikey);

        LinkageItem ExecuteLinkageReadApi(QolmsJotoModel mainModel, int linkageSystemNo);
    }

    public class LinkageRepository : ILinkageRepository
    {
        private const int JOTO_GINOWAN_SYSTEM_NO = 47900021;
        private const byte APPROVED_STATUS_TYPE = (byte)QsDbLinkageStatusTypeEnum.Approved;

        private static void WriteApiFailureLog(QoLinkageLinkageSystemIdReadApiArgs apiArgs, QoLinkageLinkageSystemIdReadApiResults apiResults)
        {
            AccessLogWorker.WriteErrorLog(
                null,
                String.Empty,
                string.Format(
                    "LinkageSystemIdRead API failed. ActorKey={0}, LinkageSystemNo={1}, StatusType={2}, Result={3}",
                    apiArgs.ActorKey,
                    apiArgs.LinkageSystemNo,
                    apiArgs.StatusType,
                    apiResults.Result
                )
            );
        }

        private static void WriteNotFoundLog(QoLinkageLinkageSystemIdReadApiArgs apiArgs)
        {
            AccessLogWorker.WriteErrorLog(
                null,
                String.Empty,
                string.Format(
                    "LinkageSystemId record not found. ActorKey={0}, LinkageSystemNo={1}, StatusType={2}",
                    apiArgs.ActorKey,
                    apiArgs.LinkageSystemNo,
                    apiArgs.StatusType
                )
            );
        }

        /// <summary>
        /// アカウントキーから LinkageSystemId を取得する API を実行します。
        /// </summary>
        public QoLinkageLinkageSystemIdReadApiResults ExecuteLinkageSystemIdReadApi(QolmsJotoModel mainModel, string apikey)
        {
            var apiArgs = new QoLinkageLinkageSystemIdReadApiArgs(
                QoApiTypeEnum.LinkageLinkageSystemIdRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = JOTO_GINOWAN_SYSTEM_NO.ToString(),
                StatusType = APPROVED_STATUS_TYPE.ToString()
            };

            QoLinkageLinkageSystemIdReadApiResults apiResults;
            try
            {
                apiResults = QsApiManager.ExecuteQolmsOpenApi<QoLinkageLinkageSystemIdReadApiResults>(
                    apiArgs,
                    apikey
                );
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, String.Empty, ex);
                throw;
            }

            if (apiResults.IsSuccess.TryToValueType(false) && !string.IsNullOrWhiteSpace(apiResults.LinkageSystemId))
            {
                return apiResults;
            }

            if (!apiResults.IsSuccess.TryToValueType(false))
            {
                WriteApiFailureLog(apiArgs, apiResults);
            }
            else
            {
                WriteNotFoundLog(apiArgs);
            }

            throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)));
        }


        /// <summary>
        /// 連携システム番号を指定して、JOTO API から連携情報を取得します。
        /// </summary>
        /// <param name="mainModel">メインモデル</param>
        /// <param name="linkageSystemNo">連携システム番号</param>
        /// <returns></returns>
        public LinkageItem ExecuteLinkageReadApi(QolmsJotoModel mainModel, int linkageSystemNo)
        {
            QjCommonLinkageReadApiArgs apiArgs = new QjCommonLinkageReadApiArgs(
                QjApiTypeEnum.CommonLinkageRead,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString()
            };

            QjCommonLinkageReadApiResults apiResults = QsApiManager.ExecuteQolmsJotoApi<QjCommonLinkageReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey2
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return new LinkageItem() { 
                    Dataset = apiResults.Dataset,
                     Facilitykey = apiResults.Facilitykey.TryToValueType(Guid.Empty),
                     LinkageSystemId = apiResults.LinkageSystemId,
                     LinkageSystemName = apiResults.LinkageSystemName,
                     LinkageSystemNo = apiResults.LinkageSystemNo.TryToValueType(int.MinValue),
                     StatusType = apiResults.StatusType.TryToValueType(QjLinkageStatusTypeEnum.None)
                } ;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs))
                );
            }
        }

    }
}
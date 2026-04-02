using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IAmazonGiftCardRepository
    {
        QhYappliPortalAmazonGiftCardReadApiResults ExecuteAmazonGiftCardReadApi(QolmsJotoModel mainModel);
        QhYappliPortalAmazonGiftCardMasterReadApiResults ExecuteAmazonGiftCardMasterReadApi(QolmsJotoModel mainModel, byte giftCardType);

        QhYappliPortalAmazonGiftCardWriteApiResults ExecuteAmazonGiftCardWriteApi(QolmsJotoModel mainModel, byte giftCardType, string serialCode);
    }

    public class AmazonGiftCardRepository: IAmazonGiftCardRepository
    {

        public QhYappliPortalAmazonGiftCardReadApiResults ExecuteAmazonGiftCardReadApi(QolmsJotoModel mainModel)
        {
            var apiArgs = new QhYappliPortalAmazonGiftCardReadApiArgs(
                QhApiTypeEnum.YappliPortalAmazonGiftCardRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalAmazonGiftCardReadApiResults>(
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
                throw new InvalidOperationException(
                    string.Format(
                        "{0} API の実行に失敗しました。",
                        QsApiManager.GetQolmsApiName(apiArgs)
                    )
                );
            }
        }

        public QhYappliPortalAmazonGiftCardMasterReadApiResults ExecuteAmazonGiftCardMasterReadApi(QolmsJotoModel mainModel, byte giftCardType)
        {
            var apiArgs = new QhYappliPortalAmazonGiftCardMasterReadApiArgs(
                QhApiTypeEnum.YappliPortalAmazonGiftCardMasterRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                GiftCardType = giftCardType.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalAmazonGiftCardMasterReadApiResults>(
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
                throw new InvalidOperationException(
                    string.Format(
                        "{0} API の実行に失敗しました。",
                        QsApiManager.GetQolmsApiName(apiArgs)
                    )
                );
            }
        }

        public QhYappliPortalAmazonGiftCardWriteApiResults ExecuteAmazonGiftCardWriteApi(QolmsJotoModel mainModel, byte giftCardType, string serialCode)
        {
            var apiArgs = new QhYappliPortalAmazonGiftCardWriteApiArgs(
                QhApiTypeEnum.YappliPortalAmazonGiftCardWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                GiftCardType = giftCardType.ToString(),
                SerialCode = serialCode
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalAmazonGiftCardWriteApiResults>(
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
                throw new InvalidOperationException(
                    string.Format(
                        "{0} API の実行に失敗しました。",
                        QsApiManager.GetQolmsApiName(apiArgs)
                    )
                );
            }
        }
    }
}
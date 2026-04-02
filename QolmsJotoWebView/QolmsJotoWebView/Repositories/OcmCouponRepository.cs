using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IOcmCouponRepository
    {
        QhYappliPortalPointExchangeReadApiResults ExecutePointExchangeReadApi(QolmsJotoModel mainModel);
        QhYappliPortalPointExchangeMasterReadApiResults ExecutePointExchangeMasterReadApi(QolmsJotoModel mainModel, byte couponType);
        QhYappliPortalPointExchangeWriteApiResults ExecutePointExchangeWriteApi(QolmsJotoModel mainModel, int couponType, string serialCode);
    }

    class OcmCouponRepository : IOcmCouponRepository
    {
        public QhYappliPortalPointExchangeReadApiResults ExecutePointExchangeReadApi(QolmsJotoModel mainModel)
        {
            var apiArgs = new QhYappliPortalPointExchangeReadApiArgs(
                QhApiTypeEnum.YappliPortalPointExchangeRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalPointExchangeReadApiResults>(
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
                    string.Format("{0} API の実行に失敗しました。",
                    QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }

        public QhYappliPortalPointExchangeMasterReadApiResults ExecutePointExchangeMasterReadApi(QolmsJotoModel mainModel, byte couponType)
        {
            var apiArgs = new QhYappliPortalPointExchangeMasterReadApiArgs(
                QhApiTypeEnum.YappliPortalPointExchangeMasterRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                CouponType = couponType.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalPointExchangeMasterReadApiResults>(
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
                    string.Format("{0} API の実行に失敗しました。",
                    QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }

        public QhYappliPortalPointExchangeWriteApiResults ExecutePointExchangeWriteApi(QolmsJotoModel mainModel, int couponType, string serialCode)
        {
            var apiArgs = new QhYappliPortalPointExchangeWriteApiArgs(
                QhApiTypeEnum.YappliPortalPointExchangeWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                CouponType = couponType.ToString(),
                SerialCode = serialCode
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalPointExchangeWriteApiResults>(
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
                    string.Format("{0} API の実行に失敗しました。",
                    QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }
    }
}
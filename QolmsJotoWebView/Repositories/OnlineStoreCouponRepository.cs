using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IOnlineStoreCouponRepository
    {
        QjPortalCouponForFitbitReadApiResults ExecuteCouponForFitbitReadApi(QolmsJotoModel mainModel);
        QjPortalCouponForFitbitMasterReadApiResults ExecuteCouponForFitbitMasterReadApi(QolmsJotoModel mainModel, byte couponType);
        QjPortalCouponForFitbitWriteApiResults ExecuteCouponForFitbitWriteApi(QolmsJotoModel mainModel, int couponType, string serialCode);
    }

    public class OnlineStoreCouponRepository: IOnlineStoreCouponRepository
    {

        public QjPortalCouponForFitbitReadApiResults ExecuteCouponForFitbitReadApi(QolmsJotoModel mainModel)
        {
            var apiArgs = new QjPortalCouponForFitbitReadApiArgs(
                QjApiTypeEnum.PortalCouponForFitbitRead,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            };

            var apiResults = QsApiManager.ExecuteQolmsJotoApi<QjPortalCouponForFitbitReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey2
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。",
                    QsApiManager.GetQolmsJotoApiName(apiArgs))
                );
            }
        }

        public QjPortalCouponForFitbitMasterReadApiResults ExecuteCouponForFitbitMasterReadApi(QolmsJotoModel mainModel, byte couponType)
        {
            var apiArgs = new QjPortalCouponForFitbitMasterReadApiArgs(
                QjApiTypeEnum.PortalCouponForFitbitMasterRead,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                CouponType = couponType.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsJotoApi<QjPortalCouponForFitbitMasterReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey2
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。",
                    QsApiManager.GetQolmsJotoApiName(apiArgs))
                );
            }
        }

        public QjPortalCouponForFitbitWriteApiResults ExecuteCouponForFitbitWriteApi(QolmsJotoModel mainModel, int couponType, string serialCode)
        {
            var apiArgs = new QjPortalCouponForFitbitWriteApiArgs(
                QjApiTypeEnum.PortalCouponForFitbitWrite,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                CouponType = couponType.ToString(),
                SerialCode = serialCode
            };

            var apiResults = QsApiManager.ExecuteQolmsJotoApi<QjPortalCouponForFitbitWriteApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey2
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。",
                    QsApiManager.GetQolmsJotoApiName(apiArgs))
                );
            }
        }
    }
}
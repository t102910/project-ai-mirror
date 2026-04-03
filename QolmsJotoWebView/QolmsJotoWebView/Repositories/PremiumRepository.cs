using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IPremiumRepository
    {
        /// <summary>
        /// プレミアム会員情報を取得します。
        /// </summary>
        QhYappliPremiumReadApiResults GetPremiumInfo(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid actorAccountKey);

        /// <summary>
        /// プレミアム会員退会処理を行います（DB レコードをキャンセル済みに更新）。
        /// </summary>
        void CancelPremium(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid actorAccountKey, QhYappliPremiumReadApiResults premiumInfo);
    }

    public class PremiumRepository : IPremiumRepository
    {
        /// <summary>
        /// プレミアム会員情報を取得します。
        /// </summary>
        public QhYappliPremiumReadApiResults GetPremiumInfo(
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey,
            Guid actorAccountKey)
        {
            var apiArgs = new QhYappliPremiumReadApiArgs(
                QhApiTypeEnum.YappliPremiumRead,
                QsApiSystemTypeEnum.Qolms,
                apiExecutor,
                apiExecutorName
            )
            {
                ActorKey = actorAccountKey.ToApiGuidString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPremiumReadApiResults>(
                apiArgs,
                sessionId,
                apiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

        /// <summary>
        /// プレミアム会員退会処理を行います（StatusNo=10、DeleteFlag=True）。
        /// </summary>
        public void CancelPremium(
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey,
            Guid actorAccountKey,
            QhYappliPremiumReadApiResults premiumInfo)
        {
            var apiArgs = new QhYappliPremiumWriteApiArgs(
                QhApiTypeEnum.YappliPremiumWrite,
                QsApiSystemTypeEnum.Qolms,
                apiExecutor,
                apiExecutorName
            )
            {
                ActorKey = actorAccountKey.ToApiGuidString(),
                MemberManageNo = premiumInfo.MemberManageNo,
                MemberShipType = premiumInfo.MemberShipType,
                PaymentType = premiumInfo.PaymentType,
                CustomerId = premiumInfo.CustomerId,
                SubscriptionId = premiumInfo.SubscriptionId,
                ContinueAccountId = premiumInfo.ContinueAccountId,
                ContinueAccountStartDate = premiumInfo.ContinueAccountStartDate,
                StartDate = premiumInfo.StartDate,
                EndDate = DateTime.Now.ToApiDateString(),
                StatusNo = "10",
                AuErrorCode = string.Empty,
                DeleteFlag = "True"
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPremiumWriteApiResults>(
                apiArgs,
                sessionId,
                apiAuthorizeKey
            );

            if (!apiResults.IsSuccess.TryToValueType(false))
            {
                throw new InvalidOperationException(
                    string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }
    }
}

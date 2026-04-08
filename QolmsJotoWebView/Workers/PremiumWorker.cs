using System;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// プレミアム会員情報に関する処理を提供します。
    /// </summary>
    internal sealed class PremiumWorker
    {
        private const byte BusinessPaymentType = 255;
        private const int PremiumStatusSuccess = 3;
        private const int PremiumStatusCanceled = 10;
        private const string BusinessFreeFacilityId = "47500107";

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        private PremiumWorker()
        {
        }

        /// <summary>
        /// プレミアム会員情報取得APIを実行します。
        /// </summary>
        private static QhYappliPremiumReadApiResults ExecutePremiumReadApi(QolmsJotoModel mainModel, bool containDeleted)
        {
            var apiArgs = new QhYappliPremiumReadApiArgs(
                QhApiTypeEnum.YappliPremiumRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                ContainsBeingProcessed = containDeleted.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPremiumReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }

            throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
        }

        /// <summary>
        /// プレミアム会員情報更新APIを実行します。
        /// </summary>
        private static QhYappliPremiumWriteApiResults ExecutePremiumWriteApi(
            QolmsJotoModel mainModel,
            long memberManageNo,
            byte memberShipType,
            byte paymentType,
            string customerId,
            string subscriptionId,
            DateTime continueAccountStartDate,
            DateTime startDate,
            DateTime endDate,
            int statusNo,
            string additionalSet,
            bool deleteFlag)
        {
            var apiArgs = new QhYappliPremiumWriteApiArgs(
                QhApiTypeEnum.YappliPremiumWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                MemberManageNo = memberManageNo.ToString(),
                MemberShipType = memberShipType.ToString(),
                PaymentType = paymentType.ToString(),
                CustomerId = customerId ?? string.Empty,
                SubscriptionId = subscriptionId ?? string.Empty,
                ContinueAccountStartDate = continueAccountStartDate.ToApiDateString(),
                StartDate = startDate.ToApiDateString(),
                EndDate = endDate.ToApiDateString(),
                StatusNo = statusNo.ToString(),
                AdditionalSet = additionalSet ?? string.Empty,
                DeleteFlag = deleteFlag.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPremiumWriteApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }

            throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
        }

        private static DateTime ResolveContinueAccountStartDate(QhYappliPremiumReadApiResults apiResult)
        {
            var continueAccountStartDate = apiResult.ContinueAccountStartDate.TryToValueType(DateTime.MinValue);
            if (continueAccountStartDate == DateTime.MinValue)
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
            }

            return continueAccountStartDate;
        }

        private static byte ResolveBusinessMemberShipType(string facilityId)
        {
            return facilityId == BusinessFreeFacilityId
                ? (byte)QjMemberShipTypeEnum.BusinessFree
                : (byte)QjMemberShipTypeEnum.Business;
        }

        internal static bool UpdatePremiumToBusiness(QolmsJotoModel mainModel, string facilityId)
        {
            var apiResult = ExecutePremiumReadApi(mainModel, false);
            var memberManageNo = apiResult.MemberManageNo.TryToValueType(long.MinValue);

            var writeResult = ExecutePremiumWriteApi(
                mainModel,
                memberManageNo,
                ResolveBusinessMemberShipType(facilityId),
                BusinessPaymentType,
                apiResult.CustomerId,
                apiResult.SubscriptionId,
                DateTime.Now,
                DateTime.Now,
                DateTime.MaxValue,
                PremiumStatusSuccess,
                apiResult.AdditionalSet,
                false);

            return writeResult.IsSuccess.TryToValueType(false);
        }

        internal static bool CancelBusinessPremium(QolmsJotoModel mainModel)
        {
            var apiResult = ExecutePremiumReadApi(mainModel, false);
            var memberManageNo = apiResult.MemberManageNo.TryToValueType(long.MinValue);
            if (memberManageNo <= 0)
            {
                return true;
            }

            var startDate = apiResult.StartDate.TryToValueType(DateTime.MinValue);
            if (startDate == DateTime.MinValue)
            {
                startDate = DateTime.Now;
            }

            var writeResult = ExecutePremiumWriteApi(
                mainModel,
                memberManageNo,
                apiResult.MemberShipType.TryToValueType((byte)QjMemberShipTypeEnum.Free),
                apiResult.PaymentType.TryToValueType(BusinessPaymentType),
                apiResult.CustomerId,
                apiResult.SubscriptionId,
                ResolveContinueAccountStartDate(apiResult),
                startDate,
                DateTime.Now,
                PremiumStatusCanceled,
                apiResult.AdditionalSet,
                true);

            return writeResult.IsSuccess.TryToValueType(false);
        }

        /// <summary>
        /// 会員種別を取得します。
        /// </summary>
        internal static byte GetMemberShipType(QolmsJotoModel mainModel)
        {
            var apiResult = ExecutePremiumReadApi(mainModel, false);
            if (apiResult.IsSuccess.TryToValueType(false))
            {
                return apiResult.MemberShipType.TryToValueType(byte.MinValue);
            }

            return byte.MinValue;
        }
    }
}

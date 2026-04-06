using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 企業連携の表示/解除処理を担当するワーカーです。
    /// </summary>
    internal sealed class IntegrationCompanyConnectionWorker
    {
        /// <summary>
        /// 企業連携詳細を取得するAPIを実行します。
        /// </summary>
        private static QhYappliPortalCompanyConnectionReadApiResults ExecuteCompanyConnectionReadApi(QolmsJotoModel mainModel, int linkageSystemNo)
        {
            var apiArgs = new QhYappliPortalCompanyConnectionReadApiArgs(
                QhApiTypeEnum.YappliPortalCompanyConnectionRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCompanyConnectionReadApiResults>(
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
        /// 企業連携解除APIを実行します。
        /// </summary>
        private static QhYappliPortalCompanyConnectionWriteApiResults ExecuteCompanyConnectionWriteApi(QolmsJotoModel mainModel, int linkageSystemNo)
        {
            var apiArgs = new QhYappliPortalCompanyConnectionWriteApiArgs(
                QhApiTypeEnum.YappliPortalCompanyConnectionWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCompanyConnectionWriteApiResults>(
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
        /// API結果を企業連携画面用のビューモデルに変換します。
        /// </summary>
        public IntegrationCompanyConnectionViewModel CreateViewModel(QolmsJotoModel mainModel, int linkageSystemNo, QjPageNoTypeEnum fromPageNoType)
        {
            var apiResult = ExecuteCompanyConnectionReadApi(mainModel, linkageSystemNo);

            if (!apiResult.IsSuccess.TryToValueType(false) || apiResult.LinkageSystemNo.TryToValueType(int.MinValue) <= 0)
            {
                return null;
            }

            var showType = apiResult.ShowType.TryToValueType(QjRelationContentTypeEnum.None);

            return new IntegrationCompanyConnectionViewModel
            {
                FromPageNoType = fromPageNoType,
                LinkageSystemNo = apiResult.LinkageSystemNo.TryToValueType(int.MinValue),
                LinkageSystemName = apiResult.LinkageSystemName,
                CompanyConnectedFlag = apiResult.StatusType.TryToValueType(byte.MinValue) == 2,
                ShareBasicInfo = showType.HasFlag(QjRelationContentTypeEnum.Information),
                ShareContactNotebook = showType.HasFlag(QjRelationContentTypeEnum.Contact),
                ShareVitalNotebook = showType.HasFlag(QjRelationContentTypeEnum.Vital),
                ShareMedicineNotebook = showType.HasFlag(QjRelationContentTypeEnum.Medicine),
                ShareExaminationNotebook = showType.HasFlag(QjRelationContentTypeEnum.Examination)
            };
        }

        /// <summary>
        /// 企業連携を解除します。
        /// </summary>
        public bool Delete(QolmsJotoModel mainModel, int linkageSystemNo, ref string message)
        {
            var apiResult = ExecuteCompanyConnectionWriteApi(mainModel, linkageSystemNo);
            if (apiResult.IsSuccess.TryToValueType(false) && PremiumWorker.CancelBusinessPremium(mainModel))
            {
                mainModel.AuthorAccount.MembershipType = QjMemberShipTypeEnum.Free;
                return true;
            }

            message = "企業連携の解除または会員情報の更新に失敗しました。";
            return false;
        }
    }
}
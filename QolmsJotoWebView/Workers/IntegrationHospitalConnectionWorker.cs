using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 病院連携の参照・解除処理を担当するワーカーです。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class IntegrationHospitalConnectionWorker
    {
        /// <summary>
        /// 病院連携詳細を取得するAPIを実行します。
        /// </summary>
        private static QhYappliPortalHospitalConnectionReadApiResults ExecuteHospitalConnectionReadApi(QolmsJotoModel mainModel, int linkageSystemNo)
        {
            var apiArgs = new QhYappliPortalHospitalConnectionReadApiArgs(
                QhApiTypeEnum.YappliPortalHospitalConnectionRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalHospitalConnectionReadApiResults>(
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
        /// 病院連携解除APIを実行します。
        /// </summary>
        private static QhYappliPortalHospitalConnectionWriteApiResults ExecuteHospitalConnectionWriteApi(QolmsJotoModel mainModel, int linkageSystemNo)
        {
            var apiArgs = new QhYappliPortalHospitalConnectionWriteApiArgs(
                QhApiTypeEnum.YappliPortalHospitalConnectionWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalHospitalConnectionWriteApiResults>(
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
        /// API結果を病院連携画面用のビューモデルに変換します。
        /// 連携情報が存在しない場合は <see langword="null"/> を返します。
        /// </summary>
        public IntegrationHospitalConnectionViewModel CreateViewModel(QolmsJotoModel mainModel, int linkageSystemNo, QjPageNoTypeEnum fromPageNoType)
        {
            var apiResult = ExecuteHospitalConnectionReadApi(mainModel, linkageSystemNo);

            if (!apiResult.IsSuccess.TryToValueType(false) || apiResult.LinkageSystemNo.TryToValueType(int.MinValue) <= 0)
            {
                return null;
            }

            var showType = apiResult.ShowType.TryToValueType(QjRelationContentTypeEnum.None);

            return new IntegrationHospitalConnectionViewModel
            {
                FromPageNoType = fromPageNoType,
                LinkageSystemNo = apiResult.LinkageSystemNo.TryToValueType(int.MinValue),
                HospitalName = apiResult.LinkageSystemName,
                PatientNo = apiResult.LinkageSystemId,
                StatusType = apiResult.StatusType.TryToValueType(QjLinkageStatusTypeEnum.None),
                DisapprovedReason = apiResult.DisapprovedReason,
                ShowType = showType,
                HospitalConnectedFlag = apiResult.StatusType.TryToValueType(QjLinkageStatusTypeEnum.None) == QjLinkageStatusTypeEnum.Approved,
                ExaminationConnectedFlag = true
            };
        }

        /// <summary>
        /// 病院連携を解除します。
        /// </summary>
        public bool Delete(QolmsJotoModel mainModel, int linkageSystemNo, ref string message)
        {
            var apiResult = ExecuteHospitalConnectionWriteApi(mainModel, linkageSystemNo);
            if (apiResult.IsSuccess.TryToValueType(false))
            {
                return true;
            }

            message = "病院連携の解除に失敗しました。";
            return false;
        }
    }
}

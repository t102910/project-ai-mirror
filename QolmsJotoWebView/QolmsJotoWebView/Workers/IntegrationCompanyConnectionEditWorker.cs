using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 企業連携編集の読込/更新処理を担当するワーカーです。
    /// </summary>
    internal sealed class IntegrationCompanyConnectionEditWorker
    {
        private const string ErrorNotEmployeeExists = "従業員が存在しません。";
        private const string ErrorLinkageRegisterFaild = "企業連携の登録に失敗しました。";
        private const string ErrorUnknown = "不明なエラーです。";

        /// <summary>
        /// 企業連携編集画面の初期値取得APIを実行します。
        /// </summary>
        private static QhYappliPortalCompanyConnectionEditReadApiResults ExecuteCompanyConnectionEditReadApi(QolmsJotoModel mainModel, int linkageSystemNo)
        {
            var apiArgs = new QhYappliPortalCompanyConnectionEditReadApiArgs(
                QhApiTypeEnum.YappliPortalCompanyConnectionEditRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCompanyConnectionEditReadApiResults>(
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
        /// 企業連携編集の更新APIを実行します。
        /// </summary>
        private static QhYappliPortalCompanyConnectionEditWriteApiResults ExecuteCompanyConnectionEditWriteApi(
            QolmsJotoModel mainModel,
            IntegrationCompanyConnectionEditInputModel model)
        {
            var apiArgs = new QhYappliPortalCompanyConnectionEditWriteApiArgs(
                QhApiTypeEnum.YappliPortalCompanyConnectionEditWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = model.LinkageSystemNo.ToString(),
                RelationContentType = Convert.ToInt64(model.RelationContentFlags).ToString(),
                MailAddress = model.MailAddress
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCompanyConnectionEditWriteApiResults>(
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
        /// API結果を企業連携編集画面のビューモデルへ反映します。
        /// </summary>
        public IntegrationCompanyConnectionEditViewModel CreateViewModel(QolmsJotoModel mainModel, int linkageSystemNo, QjPageNoTypeEnum fromPageNoType)
        {
            var result = new IntegrationCompanyConnectionEditViewModel();
            var hiddenMailAddress = (ConfigurationManager.AppSettings["CompanyConnectionEditHiddenMailaddressCsv"] ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            var apiResult = ExecuteCompanyConnectionEditReadApi(mainModel, linkageSystemNo);
            if (apiResult.IsSuccess.TryToValueType(false) && apiResult.LinkageSystemNo.TryToValueType(int.MinValue) > 0)
            {
                var showType = apiResult.ShowType.TryToValueType(QjRelationContentTypeEnum.None);

                result.FromPageNoType = fromPageNoType;
                result.LinkageSystemNo = apiResult.LinkageSystemNo.TryToValueType(int.MinValue);
                result.LinkageSystemName = apiResult.LinkageSystemName;
                result.MailAddress = hiddenMailAddress.Contains(apiResult.MailAddress) ? string.Empty : apiResult.MailAddress;
                result.ShareBasicInfo = showType.HasFlag(QjRelationContentTypeEnum.Information);
                result.ShareContactNotebook = showType.HasFlag(QjRelationContentTypeEnum.Contact);
                result.ShareVitalNotebook = showType.HasFlag(QjRelationContentTypeEnum.Vital);
                result.ShareMedicineNotebook = showType.HasFlag(QjRelationContentTypeEnum.Medicine);
                result.ShareExaminationNotebook = showType.HasFlag(QjRelationContentTypeEnum.Examination);
            }

            return result;
        }

        /// <summary>
        /// 企業連携編集を実行し、結果に応じてメッセージを返します。
        /// </summary>
        public bool Edit(
            QolmsJotoModel mainModel,
            IntegrationCompanyConnectionEditInputModel model,
            ref int linkageSystemNo,
            ref string message)
        {
            var apiResult = ExecuteCompanyConnectionEditWriteApi(mainModel, model);

            if (apiResult.IsSuccess.TryToValueType(false)
                && apiResult.ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None) == QsApiCompanyConnectionRequestErrorTypeEnum.None
                && apiResult.LinkageSystemNo.TryToValueType(int.MinValue) > 0)
            {
                linkageSystemNo = apiResult.LinkageSystemNo.TryToValueType(int.MinValue);
                return true;
            }

            switch (apiResult.ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None))
            {
                case QsApiCompanyConnectionRequestErrorTypeEnum.NotEmployeeExists:
                    message = ErrorNotEmployeeExists;
                    break;
                case QsApiCompanyConnectionRequestErrorTypeEnum.LinkageRegisterFaild:
                    message = ErrorLinkageRegisterFaild;
                    break;
                default:
                    message = ErrorUnknown;
                    break;
            }

            return false;
        }
    }
}
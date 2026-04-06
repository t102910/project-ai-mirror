using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 企業連携申請の初期化と登録処理を担当するワーカーです。
    /// </summary>
    internal sealed class IntegrationCompanyConnectionRequestWorker
    {
        private const string ErrorNotEmployeeExists = "従業員が存在しません。";
        private const string ErrorNoIdentification = "従業員情報が一致しません。";
        private const string ErrorLinkageRegisterFaild = "企業連携の登録に失敗しました。";
        private const string ErrorRelationRegisterFaild = "従業員連携の登録に失敗しました。";
        private const string ErrorEmployeeNameIdentificationRegisterFaild = "従業員情報の変換に失敗しました。";
        private const string ErrorPremiumSyncFaild = "会員情報の更新に失敗しました。";
        private const string ErrorUnknown = "不明なエラーです。";

        /// <summary>
        /// 企業連携申請APIを実行します。
        /// </summary>
        private static QhYappliPortalCompanyConnectionRequestWriteApiResults ExecuteCompanyConnectionRequestWriteApi(
            QolmsJotoModel mainModel,
            IntegrationCompanyConnectionRequestInputModel model)
        {
            var birthday = new DateTime(int.Parse(model.BirthYear), int.Parse(model.BirthMonth), int.Parse(model.BirthDay));

            var apiArgs = new QhYappliPortalCompanyConnectionRequestWriteApiArgs(
                QhApiTypeEnum.YappliPortalCompanyConnectionRequestWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                FacilityId = model.FacilityId,
                LinkageSystemId = model.EmployeeNo,
                FamilyName = model.FamilyName,
                GivenName = model.GivenName,
                FamilyKanaName = model.FamilyKanaName,
                GivenKanaName = model.GivenKanaName,
                SexType = model.SexType.ToString(),
                Birtyday = birthday.ToApiDateString(),
                RelationContentType = model.RelationContentFlags.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCompanyConnectionRequestWriteApiResults>(
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
        /// ログイン中アカウント情報から申請画面の初期値を作成します。
        /// </summary>
        public IntegrationCompanyConnectionRequestViewModel CreateViewModel(QolmsJotoModel mainModel, QjPageNoTypeEnum fromPageNoType)
        {
            return new IntegrationCompanyConnectionRequestViewModel
            {
                FromPageNoType = fromPageNoType,
                CompanyCode = string.Empty,
                EmployeeNo = string.Empty,
                FamilyName = mainModel.AuthorAccount.FamilyName,
                GivenName = mainModel.AuthorAccount.GivenName,
                FamilyKanaName = mainModel.AuthorAccount.FamilyKanaName,
                GivenKanaName = mainModel.AuthorAccount.GivenKanaName,
                SexType = mainModel.AuthorAccount.SexType,
                BirthYear = mainModel.AuthorAccount.Birthday.Year.ToString(),
                BirthMonth = mainModel.AuthorAccount.Birthday.Month.ToString(),
                BirthDay = mainModel.AuthorAccount.Birthday.Day.ToString(),
                IsPremiumMember = mainModel.AuthorAccount.MembershipType == QjMemberShipTypeEnum.LimitedTime
                    || mainModel.AuthorAccount.MembershipType == QjMemberShipTypeEnum.Premium,
                ShareBasicInfo = true,
                ShareContactNotebook = true,
                ShareVitalNotebook = true,
                ShareMedicineNotebook = true,
                ShareExaminationNotebook = true
            };
        }

        /// <summary>
        /// 企業連携申請を実行し、結果に応じてメッセージを返します。
        /// </summary>
        public bool Request(
            QolmsJotoModel mainModel,
            IntegrationCompanyConnectionRequestInputModel model,
            ref int linkageSystemNo,
            ref string message)
        {
            var apiResult = ExecuteCompanyConnectionRequestWriteApi(mainModel, model);

            if (apiResult.IsSuccess.TryToValueType(false)
                && apiResult.ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None) == QsApiCompanyConnectionRequestErrorTypeEnum.None
                && apiResult.LinkageSystemNo.TryToValueType(int.MinValue) > 0)
            {
                if (!PremiumWorker.UpdatePremiumToBusiness(mainModel, model.FacilityId))
                {
                    message = ErrorPremiumSyncFaild;
                    return false;
                }

                linkageSystemNo = apiResult.LinkageSystemNo.TryToValueType(int.MinValue);
                mainModel.AuthorAccount.MembershipType = model.FacilityId == "47500107"
                    ? QjMemberShipTypeEnum.BusinessFree
                    : QjMemberShipTypeEnum.Business;

                mainModel.AuthorAccount.FamilyName = model.FamilyName;
                mainModel.AuthorAccount.GivenName = model.GivenName;
                mainModel.AuthorAccount.FamilyKanaName = model.FamilyKanaName;
                mainModel.AuthorAccount.GivenKanaName = model.GivenKanaName;

                return true;
            }

            switch (apiResult.ErrorCode.TryToValueType(QsApiCompanyConnectionRequestErrorTypeEnum.None))
            {
                case QsApiCompanyConnectionRequestErrorTypeEnum.NotEmployeeExists:
                    message = ErrorNotEmployeeExists;
                    break;
                case QsApiCompanyConnectionRequestErrorTypeEnum.NoIdentification:
                    message = ErrorNoIdentification;
                    break;
                case QsApiCompanyConnectionRequestErrorTypeEnum.LinkageRegisterFaild:
                    message = ErrorLinkageRegisterFaild;
                    break;
                case QsApiCompanyConnectionRequestErrorTypeEnum.RelationRegisterFaild:
                    message = ErrorRelationRegisterFaild;
                    break;
                case QsApiCompanyConnectionRequestErrorTypeEnum.EmployeeNameIdentificationRegisterFaild:
                    message = ErrorEmployeeNameIdentificationRegisterFaild;
                    break;
                default:
                    message = ErrorUnknown;
                    break;
            }

            return false;
        }
    }
}
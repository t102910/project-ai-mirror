using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 病院連携申請の初期化と登録処理を担当するワーカーです。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class IntegrationHospitalConnectionRequestWorker
    {
        // Yappli 運用の既定病院コード。設定が読めない場合の最終フォールバックに使う。
        private static readonly int[] DefaultHospitalEnableLinkageList = { 47106, 47000016, 47000020, 47500110 };

        // ConnectionSettingRead 側で病院候補を取得するための対象コード一覧。
        private static readonly int[] LinkageList = { 47005, 47006, 47010, 47011, 47106, 47000016, 47000020, 47500110 };
        private static readonly int[] MedicineLinkageList = { 47009 };
        private static readonly int[] CompanyLinkageList = { 47100, 11111 };

        /// <summary>
        /// 有効な病院連携番号のリストを設定ファイルから取得します。
        /// 未設定の場合は既定の病院コードを返します。
        /// </summary>
        private static List<int> GetEnabledHospitalLinkageSystemNoList()
        {
            var result = new List<int>();
            // DevApp.config の HospitalEnableLinkageList を読む。未設定時のみ既定値へフォールバックする。
            string value = QjConfiguration.HospitalEnableLinkageList;

            if (string.IsNullOrWhiteSpace(value))
            {
                result.AddRange(DefaultHospitalEnableLinkageList);
                return result;
            }

            foreach (string item in value.Split(','))
            {
                if (int.TryParse(item, out int linkageSystemNo))
                {
                    result.Add(linkageSystemNo);
                }
            }

            if (result.Count == 0)
            {
                result.AddRange(DefaultHospitalEnableLinkageList);
            }

            return result;
        }

        /// <summary>
        /// 連携設定読み取りAPIを実行します。
        /// 病院一覧の主取得が空の場合のフォールバック用です。
        /// </summary>
        private static QhYappliPortalConnectionSettingReadApiResults ExecuteConnectionSettingReadApi(QolmsJotoModel mainModel)
        {
            // RequestRead で病院一覧が取れない場合のフォールバック元。
            var apiArgs = new QhYappliPortalConnectionSettingReadApiArgs(
                QhApiTypeEnum.YappliPortalConnectionSettingRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = new List<string>(Array.ConvertAll(LinkageList, i => i.ToString())),
                MedicineLinkageSystemNo = new List<string>(Array.ConvertAll(MedicineLinkageList, i => i.ToString())),
                CompanyLinkageSystemNo = new List<string>(Array.ConvertAll(CompanyLinkageList, i => i.ToString()))
            };

            return QsApiManager.ExecuteQolmsApi<QhYappliPortalConnectionSettingReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);
        }

        /// <summary>
        /// 病院連携申請読み取りAPIを実行します。
        /// 新規申請導線では linkageSystemNo に 0 を指定します。
        /// </summary>
        private static QhYappliPortalHospitalConnectionRequestReadApiResults ExecuteHospitalConnectionRequestReadApi(
            QolmsJotoModel mainModel,
            int linkageSystemNo)
        {
            // 病院選択肢の主取得元。新規導線では linkageSystemNo=0 で API を呼ぶ。
            var apiArgs = new QhYappliPortalHospitalConnectionRequestReadApiArgs(
                QhApiTypeEnum.YappliPortalHospitalConnectionRequestRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalHospitalConnectionRequestReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);
            return apiResults;
        }

        /// <summary>
        /// 病院連携申請書き込みAPIを実行します。
        /// </summary>
        private static QhYappliPortalHospitalConnectionRequestWriteApiResults ExecuteHospitalConnectionRequestWriteApi(
            QolmsJotoModel mainModel,
            IntegrationHospitalConnectionRequestInputModel model)
        {
            var birthday = new DateTime(int.Parse(model.BirthYear), int.Parse(model.BirthMonth), int.Parse(model.BirthDay));

            var apiArgs = new QhYappliPortalHospitalConnectionRequestWriteApiArgs(
                QhApiTypeEnum.YappliPortalHospitalConnectionRequestWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = model.LinkageSystemNo.ToString(),
                LinkageSystemId = model.LinkageSystemId,
                FamilyName = model.FamilyName,
                GivenName = model.GivenName,
                FamilyKanaName = model.FamilyKanaName,
                GivenKanaName = model.GivenKanaName,
                SexType = model.SexType.ToString(),
                BirthDay = birthday.ToApiDateString(),
                MailAddress = model.MailAddress,
                IdentityUpdateFlag = model.IdentityUpdateFlag.ToString(),
                RelationContentType = model.RelationContentFlags.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalHospitalConnectionRequestWriteApiResults>(
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
        /// ログイン中アカウント情報とAPIから病院連携申請画面用のビューモデルを作成します。
        /// </summary>
        public IntegrationHospitalConnectionRequestViewModel CreateViewModel(
            QolmsJotoModel mainModel,
            int linkageSystemNo,
            QjPageNoTypeEnum fromPageNoType)
        {
            var account = mainModel.AuthorAccount;
            bool isHospitalSelectionFixed = linkageSystemNo > 0;
            var result = new IntegrationHospitalConnectionRequestViewModel
            {
                FromPageNoType = fromPageNoType,
                LinkageSystemNo = linkageSystemNo,
                IsHospitalSelectionFixed = isHospitalSelectionFixed,
                HospitalName = string.Empty,
                PatientNo = string.Empty,
                FamilyName = account.FamilyName,
                GivenName = account.GivenName,
                FamilyKanaName = account.FamilyKanaName,
                GivenKanaName = account.GivenKanaName,
                SexType = account.SexType,
                BirthYear = account.Birthday.Year.ToString(),
                BirthMonth = account.Birthday.Month.ToString(),
                BirthDay = account.Birthday.Day.ToString(),
                BirthDateLabel = string.Format("{0}年 {1}月 {2}日", account.Birthday.Year, account.Birthday.Month, account.Birthday.Day),
                MailAddress = string.Empty,
                IdentityUpdateFlag = false,
                RelationContentFlags = QjRelationContentTypeEnum.None
            };

            int readTargetLinkageSystemNo = linkageSystemNo > 0 ? linkageSystemNo : 0;
            var apiResult = ExecuteHospitalConnectionRequestReadApi(mainModel, readTargetLinkageSystemNo);

            // RequestRead の HospitalList を設定値の病院コードで絞り込んで画面候補を作る。
            List<int> enabledLinkageSystemNoList = GetEnabledHospitalLinkageSystemNoList();
            foreach (var item in apiResult.HospitalList ?? new List<QhApiLinkageSystemMasterItem>())
            {
                int itemLinkageSystemNo = item.LinkageSystemNo.TryToValueType(int.MinValue);
                if (itemLinkageSystemNo > 0
                    && (enabledLinkageSystemNoList.Contains(itemLinkageSystemNo)
                        || itemLinkageSystemNo == linkageSystemNo))
                {
                    result.HospitalList.Add(new KeyValuePair<int, string>(itemLinkageSystemNo, item.LinkageName ?? string.Empty));
                }
            }

            if (apiResult.IsSuccess.TryToValueType(false))
            {
                result.PatientNo = apiResult.LinkageSystemId;
                result.MailAddress = apiResult.MailAddress;
                result.RelationContentFlags = (QjRelationContentTypeEnum)Enum.ToObject(
                    typeof(QjRelationContentTypeEnum),
                    apiResult.RelationContentType.TryToValueType(long.MinValue));

                foreach (var item in apiResult.HospitalList ?? new List<QhApiLinkageSystemMasterItem>())
                {
                    if (item.LinkageSystemNo.TryToValueType(int.MinValue) == linkageSystemNo)
                    {
                        result.HospitalName = item.LinkageName;
                        break;
                    }
                }
            }

            if (result.LinkageSystemNo <= 0 && result.HospitalList.Count > 0)
            {
                result.LinkageSystemNo = result.HospitalList[0].Key;
                result.HospitalName = result.HospitalList[0].Value;
            }

            if (result.HospitalList.Count == 0)
            {
                // 主取得元が空の場合は ConnectionSettingRead の病院一覧で再構築する。
                var connectionSettingResult = ExecuteConnectionSettingReadApi(mainModel);
                if (connectionSettingResult != null && connectionSettingResult.IsSuccess.TryToValueType(false))
                {
                    foreach (var item in connectionSettingResult.ConnectionSettingHospitalItemN ?? new List<QhApiLinkageItem>())
                    {
                        int itemLinkageSystemNo = item.LinkageSystemNo.TryToValueType(int.MinValue);
                        if (itemLinkageSystemNo > 0
                            && (enabledLinkageSystemNoList.Contains(itemLinkageSystemNo)
                                || itemLinkageSystemNo == linkageSystemNo))
                        {
                            result.HospitalList.Add(new KeyValuePair<int, string>(itemLinkageSystemNo, item.LinkageSystemName ?? string.Empty));
                        }
                    }

                    if (result.LinkageSystemNo <= 0 && result.HospitalList.Count > 0)
                    {
                        result.LinkageSystemNo = result.HospitalList[0].Key;
                        result.HospitalName = result.HospitalList[0].Value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 病院連携申請を実行します。
        /// 成功時にセッション上のアカウント氏名情報を更新します。
        /// </summary>
        public bool Request(
            QolmsJotoModel mainModel,
            IntegrationHospitalConnectionRequestInputModel model,
            ref string message)
        {
            var apiResult = ExecuteHospitalConnectionRequestWriteApi(mainModel, model);
            if (apiResult.IsSuccess.TryToValueType(false))
            {
                mainModel.AuthorAccount.FamilyName = model.FamilyName;
                mainModel.AuthorAccount.GivenName = model.GivenName;
                mainModel.AuthorAccount.FamilyKanaName = model.FamilyKanaName;
                mainModel.AuthorAccount.GivenKanaName = model.GivenKanaName;
                return true;
            }

            message = "病院連携の登録に失敗しました。";
            return false;
        }
    }
}

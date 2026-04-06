using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    internal sealed class PortalConnectionSettiongWorker
    {
        private static readonly int[] LinkageList = { 47005, 47006, 47010, 47011 };
        private static readonly int[] MedicineLinkageList = { 47009 };
        private static readonly int[] CompanyLinkageList = { 47100, 11111 };

        private static QhYappliPortalConnectionSettingReadApiResults ExecuteConnectionSettingReadApi(QolmsJotoModel mainModel)
        {
            var apiArgs = new QhYappliPortalConnectionSettingReadApiArgs(
                QhApiTypeEnum.YappliPortalConnectionSettingRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = LinkageList.Select(i => i.ToString()).ToList(),
                MedicineLinkageSystemNo = MedicineLinkageList.Select(i => i.ToString()).ToList(),
                CompanyLinkageSystemNo = CompanyLinkageList.Select(i => i.ToString()).ToList()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalConnectionSettingReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }

            throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)));
        }

        public PortalConnectionSettingViewModel CreateViewModel(QolmsJotoModel mainModel, QjPageNoTypeEnum fromPageNoType, byte tabNoType)
        {
            var result = new PortalConnectionSettingViewModel
            {
                FromPageNoType = fromPageNoType,
                TabNoType = tabNoType
            };

            var apiResult = ExecuteConnectionSettingReadApi(mainModel);

            foreach (var item in apiResult.LinkageItems ?? new List<QhApiLinkageItem>())
            {
                if (int.TryParse(item.LinkageSystemNo, out var linkageSystemNo) && !result.ConnectionSettingItems.ContainsKey(linkageSystemNo))
                {
                    byte.TryParse(item.StatusType, out var status);

                    result.ConnectionSettingItems.Add(
                        linkageSystemNo,
                        new ConnectionSettingItem
                        {
                            LinkageNo = linkageSystemNo,
                            Devices = item.Devices ?? new List<string>(),
                            Tags = item.Tags ?? new List<string>(),
                            Status = status
                        }
                    );
                }
            }

            foreach (var item in apiResult.ConnectionSettingHospitalItemN ?? new List<QhApiLinkageItem>())
            {
                if (int.TryParse(item.LinkageSystemNo, out var linkageSystemNo) && !result.ConnectionSettingHospitalItemN.ContainsKey(linkageSystemNo))
                {
                    byte.TryParse(item.StatusType, out var status);

                    result.ConnectionSettingHospitalItemN.Add(
                        linkageSystemNo,
                        new ConnectionSettingHospitalItem
                        {
                            LinkageSystemNo = linkageSystemNo,
                            LinkageSystemName = item.LinkageSystemName ?? string.Empty,
                            Status = status
                        }
                    );
                }
            }

            foreach (var item in apiResult.ConnectionSettingMedicineItemN ?? new List<QhApiLinkageItem>())
            {
                if (Guid.TryParse(item.Facilitykey, out var facilityKey) && !result.ConnectionSettingPharmacyItemN.ContainsKey(facilityKey))
                {
                    int.TryParse(item.LinkageSystemNo, out var linkageSystemNo);
                    byte.TryParse(item.StatusType, out var status);

                    result.ConnectionSettingPharmacyItemN.Add(
                        facilityKey,
                        new ConnectionSettingPharmacyItem
                        {
                            FacilityKey = facilityKey,
                            LinkageSystemNo = linkageSystemNo,
                            LinkageSystemName = item.LinkageSystemName ?? string.Empty,
                            Status = status
                        }
                    );
                }
            }

            foreach (var item in apiResult.ConnectionSettingCompanyItemN ?? new List<QhApiLinkageItem>())
            {
                if (int.TryParse(item.LinkageSystemNo, out var linkageSystemNo) && !result.ConnectionSettingCompanyItemN.ContainsKey(linkageSystemNo))
                {
                    byte.TryParse(item.StatusType, out var status);

                    result.ConnectionSettingCompanyItemN.Add(
                        linkageSystemNo,
                        new ConnectionSettingCompanyItem
                        {
                            LinkageSystemNo = linkageSystemNo,
                            LinkageSystemName = item.LinkageSystemName ?? string.Empty,
                            Status = status
                        }
                    );
                }
            }

            return result;
        }
    }
}
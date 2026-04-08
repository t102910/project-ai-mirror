using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    interface ITanitaConnectionRepository
    {
        QhYappliPortalTanitaConnectionReadApiResults ExecuteTanitaConnectionReadApi(QolmsJotoModel mainModel, List<string> SystemNoList);
        QhYappliPortalTanitaConnectionWriteApiResults ExecuteTanitaConnectionWriteApi(QolmsJotoModel mainModel, IntegrationTanitaConnectionViewModel inputModel, string linkageId, List<byte> devices, List<string> tags, byte StatusType, bool deleteFlag);
        QhYappliPortalTanitaConnectionDeviceWriteApiResults ExecuteTanitaConnectionUpdateWriteApi(QolmsJotoModel mainModel, byte device, List<string> tags, bool @checked);
    }

    public class TanitaConnectionRepository: ITanitaConnectionRepository
    {
        public QhYappliPortalTanitaConnectionReadApiResults ExecuteTanitaConnectionReadApi(
            QolmsJotoModel mainModel, List<string> SystemNoList)
        {
            var apiArgs = new QhYappliPortalTanitaConnectionReadApiArgs(
                QhApiTypeEnum.YappliPortalTanitaConnectionRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = SystemNoList
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalTanitaConnectionReadApiResults>(
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
                    QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

        public QhYappliPortalTanitaConnectionWriteApiResults ExecuteTanitaConnectionWriteApi(
            QolmsJotoModel mainModel, IntegrationTanitaConnectionViewModel inputModel, string linkageId,
            List<byte> devices, List<string> tags, byte StatusType, bool deleteFlag)
        {
            var apiArgs = new QhYappliPortalTanitaConnectionWriteApiArgs(
                QhApiTypeEnum.YappliPortalTanitaConnectionWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString(),
                LinkageSystemId = linkageId,
                Tags = tags,
                Devices = devices.ConvertAll(i => i.ToString()),
                StatusType = StatusType.ToString(),
                DeleteFlag = deleteFlag.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalTanitaConnectionWriteApiResults>(
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
                    QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }

        public QhYappliPortalTanitaConnectionDeviceWriteApiResults ExecuteTanitaConnectionUpdateWriteApi(
            QolmsJotoModel mainModel, byte device, List<string> tags, bool @checked)
        {
            var apiArgs = new QhYappliPortalTanitaConnectionDeviceWriteApiArgs(
                QhApiTypeEnum.YappliPortalTanitaConnectionDeviceWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString(),
                Device = device.ToString(),
                Tags = tags,
                Checked = @checked.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalTanitaConnectionDeviceWriteApiResults>(
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
                    QsApiManager.GetQolmsApiName(apiArgs)));
            }
        }
    }
}
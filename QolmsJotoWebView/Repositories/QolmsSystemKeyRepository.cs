using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IQolmsSystemKeyRepository
    {
        string GetQolmsSystemKey(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey);
    }

    public class QolmsSystemKeyRepository: IQolmsSystemKeyRepository
    {
        public string GetQolmsSystemKey(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey)
        {
            var apiArgs = new QoQolmsSystemKeyGenerateApiArgs(
                QoApiTypeEnum.QolmsSystemKeyGenerate,
                QsApiSystemTypeEnum.JotoNative,
                apiExecutor,
                apiExecutorName
            );

            QoQolmsSystemKeyGenerateApiResults apiResults = QsApiManager.ExecuteQolmsOpenApi<QoQolmsSystemKeyGenerateApiResults>(
                apiArgs,
                string.Empty
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults.QolmsSystemKey;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsOpenApiName(apiArgs)));
            }
        }
    }
}
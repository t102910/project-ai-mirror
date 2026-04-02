using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface ITermsRepository
    {
        QjCommonTermsReadApiResults GetTermsContent(QolmsJotoModel mainModel, int tarmsNo);
    }

    public class TermsRepository: ITermsRepository
    {
        public QjCommonTermsReadApiResults GetTermsContent(QolmsJotoModel mainModel, int termsNo)
        {

            QjCommonTermsReadApiArgs apiArgs = new QjCommonTermsReadApiArgs(
                QjApiTypeEnum.CommonTermsRead,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                TermsNo = termsNo.ToString(),
                SystemType = ((byte)QsApiSystemTypeEnum.QolmsJoto).ToString()
            };

            QjCommonTermsReadApiResults apiResults = QsApiManager.ExecuteQolmsJotoApi<QjCommonTermsReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey2
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs))
                );
            }
        }


    }
}
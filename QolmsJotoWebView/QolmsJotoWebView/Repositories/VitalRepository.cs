using System;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    /// <summary>
    /// バイタル情報入出力インターフェース
    /// </summary>
    public interface IVitalRepository
    {
        bool GetLatestHeightAndWeight(Guid executor, string executorName, string sessionid, Guid apiAuthorizeKey2, Guid accountkey, ref decimal height, ref decimal weight);
    }

    public class VitalRepository : IVitalRepository
    {
        public bool GetLatestHeightAndWeight(Guid executor, string executorName,string sessionid, Guid apiAuthorizeKey2, Guid accountkey, ref decimal height, ref decimal weight)
        {
            var apiArgs = new QjCommonLatestHeightAndWeightReadApiArgs(
                QjApiTypeEnum.CommonLatestHeightAndWeightRead,
                QsApiSystemTypeEnum.QolmsJoto,
                executor,
                executorName)
            {
                Accountkey = accountkey.ToApiGuidString()
            };

            var apiResults = QsApiManager.ExecuteQolmsJotoApi<QjCommonLatestHeightAndWeightReadApiResults>(
                apiArgs,
                sessionid,
                apiAuthorizeKey2);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                height = apiResults.Height.TryToValueType(decimal.MinValue);
                weight = apiResults.Weight.TryToValueType(decimal.MinValue);
                return true;
            }
            else
            {
                throw new InvalidOperationException($"{QsApiManager.GetQolmsJotoApiName(apiArgs)} API の実行に失敗しました。");
            }
        }
    }
}
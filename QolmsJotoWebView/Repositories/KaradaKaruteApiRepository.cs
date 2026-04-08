using MGF.QOLMS.QolmsKaradaKaruteApiCoreV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IKaradaKaruteApiRepository
    {
        MemberAuthApiResults ExecuteKaradaKaruteConnectionApi(string loginId, string loginPassword);

        MemberAuthApiResults ExecuteKaradaKaruteCancel(string memberNo);
    };

    public class KaradaKaruteApiRepository: IKaradaKaruteApiRepository
    {
        public MemberAuthApiResults ExecuteKaradaKaruteConnectionApi(string loginId, string loginPassword)
        {
            var apiArgs = new MemberAuthApiArgs()
            {
                siteId = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSiteId,
                sitePass = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSitePassword,
                loginId = loginId,
                loginPass = loginPassword,
                mode = "0"
            };

            var apiResults = QsKaradaKaruteApiManager.Execute<MemberAuthApiArgs, MemberAuthApiResults>(apiArgs);

            if (apiResults.IsSuccess)
            {
                switch (apiResults.status)
                {
                    case "0":
                        break;
                    case "1":
                        apiResults.message = "IDまたはパスワードが間違っています。";
                        break;
                    case "-1":
                        apiResults.message = "パラメータ 異常";
                        break;
                    case "-2":
                        apiResults.message = "データベース 異常";
                        break;
                    case "-3":
                        apiResults.message = "IDまたはパスワードが間違っています。";
                        break;
                    case "-4":
                        apiResults.message = "その他 エラー";
                        break;
                }

                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", "KaradaKarute"));
            }
        }

        public MemberAuthApiResults ExecuteKaradaKaruteCancel(string memberNo)
        {
            var apiArgs = new MemberAuthApiArgs()
            {
                siteId = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSiteId,
                sitePass = QsKaradaKaruteApiConfiguration.KaradaKaruteApiSitePassword,
                memberNo = memberNo,
                mode = "1"
            };

            var apiResults = QsKaradaKaruteApiManager.Execute<MemberAuthApiArgs, MemberAuthApiResults>(apiArgs);

            if (apiResults.IsSuccess)
            {
                switch (apiResults.status)
                {
                    case "0":
                        break;
                    case "1":
                        apiResults.message = "IDまたはパスワードが間違っています。";
                        break;
                    case "-1":
                        apiResults.message = "パラメータ 異常";
                        break;
                    case "-2":
                        apiResults.message = "データベース 異常";
                        break;
                    case "-3":
                        apiResults.message = "IDまたはパスワードが間違っています。";
                        break;
                    case "-4":
                        apiResults.message = "その他 エラー";
                        break;
                }

                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", "KaradaKarute"));
            }
        }

    }
}
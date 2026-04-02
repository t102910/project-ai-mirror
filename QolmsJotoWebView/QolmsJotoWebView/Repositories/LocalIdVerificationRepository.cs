using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{

    public interface ILocalIdVerificationRepository
    {
        QjPortalLocalIdVerificationRegisterReadApiResults ExecutePortalLocalIdVerificationRegisterReadApi(QolmsJotoModel mainModel);
            
        QjPortalLocalIdVerificationRegisterWriteApiResults ExecutePortalLocalIdVerificationRegisterWriteApi(QolmsJotoModel mainModel, PortalLocalIdVerificationRegisterInputModel inputModel, bool deleteFlag);
    }

    public class LocalIdVerificationRepository: ILocalIdVerificationRepository
    {

        /// <summary>
        /// 「市民確認登録（ぎのわんPJ）」画面取得 API を実行します。
        /// </summary>
        /// <param name="mainModel">ログイン済みモデル。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public QjPortalLocalIdVerificationRegisterReadApiResults ExecutePortalLocalIdVerificationRegisterReadApi(QolmsJotoModel mainModel)
        {
            QjPortalLocalIdVerificationRegisterReadApiArgs apiArgs = new QjPortalLocalIdVerificationRegisterReadApiArgs(
                QjApiTypeEnum.PortalLocalIdVerificationRegisterRead,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO.ToString()
            };

            QjPortalLocalIdVerificationRegisterReadApiResults apiResults = QsApiManager.ExecuteQolmsJotoApi<QjPortalLocalIdVerificationRegisterReadApiResults>(
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
                    string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs))
                );
            }
        }

        /// <summary>
        /// 「市民確認登録（ぎのわんPJ）」画面登録 API を実行します。
        /// </summary>
        /// <param name="mainModel">ログイン済みモデル。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public QjPortalLocalIdVerificationRegisterWriteApiResults ExecutePortalLocalIdVerificationRegisterWriteApi(
            QolmsJotoModel mainModel,
            PortalLocalIdVerificationRegisterInputModel inputModel,
            bool deleteFlag)
        {
            QjPortalLocalIdVerificationRegisterWriteApiArgs apiArgs = new QjPortalLocalIdVerificationRegisterWriteApiArgs(
                QjApiTypeEnum.PortalLocalIdVerificationRegisterWrite,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo =QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO.ToString(),
                MailAddress = inputModel.MailAddress,
                PhoneNumber = inputModel.PhoneNumber,
                IdentityUpdateFlag = bool.TrueString,
                RelationContentType = Convert.ToByte(inputModel.RelationContentFlags).ToString(),
                DeleteFlag = deleteFlag.ToString(),
                GinowanProjectJoin = QjLinkageSystemNo.JOTO_GINOWANENTRY_SYSTEM_NO.ToString()
            };

            QjPortalLocalIdVerificationRegisterWriteApiResults apiResults = QsApiManager.ExecuteQolmsJotoApi<QjPortalLocalIdVerificationRegisterWriteApiResults>(
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
                    string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs))
                );
            }
        }
    }
}
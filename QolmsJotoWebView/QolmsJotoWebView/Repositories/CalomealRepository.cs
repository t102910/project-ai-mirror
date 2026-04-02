using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{

    /// <summary>
    /// カロミル情報入出力インターフェース
    /// </summary>
    public interface ICalomealRepository
    {
        /// <summary>
        /// カロミルのアクセストークンを取得します
        /// </summary>
        /// <param name="mainModel"></param>
        /// <returns></returns>
        QhYappliPortalCalomealConnectionTokenReadApiResults ExecuteCalomealConnectionTokenReadApi(QolmsJotoModel mainModel, int linkgaSystemNo);

        /// <summary>
        /// カロミルアクセストークンと最終同期日時を取得します
        /// </summary>
        /// <param name="mainModel"></param>
        /// <param name="linkgaSystemNo"></param>
        /// <returns></returns>
        QhYappliNoteCalomealMealSyncReadApiResults ExecuteCalomealMealSyncRead(QolmsJotoModel mainModel, int linkgaSystemNo);
        
        /// <summary>
        /// カロミルアクセストークンを登録して、新規登録に必要な基本情報を取得します。
        /// </summary>
        /// <param name="mainModel"></param>
        /// <param name="linkgaSystemId"></param>
        /// <param name="token"></param>
        /// <param name="deleteFlag"></param>
        /// <returns></returns>
        QhYappliPortalCalomealConnectionWriteApiResults ExecuteCalomealConnectionWriteApi(QolmsJotoModel mainModel, int linkgaSystemNo, string linkgaSystemId, CalomealAccessTokenSet token, bool deleteFlag);

        /// <summary>
        /// 食事履歴を登録します
        /// </summary>
        /// <param name="mainModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        QhYappliNoteCalomealMealSyncWriteApiResults ExecuteCalomealMealSyncWrite(QolmsJotoModel mainModel, List<NoteMeal2InputModel> model);

    }

    public class CalomealRepository : ICalomealRepository
    {
        public QhYappliPortalCalomealConnectionWriteApiResults ExecuteCalomealConnectionWriteApi(QolmsJotoModel mainModel, int linkgaSystemNo, string linkgaSystemId, CalomealAccessTokenSet token, bool deleteFlag)
        {
            var apiArgs = new QhYappliPortalCalomealConnectionWriteApiArgs(
                QhApiTypeEnum.YappliPortalCalomealConnectionWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkgaSystemNo.ToString(),
                LinkageSystemId = linkgaSystemId,
                DeleteFlag = deleteFlag.ToString(),
                TokenSet = new QhApiCalomealTokenSetItem()
                {
                    Token = token.access_token,
                    TokenExpires = token.TokenExpires.ToApiDateString(),
                    RefreshToken = token.refresh_token,
                    RefreshTokenExpires = token.RefreshTokenExpires.ToApiDateString()
                }
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCalomealConnectionWriteApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException($"{QsApiManager.GetQolmsApiName(apiArgs)} API の実行に失敗しました。");
            }
        }

        public QhYappliPortalCalomealConnectionTokenReadApiResults ExecuteCalomealConnectionTokenReadApi(QolmsJotoModel mainModel, int linkgaSystemNo)
        {
            var apiArgs = new QhYappliPortalCalomealConnectionTokenReadApiArgs(
                QhApiTypeEnum.YappliPortalCalomealConnectionTokenRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkgaSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCalomealConnectionTokenReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException($"{QsApiManager.GetQolmsApiName(apiArgs)} API の実行に失敗しました。");
            }
        }

        public QhYappliNoteCalomealMealSyncReadApiResults ExecuteCalomealMealSyncRead( QolmsJotoModel mainModel, int linkgaSystemNo)
        {
            var apiArgs = new QhYappliNoteCalomealMealSyncReadApiArgs(
                QhApiTypeEnum.YappliNoteCalomealMealSyncRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = linkgaSystemNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliNoteCalomealMealSyncReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException($"{QsApiManager.GetQolmsApiName(apiArgs)} API の実行に失敗しました。");
            }
        }

        public QhYappliNoteCalomealMealSyncWriteApiResults ExecuteCalomealMealSyncWrite(QolmsJotoModel mainModel, List<NoteMeal2InputModel> model)
        {
            var apiArgs = new QhYappliNoteCalomealMealSyncWriteApiArgs(
                QhApiTypeEnum.YappliNoteCalomealMealSyncWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                MealItemN = model.ConvertAll(i => new QhApiCalomealMealSyncItem()
                {
                    AnalysisSet = i.AnalysisSet,
                    AnalysisType = i.AnalysisType.ToString(),
                    Calorie = i.Calorie.ToString(),
                    ChooseSet = i.ChooseSet,
                    DeleteFlag = i.DeleteFlag.ToString(),
                    HistoryId = i.HistoryId.ToString(),
                    ItemName = i.ItemName.ToString(),
                    MealType = i.MealType.ToString(),
                    Rate = i.Rate.ToString(),
                    RecordDate = i.RecordDate.ToApiDateString(),
                    HasImage = i.HasImage.ToString()
                })
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliNoteCalomealMealSyncWriteApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException($"{QsApiManager.GetQolmsApiName(apiArgs)} API の実行に失敗しました。");
            }
        }

    }

}
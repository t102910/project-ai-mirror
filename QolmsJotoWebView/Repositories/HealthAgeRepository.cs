using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IHealthAgeRepository
    {
        QhYappliHealthAgeReadApiResults ExecuteHealthAgeReadApi(QolmsJotoModel mainModel);

        QhYappliHealthAgeReportReadApiResults ExecuteHealthAgeReportRead(QolmsJotoModel mainModel, QjHealthAgeReportTypeEnum healthAgeReportType, int dataCount);


        QhYappliHealthAgeEditReadApiResults ExecuteHealthAgeEditReadApi(QolmsJotoModel mainModel);

        QhYappliHealthAgeEditWriteApiResults ExecuteHealthAgeEditWriteApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel, List<QhApiHealthAgeResponseItem> responseN);

        QhYappliHealthAgeEditBayesianWriteApiResults ExecuteHealthAgeEditBayesianWriteApi(Guid accountKey, QhApiHealthAgeResponseItem response, Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey);
    }

    public class HealthAgeRepository: IHealthAgeRepository
    {

        /// <summary>
        /// 「健康年齢測定」画面 インプット モデル の内容を、
        /// API 用の健康年齢値情報のリストへ変換します。
        /// </summary>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// API 用の健康年齢値情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        private List<QhApiHealthAgeValueItem> ToApiHealthAgeValueList(HealthAgeEditInputModel inputModel)
        {
            List<QhApiHealthAgeValueItem> result = new List<QhApiHealthAgeValueItem>();
            string recordDate = inputModel.RecordDate.ToApiDateString(); // 日まで

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.BMI.ToString(),
                RecordDate = recordDate,
                Value = inputModel.BMI.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch014.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch014.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch016.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch016.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch019.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch019.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch021.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch021.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch023.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch023.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch025.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch025.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch027.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch027.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch029.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch029.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch035.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch035.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch035FBG.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch035FBG.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch037.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch037.ToString()
            });

            result.Add(new QhApiHealthAgeValueItem
            {
                HealthAgeValueType = QjHealthAgeValueTypeEnum.Ch039.ToString(),
                RecordDate = recordDate,
                Value = inputModel.Ch039.ToString()
            });

            return result;
        }



        public QhYappliHealthAgeReadApiResults ExecuteHealthAgeReadApi(QolmsJotoModel mainModel)
        {
            QhYappliHealthAgeReadApiArgs apiArgs = new QhYappliHealthAgeReadApiArgs(
                QhApiTypeEnum.YappliHealthAgeRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                PageNo = Convert.ToByte(QjPageNoTypeEnum.HealthAge).ToString(),
                IsInitialize = bool.TrueString,
                Birthday = mainModel.AuthorAccount.Birthday.ToApiDateString()
            };

            QhYappliHealthAgeReadApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliHealthAgeReadApiResults>(
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
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }

        public QhYappliHealthAgeReportReadApiResults ExecuteHealthAgeReportRead(
            QolmsJotoModel mainModel,
            QjHealthAgeReportTypeEnum healthAgeReportType,
            int dataCount)
        {
            QhYappliHealthAgeReportReadApiArgs apiArgs = new QhYappliHealthAgeReportReadApiArgs(
                QhApiTypeEnum.YappliHealthAgeReportRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                HealthAgeReportType = healthAgeReportType.ToString(),
                DataCount = dataCount.ToString()
            };

            QhYappliHealthAgeReportReadApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliHealthAgeReportReadApiResults>(
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
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }



        /// <summary>
        /// 「健康年齢測定」画面の取得 API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <returns>
        /// 成功なら Web API 戻り値クラス、失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public QhYappliHealthAgeEditReadApiResults ExecuteHealthAgeEditReadApi(QolmsJotoModel mainModel)
        {
            QhYappliHealthAgeEditReadApiArgs apiArgs = new QhYappliHealthAgeEditReadApiArgs(
                QhApiTypeEnum.YappliHealthAgeEditRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                PageNo = Convert.ToByte(QjPageNoTypeEnum.HealthAgeEdit).ToString()
            };

            QhYappliHealthAgeEditReadApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliHealthAgeEditReadApiResults>(
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
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }

        /// <summary>
        /// 「健康年齢測定」画面の登録 API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <param name="responseN">健康年齢 Web API のレスポンス情報のリスト。</param>
        /// <returns>
        /// 成功なら Web API 戻り値クラス、失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public QhYappliHealthAgeEditWriteApiResults ExecuteHealthAgeEditWriteApi(
            QolmsJotoModel mainModel,
            HealthAgeEditInputModel inputModel,
            List<QhApiHealthAgeResponseItem> responseN)
        {
            QhYappliHealthAgeEditWriteApiArgs apiArgs = new QhYappliHealthAgeEditWriteApiArgs(
                QhApiTypeEnum.YappliHealthAgeEditWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                PageNo = Convert.ToByte(QjPageNoTypeEnum.HealthAgeEdit).ToString(),
                HealthAgeValueN = this.ToApiHealthAgeValueList(inputModel),
                HealthAgeResponseN = responseN
            };

            QhYappliHealthAgeEditWriteApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliHealthAgeEditWriteApiResults>(
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
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }

        /// <summary>
        /// 健康年齢（ベイジアン）の登録 API を実行します。
        /// 体重の編集後に使用します。
        /// </summary>
        /// <param name="accountKey">アカウントキー。</param>
        /// <param name="response">健康年齢 Web API のレスポンス情報。</param>
        /// <param name="apiExecutor">Web API の実行者のアカウントキー。</param>
        /// <param name="apiExecutorName">Web API の実行者名。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="apiAuthorizeKey">API 認証キー。</param>
        /// <returns>
        /// 成功なら Web API 戻り値クラス、失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public QhYappliHealthAgeEditBayesianWriteApiResults ExecuteHealthAgeEditBayesianWriteApi(
            Guid accountKey,
            QhApiHealthAgeResponseItem response,
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey)
        {
            QhYappliHealthAgeEditBayesianWriteApiArgs apiArgs = new QhYappliHealthAgeEditBayesianWriteApiArgs(
                QhApiTypeEnum.YappliHealthAgeEditBayesianWrite,
                QsApiSystemTypeEnum.Qolms,
                apiExecutor,
                apiExecutorName
            )
            {
                ActorKey = accountKey.ToApiGuidString(),
                HealthAgeResponse = response
            };

            QhYappliHealthAgeEditBayesianWriteApiResults apiResults = QsApiManager.ExecuteQolmsApi<QhYappliHealthAgeEditBayesianWriteApiResults>(
                apiArgs,
                sessionId,
                apiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs))
                );
            }
        }

    }
}
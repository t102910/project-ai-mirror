using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsHealthAgeApiCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IJmdcHealthAgeApiRepository
    {
        QhApiHealthAgeResponseItem ExecuteCalculationApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel);
        QhApiHealthAgeResponseItem ExecuteAgeDistributionApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel);
        QhApiHealthAgeResponseItem ExecuteInsDevianceApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel);
        QhApiHealthAgeResponseItem ExecuteInsComparisonApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel);
        QhApiHealthAgeResponseItem ExecuteAdviceApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel);
        QhApiHealthAgeResponseItem ExecuteBayesianApi(QjSexTypeEnum sexType, DateTime birthday, HealthAgeEditInputModel inputModel, decimal plus);
    }

    public class JmdcHealthAgeApiRepository: IJmdcHealthAgeApiRepository
    {
        #region Constant

        /// <summary>
        /// JMDC 健康年齢 Web API 用の ID を保持します（任意値）。
        /// </summary>
        /// <remarks></remarks>
        private string jmdcId = "jotohdr";

        #endregion

        /// <summary>
        /// 例外メッセージを構築します。
        /// </summary>
        /// <param name="ex">例外オブジェクト。</param>
        /// <param name="builder">
        /// メッセージが格納される可変型文字列（オプショナル）。
        /// 未指定の場合はメソッド内部でインスタンスを作成。
        /// </param>
        /// <returns>メッセージが格納された可変型文字列。</returns>
        /// <remarks></remarks>
        private StringBuilder BuildExceptionMessage(Exception ex, StringBuilder builder = null)
        {
            if (builder == null)
            {
                builder = new StringBuilder();
            }

            if (ex != null)
            {
                builder.AppendFormat("■{0}：{1}", ex.GetType().ToString(), ex.Message).AppendLine();

                if (ex.InnerException != null)
                {
                    builder = BuildExceptionMessage(ex.InnerException, builder);
                }
            }

            return builder;
        }

        #region JMDC 健康年齢 Web API

        /// <summary>
        /// 健康年齢算出 API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// 健康年齢 Web API のレスポンス情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        public  QhApiHealthAgeResponseItem ExecuteCalculationApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel)
        {
            QhApiHealthAgeResponseItem result = new QhApiHealthAgeResponseItem();
            calculationApiArgs apiArgs = new calculationApiArgs();
            calculationApiResults apiResults = null;

            result.RecordDate = inputModel.RecordDate.Date.ToApiDateString(); // 日まで
            result.ApiName = apiArgs.ApiName;
            result.ValueSet = string.Empty;
            result.Status = byte.MinValue.ToString();
            result.StatusCode = int.MinValue.ToString();
            result.Message = string.Empty;

            apiArgs.id = this.jmdcId;
            apiArgs.visitDate = inputModel.RecordDate.ToString("yyyy/MM/dd");
            apiArgs.gender = mainModel.AuthorAccount.SexType == QjSexTypeEnum.Male ? 0 : 1;
            apiArgs.age = mainModel.AuthorAccount.GetAgeOn(inputModel.RecordDate.Date);
            apiArgs.height = 0m;
            apiArgs.weight = 0m;
            apiArgs.bmi = inputModel.BMI.TryToValueType(decimal.Zero);
            apiArgs.ch014 = inputModel.Ch014.TryToValueType(decimal.Zero);
            apiArgs.ch016 = inputModel.Ch016.TryToValueType(decimal.Zero);
            apiArgs.ch019 = inputModel.Ch019.TryToValueType(decimal.Zero);
            apiArgs.ch021 = inputModel.Ch021.TryToValueType(decimal.Zero);
            apiArgs.ch023 = inputModel.Ch023.TryToValueType(decimal.Zero);
            apiArgs.ch025 = inputModel.Ch025.TryToValueType(decimal.Zero);
            apiArgs.ch027 = inputModel.Ch027.TryToValueType(decimal.Zero);
            apiArgs.ch029 = inputModel.Ch029.TryToValueType(decimal.Zero);
            apiArgs.ch035 = inputModel.Ch035.TryToValueType(decimal.Zero);
            apiArgs.ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(decimal.Zero);
            apiArgs.ch037 = inputModel.Ch037.TryToValueType(decimal.Zero);
            apiArgs.ch039 = inputModel.Ch039.TryToValueType(decimal.Zero);

            try
            {
                apiResults = QsHealthAgeApiManager.Execute<calculationApiArgs, calculationApiResults>(apiArgs);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                this.BuildExceptionMessage(ex, sb);
                result.Message = sb.ToString();
            }

            if (apiResults != null && apiResults.IsSuccess)
            {
                result.ValueSet = apiResults.ResponseString;
                result.Status = 2.ToString();
                result.StatusCode = apiResults.StatusCode.ToString();
            }

            return result;
        }

        /// <summary>
        /// 同世代健康年齢分布取得 API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// 健康年齢 Web API のレスポンス情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        public QhApiHealthAgeResponseItem ExecuteAgeDistributionApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel)
        {
            QhApiHealthAgeResponseItem result = new QhApiHealthAgeResponseItem();
            ageDistributionApiArgs apiArgs = new ageDistributionApiArgs();
            ageDistributionApiResults apiResults = null;

            result.RecordDate = inputModel.RecordDate.Date.ToApiDateString(); // 日まで
            result.ApiName = apiArgs.ApiName;
            result.ValueSet = string.Empty;
            result.Status = byte.MinValue.ToString();
            result.StatusCode = int.MinValue.ToString();
            result.Message = string.Empty;

            apiArgs.id = this.jmdcId;
            apiArgs.gender = mainModel.AuthorAccount.SexType == QjSexTypeEnum.Male ? 0 : 1;
            apiArgs.age = mainModel.AuthorAccount.GetAgeOn(inputModel.RecordDate.Date);

            try
            {
                apiResults = QsHealthAgeApiManager.Execute<ageDistributionApiArgs, ageDistributionApiResults>(apiArgs);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                this.BuildExceptionMessage(ex, sb);
                result.Message = sb.ToString();
            }

            if (apiResults != null && apiResults.IsSuccess)
            {
                result.ValueSet = apiResults.ResponseString;
                result.Status = 2.ToString();
                result.StatusCode = apiResults.StatusCode.ToString();
            }

            return result;
        }

        /// <summary>
        /// 健診結果レベル判定 API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// 健康年齢 Web API のレスポンス情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        public QhApiHealthAgeResponseItem ExecuteInsDevianceApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel)
        {
            QhApiHealthAgeResponseItem result = new QhApiHealthAgeResponseItem();
            insDevianceApiArgs apiArgs = new insDevianceApiArgs();
            insDevianceApiResults apiResults = null;

            result.RecordDate = inputModel.RecordDate.Date.ToApiDateString(); // 日まで
            result.ApiName = apiArgs.ApiName;
            result.ValueSet = string.Empty;
            result.Status = byte.MinValue.ToString();
            result.StatusCode = int.MinValue.ToString();
            result.Message = string.Empty;

            apiArgs.id = this.jmdcId;
            apiArgs.height = 0m;
            apiArgs.weight = 0m;
            apiArgs.bmi = inputModel.BMI.TryToValueType(decimal.Zero);
            apiArgs.ch014 = inputModel.Ch014.TryToValueType(decimal.Zero);
            apiArgs.ch016 = inputModel.Ch016.TryToValueType(decimal.Zero);
            apiArgs.ch019 = inputModel.Ch019.TryToValueType(decimal.Zero);
            apiArgs.ch021 = inputModel.Ch021.TryToValueType(decimal.Zero);
            apiArgs.ch023 = inputModel.Ch023.TryToValueType(decimal.Zero);
            apiArgs.ch025 = inputModel.Ch025.TryToValueType(decimal.Zero);
            apiArgs.ch027 = inputModel.Ch027.TryToValueType(decimal.Zero);
            apiArgs.ch029 = inputModel.Ch029.TryToValueType(decimal.Zero);
            apiArgs.ch035 = inputModel.Ch035.TryToValueType(decimal.Zero);
            apiArgs.ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(decimal.Zero);

            try
            {
                apiResults = QsHealthAgeApiManager.Execute<insDevianceApiArgs, insDevianceApiResults>(apiArgs);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                this.BuildExceptionMessage(ex, sb);
                result.Message = sb.ToString();
            }

            if (apiResults != null && apiResults.IsSuccess)
            {
                result.ValueSet = apiResults.ResponseString;
                result.Status = 2.ToString();
                result.StatusCode = apiResults.StatusCode.ToString();
            }

            return result;
        }

        /// <summary>
        /// 同世代健診値比較 API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// 健康年齢 Web API のレスポンス情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        public QhApiHealthAgeResponseItem ExecuteInsComparisonApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel)
        {
            QhApiHealthAgeResponseItem result = new QhApiHealthAgeResponseItem();
            insComparisonApiArgs apiArgs = new insComparisonApiArgs();
            insComparisonApiResults apiResults = null;

            result.RecordDate = inputModel.RecordDate.Date.ToApiDateString(); // 日まで
            result.ApiName = apiArgs.ApiName;
            result.ValueSet = string.Empty;
            result.Status = byte.MinValue.ToString();
            result.StatusCode = int.MinValue.ToString();
            result.Message = string.Empty;

            apiArgs.id = this.jmdcId;
            apiArgs.gender = mainModel.AuthorAccount.SexType == QjSexTypeEnum.Male ? 0 : 1;
            apiArgs.age = mainModel.AuthorAccount.GetAgeOn(inputModel.RecordDate.Date);
            apiArgs.height = 0m;
            apiArgs.weight = 0m;
            apiArgs.bmi = inputModel.BMI.TryToValueType(decimal.Zero);
            apiArgs.ch014 = inputModel.Ch014.TryToValueType(decimal.Zero);
            apiArgs.ch016 = inputModel.Ch016.TryToValueType(decimal.Zero);
            apiArgs.ch019 = inputModel.Ch019.TryToValueType(decimal.Zero);
            apiArgs.ch021 = inputModel.Ch021.TryToValueType(decimal.Zero);
            apiArgs.ch023 = inputModel.Ch023.TryToValueType(decimal.Zero);
            apiArgs.ch025 = inputModel.Ch025.TryToValueType(decimal.Zero);
            apiArgs.ch027 = inputModel.Ch027.TryToValueType(decimal.Zero);
            apiArgs.ch029 = inputModel.Ch029.TryToValueType(decimal.Zero);
            apiArgs.ch035 = inputModel.Ch035.TryToValueType(decimal.Zero);
            apiArgs.ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(decimal.Zero);

            try
            {
                apiResults = QsHealthAgeApiManager.Execute<insComparisonApiArgs, insComparisonApiResults>(apiArgs);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                this.BuildExceptionMessage(ex, sb);
                result.Message = sb.ToString();
            }

            if (apiResults != null && apiResults.IsSuccess)
            {
                result.ValueSet = apiResults.ResponseString;
                result.Status = 2.ToString();
                result.StatusCode = apiResults.StatusCode.ToString();
            }

            return result;
        }

        /// <summary>
        /// 健康年齢改善アドバイス API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// 健康年齢 Web API のレスポンス情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        public QhApiHealthAgeResponseItem ExecuteAdviceApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel)
        {
            QhApiHealthAgeResponseItem result = new QhApiHealthAgeResponseItem();
            adviceApiArgs apiArgs = new adviceApiArgs();
            adviceApiResults apiResults = null;

            result.RecordDate = inputModel.RecordDate.Date.ToApiDateString(); // 日まで
            result.ApiName = apiArgs.ApiName;
            result.ValueSet = string.Empty;
            result.Status = byte.MinValue.ToString();
            result.StatusCode = int.MinValue.ToString();
            result.Message = string.Empty;

            apiArgs.id = this.jmdcId;
            apiArgs.gender = mainModel.AuthorAccount.SexType == QjSexTypeEnum.Male ? 0 : 1;
            apiArgs.age = mainModel.AuthorAccount.GetAgeOn(inputModel.RecordDate.Date);
            apiArgs.height = 0m;
            apiArgs.weight = 0m;
            apiArgs.bmi = inputModel.BMI.TryToValueType(decimal.Zero);
            apiArgs.ch014 = inputModel.Ch014.TryToValueType(decimal.Zero);
            apiArgs.ch016 = inputModel.Ch016.TryToValueType(decimal.Zero);
            apiArgs.ch019 = inputModel.Ch019.TryToValueType(decimal.Zero);
            apiArgs.ch021 = inputModel.Ch021.TryToValueType(decimal.Zero);
            apiArgs.ch023 = inputModel.Ch023.TryToValueType(decimal.Zero);
            apiArgs.ch025 = inputModel.Ch025.TryToValueType(decimal.Zero);
            apiArgs.ch027 = inputModel.Ch027.TryToValueType(decimal.Zero);
            apiArgs.ch029 = inputModel.Ch029.TryToValueType(decimal.Zero);
            apiArgs.ch035 = inputModel.Ch035.TryToValueType(decimal.Zero);
            apiArgs.ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(decimal.Zero);

            try
            {
                apiResults = QsHealthAgeApiManager.Execute<adviceApiArgs, adviceApiResults>(apiArgs);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                this.BuildExceptionMessage(ex, sb);
                result.Message = sb.ToString();
            }

            if (apiResults != null && apiResults.IsSuccess)
            {
                result.ValueSet = apiResults.ResponseString;
                result.Status = 2.ToString();
                result.StatusCode = apiResults.StatusCode.ToString();
            }

            return result;
        }

        /// <summary>
        /// 健康年齢ベイジアンネットワーク算出 API を実行します。
        /// 体重の編集後に使用します。
        /// </summary>
        /// <param name="sexType">性別の種別。</param>
        /// <param name="birthday">生年月日。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <param name="plus">今回測定した BMI。</param>
        /// <returns>
        /// 健康年齢 Web API のレスポンス情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        public QhApiHealthAgeResponseItem ExecuteBayesianApi(QjSexTypeEnum sexType, DateTime birthday, HealthAgeEditInputModel inputModel, decimal plus)
        {
            QhApiHealthAgeResponseItem result = new QhApiHealthAgeResponseItem();
            bayesianApiArgs apiArgs = new bayesianApiArgs();
            bayesianApiResults apiResults = null;

            result.RecordDate = inputModel.RecordDate.Date.ToApiDateString(); // 日まで
            result.ApiName = apiArgs.ApiName;
            result.ValueSet = string.Empty;
            result.Status = byte.MinValue.ToString();
            result.StatusCode = int.MinValue.ToString();
            result.Message = string.Empty;

            apiArgs.id = this.jmdcId;
            apiArgs.gender = sexType == QjSexTypeEnum.Male ? 0 : 1;
            apiArgs.age = DateHelper.GetAge(birthday, inputModel.RecordDate.Date);
            apiArgs.bmi = inputModel.BMI.TryToValueType(decimal.Zero);
            apiArgs.ch014 = inputModel.Ch014.TryToValueType(decimal.Zero);
            apiArgs.ch016 = inputModel.Ch016.TryToValueType(decimal.Zero);
            apiArgs.ch019 = inputModel.Ch019.TryToValueType(decimal.Zero);
            apiArgs.ch021 = inputModel.Ch021.TryToValueType(decimal.Zero);
            apiArgs.ch023 = inputModel.Ch023.TryToValueType(decimal.Zero);
            apiArgs.ch025 = inputModel.Ch025.TryToValueType(decimal.Zero);
            apiArgs.ch027 = inputModel.Ch027.TryToValueType(decimal.Zero);
            apiArgs.ch029 = inputModel.Ch029.TryToValueType(decimal.Zero);
            apiArgs.ch035 = inputModel.Ch035.TryToValueType(decimal.Zero);
            apiArgs.ch035_Fblood_glucose = inputModel.Ch035FBG.TryToValueType(decimal.Zero);
            apiArgs.ch037 = inputModel.Ch037.TryToValueType(decimal.Zero);
            apiArgs.ch039 = inputModel.Ch039.TryToValueType(decimal.Zero);
            apiArgs.plus = plus;

            try
            {
                apiResults = QsHealthAgeApiManager.Execute<bayesianApiArgs, bayesianApiResults>(apiArgs);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                this.BuildExceptionMessage(ex, sb);
                result.Message = sb.ToString();
            }

            if (apiResults != null && apiResults.IsSuccess)
            {
                result.ValueSet = apiResults.ResponseString;
                result.Status = 2.ToString();
                result.StatusCode = apiResults.StatusCode.ToString();
            }

            return result;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsHealthAgeApiCoreV1;
using MGF.QOLMS.QolmsJotoWebView.Repositories;

namespace MGF.QOLMS.QolmsJotoWebView
{
  
    /// <summary>
    /// 「健康年齢測定」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class HealthAgeEditWorker
    {

        IHealthAgeRepository _healthAgeRepo;
        IJmdcHealthAgeApiRepository _jmdcRepo;

        #region Constructor

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        public HealthAgeEditWorker(IHealthAgeRepository healthAgeRepository, IJmdcHealthAgeApiRepository jmdcHealthAgeApiRepository)
        {
            _healthAgeRepo = healthAgeRepository;
            _jmdcRepo = jmdcHealthAgeApiRepository;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// JMDC 健康年齢 Web API がメンテナンス中か判定します。
        /// </summary>
        /// <param name="refMessage">メンテナンス中の場合にメッセージが格納される変数。</param>
        /// <returns>
        /// メンテナンスなら True、そうでなければ False。
        /// </returns>
        /// <remarks></remarks>
        private bool CheckMaintenance(ref string refMessage)
        {
            refMessage = string.Empty;

            bool result = false;
            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(
                now.Year,
                now.Month,
                22 + DayOfWeek.Saturday - new DateTime(now.Year, now.Month, 1).DayOfWeek,
                20,
                0,
                0
            ); // 第 4 土曜日 20 時
            DateTime endDate = startDate.AddHours(12); // 翌 8 時

            if (now >= startDate && now <= endDate)
            {
                // 定期メンテナンス中
                refMessage = string.Format(
                    "{0}{1}{2}",
                    "健康年齢WEBAPIは定期メンテナンス中です。",
                    Environment.NewLine,
                    "（毎月第四土曜日20時～翌8時）"
                );

                result = true;
            }

            // TODO: 臨時メンテナンスの対応を検討する

            return result;
        }

        #endregion


        #region Public Method

        /// <summary>
        /// メインモデルを指定して、「健康年齢測定」画面 インプット モデルを作成します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="fromPageNoType">遷移元の画面番号の種別。</param>
        /// <returns>
        /// 成功なら「健康年齢測定」画面 インプット モデル、失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public HealthAgeEditInputModel CreateInputModel(QolmsJotoModel mainModel, QjPageNoTypeEnum fromPageNoType)
        {
            HealthAgeEditInputModel result = new HealthAgeEditInputModel(mainModel, null);

            QhYappliHealthAgeEditReadApiResults apiResult = _healthAgeRepo.ExecuteHealthAgeEditReadApi(mainModel);
            Dictionary<QjHealthAgeValueTypeEnum, Tuple<DateTime, decimal>> dic = new Dictionary<QjHealthAgeValueTypeEnum, Tuple<DateTime, decimal>>();

            if (apiResult.IsSuccess.TryToValueType(false) && apiResult.HealthAgeValueN != null && apiResult.HealthAgeValueN.Any())
            {
                apiResult.HealthAgeValueN.ForEach(i =>
                {
                    QjHealthAgeValueTypeEnum key = i.HealthAgeValueType.TryToValueType(QjHealthAgeValueTypeEnum.None);

                    if (key != QjHealthAgeValueTypeEnum.None && !dic.ContainsKey(key))
                    {
                        dic.Add(
                            key,
                            new Tuple<DateTime, decimal>(
                                i.RecordDate.TryToValueType(DateTime.MinValue),
                                i.Value.TryToValueType(decimal.MinValue)
                            )
                        );
                    }
                });
            }

            result = new HealthAgeEditInputModel(mainModel, dic);
            string maintenanceMessage = string.Empty;
            result.IsMaintenance = this.CheckMaintenance(ref maintenanceMessage);
            result.MaintenanceMessage = maintenanceMessage;
            result.FromPageNoType = fromPageNoType;

            return result;
        }

        /// <summary>
        /// メインモデルおよび健診結果を指定して、「健康年齢測定」画面 インプット モデルを作成します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="recordDate">健診受診日。</param>
        /// <param name="valueN">
        /// 健康年齢測定の種別をキー、健診結果値を値とするディクショナリ。
        /// </param>
        /// <param name="pageNo">画面番号。</param>
        /// <returns>
        /// 成功なら「健康年齢測定」画面 インプット モデル、失敗なら例外をスロー。
        /// </returns>
        /// <remarks>
        /// <paramref name="valueN" /> のキーの意味。
        /// <see cref="QjHealthAgeValueTypeEnum.BMI" />：BMI
        /// <see cref="QjHealthAgeValueTypeEnum.Ch014" />：収縮期血圧
        /// <see cref="QjHealthAgeValueTypeEnum.Ch016" />：拡張期血圧
        /// <see cref="QjHealthAgeValueTypeEnum.Ch019" />：中性脂肪
        /// <see cref="QjHealthAgeValueTypeEnum.Ch021" />：HDL コレステロール
        /// <see cref="QjHealthAgeValueTypeEnum.Ch023" />：LDL コレステロール
        /// <see cref="QjHealthAgeValueTypeEnum.Ch025" />：GOT（AST）
        /// <see cref="QjHealthAgeValueTypeEnum.Ch027" />：GPT（ALT）
        /// <see cref="QjHealthAgeValueTypeEnum.Ch029" />：γ-GT（γ-GTP）
        /// <see cref="QjHealthAgeValueTypeEnum.Ch035" />：HbA1c（NGSP）
        /// <see cref="QjHealthAgeValueTypeEnum.Ch035FBG" />：空腹時血糖
        /// <see cref="QjHealthAgeValueTypeEnum.Ch037" />：尿糖（値は数値に変換、1：－、2：±、3：＋、4：＋＋、5：＋＋＋）
        /// <see cref="QjHealthAgeValueTypeEnum.Ch039" />：尿蛋白（定性）（値は数値に変換、1：－、2：±、3：＋、4：＋＋、5：＋＋＋）
        /// </remarks>
        public HealthAgeEditInputModel CreateInputModelByExamination(
            QolmsJotoModel mainModel,
            DateTime recordDate,
            Dictionary<QjHealthAgeValueTypeEnum, decimal> valueN,
            QjPageNoTypeEnum pageNo)
        {
            HealthAgeEditInputModel result = this.CreateInputModel(mainModel, pageNo);

            if (recordDate != DateTime.MinValue && valueN != null && valueN.Any())
            {
                Dictionary<QjHealthAgeValueTypeEnum, Tuple<DateTime, decimal>> dic = new Dictionary<QjHealthAgeValueTypeEnum, Tuple<DateTime, decimal>>();

                valueN.ToList().ForEach(i =>
                {
                    if (i.Key != QjHealthAgeValueTypeEnum.None && !dic.ContainsKey(i.Key))
                    {
                        dic.Add(
                            i.Key,
                            new Tuple<DateTime, decimal>(recordDate, i.Value)
                        );
                    }
                });

                HealthAgeEditInputModel tempModel = new HealthAgeEditInputModel(mainModel, dic);
                tempModel.RecordDate = recordDate;
                result.UpdateByInput(tempModel);
                result.FromPageNoType = pageNo;
            }

            return result;
        }

        /// <summary>
        /// 健診結果を使用して作成した「健康年齢測定」画面 インプット モデルを検証します。
        /// </summary>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// 失敗した検証の情報を保持するコレクション。
        /// </returns>
        /// <remarks></remarks>
        public IEnumerable<ValidationResult> ValidateByInputModelByExamination(HealthAgeEditInputModel inputModel)
        {
            return inputModel.Validate(null);
        }

        /// <summary>
        /// 健康年齢を登録します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <param name="responseN">健康年齢 Web API のレスポンス情報のリスト。</param>
        /// <returns>
        /// 成功なら True、失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool Edit(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel, List<QhApiHealthAgeResponseItem> responseN)
        {
            DateTime pointActionDate = DateTime.Now;
            DateTime pointTargetDate = new DateTime(
                inputModel.RecordDate.Year,
                inputModel.RecordDate.Month,
                inputModel.RecordDate.Day
            );
            DateTime pointLimitDate = new DateTime(
                pointActionDate.Year,
                pointActionDate.Month,
                1
            ).AddMonths(7).AddDays(-1); // ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）

            QhYappliHealthAgeEditWriteApiResults apiResult = _healthAgeRepo.ExecuteHealthAgeEditWriteApi(mainModel, inputModel, responseN);

            // アカウント情報の会員の種別を更新
            mainModel.AuthorAccount.MembershipType = apiResult.MembershipType.TryToValueType(QjMemberShipTypeEnum.Free);

            // 「健康年齢」画面のキャッシュをクリア
            mainModel.RemoveInputModelCache<HealthAgeViewModel>();

            //// 「ホーム」画面のキャッシュをクリア
            //mainModel.RemoveInputModelCache<PortalHomeViewModel>();

            return true;
        }

        /// <summary>
        /// 健診受診日の時点での年齢が、18 歳以上かつ 74 歳以下か判定します。
        /// </summary>
        /// <param name="birthday">生年月日。</param>
        /// <param name="recordDate">健診受診日。</param>
        /// <returns>
        /// 条件を満たすなら string.Empty、そうでなければエラーメッセージ。
        /// </returns>
        /// <remarks></remarks>
        public static string CheckRecordDate(DateTime birthday, DateTime recordDate)
        {
            string result = string.Empty;

            if (recordDate != DateTime.MinValue)
            {
                if (recordDate >= birthday)
                {
                    int age = DateHelper.GetAge(birthday, recordDate); // 健診受診日における年齢

                    if (age < 18 || age > 74)
                    {
                        result = "健診受診日の時点で18歳以上74歳以下である必要があります。";
                    }
                }
                else
                {
                    result = "健診受診日が不正です。";
                }
            }
            else
            {
                result = "健診受診日を入力してください。";
            }

            return result;
        }

        /// <summary>
        /// JMDC 健康年齢 Web API を実行します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <returns>
        /// Web API のレスポンス情報のリスト。
        /// </returns>
        /// <remarks></remarks>
        public List<QhApiHealthAgeResponseItem> ExecuteJmdcHealthAgeApi(QolmsJotoModel mainModel, HealthAgeEditInputModel inputModel)
        {
            List<QhApiHealthAgeResponseItem> result = new List<QhApiHealthAgeResponseItem>();

            // 会員レベルに関係なく API はすべて実行しておく
            result.Add(_jmdcRepo.ExecuteCalculationApi(mainModel, inputModel));
            result.Add(_jmdcRepo.ExecuteAgeDistributionApi(mainModel, inputModel));
            result.Add(_jmdcRepo.ExecuteInsDevianceApi(mainModel, inputModel));
            result.Add(_jmdcRepo.ExecuteInsComparisonApi(mainModel, inputModel));
            result.Add(_jmdcRepo.ExecuteAdviceApi(mainModel, inputModel));

            return result;
        }

        /// <summary>
        /// 健康年齢（ベイジアン）を登録します。体重の編集後に使用します。
        /// </summary>
        /// <param name="accountKey">アカウントキー。</param>
        /// <param name="sexType">性別の種別。</param>
        /// <param name="birthday">生年月日。</param>
        /// <param name="membershipType">会員の種別。</param>
        /// <param name="inputModel">「健康年齢測定」画面 インプット モデル。</param>
        /// <param name="weight">体重。</param>
        /// <param name="height">身長。</param>
        /// <param name="apiExecutor">Web API の実行者のアカウントキー。</param>
        /// <param name="apiExecutorName">Web API の実行者名。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="apiAuthorizeKey">API 認証キー。</param>
        /// <returns>
        /// 成功なら True、失敗なら False もしくは例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool EditBayesian(
            Guid accountKey,
            QjSexTypeEnum sexType,
            DateTime birthday,
            QjMemberShipTypeEnum membershipType,
            HealthAgeEditInputModel inputModel,
            decimal weight,
            decimal height,
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey)
        {
            bool result = false;
            QhApiHealthAgeResponseItem response = new QhApiHealthAgeResponseItem
            {
                RecordDate = inputModel.RecordDate.Date.ToApiDateString(),
                ApiName = new bayesianApiArgs().ApiName,
                ValueSet = string.Empty,
                Status = byte.MinValue.ToString(),
                StatusCode = int.MinValue.ToString(),
                Message = string.Empty
            };
            string message = string.Empty;

            // プレミアム会員なら実行
            if ((membershipType == QjMemberShipTypeEnum.LimitedTime
                || membershipType == QjMemberShipTypeEnum.Premium
                || membershipType == QjMemberShipTypeEnum.Business
                || membershipType == QjMemberShipTypeEnum.BusinessFree)
                && string.IsNullOrWhiteSpace(HealthAgeEditWorker.CheckRecordDate(birthday, inputModel.RecordDate))
                && !this.CheckMaintenance(ref message))
            {
                // 今回の BMI 値
                decimal plus = decimal.MinValue;

                if (weight > decimal.Zero && height > decimal.Zero)
                {
                    plus = Math.Truncate(weight / (height * height) * 100000) / 10;
                }

                if (plus > decimal.Zero)
                {
                    // 登録（更新）
                    response = _jmdcRepo.ExecuteBayesianApi(sexType, birthday, inputModel, plus);

                    if (!string.IsNullOrWhiteSpace(response.ValueSet)
                        && response.Status.TryToValueType(byte.MinValue) == 2
                        && response.StatusCode.TryToValueType(int.MinValue) == 200)
                    {
                        bayesianApiResults jsonObject = null;

                        try
                        {
                            jsonObject = new QolmsDbEntityV1.QsJsonSerializer().Deserialize<bayesianApiResults>(response.ValueSet.Trim());
                        }
                        catch
                        {
                        }

                        if (jsonObject != null)
                        {
                            QhYappliHealthAgeEditBayesianWriteApiResults apiResult = _healthAgeRepo.ExecuteHealthAgeEditBayesianWriteApi(
                                accountKey,
                                response,
                                apiExecutor,
                                apiExecutorName,
                                sessionId,
                                apiAuthorizeKey
                            );

                            result = apiResult.IsSuccess.TryToValueType(false);
                        }
                    }
                }
                else
                {
                    // 削除
                    response.ValueSet = string.Empty;
                    response.Status = "2";
                    response.StatusCode = "200";

                    QhYappliHealthAgeEditBayesianWriteApiResults apiResult = _healthAgeRepo.ExecuteHealthAgeEditBayesianWriteApi(
                        accountKey,
                        response,
                        apiExecutor,
                        apiExecutorName,
                        sessionId,
                        apiAuthorizeKey
                    );

                    result = apiResult.IsSuccess.TryToValueType(false);
                }
            }

            return result;
        }

        #endregion
    }

}
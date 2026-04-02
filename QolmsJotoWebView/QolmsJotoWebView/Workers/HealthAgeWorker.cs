using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「健康年齢」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class HealthAgeWorker
    {

        IHealthAgeRepository _healthAgeRepo;

        #region Constant

        private const int DEFAULT_DATA_COUNT = 3;

        #endregion

        #region Constructor

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        public HealthAgeWorker(IHealthAgeRepository healthAgeRepo)
        {
            _healthAgeRepo = healthAgeRepo;
        }

        #endregion

        #region Private Method


        #endregion

        #region Public Method

        /// <summary>
        /// 「健康年齢」画面ビューモデルを作成します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="fromPageNoType">遷移元の画面番号の種別。</param>
        /// <returns>
        /// 「健康年齢」画面ビューモデル。
        /// </returns>
        /// <remarks></remarks>
        public HealthAgeViewModel CreateViewModel(QolmsJotoModel mainModel, QjPageNoTypeEnum fromPageNoType)
        {
            HealthAgeViewModel result = mainModel.GetInputModelCache<HealthAgeViewModel>(); // キャッシュから取得

            if (result == null || result.MembershipType != mainModel.AuthorAccount.MembershipType || result.FromPageNoType != fromPageNoType)
            {
                // キャッシュが無ければ API を実行
                QhYappliHealthAgeReadApiResults apiResult = _healthAgeRepo.ExecuteHealthAgeReadApi(mainModel);

                result = new HealthAgeViewModel(mainModel, fromPageNoType);

                // TODO:
                result.AvailableHealthAgeReportN = apiResult.AvailableHealthAgeReportN.ConvertAll(
                    i => new AvailableHealthAgeReportItem()
                    {
                        HealthAgeReportType = i.HealthAgeReportType.TryToValueType(QjHealthAgeReportTypeEnum.None),
                        LatestDate = i.LatestDate.TryToValueType(DateTime.MinValue)
                    }
                );

                // 会員の種別
                result.MembershipType = apiResult.MembershipType.TryToValueType(QjMemberShipTypeEnum.Free);

                // アカウント情報の会員の種別を更新
                mainModel.AuthorAccount.MembershipType = result.MembershipType;
                // result.MembershipType = QjMemberShipTypeEnum.Premium; // TODO: debug

                // 病院連携中かのフラグ
                result.HasHospitalConnection = apiResult.LinkegeSystemNoN.Any();

                // 健康年齢
                result.HealthAge = apiResult.HealthAge.TryToValueType(decimal.MinValue);

                // 実年齢との差
                result.HealthAgeDistance = apiResult.HealthAgeDistance.TryToValueType(decimal.MinValue);

                // 測定日
                result.LatestDate = apiResult.LatestDate.TryToValueType(DateTime.MinValue);

                // 予測医療費
                result.MedicalExpenses = apiResult.MedicalExpenses.TryToValueType(int.MinValue);

                // 健康年齢の推移のリスト
                result.HealthAgeN = apiResult.HealthAgeN.ConvertAll(i => i.ToHealthAgeValueItem());

                // 健康年齢改善アドバイス情報のリスト
                result.HealthAgeAdviceN = apiResult.HealthAgeAdviceN.ConvertAll(i => i.ToHealthAgeAdviceItem());

                // キャッシュへ追加
                mainModel.SetInputModelCache(result);
            }

            return result;
        }

        public QjHealthAgeReportPartialViewModelBase CreateReportPartialViewModel(
            QolmsJotoModel mainModel,
            HealthAgeViewModel pageViewModel,
            QjHealthAgeReportTypeEnum healthAgeReportType,
            ref string refPartialViewName)
        {
            refPartialViewName = string.Empty;

            QjHealthAgeReportPartialViewModelBase result = null;

            // 年齢分布
            if (healthAgeReportType == QjHealthAgeReportTypeEnum.Distribution)
            {
                refPartialViewName = "_HealthAgeDistributionReportPartialView";

                bool hasData = false;

                if (pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, ref hasData) && hasData)
                {
                    if (!pageViewModel.DistributionPartialViewModel.ReportItem.HealthAgeValueN.Any())
                    {
                        QhYappliHealthAgeReportReadApiResults reportResult = _healthAgeRepo.ExecuteHealthAgeReportRead(
                            mainModel,
                            QjHealthAgeReportTypeEnum.Distribution,
                            HealthAgeWorker.DEFAULT_DATA_COUNT
                        );

                        result = new HealthAgeDistributionReportPartialViewModel(
                            pageViewModel,
                            reportResult.HealthAgeReport.ToHealthAgeReportItem()
                        ); // TODO:

                        pageViewModel.DistributionPartialViewModel = (HealthAgeDistributionReportPartialViewModel)result;

                        mainModel.SetInputModelCache(pageViewModel);

                        return result;
                    }
                    else
                    {
                        return pageViewModel.DistributionPartialViewModel;
                    }
                }
                else
                {
                    return new HealthAgeDistributionReportPartialViewModel(pageViewModel, null);
                }
            }

            // 肥満
            if (healthAgeReportType == QjHealthAgeReportTypeEnum.Fat)
            {
                refPartialViewName = "_HealthAgeFatReportPartialView";

                bool hasData = false;

                if (pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, ref hasData) && hasData)
                {
                    if (!pageViewModel.FatPartialViewModel.ReportItem.HealthAgeValueN.Any())
                    {
                        QhYappliHealthAgeReportReadApiResults reportResult = _healthAgeRepo.ExecuteHealthAgeReportRead(
                            mainModel,
                            QjHealthAgeReportTypeEnum.Fat,
                            HealthAgeWorker.DEFAULT_DATA_COUNT
                        );

                        result = new HealthAgeFatReportPartialViewModel(
                            pageViewModel,
                            reportResult.HealthAgeReport.ToHealthAgeReportItem()
                        ); // TODO:

                        pageViewModel.FatPartialViewModel = (HealthAgeFatReportPartialViewModel)result;

                        mainModel.SetInputModelCache(pageViewModel);

                        return result;
                    }
                    else
                    {
                        return pageViewModel.FatPartialViewModel;
                    }
                }
                else
                {
                    return new HealthAgeFatReportPartialViewModel(pageViewModel, null);
                }
            }

            // 血糖
            if (healthAgeReportType == QjHealthAgeReportTypeEnum.Glucose)
            {
                refPartialViewName = "_HealthAgeGlucoseReportPartialView";

                bool hasData = false;

                if (pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, ref hasData) && hasData)
                {
                    if (!pageViewModel.GlucosePartialViewModel.ReportItem.HealthAgeValueN.Any())
                    {
                        QhYappliHealthAgeReportReadApiResults reportResult = _healthAgeRepo.ExecuteHealthAgeReportRead(
                            mainModel,
                            QjHealthAgeReportTypeEnum.Glucose,
                            HealthAgeWorker.DEFAULT_DATA_COUNT
                        );

                        result = new HealthAgeGlucoseReportPartialViewModel(
                            pageViewModel,
                            reportResult.HealthAgeReport.ToHealthAgeReportItem()
                        ); // TODO:

                        pageViewModel.GlucosePartialViewModel = (HealthAgeGlucoseReportPartialViewModel)result;

                        mainModel.SetInputModelCache(pageViewModel);

                        return result;
                    }
                    else
                    {
                        return pageViewModel.GlucosePartialViewModel;
                    }
                }
                else
                {
                    return new HealthAgeGlucoseReportPartialViewModel(pageViewModel, null);
                }
            }

            // 血圧
            if (healthAgeReportType == QjHealthAgeReportTypeEnum.Pressure)
            {
                refPartialViewName = "_HealthAgePressureReportPartialView";

                bool hasData = false;

                if (pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, ref hasData) && hasData)
                {
                    if (!pageViewModel.PressurePartialViewModel.ReportItem.HealthAgeValueN.Any())
                    {
                        QhYappliHealthAgeReportReadApiResults reportResult = _healthAgeRepo.ExecuteHealthAgeReportRead(
                            mainModel,
                            QjHealthAgeReportTypeEnum.Pressure,
                            HealthAgeWorker.DEFAULT_DATA_COUNT
                        );

                        result = new HealthAgePressureReportPartialViewModel(
                            pageViewModel,
                            reportResult.HealthAgeReport.ToHealthAgeReportItem()
                        ); // TODO:

                        pageViewModel.PressurePartialViewModel = (HealthAgePressureReportPartialViewModel)result;

                        mainModel.SetInputModelCache(pageViewModel);

                        return result;
                    }
                    else
                    {
                        return pageViewModel.PressurePartialViewModel;
                    }
                }
                else
                {
                    return new HealthAgePressureReportPartialViewModel(pageViewModel, null);
                }
            }

            // 脂質
            if (healthAgeReportType == QjHealthAgeReportTypeEnum.Lipid)
            {
                refPartialViewName = "_HealthAgeLipidReportPartialView";

                bool hasData = false;

                if (pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, ref hasData) && hasData)
                {
                    if (!pageViewModel.LipidPartialViewModel.ReportItem.HealthAgeValueN.Any())
                    {
                        QhYappliHealthAgeReportReadApiResults reportResult = _healthAgeRepo.ExecuteHealthAgeReportRead(
                            mainModel,
                            QjHealthAgeReportTypeEnum.Lipid,
                            HealthAgeWorker.DEFAULT_DATA_COUNT
                        );

                        result = new HealthAgeLipidReportPartialViewModel(
                            pageViewModel,
                            reportResult.HealthAgeReport.ToHealthAgeReportItem()
                        ); // TODO:

                        pageViewModel.LipidPartialViewModel = (HealthAgeLipidReportPartialViewModel)result;

                        mainModel.SetInputModelCache(pageViewModel);

                        return result;
                    }
                    else
                    {
                        return pageViewModel.LipidPartialViewModel;
                    }
                }
                else
                {
                    return new HealthAgeLipidReportPartialViewModel(pageViewModel, null);
                }
            }

            // 肝臓
            if (healthAgeReportType == QjHealthAgeReportTypeEnum.Liver)
            {
                refPartialViewName = "_HealthAgeLiverReportPartialView";

                bool hasData = false;

                if (pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, ref hasData) && hasData)
                {
                    if (!pageViewModel.LiverPartialViewModel.ReportItem.HealthAgeValueN.Any())
                    {
                        QhYappliHealthAgeReportReadApiResults reportResult = _healthAgeRepo.ExecuteHealthAgeReportRead(
                            mainModel,
                            QjHealthAgeReportTypeEnum.Liver,
                            HealthAgeWorker.DEFAULT_DATA_COUNT
                        );

                        result = new HealthAgeLiverReportPartialViewModel(
                            pageViewModel,
                            reportResult.HealthAgeReport.ToHealthAgeReportItem()
                        ); // TODO:

                        pageViewModel.LiverPartialViewModel = (HealthAgeLiverReportPartialViewModel)result;

                        mainModel.SetInputModelCache(pageViewModel);

                        return result;
                    }
                    else
                    {
                        return pageViewModel.LiverPartialViewModel;
                    }
                }
                else
                {
                    return new HealthAgeLiverReportPartialViewModel(pageViewModel, null);
                }
            }

            // 尿糖・尿蛋白
            if (healthAgeReportType == QjHealthAgeReportTypeEnum.Urine)
            {
                refPartialViewName = "_HealthAgeUrineReportPartialView";

                bool hasData = false;

                if (pageViewModel.IsAvailableHealthAgeReportType(healthAgeReportType, ref hasData) && hasData)
                {
                    if (!pageViewModel.UrinePartialViewModel.ReportItem.HealthAgeValueN.Any())
                    {
                        QhYappliHealthAgeReportReadApiResults reportResult = _healthAgeRepo.ExecuteHealthAgeReportRead(
                            mainModel,
                            QjHealthAgeReportTypeEnum.Urine,
                            HealthAgeWorker.DEFAULT_DATA_COUNT
                        );

                        result = new HealthAgeUrineReportPartialViewModel(
                            pageViewModel,
                            reportResult.HealthAgeReport.ToHealthAgeReportItem()
                        ); // TODO:

                        pageViewModel.UrinePartialViewModel = (HealthAgeUrineReportPartialViewModel)result;

                        mainModel.SetInputModelCache(pageViewModel);

                        return result;
                    }
                    else
                    {
                        return pageViewModel.UrinePartialViewModel;
                    }
                }
                else
                {
                    return new HealthAgeUrineReportPartialViewModel(pageViewModel, null);
                }
            }

            return result;
        }

        #endregion
    }

}

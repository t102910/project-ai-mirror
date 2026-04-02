using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    //外部連携用
    public class IntegrationController : QjMvcControllerBase
    {
        private const int MockCompanyLinkageSystemNo = 47001;
        private const string MockCompanyLinkageSystemName = "企業";
        private const string MockHospitalName = "城東区医師会病院";
        private const string MockHospitalPatientNo = "00012345";

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult FitbitConnection(byte? fromPageNo, bool? mockConnected)
        {
            var viewModel = new IntegrationFitbitConnectionViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                FitbitConnectedFlag = mockConnected ?? false
            };

            return View(viewModel);
        }


        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult OmronConnection(byte? fromPageNo, bool? mockConnected)
        {
            var viewModel = new IntegrationOmronConnectionViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                OmronConnectedFlag = mockConnected ?? false
            };

            return View(viewModel);
        }

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult TanitaConnection(byte? fromPageNo, bool? mockConnected)
        {
            bool isConnected = mockConnected ?? false;

            var viewModel = new IntegrationTanitaConnectionViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                ConnectionId = isConnected ? "mock-tanita-user" : string.Empty,
                AlkooConnectedFlag = false,
                BodyCompositionMeter = true,
                Sphygmomanometer = true,
                Pedometer = true
            };

            return View(viewModel);
        }

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnectionHome(byte? fromPageNo)
        {
            var viewModel = new IntegrationCompanyConnectionHomeViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                LinkageSystemName = MockCompanyLinkageSystemName
            };

            return View(viewModel);
        }

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnectionRequest(byte? fromPageNo, string linkageSystemName)
        {
            var viewModel = new IntegrationCompanyConnectionRequestViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                LinkageSystemName = string.IsNullOrWhiteSpace(linkageSystemName) ? MockCompanyLinkageSystemName : linkageSystemName,
                CompanyCode = string.Empty,
                EmployeeNo = string.Empty,
                FamilyName = "城東",
                GivenName = "太郎",
                FamilyKanaName = "ジョウトウ",
                GivenKanaName = "タロウ",
                SexLabel = "男性",
                BirthDateLabel = "1990年 1月 1日",
                IsPremiumMember = false,
                ShareBasicInfo = true,
                ShareContactNotebook = true,
                ShareVitalNotebook = true,
                ShareMedicineNotebook = true,
                ShareExaminationNotebook = true
            };

            return View(viewModel);
        }

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnection(byte? fromPageNo, int? linkageSystemNo, string linkageSystemName, bool? mockConnected)
        {
            bool isConnected = mockConnected ?? true;

            var viewModel = new IntegrationCompanyConnectionViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                LinkageSystemNo = linkageSystemNo ?? MockCompanyLinkageSystemNo,
                LinkageSystemName = string.IsNullOrWhiteSpace(linkageSystemName) ? MockCompanyLinkageSystemName : linkageSystemName,
                MockConnectedFlag = isConnected,
                ShareBasicInfo = true,
                ShareContactNotebook = true,
                ShareVitalNotebook = true,
                ShareMedicineNotebook = true,
                ShareExaminationNotebook = true
            };

            return View(viewModel);
        }

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnectionEdit(byte? fromPageNo, int? linkageSystemNo, string linkageSystemName)
        {
            var viewModel = new IntegrationCompanyConnectionEditViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                LinkageSystemNo = linkageSystemNo ?? MockCompanyLinkageSystemNo,
                LinkageSystemName = string.IsNullOrWhiteSpace(linkageSystemName) ? MockCompanyLinkageSystemName : linkageSystemName,
                MailAddress = "employee@example.jp",
                ShareBasicInfo = true,
                ShareContactNotebook = true,
                ShareVitalNotebook = true,
                ShareMedicineNotebook = true,
                ShareExaminationNotebook = true
            };

            return View(viewModel);
        }

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult HospitalConnection(byte? fromPageNo, string hospitalName, string patientNo, bool? mockConnected)
        {
            bool isConnected = mockConnected ?? true;

            var viewModel = new IntegrationHospitalConnectionViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                HospitalName = string.IsNullOrWhiteSpace(hospitalName) ? MockHospitalName : hospitalName,
                PatientNo = isConnected ? (string.IsNullOrWhiteSpace(patientNo) ? MockHospitalPatientNo : patientNo) : string.Empty,
                MockConnectedFlag = isConnected,
                ExaminationConnectedFlag = true
            };

            return View(viewModel);
        }

        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ActionResult HospitalConnectionRequest(byte? fromPageNo, string hospitalName)
        {
            var viewModel = new IntegrationHospitalConnectionRequestViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                HospitalName = string.IsNullOrWhiteSpace(hospitalName) ? MockHospitalName : hospitalName,
                HospitalCode = string.Empty,
                PatientNo = string.Empty,
                FamilyName = "城東",
                GivenName = "太郎",
                BirthDateLabel = "1990年 1月 1日",
                ConsentFlag = true
            };

            return View(viewModel);
        }

        /// <summary>
        /// 画面確認用です
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[QjAuthorize]
        //[QjLogging]
        public ContentResult MockPages()
        {
            string authority = Request?.Url?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:44384";

            var mockPages = new[]
            {
                new { Label = "タニタ連携（未連携）", Url = authority + Url.Action(nameof(TanitaConnection), "Integration", new { mockConnected = false }) },
                new { Label = "タニタ連携（連携済み）", Url = authority + Url.Action(nameof(TanitaConnection), "Integration", new { mockConnected = true }) },
                new { Label = "企業連携 ホーム", Url = authority + Url.Action(nameof(CompanyConnectionHome), "Integration", null) },
                new { Label = "企業連携 申請", Url = authority + Url.Action(nameof(CompanyConnectionRequest), "Integration", new { linkageSystemName = MockCompanyLinkageSystemName }) },
                new { Label = "企業連携 連携済み", Url = authority + Url.Action(nameof(CompanyConnection), "Integration", new { linkageSystemNo = MockCompanyLinkageSystemNo, linkageSystemName = MockCompanyLinkageSystemName, mockConnected = true }) },
                new { Label = "企業連携 未連携", Url = authority + Url.Action(nameof(CompanyConnection), "Integration", new { linkageSystemNo = MockCompanyLinkageSystemNo, linkageSystemName = MockCompanyLinkageSystemName, mockConnected = false }) },
                new { Label = "企業連携 編集", Url = authority + Url.Action(nameof(CompanyConnectionEdit), "Integration", new { linkageSystemNo = MockCompanyLinkageSystemNo, linkageSystemName = MockCompanyLinkageSystemName }) },
                new { Label = "病院連携 未連携", Url = authority + Url.Action(nameof(HospitalConnection), "Integration", new { hospitalName = MockHospitalName, mockConnected = false }) },
                new { Label = "病院連携 申請", Url = authority + Url.Action(nameof(HospitalConnectionRequest), "Integration", new { hospitalName = MockHospitalName }) },
                new { Label = "病院連携 連携済み", Url = authority + Url.Action(nameof(HospitalConnection), "Integration", new { hospitalName = MockHospitalName, patientNo = MockHospitalPatientNo, mockConnected = true }) }
            };

            string html = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /><title>Integration Mock Pages</title></head><body>"
                + "<h1>連携画面モック確認用URL</h1><ul>"
                + string.Join(string.Empty, mockPages.Select(x => string.Format("<li>{0}<br /><a href=\"{1}\">{1}</a></li>", x.Label, x.Url)))
                + "</ul></body></html>";

            return Content(html, "text/html");
        }

        private static QjPageNoTypeEnum ResolveFromPageNoType(byte? fromPageNo)
        {
            QjPageNoTypeEnum fromPageNoType = QjPageNoTypeEnum.PortalConnectionSetting;

            if (fromPageNo.HasValue && Enum.IsDefined(typeof(QjPageNoTypeEnum), fromPageNo.Value))
            {
                fromPageNoType = (QjPageNoTypeEnum)fromPageNo.Value;
            }

            return fromPageNoType;
        }

        #region 共通パーツ

        /// <summary>
        /// 「ヘッダー」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        public ActionResult IntegrationHeaderPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_IntegrationHeaderPartialView");
        }

        /// <summary>
        /// 「ヘッダー」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        [OutputCache(Duration = 600)]
        public ActionResult IntegrationFooterPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_IntegrationFooterPartialView");
        }

        #endregion
    }

}

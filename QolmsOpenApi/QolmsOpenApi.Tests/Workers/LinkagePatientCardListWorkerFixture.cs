using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class LinkagePatientCardListWorkerFixture
    {
        LinkagePatientCardListWorker _worker;
        Mock<IPatientCardRepository> _repo;

        Guid _accountKey = Guid.NewGuid();
        Guid _executor = Guid.NewGuid();
        Guid _facilityKey1 = Guid.NewGuid();
        Guid _facilityKey2 = Guid.NewGuid();
        DateTime _createdDate = DateTime.Now;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IPatientCardRepository>();
            _worker = new LinkagePatientCardListWorker(_repo.Object);
        }

        [TestMethod]
        public void AuthorKeyが不正でエラー()
        {
            var args = GetValidArgs();
            args.Executor = Guid.Empty.ToApiGuidString();

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains($"{nameof(args.AuthorKey)}").IsTrue();
            result.Result.Detail.Contains($"不正").IsTrue();
        }

        [TestMethod]
        public void Actorkeyが不正でエラー()
        {
            var args = GetValidArgs();
            args.ActorKey = Guid.Empty.ToApiGuidString();

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains($"{nameof(args.ActorKey)}").IsTrue();
            result.Result.Detail.Contains($"不正").IsTrue();
        }

        [TestMethod]
        public void DBからの診察券リスト取得に失敗でエラー()
        {
            var args = GetValidArgs();
            _repo.Setup(m => m.ReadPatientCardList(_accountKey,2,int.MinValue)).Throws(new Exception("hoge"));

            var result = _worker.Read(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("0500");
            result.Result.Detail.Contains($"DBからの診察券情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常に診察券リストが取得できる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var result = _worker.Read(args);

            // 成功
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            // 2件
            result.PatientCardItemN.Count.Is(2);
            var item1 = result.PatientCardItemN[0];
            var item2 = result.PatientCardItemN[1];

            // 正しく変換されている
            item1.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            item1.LinkUserId.Is("99999001");
            item1.Sequence.Is("0");
            item1.StatusType.Is("2");
            // カスタム書式 医療機関コード10桁 / 診察券番号フリーが設定されている
            item1.CustomCardCode.Is("270055999999999001");
            item1.AttachedFileN.Count.Is(0);
            item1.CreatedDate.Is(_createdDate.ToApiDateString());

            item2.FacilityKeyReference.Is(_facilityKey2.ToEncrypedReference());
            item2.LinkUserId.Is("9002");
            item2.Sequence.Is("1");
            item2.StatusType.Is("2");
            // カスタム書式 医療機関コード7桁 / 診察券番号6桁0埋めが設定されている
            item2.CustomCardCode.Is("0558888009002");
            item2.AttachedFileN.Count.Is(0);
            item2.CreatedDate.Is(_createdDate.ToApiDateString());
        }

        [TestMethod]
        public void 正常に連携システム番号でフィルタして診察券リストが取得できる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            args.LinkageSystemNo = "27003";

            var entityList = new List<QH_PATIENTCARD_FACILITY_VIEW>
            {
                new QH_PATIENTCARD_FACILITY_VIEW
                {
                    ACCOUNTKEY = _accountKey,
                    ADDRESS1 = "大阪府大阪市東淀川区",
                    ADDRESS2 = "下新庄1-3-5",
                    CARDCODE = 27004,
                    CARDNO = "99999001",
                    CITYNO = 0,
                    CREATEDDATE = _createdDate,
                    CustomCodeFormat = "{MedicalCode:10}{CardNo}",
                    FACILITYKEY = _facilityKey1,
                    FACILITYKANANAME = "シセツ",
                    FACILITYNAME = "施設",
                    FAX = "0666669999",
                    TEL = "0699996666",
                    MEDICALFACILITYCODE = "2700559999",
                    OFFICIALNAME = "",
                    POSTALCODE = "5558888",
                    PREFNO = 10,
                    SEQUENCE = 0,
                    STATUSTYPE = 2
                }                
            };

            _repo.Setup(m => m.ReadPatientCardList(_accountKey, 2, 27003)).Returns(entityList);


            var result = _worker.Read(args);

            // 成功
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            // 1件
            result.PatientCardItemN.Count.Is(1);
            var item1 = result.PatientCardItemN[0];

            // 正しく変換されている
            item1.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            item1.LinkUserId.Is("99999001");
            item1.Sequence.Is("0");
            item1.StatusType.Is("2");
            item1.CustomCardCode.Is("270055999999999001");
            item1.AttachedFileN.Count.Is(0);
            item1.CreatedDate.Is(_createdDate.ToApiDateString());
        }

        [TestMethod]
        public void 無効なバーコードフォーマットが指定されていてもフォールバック処理で正常とする()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            args.LinkageSystemNo = "27003";

            var entityList = new List<QH_PATIENTCARD_FACILITY_VIEW>
            {
                new QH_PATIENTCARD_FACILITY_VIEW
                {
                    ACCOUNTKEY = _accountKey,
                    ADDRESS1 = "大阪府大阪市東淀川区",
                    ADDRESS2 = "下新庄1-3-5",
                    CARDCODE = 27004,
                    CARDNO = "99999001",
                    CITYNO = 0,
                    CREATEDDATE = _createdDate,
                    CustomCodeFormat = "agdssf", // 無効な書式
                    FACILITYKEY = _facilityKey1,
                    FACILITYKANANAME = "シセツ",
                    FACILITYNAME = "施設",
                    FAX = "0666669999",
                    TEL = "0699996666",
                    MEDICALFACILITYCODE = "2700559999",
                    OFFICIALNAME = "",
                    POSTALCODE = "5558888",
                    PREFNO = 10,
                    SEQUENCE = 0,
                    STATUSTYPE = 2
                }
            };

            _repo.Setup(m => m.ReadPatientCardList(_accountKey, 2, 27003)).Returns(entityList);


            var result = _worker.Read(args);

            // 成功
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            // 1件
            result.PatientCardItemN.Count.Is(1);
            var item1 = result.PatientCardItemN[0];

            // 正しく変換されている
            item1.FacilityKeyReference.Is(_facilityKey1.ToEncrypedReference());
            item1.LinkUserId.Is("99999001");
            item1.Sequence.Is("0");
            item1.StatusType.Is("2");
            item1.CustomCardCode.Is("270055999999999001"); // フォールバックでデフォルトの書式が適用される
            item1.AttachedFileN.Count.Is(0);
            item1.CreatedDate.Is(_createdDate.ToApiDateString());
        }

        void SetupValidMethods(QoLinkagePatientCardListReadApiArgs args)
        {
            var entityList = new List<QH_PATIENTCARD_FACILITY_VIEW>
            {
                new QH_PATIENTCARD_FACILITY_VIEW
                {
                    ACCOUNTKEY = _accountKey,
                    ADDRESS1 = "大阪府大阪市東淀川区",
                    ADDRESS2 = "下新庄1-3-5",
                    CARDCODE = 27004,
                    CARDNO = "99999001",
                    CITYNO = 0,
                    CREATEDDATE = _createdDate,
                    CustomCodeFormat = "{MedicalCode:10}{CardNo}",
                    FACILITYKEY = _facilityKey1,
                    FACILITYKANANAME = "シセツ",
                    FACILITYNAME = "施設",
                    FAX = "0666669999",
                    TEL = "0699996666",
                    MEDICALFACILITYCODE = "2700559999",
                    OFFICIALNAME = "",
                    POSTALCODE = "5558888",
                    PREFNO = 10,
                    SEQUENCE = 0,
                    STATUSTYPE = 2
                },
                new QH_PATIENTCARD_FACILITY_VIEW
                {
                    ACCOUNTKEY = _accountKey,
                    ADDRESS1 = "大阪府大阪市東淀川区",
                    ADDRESS2 = "下新庄1-3-5",
                    CARDCODE = 27005,
                    CARDNO = "9002",
                    CITYNO = 0,
                    CREATEDDATE = _createdDate,
                    CustomCodeFormat = "{MedicalCode:7}{CardNo:6}",
                    FACILITYKEY = _facilityKey2,
                    FACILITYKANANAME = "シセツ",
                    FACILITYNAME = "施設",
                    FAX = "0666669999",
                    TEL = "0699996666",
                    MEDICALFACILITYCODE = "2700558888",
                    OFFICIALNAME = "",
                    POSTALCODE = "5558888",
                    PREFNO = 10,
                    SEQUENCE = 1,
                    STATUSTYPE = 2
                }
            };

            _repo.Setup(m => m.ReadPatientCardList(_accountKey,2, int.MinValue)).Returns(entityList);
        }

        QoLinkagePatientCardListReadApiArgs GetValidArgs()
        {
            return new QoLinkagePatientCardListReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _executor.ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{QsApiSystemTypeEnum.TisiOSApp}",
                StatusTypeFilter = "2"
            };
        }
    }
}

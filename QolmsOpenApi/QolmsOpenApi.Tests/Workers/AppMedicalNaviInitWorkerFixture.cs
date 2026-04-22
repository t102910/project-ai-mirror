using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AppMedicalNaviInitWorkerFixture
    {
        Mock<IFamilyRepository> _familyRepo;
        Mock<IFacilityRepository> _facilityRepo;
        Mock<IPatientCardRepository> _cardRepo;
        Mock<IAccountRepository> _accountRepo;

        AppMedicalNaviInitWorker _worker;

        Guid _accountKey = Guid.NewGuid();
        Guid _executor = Guid.NewGuid();
        Guid _facilityKey1 = Guid.NewGuid();
        Guid _facilityKey2 = Guid.NewGuid();
        Guid _facilityKey3 = Guid.NewGuid();
        List<QH_ACCOUNTINDEX_DAT> _accountRawDataList;

        [TestInitialize]
        public void Initilaize()
        {
            _familyRepo = new Mock<IFamilyRepository>();
            _facilityRepo = new Mock<IFacilityRepository>();
            _cardRepo = new Mock<IPatientCardRepository>();
            _accountRepo = new Mock<IAccountRepository>();

            _worker = new AppMedicalNaviInitWorker(_facilityRepo.Object, _cardRepo.Object, _familyRepo.Object,_accountRepo.Object);
        }

        [TestMethod]
        public void Email取得時処理で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();

            _accountRepo.Setup(m => m.GetAccountEmail(_accountKey)).Throws(new Exception());

            var results = _worker.GetInitData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("Emailの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 家族情報が存在しなかった場合はエラーとなる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 家族リストが空で返る設定
            _familyRepo.Setup(m => m.ReadFamilyList(_accountKey)).Returns(new List<QH_ACCOUNTINDEX_DAT>());

            var results = _worker.GetInitData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アカウント情報が存在しませんでした").IsTrue();
            results.Result.Detail.Contains("ユーザーの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void DBからの家族情報取得処理で例外発生したらエラーとなる()
        {
            var args = GetValidArgs();

            // 家族リストが空で返る設定
            _familyRepo.Setup(m => m.ReadFamilyList(_accountKey)).Throws(new Exception());

            var results = _worker.GetInitData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("DBからのアカウント情報リストの取得に失敗しました").IsTrue();
            results.Result.Detail.Contains("ユーザーの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 診察券取得に失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 診察券処理内部で例外を出す
            _cardRepo.Setup(m => m.ReadPatientCardList(_accountKey, 0, int.MinValue)).Throws(new Exception());

            var results = _worker.GetInitData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("診察券の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 施設情報の取得に失敗するとエラーとなる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var keyList = new List<Guid>
            {
                _facilityKey1,
                _facilityKey2,
                _facilityKey3
            };

            _facilityRepo.Setup(m => m.ReadMedicalFacilityListByKey(DateTime.MinValue, keyList.ToArray())).Throws(new Exception());

            var results = _worker.GetInitData(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("医療機関情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常にアプリ初期データを取得できる()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var results = _worker.GetInitData(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // アカウント・診察券情報件数一致
            results.AccountPatientCardN.Count.Is(2);
            // 本人アカウント情報
            var self = results.AccountPatientCardN[0].User;
            self.AccountKeyReference.Is(_accountKey.ToEncrypedReference());
            self.FamilyName.Is("仲村");
            self.GivenName.Is("聡");
            self.FamilyNameKana.Is("ナカムラ");
            self.GivenNameKana.Is("サトシ");
            self.NickName.Is("サト");
            self.Sex.Is((byte)1);
            self.Birthday.Is(new DateTime(1977, 3, 27).ToApiDateString());
            self.PersonPhotoReference.Is(_accountRawDataList[0].PHOTOKEY.ToEncrypedReference());
            // Jwtは生成されていれば良しとする
            self.AccessKey.IsNotNull();

            // 診察券は2枚。細かい内容のテストはLinkagePatientCardListWorkerFixtureの方で行う
            results.AccountPatientCardN[0].PatientCardItemN.Count.Is(2);

            // 家族アカウント情報
            var child = results.AccountPatientCardN[1].User;
            child.AccountKeyReference.Is(_accountRawDataList[1].ACCOUNTKEY.ToEncrypedReference());
            child.FamilyName.Is("中村");
            child.GivenName.Is("花子");
            child.FamilyNameKana.Is("ナカムラ");
            child.GivenNameKana.Is("ハナコ");
            child.NickName.Is("");
            child.Sex.Is((byte)2);
            child.Birthday.Is(new DateTime(2000, 6, 7).ToApiDateString());
            // 写真は未設定
            child.PersonPhotoReference.Is(string.Empty);
            // Jwtは生成されていれば良しとする
            child.AccessKey.IsNotNull();

            // 施設情報 重複除いて3件。細かい内容のテストはMasterMedicalFacilityListWorkerFixtureの方で行う
            results.FacilityN.Count.Is(3);

            // Emailが正しく取得されている
            results.AccountEmail.Is("hoge@abc.com");

            var keyList = new List<Guid>
            {
                _facilityKey1,
                _facilityKey2,
                _facilityKey3
            };

            // 施設情報は重複が除かれて取得された。
            _facilityRepo.Verify(m => m.ReadMedicalFacilityListByKey(DateTime.MinValue, keyList.ToArray()), Times.Once);
        }

        void SetUpValidMethods(QoAppMedicalNaviInitApiArgs args)
        {
            _accountRepo.Setup(m => m.GetAccountEmail(_accountKey)).Returns("hoge@abc.com");

            _accountRawDataList = new List<QH_ACCOUNTINDEX_DAT>
            {
                new QH_ACCOUNTINDEX_DAT
                {
                    ACCOUNTKEY = _accountKey,
                    FAMILYNAME = "v5bYZKwGnYbG1JFqFJvT0A==",
                    GIVENNAME = "vHRaRbAhjT8D+ghtCbRmrw==",
                    FAMILYKANANAME = "8ixM6XN2cpz0+DJFRjr4wg==",
                    GIVENKANANAME = "U0GXSKQR8kQp5F+Q3mhQHw==",
                    NICKNAME = "WK2fu8w0rLI6HW5O8pzmLQ==",
                    SEXTYPE = 1,
                    BIRTHDAY = new DateTime(1977,3,27),
                    PHOTOKEY = Guid.Parse("e9d61d42-db8c-4d4c-ad76-4fd929f3514f")                    
                },
                new QH_ACCOUNTINDEX_DAT
                {
                    ACCOUNTKEY = Guid.NewGuid(),
                    FAMILYNAME = "Ox+3yD+2iQgRfBCZQ+dwNA==",
                    GIVENNAME = "v/SKLjAW1fafEr1gvY3x8Q==",
                    FAMILYKANANAME = "8ixM6XN2cpz0+DJFRjr4wg==",
                    GIVENKANANAME = "4B4hCFooVqf74yxeNtxTew==",
                    NICKNAME = "",
                    SEXTYPE = 2,
                    BIRTHDAY = new DateTime(2000,6,7),
                    PHOTOKEY = Guid.Empty
                }
            };

            _familyRepo.Setup(m => m.ReadFamilyList(_accountKey)).Returns(_accountRawDataList);

            var cardList1 = new List<QH_PATIENTCARD_FACILITY_VIEW>
            {
                new QH_PATIENTCARD_FACILITY_VIEW
                {
                    ACCOUNTKEY = _accountKey,
                    ADDRESS1 = "大阪府大阪市東淀川区",
                    ADDRESS2 = "下新庄1-3-5",
                    CARDCODE = 27004,
                    CARDNO = "99999001",
                    CITYNO = 0,
                    CREATEDDATE = DateTime.Now,
                    CustomCodeFormat = "{MedicalCode:10}{CardNo:8}",
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
                    CARDNO = "99999002",
                    CITYNO = 0,
                    CREATEDDATE = DateTime.Now,
                    CustomCodeFormat = "{MedicalCode:10}{CardNo:8}",
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
            var cardList2 = new List<QH_PATIENTCARD_FACILITY_VIEW>
            {
                new QH_PATIENTCARD_FACILITY_VIEW
                {
                    ACCOUNTKEY = _accountRawDataList[1].ACCOUNTKEY,
                    ADDRESS1 = "大阪府大阪市東淀川区",
                    ADDRESS2 = "下新庄1-3-5",
                    CARDCODE = 27004,
                    CARDNO = "99999001",
                    CITYNO = 0,
                    CREATEDDATE = DateTime.Now,
                    CustomCodeFormat = "{MedicalCode:10}{CardNo:8}",
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
                    CARDCODE = 27004,
                    CARDNO = "99999001",
                    CITYNO = 0,
                    CREATEDDATE = DateTime.Now,
                    CustomCodeFormat = "{MedicalCode:10}{CardNo:8}",
                    FACILITYKEY = _facilityKey3,
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
            };

            _cardRepo.Setup(m => m.ReadPatientCardList(_accountKey, 0,int.MinValue)).Returns(cardList1);
            _cardRepo.Setup(m => m.ReadPatientCardList(_accountRawDataList[1].ACCOUNTKEY, 0,int.MinValue)).Returns(cardList2);

            var keyList = new List<Guid>
            {
                _facilityKey1,
                _facilityKey2,
                _facilityKey3
            };

            _facilityRepo.Setup(m => m.ReadMedicalFacilityListByKey(DateTime.MinValue, keyList.ToArray())).Returns(GetFacilityList());
        }

        QoAppMedicalNaviInitApiArgs GetValidArgs()
        {
            return new QoAppMedicalNaviInitApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _executor.ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{QsApiSystemTypeEnum.TisiOSApp}",
            };
        }

        List<QH_FACILITY_ALL_VIEW> GetFacilityList()
        {
            var assembly = GetType().Assembly;
            var file = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains("MedicalFacilityList.json"));
            var stream = GetType().Assembly.GetManifestResourceStream(file);
            using (var reader = new System.IO.StreamReader(stream))
            {
                var json = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<List<QH_FACILITY_ALL_VIEW>>(json);
            }
        }
    }
}

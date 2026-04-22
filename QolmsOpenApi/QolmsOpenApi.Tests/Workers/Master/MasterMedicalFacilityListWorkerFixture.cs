using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class MasterMedicalFacilityListWorkerFixture
    {
        MasterMedicalFacilityListWorker _worker;
        Mock<IFacilityRepository> _facilityRepo;
        Guid _accountKey = Guid.NewGuid();
        Guid _executor = Guid.NewGuid();
        Guid _facility1 = Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB");
        Guid _facility2 = Guid.Parse("0544A136-84FB-4AA8-80BF-7EA48B93B50F");
        DateTime _targetDate = new DateTime(2020, 1, 1);

        [TestInitialize]
        public void Initialize()
        {
            _facilityRepo = new Mock<IFacilityRepository>();
            _worker = new MasterMedicalFacilityListWorker(_facilityRepo.Object);
        }

        [TestMethod]
        public void 連携システム番号と施設キーが両方未指定なら空リストを返す()
        {
            var args = GetValidArgs();
            args.LinkageSystemNo = "";
            args.FacilityKeyReferenceList = new List<string>();

            var results = _worker.Read(args);

            // エラーとはならず正常終了とする(AppMedicalNaviInitWorkerなどから呼び出される場合を考慮）
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.FacilityN.Count.Is(0);
        }

        [TestMethod]
        public void DBからのデータ取得失敗でエラー()
        {
            var args = GetValidArgs();

            _facilityRepo.Setup(m => m.ReadMedicalFacilityListByLinkageSystemNo(27003, _targetDate)).Throws(new Exception("hoge"));

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("DBからの医療機関情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 連携システム番号を指定して正常に取得()
        {
            var args = GetValidArgs();
            var facilityList = GetFacilityList();

            _facilityRepo.Setup(m => m.ReadMedicalFacilityListByLinkageSystemNo(27003, _targetDate)).Returns(facilityList);

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 内部のDateTime.Nowはとれないので分まで一致していたら良しとする
            results.ReadDate.Contains(DateTime.Now.ToApiDateString().Substring(0, 17)).IsTrue();

            // 3件正しく取得
            results.FacilityN.Count.Is(3);

            // 1件目のデータ一致
            var facility1 = results.FacilityN[0];
            var origin1 = facilityList[0];
            facility1.FacilityKeyReference.Is(origin1.FACILITYKEY.ToEncrypedReference());
            facility1.Name.Is(origin1.FACILITYNAME);
            facility1.NameKana.Is(origin1.FACILITYKANANAME);
            facility1.PostalCode.Is(origin1.POSTALCODE);
            facility1.Address1.Is(origin1.ADDRESS1);
            facility1.Address2.Is(origin1.ADDRESS2);
            facility1.Location.Latitude.Is(origin1.LATITUDE.ToString());
            facility1.Location.Longitude.Is(origin1.LONGITUDE.ToString());
            facility1.MedicalFacilityCode.Is(origin1.MEDICALFACILITYCODE);
            // サムネイルも無し
            facility1.ThumbnailKey.FileKeyReference.Is("");
            facility1.ThumbnailKey.Sequence.Is("");
            // ファイル、診療科、電話リスト、URLリストは無し
            facility1.FileKeyN.Count.Is(0);
            facility1.MedicalDepartmentN.Count.Is(0);
            facility1.PhoneN.Count.Is(0);
            facility1.UrlN.Count.Is(0);

            // 2件目のデータ一致
            var facility2 = results.FacilityN[1];
            var origin2 = facilityList[1];
            facility2.FacilityKeyReference.Is(origin2.FACILITYKEY.ToEncrypedReference());
            facility2.Name.Is(origin2.FACILITYNAME);
            facility2.NameKana.Is(origin2.FACILITYKANANAME);
            facility2.PostalCode.Is(origin2.POSTALCODE);
            facility2.Address1.Is(origin2.ADDRESS1);
            facility2.Address2.Is(origin2.ADDRESS2);
            facility2.Location.Latitude.Is(origin2.LATITUDE.ToString());
            facility2.Location.Longitude.Is(origin2.LONGITUDE.ToString());
            facility2.MedicalFacilityCode.Is(origin2.MEDICALFACILITYCODE);
            facility2.MedicalDepartmentN.Count.Is(origin2.DepertmentList.Count);
            for(var i = 0; i < origin2.DepertmentList.Count; i++)
            {
                var act = facility2.MedicalDepartmentN[i];
                // 並べ替えデータと比較（並べ替えが正しく行われた証拠）
                var exp = origin2.DepertmentList.OrderBy(x => x.DispOrder).ElementAt(i);
                act.DepartmentNo.Is(exp.DepartmentNo.ToString());
                act.DepartmentName.Is(exp.DepartmentName);
                act.LocalCode.Is(exp.LocalCode);
                act.LocalName.Is(exp.LocalName);
            }

            facility2.PhoneN.Count.Is(origin2.ContactList.Count);
            for(var i = 0; i < origin2.ContactList.Count; i++)
            {
                var act = facility2.PhoneN[i];
                var exp = origin2.ContactList.OrderBy(x => x.DispOrder).ElementAt(i);
                act.PhoneNo.Is(exp.TelFull);
                act.ContactInformationTypeNo.Is(exp.ContactInformationType.ToString());
                act.Comment.Is(exp.Comment.Comment);
                act.Title.Is(exp.Comment.Title);

                act.ReceptionTimeN.Count.Is(exp.Comment.ReceptionTimeN.Count);
                for(var j = 0; j < exp.Comment.ReceptionTimeN.Count; j++)
                {
                    var timeAct = act.ReceptionTimeN[j];
                    var timeExp = exp.Comment.ReceptionTimeN[j];
                    timeAct.Tag.Is(timeExp.Tag);
                    timeAct.TimeText.Is(timeExp.TimeText);
                }   
            }

            facility2.UrlN.Count.Is(origin2.UrlList.Count);
            for (var i = 0; i < origin2.UrlList.Count; i++)
            {
                var act = facility2.UrlN[i];
                var exp = origin2.UrlList.OrderBy(x => x.DispOrder).ElementAt(i);
                act.Uri.Is(exp.Url);
                act.UriTypeNo.Is(exp.UrlType.ToString());
            }

            facility2.FileKeyN.Count.Is(origin2.FileList.Count);
            for (var i = 0; i < origin2.FileList.Count; i++)
            {
                var act = facility2.FileKeyN[i];
                var exp = origin2.FileList.OrderBy(x => x.Sequence).ElementAt(i);
                act.Sequence.Is(exp.Sequence.ToString());
                act.FileKeyReference.Is(exp.FileKey.ToEncrypedReference());
            }

            facility2.ThumbnailKey.FileKeyReference.Is(facility2.FileKeyN[0].FileKeyReference);
            facility2.ThumbnailKey.Sequence.Is(facility2.FileKeyN[0].Sequence);

            // 3件目のデータ一致
            var facility3 = results.FacilityN[2];
            var origin3 = facilityList[2];
            facility3.FacilityKeyReference.Is(origin3.FACILITYKEY.ToEncrypedReference());
            facility3.Name.Is(origin3.FACILITYNAME);
            facility3.NameKana.Is(origin3.FACILITYKANANAME);
            facility3.PostalCode.Is(origin3.POSTALCODE);
            facility3.Address1.Is(origin3.ADDRESS1);
            facility3.Address2.Is(origin3.ADDRESS2);
            facility3.Location.Latitude.Is(origin3.LATITUDE.ToString());
            facility3.Location.Longitude.Is(origin3.LONGITUDE.ToString());
            facility3.MedicalFacilityCode.Is(origin3.MEDICALFACILITYCODE);
            facility3.MedicalDepartmentN.Count.Is(origin3.DepertmentList.Count);
            for (var i = 0; i < origin3.DepertmentList.Count; i++)
            {
                var act = facility3.MedicalDepartmentN[i];
                // 並べ替えデータと比較（並べ替えが正しく行われた証拠）
                var exp = origin3.DepertmentList.OrderBy(x => x.DispOrder).ElementAt(i);
                act.DepartmentNo.Is(exp.DepartmentNo.ToString());
                act.DepartmentName.Is(exp.DepartmentName);
                act.LocalCode.Is(exp.LocalCode);
                act.LocalName.Is(exp.LocalName);
            }

            facility3.PhoneN.Count.Is(origin3.ContactList.Count);
            for (var i = 0; i < origin3.ContactList.Count; i++)
            {
                var act = facility3.PhoneN[i];
                var exp = origin3.ContactList.OrderBy(x => x.DispOrder).ElementAt(i);
                act.PhoneNo.Is(exp.TelFull);
                act.ContactInformationTypeNo.Is(exp.ContactInformationType.ToString());
                act.Comment.Is(exp.Comment.Comment);
                act.Title.Is(exp.Comment.Title);

                act.ReceptionTimeN.Count.Is(exp.Comment.ReceptionTimeN.Count);
                for (var j = 0; j < exp.Comment.ReceptionTimeN.Count; j++)
                {
                    var timeAct = act.ReceptionTimeN[j];
                    var timeExp = exp.Comment.ReceptionTimeN[j];
                    timeAct.Tag.Is(timeExp.Tag);
                    timeAct.TimeText.Is(timeExp.TimeText);
                }
            }

            facility3.UrlN.Count.Is(origin3.UrlList.Count);
            for (var i = 0; i < origin3.UrlList.Count; i++)
            {
                var act = facility3.UrlN[i];
                var exp = origin3.UrlList.OrderBy(x => x.DispOrder).ElementAt(i);
                act.Uri.Is(exp.Url);
                act.UriTypeNo.Is(exp.UrlType.ToString());
            }

            facility3.FileKeyN.Count.Is(origin3.FileList.Count);
            for (var i = 0; i < origin3.FileList.Count; i++)
            {
                var act = facility3.FileKeyN[i];
                var exp = origin3.FileList.OrderBy(x => x.Sequence).ElementAt(i);
                act.Sequence.Is(exp.Sequence.ToString());
                act.FileKeyReference.Is(exp.FileKey.ToEncrypedReference());
            }

            facility3.ThumbnailKey.FileKeyReference.Is(facility3.FileKeyN[0].FileKeyReference);
            facility3.ThumbnailKey.Sequence.Is(facility3.FileKeyN[0].Sequence);
        }

        [TestMethod]
        public void 施設キーを指定して正常終了()
        {
            var args = GetValidArgs();
            args.LinkageSystemNo = "";
            var facilityList = GetFacilityList();

            _facilityRepo.Setup(m => m.ReadMedicalFacilityListByKey(_targetDate,_facility1,_facility2)).Returns(facilityList);

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 内部のDateTime.Nowはとれないので分まで一致していたら良しとする
            results.ReadDate.Contains(DateTime.Now.ToApiDateString().Substring(0, 17)).IsTrue();

            _facilityRepo.Verify(m => m.ReadMedicalFacilityListByKey(_targetDate, _facility1, _facility2), Times.Once);
        }

        QoMasterMedicalFacilityListReadApiArgs GetValidArgs()
        {
            return new QoMasterMedicalFacilityListReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _executor.ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{QsApiSystemTypeEnum.TisiOSApp}",
                LinkageSystemNo = "27003",
                FacilityKeyReferenceList = new List<string>
                {
                    _facility1.ToEncrypedReference(),
                    _facility2.ToEncrypedReference()
                },
                UpdatedDate = _targetDate.ToApiDateString(),
            };
        }

        List<QH_FACILITY_ALL_VIEW> GetFacilityList()
        {
            var assembly = GetType().Assembly;
            var file = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains("MedicalFacilityList.json"));
            var stream = GetType().Assembly.GetManifestResourceStream(file);
            using(var reader = new System.IO.StreamReader(stream))
            {
                var json = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<List<QH_FACILITY_ALL_VIEW>>(json);
            }
        }
    }
}

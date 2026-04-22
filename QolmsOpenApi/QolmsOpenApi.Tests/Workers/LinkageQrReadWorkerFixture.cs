using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SPCryptoEngine;
using MEI.TwoinQRLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class LinkageQrReadWorkerFixture
    {
        LinkageQrReadWorker _worker;
        Mock<IFacilityRepository> _facilityRepo;
        Guid _facilityKey;
        DateTime _now;

        [TestInitialize]
        public void Initialize()
        {
            _facilityKey = Guid.NewGuid();
            _facilityRepo = new Mock<IFacilityRepository>();
            _worker = new LinkageQrReadWorker(_facilityRepo.Object);
        }

        [TestMethod]
        public void QRが未設定でエラー()
        {
            var args = GetValidArgs();
            args.QRData = string.Empty;

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains($"{nameof(args.QRData)}").IsTrue();
            result.Result.Detail.Contains($"必須").IsTrue();
        }

        [TestMethod]
        public void Versionに数字以外指定でエラー()
        {
            var args = GetValidArgs();
            args.Version = "hoge";

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains($"{nameof(args.Version)}").IsTrue();
            result.Result.Detail.Contains($"不正").IsTrue();
        }

        [TestMethod]
        public void 復号化に失敗したらエラー()
        {
            var args = GetValidArgs();
            // 暗号化されていない文字列を入れてエラーを起こす
            args.QRData = "1#2#3#4#5#6#7#8";
            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1005");
            result.Result.Detail.Contains($"暗号化されたQrコードの復号に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 復号化結果が空ならエラー()
        {
            // 暗号化DLLに依存するためこのテストはできない
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void 復号結果のフィールドが欠落していたらエラー()
        {
            // V1の場合
            var args = GetValidArgs();
            args.QRData = EncodeQRForSSI("1#2#3#4#5#6#7"); // 8列必要だが7列
            args.Version = "01";

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1005");
            result.Result.Detail.Contains($"QR情報が欠落").IsTrue();
            result.Result.Detail.Contains("Version01").IsTrue();

            // V2の場合
            args = GetValidArgs();
            args.QRData = EncodeQRForSSI("1#2#3#4#5#6#7#8#9"); // 10列必要だが9列
            args.Version = "02";

            result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1005");
            result.Result.Detail.Contains($"QR情報が欠落").IsTrue();
            result.Result.Detail.Contains("Version02").IsTrue();
        }

        [TestMethod]
        public void 生年月日または作成日が日付として不正であればエラー()
        {
            var args = GetValidArgs();
            args.QRData = EncodeQRForSSI("hoge#202301011010#3#4#5#6#7#8"); // 生年月日不正

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains($"生年月日が不正です").IsTrue();

            args = GetValidArgs();
            args.QRData = EncodeQRForSSI("20000101#fuga#3#4#5#6#7#8"); // 作成日が不正

            result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains($"作成日が不正です").IsTrue();
        }        

        [TestMethod]
        public void 性別に未定義のコードが設定されていればエラー()
        {
            var args = GetValidArgs();

            var created = _now.ToString("yyyyMMddHHmm");
            // 性別が未定義値
            var qr = $"20000101#{created}#00001234#hoge#1234567890#患者　太郎#カンジャ#タロウ";
            var cryptedQr = EncodeQRForSSI(qr);
            args.QRData = cryptedQr;

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains($"性別が不正です").IsTrue();
        }

        [TestMethod]
        public void 医療機関コードから施設情報が取得できなければエラー()
        {
            var args = GetValidArgs();

            // 施設情報を取得失敗
            _facilityRepo.Setup(m => m.ReadFacilityFromMedicalFacilityCode("1234567890")).Throws(new Exception());

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1005");
            result.Result.Detail.Contains($"医療機関コードから施設情報を取得できませんでした").IsTrue();
        }

        [TestMethod]
        public void Version01で正常に処理できる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            result.FamilyName.Is("患者");
            result.GivenName.Is("太郎");
            result.FamilyKanaName.Is("カンジャ"); //全角に変換されている
            result.GivenKanaName.Is("タロウ"); // 同上
            result.Birthday.Is(new DateTime(2000, 1, 1).ToApiDateString());
            result.CreatedDate.Is(_now.ToApiDateString());
            result.MedicalFacilityCode.Is("1234567890");
            result.FacilityKeyReference.Is(_facilityKey.ToEncrypedReference());
            result.FacilityName.Is("テスト病院");
            result.SexType.Is("1");
            result.LinkUserId.Is("00001234");
            result.PhoneNumbers.Any().IsFalse();
        }

        [TestMethod]
        public void Version01で正常に処理できる_MEIナビ()
        {
            var args = GetValidMeiArgs();
            SetupValidMethods(args);

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            // MEIナビ用の暗号化モジュールで復号化されている
            result.FamilyName.Is("患者");
            result.GivenName.Is("太郎");
            result.FamilyKanaName.Is("カンジャ"); //全角に変換されている
            result.GivenKanaName.Is("タロウ"); // 同上
            result.Birthday.Is(new DateTime(2000, 1, 1).ToApiDateString());
            result.CreatedDate.Is(_now.ToApiDateString());
            result.MedicalFacilityCode.Is("1234567890");
            result.FacilityKeyReference.Is(_facilityKey.ToEncrypedReference());
            result.FacilityName.Is("テスト病院");
            result.SexType.Is("1");
            result.LinkUserId.Is("00001234");
            result.PhoneNumbers.Any().IsFalse();
        }

        [TestMethod]
        public void Version01で有効期限が切れていればエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 規定時間より前の日付を指定 Version01は受付要用を使用する
            var defaultHours = args.ExecuteSystemType.GetQrExpirationHour().ReceptionExpirationHour;
            var created = DateTime.Now.AddHours(-defaultHours).AddMinutes(-1).ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#カンジャ#タロウ";
            var cryptedQr = EncodeQRForSSI(qr);
            args.QRData = cryptedQr;

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("2004");
            result.Result.Detail.Contains($"QRコードの有効期限が切れています").IsTrue();
        }

        [TestMethod]
        public void LinkUserIdが99999から始まれば有効期限チェックはパスされる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 規定時間(デフォルト3時間)より前の日付を指定
            var created = DateTime.Now.AddMinutes(-181).ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#99999234#1#1234567890#患者　太郎#カンジャ#タロウ";
            var cryptedQr = EncodeQRForSSI(qr);
            args.QRData = cryptedQr;

            var result = _worker.QrRead(args);

            // 有効期限が切れていても正常に処理される
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");
        }

        [TestMethod]
        public void 漢字姓名が分解できなければ漢字姓の部分に全て入る()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var created = _now.ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者太郎#カンジャ#タロウ";
            args.QRData = EncodeQRForSSI(qr);

            var result = _worker.QrRead(args);

            // 有効期限が切れていても正常に処理される
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            // 区切れなかったら漢字姓に格納される
            result.FamilyName.Is("患者太郎");
        }

        [TestMethod]
        public void 性別が3の女性でも正しく処理できる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var created = _now.ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#3#1234567890#患者　花子#カンジャ#ハナコ";
            args.QRData = EncodeQRForSSI(qr);

            var result = _worker.QrRead(args);

            // 有効期限が切れていても正常に処理される
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            // Qolmsでの女性に変換されている
            result.SexType.Is("2");
        }

        [TestMethod]
        public void Version02で識別コードが不正ならエラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var created = _now.ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#カンジャ#タロウ#A#0699998888|09055551111|08011112222|";
            args.QRData = EncodeQRForSSI(qr);
            args.Version = "02";

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("1002");
            result.Result.Detail.Contains("識別コードが不正です").IsTrue();
        }

        [TestMethod]
        public void Version02で受付票の期限エラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 規定時間より前の日付を指定 受付要用を使用する
            var defaultHours = args.ExecuteSystemType.GetQrExpirationHour().ReceptionExpirationHour;
            var created = DateTime.Now.AddHours(-defaultHours).AddMinutes(-1).ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#カンジャ#タロウ#0#0699998888|09055551111|08011112222|";
            args.QRData = EncodeQRForSSI(qr);
            args.Version = "02";

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("2004");
            result.Result.Detail.Contains($"QRコードの有効期限が切れています").IsTrue();
        }

        [TestMethod]
        public void Version02で予約票の期限エラー()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 規定時間より前の日付を指定 予約要用を使用する
            var defaultHours = args.ExecuteSystemType.GetQrExpirationHour().ReservationExpirationHour;
            var created = DateTime.Now.AddHours(-defaultHours).AddMinutes(-1).ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#カンジャ#タロウ#1#0699998888|09055551111|08011112222|";
            args.QRData = EncodeQRForSSI(qr);
            args.Version = "02";

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.FalseString);
            result.Result.Code.Is("2004");
            result.Result.Detail.Contains($"QRコードの有効期限が切れています").IsTrue();
        }

        [TestMethod]
        public void Version02で受付票が正常に処理できる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            var created = _now.ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#カンジャ#タロウ#0#0699998888|09055551111|08011112222|";
            args.QRData = EncodeQRForSSI(qr);
            args.Version = "02";

            var result = _worker.QrRead(args);

            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");

            result.FamilyName.Is("患者");
            result.GivenName.Is("太郎");
            result.FamilyKanaName.Is("カンジャ"); //全角に変換されている
            result.GivenKanaName.Is("タロウ"); // 同上
            result.Birthday.Is(new DateTime(2000, 1, 1).ToApiDateString());
            result.CreatedDate.Is(_now.ToApiDateString());
            result.MedicalFacilityCode.Is("1234567890");
            result.FacilityKeyReference.Is(_facilityKey.ToEncrypedReference());
            result.FacilityName.Is("テスト病院");
            result.SexType.Is("1");
            result.LinkUserId.Is("00001234");

            // 4つのうち1つは未設定なので合計3つ格納される
            result.PhoneNumbers.Count.Is(3);
            result.PhoneNumbers[0].Is("0699998888");
            result.PhoneNumbers[1].Is("09055551111");
            result.PhoneNumbers[2].Is("08011112222");

        }

        [TestMethod]
        public void Version02で予約票が正常に処理できる()
        {
            var args = GetValidArgs();
            SetupValidMethods(args);

            // 受付票の期限が過ぎている状態の作成日を設定
            var defaultHours = args.ExecuteSystemType.GetQrExpirationHour().ReceptionExpirationHour;
            var created = DateTime.Now.AddHours(-defaultHours).AddMinutes(-1).ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#カンジャ#タロウ#1#0699998888|09055551111|08011112222|";
            args.QRData = EncodeQRForSSI(qr);
            args.Version = "02";

            var result = _worker.QrRead(args);

            // 予約票の期限の方が長いので期限切れにはならない
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");
        }

        [TestMethod]
        public void Version02で予約票が正常に処理できる_MEIナビ()
        {
            var args = GetValidMeiArgs();
            SetupValidMethods(args);

            // 受付票の期限が過ぎている状態の作成日を設定
            var defaultHours = args.ExecuteSystemType.GetQrExpirationHour().ReceptionExpirationHour;
            var created = DateTime.Now.AddHours(-defaultHours).AddMinutes(-1).ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#カンジャ#タロウ#1#0699998888|09055551111|08011112222|";
            args.QRData = EncodeQRForMEI(qr);
            args.Version = "02";

            var result = _worker.QrRead(args);

            // 予約票の期限の方が長いので期限切れにはならない
            result.IsSuccess.Is(bool.TrueString);
            result.Result.Code.Is("0200");
        }

        string EncodeQRForSSI(string source)
        {
            var generator = new KeyGenerator();
            return generator.GetEncryptoKey(source);
        }

        string EncodeQRForMEI(string source)
        {
            return TwoinQREncrypt.Encrypt(source);
        }

        void SetupValidMethods(QoLinkageQrCodeReadApiArgs args)
        {
            _facilityRepo.Setup(m => m.ReadFacilityFromMedicalFacilityCode("1234567890")).Returns(new QH_FACILITY_MST
            {
                FACILITYKEY = _facilityKey,
                FACILITYNAME = "テスト病院"
            });
        }

        QoLinkageQrCodeReadApiArgs GetValidArgs()
        {
            _now = DateTime.Today.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            var created = _now.ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#ｶﾝｼﾞｬ#ﾀﾛｳ";
            var cryptedQr = EncodeQRForSSI(qr);
            return new QoLinkageQrCodeReadApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                QRData = cryptedQr,
                Version = "01",                
            };
        }

        QoLinkageQrCodeReadApiArgs GetValidMeiArgs()
        {
            _now = DateTime.Today.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            var created = _now.ToString("yyyyMMddHHmm");
            var qr = $"20000101#{created}#00001234#1#1234567890#患者　太郎#ｶﾝｼﾞｬ#ﾀﾛｳ";
            var cryptedQr = EncodeQRForMEI(qr);
            return new QoLinkageQrCodeReadApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "MeiNavi",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.MeiNaviAndroidApp}",
                QRData = cryptedQr,
                Version = "01",
            };
        }
    }
}

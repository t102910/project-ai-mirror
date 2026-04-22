using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class NoteMedicineReadWorkerFixture
    {
        Mock<INoteMedicineRepository> _repo;
        NoteMedicineReadWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<INoteMedicineRepository>();
            _worker = new NoteMedicineReadWorker(_repo.Object);
        }

        [TestMethod]
        public void DayRead_指定日未指定でエラー()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
            };

            var ret = _worker.DayRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
        }

        [TestMethod]
        public void DayRead_アカウントキーがEmptyでエラー()
        {
            var accountKey = Guid.Empty;

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                TargetDate = DateTime.Today.ToApiDateString()
            };

            var ret = _worker.DayRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
        }

        [TestMethod]
        public void DayRead_DataTypeが未定義値でエラー()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                TargetDate = DateTime.Today.ToApiDateString(),
                DataType = "999",
            };

            var ret = _worker.DayRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
        }

        [TestMethod]
        public void DayRead_DB処理例外でエラー()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                TargetDate = DateTime.Today.ToApiDateString(),
                DataType = "1",
            };

            _repo.Setup(m => m.ReadMedicineDayList(accountKey, DateTime.Today, It.IsAny<List<byte>>(), Guid.Empty, 0)).Throws(new Exception());

            var ret = _worker.DayRead(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003"); // DBエラー
        }

        [TestMethod]
        public void DayRead_該当データ無しでも正常とする()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                TargetDate = new DateTime(2021, 9, 2).ToApiDateString(),
                DataType = "1",
            };

            var entityN = GetEthicalEntityN();

            _repo.Setup(m => m.ReadMedicineDayList(accountKey, new DateTime(2021, 9, 2), It.IsAny<List<byte>>(), Guid.Empty, 0))
                .Returns(new List<QH_MEDICINE_DAT>());
                

            var ret = _worker.DayRead(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // データ無しなのでDataはnullとなっている
            ret.Data.IsNull();
        }

        [TestMethod]
        public void DayRead_調剤データを正常に取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                TargetDate = new DateTime(2021,9,2).ToApiDateString(),
                DataType = "1",
            };

            var entityN = GetEthicalEntityN();

            _repo.Setup(m => m.ReadMedicineDayList(accountKey, new DateTime(2021, 9, 2), It.IsAny<List<byte>>(), Guid.Empty, 0))
                .Returns(entityN)
                .Callback<Guid,DateTime,List<byte>,Guid,int>((a,b,dataTypes,d,e)=>
                {
                    dataTypes[0].Is<byte>(1);
                    dataTypes[1].Is<byte>(100); // SSMIXが自動で追加されている
                });

            var ret = _worker.DayRead(args);

            ret.IsSuccess.Is(bool.TrueString);
            // RecordDateが指定日
            ret.Data.RecordDate.Is(new DateTime(2021, 9, 2).ToApiDateString());
            // 2件取得
            ret.Data.MedicineSetN.Count.Is(2);

            // レコードごとに正しく変換されている（中身の検証は省略）
            var rec1 = ret.Data.MedicineSetN[0];
            rec1.DataType.Is("1");
            rec1.OwnerType.Is("2");
            rec1.DataIdReference.Is(entityN[0].RECEIPTNO.ToEncrypedReference());
            rec1.ConvertedMedicineSet.IsNot("");
            rec1.OtcMedicineSet.RecordDate.Is("");
            rec1.OtcPhotoMedicineSet.RecordDate.Is("");

            var rec2 = ret.Data.MedicineSetN[1];
            rec2.DataType.Is("1");
            rec2.OwnerType.Is("2");
            rec2.DataIdReference.Is(entityN[1].RECEIPTNO.ToEncrypedReference());
            rec2.ConvertedMedicineSet.IsNot("");
            rec2.OtcMedicineSet.RecordDate.Is("");
            rec2.OtcPhotoMedicineSet.RecordDate.Is("");
        }

        [TestMethod]
        public void DayRead_市販薬データを正常に取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                TargetDate = new DateTime(2022, 6, 17).ToApiDateString(),
                DataType = "2",
            };

            var entityN = GetOtcEntityN();

            _repo.Setup(m => m.ReadMedicineDayList(accountKey, new DateTime(2022, 6, 17), It.IsAny<List<byte>>(), Guid.Empty, 0))
                .Returns(entityN)
                .Callback<Guid, DateTime, List<byte>, Guid, int>((a, b, dataTypes, d, e) =>
                {
                    dataTypes[0].Is<byte>(2);
                    dataTypes[1].Is<byte>(4); // 市販薬写真が自動で追加されている
                });

            var ret = _worker.DayRead(args);

            ret.IsSuccess.Is(bool.TrueString);
            // RecordDateが指定日
            ret.Data.RecordDate.Is(new DateTime(2022, 6, 17).ToApiDateString());
            // 1件取得
            ret.Data.MedicineSetN.Count.Is(1);

            // データが正しく変換されている(中身の検証は省略）
            var rec1 = ret.Data.MedicineSetN[0];
            rec1.DataType.Is("2");
            rec1.OwnerType.Is("1");
            rec1.DataIdReference.Is(entityN[0].RECEIPTNO.ToEncrypedReference());
            rec1.ConvertedMedicineSet.IsNot("");
            rec1.OtcMedicineSet.RecordDate.Is(new DateTime(2022, 6, 17).ToApiDateString());
            rec1.OtcPhotoMedicineSet.RecordDate.Is("");
        }

        List<QH_MEDICINE_DAT> GetEthicalEntityN()
        {
            return new List<QH_MEDICINE_DAT>
            {
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2021,9,2),
                    SEQUENCE = 1,
                    DATATYPE = 1,
                    OWNERTYPE = 2,
                    RECEIPTNO = "20210901000001",
                    CONVERTEDMEDICINESET = "9FV9wnJn44UQkdrhASYLuF499VBzI7T3++jiO3UaoZpf2Zf34QW82cgNm0c7lHADYLBcWcFx0GANJQ8yhk1VdcVYFCEOMFHPyJnWW1NgOSLXV6hOkd8KrJdlWwAEeNW68eLdeFRcG9w1vwyvApFWa6Sq4e9qU83nRE9SbVM8pDzQyTU7u2NCZXboplRDZzCmQofB2nQixV1yVPos/PZOGKL5N0hObCjIDtUGFwyOAnIf6BtVuk2rirzF+EOSZF8AyEYFBnykPjrvVaD6VjYOaz48uW4drfoYITdwiirAr4LYlFUeNOYwuncoF4YAs5QPqEMmVT+VnWMPwCvPEGcq7lGLd+eWvgxAo127EFtiPHTjKKFOTtND437ibpgB2wZQ2NUDVrUpLc+z+ozSEEBWhvr3xFtdsjwLktxnqV9GykVTWDjtUvuCDzZ56YpFygsVozoHuZlsyg0iOt7vKzPZAGMnCiogQYSOGQnBrjjJ7j+nmXX4oK3N+zIDQDnnCZP3I693w9D0pC9sbI0ebUzU/qZi8cg+U0lPMZcPThN1SquA7CLfTma42X2iNuCHog+qJVUG1i+HUe52p2LugOUhImcmewlOxpbeN1GU7ezYIHPWjgytviDwnwdtK0tz6hxu3w9ZPC9Zt2xSVq6XCazEpf53pKvNSVSsR07PXAkYVSQ297VFzH6kpoLbJYK4uwFekEl1edVfQI0JdlYKuOPUHl8kLPqvUTPfSIUHeTw/QS1Cx6UudO+oLXKWRbXKmsVVubpw4+Avdq7V0yRtrmA3P2mSU3r+bUyg7NLQotSfTE9o9jmDQME7vKD34AzKR+RZa6SCBcLhdj8OuRuYWAJvLGKKRFKxqWPv9AGqpUbXQ+xEdKIJsD+SckfaZyWcd9OcKNekpYWNt+Ghvu5aPbPR04+nt4UZGgCUw/EadOsEPVY8iealtfecJFrNf9lEkho6C14thCqkKcIwhmnfLMwG8Hm948TrjRTumHEmJjZvmwjDly58Q0NWkQjrfaYjrJjm//raJbr/jy6qFznwBBqWHiwjeo4whMBH1ywXbVdtUtmy0auJML0uaIPb4E3yfFgDfIg/f2ZSxEimHF7LkFlqtHZlolAOKDoLKkrF3k7B6CNCUo3Y2lP7S3u0AlUKjqAC/dFYrx8klt2rTHp1qsbucNt7UBWFSnztAm7zoR5a2SEPspQSgyHM9so586JUAR4CuyBJGTfB4Lbh0bQOP5P0NSBjFifnWg2a4It2E9exmpCg+sUNpdLK1YqOiwGOlIFdyX6VHs3GnVLo4qwgAGK9O4xzdTTqZavl9Ne3JFheO8341hT0lMmIAMExTYfu+0xxUZQuNuJzzZk80ayDKm/ZctkYbRazWj+Ye/Ep60ZD6Myne3qxYQu9EpKFG1yz77yhR65+E1UfSX7uhC/VytJAlHhbFyrEepYs9irdK/RhT/gMBXcPdiZSZMOQIgLd/Yk7q6Kg0juuIwsX2sHAAurUtjBBp7ZdZnInUhV8nqUaOyQD+y4BzbfBEw3FWt/ett9N4FL+4aV+FbRemgszMTcEeno7CX4QcFgaLTyqC9+Y3+ZTQA3FBkF/S2eEZRngCFwv7qq4Xz6x4UUYFPTeLre7gjioPbdiDfuipNpNIdYNXm1Mq1sZ1IwRulno51er6p71Xb4f6U8bEuqyZ76lhfV6nAMdf1LfENZQiEIG3XrPGKg=",
                    MEDICINESET = "UhG4vqHCZNFWPMWOQza2HB5NKfDB0fjwTDkl4jcWCuJ02S7BHwpz/hFsQvaXog7adSagl4jKihilXFdIPZldk01S6P7X7K6kZPPseiwcE4A7yPuItH8+my3qcQdIe4HNVfQjweIvAAjANxWrryAY0fpHoIq5hERajSkgcuERtD6DaQGQtJ5hU10DrhzpKio2Ov8DQrLW49+vobmCa7dFV4JUpGT1sMRe9hWFaxRwuGHo11eA2wh6WnTbxGsGzAeQPo6Bdhna1bMN/ER2Ji7iA3PveNfi3i3auCNbmdbTrEXZ7jwXTuStd8jGxdkqzrLlzop/Y9STtfkOufZZiH5koGVMvNFyi78OauoBYL7dpfBLWQRNMAlWqUNfyDKhNk6wynRd9fVgwAYn4c145nWeJZExS7TEsdTBo9vJ/OAy9Gp4p0+T3ntQLt1M2+fhjch6Nve1tD70JuEODOAq6vdxxDCK9+gkuCTORtCv09AbB50UlRLpyUUck/gASj6PQl/mQpUjfTetDjSFXX7Y+hkhQeKQdQa9kcMIfz2G59UUK9jt3fT+rPnLnGfsmIN8l69Nl9mZz+WIrmrY8iBBAXK4DPgM8bf+k6pVx+rV+ZDfPPMxPEjpdNCPzVLnFHFV5T86HCGdARyt3vqJaAtTeb7PURBLGyq9sgcsQYzUPsLYp1/MMgLVayHAila/HM05LEIEu/M7/pH9rWqAovlKiIFpdUlpk/fccRCu0y+5hG0hr52NuC9kf7AAKRjYfc1lZZ3sQnsBUNYu6uY9aSQdQu77UEvLThd9GgHbeKvjBELJHJ47sZflLuXAnu+tcdmpdF2IwyZwvtZrz2bgbuaEaExxEi60M5/2TObL66u8RMGQKzWD818jppQcpGlSPORWKTtyY5sReKl6Lg7Bz3JTMrdIXluEOAxnQaZvSdrF4VIdqCLWUlMfPcyLKigWTlLxatwaNjD965Rodisyp6DWTk7VbCfEqmQR1Ygq2KvhhGnrjWfIXKI9WcsKuBxxuIcXDV2/",

                },
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2021,9,2),
                    SEQUENCE = 2,
                    DATATYPE = 1,
                    OWNERTYPE = 2,
                    RECEIPTNO = "20210901000002",
                    CONVERTEDMEDICINESET = "9FV9wnJn44UQkdrhASYLuF499VBzI7T3++jiO3UaoZpf2Zf34QW82cgNm0c7lHADYLBcWcFx0GANJQ8yhk1VdcVYFCEOMFHPyJnWW1NgOSLXV6hOkd8KrJdlWwAEeNW68eLdeFRcG9w1vwyvApFWa6Sq4e9qU83nRE9SbVM8pDzQyTU7u2NCZXboplRDZzCmQofB2nQixV1yVPos/PZOGKL5N0hObCjIDtUGFwyOAnIf6BtVuk2rirzF+EOSZF8AyEYFBnykPjrvVaD6VjYOaz48uW4drfoYITdwiirAr4LYlFUeNOYwuncoF4YAs5QPqEMmVT+VnWMPwCvPEGcq7lGLd+eWvgxAo127EFtiPHTjKKFOTtND437ibpgB2wZQ2NUDVrUpLc+z+ozSEEBWhvr3xFtdsjwLktxnqV9GykVTWDjtUvuCDzZ56YpFygsVozoHuZlsyg0iOt7vKzPZAGMnCiogQYSOGQnBrjjJ7j+nmXX4oK3N+zIDQDnnCZP3I693w9D0pC9sbI0ebUzU/qZi8cg+U0lPMZcPThN1SquA7CLfTma42X2iNuCHog+qJVUG1i+HUe52p2LugOUhImcmewlOxpbeN1GU7ezYIHPWjgytviDwnwdtK0tz6hxu3w9ZPC9Zt2xSVq6XCazEpf53pKvNSVSsR07PXAkYVSQ297VFzH6kpoLbJYK4uwFekEl1edVfQI0JdlYKuOPUHl8kLPqvUTPfSIUHeTw/QS1Cx6UudO+oLXKWRbXKmsVVubpw4+Avdq7V0yRtrmA3P2mSU3r+bUyg7NLQotSfTE9o9jmDQME7vKD34AzKR+RZa6SCBcLhdj8OuRuYWAJvLGKKRFKxqWPv9AGqpUbXQ+xEdKIJsD+SckfaZyWcd9OcKNekpYWNt+Ghvu5aPbPR04+nt4UZGgCUw/EadOsEPVY8iealtfecJFrNf9lEkho6C14thCqkKcIwhmnfLMwG8Hm948TrjRTumHEmJjZvmwjDly58Q0NWkQjrfaYjrJjm//raJbr/jy6qFznwBBqWHiwjeo4whMBH1ywXbVdtUtmy0auJML0uaIPb4E3yfFgDfIg/f2ZSxEimHF7LkFlqtHZlolAOKDoLKkrF3k7B6CNCUo3Y2lP7S3u0AlUKjqAC/dFYrx8klt2rTHp1qsbucNt7UBWFSnztAm7zoR5a2SEPspQSgyHM9so586JUAR4CuyBJGTfB4Lbh0bQOP5P0NSBjFifnWg2a4It2E9exmpCg+sUNpdLK1YqOiwGOlIFdyX6VHs3GnVLo4qwgAGK9O4xzdTTqZavl9Ne3JFheO8341hT0lMmIAMExTYfu+0xxUZQuNuJzzZk80ayDKm/ZctkYbRazWj+Ye/Ep60ZD6Myne3qxYQu9EpKFG1yz77yhR65+E1UfSX7uhC/VytJAlHhbFyrEepYs9irdK/RhT/gMBXcPdiZSZMOQIgLd/Yk7q6Kg0juuIwsX2sHAAurUtjBBp7ZdZnInUhV8nqUaOyQD+y4BzbfBEw3FWt/ett9N4FL+4aV+FbRemgszMTcEeno7CX4QcFgaLTyqC9+Y3+ZTQA3FBkF/S2eEZRngCFwv7qq4Xz6x4UUYFPTeLre7gjioPbdiDfuipNpNIdYNXm1Mq1sZ1IwRulno51er6p71Xb4f6U8bEuqyZ76lhfV6nAMdf1LfENZQiEIG3XrPGKg=",
                    MEDICINESET = "UhG4vqHCZNFWPMWOQza2HB5NKfDB0fjwTDkl4jcWCuJ02S7BHwpz/hFsQvaXog7adSagl4jKihilXFdIPZldk01S6P7X7K6kZPPseiwcE4A7yPuItH8+my3qcQdIe4HNVfQjweIvAAjANxWrryAY0fpHoIq5hERajSkgcuERtD6DaQGQtJ5hU10DrhzpKio2Ov8DQrLW49+vobmCa7dFV4JUpGT1sMRe9hWFaxRwuGHo11eA2wh6WnTbxGsGzAeQPo6Bdhna1bMN/ER2Ji7iA3PveNfi3i3auCNbmdbTrEXZ7jwXTuStd8jGxdkqzrLlzop/Y9STtfkOufZZiH5koGVMvNFyi78OauoBYL7dpfBLWQRNMAlWqUNfyDKhNk6wynRd9fVgwAYn4c145nWeJZExS7TEsdTBo9vJ/OAy9Gp4p0+T3ntQLt1M2+fhjch6Nve1tD70JuEODOAq6vdxxDCK9+gkuCTORtCv09AbB50UlRLpyUUck/gASj6PQl/mQpUjfTetDjSFXX7Y+hkhQeKQdQa9kcMIfz2G59UUK9jt3fT+rPnLnGfsmIN8l69Nl9mZz+WIrmrY8iBBAXK4DPgM8bf+k6pVx+rV+ZDfPPMxPEjpdNCPzVLnFHFV5T86HCGdARyt3vqJaAtTeb7PURBLGyq9sgcsQYzUPsLYp1/MMgLVayHAila/HM05LEIEu/M7/pH9rWqAovlKiIFpdUlpk/fccRCu0y+5hG0hr52NuC9kf7AAKRjYfc1lZZ3sQnsBUNYu6uY9aSQdQu77UEvLThd9GgHbeKvjBELJHJ47sZflLuXAnu+tcdmpdF2IwyZwvtZrz2bgbuaEaExxEi60M5/2TObL66u8RMGQKzWD818jppQcpGlSPORWKTtyY5sReKl6Lg7Bz3JTMrdIXluEOAxnQaZvSdrF4VIdqCLWUlMfPcyLKigWTlLxatwaNjD965Rodisyp6DWTk7VbCfEqmQR1Ygq2KvhhGnrjWfIXKI9WcsKuBxxuIcXDV2/",
                }
            };
        }

        List<QH_MEDICINE_DAT> GetOtcEntityN()
        {
            return new List<QH_MEDICINE_DAT>
            {
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2022,6,17),
                    SEQUENCE = 1,
                    DATATYPE = 2,
                    OWNERTYPE = 1,
                    RECEIPTNO = "20220617000001",
                    CONVERTEDMEDICINESET = "9FV9wnJn44UQkdrhASYLuFmYrimAfWJqEFoYrmZDJ1Oou/shn2625APBsOv1bqYPmfudnh7beJMAXQa2c8ekYvdNebUor+REG1nm8Ve6OyAtOsydAqOG9dY/kZmSKJMkymny7/DNT/+onKCUwLXGye3Xtvjr+a8Z258d+Z3OI6lrcFHDeuWg7bieYt29Do15srCE0Y8ByI89xaFDi+moljAaczvQ572R2WxR39WZLYKDLJ3NWs1TENU5kUtoeRH9mNq0Wxyl5Ttd4ueZd+zHEOaifamYPm6p8K3Zv/7jrKmGBgKZ8Iv1fYe3WbeGhcXMSQ/drINclCMbF/m05qAonsBYpbrRigVa8OqAkjzlyVg4ahetBlYhgl5uJBq5Ylhfxyg4RNPDdCDYxH9qNHA7hnCh7u3YocXFZNZaLVbVVCiXkkvZQDrNfY1VxslTsAQXCnMbsrCd693CGiM+28jPdiGKq0Ztb809ETkRA61GvcooCmu7TElETR8m23vgfT40HuoM0llGc/QiJuIZ/RmqQNprI0/bIbHKtWGFCZyGmq4METCtTpekPZ4Tit4VJBH/JGw81L7iUF8VIcrFBJBY+BaABBvZ/TP07i6cs5O/JD0fpkfwZ6dFACFMkNmULIhnpZaGwE2nMfM+WLF+r7jBVF3OvAhDV/pal2ybyGoGOmffMpoZCgVo1iIS7wlzCsGDomrwGjlEHvhSGPCu+dHdIcrSRDro7S5G8s6vsmK/Frjw60OqhcYiDxf2cKYuw/PO0P5I59qjiIOe56foj+QVR0ByEfPvlsBWCxxbHcuVChXOv5RS2pG5vmV+vZh4FI6IVruFdOZDDlatcrbETfBihoZd2NcMqTsRJjEu9WdCw5l1m84M9Y6w9bk3JsxULwMgOsSDJS8rX2Y+M/PGAOrCXIrx1sgZr7bBOyvckNe8XQzqWtZ3ASQ0HgFCNIiQ96GJATpEv1jatFQQPNgmdcy/KzJ50uMJgteYm+GMJKG0LtOWoKzx3qxpTMxnrKreWUnpqCOHb5JZJkomehQ+S0JV1Iy3BhzTsGAB6wh0xBoOse+rJgyswW8/XYFgWNmjE9ru3VSppDIjNT97chFxS6Aw/p4dWCSpEFim0vcO8jolivNw2MY4N6+/LEvtvQd+F0WC",
                    MEDICINESET = "1qZDV2Zq2SrET5XRrSl9NyxAtzrpDGjX+99mSbFopthfmQkgaiD+fHSo2CgEP6znM5YsfYFaS6bDK0EsY2Y+ofFYO4NNtWEbpGNrGISlGZ+1RE5wnOOq08ndcIgPzLHAD5kIs79lZoeqiIB4WXvQhSHhSv6u/skQulLupvFD9JUu8AyTiRHzvpzfVo/rE6wmWk+nlGY5HNCoFL86ofvptiOeOOOts+8+qh7Hqhv1Y0njotaNQDAYOyV7udBYbr1xj+0Pr+vaYoZ5Z14gS7IP0vGqh79EliIb8+ZlDnA7JqGKim9JZyc8VXHXNMW5HFWM1Nk0z309z68R5rraK4w7hTokoubSkb8kECsyeADE2PwPi6mbuyzdGGJTZ/dEbF8xmAPqWXuKVxhqslU5PwM6PcahZ2dj2n0qNTxyjiB5mMWjcTKUUX7PpHegTHN8t2Wx/rsvVXZiYcd7wVRnjZdtK7Rca0YK7vEYY7sxRUrdvC6pvxNUzJjbkkIyjU8HaC5rXw2gBYkpDCJf5bquawuQ2jO5lMVEO3tgxa7KjxaUG5nWYYiAvESSUGa0FidLAC6O",

                },                
            };
        }
    }
}

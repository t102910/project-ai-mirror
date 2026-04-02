using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace QolmsJotoWebView.Tests
{
    [TestFixture]
    public class AccessKeySsoUseCaseFixture
    {
        Mock<ILoginRepository> _loginRepo;
        AccessKeySsoUseCase _usecase;

        Guid _accountKey = Guid.NewGuid();
        Guid _parentAccountKey = Guid.NewGuid();
        Guid _executor = Guid.Parse("FFAF90C8-0103-4901-0000-000000047003");

        [SetUp]
        public void Setup()
        {
            _loginRepo = new Mock<ILoginRepository>();
            _usecase = new AccessKeySsoUseCase(_loginRepo.Object);
        }

        [Test]
        public void 正常()
        {
            var args = GetValidArgs();

            _loginRepo.Setup(m => m.SsoAccountExists(_executor, _accountKey)).Returns(_accountKey);
            QjAccessKeySsoApiResults results = _usecase.Generate(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.Result.Detail.Contains("正常に終了しました。").IsTrue();
        }

        QjAccessKeySsoApiArgs GetValidArgs()
        {
            return new QjAccessKeySsoApiArgs
            {
                Executor = _executor.ToApiGuidString(),
                ExecutorName = "",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsJoto}",
                ActorKey = _accountKey.ToApiGuidString(),
                ApiType = "",
                QolmsJotoJwtPageNo = "2"
            };
        }

    }
}
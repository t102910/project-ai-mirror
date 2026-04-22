using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AppPushNotificationEntryDeleteWorkerFixture
    {
        Mock<IQoPushNotification> _pushNotification;
        AppPushNotificationEntryDeleteWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _pushNotification = new Mock<IQoPushNotification>();
            _worker = new AppPushNotificationEntryDeleteWorker(_pushNotification.Object);
        }

        [TestMethod]
        public async Task アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = await _worker.DeleteAsync(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            _pushNotification.Verify(m => m.DeleteInstallationAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task DeviceIdが未指定でエラーとなる()
        {
            var args = GetValidArgs();
            args.DeviceId = "";

            var results = await _worker.DeleteAsync(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            _pushNotification.Verify(m => m.DeleteInstallationAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task DeviceIdが101文字でエラーとなる()
        {
            var args = GetValidArgs();
            args.DeviceId = new string('a', 101);

            var results = await _worker.DeleteAsync(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            _pushNotification.Verify(m => m.DeleteInstallationAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteSystemTypeからNotificationHub設定が取得できない場合はエラーとなる()
        {
            var args = GetValidArgs();
            args.ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.None}";

            var results = await _worker.DeleteAsync(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            _pushNotification.Verify(m => m.DeleteInstallationAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteInstallationAsyncがfalseを返した場合は削除失敗エラーとなる()
        {
            var args = GetValidArgs();
            _pushNotification.Setup(m => m.DeleteInstallationAsync(It.IsAny<string>())).ReturnsAsync(false);

            var results = await _worker.DeleteAsync(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.IsNot("0200");
            _pushNotification.Verify(m => m.DeleteInstallationAsync(It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task 正常にデバイス情報が削除される()
        {
            var args = GetValidArgs();
            _pushNotification.Setup(m => m.DeleteInstallationAsync(It.IsAny<string>())).ReturnsAsync(true);

            var results = await _worker.DeleteAsync(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            _pushNotification.Verify(m => m.Initialize(It.IsAny<MGF.QOLMS.QolmsOpenApi.Models.NotificationHubsSettings>()), Times.Once());
            _pushNotification.Verify(m => m.DeleteInstallationAsync(It.IsAny<string>()), Times.Once());
        }

        QoAppPushNotificationEntryDeleteApiArgs GetValidArgs()
        {
            return new QoAppPushNotificationEntryDeleteApiArgs
            {
                ActorKey = Guid.NewGuid().ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "TestApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                DeviceId = "test-device-id-12345"
            };
        }
    }
}

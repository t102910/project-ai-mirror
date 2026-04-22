using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{
    [TestClass]
    public class AppSettingsFixture
    {
        IAppSettingsRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new AppSettingsRepository();
        }

        [TestMethod]
        public void レコード追加()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");
            var appType = QsDbApplicationTypeEnum.HeartMonitorApp;

            var settings = new QhSettingsHeartMonitorOfJson
            {
                HeartRateThresholdLow = new QhHeartRateThresholdItem
                {
                    MinValue = 30,
                    MaxValue = 100
                },
                HeartRateThresholdMid = new QhHeartRateThresholdItem
                {
                    MinValue = 35,
                    MaxValue = 110
                },
                HeartRateThresholdHigh = new QhHeartRateThresholdItem
                {
                    MinValue = 40,
                    MaxValue = 120
                },
                RestingHeartRate = 100,
                MonitorList = new List<QhHeartMonitorMemberItem>
                {
                    new QhHeartMonitorMemberItem
                    {
                        RelationshipType = QsDbFamilyRelationshipTypeEnum.Father,
                        MailAddress = "abc@def.com",
                        RequireNotifyMail = false
                    },
                    new QhHeartMonitorMemberItem
                    {
                        RelationshipType = QsDbFamilyRelationshipTypeEnum.Child,
                        MailAddress = "hoge@def.com",
                        RequireNotifyMail = true
                    }
                }
            };

            var json = new QsJsonSerializer().Serialize(settings);

            var entity = new QH_APPSETTINGS_DAT
            {
                ACCOUNTKEY = accountKey,
                APPTYPE = (int)appType,
                VALUE = json
            };

            _repo.InsertEntity(entity);
        }

        [TestMethod]
        public void レコード取得()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");
            var appType = QsDbApplicationTypeEnum.HeartMonitorApp;

            var entity = _repo.ReadEntity(accountKey, (int)appType);

            var settings = new QsJsonSerializer().Deserialize<QhSettingsHeartMonitorOfJson>(entity.VALUE);
        }

        [TestMethod]
        public void レコード更新()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");
            var appType = QsDbApplicationTypeEnum.HeartMonitorApp;

            var entity = _repo.ReadEntity(accountKey, (int)appType);

            var settings = new QsJsonSerializer().Deserialize<QhSettingsHeartMonitorOfJson>(entity.VALUE);

            settings.HeartRateThresholdLow.MaxValue = 999;

            entity.VALUE = new QsJsonSerializer().Serialize(settings);

            _repo.UpdateEntity(entity);

            entity = _repo.ReadEntity(accountKey, (int)appType);

            settings = new QsJsonSerializer().Deserialize<QhSettingsHeartMonitorOfJson>(entity.VALUE);
        }

        [TestMethod]
        public void レコード論理削除()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");
            var appType = QsDbApplicationTypeEnum.HeartMonitorApp;

            var entity = _repo.ReadEntity(accountKey, (int)appType);

            _repo.DeleteEntity(entity);
        }

        [TestMethod]
        public void レコード物理削除()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");
            var appType = QsDbApplicationTypeEnum.HeartMonitorApp;

            _repo.PhysicalDeleteEntity(accountKey, (int)appType);
        }
    }
}

using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class ReproFixtuer
    {
        IReproRepository _repo;
        Guid _accountkey = Guid.Parse("8A7D5856-621A-4CC3-981B-0317EE3FE05C");

        [TestInitialize]
        public void Initialize()
        {
            _repo = new ReproRepository();
        }

        [TestMethod]
        public void Reproにプッシュ通知リクエストを送信できる()
        {
            var args = GetValidArgs();
            var ret = _repo.ReproPushApiCall(args.Contents, new List<Guid>() { _accountkey }, args.StartDate.TryToValueType(DateTime.MinValue), args.DeeplinkUrl);

            ret.IsTrue();
        }

        [TestMethod]
        public void プッシュ通知ログを登録できる()
        {
            var args = GetValidArgs();
            var ret = _repo.InsertPushSend(new List<Guid>() { _accountkey },DateTime.Now);

            ret.IsTrue();
        }

        QoPushSendApiArgs GetValidArgs()
        {
            return new QoPushSendApiArgs
            {
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "JotoGinowan",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.JotoGinowan}",
                LinkageSystemNo = "47900021",
                LinkageIds = new List<string>() { "xxxx" },
                Title = "テストPushAPI",
                Contents = "てすと本文です。",
                PriorityNo = "3",
                CategoryNo = "1",
                StartDate = "2026/1/30",
                EndDate = "2026/2/28",
                DeeplinkUrl = "https://devjoto.qolms.com/start/LoginById",
            };
        }
    }
}

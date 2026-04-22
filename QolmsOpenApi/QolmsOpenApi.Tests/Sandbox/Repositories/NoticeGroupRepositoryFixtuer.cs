using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class NoticeGroupRepositoryFixtuer
    {
        INoticeGroupRepository _repo;
        Guid _accountkey = Guid.Parse("8A7D5856-621A-4CC3-981B-0317EE3FE05C");
        Guid _accountkey2 = Guid.Parse("08251549-B532-4E88-ACC4-1A8F9F6419EC");
        Guid _accountkey3 = Guid.Parse("39E0E96C-461E-4A9B-9DFB-260D0FF5FB9B");

        /// <summary>
        /// 登録時のfailekeyを取得して指定してください。（通しテストできません）
        /// </summary>
        long _noticeno =6; //= Guid.Parse("0386e9e1-818f-41b9-beb9-25f529357e11");

        bool _pushSend = true;
        bool _mailSend = false;
        string[] _noticeId = new string[] { "1", "2" };


        [TestInitialize]
        public void Initialize()
        {
            _repo = new NoticeGroupRepository();
        }


        [TestMethod]
        public void グループ用お知らせを取得できる()
        {
            var args = GetValidArgs();
            var ret = _repo.GetNoticeGroup(_noticeno);

            (ret.NOTICENO > 0).IsTrue();
        }

        [TestMethod]
        public void グループ用お知らせに登録できる()
        {
            var args = GetValidArgs();
            var ret = _repo.InsertNoticeGroupData(CreateNoticeGroup(args));

            _noticeno = ret;

            (ret >0).IsTrue();
        }

        [TestMethod]
        public void グループ用お知らせに更新できる()
        {
            var args = GetValidArgs();
            var ret = _repo.UpdateNoticeGroupData(_noticeno, _pushSend, _mailSend, new List<string[]>() { _noticeId });

            ret.IsTrue();
        }

        [TestMethod]
        public void グループ用お知らせの対象者を登録できる()
        {
            var args = GetValidArgs();
            var ret = _repo.WriteGroupTarget(_noticeno, new List<(Guid,string[])>() { (_accountkey,new string[]{ "id1", "id2" }), (_accountkey2, new string[] { "id1", "id2" }), (_accountkey3, new string[] { "id1", "id2" }) }, DateTime.Now, DateTime.Now);

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
                LinkageIds = new List<string>() {
                    "kuQmpH94",
                    "Gx8SB7Gi",
                    "abcdefg",
                    "hVSt97Ch",
                    "EEfXQNsw",
                    "nKu2Yd5V"
                },
                Title = "テストPushAPI",
                Contents = "てすと本文です。",
                PriorityNo = "3",
                CategoryNo = "1",
                StartDate = "2026/1/30",
                EndDate = "2026/2/28",
                DeeplinkUrl = "https://devjoto.qolms.com/start/LoginById",
            };
        }

        private QH_NOTICEGROUP_DAT CreateNoticeGroup(QoPushSendApiArgs args)
        {
            var sr = new QsJsonSerializer();

            int linkageSystemNo = int.Parse(args.LinkageSystemNo);
            DateTime startDate = DateTime.Parse(args.StartDate);
            DateTime endDate = DateTime.Parse(args.EndDate);
            DateTime now = DateTime.Now;

            QhNoticeDataSetOfJson json = new QhNoticeDataSetOfJson()
            {
                AttachedFileN = new List<QhAttachedFileOfJson>(),
                LinkN = new List<QhNoticeLinkItemOfJson>() { new QhNoticeLinkItemOfJson() { LinkText = "", LinkUrl = args.DeeplinkUrl } },
                PushSendN = new List<QhNoticePushSendItemOfJson>() { new QhNoticePushSendItemOfJson() { PushDate = "", PushId = "" } }
            };

            return new QH_NOTICEGROUP_DAT()
            {
                NOTICENO = long.MinValue,
                CONTENTS = args.Contents,
                CATEGORYNO = byte.Parse(args.CategoryNo),
                PRIORITYNO = byte.Parse(args.PriorityNo),
                FROMSYSTEMTYPE = (byte)QsDbSystemTypeEnum.QolmsJoto,//argsのsystemTypeを入れる？
                TOSYSTEMTYPE = (byte)QsDbSystemTypeEnum.QolmsJoto,
                FACILITYKEY = Guid.Empty,//表示先施設キー　全体？のときはどうする？JOTOのFacilitykeyないパターン
                TARGETTYPE = 2,
                STARTDATE = startDate,
                ENDDATE = endDate,
                MAILSENDFLAG = false,
                PUSHSENDFLAG = true,
                SCHEDULENO = 0,
                NOTICEDATASET = sr.Serialize(json),
                DELETEFLAG = false,
                CREATEDDATE = now,
                CREATEDACCOUNTKEY = Guid.Parse(args.Executor),
                UPDATEDDATE = now,
                UPDATEDACCOUNTKEY = Guid.Parse(args.Executor)
            };
        }

        
    }
}

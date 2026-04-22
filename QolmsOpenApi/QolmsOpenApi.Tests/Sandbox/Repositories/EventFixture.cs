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
    public class EventFixture
    {
        IEventRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new EventRepository();
        }

        [TestMethod]
        public void 日記投稿数情報()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var ret = _repo.GetDiaryPostCount(accountKey);
        }

        [TestMethod]
        public void 日記レコード追加()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var entity = new QH_EVENT_DAT
            {
                ACCOUNTKEY = accountKey,
                LINKAGESYSTEMNO = 99999,
                EVENTDATE = DateTime.Now.AddDays(-7),
                EVENTSEQUENCE = 0,
                EVENTSETTYPENAME = nameof(QhQolmsDiaryEventSetOfJson)
            };

            _repo.InsertEntity(entity);
        }

        [TestMethod]
        public void いいね情報取得()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var ret = _repo.GetLikeCount(accountKey);
        }

        [TestMethod]
        public void リアクションレコード追加()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");


            var entity = new QH_EVENTREACTION_DAT
            {
                ACCOUNTKEY = accountKey,
                LINKAGESYSTEMNO = 99999,
                EVENTDATE = DateTime.Now.AddDays(-2),
                EVENTSEQUENCE = 0,
                FROMACCOUNTKEY = Guid.NewGuid(),
                REACTIONTYPE = (byte)QsDbReactionTypeEnum.Good
            };

            _repo.InsertReactionEntity(entity);
        }
    }
}

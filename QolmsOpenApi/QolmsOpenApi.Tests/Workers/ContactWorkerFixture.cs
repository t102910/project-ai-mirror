using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class ContactWorkerFixture
    {
        ContactWorker _worker;
        Mock<IContactRepository> _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IContactRepository>();
            _worker = new ContactWorker(_repo.Object);
        }

        [TestMethod]
        public void ReadContactでデータ取得失敗でエラーを返す()
        {
            var args = new QoContactReadApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
                
            };

            _repo.Setup(m => m.ReadContactEntity(It.IsAny<Guid>())).Throws(new Exception());
                        
            var ret =  _worker.ReadContact(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            // 1003 DBErrorが返る
            ret.Result.Code.Is("1003");                    
        }

        [TestMethod]
        public void ReadContactで0件だった場合はnullで正常で返す()
        {
            var args = new QoContactReadApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55"
            };

            // データ無し
            _repo.Setup(m => m.ReadContactEntity(It.IsAny<Guid>())).Returns(default(QH_CONTACT_DAT));

            var ret = _worker.ReadContact(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 成功コードが返る
            ret.Result.Code.Is("0200");
            
        }

        [TestMethod]
        public void ReadContactでキー検証を通らなかったらエラーを返す()
        {
            var args = new QoContactReadApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55"
            };

            var entity = new QH_CONTACT_DAT
            {
                ACCOUNTKEY = Guid.Empty
            };

            // キーが不正のデータを返すように設定
            _repo.Setup(m => m.ReadContactEntity(It.IsAny<Guid>())).Returns(entity);

            var ret = _worker.ReadContact(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            // 汎用エラーが返る
            ret.Result.Code.Is("1001");
        }

        [TestMethod]
        public void ReadContactでContactSetが空だった場合はItemに空のインスタンスを返し正常扱いとなる()
        {
            var args = new QoContactReadApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55"
            };

            var entity = new QH_CONTACT_DAT
            {
                ACCOUNTKEY = Guid.Parse(args.ActorKey),
                CONTACTSET = ""
            };

            // ContactSetが空のデータを返すように設定
            _repo.Setup(m => m.ReadContactEntity(It.IsAny<Guid>())).Returns(entity);

            var ret = _worker.ReadContact(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 成功コードが返る
            ret.Result.Code.Is("0200");

            // 初期値が設定されている（いくつかピックアップ）
            ret.ContactItem.IsNotNull();
            ret.ContactItem.CareLevelType.Is("None");
            ret.ContactItem.PhoneN.Count.Is(0);
            ret.ContactItem.IsAboveground.Is(bool.TrueString);
        }

        [TestMethod]
        public void ReadContactで正常にデータが取得できた場合は成功とする()
        {
            var args = new QoContactReadApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55"
            };

            var original = new QhContactSetOfJson()
            {
                Address1 = "hoge",
                Address2 = "fuga",
                BloodType = QsDbBloodTypeEnum.A.ToString()
            };

            var crypted = new QsCrypt(QsCryptTypeEnum.QolmsSystem).EncryptString(new QsJsonSerializer().Serialize(original));

            var entity = new QH_CONTACT_DAT
            {
                ACCOUNTKEY = Guid.Parse(args.ActorKey),
                CONTACTSET = crypted
            };

            // ContactSetが空のデータを返すように設定
            _repo.Setup(m => m.ReadContactEntity(It.IsAny<Guid>())).Returns(entity);

            var ret = _worker.ReadContact(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 成功コードが返る
            ret.Result.Code.Is("0200");

            // 値が設定されている（いくつかピックアップ）
            ret.ContactItem.IsNotNull();
            ret.ContactItem.Address1.Is("hoge");
            ret.ContactItem.Address2.Is("fuga");
            ret.ContactItem.BloodType.Is("A");
        }

        [TestMethod]
        public void WriteContactでContactItem未設定時はエラーが返る()
        {
            var args = new QoContactWriteApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
                ContactItem = null
            };

            var ret = _worker.WriteContact(args);

            // 引数エラーとなる
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");

            // DB書き込み処理まで到達しなかった。
            _repo.Verify(m => m.WriteContactEntity(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void WriteContactでDB登録エラーの場合はエラーが返る()
        {
            var args = new QoContactWriteApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
                ContactItem = new QoApiContactItem()
            };

            _repo.Setup(m => m.WriteContactEntity(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>())).Throws(new Exception());

            var ret = _worker.WriteContact(args);

            // DBエラーとなる
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1003");
        }

        [TestMethod]
        public void WriteContactで正常に登録できる()
        {

            var args = new QoContactWriteApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
                ContactItem = new QoApiContactItem()
            };


            var ret = _worker.WriteContact(args);

            // 正常に完了した。
            ret.IsSuccess.Is(bool.TrueString);
            ret.Result.Code.Is("0200");
        }

        //[TestMethod]
        //public void WriteContactでシリアライズ失敗時はエラーが返る()
        //{
        //    // Interfaceがないので検証不可           
        //}        
    }
}

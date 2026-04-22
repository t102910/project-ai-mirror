using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsDbCoreV1;
using System.Data.Common;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class TestContactRepository
    {
        IContactRepository _repo;

        [TestInitialize]
        public void Initialzie()
        {
            _repo = new ContactRepository();
        }

        [TestMethod]
        public void 正しく連絡手帳データを取得できる()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var ret = _repo.ReadContactEntity(accountKey);

            string decoded;
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                decoded = crypt.DecryptString(ret.CONTACTSET);                
            }

            var contactSet = new QsJsonSerializer().Deserialize<QoApiContactItem>(decoded);

            // いくつかピックアップして実データと一致する
            contactSet.BloodType.Is("A");
            contactSet.AllergyN[0].Is("小麦");
            contactSet.PhoneN[0].PhoneNumber.Is("0699999999");
            contactSet.PhoneN[0].PhoneType.Is("Home");
        }

        [TestMethod]
        public void 該当データがない場合はnullが返る()
        {
            var accountKey = new Guid("11111111-5da2-495f-8a15-f2223dcc2dcf");

            var ret = _repo.ReadContactEntity(accountKey);

            ret.IsNull();
        }

        [TestMethod]
        public void 既存情報を更新できる()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            // 現在のレコードを取得
            var contactSet = GetContactItem(accountKey);

            // データを変更
            contactSet.AllergyN[2] = "アーモンド";
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            contactSet.FamilyHistoryN = new List<QoApiContactFamilyHistoryItem>
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            {
                new QoApiContactFamilyHistoryItem
                {
                    RelationshipType = QsDbFamilyRelationshipTypeEnum.Self.ToString(),
                    FatherId = "0",
                    MotherId = "0",
                    GenderType = QsDbSexTypeEnum.Male.ToString(),
                    Id = "1",
                    Name = "Test",
                    Sequence = "0",
                    Birthday = new DateTime(1977,3,27).ToApiDateString(),
                    MedicalHistoryN = new List<QoApiContactMedicalHistoryItem>
                    {
                        new QoApiContactMedicalHistoryItem
                        {
                            DiseaseType = QsDbIcd10TypeEnum.E14.ToString(),
                            DiseaseName = "糖尿病",
                            HaveSurgery = bool.FalseString,
                            When = new DateTime(2000,11,1).ToApiDateString(),
                            FacilityName = "テスト病院",
                        }
                    }
                }
            };

            var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem);
            var cryptedJson = crypt.EncryptString(new QsJsonSerializer().Serialize(contactSet));

            var actionKey = Guid.NewGuid();

            _repo.WriteContactEntity(accountKey, cryptedJson, actionKey);

            // 再度レコードを取得
            var updated = GetContactItem(accountKey);

            // データが更新されている
            updated.AllergyN[2].Is("アーモンド");
            updated.FamilyHistoryN.Count.Is(1);
            var person = updated.FamilyHistoryN[0];
            person.RelationshipType.Is("Self");
            person.GenderType.Is("Male");
            person.Name.Is("Test");
            person.Birthday.Is(new DateTime(1977, 3, 27).ToApiDateString());
            person.MedicalHistoryN[0].DiseaseType.Is("E14");
            person.MedicalHistoryN[0].DiseaseName.Is("糖尿病");

            // QH_CONTACTHIST_LOG は目視で確認する

           
        }

        [TestMethod]
        public void 新規データを登録できる()
        {
            var contactSet = new QoApiContactItem
            {
                Address1 = "hoge",
                Address2 = "fuga",
                BloodType = QsDbBloodTypeEnum.B.ToString(),
                CareLevelType = QsDbCareLevelTypeEnum.Support.ToString()
            };

            var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem);
            var cryptedJson = crypt.EncryptString(new QsJsonSerializer().Serialize(contactSet));

            var accountKey = new Guid("11111111-5da2-495f-8a15-f2223dcc2dcf");

            var actionKey = Guid.NewGuid();

            _repo.WriteContactEntity(accountKey, cryptedJson, actionKey);

            var newItem = GetContactItem(accountKey);


            newItem.Address1.Is("hoge");
            newItem.Address2.Is("fuga");
            newItem.BloodType.Is("B");
            newItem.CareLevelType.Is("Support");

            // QH_CONTACTHIST_LOG は目視で確認する
        }


        QoApiContactItem GetContactItem(Guid accountKey)
        {
            var entity = _repo.ReadContactEntity(accountKey);

            string decoded;
            var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem);
            decoded = crypt.DecryptString(entity.CONTACTSET);
          
            return new QsJsonSerializer().Deserialize<QoApiContactItem>(decoded);
        }
    }
}

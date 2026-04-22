using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 連絡手帳の情報を処理します。
    /// </summary>
    public class ContactWorker
    {
        readonly IContactRepository _contactRepo;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="contactRepository"></param>
        public ContactWorker(IContactRepository contactRepository)
        {
            _contactRepo = contactRepository;
        }


        /// <summary>
        /// 連絡手帳の情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoContactReadApiResult ReadContact(QoContactReadApiArgs args)
        {
            var result = new QoContactReadApiResult
            {
                IsSuccess = bool.FalseString,
                Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError)
            };
            var publicAccountKey = Guid.Parse(args.ActorKey);

            QH_CONTACT_DAT entity;
            try
            {
                entity = _contactRepo.ReadContactEntity(publicAccountKey);
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            // 0件は正常として返す
            if(entity == null)
            {
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                return result;
            }

            if (!entity.IsKeysValid())
            {
                return result;
            }

            if (!string.IsNullOrEmpty(entity.CONTACTSET))
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    var contact
                    = new QsJsonSerializer().Deserialize<QoApiContactItem>(crypt.DecryptString(entity.CONTACTSET));

                    result.ContactItem = contact;
                }
            }
            else
            {
                // 基本的には空というのは想定外だが、もしその場合は空のインスタンスをセットする
                result.ContactItem = new QoApiContactItem();
            }

            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return result;
        }

        /// <summary>
        /// 連絡手帳の情報を設定します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoContactWriteApiResult WriteContact(QoContactWriteApiArgs args)
        {            
            var result = new QoContactWriteApiResult
            {
                IsSuccess = bool.FalseString,
                Result = QoApiResult.Build(QoApiResultCodeTypeEnum.GeneralError)
            };
            var publicAccountKey = Guid.Parse(args.ActorKey);

            // Item未設定は引数エラーとする
            if (args.ContactItem == null)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }

            var json = new QsJsonSerializer().Serialize(args.ContactItem);

            // シリアライズ失敗時は引数エラーとする
            if (string.IsNullOrEmpty(json))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return result;
            }

            string cryptedJson;
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                cryptedJson = crypt.EncryptString(json);
            }

            var actionKey = Guid.NewGuid();

            try
            {
                _contactRepo.WriteContactEntity(publicAccountKey, cryptedJson, actionKey);
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return result;
        } 
    }

    
}
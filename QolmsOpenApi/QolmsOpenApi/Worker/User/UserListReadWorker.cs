using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsCryptV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// ユーザー情報リスト取得
    /// </summary>
    public class UserListReadWorker
    {
        IFamilyRepository _familyRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="familyRepository"></param>
        public UserListReadWorker(IFamilyRepository familyRepository)
        {
            _familyRepo = familyRepository;
        }

        /// <summary>
        /// 取得処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoUserListReadApiResults Read(QoUserListReadApiArgs args)
        {
            var results = new QoUserListReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // 本人含む家族リストを取得
            if (!TryReadFamilyList(accountKey, results, out var entityList))
            {
                return results;
            }

            try
            {
                // EntityをAPI形式に変換
                results.UserList = entityList.ConvertAll(x => BuildAccountUserItem(args.Executor, x, accountKey));
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ユーザー情報の変換に失敗しました。");
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        // DBから家族情報を取得
        bool TryReadFamilyList(Guid accountKey, QoApiResultsBase results, out List<QH_ACCOUNTINDEX_DAT> entityList)
        {
            try
            {
                entityList = _familyRepo.ReadFamilyList(accountKey);
                if (!entityList.Any())
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "アカウント情報が存在しませんでした。");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                entityList = null;
                results.Result = QoApiResult.Build(ex, "DBからのアカウント情報リストの取得に失敗しました。");
                return false;
            }
        }

        QoApiUserItem BuildAccountUserItem(string executor, QH_ACCOUNTINDEX_DAT entity, Guid publicAccountKey = default)
        {
            var publicKey = Guid.Empty;
            if (publicAccountKey != default && publicAccountKey != entity.ACCOUNTKEY)
            {
                publicKey = publicAccountKey;
            }

            var tokenProvider = new QsJwtTokenProvider();
            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                var encExeCutor = crypt.EncryptString(executor.TryToValueType(Guid.Empty).ToString("N"));
                return new QoApiUserItem()
                {
                    AccountKeyReference = entity.ACCOUNTKEY.ToEncrypedReference(),
                    FamilyName = crypt.TryDecrypt(entity.FAMILYNAME),
                    GivenName = crypt.TryDecrypt(entity.GIVENNAME),
                    FamilyNameKana = crypt.TryDecrypt(entity.FAMILYKANANAME),
                    GivenNameKana = crypt.TryDecrypt(entity.GIVENKANANAME),
                    Sex = entity.SEXTYPE,
                    NickName = crypt.TryDecrypt(entity.NICKNAME),
                    Birthday = entity.BIRTHDAY.ToApiDateString(),
                    AccessKey = publicKey == Guid.Empty ? string.Empty : tokenProvider.CreateOpenApiJwtAccessKey(encExeCutor, entity.ACCOUNTKEY, publicKey, (int)QoApiFunctionTypeEnum.All),
                    PersonPhotoReference = entity.PHOTOKEY == Guid.Empty ? string.Empty : entity.PHOTOKEY.ToEncrypedReference(),
                };
            }
        }
    }
}
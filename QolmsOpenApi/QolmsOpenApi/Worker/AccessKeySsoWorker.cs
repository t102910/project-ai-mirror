using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// シングルサインオンキー取得処理
    /// </summary>
    public class AccessKeySsoWorker
    {
        IAccountRepository _accountRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accountRepository"></param>
        public AccessKeySsoWorker(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// SSOキーの生成
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAccessKeySsoApiResults Generate(QoAccessKeySsoApiArgs args)
        {
            var results = new QoAccessKeySsoApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーに変換
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);

            // 該当アカウントをマスタより取得
            if(!TryGetAccount(accountKey, results, out var user))
            {
                return results;
            }

            // 親アカウントキーを取得。親であればGuid.Emptyがセットされる。
            if (!TryGetParentAccountKey(user, results, out var parentAccountKey))
            {
                return results;
            }

            try
            {
                // SSOの生成
                var tokenProvider = new QsJwtTokenProvider();
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    var encExeCutor = crypt.EncryptString(args.Executor.TryToValueType(Guid.Empty).ToString("N"));
                    // 有効期限15分
                    results.SsoKey = tokenProvider.CreateQolmsJwtSsoKey(encExeCutor, user.ACCOUNTKEY, parentAccountKey, 1, DateTime.Now.AddMinutes(15));
                }
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "シングルサインオンキーの生成に失敗しました。");
                return results;
            }

            try
            {
                // PASSWORDMANAGEMENT取得
                if (int.TryParse(args.QolmsPageNo, out int pageNo) && int.TryParse(args.QolmsSsoShowMenu, out int ssoShowMenu) && TryGetPasswordManagement(accountKey, results, out var passwordManagement))
                {
                    //Qolms用SSOKey生成
                    results.QolmsSsoKey = CreateQolmsSSOKey(passwordManagement, pageNo, args.ExecuteSystemType,ssoShowMenu );
                }
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "QOLMS用シングルサインオンキーの生成に失敗しました。");
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.Success);

            return results;            
        }

        bool TryGetAccount(Guid accountKey, QoApiResultsBase results, out QH_ACCOUNT_MST entity)
        {
            try
            {
                // アカウントマスタを取得
                entity = _accountRepository.ReadMasterEntity(accountKey);
                if(entity == null)
                {
                    results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.OperationError, "アカウントが存在しませんでした。");
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                entity = null;
                results.Result = QoApiResult.Build(ex, "アカウント情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryGetParentAccountKey(QH_ACCOUNT_MST childAccount, QoApiResultsBase results, out Guid parentAccountKey)
        {
            parentAccountKey = Guid.Empty;
            if(!childAccount.PRIVATEACCOUNTFLAG)
            {
                // 対象が親アカウントであれば何もしない
                return true;
            }

            try
            {
                // 親アカウントを取得
                var parentUser = _accountRepository.ReadParentMasterEntity(childAccount.ACCOUNTKEY);
                if(parentUser == null)
                {
                    results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.OperationError, "親アカウントが存在しませんでした。");
                    return false;
                }
                parentAccountKey = parentUser.ACCOUNTKEY;

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "親アカウント情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryGetPasswordManagement(Guid accountKey, QoApiResultsBase results, out QH_PASSWORDMANAGEMENT_DAT entity)
        {
            try
            {
                // QH_PASSWORDMANAGEMENT_DATを取得
                entity = _accountRepository.ReadPasswordManagementEntity(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(Enums.QoApiResultCodeTypeEnum.OperationError, "アカウントが存在しませんでした。");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                entity = null;
                results.Result = QoApiResult.Build(ex, "アカウント情報の取得に失敗しました。");
                return false;
            }
        }

        string CreateQolmsSSOKey(QH_PASSWORDMANAGEMENT_DAT entity, int pageNo, string systemType , int ssoShowMenu)
        {
            try
            {
                var QolmsSsoKey = string.Empty;
                StringBuilder hash = new StringBuilder();
                using (var sha256 = new SHA256CryptoServiceProvider())
                {
                    byte[] beforeByteArray = Encoding.UTF8.GetBytes(entity.USERPASSWORD);
                    byte[] afterByteArray = sha256.ComputeHash(beforeByteArray);

                    // バイト配列を16進数文字列に変換
                    foreach (byte b in afterByteArray)
                    {
                        hash.Append(b.ToString("x2"));
                    }
                }
                // QOLMS用SSOの生成
                // 有効期限15分
                var json = new QiQolmsSimpleSsoTokenOfJson()
                {
                    SystemType = systemType,
                    AuthorKey = entity.ACCOUNTKEY.ToString("N"),
                    ActorKey = entity.ACCOUNTKEY.ToString("N"),
                    UserId = entity.USERID,
                    PasswordHash = hash.ToString(),
                    PageNo = pageNo.ToString(),
                    SsoShowMenu = ssoShowMenu.ToString(),
                    Expires = DateTime.Now.AddMinutes(15).ToString(),
                };
                var serializer = new QsJsonSerializer();
                var strKey = serializer.Serialize(json);

                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                {
                    QolmsSsoKey = crypt.EncryptString(strKey);
                }
                return QolmsSsoKey;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return string.Empty;
            }
        }
    }
}
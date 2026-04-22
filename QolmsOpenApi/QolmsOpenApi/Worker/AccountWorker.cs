using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System.Text.RegularExpressions;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// アカウント関連の処理
    /// </summary>
    public sealed class AccountWorker
    {
       

        #region "Private Method"
        

        private static System.Threading.Tasks.Task<bool> SendSignUpMail(string mailAddress, string url,string param)
        {
            string bodthPath = string.Format("~/App_Data/MailBodySignUp_{0}.txt", param);
            string footerPath = string.Format("~/App_Data/MailFooter_{0}.txt", param);
            string settingsName = string.Format("MailSettingsNameSignUp_{0}", param);
            string subject = string.Format("MailSubjectSignUp_{0}", param);
            return new SignUpNoticeClient(new SignUpNoticeClientArgs(settingsName, subject, mailAddress, url,bodthPath,footerPath)).SendAsync();
        }

        /// <summary>
        /// パスワードリセットメールを送信します。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static System.Threading.Tasks.Task<bool> SendPasswordResetMail(string mailAddress, string url,string param)
        {
            string bodyPath = string.Format("~/App_Data/MailBodyPasswordReset_{0}.txt", param);
            string footerPath =string.Format("~/App_Data/MailFooter_{0}.txt", param);
            string settingsName = string.Format("MailSettingsNamePasswordReset_{0}", param);
            string subject = string.Format("MailSubjectPasswordReset_{0}", param);
            QoAccessLog.WriteInfoLog(bodyPath);
            return new PasswordResetNoticeClient(new PasswordResetNoticeClientArgs(settingsName, subject, mailAddress, url, bodyPath, footerPath)).SendAsync();
        }

        /// <summary>
        /// メールアドレス変更メール（メール認証）を送信します。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static System.Threading.Tasks.Task<bool> SendMailAddressChangeMail(string mailAddress, string url, string param)
        {
            string bodyPath = string.Format("~/App_Data/MailBodyMailAddressChange_{0}.txt", param);
            string footerPath = string.Format("~/App_Data/MailFooter_{0}.txt", param);
            string settingsName = string.Format("MailSettingsNameMailAddressChange_{0}", param);
            string subject = string.Format("MailSubjectMailAddressChange_{0}", param);
            return new MailAddressChangeNoticeClient(new MailAddressChangeNoticeClientArgs(settingsName, subject, mailAddress, url,bodyPath,footerPath )).SendAsync();
        }

        // <summary>
        /// メールアドレス設定メール（UserIDお知らせ）を送信します。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static System.Threading.Tasks.Task<bool> SendMailAddressSetMail(string mailAddress, string userid, string param)
        {
            string bodyPath = string.Format("~/App_Data/MailBodyMailAddressSet_{0}.txt", param);
            string footerPath = string.Format("~/App_Data/MailFooter_{0}.txt", param);
            string settingsName = string.Format("MailSettingsNameMailAddressSet_{0}", param);
            string subject = string.Format("MailSubjectMailAddressSet_{0}", param);
            return new MailAddressSetNoticeClient(new MailAddressSetNoticeClientArgs(settingsName, subject, mailAddress, userid, bodyPath, footerPath)).SendAsync();
        }

        /// <summary>
        /// ID問い合わせメールを送信します。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="forgetid"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static System.Threading.Tasks.Task<bool> SendForgetIdMail(string mailAddress, string forgetid, string param)
        {
            string bodyPath = string.Format("~/App_Data/MailBodyForgetId_{0}.txt", param);
            string footerPath = string.Format("~/App_Data/MailFooter_{0}.txt", param);
            string settingsName = string.Format("MailSettingsNameForgetId_{0}", param);
            string subject = string.Format("MailSubjectForgetId_{0}", param);
            QoAccessLog.WriteInfoLog(bodyPath);
            return new ForgetIdNoticeClient(new ForgetIdNoticeClientArgs(settingsName, subject, mailAddress, forgetid, bodyPath, footerPath)).SendAsync();
        }

        /// <summary>
        /// 退会処理をおこないます。
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="actorKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="unsubscribeItemNo"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static bool ProcessWithdraw(Guid authorKey, Guid actorKey, int linkageSystemNo, byte unsubscribeItemNo, string comment)
        {
            DbUnsubscribeWriter writer = new DbUnsubscribeWriter();
            DbUnsubscribeWriterArgs writerArgs = new DbUnsubscribeWriterArgs()
            {
                AuthorKey = authorKey,
                ActorKey = actorKey,
                LinkageSystemNo = linkageSystemNo,
                UnsubscribeItemNo = unsubscribeItemNo,
                Comment = comment
            };
            DbUnsubscribeWriterResults writerResults = new DbUnsubscribeWriterResults();

            try
            {
                // DbLibraryの退会処理呼出し
                return QsDbManager.Write(writer, writerArgs).IsSuccess;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// アカウントキーからLinkageテーブルをみてLinkageIdを取得してPush通知用に返す
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="systemType"></param>
        /// <returns></returns>
        private static string GetLinkageIdReference(Guid accountKey, QsApiSystemTypeEnum systemType)
        {
            // 対応するLinkageSystemNoを取得
            var linkageSystemNo = systemType.ToLinkageSystemNo();
            if (!linkageSystemNo.IsNaviApp())
            {
                // 医療ナビ系(HOSPA/医療ナビ)以外はアカウントキー参照をPush通知IDとする
                return accountKey.ToEncrypedReference();
            }            

            // 対応LinkageSystemNoに対応するユーザーの連携IDを取得する(Push通知用)
            var entity = new QH_LINKAGE_DAT() { ACCOUNTKEY = accountKey, LINKAGESYSTEMNO = linkageSystemNo };
            var readerArgs = new QhLinkageEntityReaderArgs() { Data = new List<QH_LINKAGE_DAT>() { entity } };
            QhLinkageEntityReaderResults readerResults = QsDbManager.Read(new QhLinkageEntityReader(), readerArgs);
            if (readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
            {
                return readerResults.Result.First().LINKAGESYSTEMID.ToEncrypedReference();
            }
            else
            {
                // エラーを返さなくて良いのか検討しても良いかも知れない
                return string.Empty;
            }            
        }
        /// <summary>
        /// Executor を暗号化して返す
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        private static string GetEncryptExecutor(string executor)
        {
            string encExeCutor = executor;
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                try
                {
                    encExeCutor = crypt.EncryptString(executor.TryToValueType(Guid.Empty).ToString("N"));
                }
                catch (Exception) { }
            }
            return encExeCutor;
        }

        /// <summary>
        /// アカウントマスタテーブルエンティティを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="includeDelete">
        /// 削除済みも対象にするかのフラグ（オプショナル）。
        /// 対象にするなら True、
        /// 対象にしない False（デフォルト）を指定します。</param>
        /// <returns>データが存在するなら該当するテーブルエンティティ、存在しないなら Nothing。</returns>
        private static QH_ACCOUNT_MST SelectAccountEntity(Guid accountKey, bool includeDelete = false)
        {
            var entity = new QH_ACCOUNT_MST() { ACCOUNTKEY = accountKey };
            var reader = new QhAccountEntityReader();
            var readerArgs = new QhAccountEntityReaderArgs() { Data = new List<QH_ACCOUNT_MST>() { entity } };
            QhAccountEntityReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if(readerResults.IsSuccess && 
                readerResults.Result.Count == 1 &&
                ( includeDelete || !readerResults.Result.First().DELETEFLAG))
            {
                return readerResults.Result.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// アカウントインデックスデータテーブルエンティティを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="includeDelete">
        /// 削除済みも対象にするかのフラグ（オプショナル）。
        /// 対象にするなら True、
        /// 対象にしない False（デフォルト）を指定します。</param>
        /// <returns>データが存在するなら該当するテーブルエンティティ、存在しないなら Nothing。</returns>
        private static QH_ACCOUNTINDEX_DAT SelectAccountIndexEntity(Guid accountKey, bool includeDelete = false)
        {
            var entity = new QH_ACCOUNTINDEX_DAT() { ACCOUNTKEY = accountKey };
            var reader = new QhAccountIndexEntityReader();
            var readerArgs = new QhAccountIndexEntityReaderArgs() { Data = new List<QH_ACCOUNTINDEX_DAT>() { entity } };
            QhAccountIndexEntityReaderResults readerResults  = QsDbManager.Read(reader, readerArgs);

            if (readerResults.IsSuccess &&
                readerResults.Result.Count == 1 &&
                (includeDelete || !readerResults.Result.First().DELETEFLAG))
            {
                return readerResults.Result.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 忘れたIDを検索項目をもとに取得
        /// </summary>
        /// <param name="mailAddress">登録メールアドレス</param>
        /// <param name="familyName">登録姓</param>
        /// <param name="givenName">登録名</param>
        /// <param name="birthday">登録生年月日</param>
        /// <param name="sex">登録性別</param>
        /// <returns></returns>
        private static Tuple<int, string> GetIdForget(string mailAddress, string familyName, string givenName, DateTime birthday, byte sex)
        {
            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                //メールアドレスを暗号化
                mailAddress = crypt.EncryptString(mailAddress);
                // 漢字姓を暗号化
                familyName = crypt.EncryptString(familyName);
                // 漢字名を暗号化
                givenName = crypt.EncryptString(givenName);
            }
            var readerArgs = new AccountIdForgetReaderArgs() { MailAddress = mailAddress, FamilyName = familyName, GivenName = givenName, Birthday = birthday, Sex = sex };
            var readerResults = QsDbManager.Read(new AccountIdForgetReader(), readerArgs);
            
            if (readerResults.IsSuccess && readerResults.Result != null && readerResults.AccountId.Count == 1)
            {
                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    //正常の場合取得件数が1件
                    return Tuple.Create(readerResults.AccountId.Count, crypt.DecryptString(readerResults.AccountId.First()));
                }
            }
            else if (readerResults.IsSuccess && readerResults.Result != null)
            {
                //取得結果が異常の場合Countのみ返却
                return Tuple.Create(readerResults.AccountId.Count, string.Empty);
            }
            //上記以外
            return Tuple.Create(int.MinValue, string.Empty);
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// アカウントの存在確認を行います。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public static bool IsExistsAccount(Guid accountKey)
        {
            var result = false;

            if(accountKey != Guid.Empty)
            {
                QH_ACCOUNT_MST account = AccountWorker.SelectAccountEntity(accountKey);

                if(account != null && account.IsKeysValid())
                {
                    QH_ACCOUNTINDEX_DAT accountIndex = AccountWorker.SelectAccountIndexEntity(accountKey);
                    if (accountIndex != null && accountIndex.IsKeysValid())
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// ログイン認証を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountLoginApiResults Login(QoAccountLoginApiArgs args)
        {
            var result = new QoAccountLoginApiResults()
            {
                IsSuccess = bool.FalseString,
                LoginResultType = QsApiLoginResultTypeEnum.None.ToString(),
                Token = string.Empty
            };

            // OpenApi 専用処理
            string encExeCutor = string.Empty;
            using (var crypt=new QsCrypt(QsCryptTypeEnum.QolmsSystem) )
            {
                try
                {
                    //　二段階認証を行う場合のみUserIdは暗号化されているので自力で復号化
                    if (!string.IsNullOrWhiteSpace(args.PasswordHash) && string.IsNullOrWhiteSpace(args.Password) && !string.IsNullOrEmpty(args.UserId))
                    {
                        args.UserId = crypt.DecryptString(args.UserId);
                    }
                    encExeCutor = crypt.EncryptString(args.Executor.TryToValueType(Guid.Empty).ToString("N"));
                }
                catch (Exception){}                  
            }

            //Identity Apiをよんでログイン
            QiQolmsLoginApiResults identityResult=QoIdentityClient.ExecuteQolmsLoginApi(args.SessionId, args.UserId, args.Password, args.PasswordHash,
                args.UseTwoFactorAuthentication.TryToValueType<bool>(false), args.TwoFactorAuthenticationToken);
            result.LoginResultType = identityResult.LoginResultType;
            switch (identityResult.LoginResultType.TryToValueType<QsApiLoginResultTypeEnum>(QsApiLoginResultTypeEnum.None))
            {
                case QsApiLoginResultTypeEnum.Success:
                    result.IsSuccess = bool.TrueString;
                    Guid accountKey = identityResult.AccountKey.TryToValueType(Guid.Empty);
                    result.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(encExeCutor,accountKey );
                    result.LinkageIdReference = GetLinkageIdReference(accountKey,args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
                    //TODO: ログイン成功時のメール、OpenApiでメールを送信しないといけない
                    break;
                case QsApiLoginResultTypeEnum.Lockdown:
                    //ロックダウン中
                    result.LoginRetryCount = identityResult.LoginRetryCount;
                    result.LoginLockdownExpires = identityResult.LoginLockdownExpires;
                    result .LoginNotificationMailAddress =identityResult.LoginNotificationMailAddress;
                    //TODO：ロックダウン時のメール、OpenApiでメールを送信しないといけない
                    break;
                case QsApiLoginResultTypeEnum.Retry:
                case QsApiLoginResultTypeEnum.TwoFactorRetry:
                case QsApiLoginResultTypeEnum.TwoFactorTimeout:
                    break;
                case QsApiLoginResultTypeEnum.TwoFactorRequire:
                    //二要素認証が必要
                    result.UserId = args.UserId;
                    result.PasswordHash = identityResult.PasswordHash;
                    result.TwoFactorAuthenticationMailAddress = identityResult.TwoFactorAuthenticationMailAddress;
                    result.TwoFactorAuthenticationToken = identityResult.TwoFactorAuthenticationToken;
                    result.TwoFactorAuthenticationExpires = identityResult.TwoFactorAuthenticationExpires;
                    if (!string.IsNullOrWhiteSpace(result.TwoFactorAuthenticationMailAddress))
                    {
                        //TODO：QOLMSはWebサイト側で二段階認証用のメールを送信しているが、OpenApiは自力で送信しないといけない
                    }
                    break;

                default:
                    break;
            }
            //== OpenApi専用処理 ==            
            if( result.LoginResultType.TryToValueType(QsApiLoginResultTypeEnum.None) == QsApiLoginResultTypeEnum.Lockdown)
            {
                //ロックダウン時は、メッセージからアカウントを特定されないようRetry扱いで返す
                result.LoginResultType = QsApiLoginResultTypeEnum.Retry.ToString();
                result.LoginRetryCount = string.Empty;
                result.LoginLockdownExpires = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// アカウント仮登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        public static QoAccountSignUpApiResults SignUp(QoAccountSignUpApiArgs args)
        {
            QoAccountSignUpApiResults result = new QoAccountSignUpApiResults() { IsSuccess = bool.FalseString };
            string message = string.Empty;
            Guid accountkey = Guid.Empty;
            // 同日に規定回数以上同じ申請があった場合は注意書き表示をする
            int count = QoApiConfiguration.SignUpMailAddressMaxCount;

            try
            {
                if (SignUpWorker.SignUpRead(args.Mail) >= count)
                {
                    // メールアドレスの重複エラー
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SignUpMailAddressCountOver);
                    message = result.Result.Detail;
                }
                //else if (PasswordManagementHelper.IsUsedMailAddress(args.Mail))
                //{
                //    // メールアドレス使用済み
                //    result.Result = ApiResultWorker.Build(QoApiResultCodeTypeEnum.UsedMailAddress);
                //    message = result.Result.Detail;
                //}
                else
                {
                    // IdentityApi実行
                    accountkey = SignUpWorker.SignUpWrite(args.Mail);

                    if (accountkey != Guid.Empty)
                    {
                        string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
                        string url = string.Format("{0}?p={2}&a={1}",QoApiConfiguration.SignUpUrl, accountkey.ToEncrypedReference(DateTime.Now), p);

                        // API実行が成功したら、URLを生成し、メール送信
                        AccountWorker.SendSignUpMail(args.Mail, url,p);

                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    }
                    else
                    {
                        // 仮登録エラー
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SignUpGuidError);
                        message = result.Result.Detail;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                message = ex.Message;
            }

            try
            {
                // ログ出力
                QoAccessLog.AccessTypeEnum accessType = QoAccessLog.AccessTypeEnum.None;

                if (!string.IsNullOrWhiteSpace(message))
                    // 失敗 (メールアドレスの重複エラー、仮登録エラー)
                    accessType = QoAccessLog.AccessTypeEnum.Error;
                else
                    // 成功
                    message = string.Format("AccountKey={0}：{1}", accountkey, "アカウント仮登録の申請がありました。");

                QoAccessLog.WriteAccessLog(null/* TODO Change to default(_) if this is not a reference type */, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Parse(args.Executor), DateTime.Now, accessType, "/Account/SignUp", message);
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// アカウント本登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public static QoAccountRegisterApiResults Register(QoAccountRegisterApiArgs args)
        {
            QoAccountRegisterApiResults result = new QoAccountRegisterApiResults() { IsSuccess = bool.FalseString };
            List<string> errorList = new List<string>();
            if (args.Account == null)
                throw new ArgumentNullException("Account", "本登録情報が不正です。");
            if (!args.Account.Validate(ref errorList))
                throw new ArgumentException("Account", string.Join(Environment.NewLine, errorList.ToArray()));
            if (!string.IsNullOrWhiteSpace(args.Account.Tel) && !new Regex(QoApiConfiguration.REGEX_TEL).IsMatch(args.Account.Tel))
                throw new ArgumentException("電話番号のフォーマットが不正です。");

            
            // キーが正しいかどうか確認
            Guid accountkey = args.Account.AccountKeyReference.ToDecrypedReference<Guid>();

            // 仮登録データチェック
            if (SignUpWorker.CheckTheKey(accountkey,out string mailaddress,out bool expiresOk) && expiresOk)
            {
                args.Account.Mail = mailaddress;

                /*if (PasswordManagementHelper.IsUsedMailAddress(args.Account.Mail))
                    // メールアドレス使用済み
                    result.Result = ApiResultWorker.Build(QoApiResultCodeTypeEnum.UsedMailAddress);
                else*/ if (SignUpWorker.RegisterWrite(args.Account, ref errorList))// IdentityApiへ値を投げる
                {
                    // 電話番号の更新（連絡手帳）
                    // If Not String.IsNullOrWhiteSpace(args.Account.Tel) Then AccountFamilyWorker.WriteContactEntity(accountkey, args.Account.Tel)
                    string encExeCutor = GetEncryptExecutor(args.Executor);
                    result.IsSuccess = bool.TrueString;
                    result.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(encExeCutor, accountkey);

                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                }
                else if (errorList.Contains("UserId"))
                    // ユーザーID重複
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.UserIdDuplicate);
                else
                    // 登録失敗
                    QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError);
            }
            else if (!expiresOk)
                // 有効期限切れ
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.AccountRegisterExpired);
            else
                // ここには来ないはず？
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.UnknownError);

            return result;
        }

        /// <summary>
        /// パスワードリセット処理をするためのメールを送ります。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountPasswordResetApiResults PasswordReset(QoAccountPasswordResetApiArgs args)
        {
            QoAccountPasswordResetApiResults result = new QoAccountPasswordResetApiResults() { IsSuccess = bool.FalseString };

            if (string.IsNullOrWhiteSpace(args.UserId))
                throw new ArgumentNullException("UserId", "ユーザーIDが不正です。");
            if (string.IsNullOrWhiteSpace(args.Mail))
                throw new ArgumentNullException("Mail", "メールアドレスが不正です。");

            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                Guid accountKey = QoPasswordManagement.CheckUserIdMailAddress<QH_PASSWORDMANAGEMENT_DAT>(args.UserId.Trim(), args.Mail.Trim(), crypt);

                if (accountKey != Guid.Empty)
                {
                    // ユーザーID、メールアドレスが一致
                    string tempPass = QoPasswordManagement.CreateTemporaryPassword();

                    if (QoPasswordManagement.EditPassword<QH_PASSWORDMANAGEMENT_DAT>(accountKey, tempPass))
                    {
                        string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
                       
                        string url = string.Format("{0}?p={3}&d={1}&h={2}", 
                            QoApiConfiguration.PasswordResetUrl , 
                            args.UserId.Trim().ToEncrypedReference(DateTime.Now.AddDays(1)),
                            QoPasswordManagement.CreatePasswordHashString(tempPass), p );

                        // API実行が成功したら、URLを生成し、メール送信
                        AccountWorker.SendPasswordResetMail(args.Mail, url,p);

                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// パスワードを編集します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountPasswordEditApiResults PasswordEdit(QoAccountPasswordEditApiArgs args)
        {
            QoAccountPasswordEditApiResults result = new QoAccountPasswordEditApiResults() { IsSuccess = bool.FalseString };

            if (string.IsNullOrWhiteSpace(args.NewPassword))
                throw new ArgumentNullException("NewPassword", "新しいパスワードが不正です。");

            Guid accountKey = Guid.Empty;

            if (string.IsNullOrWhiteSpace(args.ActorKey))
            {
                // 未ログイン（パスワードリセット時）
                if (string.IsNullOrWhiteSpace(args.PasswordHash))
                    throw new ArgumentNullException("PasswordHash", "パスワードハッシュが不正です。");
                if (string.IsNullOrWhiteSpace(args.UserIdReference))
                    throw new ArgumentNullException("UserIdReference", "ユーザーID参照文字列が不正です。");

                Nullable<DateTime> timestamp = DateTime.MinValue;
                string userId = args.UserIdReference.ToDecrypedReference(ref timestamp);
                string refPasswordHash = string.Empty;
                if (timestamp != null && timestamp >= DateTime.Now)
                {
                    using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                    {
                        QoPasswordManagement.CheckPassword<QH_PASSWORDMANAGEMENT_DAT>(userId, string.Empty, args.PasswordHash, crypt, ref accountKey, ref refPasswordHash);
                    }
                }
            }
            else
                // ログイン済み（JWTトークン認証により人物が特定出来る）
                accountKey = args.ActorKey.ToValueType<Guid>();

            if (accountKey != Guid.Empty)
            {
                string errorMessage = string.Empty;

                // 新しいパスワードの有効性チェック
                if (QoPasswordManagement.CheckNewPassword<QH_PASSWORDMANAGEMENT_DAT>(accountKey, args.NewPassword.Trim(), ref errorMessage))
                {

                    // パスワード変更
                    if (QoPasswordManagement.EditPassword<QH_PASSWORDMANAGEMENT_DAT>(accountKey, args.NewPassword))
                    {
                        string encExeCutor = GetEncryptExecutor(args.Executor);
                        result.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(encExeCutor, accountKey);
                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    }
                }
                else
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, errorMessage);
            }
            else
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.UnknownError);

            return result;
        }

        /// <summary>
        /// アカウント詳細情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountInformationReadApiResults InformationRead(QoAccountInformationReadApiArgs args)
        {
            QoAccountInformationReadApiResults result = new QoAccountInformationReadApiResults() { IsSuccess = bool.FalseString };

            Guid accountKey = args.ActorKey.ToValueType<Guid>();
            QH_PASSWORDMANAGEMENT_DAT entity = new DbPasswordManagementReaderCore().ReadPasswordManagementEntity<QH_PASSWORDMANAGEMENT_DAT>(accountKey);
            QH_CONTACT_DAT entity2 = AccountFamilyWorker.ReadContactEntity(accountKey);

            string mail = string.Empty;
            string tel = string.Empty;
            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                if (entity != null && entity.IsKeysValid())
                    mail = string.IsNullOrWhiteSpace(entity.PASSWORDRECOVERYMAILADDRESS) ? string.Empty : crypt.DecryptString(entity.PASSWORDRECOVERYMAILADDRESS);

                if (entity2 != null && entity.IsKeysValid())
                    tel = AccountFamilyWorker.GetPhoneNumber(entity2, crypt);
            }

            // TODO 二要素認証情報の取得・返却
            // TODO パスワード変更情報（最終更新日、パスワード変更メッセージの非表示期間）の取得・返却？
            // TODO デバイス情報、プッシュ通知の取得・返却？

            
            result.Information = new QoApiAccountInformationItem() { Mail = mail, Tel = tel };
            
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            

            return result;
        }

        /// <summary>
        /// アカウント詳細情報を保存します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountInformationWriteApiResults InformationWrite(QoAccountInformationWriteApiArgs args)
        {
            QoAccountInformationWriteApiResults result = new QoAccountInformationWriteApiResults() { IsSuccess = bool.FalseString };

            Guid accountKey = args.ActorKey.ToValueType<Guid>();

            // アカウント情報の更新
            if (args.Information != null)
            {
                // パスワードリカバリ用メールアドレスの更新
                if (!string.IsNullOrWhiteSpace(args.Information.Mail))
                {
                    string mailAddress = args.Information.Mail;
                    try
                    {
                        if (!mailAddress.Contains("@"))         
                            mailAddress = mailAddress.ToDecrypedReference();
                    }
                    catch (Exception ex)
                    {
                        QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
                    }

                    //if (PasswordManagementHelper.IsUsedMailAddress(mailAddress))
                    //    // メールアドレス使用済み
                    //    result.Result = ApiResultWorker.Build(QoApiResultCodeTypeEnum.UsedMailAddress);
                    //else
                    //{
                        // 更新不要はTrue
                        bool isSuccessEditMail = true;
                        bool isSuccessEditTel = true;
                        var isHospa = args.ExecuteSystemType.ToLinkageSystemNo() == QoLinkage.TIS_LINKAGE_SYSTEM_NO;
                        (string oldMailAddress,string userid) = QoPasswordManagement.GetMailAddress(accountKey);
                        if (isHospa && string.IsNullOrEmpty(oldMailAddress) && !string.IsNullOrEmpty(userid))
                        {
                            //ユーザIDは自動でメールアドレスなしで登録するTIS用処理
                            // HOSPA1.0.6での仕様変更によりこの処理は使われなくなる
                            // 互換用にしばらくは残す
                            isSuccessEditMail = QoPasswordManagement.EditMailAddress<QH_PASSWORDMANAGEMENT_DAT>(accountKey, mailAddress);
                            //ユーザIDをメールに入れて送る
                            SendMailAddressSetMail(mailAddress, userid,AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)));
                        }
                        else if(oldMailAddress != mailAddress && !string.IsNullOrEmpty(userid))
                        {
                            isSuccessEditMail = QoPasswordManagement.EditMailAddress<QH_PASSWORDMANAGEMENT_DAT>(accountKey, mailAddress);
                        }
                    // 電話番号（連絡手帳）の更新（空白なら削除）
                    // isSuccessEditTel = AccountFamilyWorker.WriteContactEntity(accountKey, args.Information.Tel)


                    if (isSuccessEditMail && isSuccessEditTel)
                        {
                            result.IsSuccess = bool.TrueString;
                            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                        }
                        else
                            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError);
                        
                    //}
                }
            }

            return result;
        }

        /// <summary>
        /// 退会理由マスタを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoAccountWithdrawMstReadApiResults WithdrawMstRead(QoAccountWithdrawMstReadApiArgs args)
        {
            QoAccountWithdrawMstReadApiResults result = new QoAccountWithdrawMstReadApiResults() { IsSuccess = bool.FalseString };

            DbUnsubscribeReader reader = new DbUnsubscribeReader();
            DbUnsubscribeReaderArgs readerArgs = new DbUnsubscribeReaderArgs()
            {
                AuthorKey = args.AuthorKey.TryToValueType(Guid.Empty),
                ActorKey = args.ActorKey.TryToValueType(Guid.Empty),
                LinkageSystemNo = args.LinkageSystemNo.TryToValueType(int.MinValue)
            };
            DbUnsubscribeReaderResults readerResults = new DbUnsubscribeReaderResults();

            try
            {
                readerResults = QsDbManager.Read(reader, readerArgs);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex,"退会理由マスタの取得に失敗しました。",args.Executor.TryToValueType(Guid.Empty) );
            }

           
            if (readerResults.IsSuccess && readerResults.UnsubscribeItemN != null)
            {
                if (readerResults.UnsubscribeItemN.Any())
                    result.WithdrawMstN = readerResults.UnsubscribeItemN.ConvertAll(i => new QoApiAccountWithdrawMstItem()
                    {
                        LinkageSystemNo = i.LinkageSystemNo.ToString(),
                        UnsubscribeItemNo = i.UnsubscribeItemNo.ToString(),
                        UnsubscribeItemName = i.UnsubscribeItemName
                    } );
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            }
            

            return result;
        }

        /// <summary>
        /// 退会処理をおこないます。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoAccountWithdrawApiResults Withdraw(QoAccountWithdrawApiArgs args)
        {
            QoAccountWithdrawApiResults result = new QoAccountWithdrawApiResults() { IsSuccess = bool.FalseString };

            if (args == null)
                throw new ArgumentNullException();
            if (string.IsNullOrWhiteSpace(args.ActorKey))
                throw new ArgumentException("ActorKeyが取得できません");
            if (string.IsNullOrWhiteSpace(args.AuthorKey))
                throw new ArgumentException("AuthorKeyが取得できません");
            if (string.IsNullOrWhiteSpace(args.Executor))
                throw new ArgumentException("Executorが指定されてません");
            if (int.TryParse(args.LinkageSystemNo, out int _) == false)
                throw new ArgumentException("LinkageSystemNoが不正です");

            Guid accountKey = args.ActorKey.ToValueType<Guid>();

            // TODO
            try
            {
                // 退会処理実施
                bool writerResults = ProcessWithdraw(args.AuthorKey.TryToValueType(Guid.Empty), args.ActorKey.TryToValueType(Guid.Empty), args.LinkageSystemNo.TryToValueType(int.MinValue), args.UnsubscribeItemNo.TryToValueType(byte.MinValue), args.Comment);

                if (writerResults == false)
                    QoAccessLog.WriteErrorLog("退会処理に失敗しました。", args.Executor.TryToValueType(Guid.Empty));
                else
                {
                    result.IsSuccess = writerResults.ToString();
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
            }

            return result;
        }

        /// <summary>
        /// メールアドレス変更要求を処理します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountChangeMailRequestApiResults ChangeMailRequest(QoAccountChangeMailRequestApiArgs args)
        {
            QoAccountChangeMailRequestApiResults result = new QoAccountChangeMailRequestApiResults() { IsSuccess = bool.FalseString };

            Guid accountKey = args.ActorKey.ToValueType<Guid>();

            if (accountKey != Guid.Empty && !string.IsNullOrWhiteSpace(args.Mail))
            {
                //if (PasswordManagementHelper.IsUsedMailAddress(args.Mail))
                //    // メールアドレス使用済み
                //    result.Result = ApiResultWorker.Build(QoApiResultCodeTypeEnum.UsedMailAddress);
                //else
                //{
                    string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
                    string url = string.Format("{0}?p={3}&d={1}&k={2}", 
                        QoApiConfiguration.MailAddressChangeUrl,
                        args.Mail.Trim().ToEncrypedReference(), 
                        accountKey.ToEncrypedReference(DateTime.Now.AddDays(1)), 
                        p );

                    // API実行が成功したら、URLを生成し、メール送信
                    AccountWorker.SendMailAddressChangeMail(args.Mail, url, p);

                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    
                //}
            }

            return result;
        }

        /// <summary>
        /// 忘れたIDを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountIdForgetReadApiResults IdForgetRead(QoAccountIdForgetReadApiArgs args)
        {
            var results = new QoAccountIdForgetReadApiResults() { IsSuccess = bool.FalseString };

            //引数チェック
            string mailAddress = args.MailAddress;
            string familyName = args.FamilyName;
            string givenName = args.GivenName;
            if (mailAddress == string.Empty || familyName == string.Empty || givenName == string.Empty ||
                !DateTime.TryParse(args.Birthday, out DateTime birthday) || !byte.TryParse(args.Sex, out byte sex))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError);
                return results;
            }

            //ID検索
            Tuple<int, string> ret = GetIdForget(mailAddress, familyName, givenName, birthday, sex);

            //取得結果が１件の場合正常
            if (ret.Item1 == 1 && !string.IsNullOrEmpty(ret.Item2))
            {
                results.IsSuccess = bool.TrueString;
                // UserIDはメールでのみ通知
            }
            //取得結果が2件以上の場合準異常
            else if (ret.Item1 >= 2)
            {
                results.IsSuccess = bool.FalseString;
                results.Result = new QoApiResultItem() { Code = $"{(int)QoApiResultCodeTypeEnum.UserIdForgetMultipleResults:D4}", Detail = string.Format($"左記の条件にて複数の対象が存在しました。【条件】メールアドレス:{mailAddress},漢字姓:{familyName},漢字名:{givenName},生年月日:{birthday},性別:{sex}") };
            }
            //取得結果が0件の場合準異常
            else if (ret.Item1 == 0)
            {
                results.IsSuccess = bool.FalseString;
                results.Result = new QoApiResultItem() { Code = $"{(int)QoApiResultCodeTypeEnum.UserIdForgetNotFound:D4}", Detail = string.Format($"左記の条件にて対象が存在しませんでした。【条件】メールアドレス:{mailAddress},漢字姓:{familyName},漢字名:{givenName},生年月日:{birthday},性別:{sex}") };
            }
            else
            {
                results.IsSuccess = bool.FalseString;
            }

            //ID検索が正常な場合メール送信
            if (results.IsSuccess == bool.TrueString)
            {
                string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
                AccountWorker.SendForgetIdMail(mailAddress, ret.Item2, p);
            }

            return results;

        }

        #endregion
    }
}
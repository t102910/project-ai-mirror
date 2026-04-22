using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    public class LinkageRegisterWorkerBase: LinkagePatientCardWorkerBase
    {
        ConcurrentDictionary<string, byte> _existingIds = new ConcurrentDictionary<string, byte>();
        // ID使用文字列 0-9, A-H, J-N, P-Y
        static readonly char[] IdChars = "0123456789ABCDEFGHJKLMNPQRSTUVWXY".ToCharArray();

        protected ILinkageRepository _linkageRepository;
        protected IIdentityApiRepository _identityApi;
        protected IAccountRepository _accountRepository;

        public LinkageRegisterWorkerBase(
            ILinkageRepository linkageRepository,
            IIdentityApiRepository identityApiRepository,
            IAccountRepository accountRepository):base(linkageRepository,accountRepository)
        {
            _linkageRepository = linkageRepository;
            _identityApi = identityApiRepository;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// 退会後規定時間以内の登録か検証する
        /// </summary>
        /// <param name="linkageSystemId"></param>
        /// <param name="facilityKey"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool CheckRegisterInterval(string linkageSystemId, Guid facilityKey, QoApiResultsBase results)
        {

            // 99999で始まる8桁は検証用IDとしてスルーする
            // TODO: 検証用IDは今後パターンが増える可能性あり
            if (Regex.IsMatch(linkageSystemId, @"99999.{3}"))
            {
                return true;
            }

            try
            {
                // QH_LINKAGE_DATの更新日時を取得
                var updated = _linkageRepository.GetLinkageUpdated(facilityKey, linkageSystemId);

                //規定時間内かチェック
                if (updated.AddMinutes(QoApiConfiguration.NewUserRegisterInterval) > DateTime.Now)
                {
                    // 2999: 退会後規定時間以内のエラー Detailに規定の分数が入る
                    results.Result = new QoApiResultItem
                    {
                        Code = $"{(int)QoApiResultCodeTypeEnum.RegisterIntervalError:D4}",
                        Detail = QoApiConfiguration.NewUserRegisterInterval.ToString()
                    };
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex);
                return false;
            }
        }

        /// <summary>
        /// 重複エラーがなくなるまで規定回数、登録処理を試みる
        /// 10回重複したらエラーとして返す
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="userPass"></param>
        /// <param name="familyName"></param>
        /// <param name="givenName"></param>
        /// <param name="familyKananame"></param>
        /// <param name="givenKanaName"></param>
        /// <param name="sexType"></param>
        /// <param name="birthDay"></param>
        /// <param name="mailAddress"></param>
        /// <param name="retryCnt"></param>
        /// <returns></returns>
        protected (bool isSuccess, QoApiResultItem apiResult, string userId, Guid accountKey) TryRegisterUser(int linkageSystemNo, string linkageSystemId, string userPass,
            string familyName, string givenName, string familyKananame, string givenKanaName,
            string sexType, string birthDay, string mailAddress, int retryCnt = 0)
        {
            try
            {
                if (retryCnt >= 10)
                {
                    return (false, QoApiResult.Build(QoApiResultCodeTypeEnum.UserIdDuplicate), string.Empty, Guid.Empty);
                }
                // UserIdの生成 英数2~4桁+英数10桁
                var prefix = GetAutoUserIdPrefix(linkageSystemNo);
                var userID = $"{prefix}{GenerateUserId()}";

                IdentityRegisterApiResults results;
                // 連携システムを利用しないアプリの場合(Kagamino,お薬手帳)
                // 連携なしでアカウント登録する
                if(linkageSystemNo.IsNoLinkageSystem())
                {
                    results = _identityApi.ExecuteRegisterWriteApi(userID, userPass, familyName, givenName, familyKananame, givenKanaName, sexType, birthDay, mailAddress);
                }
                // 連携システム対応アプリは連携ユーザーとしてアカウント登録を行う
                else
                {
                    results = _identityApi.ExecuteLinkageUserRegisterApi(linkageSystemNo.ToString(), linkageSystemId, userID, userPass, familyName, givenName, familyKananame, givenKanaName, sexType, birthDay, mailAddress);
                }

                if (!results.IsSuccess)
                {
                    if (results.ErrorList.Contains("UserId"))
                    {
                        return TryRegisterUser(linkageSystemNo, linkageSystemId, userPass, familyName, givenName, familyKananame, givenKanaName, sexType, birthDay, mailAddress, ++retryCnt);
                    }

                    return (false, QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, string.Join(",", results.ErrorList)), string.Empty, Guid.Empty);
                }

                var retAccountKey = results.AccountKey.TryToValueType(Guid.Empty);
                if (retAccountKey == Guid.Empty)
                {
                    return (false, QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"本登録処理でアカウントキーが返されませんでした。{string.Join(",", results.ErrorList)}"), string.Empty, Guid.Empty);
                }

                return (true, QoApiResult.Build(QoApiResultCodeTypeEnum.Success), userID, retAccountKey);
            }
            catch (Exception ex)
            {
                return (false, QoApiResult.Build(ex), string.Empty, Guid.Empty);
            }
        }        

        // 同一登録情報のユーザー存在チェック
        protected bool CheckDuplicateUser(string mail, string familyName, string givenName, DateTime birthDate, QsDbSexTypeEnum sex, QoApiResultsBase results)
        {
            try
            {
                (var count, var userId) = _accountRepository.GetRegisteredAccountId(mail, familyName, givenName, birthDate, (byte)sex);
                if (count < 0)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "同一登録情報重複チェックでエラーが発生しました。");
                    return false;
                }
                else if (count >= 1)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.UserInfoDuplicate, "同一登録情報のユーザーが既に存在します。");
                    return false;
                }

                // 同一登録情報のユーザーが存在しないのでOK
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "同一登録情報重複チェックで例外が発生しました。");
                return false;
            }
        }

        // 家族追加処理
        protected bool AddFamilyUser(Guid accountKey, string executorName, QoApiAccountFamilyInputItem familyAccount, QoApiResultsBase apiResults, out Guid familyAccountKey)
        {
            familyAccountKey = Guid.Empty;
            try
            {

                var results = _identityApi.ExecuteAccountConnectFamilyAccountEditWriteApi(accountKey, executorName, familyAccount, familyAccount.Sex, familyAccount.Birthday);

                if (results == null || results.IsSuccess != bool.TrueString)
                {
                    apiResults.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "家族アカウントの追加に失敗しました。");
                    return false;
                }

                apiResults.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                familyAccountKey = results.Accountkey;
                return true;

            }
            catch (Exception ex)
            {
                apiResults.Result = QoApiResult.Build(ex);
                return false;
            }
        }

        // 失敗時に一貫性を保つためにアカウントを削除する
        protected void WithdrawAccount(QoApiArgsBase args, int linkageSystemNo, Guid accountKey)
        {
            var executor = args.Executor.TryToValueType(Guid.Empty);
            var authorKey = args.AuthorKey.TryToValueType(Guid.Empty);
            // 退会実行（成否は問わない）
            _linkageRepository.WithdrawAccountOnRegister(executor, authorKey, accountKey, linkageSystemNo, 255, "新規連携ユーザー登録処理中での退会処理");
        }

        /// <summary>
        /// ニックネームを更新する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="nickName"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool TryUpdateNickName(Guid accountKey, string nickName, QoApiResultsBase results)
        {
            try
            {
                var entity = _accountRepository.ReadAccountIndexDat(accountKey);
                entity.NICKNAME = nickName;

                _accountRepository.UpdateIndexEntity(entity);

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ニックネームの登録に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// Push通知用IDを更新する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool TryUpdateNotificationId(Guid accountKey, int linkageSystemNo, QoApiResultsBase results)
        {
            if (linkageSystemNo.IsNoLinkageSystem())
            {
                // Kagamino/お薬手帳など連携システムなしの場合は更新不要
                return true;
            }
            try
            {                
                var entity = _linkageRepository.ReadEntity(accountKey, linkageSystemNo);
                // Push通知用のIDをアカウントキーとする
                entity.LINKAGESYSTEMID = accountKey.ToString("N");
                _linkageRepository.UpdateEntity(entity);

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "Push通知IDの更新に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// Executor を暗号化して返す
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        internal static string GetEncryptExecutor(string executor)
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
        /// ユーザーID(10桁部分)を生成
        /// 数字とアルファベット大文字(IOZを除く23文字)で構成される。
        /// 十分な数のIDに対応できるがIDが1190万あたりを越えると衝突しやすくなるため
        /// そのあたりでDBのテーブルのIDの重複チェックを行った方が良い
        /// IdentityApiの方で重複チェックは行っているが通信コストがかかる
        /// </summary>
        /// <returns></returns>
        public string GenerateUserId()
        {
            string newId;
            do
            {
                newId = GenerateRandomAlphanumericString(10);
            }
            while (!_existingIds.TryAdd(newId, 0));

            return newId;
        }

        private string GenerateRandomAlphanumericString(int length)
        {
            char[] buffer = new char[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < length; i++)
                {
                    byte[] randomNumber = new byte[1];
                    rng.GetBytes(randomNumber);

                    // Get a random index into our array of valid characters
                    int index = randomNumber[0] % IdChars.Length;
                    buffer[i] = IdChars[index];
                }
            }

            return new string(buffer);
        }

        private static string GetAutoUserIdPrefix(int linkageSystemNo)
        {
            switch (linkageSystemNo)
            {
                // TIS
                case QoLinkage.TIS_LINKAGE_SYSTEM_NO:
                    return "TS";
                // 医療ナビ
                case QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO:
                    return "MN";
                // 健康DIARY
                case QoLinkage.HEALTHDIARY_LINKAGE_SYSTEM_NO:
                    return "HD";
                // 心拍見守りアプリ(JOTO扱いとする)
                case QoLinkage.JOTO_HEARTMONITOR_SYSTEM_NO:
                // JOTOネイティブ
                case QoLinkage.JOTO_LINKAGE_SYSTEM_NO:
                    return "JOTO";
                // MEIナビ
                case QoLinkage.MEINAVI_LINKAGE_SYSTEM_NO:
                    return "MEI";
                
                // その他
                default:
                    return "QQ";
            }
        }
    }
}
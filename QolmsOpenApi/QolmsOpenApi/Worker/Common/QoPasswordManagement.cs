using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    internal sealed class QoPasswordManagement
    {


        /// <summary>
        /// 仮パスワードの長さを表します。
        /// </summary>
        /// <remarks></remarks>
        private static readonly int TemporaryPasswordLength = 15;



        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private QoPasswordManagement()
        {
        }
     

        public static (string mailaddress,string userid) GetMailAddress(Guid accountKey)
        {
            QhPasswordManagementEntityReaderArgs args = new QhPasswordManagementEntityReaderArgs() { Data = new List<QH_PASSWORDMANAGEMENT_DAT> { new QH_PASSWORDMANAGEMENT_DAT() { ACCOUNTKEY = accountKey } } };
            QhPasswordManagementEntityReaderResults result = QsDbManager.Read(new QhPasswordManagementEntityReader(), args);
            if(result.IsSuccess && result.Result != null && result.Result.Count == 1)
            {
                var entity = result.Result.First();
                string mailaddress= entity.PASSWORDRECOVERYMAILADDRESS;
                string userId = entity.USERID;
                if (!string.IsNullOrWhiteSpace(userId))
                {                 
                    using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                    {
                        try
                        {
                            return (string.IsNullOrEmpty(mailaddress)? string.Empty : crypt.DecryptString(mailaddress), crypt.DecryptString(userId));
                        }
                        catch (Exception ex)
                        {
                            QoAccessLog.WriteErrorLog(ex, accountKey);
                        }                   
                    }
                }
            }
            return (string.Empty,string.Empty);
        }
        /// <summary>
        /// パスワードの有効性を判定します。
        /// </summary>
        /// <typeparam name="TEntity">パスワード管理データテーブルエンティティの型。</typeparam>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="password">
        /// パスワード。
        /// この値は <paramref name="passwordHash" /> より優先されます。
        /// </param>
        /// <param name="passwordHash">
        /// パスワードハッシュ。
        /// <paramref name="password" /> が指定されている場合は、
        /// そちらが優先されます。
        /// </param>
        /// <param name="cryptor"></param>
        /// <param name="refAccountKey">アカウントキーが格納される変数。</param>
        /// <param name="refPasswordHash">パスワードハッシュが格納される変数。</param>
        /// <returns>
        /// 有効なら True、
        /// 無効なら False。
        /// </returns>
        /// <remarks></remarks>
        public static bool CheckPassword<TEntity>(string userId, string password, string passwordHash, QsCrypt cryptor, ref Guid refAccountKey, ref string refPasswordHash) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            refAccountKey = Guid.Empty;
            refPasswordHash = string.Empty;

            // Dim result As Boolean = False
            // Dim entity As TEntity = PasswordManagementHelper.ReadPasswordManagementEntity(Of TEntity)(Guid.Empty, userId)

            // If entity IsNot Nothing AndAlso entity.ACCOUNTKEY <> Guid.Empty Then
            // Dim userPassword As String = String.Empty

            // Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            // ' 復号化
            // If Not String.IsNullOrWhiteSpace(entity.USERPASSWORD) Then userPassword = crypt.DecryptString(entity.USERPASSWORD)
            // End Using

            // refAccountKey = entity.ACCOUNTKEY
            // refPasswordHash = PasswordManagementHelper.CreatePasswordHashString(userPassword)

            // If Not String.IsNullOrWhiteSpace(password) Then
            // ' パスワードでチェック
            // result = (userPassword.CompareTo(password) = 0)
            // ElseIf Not String.IsNullOrWhiteSpace(passwordHash) Then
            // ' パスワードハッシュでチェック
            // result = (refPasswordHash.CompareTo(passwordHash) = 0)
            // End If
            // End If

            bool result = false;

            // Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
            // ' ユーザー ID は暗号化が必要
            // Dim entity As TEntity = PasswordManagementHelper.ReadPasswordManagementEntity(Of TEntity)(userId, crypt)

            // If entity IsNot Nothing _
            // AndAlso entity.ACCOUNTKEY <> Guid.Empty _
            // AndAlso Not String.IsNullOrWhiteSpace(entity.USERID) _
            // AndAlso Not String.IsNullOrWhiteSpace(entity.USERPASSWORD) Then

            // ' 復号化
            // Dim userPassword As String = crypt.DecryptString(entity.USERPASSWORD)

            // refAccountKey = entity.ACCOUNTKEY
            // refPasswordHash = PasswordManagementHelper.CreatePasswordHashString(userPassword)

            // If Not String.IsNullOrWhiteSpace(password) Then
            // ' パスワードでチェック
            // result = (userPassword.CompareTo(password) = 0)
            // ElseIf Not String.IsNullOrWhiteSpace(passwordHash) Then
            // ' パスワードハッシュでチェック
            // result = (refPasswordHash.CompareTo(passwordHash) = 0)
            // End If
            // End If
            // End Using

            // ユーザー ID は暗号化が必要
            TEntity entity = new DbPasswordManagementReaderCore().ReadPasswordManagementEntity<TEntity>(userId, cryptor);

            if (entity != null && entity.ACCOUNTKEY != Guid.Empty && !string.IsNullOrWhiteSpace(entity.USERID) && !string.IsNullOrWhiteSpace(entity.USERPASSWORD))
            {

                // 復号化
                string userPassword = cryptor.DecryptString(entity.USERPASSWORD);

                refAccountKey = entity.ACCOUNTKEY;
                refPasswordHash = QoPasswordManagement.CreatePasswordHashString(userPassword);

                if (!string.IsNullOrWhiteSpace(password))
                    // パスワードでチェック
                    result = (userPassword.CompareTo(password) == 0);
                else if (!string.IsNullOrWhiteSpace(passwordHash))
                    // パスワードハッシュでチェック
                    result = (refPasswordHash.CompareTo(passwordHash) == 0);
            }

            return result;
        }

        /// <summary>
        /// ユーザーID、メールアドレスの有効性を判定します。
        /// （パスワードリセットに使用）
        /// </summary>
        /// <typeparam name="TEntity">パスワード管理データテーブルエンティティの型。</typeparam>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="mailAddress">メールアドレス。</param>
        /// <param name="cryptor">暗号化/復号化 機能クラス。</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Guid CheckUserIdMailAddress<TEntity>(string userId, string mailAddress, QsCrypt cryptor) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            Guid result = Guid.Empty;

            TEntity entity = new DbPasswordManagementReaderCore().ReadPasswordManagementEntity<TEntity>(userId, cryptor);

            if (entity != null && entity.ACCOUNTKEY != Guid.Empty && !string.IsNullOrWhiteSpace(entity.USERID) && !string.IsNullOrWhiteSpace(entity.PASSWORDRECOVERYMAILADDRESS))
            {

                // ユーザーIDとメールアドレスが完全一致であればパスワードリセットして良い
                if (cryptor.EncryptString(userId).CompareTo(entity.USERID) == 0 && cryptor.EncryptString(mailAddress).CompareTo(entity.PASSWORDRECOVERYMAILADDRESS) == 0)
                    result = entity.ACCOUNTKEY;
            }

            return result;
        }

        /// <summary>
        /// 仮パスワードを生成します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string CreateTemporaryPassword()
        {
            return System.Web.Security.Membership.GeneratePassword(QoPasswordManagement.TemporaryPasswordLength, 0);
        }

        /// <summary>
        /// パスワードハッシュを生成します。
        /// </summary>
        /// <param name="password">パスワード。</param>
        /// <returns>
        /// パスワードハッシュ。
        /// </returns>
        /// <remarks></remarks>
        public static string CreatePasswordHashString(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException();

            StringBuilder result = new StringBuilder();

            using (SHA256Managed hash = new SHA256Managed())
            {
                hash.ComputeHash(Encoding.UTF8.GetBytes(password)).ToList().ForEach(i => result.Append(i.ToString("x2")));
            }

            return result.ToString();
        }

        /// <summary>
        /// 入力されたパスワードをチェックします。
        /// </summary>
        /// <param name="authorKey">所有者アカウント キー。</param>
        /// <param name="newPassword">新パスワード。</param>
        /// <param name="refErrorMessage">エラー メッセージが格納される変数。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        /// <remarks></remarks>
        public static bool CheckNewPassword<TEntity>(Guid authorKey, string newPassword, ref string refErrorMessage) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            refErrorMessage = "パスワードが無効です。";

            bool result = false;

            // DB から取得
            DbAccountInformationPasswordReader<TEntity> reader = new DbAccountInformationPasswordReader<TEntity>();
            DbAccountInformationPasswordReaderArgs readerArgs = new DbAccountInformationPasswordReaderArgs() { AuthorKey = authorKey };
            DbAccountInformationPasswordReaderResults readerResults = QsDbManager.Read(reader, readerArgs);
            if (readerResults.IsSuccess)
            {
                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    string userId = "";
                    try
                    {
                        userId = string.IsNullOrWhiteSpace(readerResults.UserId)? readerResults.UserId: crypt.DecryptString(readerResults.UserId);
                    }
                    catch (Exception ex)
                    {
                        QoAccessLog.WriteErrorLog(ex, authorKey);
                    }
                    if (userId.Trim() == newPassword.Trim())
                    {
                        refErrorMessage = "UserIDとパスワードが同じです。";
                        return false;
                    }
                }
                switch (true)
                {
                    case bool _ when string.IsNullOrWhiteSpace(readerResults.Password):
                        refErrorMessage = "登録済みパスワードが空白です。";
                        break;
                    case bool _ when readerResults.LastUpdatePasswordDate == DateTime.MinValue:
                        refErrorMessage = "登録済みパスワードの更新日時が不正です。";
                        break;
                   case bool _ when string.IsNullOrWhiteSpace(newPassword):
                        refErrorMessage = "新しいパスワードが空白です。";
                        break;
                   case bool _ when readerResults.Password.CompareTo(newPassword) == 0:
                        refErrorMessage = "現在のパスワードと新しいパスワードが同じです。";
                        break;
                   default:
                        refErrorMessage = string.Empty;
                        result = true;
                        break;
                }
            }
            

            return result;
        }

        /// <summary>
        /// パスワードを変更します。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="accountKey"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool EditPassword<TEntity>(Guid accountKey, string newPassword) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            bool result = false;
            DbAccountInformationPasswordWriter<TEntity> writer = new DbAccountInformationPasswordWriter<TEntity>();
            DbAccountInformationPasswordWriterArgs writerArgs = new DbAccountInformationPasswordWriterArgs()
            {
                AuthorKey = accountKey,
                Password = newPassword
            };
            DbAccountInformationPasswordWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

            if (writerResults != null)
            {
                result = writerResults.IsSuccess && writerResults.Result == 1;
            }

            return result;
        }

        /// <summary>
        /// パスワードリカバリ用メールアドレスを変更します。
        /// ※セキュリティ質問は使用しません（OpenApi専用処理）。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="accountKey"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool EditMailAddress<TEntity>(Guid accountKey, string mailAddress) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            bool result = false;

            // パスワードリカバリ用メールアドレスのみ更新（セキュリティ質問を使用しない）
            AccountInformationMailAddressWriter<TEntity> writer = new AccountInformationMailAddressWriter<TEntity>();
            AccountInformationMailAddressWriterArgs writerArgs = new AccountInformationMailAddressWriterArgs()
            {
                AuthorKey = accountKey,
                PasswordRecoveryMailAddress = mailAddress
            };
            AccountInformationMailAddressWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

            if (writerResults != null)
                result = writerResults.IsSuccess && writerResults.Result == 1;
            return result;
        }

        /*public static bool IsUsedMailAddress(string mailAddress)
        {
            bool result = true; // デフォルトは使用済み(重複あり)=失敗扱い
            if (string.IsNullOrWhiteSpace(mailAddress))
                throw new ArgumentNullException("mailAddress", "メールアドレスが未指定です。");

            bool isCheckUsedMailAddress = false;
            try
            {
                isCheckUsedMailAddress = bool.Parse(ConfigurationManager.AppSettings("CheckUsedMailAddress"));
            }
            catch (Exception ex)
            {
            }

            if (isCheckUsedMailAddress)
            {
                // メールアドレス使用済みチェックあり
                string encryptedMailAddress = new QsCrypt(QsCryptTypeEnum.QolmsSystem).EncryptString(mailAddress);
                UsedMailAddressReader reader = new UsedMailAddressReader();
                UsedMailAddressReaderArgs readerArgs = new UsedMailAddressReaderArgs() { PasswordRecoveryMailAddress = encryptedMailAddress };
                UsedMailAddressReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

                if (readerResults != null && readerResults.IsSuccess)
                    result = readerResults.IsUsedMailAddress;
                    
            }
            else
                // 設定なし、重複OK
                result = false;

            return result;
        }*/
    }

}
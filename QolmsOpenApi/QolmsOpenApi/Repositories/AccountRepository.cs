using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// アカウント情報への入出力インターフェース
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// アカウントに紐づくEmailを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        string GetAccountEmail(Guid accountKey);

        /// <summary>
        /// QH_ACCOUNTINDEX_DATからレコードを取得する
        /// 暗号化部分は復号化して返される
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_ACCOUNTINDEX_DAT ReadAccountIndexDat(Guid accountKey);

        /// <summary>
        /// QH_ACCOUNTINDEX_DATのレコードを追加します。
        /// 暗号化対象は暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void InsertIndexEntity(QH_ACCOUNTINDEX_DAT entity);

        /// <summary>
        /// QH_ACCOUNTINDEX_DATのレコードを更新します。
        /// 暗号化対象は暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateIndexEntity(QH_ACCOUNTINDEX_DAT entity);

        /// <summary>
        /// メールアドレス、姓、名、生年月日、性別 から同一ユーザーと推測される
        /// ユーザーのUserIdを取得する。
        /// 取得件数と取得UserIdを返却。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="familyName"></param>
        /// <param name="givenName"></param>
        /// <param name="birthday"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        (int count, string accountId) GetRegisteredAccountId(string mailAddress, string familyName, string givenName, DateTime birthday, byte sex);

        /// <summary>
        /// QH_ACCOUNT_MSTから主キーでレコードを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_ACCOUNT_MST ReadMasterEntity(Guid accountKey);

        /// <summary>
        /// QH_ACCOUNT_MSTにレコードを追加する
        /// </summary>
        /// <param name="entity"></param>
        void InsertMasterEntity(QH_ACCOUNT_MST entity);


        /// <summary>
        /// 子アカウントキーから親アカウントのQH_ACCOUNT_MSTのレコードを取得する
        /// </summary>
        /// <param name="childAccountKey"></param>
        /// <returns></returns>
        QH_ACCOUNT_MST ReadParentMasterEntity(Guid childAccountKey);

        /// <summary>
        /// アカウントキーが子であれば親のレコードを、親であれば本人のレコードを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_ACCOUNT_MST ReadParentOrSelfMasterEntiry(Guid accountKey);

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを1件取得します。
        /// 暗号化が解除されたデータを返します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_ACCOUNTPHONE_MST ReadPhoneEntity(Guid accountKey);

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを電話番号で抽出し1件取得します。
        /// 暗号化が解除されたデータを返します。
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        QH_ACCOUNTPHONE_MST ReadPhoneEntityByNumber(string phoneNumber);

        /// <summary>
        ///  QH_ACCOUNTPHONE_MSTにレコードを挿入します。
        ///  電話番号は暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void InsertPhoneEntity(QH_ACCOUNTPHONE_MST entity);
        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを更新します。
        /// 電話番号は暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void UpdatePhoneEntity(QH_ACCOUNTPHONE_MST entity);
        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        void DeletePhoneEntity(QH_ACCOUNTPHONE_MST entity);
        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを物理削除します。
        /// </summary>
        /// <param name="accountKey"></param>
        void PhysicalDeletePhoneEntity(Guid accountKey);

        /// <summary>
        /// QH_PASSWORDMANAGEMENT_DATのレコードを1件取得します。
        /// 暗号化が解除されたデータを返します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_PASSWORDMANAGEMENT_DAT ReadPasswordManagementEntity(Guid accountKey);
    }

    /// <summary>
    /// アカウント情報への入出力実装
    /// </summary>
    public class AccountRepository: QsDbReaderBase, IAccountRepository
    {
        /// <summary>
        /// アカウントに紐づくEmailを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public string GetAccountEmail(Guid accountKey)
        {
            var entity = new DbPasswordManagementReaderCore().ReadPasswordManagementEntity<QH_PASSWORDMANAGEMENT_DAT>(accountKey);
            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                if (entity != null && entity.IsKeysValid())
                {
                    return string.IsNullOrWhiteSpace(entity.PASSWORDRECOVERYMAILADDRESS) ? string.Empty : crypt.DecryptString(entity.PASSWORDRECOVERYMAILADDRESS);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// QH_ACCOUNTINDEX_DATからレコードを取得する
        /// 暗号化部分は復号化して返される
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public QH_ACCOUNTINDEX_DAT ReadAccountIndexDat(Guid accountKey)
        {
            var readerArgs = new QhAccountIndexEntityReaderArgs() 
            {
                Data = new List<QH_ACCOUNTINDEX_DAT>() 
                { 
                    new QH_ACCOUNTINDEX_DAT() 
                    { 
                        ACCOUNTKEY = accountKey 
                    } 
                } 
            };
            var readerResults = QsDbManager.Read(new QhAccountIndexEntityReader(), readerArgs);
            if(!readerResults.IsSuccess || readerResults.Result == null || readerResults.Result.Count > 1)
            {
                throw new InvalidOperationException();
            }

            if(readerResults.Result.Count == 0)
            {
                return null;
            }

            var entity = readerResults.Result.First();

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                entity.FAMILYNAME = crypt.TryDecrypt(entity.FAMILYNAME);
                entity.GIVENNAME = crypt.TryDecrypt(entity.GIVENNAME);
                entity.FAMILYKANANAME = crypt.TryDecrypt(entity.FAMILYKANANAME);
                entity.GIVENKANANAME = crypt.TryDecrypt(entity.GIVENKANANAME);
                entity.NICKNAME = crypt.TryDecrypt(entity.NICKNAME);
            }

            return entity;
        }

        /// <inheritdoc/>
        public void InsertIndexEntity(QH_ACCOUNTINDEX_DAT entity)
        {           
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                entity.FAMILYNAME = crypt.TryEncrypt(entity.FAMILYNAME);
                entity.GIVENNAME = crypt.TryEncrypt(entity.GIVENNAME);
                entity.FAMILYKANANAME = crypt.TryEncrypt(entity.FAMILYKANANAME);
                entity.GIVENKANANAME = crypt.TryEncrypt(entity.GIVENKANANAME);
                entity.NICKNAME = crypt.TryEncrypt(entity.NICKNAME);
            }

            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var now = DateTime.Now;
            entity.CREATEDDATE = now;
            entity.UPDATEDDATE = now;

            var args = new QhAccountIndexEntityWriterArgs
            {
                Data = new List<QH_ACCOUNTINDEX_DAT> { entity }
            };
            var result = new QhAccountIndexEntityWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_ACCOUNTINDEX_DAT)}の追加に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void UpdateIndexEntity(QH_ACCOUNTINDEX_DAT entity)
        {
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                entity.FAMILYNAME = crypt.TryEncrypt(entity.FAMILYNAME);
                entity.GIVENNAME = crypt.TryEncrypt(entity.GIVENNAME);
                entity.FAMILYKANANAME = crypt.TryEncrypt(entity.FAMILYKANANAME);
                entity.GIVENKANANAME = crypt.TryEncrypt(entity.GIVENKANANAME);
                entity.NICKNAME = crypt.TryEncrypt(entity.NICKNAME);
            }               
            
            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new QhAccountIndexEntityWriterArgs
            {
                Data = new List<QH_ACCOUNTINDEX_DAT> { entity }
            };
            var result = new QhAccountIndexEntityWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_ACCOUNTINDEX_DAT)}の更新に失敗しました。");
            }
        }

        /// <summary>
        /// メールアドレス、姓、名、生年月日、性別 から同一ユーザーと推測される
        /// ユーザーのUserIdを取得する。
        /// 取得件数と取得UserIdを返却。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="familyName"></param>
        /// <param name="givenName"></param>
        /// <param name="birthday"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public (int count, string accountId) GetRegisteredAccountId(string mailAddress, string familyName, string givenName, DateTime birthday, byte sex)
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
                    // 取得件数1件の場合はUserIDとともに返却（登録済とみなせる）
                    return (readerResults.AccountId.Count, crypt.DecryptString(readerResults.AccountId.First()));
                }
            }
            else if (readerResults.IsSuccess && readerResults.Result != null)
            {
                // 取得件数0件または2件以上はカウントだけを返却（未登録または複数エントリあり）
                return (readerResults.AccountId.Count, string.Empty);
            }
            // 上記以外 DBエラー等 異常扱い
            return (int.MinValue, string.Empty);

        }

        /// <summary>
        /// QH_ACCOUNT_MSTから主キーでレコードを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public QH_ACCOUNT_MST ReadMasterEntity(Guid accountKey)
        {
            var entity = new QH_ACCOUNT_MST
            {
                ACCOUNTKEY = accountKey,
                DELETEFLAG = false,              
            };
            var reader = new QhAccountEntityReader();
            var readerArgs = new QhAccountEntityReaderArgs 
            {
                Data = new List<QH_ACCOUNT_MST>() { entity }                 
            };
            var readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults.IsSuccess)
            {
                return readerResults.Result.FirstOrDefault(x => !x.DELETEFLAG);
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public void InsertMasterEntity(QH_ACCOUNT_MST entity)
        {
            var now = DateTime.Now;
            entity.ACCOUNTKEY = Guid.NewGuid();
            entity.DataState = QsDbEntityStateTypeEnum.Added;
            entity.CREATEDDATE = now;
            entity.UPDATEDDATE = now;

            var args = new QhAccountEntityWriterArgs
            {
                Data = new List<QH_ACCOUNT_MST> { entity }
            };
            var result = new QhAccountEntityWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_ACCOUNTINDEX_DAT)}の追加に失敗しました。");
            }
        }

        /// <summary>
        /// 子アカウントキーから親アカウントのQH_ACCOUNT_MSTのレコードを取得する
        /// </summary>
        /// <param name="childAccountKey"></param>
        /// <returns></returns>
        public QH_ACCOUNT_MST ReadParentMasterEntity(Guid childAccountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNT_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", childAccountKey),
                };

                var sql = $@"
                    SELECT *
                    FROM QH_ACCOUNT_MST
                    WHERE ACCOUNTKEY = (
                        SELECT RELATIONACCOUNTKEY
                        FROM QH_ACCOUNTRELATION_DAT
                        WHERE ACCOUNTKEY = @p1
                        AND RELATIONDIRECTIONTYPE = 2
                        AND DELETEFLAG = 0
                    )
                    AND DELETEFLAG = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_ACCOUNT_MST>(con, null, sql, parameters);

                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// アカウントキーが子であれば親のレコードを、親であれば本人のレコードを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public QH_ACCOUNT_MST ReadParentOrSelfMasterEntiry(Guid accountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNT_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", accountKey),
                };

                var sql = $@"
                    SELECT *
                    FROM QH_ACCOUNT_MST
                    WHERE ACCOUNTKEY = 
                    (
                        SELECT RELATIONACCOUNTKEY
                        FROM QH_ACCOUNTRELATION_DAT
                        WHERE 
                        ACCOUNTKEY = @p1
                        AND RELATIONDIRECTIONTYPE = 2
                        AND RELATIONTYPE = 1
                        AND DELETEFLAG = 0
                    )
                    AND PRIVATEACCOUNTFLAG = 0
                    AND DELETEFLAG = 0

                    UNION ALL

                    SELECT *
                    FROM QH_ACCOUNT_MST
                    WHERE ACCOUNTKEY = @p1
                    AND PRIVATEACCOUNTFLAG = 0
                    AND DELETEFLAG = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_ACCOUNT_MST>(con, null, sql, parameters);

                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを1件取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public QH_ACCOUNTPHONE_MST ReadPhoneEntity(Guid accountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNTPHONE_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_ACCOUNTPHONE_MST)}
                    WHERE {nameof(QH_ACCOUNTPHONE_MST.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_ACCOUNTPHONE_MST.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_ACCOUNTPHONE_MST>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                if (entity != null)
                {
                    entity.PHONENUMBER = entity.PHONENUMBER.TryDecrypt();
                }      

                return entity;
            }
        }

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを電話番号で抽出し1件取得します。
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public QH_ACCOUNTPHONE_MST ReadPhoneEntityByNumber(string phoneNumber)
        {
            using(var con = QsDbManager.CreateDbConnection<QH_ACCOUNTPHONE_MST>())
            {
                var encryptedNumber = phoneNumber.TryEncrypt();

                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", encryptedNumber),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_ACCOUNTPHONE_MST)}
                    WHERE {nameof(QH_ACCOUNTPHONE_MST.PHONENUMBER)} = @p1
                    AND   {nameof(QH_ACCOUNTPHONE_MST.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_ACCOUNTPHONE_MST>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                if (entity != null)
                {
                    entity.PHONENUMBER = phoneNumber;
                }

                return entity;
            }
        }

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        public void InsertPhoneEntity(QH_ACCOUNTPHONE_MST entity)
        {
            entity.PHONENUMBER = entity.PHONENUMBER.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new AccountPhoneWriterArgs { Entity = entity };
            var result = new AccountPhoneWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_ACCOUNTPHONE_MST)}の挿入に失敗しました。");
            }
        }

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        public void UpdatePhoneEntity(QH_ACCOUNTPHONE_MST entity)
        {
            entity.PHONENUMBER = entity.PHONENUMBER.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new AccountPhoneWriterArgs { Entity = entity };
            var result = new AccountPhoneWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_ACCOUNTPHONE_MST)}の更新に失敗しました。");
            }
        }

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        public void DeletePhoneEntity(QH_ACCOUNTPHONE_MST entity)
        {
            entity.PHONENUMBER = entity.PHONENUMBER.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new AccountPhoneWriterArgs { Entity = entity };
            var result = new AccountPhoneWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_ACCOUNTPHONE_MST)}の削除に失敗しました。");
            }
        }

        /// <summary>
        /// QH_ACCOUNTPHONE_MSTのレコードを物理削除します。
        /// </summary>
        /// <param name="accountKey"></param>
        public void PhysicalDeletePhoneEntity(Guid accountKey)
        {
            var entity = new QH_ACCOUNTPHONE_MST
            {
                ACCOUNTKEY = accountKey,
            };

            var args = new AccountPhoneWriterArgs
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new AccountPhoneWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_ACCOUNTPHONE_MST)}の物理削除に失敗しました。");
            }
        }

        /// <summary>
        /// QH_PASSWORDMANAGEMENT_DATのレコードを1件取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public QH_PASSWORDMANAGEMENT_DAT ReadPasswordManagementEntity(Guid accountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_PASSWORDMANAGEMENT_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_PASSWORDMANAGEMENT_DAT)}
                    WHERE {nameof(QH_PASSWORDMANAGEMENT_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_PASSWORDMANAGEMENT_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_PASSWORDMANAGEMENT_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();

                if (entity != null)
                {
                    entity.USERID = entity.USERID.TryDecrypt();
                    entity.USERPASSWORD = entity.USERPASSWORD.TryDecrypt();
                }

                return entity;
            }
        }
    }
}
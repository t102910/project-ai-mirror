using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    internal sealed class DbPasswordManagementReaderCore:QsDbReaderBase
    {
        #region "Private Method
        /// <summary>
        /// 値を指定して、
        /// パスワード管理データテーブルエンティティを取得します。
        /// </summary>
        /// <typeparam name="TEntity">パスワード管理データテーブルエンティティの型。</typeparam>
        /// <param name="accountKey">
        /// アカウントキー。
        /// この値は <paramref name="userId" /> より優先されます。</param>
        /// <param name="userId">
        /// ユーザー ID。
        /// <paramref name="accountKey" /> が指定されている場合は、
        /// そちらが優先されます。
        /// </param>
        /// <param name="includeDelete">
        /// 削除済みも対象にするかのフラグ（オプショナル）。
        /// 対象にするなら True、
        /// 対象にしない False（デフォルト）を指定します。
        /// </param>
        /// <returns>
        /// データが存在するなら値がセットされたテーブルエンティティ、
        /// 存在しないなら Nothing。
        /// </returns>
        /// <remarks></remarks>
        private TEntity ReadPasswordManagementEntity<TEntity>(Guid accountKey, string userId, bool includeDelete = false) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            PasswordManagementReader<TEntity> reader = new PasswordManagementReader<TEntity>();
            PasswordManagementReaderArgs<TEntity> readerArgs = new PasswordManagementReaderArgs<TEntity>()
            {
                AccountKey = accountKey,
                UserId = userId,
                IncludeDelete = includeDelete
            };
            PasswordManagementReaderResults<TEntity> readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults.IsSuccess && readerResults.Result.Count == 1)
                return readerResults.Result.First();
            else
                return null;
        }
        #endregion


        #region "Public Method"

        public  TEntity ReadPasswordManagementEntity<TEntity>(string userId, QsCrypt cryptor) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            string encryptedUserId = string.IsNullOrWhiteSpace(userId) || cryptor == null ? userId : cryptor.EncryptString(userId);

            return this.ReadPasswordManagementEntity<TEntity>(Guid.Empty, encryptedUserId);
        }

        public  TEntity ReadPasswordManagementEntity<TEntity>(Guid accountKey) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            if (accountKey == Guid.Empty)
                throw new ArgumentNullException("accountKey", "アカウントキーが不正です。");

            return this.ReadPasswordManagementEntity<TEntity>(accountKey, string.Empty);
        }
        #endregion
    }
}
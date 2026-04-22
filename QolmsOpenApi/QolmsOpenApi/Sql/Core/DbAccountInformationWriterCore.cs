using System;
using System.Collections.Generic;
using System.Text;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Data.Common;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    
    /// <summary>
    /// アカウント情報を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks>
    /// このクラスはOpenApi専用の独自処理クラスです。共通処理はQolmsDbLibraryV1.AccountWriterCoreにあるのでそちらを参照してください。
    /// </remarks>
    internal sealed class DbAccountInformationWriterCore : QsDbWriterBase
    {


        /// <summary>
        /// 所有者アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _authorKey = Guid.Empty;

        /// <summary>
        /// 対象者アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _actorKey = Guid.Empty;



        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbAccountInformationWriterCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbAccountInformationWriterCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="authorKey">所有者アカウントキー。</param>
        /// <param name="actorKey">対象者アカウントキー。</param>
        /// <remarks></remarks>
        public DbAccountInformationWriterCore(Guid authorKey, Guid actorKey)
        {
            this._authorKey = authorKey;
            this._actorKey = actorKey;

            // TODO: アカウントキーの有効性をチェック

            if (this._authorKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("authorKey", "所有者アカウントキーが不正です。");
            if (this._actorKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("actorKey", "対象者アカウントキーが不正です。");
        }



        /// <summary>
        /// パスワード管理データテーブルエンティティを登録します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private bool UpsertPasswordManagementEntityByMailAddress<TEntity>(TEntity entity) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象のページ設定情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._actorKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<TEntity>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@p1", entity.ACCOUNTKEY),
                    this.CreateParameter(connection, "@p2", entity.PASSWORDRECOVERYMAILADDRESS),
                    this.CreateParameter(connection, "@p3", entity.UPDATEDDATE)
                };
               
                query.Append("update ");
                query.Append(new TEntity().GetEntityName());
                query.Append(" set passwordrecoverymailaddress=@p2, updateddate=@p3 ");
                query.Append(" where accountkey=@p1 and deleteflag=0;");
                
                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params) == 1;
            }
        }



        /// <summary>
        /// パスワードリカバリ用メールアドレスを指定して、
        /// パスワード管理テーブルを更新します。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="passwordRecoveryMailAddress"></param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        /// <remarks>セキュリティ質問を使用しません。</remarks>
        public bool WritePasswordManagementEntityByMailAddress<TEntity>(string passwordRecoveryMailAddress, DateTime actionDate, Guid actionKey  ) where TEntity : QsPasswordManagementDataEntityBase, new()
        {
            bool result = false;
            TEntity entity = new DbPasswordManagementReaderCore().ReadPasswordManagementEntity<TEntity>(this._actorKey);

            if (entity != null)
            {
                    using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    // シリアライズして、暗号化
                    entity.PASSWORDRECOVERYMAILADDRESS = string.IsNullOrWhiteSpace(passwordRecoveryMailAddress) ? string.Empty : crypt.EncryptString(passwordRecoveryMailAddress);
                }

                entity.UPDATEDDATE = actionDate;
                entity.DataState = QsDbEntityStateTypeEnum.Modified;
                

                result = this.UpsertPasswordManagementEntityByMailAddress(entity);
            }
            else
            {
            }

            return result;
        }
    }


}
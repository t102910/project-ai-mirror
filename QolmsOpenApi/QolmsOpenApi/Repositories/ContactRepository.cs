using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// 連絡手帳の入出力インターフェース
    /// </summary>
    public interface IContactRepository
    {
        /// <summary>
        /// 連絡手帳の対象アカウントのレコードを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        QH_CONTACT_DAT ReadContactEntity(Guid accountKey);

        /// <summary>
        /// 連絡手帳の対象アカウントのレコードを登録します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="cryptedJson">暗号化されたjson文字列</param>
        /// <param name="actionKey">操作キー</param>
        void WriteContactEntity(Guid accountKey, string cryptedJson, Guid actionKey);
    }

    /// <summary>
    /// 連絡手帳の入出力実装
    /// </summary>
    public class ContactRepository: IContactRepository
    {
        /// <summary>
        /// 連絡手帳の対象アカウントのレコードを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <returns></returns>
        public QH_CONTACT_DAT ReadContactEntity(Guid accountKey)
        {
            var reader = new QhContactEntityReader();
            var readerArgs = new QhContactEntityReaderArgs
            {
                Data = new List<QH_CONTACT_DAT>
                {
                    new QH_CONTACT_DAT() { ACCOUNTKEY = accountKey }
                }
            };
            var readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults == null ||
                !readerResults.IsSuccess ||
                readerResults.Result == null)
            {
                // 失敗または何かしらの理由で正常に取得できない場合は例外とする
                throw new Exception();
            }

            var result = readerResults.Result.FirstOrDefault();

            if (result?.DELETEFLAG ?? true)
            {
                // 削除フラグ付き、あるいは結果レコード無しはnullを返す（正常扱い）
                return null;
            }

            return result;
        }

        /// <summary>
        /// 連絡手帳の対象アカウントのレコードを登録します。
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="cryptedJson">暗号化されたjson文字列</param>
        /// <param name="actionKey">操作キー</param>
        public void WriteContactEntity(Guid accountKey, string cryptedJson, Guid actionKey)
        {
            var now = DateTime.Now;
            byte historyType = 3; // 変更

            
            var entity = ReadContactEntity(accountKey);
            var historyItem = entity?.CONTACTSET ?? string.Empty;
            if (entity == null)
            {
                historyType = 1; // 追加
                entity = new QH_CONTACT_DAT
                {
                    ACCOUNTKEY = accountKey,
                    CREATEDDATE = now,
                    DataState = QsDbEntityStateTypeEnum.Added
                };
            }
            else
            {
                entity.DataState = QsDbEntityStateTypeEnum.Modified;
            }

            entity.CONTACTSET = cryptedJson;
            entity.UPDATEDDATE = now;

            var writer = new QhContactEntityWriter();
            var writerArgs = new QhContactEntityWriterArgs
            {
                Data = new List<QH_CONTACT_DAT> { entity },
            };

            var historyWriter = new QhContactHistEntityWriter();
            var historyArgs = new QhContactHistEntityWriterArgs
            {
                Data = new List<QH_CONTACTHIST_LOG>
                {
                    new QH_CONTACTHIST_LOG
                    {
                        ACCOUNTKEY = accountKey,
                        ACTIONDATE = now,
                        HISTTYPE = historyType,
                        CONTACTSET = historyItem,
                        DataState = QsDbEntityStateTypeEnum.Added,
                        ACTIONKEY = actionKey
                    }
                }
            };

            // トランザクション開始
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable 
                }))
            {
                var writeResult = QsDbManager.WriteByCurrent
               (writer, writerArgs);

                if (!writeResult.IsSuccess)
                {
                    throw new Exception();
                }

                var historyResult = QsDbManager.WriteByCurrent(historyWriter, historyArgs);

                if (!historyResult.IsSuccess)
                {
                    throw new Exception();
                }

                scope.Complete();
            }           
        }
    }
}
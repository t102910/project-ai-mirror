using System;
using System.Collections.Generic;
using System.Linq;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// ファイル情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbFileStorageReaderCore<TEntity> where TEntity : QsUploadFileDataEntityBase
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
        private DbFileStorageReaderCore()
        {
        }

        /// <summary>
        /// 所有者アカウントキーおよび対象者アカウントキーを指定して、
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="authorKey">所有者アカウントキー。</param>
        /// <param name="actorKey">対象者アカウントキー。</param>
        /// <remarks></remarks>
        public DbFileStorageReaderCore(Guid authorKey, Guid actorKey)
        {
            this._authorKey = authorKey;
            this._actorKey = actorKey;

            // TODO: アカウントキーの有効性をチェック

            if (this._authorKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("authorKey", "所有者アカウントキーが不正です。");
            if (this._actorKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("actorKey", "対象者アカウントキーが不正です。");
        }



        private DbFileStorageItem ReadQhUploadFileItem(Guid fileKey)
        {
            DbFileStorageItem result = new DbFileStorageItem();

            QhUploadFileEntityReader reader = new QhUploadFileEntityReader();
            QhUploadFileEntityReaderArgs readerArgs = new QhUploadFileEntityReaderArgs() { Data =  new List<QH_UPLOADFILE_DAT>() { new QH_UPLOADFILE_DAT() { FILEKEY = fileKey } } };
            QhUploadFileEntityReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults.IsSuccess && readerResults.Result.Count == 1 && !readerResults.Result.First().DELETEFLAG)
            {
                result.AccountKey = this._authorKey;
                result.FileKey = readerResults.Result.First().FILEKEY;
                result.OriginalName = readerResults.Result.First().ORIGINALNAME;
                result.ContentType = readerResults.Result.First().CONTENTTYPE;
            }
           
            return result;
        }

        private DbFileStorageItem ReadQpUploadFileItem(Guid fileKey)
        {
            DbFileStorageItem result = new DbFileStorageItem();

            QpUploadFileEntityReader reader = new QpUploadFileEntityReader();
            QpUploadFileEntityReaderArgs readerArgs = new QpUploadFileEntityReaderArgs() { Data = new List<QP_UPLOADFILE_DAT>() { new QP_UPLOADFILE_DAT() { FILEKEY = fileKey }  } };
            QpUploadFileEntityReaderResults readerResults = QsDbManager.Read(reader, readerArgs);
           
            if (readerResults.IsSuccess && readerResults.Result.Count == 1 && !readerResults.Result.First().DELETEFLAG)
            {
                result.AccountKey = this._authorKey;
                result.FileKey = readerResults.Result.First().FILEKEY;
                result.OriginalName = readerResults.Result.First().ORIGINALNAME;
                result.ContentType = readerResults.Result.First().CONTENTTYPE;
            }

            return result;
        }

        /// <summary>
        /// ファイルキーを指定して、
        /// ファイル情報を取得します。
        /// </summary>
        /// <param name="fileKey">ファイルキー。</param>
        /// <returns>
        /// データが存在するなら値がセットされたファイル情報、
        /// 存在しないなら空のファイル情報。
        /// </returns>
        /// <remarks></remarks>
        public DbFileStorageItem GetFileStorageItem(Guid fileKey)
        {
            DbFileStorageItem result = new DbFileStorageItem();

            // DB から取得
            switch (typeof(TEntity).Name)
            {
                case nameof(QH_UPLOADFILE_DAT):
                    // QOLMSから（顔写真）
                    result = this.ReadQhUploadFileItem(fileKey);
                    break;
                    
                case nameof(QP_UPLOADFILE_DAT):
                    // 薬局から（お薬手帳、処方箋関連）
                    result = this.ReadQpUploadFileItem(fileKey);
                    break;
            }

            return result;
        }
    }


}
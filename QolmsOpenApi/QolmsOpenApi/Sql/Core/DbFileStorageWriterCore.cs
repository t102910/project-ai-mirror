using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core 
{
    
    /// <summary>
    /// ファイル情報を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbFileStorageWriterCore<TEntity> where TEntity : QsUploadFileDataEntityBase
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
        private DbFileStorageWriterCore()
        {
        }

        /// <summary>
        /// 所有者アカウントキーおよび対象者アカウントキーを指定して、
        /// <see cref="DbFileStorageWriterCore &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="authorKey">所有者アカウントキー。</param>
        /// <param name="actorKey">対象者アカウントキー。</param>
        /// <remarks></remarks>
        public DbFileStorageWriterCore(Guid authorKey, Guid actorKey)
        {
            this._authorKey = authorKey;
            this._actorKey = actorKey;
        }



        private bool WriteQhUploadFileEntity(Guid fileKey, string originalName, string contentType)
        {
            DateTime now = DateTime.Now;
            // DB へ登録
            QhUploadFileEntityWriter writer = new QhUploadFileEntityWriter();
            var writerArgs = new QhUploadFileEntityWriterArgs()
            {
                Data = new List<QH_UPLOADFILE_DAT>(){ new QH_UPLOADFILE_DAT() {
                    FILEKEY = fileKey,
                    ORIGINALNAME = originalName,
                    CONTENTTYPE = contentType,
                    DELETEFLAG = false,
                    CREATEDDATE = now,
                    CREATEDACCOUNTKEY = _authorKey,
                    UPDATEDDATE = now,
                    UPDATEDACCOUNTKEY = _authorKey
                }}
            };


            QhUploadFileEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
                return true;
            else
                throw new InvalidOperationException("QH_UPLOADFILE_DATテーブルへの登録に失敗しました。");
        }

        private bool WriteQpUploadFileEntity(Guid fileKey, string originalName, string contentType)
        {
            DateTime now = DateTime.Now;
            // DB へ登録
            QpUploadFileEntityWriter writer = new QpUploadFileEntityWriter();
            var writerArgs = new QpUploadFileEntityWriterArgs()
            {
                Data = new List<QP_UPLOADFILE_DAT>()
                {
                    new QP_UPLOADFILE_DAT() {
                    FILEKEY = fileKey,
                    ORIGINALNAME = originalName,
                    CONTENTTYPE = contentType,
                    DELETEFLAG = false ,
                    CREATEDDATE = now,
                    CREATEDACCOUNTKEY = _authorKey,
                    UPDATEDDATE = now,
                    UPDATEDACCOUNTKEY = _authorKey
                    }
                }
            };
        
            QpUploadFileEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
                return true;
            else
                throw new InvalidOperationException("QH_UPLOADFILE_DATテーブルへの登録に失敗しました。");
           
        }


        /// <summary>
        /// ファイル情報を登録します。
        /// </summary>
        /// <param name="fileKey">ファイルキー。</param>
        /// <param name="originalName">オリジナルファイル名。</param>
        /// <param name="contentType">MIME タイプ。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool WriteUploadFileEntity(Guid fileKey, string originalName, string contentType)
        {
            if (fileKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("fileKey", "ファイルキーが不正です。");
            if (string.IsNullOrWhiteSpace(originalName))
                throw new ArgumentNullException("originalName", "オリジナルファイル名がNull参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentNullException("contentType", "MIMEタイプがNull参照もしくは空白です。");

            switch (typeof(TEntity).Name)
            {
                case nameof(QH_UPLOADFILE_DAT):
                    return this.WriteQhUploadFileEntity(fileKey, originalName, contentType);
                case nameof(QP_UPLOADFILE_DAT):
                    return this.WriteQpUploadFileEntity(fileKey, originalName, contentType);
                default:
                    throw new InvalidOperationException("そのテーブルエンティティはサポートされていません。");
            }
        }
    }


}
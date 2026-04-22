using MGF.QOLMS.QolmsAzureStorageCoreV1;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage.UploadCheckupPostingFile
{
    /// <summary>
    /// データ投稿（健診ファイル） データ を、
    /// ブロブ ストレージ へ登録するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class UploadCheckupPostingFileBlobEntityWriter : QsAzureBlobStorageWriterBase<QmUploadCheckupPostingFileBlobEntity>, IQsAzureBlobStorageWriter<QmUploadCheckupPostingFileBlobEntity, UploadCheckupPostingFileBlobEntityWriterArgs, UploadCheckupPostingFileBlobEntityWriterResults>
    {


        /// <summary>
        /// <see cref="UploadCheckupPostingFileBlobEntityWriter" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadCheckupPostingFileBlobEntityWriter() : base(true, BlobContainerPublicAccessType.Off)
        {
        }



        /// <summary>
        /// Azure ブロブ ストレージ へ値を設定します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>
        /// 戻り値 クラス。
        /// </returns>
        /// <remarks></remarks>
        public UploadCheckupPostingFileBlobEntityWriterResults Execute(UploadCheckupPostingFileBlobEntityWriterArgs args)
        {
            var result = new UploadCheckupPostingFileBlobEntityWriterResults()
            {
                IsSuccess = false,
                Result = Guid.Empty
            };

            result.Result = base.Write(args.Entity); // 既存の ブロブ に対する上書きを禁止
            result.IsSuccess = result.Result != Guid.Empty;

            return result;
        }
    }
}
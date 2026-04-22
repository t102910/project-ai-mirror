using System;

using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   
    /// <summary>
    /// お薬手帳アプリ ログ ファイル データ を、
    /// ブロブ ストレージ へ登録するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class AppLogFileBlobEntityWriter : QsAzureBlobStorageWriterBase<QhInquiryFileBlobEntity>, IQsAzureBlobStorageWriter<QhInquiryFileBlobEntity, AppLogFileBlobEntityWriterArgs, AppLogFileBlobEntityWriterResults>
    {


        /// <summary>
        /// <see cref="AppLogFileBlobEntityWriter" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AppLogFileBlobEntityWriter() : base(true, BlobContainerPublicAccessType.Off)
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
        public AppLogFileBlobEntityWriterResults Execute(AppLogFileBlobEntityWriterArgs args)
        {
            AppLogFileBlobEntityWriterResults result = new AppLogFileBlobEntityWriterResults()
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
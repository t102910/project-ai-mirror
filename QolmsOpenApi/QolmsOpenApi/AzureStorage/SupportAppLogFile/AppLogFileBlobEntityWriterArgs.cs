
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
  
    /// <summary>
    /// お薬手帳アプリログ ファイル データ を、
    /// ブロブ ストレージ へ登録するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class AppLogFileBlobEntityWriterArgs : QsAzureBlobStorageWriterArgsBase<QhInquiryFileBlobEntity>
    {
        /// <summary>
        /// <see cref="AppLogFileBlobEntityWriterArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public AppLogFileBlobEntityWriterArgs() : base()
        {
        }
    }


}
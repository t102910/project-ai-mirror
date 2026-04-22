using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// データ投稿（健診ファイル） データ を、
    /// ブロブ ストレージ から削除するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class UploadCheckupPostingFileBlobEntityDeleterArgs : QsAzureBlobStorageWriterArgsBase<QmUploadCheckupPostingFileBlobEntity>
    {
        /// <summary>
        /// <see cref="UploadCheckupPostingFileBlobEntityDeleterArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadCheckupPostingFileBlobEntityDeleterArgs() : base()
        {
        }
    }
}
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{

    /// <summary>
    /// アップロード された ファイル の表示用情報の メタデータ を、
    /// ブロブ ストレージ から取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class UploadEditedFileBlobMetadataReaderArgs : QsAzureBlobStorageMetadataReaderArgsBase
    {


        /// <summary>
        /// <see cref="UploadEditedFileBlobMetadataReaderArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadEditedFileBlobMetadataReaderArgs() : base()
        {
        }
    }



}
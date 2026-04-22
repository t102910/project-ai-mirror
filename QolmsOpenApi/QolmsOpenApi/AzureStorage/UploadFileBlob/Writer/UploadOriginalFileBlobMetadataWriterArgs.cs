using MGF.QOLMS.QolmsAzureStorageCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{


    /// <summary>
    /// アップロード された ファイル の オリジナル 情報の メタデータ を、
    /// ブロブ ストレージ へ登録した結果を格納する戻り値 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル（オリジナル）ブロブ エンティティ クラス の型。</typeparam>
    /// <remarks></remarks>
    internal sealed class UploadOriginalFileBlobMetadataWriterArgs<TEntity> : QsAzureBlobStorageMetadataWriterArgsBase<TEntity> where TEntity : QsUploadFileBlobEntityBase
    {


        /// <summary>
        /// <see cref="UploadOriginalFileBlobMetadataWriterArgs &lt; TEntity &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadOriginalFileBlobMetadataWriterArgs() : base()
        {
        }
    }


}
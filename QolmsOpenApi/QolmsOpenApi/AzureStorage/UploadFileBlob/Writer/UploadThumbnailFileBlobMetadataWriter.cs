using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    

    /// <summary>
    /// アップロード された ファイル の サムネイル 情報の メタデータ を、
    /// ブロブ ストレージ へ登録するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル（サムネイル）ブロブ エンティティ クラス の型。</typeparam>
    /// <remarks></remarks>
    internal sealed class UploadThumbnailFileBlobMetadataWriter<TEntity> : UploadFileBlobStorageMetadataWriterBase<TEntity, UploadThumbnailFileBlobMetadataWriterArgs<TEntity>, UploadThumbnailFileBlobMetadataWriterResults> where TEntity : QsUploadFileBlobEntityBase, new()
    {


        /// <summary>
        /// <see cref="UploadThumbnailFileBlobMetadataWriter &lt; TEntity &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadThumbnailFileBlobMetadataWriter() : base()
        {
        }



        /// <summary>
        /// Azure ブロブ ストレージ へ メタデータ を設定します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override UploadThumbnailFileBlobMetadataWriterResults Execute(UploadThumbnailFileBlobMetadataWriterArgs<TEntity> args)
        {
            return new UploadThumbnailFileBlobMetadataWriterResults() { IsSuccess = base.Write(args.Entity) };
        }
    }


}
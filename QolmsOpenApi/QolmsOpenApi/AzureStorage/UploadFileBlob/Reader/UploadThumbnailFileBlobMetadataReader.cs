using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
  
    /// <summary>
    /// アップロード された ファイル の サムネイル 情報の メタデータ を、
    /// ブロブ ストレージ から取得するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル（サムネイル）ブロブ エンティティ クラス の型。</typeparam>
    /// <remarks></remarks>
    internal sealed class UploadThumbnailFileBlobMetadataReader<TEntity> : UploadFileBlobStorageMetadataReaderBase<TEntity, UploadThumbnailFileBlobMetadataReaderArgs, UploadThumbnailFileBlobMetadataReaderResults<TEntity>> where TEntity : QsUploadFileBlobEntityBase, new()
    {
        /// <summary>
        /// <see cref="UploadThumbnailFileBlobMetadataReader &lt; TEntity &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadThumbnailFileBlobMetadataReader() : base()
        {
        }



        /// <summary>
        /// Azure ブロブ ストレージ から メタデータ を取得します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>
        /// 戻り値 クラス。
        /// </returns>
        /// <remarks></remarks>
        public override UploadThumbnailFileBlobMetadataReaderResults<TEntity> Execute(UploadThumbnailFileBlobMetadataReaderArgs args)
        {
            UploadThumbnailFileBlobMetadataReaderResults<TEntity> result = new UploadThumbnailFileBlobMetadataReaderResults<TEntity>()
            {
                IsSuccess = false,
                Result = null
            };

            result.Result = base.Read(args.Name);
            result.IsSuccess = result.Result != null;

            return result;
        }
    }


}
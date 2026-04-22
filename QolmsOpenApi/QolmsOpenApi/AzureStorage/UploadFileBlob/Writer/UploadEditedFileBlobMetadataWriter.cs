using MGF.QOLMS.QolmsAzureStorageCoreV1;
namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// アップロード された ファイル の表示用情報の メタデータ を、
    /// ブロブ ストレージ へ登録するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル（表示用）ブロブ エンティティ クラス の型。</typeparam>
    /// <remarks></remarks>
    internal sealed class UploadEditedFileBlobMetadataWriter<TEntity> : UploadFileBlobStorageMetadataWriterBase<TEntity, UploadEditedFileBlobMetadataWriterArgs<TEntity>, UploadEditedFileBlobMetadataWriterResults> where TEntity : QsUploadFileBlobEntityBase, new()
    {

        /// <summary>
        /// <see cref="UploadEditedFileBlobMetadataWriter &lt; TEntity &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadEditedFileBlobMetadataWriter() : base()
        {
        }



        /// <summary>
        /// Azure ブロブ ストレージ へ メタデータ を設定します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override UploadEditedFileBlobMetadataWriterResults Execute(UploadEditedFileBlobMetadataWriterArgs<TEntity> args)
        {
            return new UploadEditedFileBlobMetadataWriterResults() { IsSuccess = base.Write(args.Entity) };
        }
    }


}
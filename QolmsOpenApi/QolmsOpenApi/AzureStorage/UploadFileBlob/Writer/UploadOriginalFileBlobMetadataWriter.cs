using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
   
    /// <summary>
    /// アップロード された ファイル の オリジナル 情報の メタデータ を、
    /// ブロブ ストレージ へ登録するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル（オリジナル）ブロブ エンティティ クラス の型。</typeparam>
    /// <remarks></remarks>
    internal sealed class UploadOriginalFileBlobMetadataWriter<TEntity> : UploadFileBlobStorageMetadataWriterBase<TEntity, UploadOriginalFileBlobMetadataWriterArgs<TEntity>, UploadOriginalFileBlobMetadataWriterResults> where TEntity : QsUploadFileBlobEntityBase, new()
    {


        /// <summary>
        /// <see cref="UploadOriginalFileBlobMetadataWriter &lt; TEntity &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadOriginalFileBlobMetadataWriter() : base()
        {
        }



        /// <summary>
        /// Azure ブロブ ストレージ へ メタデータ を設定します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override UploadOriginalFileBlobMetadataWriterResults Execute(UploadOriginalFileBlobMetadataWriterArgs<TEntity> args)
        {
            return new UploadOriginalFileBlobMetadataWriterResults() { IsSuccess = base.Write(args.Entity) };
        }
    }


}
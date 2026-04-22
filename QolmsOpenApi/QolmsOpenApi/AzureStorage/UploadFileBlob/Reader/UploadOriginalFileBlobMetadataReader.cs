
using Microsoft.WindowsAzure.Storage.Blob;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{

    /// <summary>
    /// アップロード された ファイル の表示用情報の メタデータ を、
    /// ブロブ ストレージ から取得するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TEntity">アップロード ファイル（表示用）ブロブ エンティティ クラス の型。</typeparam>
    /// <remarks></remarks>
    internal sealed class UploadOriginalFileBlobMetadataReader<TEntity> : UploadFileBlobStorageMetadataReaderBase<TEntity, UploadOriginalFileBlobMetadataReaderArgs, UploadOriginalFileBlobMetadataReaderResults<TEntity>> where TEntity : QsUploadFileBlobEntityBase, new()
    {


        /// <summary>
        /// <see cref="UploadOriginalFileBlobMetadataReader &lt; TEntity &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadOriginalFileBlobMetadataReader() : base()
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
        public override UploadOriginalFileBlobMetadataReaderResults<TEntity> Execute(UploadOriginalFileBlobMetadataReaderArgs args)
        {
            UploadOriginalFileBlobMetadataReaderResults<TEntity> result = new UploadOriginalFileBlobMetadataReaderResults<TEntity>()
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
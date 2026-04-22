
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
    internal sealed class UploadEditedFileBlobMetadataReader<TEntity> : UploadFileBlobStorageMetadataReaderBase<TEntity, UploadEditedFileBlobMetadataReaderArgs, UploadEditedFileBlobMetadataReaderResults<TEntity>> where TEntity : QsUploadFileBlobEntityBase, new()
    {


        /// <summary>
        /// <see cref="UploadEditedFileBlobMetadataReader &lt; TEntity &gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadEditedFileBlobMetadataReader() : base()
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
        public override UploadEditedFileBlobMetadataReaderResults<TEntity> Execute(UploadEditedFileBlobMetadataReaderArgs args)
        {
            UploadEditedFileBlobMetadataReaderResults<TEntity> result = new UploadEditedFileBlobMetadataReaderResults<TEntity>()
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
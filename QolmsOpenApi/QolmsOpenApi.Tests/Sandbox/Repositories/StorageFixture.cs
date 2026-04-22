using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.AzureStorage;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{
    [TestClass]
    public class StorageFixture
    {
        StorageRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new StorageRepository();
        }

        [TestMethod]
        public void MyTestMethod()
        {          
            var fileKey = Guid.Parse("a857d068b5af4d5db35cffa2ecc6a257");
            var file = _repo.ReadImage(fileKey, QsApiFileTypeEnum.Original);
        }

        [TestMethod]
        public void 画像以外のファイルを登録し読み出し削除ができる()
        {
            var accountKey = Guid.Parse("112f7447-b2f8-4336-8242-9eede7b378dd");
            var authorKey = Guid.Parse("FFFFFFFF-9999-0000-0044-000000000001");

            var asm = GetType().Assembly;
            var fileName = asm.GetManifestResourceNames().FirstOrDefault(x => x.Contains("sample.zip"));
            var st = asm.GetManifestResourceStream(fileName);

            var ms = new MemoryStream();
            st.CopyTo(ms);

            var data = Convert.ToBase64String(ms.ToArray());

            var fileItem = new QoApiFileItem
            {
                OriginalName = "sample.zip",
                ContentType = "application/zip",
                Data = data
            };

            // 実行
            var fileKey = _repo.WriteFile(accountKey, authorKey, fileItem);

            // DBの値を取得
            var entity = _repo.ReadFileEntity(fileKey);

            // 情報一致
            entity.ORIGINALNAME.Is(fileItem.OriginalName);
            entity.CONTENTTYPE.Is(fileItem.ContentType);
            entity.FILEKEY.Is(fileKey);

            // ストレージから取得
            var storageFile = _repo.ReadFile(fileKey);
            // データ一致
            storageFile.Data.Is(data);

            // 削除
            _repo.DeleteFile(authorKey, fileKey);

            // ストレージから取得
            var args = new UploadOriginalFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var reader = new UploadOriginalFileBlobEntityReader<QhUploadOriginalFileBlobEntity>();
            var results = reader.Execute(args);

            // ファイルは削除済み
            results.IsSuccess.IsFalse();
            results.Result.IsNull();
        }

        [TestMethod]
        public void 画像ファイルを登録し読み出し削除ができる()
        {
            var accountKey = Guid.Parse("112f7447-b2f8-4336-8242-9eede7b378dd");
            var authorKey = Guid.Parse("FFFFFFFF-9999-0000-0044-000000000001");

            var asm = GetType().Assembly;
            var fileName = asm.GetManifestResourceNames().FirstOrDefault(x => x.Contains("sample.jpg"));
            var st = asm.GetManifestResourceStream(fileName);

            var ms = new MemoryStream();
            st.CopyTo(ms);

            var data = Convert.ToBase64String(ms.ToArray());

            var fileItem = new QoApiFileItem
            {
                OriginalName = "sample.jpg",
                ContentType = "image/jpeg",
                Data = data
            };

            // 実行
            var fileKey = _repo.WriteImage(accountKey, authorKey, fileItem);

            // DBの値を取得
            var entity = _repo.ReadFileEntity(fileKey);

            // 情報一致
            entity.ORIGINALNAME.Is(fileItem.OriginalName);
            entity.CONTENTTYPE.Is(fileItem.ContentType);
            entity.FILEKEY.Is(fileKey);

            // ストレージから取得
            var original = _repo.ReadImage(fileKey, QsApiFileTypeEnum.Original);
            // データ一致
            original.Data.Is(data);

            var edited = _repo.ReadImage(fileKey, QsApiFileTypeEnum.Edited);
            edited.Data.IsNotNull();

            var thumb = _repo.ReadImage(fileKey, QsApiFileTypeEnum.Thumbnail);
            thumb.Data.IsNotNull();

            // 削除
            _repo.DeleteFile(authorKey, fileKey);

            // ストレージから取得
            var args = new UploadOriginalFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var reader = new UploadOriginalFileBlobEntityReader<QhUploadOriginalFileBlobEntity>();
            var results = reader.Execute(args);

            // ファイルは削除済み
            results.IsSuccess.IsFalse();
            results.Result.IsNull();

            // ストレージから取得(編集画像)
            var eArgs = new UploadEditedFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var eReader = new UploadEditedFileBlobEntityReader<QhUploadEditedFileBlobEntity>();
            var eResults = eReader.Execute(eArgs);

            // ファイルは削除済み
            eResults.IsSuccess.IsFalse();
            eResults.Result.IsNull();

            // ストレージから取得(サムネイル画像)
            var tArgs = new UploadThumbnailFileBlobEntityReaderArgs
            {
                Name = fileKey
            };
            var tReader = new UploadThumbnailFileBlobEntityReader<QhUploadThumbnailFileBlobEntity>();
            var tResults = tReader.Execute(tArgs);

            // ファイルは削除済み
            tResults.IsSuccess.IsFalse();
            tResults.Result.IsNull();
        }
    }
}

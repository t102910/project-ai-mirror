using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 子アカウントを含むユーザー情報に関する機能を提供するAPIコントローラー
    /// </summary>
    public class UserController: QoApiControllerBase
    {
        /// <summary>
        /// 本人を含むユーザー情報一覧を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("ListRead")]
        public QoUserListReadApiResults PostListRead(QoUserListReadApiArgs args)
        {
            var worker = new UserListReadWorker(new FamilyRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 子ユーザー(家族ユーザー)を追加します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("FamilyAdd")]
        public QoUserFamilyAddApiResults PostFamilyAdd(QoUserFamilyAddApiArgs args)
        {
            var worker = new UserFamilyAddWorker(
                new AccountRepository(),
                new FamilyRepository(),
                new StorageRepository()
            );
            return ExecuteWorkerMethod(args, worker.Add);
        }

        /// <summary>
        /// 子ユーザー(家族ユーザー)を削除します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("FamilyDelete")]
        public QoUserFamilyDeleteApiResults PostFamilyDelete(QoUserFamilyDeleteApiArgs args)
        {
            var worker = new UserFamilyDeleteWorker(
               new AccountRepository(),
               new FamilyRepository(),
               new StorageRepository()
           );
            return ExecuteWorkerMethod(args, worker.Delete);
        }

        /// <summary>
        /// ユーザー情報(本人含む)を更新します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Write")]
        public QoUserWriteApiResults PostWrite(QoUserWriteApiArgs args)
        {
            var worker = new UserWriteWorker(
               new AccountRepository(),
               new FamilyRepository(),
               new StorageRepository()
            );
            return ExecuteWorkerMethod(args, worker.Write);
        }

        /// <summary>
        /// ユーザー情報(本人含む)を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Read")]
        public QoUserReadApiResults PostRead(QoUserReadApiArgs args)
        {
            var worker = new UserReadWorker(
               new AccountRepository(),
               new FamilyRepository(),
               new StorageRepository(),
               new PasswordManagementRepository()
            );
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// ユーザーの写真情報を更新します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("PhotoWrite")]
        public QoUserPhotoWriteApiResults PostPhotoWrite(QoUserPhotoWriteApiArgs args)
        {
            var worker = new UserPhotoWriteWorker(
              new AccountRepository(),
              new FamilyRepository(),
              new StorageRepository()
           );
            return ExecuteWorkerMethod(args, worker.Write);
        }

        /// <summary>
        /// ユーザーの写真情報を削除します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("PhotoDelete")]
        public QoUserPhotoDeleteApiResults PostPhotoDelete(QoUserPhotoDeleteApiArgs args)
        {
            var worker = new UserPhotoDeleteWorker(
              new AccountRepository(),
              new FamilyRepository(),
              new StorageRepository()
           );
            return ExecuteWorkerMethod(args, worker.Delete);
        }
    }
}
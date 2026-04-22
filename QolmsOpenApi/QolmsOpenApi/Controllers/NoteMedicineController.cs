using System.Net;
using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// お薬手帳に関する機能を提供するAPIコントローラです。
    /// </summary>
    public class NoteMedicineController : QoApiControllerBase
    {
        /// <summary>
        /// お薬手帳情報 アクセスキー （一般利用者向け）を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtToken, QoApiFunctionTypeEnum.AccessKey)]
        [ActionName("AccessKeyGenerate")]
        public QoNoteMedicineAccessKeyGenerateApiResults PostAccessKeyGenerate(QoNoteMedicineAccessKeyGenerateApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NoteMedicineWorker.AccessKeyGenerate);
        }

        /// <summary>
        /// お薬手帳情報アクセスキー （一般利用者向け）を再生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("AccessKeyRefresh")]
        public QoNoteMedicineAccessKeyRefreshApiResults PostAccessKeyRefresh(QoNoteMedicineAccessKeyRefreshApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NoteMedicineWorker.AccessKeyRefresh);
        }

        /// <summary>
        /// お薬手帳の同期用履歴情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("SyncHistoryRead")]
        public QoNoteMedicineSyncHistoryReadApiResults PostSyncHistoryRead(QoNoteMedicineSyncHistoryReadApiArgs args)
        {
            var worker = new NoteMedicineSyncHistoryReadWorker(new NoteMedicineRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// お薬手帳の同期用更新情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("SyncUpdateRead")]
        public QoNoteMedicineSyncUpdateReadApiResults PostSyncUpdateRead(QoNoteMedicineSyncUpdateReadApiArgs args)
        {
            var worker = new NoteMedicineSyncUpdateReadWorker(new NoteMedicineRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// お薬手帳の同期用情報を追加します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("SyncDataAdd")]
        public QoNoteMedicineSyncDataAddApiResults PostSyncDataAdd(QoNoteMedicineSyncDataAddApiArgs args)
        {
            var worker = new NoteMedicineSyncDataAddWorker(
                new NoteMedicineRepository(),
                new AccountRepository()
            );
            return ExecuteWorkerMethod(args, worker.Add);
        }

        /// <summary>
        /// お薬手帳の同期用情報を編集します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("SyncDataEdit")]
        public QoNoteMedicineSyncDataEditApiResults PostSyncDataEdit(QoNoteMedicineSyncDataEditApiArgs args)
        {
            var worker = new NoteMedicineSyncDataEditWorker(
                new NoteMedicineRepository()
            );
            return ExecuteWorkerMethod(args, worker.Edit);
        }

        /// <summary>
        /// お薬手帳の同期用情報を削除します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("SyncDataDelete")]
        public QoNoteMedicineSyncDataDeleteApiResults PostSyncDataDelete(QoNoteMedicineSyncDataDeleteApiArgs args)
        {
            var worker = new NoteMedicineSyncDataDeleteWorker(
                new NoteMedicineRepository()
            );
            return ExecuteWorkerMethod(args, worker.Delete);
        }

        /// <summary>
        /// お薬手帳情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("Read")]
        public QoNoteMedicineReadApiResults PostRead(QoNoteMedicineReadApiArgs args)
        {
            var worker = new NoteMedicineWorker(new NoteMedicineRepository());
            return this.ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// お薬手帳情報を追加します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("Add")]
        public QoNoteMedicineAddApiResults PostAdd(QoNoteMedicineAddApiArgs args)
        {
            var worker = new NoteMedicineWorker(new NoteMedicineRepository());
            return this.ExecuteWorkerMethod(args, worker.Add);
        }

        /// <summary>
        /// お薬手帳情報を編集します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("Edit")]
        public QoNoteMedicineEditApiResults PostEdit(QoNoteMedicineEditApiArgs args)
        {
            var worker = new NoteMedicineWorker(new NoteMedicineRepository());
            return this.ExecuteWorkerMethod(args, worker.Edit);
        }

        /// <summary>
        /// お薬手帳情報を削除します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("Delete")]
        public QoNoteMedicineDeleteApiResults PostDelete(QoNoteMedicineDeleteApiArgs args)
        {
            var worker = new NoteMedicineWorker(new NoteMedicineRepository());
            return this.ExecuteWorkerMethod(args, worker.Delete);
        }

        /// <summary>
        /// ユーザーIDを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        //[ActionName("UserIdRead")]
        //public QoNoteMedicineUserIdReadApiResults PostUserIdRead(QoNoteMedicineUserIdReadApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, NoteMedicineWorker.UserIdRead);
        //}

        /// <summary>
        /// e薬Link用のワンタイムコードを生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("OnetimeCodeGenerate")]
        public QoNoteMedicineOnetimeCodeGenerateApiResults PostOnetimeCodeGenerate(QoNoteMedicineOnetimeCodeGenerateApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NoteMedicineWorker.OnetimeCodeGenerate);
        }

        /// <summary>
        /// お薬手帳の画像を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("ImageRead")]
        public QoNoteMedicineImageReadApiResults PostImageRead(QoNoteMedicineImageReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NoteMedicineWorker.ImageRead);
        }

        /// <summary>
        /// お薬手帳の画像を登録します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("ImageWrite")]
        public QoNoteMedicineImageWriteApiResults PostImageWrite(QoNoteMedicineImageWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NoteMedicineWorker.ImageWrite);
        }

        /// <summary>
        /// 最近のお薬情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("RecentMedicineRead")]
        public QoNoteMedicineRecentMedicineReadApiResults PostRecentMedicineRead(QoNoteMedicineRecentMedicineReadApiArgs args)
        {
            var worker = new NoteMedicineWorker(new NoteMedicineRepository());
            return this.ExecuteWorkerMethod(args, worker.RecentMedicineRead);
        }

        /// <summary>
        /// 指定日のお薬情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        [ActionName("DayRead")]
        public QoNoteMedicineDayReadApiResults PostDayRead(QoNoteMedicineDayReadApiArgs args)
        {
            var worker = new NoteMedicineReadWorker(new NoteMedicineRepository());
            return ExecuteWorkerMethod(args, worker.DayRead);
        }

        /// <summary>
        /// お薬手帳用の利用規約を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("TermsRead")]
        public QoNoteMedicineTermsReadApiResults PostTermsRead(QoNoteMedicineTermsReadApiArgs args)
        {
            var worker = new NoteMedicineWorker(new NoteMedicineRepository());
            return this.ExecuteWorkerMethod(args, worker.TermsRead);
        }
    }
}
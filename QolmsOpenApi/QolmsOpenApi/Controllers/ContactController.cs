using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 連絡手帳に関する機能を提供するAPIコントローラです。
    /// </summary>
    public class ContactController: QoApiControllerBase
    {
        /// <summary>
        /// 連絡手帳の情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Contact)]
        [ActionName("Read")]
        public QoContactReadApiResult PostRead(QoContactReadApiArgs args)
        {
            var worker = new ContactWorker(new ContactRepository());
            return ExecuteWorkerMethod(args, worker.ReadContact);
        }

        /// <summary>
        /// 連絡手帳の情報を設定します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Contact)]
        [ActionName("Write")]
        public QoContactWriteApiResult PostWrite(QoContactWriteApiArgs args)
        {
            var worker = new ContactWorker(new ContactRepository());
            return ExecuteWorkerMethod(args, worker.WriteContact);
        }
    }
}
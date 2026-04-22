using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 健診データ取り込み に関する機能を提供する API コントローラ です。
    /// </summary>
    public class CheckupFileController : QoApiControllerBase
    {

        /// <summary>
        /// フレイル健診データ取り込みをリクエストします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("FrailtyUpload")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtToken)]
        public QoCheckupFileFrailtyUploadApiResults PostFrailtyUpload(QoCheckupFileFrailtyUploadApiArgs args)
        {
            return base.ExecuteWorkerMethod(args, CheckupFileWorker.FrailtyUpload);
        }

        /// <summary>
        /// 介護情報データ取り込みをリクエストします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("CareUpload")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtToken)]
        public QoCheckupFileCareUploadApiResults PostCareUpload(QoCheckupFileCareUploadApiArgs args)
        {
            return base.ExecuteWorkerMethod(args, CheckupFileWorker.CareUpload);
        }

        /// <summary>
        /// 学童健診データ取り込みをリクエストします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("AfterSchoolUpload")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtToken)]
        public QoCheckupFileAfterSchoolUploadApiResults PostAfterSchoolUpload(QoCheckupFileAfterSchoolUploadApiArgs args)
        {
            return base.ExecuteWorkerMethod(args, CheckupFileWorker.AfterSchoolUpload);
        }
    }
}

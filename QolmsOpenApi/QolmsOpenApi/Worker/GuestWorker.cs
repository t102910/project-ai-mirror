using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// ゲスト関連の機能を提供します。
    /// </summary>
    public class GuestWorker
    {
        /// <summary>
        /// ゲスト用のアクセスキーを生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoGuestAccessKeyGenerateApiResults Generate(QoGuestAccessKeyGenerateApiArgs args)
        {
            QoGuestAccessKeyGenerateApiResults result = new QoGuestAccessKeyGenerateApiResults() { IsSuccess = bool.FalseString };
            result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(AccessKeyWorker.GetEncExeCutor(args.Executor), Guid.Empty, Guid.Empty, (int)QoApiFunctionTypeEnum.GuestUser );
            //    result.AccessKey = JwtAuthenticateWorker.CreateAccessKey(QoApiAuthorizeTypeEnum.JwtAccessKey, Guid.Empty, QoApiFunctionTypeEnum.Guest, DateTime.Now.AddDays(30), Guid.Empty);
            result.IsSuccess = bool.TrueString;

            return result;
        }

        /// <summary>
        /// ゲスト用のアクセスキーを再生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QoGuestAccessKeyRefreshApiResults Refresh(QoGuestAccessKeyRefreshApiArgs args)
        {
            QoGuestAccessKeyRefreshApiResults result = new QoGuestAccessKeyRefreshApiResults() { IsSuccess = bool.FalseString };
            result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(AccessKeyWorker.GetEncExeCutor(args.Executor), Guid.Empty, Guid.Empty, (int)QoApiFunctionTypeEnum.GuestUser);
            //result.AccessKey = JwtAuthenticateWorker.CreateAccessKey(QoApiAuthorizeTypeEnum.JwtAccessKey, Guid.Empty, QoApiFunctionTypeEnum.Guest, DateTime.Now.AddDays(30), Guid.Empty);
            result.IsSuccess = bool.TrueString;

            return result;        // 
        }
    }
    

}
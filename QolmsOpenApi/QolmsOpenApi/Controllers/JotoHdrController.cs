
using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// JOTO関連の公開API
    /// </summary>
    public class JotoHdrController : QoApiControllerBase
    {
        //[QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrMemberStatus)]
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey)]
        //[ActionName("MemberStatus")]
        //public QoJotoHdrMemberStatusApiResults PostMemberStatus(QoJotoHdrMemberStatusApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, JotoHdrWorker.MemberStatus);
        //}

        ///// <summary>
        ///// MEXからの呼び出しのアクションテストを行います（データは一切関係なく、純粋にアクションメソッドへの到達テスト用）。
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //[QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrMemberStatus)]
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey)]
        //[ActionName("MemberStatusTest")]
        //public QoJotoHdrMemberStatusApiResults PostMemberStatusTest(QoJotoHdrMemberStatusApiArgs args)
        //{
        //    return new QoJotoHdrMemberStatusApiResults() { IsSuccess = bool.TrueString, Result = new QoApiResultItem() { Code = "200", Detail = "テスト成功" } };
        //}
        /// <summary>
        /// JOTO用トークンの発行
        /// </summary>
        /// <returns></returns>
        [ActionName("GenerateToken")]
        public string GetGenerateToken(string executor)
        {
            // v1/support/GetIsDebug?debug=trueを呼んでデバッグモードに設定するとログを吐きます。終わったらdebug=falseで戻します。
            if (executor.TryToValueType(Guid.Empty) == Guid.Empty)
                return "Executorが不正です";

            QoAccessLog.WriteInfoLog( new QsJwtTokenProvider().CreateOpenApiJwtApiKey(executor, (int)QoApiFunctionTypeEnum.JotoHdr, System.DateTime.Now.AddYears(10)));
            return "サーバー側で確認してください。";
        }

        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoHdr)]
        //[ActionName("MemberShip")]
        //public QoJotoHdrMemberShipApiResults PostMemberShip(QoJotoHdrMemberShipApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, JotoHdrWorker.MemberShip);
        //}

        ///// <summary>
        ///// アクセスキー取得を行います。
        ///// </summary>
        ///// <param name="args">Web API 引数クラス。</param>
        ///// <returns>Web API 戻り値クラス。</returns>
        ///// <remarks></remarks>
        //[ActionName("AccessKeyGenerate")]
        //public QoJotoHdrAccessKeyGenerateApiResults PostAccessKeyGenerate(QoJotoHdrAccessKeyGenerateApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, JotoHdrWorker.AccessKeyGenerate);
        //}

        ///// <summary>
        ///// JOTO ALKOOからのポイント付与の処理を行います。
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //[QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrPointWrite)]
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey)]
        //[ActionName("PointWrite")]
        //public QoJotoHdrPointWriteApiResults PostPointWrite(QoJotoHdrPointWriteApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, JotoHdrWorker.PointWrite);
        //}

        ///// <summary>
        ///// JOTO ALKOOからのポイント付与のアクションテストを行います（データは一切関係なく、純粋にアクションメソッドへの到達テスト用）。
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //[QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrPointWrite)]
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey)]
        //[ActionName("PointWriteTest")]
        //public QoJotoHdrPointWriteApiResults PostPointWriteTest(QoJotoHdrPointWriteApiArgs args)
        //{
        //    return new QoJotoHdrPointWriteApiResults() { IsSuccess = bool.TrueString, Result = new QoApiResultItem() { Code = "200", Detail = "テスト成功" } };
        //}

        ///// <summary>
        ///// JOTO ALKKOOからのイベント参加者情報書き込みを行います。
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //[QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrChallengeEntryWrite)]
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey)]
        //[ActionName("ChallengeEntryWrite")]
        //public QoJotoHdrChallengeEntryWriteApiResults PostChallengeEntryWrite(QoJotoHdrChallengeEntryWriteApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, JotoHdrWorker.ChallengeEntryWrite);
        //}

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        //[QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrPushSend)]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey,QoApiFunctionTypeEnum.JotoHdr)]
        [ActionName("PushSend")]
        public QoJotoHdrPushSendApiResults PostPush(QoJotoHdrPushSendApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, JotoHdrWorker.PushSend);
        }

        /// <summary>
        /// JOTO 心臓見守りアプリへアラート情報を返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrHeartRateWarningRead )]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey, QoApiFunctionTypeEnum.JotoHdr)]
        [ActionName("HeartRateWarningRead")]
        public QoJotoHdrHeartRateWarningReadApiResults PostHeartRateWarningRead([FromBody]QoJotoHdrHeartRateWarningReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, JotoHdrWorker.HeartRateWarningRead);
        }
        /// <summary>
        /// JOTO 心臓見守りアプリへアラート情報のアクション到達テスト用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.JotoHdrHeartRateWarningRead)]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey, QoApiFunctionTypeEnum.JotoHdr)]
        [ActionName("HeartRateWarningReadTest")]
        public QoJotoHdrHeartRateWarningReadApiResults PostHeartRateWarningReadTest([FromBody]QoJotoHdrHeartRateWarningReadApiArgs args)
        {
            return new QoJotoHdrHeartRateWarningReadApiResults() { IsSuccess = bool.TrueString, Result = new QoApiResultItem() { Code = "200", Detail = "テスト成功" } };
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤から 呼び出される。イベント 情報受取。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoIpFilter(QoApiTypeEnum.JotoHdrEventWrite)]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey, QoApiFunctionTypeEnum.JotoHdr)]
        [ActionName("EvntInfoSend")]
        public QoJotoHdrEventInfoWriteApiResults PostEvntInfoSend([FromBody] QoJotoHdrEventInfoWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, JotoHdrWorker.EventInfoWrite);
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤 API を呼び出し、合計地域ポイントを受け取ります。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoIpFilter(QoApiTypeEnum.JotoHdrLocalPointSummaryRead)]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoHdr| QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("LocalPointSummaryRead")]
        public QoJotoHdrLocalPointSummaryReadApiResults PostLocalPointSummaryRead([FromBody] QoJotoHdrLocalPointSummaryReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, JotoHdrWorker.LocalPointSummaryRead);
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤 API を呼び出し、ポイント交換を行い、結果を受け取ります。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoIpFilter(QoApiTypeEnum.JotoHdrLocalPointRedeemRead)]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoHdr | QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("LocalPointRedeemRead")]
        public QoJotoHdrLocalPointRedeemReadApiResults PostLocalPointRedeemRead([FromBody] QoJotoHdrLocalPointRedeemReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, JotoHdrWorker.LocalPointRedeemRead);
        }

        /// <summary>
        /// ぎのわん PJ データ 基盤 API を呼び出し、地域ポイント履歴を受け取ります。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoIpFilter(QoApiTypeEnum.JotoHdrLocalPointHistoryRead)]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoHdr | QoApiFunctionTypeEnum.JotoApp)]
        [ActionName("LocalPointHistoryRead")]
        public QoJotoHdrLocalPointHistoryReadApiResults PostLocalPointHistoryRead([FromBody] QoJotoHdrLocalPointHistoryReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, JotoHdrWorker.LocalPointHistoryRead);
        }
    }
}

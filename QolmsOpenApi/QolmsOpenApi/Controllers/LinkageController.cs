using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MGF.QOLMS.QolmsJwtAuthCore;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 連携ユーザに関する機能を提供する API コントローラ です。
    /// </summary>
    public sealed class LinkageController : QoApiControllerBase
    {
        /// <summary>
        /// <see cref="LinkageController" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public LinkageController() : base() { }

        /// <summary>
        /// 連携ユーザーとしてアカウント本登録を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("NewUserRegister")]
        public QoLinkageNewUserRegisterApiResults PostNewUserRegister(QoLinkageNewUserRegisterApiArgs args)
        {
            var worker = new LinkageRegisterWorker(
                new LinkageRepository(),
                new IdentityApiRepository(),
                new SignUpRepository(),
                new NoticeApiRepository(),
                new AccountRepository()
            );

            return ExecuteWorkerMethod(args, worker.RegisterNewUser);
        }

        /// <summary>
        /// SMS認証を使用して連携ユーザーとしてアカウント本登録を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("NewUserRegisterForSms")]
        public async Task<QoLinkageNewUserRegisterForSmsApiResults> PostNewUserRegisterForSms(QoLinkageNewUserRegisterForSmsApiArgs args)
        {
            var worker = new LinkageRegisterForSmsWorker(
                new LinkageRepository(),
                new IdentityApiRepository(),
                new AccountRepository(),
                new SmsAuthCodeRepository(),
                new QoSmsClient());

            return await ExecuteWorkerMethodAsync(args, worker.RegisterNewUser);
        }

        /// <summary>
        /// 連携ユーザー登録 / 診察券登録のQRコードを復号化します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("QrCodeRead")]
        public QoLinkageQrCodeReadApiResults PostQrCodeRead(QoLinkageQrCodeReadApiArgs args)
        {
            var worker = new LinkageQrReadWorker(new FacilityRepository());
            return ExecuteWorkerMethod(args, worker.QrRead);
        }

        /// <summary>
        /// アカウント本登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>        
        [Obsolete("このAPIは廃止予定です。HOSPA1.0.5まででしか使用しません。")]
        [ActionName("UserRegister")]
        public QoLinkageUserRegisterApiResults PostUserRegister(QoLinkageUserRegisterApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, LinkageWorker.UserRegister);
        }

        /// <summary>
        /// 診察券登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("PatientCardAdd")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoLinkagePatientCardAddApiResults PostPatientCardAdd(QoLinkagePatientCardAddApiArgs args)
        {
            var worker = new LinkagePatientCardAddWorker(
                new LinkageRepository(),
                new AccountRepository(),
                new FamilyRepository(), 
                new PatientCardRepository(),
                new StorageRepository()
            );
            return this.ExecuteWorkerMethod(args, worker.Add);
        }
        /// <summary>
        /// 診察券リスト取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("PatientCardListRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoLinkagePatientCardListReadApiResults PostPatientCardListRead(QoLinkagePatientCardListReadApiArgs args)
        {
            var worker = new LinkagePatientCardListWorker(new PatientCardRepository());
            return this.ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 診察券削除を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("PatientCardDelete")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoLinkagePatientCardDeleteApiResults PostPatientCardDelete(QoLinkagePatientCardDeleteApiArgs args)
        {
            var worker = new LinkagePatientCardDeleteWorker(new LinkageRepository(), new AccountRepository());
            return this.ExecuteWorkerMethod(args, worker.Delete);
        }

        /// <summary>
        /// LinkageSystemId の取得を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("LinkageSystemIdRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.JotoHdr | QoApiFunctionTypeEnum.JotoApp)]
        public QoLinkageLinkageSystemIdReadApiResults PostLinkageSystemIdRead(QoLinkageLinkageSystemIdReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, LinkageWorker.LinkageSystemIdRead);
        }

        /// <summary>
        /// LinkageSystemIdの登録を行います。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("LinkageSystemIdWrite")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        public QoLinkageLinkageSystemIdWriteApiResults PostLinkageSystemIdWrite(QoLinkageLinkageSystemIdWriteApiArgs args)
        {
            var worker = new LinkageSystemIdWriteWorker(
                new LinkageRepository()
            );

            return this.ExecuteWorkerMethod(args, worker.Write);
        }
    }
}

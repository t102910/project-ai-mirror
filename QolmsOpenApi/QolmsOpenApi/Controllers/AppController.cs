using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// アプリ関連の機能を提供します。
    /// </summary>
    public class AppController : QoApiControllerBase
    {
        /// <summary>
        /// 死活監視用
        /// </summary>
        /// <returns></returns>
        [ActionName("Ping")]
        public IHttpActionResult GetPing()
        {
           /*
            foreach (var item in Request.Headers)
            {
                QoAccessLog.WriteInfoLog(string.Format("{0}:{1}", item.Key,string.Join(",", item.Value)));
            }*/
            return new OkResult(this) ;
        }

        /// <summary>
        /// アカウント仮登録URLスキームでアプリを起動します。
        /// </summary>
        /// <param name="a">アカウントキー参照文字列。</param>
        /// <param name="p">未使用。</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [ActionName("Signup")]
        public IHttpActionResult GetSignup(string a, string p)
        {
            string scheme = AppWorker.GetUrlScheme(p);
            if (!string.IsNullOrWhiteSpace(scheme) && !string.IsNullOrWhiteSpace(a))
            {
                string uri = string.Format("{0}signup?accountkey={1}", scheme, a);

                return Redirect(uri);
            }
            return new NotFoundResult(this);
        }


        /// <summary>
        /// パスワードリセットURLスキームでアプリを起動します。
        /// </summary>
        /// <param name="d">ユーザーID参照文字列。</param>
        /// <param name="h">パスワードハッシュ。</param>
        /// <param name="p">アプリを表すパラメータ。</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [ActionName("ResetPw")]
        public IHttpActionResult GetResetPw(string d, string h, string p = "")
        {
            string scheme = AppWorker.GetUrlScheme(p);

            if (!string.IsNullOrWhiteSpace(scheme) && !string.IsNullOrWhiteSpace(d) && !string.IsNullOrWhiteSpace(h))
            {
                string uri = string.Format("{0}passwordreset?id={1}&hs={2}", scheme, d, h);
                return Redirect(uri);
            }

            return new NotFoundResult(this);
        }

        /// <summary>
        /// メールアドレス変更URLスキームでアプリを起動します。
        /// </summary>
        /// <param name="d">メールアドレス参照文字列。</param>
        /// <param name="k">アカウントキー参照文字列。</param>
        /// <param name="p">アプリを表すパラメータ。</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [ActionName("ChMail")]
        public IHttpActionResult GetChMail(string d, string k, string p = "")
        {
            string scheme = AppWorker.GetUrlScheme(p);

            string uri = AppWorker.ChangeMail(scheme, d, k);

            if (!string.IsNullOrWhiteSpace(uri))
                return Redirect((uri));

            return new NotFoundResult(this);
        }

        /// <summary>
        /// 最新アプリバージョン情報を取得します。
        /// </summary>
        /// <param name="os"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [ActionName("Version")]
        public VersionJsonResult GetVersion(string os, string p="")
        {
            string jsonFileName = AppWorker.GetVersionFileName(os, p);
            VersionJsonResult result;
            try
            {
                string tmp = System.IO.File.ReadAllText(jsonFileName);
                result = new QsJsonSerializer().Deserialize<VersionJsonResult>(tmp);

            }
            catch (Exception ex)
            {
                result = new VersionJsonResult();
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }
            

            return result;
        }
        
        /// <summary>
        /// 薬局画像を取得します。
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="isOriginal"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [ActionName("FacilityImage")]
        public HttpResponseMessage GetFacilityImage(string fileKey, bool isOriginal = true)
        {
            if (string.IsNullOrWhiteSpace(fileKey))
                return Request.CreateResponse(HttpStatusCode.NotFound);

            byte[] data = null;
            string contentType = "image/jpg";
            string filename = "temp.jpg";
            if (FacilityWorker.FileStorageRead(fileKey.ToDecrypedReference(), isOriginal, ref data, ref contentType, ref filename))
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(data)
                };
                //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = filename
                //};これ入れるとブラウザではダウンロード扱いになる。
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream")
                //{
                //    MediaType = contentType
                //};
                
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(contentType);
                
                return result;
            }


            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// 画像をダウンロードする(施設画像は除く)
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="sizeType"></param>
        /// <param name="fileCategory"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.All)]
        [ActionName("DownloadImage")]
        public HttpResponseMessage GetDownloadImage(string fileKey, QsApiFileTypeEnum sizeType = QsApiFileTypeEnum.Original, 
            QsApiFileCategoryTyepEnum fileCategory = QsApiFileCategoryTyepEnum.Upload, int seq = 0)
        {
            var worker = new AppDownloadImageWorker(new StorageRepository(),new MedicineStorageRepository());
            return worker.DownloadImage(fileKey, sizeType, fileCategory, seq);
        }

        /// <summary>
        /// 個人宛お知らせURLスキームでアプリを起動します。
        /// </summary>
        /// <param name="n">お知らせ番号参照文字列。</param>
        /// <param name="k">アカウントキー参照文字列。</param>
        /// <param name="p">アプリを表すパラメータ。</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [ActionName("SendNotice")]
        public IHttpActionResult GetSendNotice(string n, string k, string p = "")
        {
            string scheme = AppWorker.GetUrlScheme(p);

            if (!string.IsNullOrWhiteSpace(scheme) && !string.IsNullOrWhiteSpace(n) && !string.IsNullOrWhiteSpace(k))
            {
                string uri = string.Format("{0}notification?no={1}&user={2}", scheme, n, k);
                return Redirect(uri);
            }

            return new NotFoundResult(this);
        }

        /// <summary>
        /// OAuth認証用のコールバックURLを返す
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [ActionName("OAuthCallback")]
        public IHttpActionResult GetOAuthCallback(string p = "")
        {
            var scheme = AppWorker.GetUrlScheme(p);

            if (!string.IsNullOrWhiteSpace(scheme))
            {
                // schemeに-oauthを付加する
                scheme = scheme.Replace(":", "-oauth:");
                var uri = $"{scheme}callback";
                return Redirect(uri);
            }

            return new NotFoundResult(this);
        }

        /// <summary>
        /// 医療ナビ初期データ取得API
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.MedicalNavi)]
        [ActionName("MedicalNaviInit")]
        public QoAppMedicalNaviInitApiResults PostMedicalNaviInit(QoAppMedicalNaviInitApiArgs args)
        {
            var worker = new AppMedicalNaviInitWorker(
                new FacilityRepository(), 
                new PatientCardRepository(), 
                new FamilyRepository(),
                new AccountRepository()
            );
            return ExecuteWorkerMethod(args, worker.GetInitData);
        }

        /// <summary>
        /// 心拍見守りアプリ初期データ取得API
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.HeartMonitor)]
        [ActionName("HeartMonitorInit")]
        public QoAppHeartMonitorInitApiResults PostHeartMonitorInit(QoAppHeartMonitorInitApiArgs args)
        {
            var worker = new AppHeartMonitorInitWorker(
                new AccountRepository(),
                new AppSettingsRepository()
            );
            return ExecuteWorkerMethod(args, worker.GetInitData);
        }

        /// <summary>
        /// Live2DWebアプリ ログインと初期データ取得API
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtToken)]
        [ActionName("Live2DInit")]
        public QoAppLive2DInitApiResults PostLive2DInit(QoAppLive2DInitApiArgs args)
        {
            var worker = new AppLive2DInitWorker(
                new PasswordManagementRepository(),
                new AccountRepository(),
                new AppSettingsRepository(),
                new QkRandomAdviceRepository(),
                new IdentityApiRepository());

            return ExecuteWorkerMethod(args, worker.LoginAndLoadData);
        }

        /// <summary>
        /// アプリ設定取得API
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.All)]
        [ActionName("SettingsRead")]
        public QoAppSettingsReadApiResults PostSettingsRead(QoAppSettingsReadApiArgs args)
        {
            var worker = new AppSettingsReadWorker(new AppSettingsRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// アプリ設定書き込みAPI
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.All)]
        [ActionName("SettingsWrite")]
        public QoAppSettingsWriteApiResults PostSettingsWrite(QoAppSettingsWriteApiArgs args)
        {
            var worker = new AppSettingsWriteWorker(new AppSettingsRepository());
            return ExecuteWorkerMethod(args, worker.Write);
        }

        /// <summary>
        /// PushNotificationのインストール情報を削除します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.All)]
        [ActionName("PushNotificationEntryDelete")]
        public async Task<QoAppPushNotificationEntryDeleteApiResults> PostPushNotificationEntryDelete(QoAppPushNotificationEntryDeleteApiArgs args)
        {
            var worker = new AppPushNotificationEntryDeleteWorker(new QoPushNotification());
            return await ExecuteWorkerMethodAsync(args, worker.DeleteAsync);
        }

        /// <summary>
        /// アプリ審査用認証API
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("ReviewAuth")]
        public QoAppReviewAuthApiResults PostReviewAuth(QoAppReviewAuthApiArgs args)
        {
            var worker = new AppReviewAuthWorker();
            return ExecuteWorkerMethod(args, worker.Auth);
        }
    }
}

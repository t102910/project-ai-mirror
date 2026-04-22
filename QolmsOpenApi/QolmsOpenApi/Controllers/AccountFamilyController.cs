using System;
using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{

    /// <summary>
    /// 子アカウントに関する機能を提供する API コントローラ です。
    /// </summary>
    /// <remarks></remarks>
    public class AccountFamilyController : QoApiControllerBase
    {

        /// <summary>
        /// 利用者一覧を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [Obsolete("このAPIは廃止予定です。代わりにUser/ListReadを使用してください。")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("ListRead")]
        public QoAccountFamilyListReadApiResults PostListRead(QoAccountFamilyListReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountFamilyWorker.ListRead);
        }

        /// <summary>
        /// 利用者（子アカウント）を追加します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [Obsolete("このAPIは廃止予定です。代わりにUser/FamilyAddを使用してください。")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Add")]
        public QoAccountFamilyAddApiResults PostAdd(QoAccountFamilyAddApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountFamilyWorker.Add);
        }

        /*
        /// <summary>
        /// 利用者情報詳細を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("DetailRead")]
        public QoAccountFamilyDetailReadApiResults PostDetailRead(QoAccountFamilyDetailReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountFamilyWorker.DetailRead);
        }
        */
        /// <summary>
        /// 利用者情報詳細を更新します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [Obsolete("このAPIは廃止予定です。代わりにUser/Writeを使用してください。")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("DetailWrite")]
        public QoAccountFamilyDetailWriteApiResults PostDetailWrite(QoAccountFamilyDetailWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountFamilyWorker.DetailWrite);
        }

        /// <summary>
        /// 利用者（子アカウント）を削除します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [Obsolete("このAPIは廃止予定です。代わりにUser/FamilyDeleteを使用してください。")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Delete")]
        public QoAccountFamilyDeleteApiResults PostDelete(QoAccountFamilyDeleteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountFamilyWorker.Delete);
        }

        /// <summary>
        /// 利用者写真を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("PersonPhotoRead")]
        public QoAccountFamilyPersonPhotoReadApiResults PostPersonPhotoRead(QoAccountFamilyPersonPhotoReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountFamilyWorker.PersonPhotoRead);
        }

        /// <summary>
        /// 利用者写真を更新します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("PersonPhotoWrite")]
        public QoAccountFamilyPersonPhotoWriteApiResults PostPersonPhotoWrite(QoAccountFamilyPersonPhotoWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccountFamilyWorker.PersonPhotoWrite);
        }
    }


}

using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「市民確認 同意」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class PortalLocalIdVerificationRegisterWorker
    {

        ILocalIdVerificationRepository _localIdVerificationRepo;

        #region Constant

        #endregion

        #region Constructor

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        public PortalLocalIdVerificationRegisterWorker(ILocalIdVerificationRepository localIdVerificationRepository)
        {
            _localIdVerificationRepo = localIdVerificationRepository;
        }

        #endregion

        #region Private Method


        #endregion

        #region Public Method

        /// <summary>
        /// 市民確認登録画面モデルを取得します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <returns>
        /// 成功なら True、失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public PortalLocalIdVerificationRegisterInputModel CreateViewModel(QolmsJotoModel mainModel)
        {
            PortalLocalIdVerificationRegisterInputModel result = new PortalLocalIdVerificationRegisterInputModel();

            // メールアドレスと電話番号はここから修正させる？（できればデフォルトのユーザー情報編集画面からしてほしい）

            QjPortalLocalIdVerificationRegisterReadApiResults apiResult = _localIdVerificationRepo.ExecutePortalLocalIdVerificationRegisterReadApi(mainModel);

            // 編集モードで開く(LinkageSystemNo>0)
            // 編集だったら中止できるように表示する
            result.LinkageSystemNo = apiResult.LinkageSystemNo.TryToValueType(int.MinValue);
            result.LinkageSystemId = apiResult.LinkageSystemId;
            result.Status = apiResult.Status.TryToValueType(byte.MinValue);
            // メールアドレスとでんわ番号とってくる
            result.MailAddress = apiResult.MailAddress;
            result.PhoneNumber = apiResult.PhoneNumber;

            result.RelationContentFlags = apiResult.RelationContentFlags.TryToValueType(QjRelationContentTypeEnum.None);
            // 画面の元の値をキャッシュ
            mainModel.SetInputModelCache(result);

            return result;
        }

        /// <summary>
        /// 市民確認エントリーを登録した結果を返却します。
        /// </summary>
        /// <param name="mainModel"></param>
        /// <returns></returns>
        public bool Register(QolmsJotoModel mainModel, PortalLocalIdVerificationRegisterInputModel input, ref string errorMessage)
        {
            bool deleteFlag = false;
            QjPortalLocalIdVerificationRegisterWriteApiResults apiResult = _localIdVerificationRepo.ExecutePortalLocalIdVerificationRegisterWriteApi(mainModel, input, deleteFlag);

            errorMessage = apiResult.ErrorMessage;
            // 登録結果
            return apiResult.IsSuccess.TryToValueType(false) && string.IsNullOrWhiteSpace(errorMessage);
        }

        internal bool Cancel(QolmsJotoModel mainModel, ref string errorMessage)
        {
            // 画面の元の値キャッシュを
            PortalLocalIdVerificationRegisterInputModel inp = mainModel.GetInputModelCache<PortalLocalIdVerificationRegisterInputModel>();

            bool deleteFlag = true;
            QjPortalLocalIdVerificationRegisterWriteApiResults apiResult = _localIdVerificationRepo.ExecutePortalLocalIdVerificationRegisterWriteApi(mainModel, inp, deleteFlag);

            errorMessage = apiResult.ErrorMessage;
            // 登録結果
            return apiResult.IsSuccess.TryToValueType(false) && string.IsNullOrWhiteSpace(errorMessage);
        }

        #endregion
    }
}

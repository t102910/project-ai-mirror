using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Configuration;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{

    /// <summary>
    /// 「市民確認 同意」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class PortalLocalIdVerificationRequestWorker
    {
        ILinkageRepository _linkageRepo;

        #region Constant
        #endregion

        #region Constructor

        public PortalLocalIdVerificationRequestWorker(ILinkageRepository linkageRepository)
        {
            _linkageRepo = linkageRepository;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 設定を取得します。
        /// </summary>
        /// <returns></returns>
        private static string GetSetting(string settingName)
        {
            string result = string.Empty;
            string value = ConfigurationManager.AppSettings[settingName];

            if (value != null && !string.IsNullOrWhiteSpace(value))
            {
                result = value;
            }

            return result;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public PortalLocalIdVerificationRequestViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            var result = new PortalLocalIdVerificationRequestViewModel();

            LinkageItem linkageItem = _linkageRepo.ExecuteLinkageReadApi(mainModel, QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO);

            if (linkageItem.LinkageSystemNo == QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO) // 連携がすでにあるかどうか
            {
                result.LinkageSystemNo = linkageItem.LinkageSystemNo;
                result.LinkageSystemId = linkageItem.LinkageSystemId;
                result.LinkageSystemName = linkageItem.LinkageSystemName;
                result.Status = linkageItem.StatusType;

                try
                {
                    var ser = new QsJsonSerializer();
                    var dataset = ser.Deserialize<QhLinkageDataSetOfJson>(linkageItem.Dataset);

                    result.Reason = dataset.DisapprovedReason;
                }
                catch (Exception ex)
                {
                }
            }

            result.Url = HttpUtility.UrlEncode(QjConfiguration.GinowanApplyUrl);

            return result;
        }

        #endregion
    }

}
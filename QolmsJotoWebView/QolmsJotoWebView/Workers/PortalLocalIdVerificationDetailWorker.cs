using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「市民確認 確認」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class PortalLocalIdVerificationDetailWorker
    {

        ILinkageRepository _linkageRepo;

        public PortalLocalIdVerificationDetailWorker(ILinkageRepository linkageRepository)
        {
            _linkageRepo = linkageRepository;
        }

        #region Public Method

        /// <summary>
        /// チャレンジエントリー画面モデルを取得します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public PortalLocalIdVerificationDetailViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            var result = new PortalLocalIdVerificationDetailViewModel();

            LinkageItem linkageItem = _linkageRepo.ExecuteLinkageReadApi(mainModel, QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO);

            if (linkageItem.LinkageSystemNo == QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO)
            {
                result.LinkageSystemNo = linkageItem.LinkageSystemNo;
                result.LinkageSystemName = linkageItem.LinkageSystemName;
                result.Status = linkageItem.StatusType;

                switch (result.Status)
                {
                    case QjLinkageStatusTypeEnum.Applying:
                        // result.Reason = "JOTO連携IDが発行済みです。参加申請がお済みの方は、申請結果の通知をお待ちください。申請がお済みでない方は、「申請画面に戻る」より参加申請を行ってください。申請状況は、ぴったりサービスよりご確認いただけます。​"
                        break;
                    case QjLinkageStatusTypeEnum.Approved:
                        // result.Reason = "ぎのわんスマート健康増進プロジェクトへのエントリーが完了しました。"
                        break;
                    case QjLinkageStatusTypeEnum.Refused:
                        // result.Reason = "宜野湾市民であることの確認ができませんでした。本エントリーは宜野湾市民限定となっております。ご不明な点がございましたら、プロジェクト事務局（098-880-2469）までお問い合わせください。​"
                        break;
                }

                try
                {
                    var ser = new QsJsonSerializer();
                    var dataset = ser.Deserialize<QhLinkageDataSetOfJson>(linkageItem.Dataset);

                    result.Reason += dataset.DisapprovedReason;
                }
                catch (Exception ex)
                {
                    // 例外処理（ここでは何もしない）
                }
            }

            return result;
        }

        #endregion
    }

}
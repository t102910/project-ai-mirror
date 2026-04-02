using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class PortalLocalIdVerificationAgreementWorker
    {

        ITermsRepository _termsRepo;

        const int TARMS_NO = 106;//設定に外だしを検討（規約の番号が環境で変わってもいいように）

        public PortalLocalIdVerificationAgreementWorker(ITermsRepository termsRepository) 
        {
            _termsRepo = termsRepository;
        }

        /// <summary>
        /// 市民確認規約画面の画面モデルを取得します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <returns>
        /// 成功なら True、失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public PortalLocalIdVerificationAgreementViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            PortalLocalIdVerificationAgreementViewModel result = new PortalLocalIdVerificationAgreementViewModel();
            result.Terms = _termsRepo.GetTermsContent(mainModel, TARMS_NO).Contents;

            return result;
        }

    }
}
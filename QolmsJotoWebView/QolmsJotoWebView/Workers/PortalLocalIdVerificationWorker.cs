using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class PortalLocalIdVerificationWorker
    {
        ILinkageRepository _linkageRepo;

        public PortalLocalIdVerificationWorker(ILinkageRepository linkageRepository)
        {
            _linkageRepo = linkageRepository;
        }

        /// <summary>
        /// ぎのわんPJに参加中かどうかを判定します。
        /// </summary>
        /// <param name="mainModel"></param>
        /// <returns></returns>
        public bool IsEntered(QolmsJotoModel mainModel)
        {
            var ginowanEntiry = _linkageRepo.ExecuteLinkageReadApi(mainModel, QjLinkageSystemNo.JOTO_GINOWANENTRY_SYSTEM_NO); //エントリー
            var ginowan = _linkageRepo.ExecuteLinkageReadApi(mainModel, QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO);//承認済みかどうか？

            //どっちの連携もある状態でぎのわん参加中扱い
            return ginowanEntiry.LinkageSystemNo == QjLinkageSystemNo.JOTO_GINOWANENTRY_SYSTEM_NO 
                && ginowan.LinkageSystemNo == QjLinkageSystemNo.JOTO_GINOWAN_SYSTEM_NO;
        }


    }
}
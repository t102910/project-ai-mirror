using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// Point 系画面ビューモデルの基本クラスを表します。
    /// </summary>
    [Serializable()]
    public abstract class QjPointAmazonGiftCardViewModelBase : QjPageViewModelBase
    {

        #region "Constructor"

        public QjPointAmazonGiftCardViewModelBase(QolmsJotoModel mainModel, QjPageNoTypeEnum pageNo) : base(mainModel, pageNo) { }

        //検証用
        public QjPointAmazonGiftCardViewModelBase() : base() { }

        #endregion
    }
}
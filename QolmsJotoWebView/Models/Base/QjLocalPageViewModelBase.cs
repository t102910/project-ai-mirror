using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 地域Point 系画面ビューモデルの基本クラスを表します。
    /// </summary>
    [Serializable()]
    public abstract class QjLocalPageViewModelBase: QjPageViewModelBase
    {

        #region "Constructor"

        public QjLocalPageViewModelBase(QolmsJotoModel mainModel, QjPageNoTypeEnum pageNo ) : base(mainModel, pageNo) { }

        //検証用
        public QjLocalPageViewModelBase() : base() { }

        #endregion
    }
}
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// デモ画面ビューモデルの基本クラスを表します。
    /// </summary>
    [Serializable()]
    public abstract class QjDemoPageViewModelBase: QjPageViewModelBase
    {

        #region "Constructor"

        public QjDemoPageViewModelBase(QolmsJotoModel mainModel, QjPageNoTypeEnum pageNo ) : base(mainModel, pageNo) { }

        //検証用
        public QjDemoPageViewModelBase() : base() { }

        #endregion
    }
}
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// Note 系画面ビューモデルの基本クラスを表します。
    /// </summary>
    [Serializable()]
    public abstract class QjNotePageViewModelBase : QjPageViewModelBase
    {

        #region "Constructor"

        public QjNotePageViewModelBase(QolmsJotoModel mainModel, QjPageNoTypeEnum pageNo) : base(mainModel, pageNo) { }

        //検証用
        public QjNotePageViewModelBase() : base() { }

        #endregion
    }
}
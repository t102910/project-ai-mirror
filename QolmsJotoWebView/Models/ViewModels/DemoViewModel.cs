using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// デモ画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class DemoViewModel : QjDemoPageViewModelBase
    {
        #region "Public Property"

        /// <summary>
        /// 画面名を取得または設定します。
        /// </summary>
        public string PageName { get; set; } = string.Empty;

        #endregion

        #region "Constructor"

        public DemoViewModel(QolmsJotoModel mainModel) : base(mainModel, QjPageNoTypeEnum.Demo) { }

        //検証用
        public DemoViewModel() : base() { }

        #endregion
    }
}

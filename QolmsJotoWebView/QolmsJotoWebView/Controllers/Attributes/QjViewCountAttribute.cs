using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アクション メソッド の実行時に、
    /// 現在の セッション 内での画面表示回数を保持するかを指定する属性を表します。
    /// この クラス は継承できません。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple= false)]
    public class QjViewCountAttribute: Attribute
    {
        #region "Public Property"

        /// <summary>
        /// 画面番号の種別を取得または設定します。
        /// </summary>
        public QjPageNoTypeEnum PageNo { get; set; } = QjPageNoTypeEnum.None;

        #endregion

        #region "Constructor"

        public QjViewCountAttribute() : base() { }

        public QjViewCountAttribute(QjPageNoTypeEnum pageNo) : base() 
        {
            this.PageNo = PageNo;
        }

        #endregion
    }
}
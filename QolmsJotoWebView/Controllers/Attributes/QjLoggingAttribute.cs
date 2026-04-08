using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アクションメソッドの実行時に、
    /// アクセス ログを出力するかを指定する属性を表します。
    /// このクラスは継承できません。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple= false)]
    public class QjLoggingAttribute: Attribute
    {
        #region "Constructor"
        public QjLoggingAttribute() : base() { }

        #endregion
    }
}
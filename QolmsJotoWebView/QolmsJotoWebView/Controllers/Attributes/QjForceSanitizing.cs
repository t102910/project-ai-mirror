using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「JOTO ホーム ドクター」で使用する <see cref="QjJsonResultBase" /> クラス の プロパティ が、
    /// サニタイズ の対象かを指定する属性を表します。
    /// この クラス は継承できません。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple =false)]
    internal sealed class QjForceSanitizing: Attribute
    {

        #region "Constructor"
        public QjForceSanitizing() : base() { }

        #endregion
    }
}
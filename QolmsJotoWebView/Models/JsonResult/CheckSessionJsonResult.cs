using System;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// セッションが有効かチェックした結果を保持する、
    /// JSON 形式のコンテンツを表します。
    /// このクラスは継承できません。
    /// </summary>
    [DataContract()]
    [Serializable]
    public sealed class CheckSessionJsonResult: QjJsonResultBase
    {
        #region "Constructor"
        public CheckSessionJsonResult() : base() { }

        #endregion

    }
}
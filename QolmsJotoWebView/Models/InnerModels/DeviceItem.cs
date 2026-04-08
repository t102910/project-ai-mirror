using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// カラダカルテのデバイスを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class DeviceItem
    {
        /// <summary>
        /// デバイス名を取得または設定します。
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// デバイスプロパティ名を取得または設定します。
        /// </summary>
        public string DevicePropertyName { get; set; } = string.Empty;

        /// <summary>
        /// チェックボックスの値を取得または設定します。
        /// </summary>
        public bool Checked { get; set; } = false;
    }

}
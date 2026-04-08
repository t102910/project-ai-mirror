namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// キャッシュ対象の種別を表します。
    /// </summary>
    public enum QjCacheTypeEnum:byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = 0,

        /// <summary>
        /// ビューモデルのプロパティです。
        /// </summary>
        ModelProperty = 1,

        /// <summary>
        /// インプットモデルです。
        /// </summary>
        InputModel = 2,

        /// <summary>
        /// 仮アップロードされたファイルです。
        /// </summary>
        PostedFile = 3,

        /// <summary>
        /// サムネイル画像です。
        /// </summary>
        Thumbnail = 4
    }
}
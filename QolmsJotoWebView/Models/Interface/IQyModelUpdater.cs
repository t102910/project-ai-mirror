namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// インプットモデルの内容を現在のインスタンスに反映するための機能を実装します。
    /// </summary>
    /// <typeparam name="TModel">インプットモデルの型。</typeparam>
    public interface IQjModelUpdater<TModel> where TModel : class
    {
        /// <summary>
        /// インプットモデルの内容を現在のインスタンスに反映します。
        /// </summary>
        /// <param name="inputModel">インプットモデル。</param>
        void UpdateByInput(TModel inputModel);
    }
}

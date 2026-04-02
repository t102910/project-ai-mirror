using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「コルムス ヤプリ サイト」で使用する、
    /// 画面パーシャル ビュー モデルの基本クラスを表します。
    /// </summary>
    /// <typeparam name="TModel">親画面ビュー モデルの型。</typeparam>
    [Serializable]
    public abstract class QjPartialViewModelBase<TModel> where TModel : QjPageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 親画面ビュー モデルを取得または設定します。
        /// </summary>
        public TModel PageViewModel { get; set; } = null;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="QjPartialViewModelBase{TModel}" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected QjPartialViewModelBase()
        {
        }

        /// <summary>
        /// 親画面ビュー モデルを指定して、
        /// <see cref="QjPartialViewModelBase{TModel}" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="model">親画面ビュー モデル。</param>
        protected QjPartialViewModelBase(TModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model), "親画面ビュー モデルが Null 参照です。");

            PageViewModel = model;
        }

        #endregion
    }
}

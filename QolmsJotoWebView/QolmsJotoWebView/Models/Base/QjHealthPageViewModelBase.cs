using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「コルムス ヤプリ サイト」で使用する、
    /// Health 系画面ビューモデルの基本クラスを表します。
    /// </summary>
    [Serializable]
    public abstract class QjHealthPageViewModelBase : QjPageViewModelBase
    {
        #region Constructor

        /// <summary>
        /// <see cref="QjHealthPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public QjHealthPageViewModelBase()
            : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="QjHealthPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        /// <param name="pageNo">画面番号の種別。</param>
        protected QjHealthPageViewModelBase(QolmsJotoModel mainModel, QjPageNoTypeEnum pageNo)
            : base(mainModel, pageNo)
        {
        }

        #endregion
    }
}

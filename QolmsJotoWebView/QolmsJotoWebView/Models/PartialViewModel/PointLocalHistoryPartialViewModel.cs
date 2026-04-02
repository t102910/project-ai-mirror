using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「地域ポイント」画面の履歴パーシャルビューモデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    public sealed class PointLocalHistoryPartialViewModel : QjPartialViewModelBase<PointLocalHistoryInputModel>
    {
        #region Constructor

        /// <summary>
        /// 親ビューモデルを指定して、
        /// <see cref="PointLocalHistoryPartialViewModel" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="model">親ビューモデル。</param>
        public PointLocalHistoryPartialViewModel(PointLocalHistoryInputModel model)
            : base(model)
        {
        }

        #endregion
    }
}

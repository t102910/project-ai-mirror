using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「健診結果」画面の検査結果表パーシャルビューモデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    public sealed class NoteExaminationResultPartialViewModel : QjPartialViewModelBase<NoteExaminationViewModel>
    {
        #region Constructor

        /// <summary>
        /// 親ビューモデルを指定して、
        /// <see cref="NoteExaminationResultPartialViewModel" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="model">親ビューモデル。</param>
        public NoteExaminationResultPartialViewModel(NoteExaminationViewModel model)
            : base(model)
        {
        }

        #endregion
    }
}

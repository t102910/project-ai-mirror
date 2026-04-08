using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「ガルフスポーツ動画」画面の動画アイテムを表します。
    /// </summary>
    [Serializable]
    public sealed class GulfSportsMovieItem
    {
        /// <summary>YouTube 動画 ID を取得または設定します。</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>運動の種別を取得または設定します。</summary>
        public byte ExerciseType { get; set; } = byte.MinValue;

        /// <summary>カロリー（kcal）を取得または設定します。</summary>
        public int Calorie { get; set; } = 0;

        /// <summary>動画の説明を取得または設定します。</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>動画の時間を取得または設定します。</summary>
        public string Time { get; set; } = string.Empty;
    }

    /// <summary>
    /// 「ガルフスポーツ動画」画面ビューモデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class NoteGulfSportsMovieViewModel : QjNotePageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 動画の種別を取得または設定します。
        /// </summary>
        public byte MovieType { get; set; } = byte.MinValue;

        /// <summary>
        /// 動画アイテムのリストを取得または設定します。
        /// </summary>
        public List<GulfSportsMovieItem> MovieItemN { get; set; } = new List<GulfSportsMovieItem>();

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="NoteGulfSportsMovieViewModel"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public NoteGulfSportsMovieViewModel() : base() { }

        /// <summary>
        /// メインモデルを指定して、
        /// <see cref="NoteGulfSportsMovieViewModel"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="movieType">動画の種別。</param>
        public NoteGulfSportsMovieViewModel(QolmsJotoModel mainModel, byte movieType) : base(mainModel, QjPageNoTypeEnum.None)
        {
            this.MovieType = movieType;
        }

        #endregion
    }
}
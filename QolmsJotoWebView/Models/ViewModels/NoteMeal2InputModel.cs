using System;

namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    /// <summary>
    /// 食事情報の入力モデルを表します。
    /// </summary>
    [Serializable]
    public sealed class NoteMeal2InputModel : QjNotePageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 記録日を取得または設定します。
        /// </summary>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 食事種別を取得または設定します。
        /// </summary>
        public byte MealType { get; set; } = byte.MinValue;

        /// <summary>
        /// 品目名を取得または設定します。
        /// </summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// カロリーを取得または設定します。
        /// </summary>
        public short Calorie { get; set; } = short.MinValue;

        /// <summary>
        /// 食事解析種別を取得または設定します。
        /// カロミルから取得の場合は AnalysisType = 5
        /// </summary>
        public byte AnalysisType { get; set; } = byte.MinValue;

        /// <summary>
        /// 食事解析結果情報を取得または設定します。
        /// カロミルから取得の場合は 取得したJson文字列をそのまま入れます。
        /// </summary>
        public string AnalysisSet { get; set; } = string.Empty;

        /// <summary>
        /// 食事解析結果として採用した情報を取得または設定します。
        /// カロミルから取得の場合は 取得したJson文字列をそのまま入れます。
        /// </summary>
        public string ChooseSet { get; set; } = string.Empty;

        /// <summary>
        /// 食事割合を取得または設定します。
        /// </summary>
        public decimal Rate { get; set; } = decimal.MinValue;

        /// <summary>
        /// 削除フラグを取得または設定します。
        /// </summary>
        public bool DeleteFlag { get; set; } = false;

        /// <summary>
        /// カロミルの履歴番号を取得または設定します。
        /// </summary>
        public int HistoryId { get; set; } = int.MinValue;

        /// <summary>
        /// 画像取得フラグを取得または設定します。
        /// </summary>
        public bool HasImage { get; set; } = false;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="NoteMeal2InputModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public NoteMeal2InputModel()
        {
        }

        #endregion

        #region Public Method

        /// <summary>
        /// インプットモデルの内容を現在のインスタンスに反映します。
        /// </summary>
        /// <param name="inputModel">インプットモデル。</param>
        public void UpdateByInput(NoteMeal2InputModel inputModel)
        {
            if (inputModel != null)
            {
                this.AnalysisSet = inputModel.AnalysisSet;
                this.AnalysisType = inputModel.AnalysisType;
                this.Calorie = inputModel.Calorie;
                this.ChooseSet = inputModel.ChooseSet;
                this.DeleteFlag = inputModel.DeleteFlag;
                this.HistoryId = inputModel.HistoryId;
                this.ItemName = inputModel.ItemName;
                this.MealType = inputModel.MealType;
                this.Rate = inputModel.Rate;
                this.RecordDate = inputModel.RecordDate;
                this.HasImage = inputModel.HasImage;
            }
        }

        #endregion
    }

}
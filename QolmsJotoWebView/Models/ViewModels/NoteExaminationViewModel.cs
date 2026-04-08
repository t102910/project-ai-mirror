using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{

    /// <summary>
    /// 「健診結果」画面ビューモデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class NoteExaminationViewModel : QjNotePageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 表示開始日を取得または設定します。
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 表示終了日を取得または設定します。
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 検査結果表のリストを取得または設定します。
        /// </summary>
        public List<ExaminationMatrix> MatrixN { get; set; } = new List<ExaminationMatrix>();

        /// <summary>
        /// 特定の健診分類のみへ絞り込むための健診分類IDを取得または設定します。
        /// </summary>
        public byte NarrowInCategory { get; set; } = byte.MinValue;

        /// <summary>
        /// 基準範囲外の結果のみへ絞り込むかを取得または設定します。
        /// </summary>
        public bool NarrowInAbnormal { get; set; } = false;

        /// <summary>
        /// 検査グループ情報のリストを取得または設定します。
        /// </summary>
        public List<ExaminationGroupItem> ExaminationGroupN { get; set; } = new List<ExaminationGroupItem>();

        /// <summary>
        /// 日付および日付内連番ごとの検査結果情報のリストを取得または設定します。
        /// </summary>
        public List<ExaminationSetItem> ExaminationSetN { get; set; } = new List<ExaminationSetItem>();

        /// <summary>
        /// 健康年齢計算用パラメータのJson形式の文字列を取得または設定します。
        /// </summary>
        public string HealthAgeCalcJson { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="NoteExaminationViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public NoteExaminationViewModel()
            : base()
        {
        }

        /// <summary>
        /// メインモデルを指定して、
        /// <see cref="NoteExaminationViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        public NoteExaminationViewModel(QolmsJotoModel mainModel)
            : base(mainModel, QjPageNoTypeEnum.NoteExamination)
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="NoteExaminationViewModel" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">ログイン済みモデル。</param>
        /// <param name="startDate">表示開始日。</param>
        /// <param name="endDate">表示終了日。</param>
        /// <param name="matrixN">検査結果表のリスト。</param>
        /// <param name="narrowInAbnormal">基準範囲外の結果のみへ絞り込むかのフラグ。</param>
        /// <param name="examinationN">検査結果のリスト。</param>
        /// <param name="groupN">検査種別のリスト。</param>
        /// <param name="healthAgeCalcJson">健康年齢計算用パラメータのJson形式の文字列。</param>
        public NoteExaminationViewModel(
            QolmsJotoModel mainModel,
            DateTime startDate,
            DateTime endDate,
            List<ExaminationMatrix> matrixN,
            bool narrowInAbnormal,
            List<ExaminationSetItem> examinationN,
            List<ExaminationGroupItem> groupN,
            string healthAgeCalcJson
        )
            : base(mainModel, QjPageNoTypeEnum.NoteExamination)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.ExaminationSetN = examinationN;
            this.ExaminationGroupN = groupN;

            this.HealthAgeCalcJson = healthAgeCalcJson;

            // 要検討
            // categoryTreeN As IEnumerable(Of CheckupTreeNode),
            // selectedN As IEnumerable(Of CheckupReferenceJsonParameter),
            // narrowInCategory As Byte,

            this.InitializeBy(
                matrixN,
                this.NarrowInCategory,
                narrowInAbnormal
            );
            // categoryTreeN,
            // selectedN,
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 値を指定して、
        /// <see cref="NoteExaminationViewModel" />クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="matrixN">検査結果表のリスト。</param>
        /// <param name="narrowInCategory">特定の健診分類のみへ絞り込むための健診分類ID。</param>
        /// <param name="narrowInAbnormal">基準範囲外の結果のみへ絞り込むかのフラグ。</param>
        private void InitializeBy(
            List<ExaminationMatrix> matrixN,
            byte narrowInCategory,
            bool narrowInAbnormal
        )
        {
            // categoryTreeN As IEnumerable(Of CheckupTreeNode),
            // selectedN As IEnumerable(Of CheckupReferenceJsonParameter),

            // If categoryTreeN IsNot Nothing AndAlso categoryTreeN.Any() Then Me.CategoryTreeN = categoryTreeN.ToList()

            // If selectedN IsNot Nothing AndAlso selectedN.Any() Then
            //     Me.SelectedN = selectedN.ToList()

            var newMatrixN = new List<ExaminationMatrix>();

            if (matrixN != null && matrixN.Any())
            {
                foreach (ExaminationMatrix mat0 in matrixN)
                {
                    ExaminationMatrix mat = mat0;

                    if (mat.ColCount > 0 && mat.RowCount > 0)
                    {
                        // 特定の健診分類IDのみへ絞り込む
                        // if (this.NarrowInCategory != byte.MinValue) mat = mat.NarrowInCategory(this.NarrowInCategory);

                        // 基準範囲外の結果のみへ絞り込む
                        if (this.NarrowInAbnormal) mat = mat.NarrowInAbnormal();
                    }
                    else
                    {
                        this.NarrowInAbnormal = false;
                        this.NarrowInCategory = byte.MinValue;
                    }

                    newMatrixN.Add(mat);
                }
            }

            this.MatrixN = newMatrixN;
            this.NarrowInCategory = narrowInCategory;
            this.NarrowInAbnormal = narrowInAbnormal;
        }

        #endregion

        #region Public Method

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 結果項目を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class ExaminationItem
    {
        #region Variable

        /// <summary>
        /// 一意のキーを保持します。
        /// </summary>
        private readonly Guid _keyGuid = Guid.NewGuid();

        #endregion

        #region Public Property

        /// <summary>
        /// 一意のキーを取得します。
        /// </summary>
        public Guid KeyGuid
        {
            get { return _keyGuid; }
        }

        /// <summary>
        /// 検査結果項目の種別を取得または設定します。
        /// </summary>
        public QjExaminationItemTypeEnum ItemType { get; set; } = QjExaminationItemTypeEnum.None;

        /// <summary>
        /// 検査項目コード（JLAC10）を取得または設定します。
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 検査項目コード（ローカルコード）を取得または設定します。
        /// </summary>
        public string LocalCode { get; set; } = string.Empty;

        /// <summary>
        /// 検査項目コード表示名を取得または設定します。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 検査結果データ型を取得または設定します。
        /// </summary>
        public QjExaminationItemValueTypeEnum ValueType { get; set; } = QjExaminationItemValueTypeEnum.None;

        /// <summary>
        /// 結果を取得または設定します。
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 単位コードを取得または設定します。
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// 結果解釈コード（H | L | N）を取得または設定します。
        /// </summary>
        public string Interpretation { get; set; } = string.Empty;

        /// <summary>
        /// 基準値下限閾値を取得または設定します。
        /// </summary>
        public string Low { get; set; } = string.Empty;

        /// <summary>
        /// 基準値上限閾値を取得または設定します。
        /// </summary>
        public string High { get; set; } = string.Empty;

        /// <summary>
        /// 基準値を表す文字列パターンを取得または設定します。
        /// </summary>
        public string ReferenceDisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 検査結果情報のリストを取得または設定します。
        /// </summary>
        public List<ExaminationItem> ChildN { get; set; } = new List<ExaminationItem>();

        /// <summary>
        /// 検査結果に対するコメントを取得または設定します。
        /// </summary>
        [Obsolete("検討中")]
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// 基準とする単位系と異なるかを取得または設定します。
        /// </summary>
        public bool IsDifferentUnit { get; set; } = false;

        /// <summary>
        /// 結果を保持しているかを取得します。
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(this.Value); }
        }

        /// <summary>
        /// 結果データ型が物理量かを取得します。
        /// </summary>
        public bool IsPhysicalQuantity
        {
            get { return this.ValueType == QjExaminationItemValueTypeEnum.PQ; }
        }

        /// <summary>
        /// 結果が下限閾値を下回っているかを取得します。
        /// この値は結果データ型が物理量の場合のみ有効です。
        /// </summary>
        public bool IsLower
        {
            get { return this.IsPhysicalQuantity && string.Compare(this.Interpretation, "L", true) == 0; }
        }

        /// <summary>
        /// 結果が上限閾値を上回っているかを取得します。
        /// この値は結果データ型が物理量の場合のみ有効です。
        /// </summary>
        public bool IsHigher
        {
            get { return this.IsPhysicalQuantity && string.Compare(this.Interpretation, "H", true) == 0; }
        }

        /// <summary>
        /// 結果が基準値範囲外かを取得します。
        /// この値は結果データ型が物理量の場合のみ有効です。
        /// </summary>
        public bool IsAbnormalValue
        {
            get { return this.IsLower || this.IsHigher; }
        }

        [Obsolete("検討中")]
        public string GroupNo { get; set; } = string.Empty;

        [Obsolete("検討中")]
        public string DispOrder { get; set; } = string.Empty;

        #endregion

        #region Public Method

        [Obsolete("検討中")]
        public Tuple<string, string> GetKey()
        {
            return new Tuple<string, string>(this.Code, this.LocalCode);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="ExaminationItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ExaminationItem()
        {
        }

        /// <summary>
        /// 検査結果項目を指定して、
        /// <see cref="CheckupElement" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="entry">検査結果項目。</param>
        public ExaminationItem(QhApiExaminationItem entry)
        {
            InitializeBy(entry);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 検査結果項目を指定して、
        /// <see cref="CheckupElement" />クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="entry">検査結果項目。</param>
        private void InitializeBy(QhApiExaminationItem entry)
        {
            if (entry != null && !string.IsNullOrWhiteSpace(entry.Value))
            {
                var enumItemType = QjExaminationItemTypeEnum.None;
                var enumValueType = QjExaminationItemValueTypeEnum.None;

                Enum.TryParse(entry.ItemType, out enumItemType);
                Enum.TryParse(entry.ValueType, out enumValueType);

                if ((enumItemType == QjExaminationItemTypeEnum.Value || enumItemType == QjExaminationItemTypeEnum.TotalJudgment)
                    && enumValueType != QjExaminationItemValueTypeEnum.None)
                {
                    this.ItemType = enumItemType;
                    this.Code = entry.Code;
                    this.Name = entry.Name;
                    this.ValueType = enumValueType;
                    this.Value = entry.Value;

                    if (this.ValueType == QjExaminationItemValueTypeEnum.PQ)
                    {
                        this.Unit = entry.Unit;
                        this.Interpretation = entry.Interpretation;
                        this.Low = entry.Low;
                        this.High = entry.High;
                        this.ReferenceDisplayName = entry.ReferenceDisplayName;
                    }

                    // 検査項目を入れ子構造で持つかどうかは要検討
                    // ...
                }
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// インスタンスのディープコピーを作成します。
        /// </summary>
        /// <returns>
        /// このインスタンスをディープコピーしたオブジェクト。
        /// </returns>
        public ExaminationItem Copy()
        {
            ExaminationItem result = null;

            using (var stream = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);

                stream.Position = 0;

                result = (ExaminationItem)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            }

            return result;
        }

        #endregion
    }
}

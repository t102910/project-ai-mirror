using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 健診結果表を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class ExaminationMatrix
    {
        #region Variable

        /// <summary>
        /// 列項目のリストを保持します。
        /// </summary>
        private readonly List<ExaminationAxis> _xAxis = new List<ExaminationAxis>();

        /// <summary>
        /// 行項目のリストを保持します。
        /// </summary>
        private readonly List<ExaminationAxis> _yAxis = new List<ExaminationAxis>();

        /// <summary>
        /// 結果項目の行列を保持します。
        /// </summary>
        private readonly List<List<ExaminationItem>> _Items = new List<List<ExaminationItem>>();

        #endregion

        #region Public Property

        /// <summary>
        /// 列項目の読み取り専用コレクションを取得します。
        /// </summary>
        public ReadOnlyCollection<ExaminationAxis> XAxis
        {
            get { return _xAxis.AsReadOnly(); }
        }

        /// <summary>
        /// 行項目の読み取り専用コレクションを取得します。
        /// </summary>
        public ReadOnlyCollection<ExaminationAxis> YAxis
        {
            get { return _yAxis.AsReadOnly(); }
        }

        /// <summary>
        /// 結果項目の行列の読み取り専用コレクションを取得します。
        /// </summary>
        public ReadOnlyCollection<ReadOnlyCollection<ExaminationItem>> Items
        {
            get { return _Items.ConvertAll(i => i.AsReadOnly()).AsReadOnly(); }
        }

        /// <summary>
        /// 列項目数を取得します。
        /// </summary>
        public int XAxisCount
        {
            get { return _xAxis.Count; }
        }

        /// <summary>
        /// 行項目数を取得します。
        /// </summary>
        public int YAxisCount
        {
            get { return _yAxis.Count; }
        }

        /// <summary>
        /// 列項目数を取得します。
        /// </summary>
        public int ColCount
        {
            get { return XAxisCount; }
        }

        /// <summary>
        /// 行項目数を取得します。
        /// </summary>
        public int RowCount
        {
            get { return YAxisCount; }
        }

        /// <summary>
        /// 基準値範囲外の結果を保持しているかを取得します。
        /// </summary>
        public bool HasAbnormalValue
        {
            get
            {
                return _xAxis.FindIndex(i => i.HasAbnormalValue) >= 0
                    || _yAxis.FindIndex(i => i.HasAbnormalValue) >= 0;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="ExaminationMatrix" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ExaminationMatrix()
        {
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 指定した列インデックスが有効かチェックします。
        /// </summary>
        /// <param name="index">列インデックス。</param>
        /// <returns>
        /// 有効ならTrue、
        /// 無効ならFalse。
        /// </returns>
        private bool CheckColIndex(int index)
        {
            return index >= 0 && index < ColCount;
        }

        /// <summary>
        /// 指定した行インデックスが有効かチェックします。
        /// </summary>
        /// <param name="index">行インデックス。</param>
        /// <returns>
        /// 有効ならTrue、
        /// 無効ならFalse。
        /// </returns>
        private bool CheckRowIndex(int index)
        {
            return index >= 0 && index < RowCount;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 検索条件を指定して、
        /// 列インデックスを検索します。
        /// </summary>
        /// <param name="match">検索条件。</param>
        /// <returns>
        /// 列が見つかれば0以上の列インデックス、
        /// 列が見つからなければInteger.MinValue。
        /// </returns>
        public int FindCol(Predicate<ExaminationAxis> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match), "検索条件がNull参照です。");

            int result = _xAxis.FindIndex(match);

            return (result < 0) ? int.MinValue : result;
        }

        /// <summary>
        /// 列キーを指定して、
        /// 列インデックスを検索します。
        /// </summary>
        /// <param name="key">列キー。</param>
        /// <returns>
        /// 列が見つかれば0以上の列インデックス、
        /// 列が見つからなければInteger.MinValue。
        /// </returns>
        public int FindCol(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), "列キーがNull参照もしくは空白です。");

            int result = FindCol(i => string.Compare(i.Key, key, StringComparison.Ordinal) == 0);

            return (result < 0) ? int.MinValue : result;
        }

        /// <summary>
        /// 検索条件を指定して、
        /// 行インデックスを検索します。
        /// </summary>
        /// <param name="match">検索条件。</param>
        /// <returns>
        /// 行が見つかれば0以上の行インデックス、
        /// 行が見つからなければInteger.MinValue。
        /// </returns>
        public int FindRow(Predicate<ExaminationAxis> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match), "検索条件がNull参照です。");

            int result = _yAxis.FindIndex(match);

            return (result < 0) ? int.MinValue : result;
        }

        /// <summary>
        /// 行キーを指定して、
        /// 行インデックスを検索します。
        /// </summary>
        /// <param name="key">行キー。</param>
        /// <returns>
        /// 行が見つかれば0以上の行インデックス、
        /// 行が見つからなければInteger.MinValue。
        /// </returns>
        public int FindRow(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), "行キーがNull参照もしくは空白です。");
            if (ColCount == 0) throw new ArgumentOutOfRangeException(nameof(ColCount), "列が存在しません。");

            int result = FindRow(i => string.Compare(i.Key, key, StringComparison.Ordinal) == 0);

            return (result < 0) ? int.MinValue : result;
        }

        /// <summary>
        /// 列項目を指定して、
        /// 列を追加します。
        /// </summary>
        /// <param name="axis">列項目。</param>
        /// <param name="index">
        /// 列項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
        /// 未指定の場合は末尾に追加。
        /// </param>
        /// <returns>
        /// 挿入された位置を表す列インデックス。
        /// <paramref name="key" />に該当する列がすでに存在する場合は挿入は行わず、
        /// 該当する列インデックスを返却。
        /// </returns>
        public int AddCol(ExaminationAxis axis, int index = int.MaxValue)
        {
            if (axis == null) throw new ArgumentNullException(nameof(axis), "列項目がNull参照です。");
            if (string.IsNullOrWhiteSpace(axis.Key)) throw new ArgumentNullException(nameof(axis.Key), "列キーがNull参照もしくは空白です。");
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "挿入位置が不正です。");

            int result = FindCol(axis.Key);

            if (result == int.MinValue)
            {
                int pos = Math.Min(ColCount, index);

                // TODO: 実装中
                _xAxis.Insert(
                    pos,
                    new ExaminationAxis(
                        axis.Key,
                        axis.Header1,
                        axis.Header2,
                        axis.HeaderUnit,
                        axis.HeaderStandardValue,
                        decimal.MinValue,
                        axis.Comment,
                        axis.AssociatedFileN,
                        axis.ExaminationJudgementN
                    )
                );
                _Items.Insert(pos, new List<ExaminationItem>());

                if (RowCount > 0)
                {
                    for (int y = 0; y <= RowCount - 1; y++)
                    {
                        _Items[pos].Add(new ExaminationItem());
                    }
                }

                result = pos;
            }

            return result;
        }

        /// <summary>
        /// 値を指定して、
        /// 列を追加します。
        /// </summary>
        /// <param name="key">列キー。</param>
        /// <param name="header1">検査受診施設名。</param>
        /// <param name="header2">検査受診日（yyyy/M/d）。</param>
        /// <param name="associatedFileN">検査手帳付随 ファイル 情報の リスト。</param>
        /// <param name="index">
        /// 列項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
        /// 未指定の場合は末尾に追加。
        /// </param>
        /// <returns>
        /// 挿入された位置を表す列インデックス。
        /// <paramref name="key" />に該当する列がすでに存在する場合は挿入は行わず、
        /// 該当する列インデックスを返却。
        /// </returns>
        [Obsolete("実装中")]
        public int AddCol(
            string key,
            string header1,
            string header2,
            List<AssociatedFileItem> associatedFileN,
            Dictionary<string, ExaminationJudgementItem> JudgementN,
            decimal healthAge,
            string comment,
            int index = int.MaxValue
        )
        {
            return AddCol(new ExaminationAxis(key, header1, header2, string.Empty, string.Empty, healthAge, comment, associatedFileN, JudgementN), index);
        }

        /// <summary>
        /// 行項目を指定して、
        /// 行を追加します。
        /// </summary>
        /// <param name="row">検査項目アイテム。</param>
        /// <param name="index">
        /// 行項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
        /// 未指定の場合は末尾に追加。
        /// </param>
        /// <returns>
        /// 挿入された位置を表す行インデックス。
        /// <paramref name="key" />に該当する行がすでに存在する場合は挿入は行わず、
        /// 該当する行インデックスを返却。
        /// </returns>
        public int AddRow(ExaminationItem row, int index = int.MaxValue)
        {
            var axis = new ExaminationAxis(row.Code, row.Name, string.Empty, row.Unit, row.ReferenceDisplayName, decimal.MinValue, row.Comment);

            if (axis == null) throw new ArgumentNullException(nameof(axis), "行項目がNull参照です。");
            if (string.IsNullOrWhiteSpace(axis.Key)) throw new ArgumentNullException(nameof(axis.Key), "行キーがNull参照もしくは空白です。");
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "挿入位置が不正です。");
            if (ColCount == 0) throw new ArgumentOutOfRangeException(nameof(ColCount), "列が存在しません。");

            int result = FindRow(axis.Key);

            if (result == int.MinValue)
            {
                int pos = Math.Min(RowCount, index);

                _yAxis.Insert(
                    pos,
                    new ExaminationAxis(
                        axis.Key,
                        axis.Header1,
                        axis.Header2,
                        axis.HeaderUnit,
                        axis.HeaderStandardValue,
                        decimal.MinValue,
                        axis.Comment
                    )
                );

                _Items.ForEach(i => i.Insert(pos, new ExaminationItem()));

                result = pos;
            }

            return result;
        }

        /// <summary>
        /// 行項目を指定して、
        /// 行を追加します。
        /// ＊検査種別（グループ）表示用行＊
        /// </summary>
        /// <param name="grp">検査グループアイテム。</param>
        /// <param name="index">
        /// 行項目を挿入する位置の0から始まるインデックス（オプショナル、デフォルト=Integer.MaxValue）。
        /// 未指定の場合は末尾に追加。
        /// </param>
        /// <returns>
        /// 挿入された位置を表す行インデックス。
        /// <paramref name="key" />に該当する行がすでに存在する場合は挿入は行わず、
        /// 該当する行インデックスを返却。
        /// </returns>
        public int AddGroupRow(ExaminationGroupItem grp, int index = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(grp.GroupNo.ToString())) throw new ArgumentNullException("groupNo", "グループ番号がNull参照もしくは空白です。");
            // If index < 0 Then Throw New ArgumentOutOfRangeException("index", "挿入位置が不正です。")
            if (ColCount == 0) throw new ArgumentOutOfRangeException(nameof(ColCount), "列が存在しません。");

            int result = int.MinValue;

            if (result == int.MinValue)
            {
                int pos = Math.Min(RowCount, index);

                _yAxis.Insert(
                    pos,
                    new ExaminationAxis(
                        grp.GroupNo.ToString(),
                        grp.Name,
                        "groupRow",
                        string.Empty,
                        string.Empty,
                        decimal.MinValue,
                        grp.Comment
                    )
                ); // 二つ判定があれば二つ出す

                _Items.ForEach(i => i.Insert(pos, new ExaminationItem { Value = "gr" }));

                result = pos;
            }

            return result;
        }

        /// <summary>
        /// 列インデックスを指定して、
        /// 列を削除します。
        /// </summary>
        /// <param name="index">列インデックス。</param>
        public void RemoveCol(int index)
        {
            if (!CheckColIndex(index)) throw new ArgumentOutOfRangeException(nameof(index), "列インデックスが不正です。");

            _Items.RemoveAt(index);
            _xAxis.RemoveAt(index);
        }

        /// <summary>
        /// 列キーを指定して、
        /// 列を削除します。
        /// </summary>
        /// <param name="key">列キー。</param>
        public void RemoveCol(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), "列キーがNull参照もしくは空白です。");

            int index = FindCol(key);

            if (index >= 0) RemoveCol(index);
        }

        /// <summary>
        /// 行インデックスを指定して、
        /// 行を削除します。
        /// </summary>
        /// <param name="index">行インデックス。</param>
        public void RemoveRow(int index)
        {
            if (!CheckRowIndex(index)) throw new ArgumentOutOfRangeException(nameof(index), "行インデックスが不正です。");

            _Items.ForEach(i => i.RemoveAt(index));
            _yAxis.RemoveAt(index);
        }

        /// <summary>
        /// 行キーを指定して、
        /// 行を削除します。
        /// </summary>
        /// <param name="key">行キー。</param>
        public void RemoveRow(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), "行キーがNull参照もしくは空白です。");

            int index = FindRow(key);

            if (index >= 0) RemoveRow(index);
        }

        /// <summary>
        /// 列インデックスで指定された列を、
        /// 新しい位置へ移動します。
        /// </summary>
        /// <param name="sourceIndex">移動する列を表すインデックス。</param>
        /// <param name="destIndex">新しい位置を表すインデックス。</param>
        public void MoveCol(int sourceIndex, int destIndex)
        {
            if (!CheckColIndex(sourceIndex)) throw new ArgumentOutOfRangeException(nameof(sourceIndex), "移動元列インデックスが不正です。");

            if (ColCount > 1)
            {
                int pos = int.MinValue;
                List<ExaminationItem> Items = _Items[sourceIndex];
                ExaminationAxis axis = _xAxis[sourceIndex];

                if (sourceIndex > destIndex)
                {
                    pos = Math.Max(0, destIndex);
                }
                else if (sourceIndex < destIndex)
                {
                    pos = Math.Min(destIndex, ColCount - 1);
                }

                if (pos >= 0)
                {
                    _Items.Remove(Items);
                    _Items.Insert(pos, Items);
                    _xAxis.Remove(axis);
                    _xAxis.Insert(pos, axis);
                }
            }
        }

        /// <summary>
        /// 列キーで指定された列を、
        /// 新しい位置へ移動します。
        /// </summary>
        /// <param name="sourceKey">移動する列を表すキー。</param>
        /// <param name="destIndex">新しい位置を表すインデックス。</param>
        public void MoveCol(string sourceKey, int destIndex)
        {
            if (string.IsNullOrWhiteSpace(sourceKey)) throw new ArgumentNullException(nameof(sourceKey), "移動元列キーがNull参照もしくは空白です。");

            MoveCol(FindCol(sourceKey), destIndex);
        }

        /// <summary>
        /// 行インデックスで指定された行を、
        /// 新しい位置へ移動します。
        /// </summary>
        /// <param name="sourceIndex">移動する行を表すインデックス。</param>
        /// <param name="destIndex">新しい位置を表すインデックス。</param>
        public void MoveRow(int sourceIndex, int destIndex)
        {
            if (!CheckRowIndex(sourceIndex)) throw new ArgumentOutOfRangeException(nameof(sourceIndex), "移動元行インデックスが不正です。");

            if (ColCount > 0 && RowCount > 1)
            {
                int pos = int.MinValue;
                ExaminationAxis axis = _yAxis[sourceIndex];

                if (sourceIndex > destIndex)
                {
                    pos = Math.Max(0, destIndex);
                }
                else if (sourceIndex < destIndex)
                {
                    pos = Math.Min(destIndex, RowCount - 1);
                }

                if (pos > 0)
                {
                    for (int x = 0; x <= ColCount - 1; x++)
                    {
                        ExaminationItem Item = _Items[x][sourceIndex];

                        _Items[x].Remove(Item);
                        _Items[x].Insert(pos, Item);
                    }

                    _yAxis.Remove(axis);
                    _yAxis.Insert(pos, axis);
                }
            }
        }

        /// <summary>
        /// 行キーで指定された行を、
        /// 新しい位置へ移動します。
        /// </summary>
        /// <param name="sourceKey">移動する行を表すキー。</param>
        /// <param name="destIndex">新しい位置を表すインデックス。</param>
        public void MoveRow(string sourceKey, int destIndex)
        {
            if (string.IsNullOrWhiteSpace(sourceKey)) throw new ArgumentNullException(nameof(sourceKey), "移動元行キーがNull参照もしくは空白です。");

            MoveCol(FindRow(sourceKey), destIndex);
        }

        /// <summary>
        /// 列キーを使用して列をソートします。
        /// </summary>
        public void SortColByKey()
        {
            if (ColCount > 1)
            {
                List<ExaminationAxis> sortedXAxis = XAxis.ToList();

                // 降順ソート
                sortedXAxis.Sort(
                    new Comparison<ExaminationAxis>(
                        (x, y) =>
                        {
                            string[] parts = x.Key.Split(new[] { "_" }, StringSplitOptions.None);
                            int day = 0;
                            int seq = 0;
                            int id = 0;
                            decimal keyX = 0;
                            decimal keyY = 0;

                            if (parts.Length == 3
                                && int.TryParse(parts[0], out day)
                                && int.TryParse(parts[1], out seq)
                                && int.TryParse(parts[2], out id))
                            {
                                keyX = decimal.Parse(string.Format("{0:d8}{1:d10}{2:d3}", day, int.MaxValue - seq, int.MaxValue - id));

                                parts = y.Key.Split(new[] { "_" }, StringSplitOptions.None);
                                day = 0;
                                seq = 0;
                                id = 0;

                                if (parts.Length == 3
                                    && int.TryParse(parts[0], out day)
                                    && int.TryParse(parts[1], out seq)
                                    && int.TryParse(parts[2], out id))
                                {
                                    keyY = decimal.Parse(string.Format("{0:d8}{1:d10}{2:d3}", day, int.MaxValue - seq, int.MaxValue - id));

                                    return keyX.CompareTo(keyY);
                                }

                                return 0;
                            }

                            return 0;
                        }
                    )
                );

                for (int x = 0; x <= sortedXAxis.Count - 1; x++)
                {
                    int sourceIndex = FindCol(sortedXAxis[x].Key);

                    if (sourceIndex >= 0) MoveCol(sourceIndex, int.MaxValue);
                }
            }
        }

        /// <summary>
        /// 行キーを使用して行をソートします。
        /// </summary>
        public void SortRowByKey()
        {
            if (ColCount > 0 && RowCount > 1)
            {
                List<ExaminationAxis> sortedYAxis = YAxis.ToList();

                // 昇順ソート
                sortedYAxis.Sort(new Comparison<ExaminationAxis>((x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal)));

                for (int y = 0; y <= sortedYAxis.Count - 1; y++)
                {
                    int sourceIndex = FindRow(sortedYAxis[y].Key);

                    if (sourceIndex >= 0) MoveRow(sourceIndex, int.MaxValue);
                }
            }
        }

        /// <summary>
        /// 表示順定義を使用して行をソートします。
        /// </summary>
        /// <param name="definition">表示順定義（検査項目コードをキー、表示順を値とするディクショナリ）。</param>
        [Obsolete("検証中、未使用")]
        public void SortRowByDisplayOrder(Dictionary<string, int> definition)
        {
            if (ColCount > 0 && RowCount > 1)
            {
                List<ExaminationAxis> sortedYAxis = YAxis.ToList();

                // 昇順ソート
                sortedYAxis.Sort(
                    new Comparison<ExaminationAxis>(
                        (x, y) =>
                        {
                            int xOrder = definition.ContainsKey(x.Key) ? definition[x.Key] : int.MaxValue;
                            int yOrder = definition.ContainsKey(y.Key) ? definition[y.Key] : int.MaxValue;

                            return xOrder.CompareTo(yOrder);
                        }
                    )
                );

                for (int y = 0; y <= sortedYAxis.Count - 1; y++)
                {
                    int sourceIndex = FindRow(sortedYAxis[y].Key);

                    if (sourceIndex >= 0) MoveRow(sourceIndex, int.MaxValue);
                }
            }
        }

        /// <summary>
        /// インデックスを指定して、
        /// 検査結果項目を取得します。
        /// </summary>
        /// <param name="colIndex">列インデックス。</param>
        /// <param name="rowIndex">行インデックス。</param>
        /// <returns>
        /// 成功なら該当する検査結果項目のコピーインスタンス、
        /// 失敗なら例外をスロー。
        /// </returns>
        public ExaminationItem GetItem(int colIndex, int rowIndex)
        {
            if (!CheckColIndex(colIndex)) throw new ArgumentOutOfRangeException(nameof(colIndex), "列インデックスが不正です。");
            if (!CheckRowIndex(rowIndex)) throw new ArgumentOutOfRangeException(nameof(rowIndex), "行インデックスが不正です。");

            return _Items[colIndex][rowIndex].Copy();
        }

        /// <summary>
        /// 列キーおよび行キーを指定して、
        /// 検査結果項目を取得します。
        /// </summary>
        /// <param name="colKey">列キー。</param>
        /// <param name="rowKey">行キー。</param>
        /// <returns>
        /// 成功なら該当する検査結果項目のコピーインスタンス、
        /// 失敗なら例外をスロー。
        /// </returns>
        public ExaminationItem GetItem(string colKey, string rowKey)
        {
            return GetItem(FindCol(colKey), FindRow(rowKey));
        }

        /// <summary>
        /// 一意のキーを指定して、
        /// 検査結果項目を取得します。
        /// </summary>
        /// <param name="keyGuid"></param>
        /// <returns>
        /// 成功なら該当する検査結果項目のコピーインスタンス、
        /// 失敗ならNothing。
        /// </returns>
        public ExaminationItem GetItem(Guid keyGuid)
        {
            ExaminationItem result = null;

            _Items.ForEach(
                x =>
                {
                    x.ForEach(
                        y =>
                        {
                            if (y.KeyGuid == keyGuid)
                            {
                                result = y.Copy();
                                return;
                            }
                        }
                    );

                    if (result != null) return;
                }
            );

            return result;
        }

        /// <summary>
        /// インデックスを指定して、
        /// 検査結果項目を設定します。
        /// </summary>
        /// <param name="Item">検査結果項目。</param>
        /// <param name="colIndex">列インデックス。</param>
        /// <param name="rowIndex">行インデックス。</param>
        public void SetItem(ExaminationItem Item, int colIndex, int rowIndex)
        {
            if (Item == null) throw new ArgumentNullException(nameof(Item), "検査結果項目がNull参照です。");
            if (!CheckColIndex(colIndex)) throw new ArgumentOutOfRangeException(nameof(colIndex), "列インデックス");
            //If Not Me.CheckRowIndex(rowIndex) Then Throw New ArgumentOutOfRangeException("rowIndex", "行インデックス")

            string code = string.Empty;

            if (rowIndex == int.MinValue)
            {
            }
            else
            {
                if (ExaminationAxis.IsRowKey(Item.Code, ref code)
                    && string.Compare(_yAxis[rowIndex].Key, code, StringComparison.Ordinal) == 0)
                {
                    _Items[colIndex][rowIndex] = Item.Copy();
                }
            }
        }

        /// <summary>
        /// 列キーおよび行キーを指定して、
        /// 検査結果項目を設定します。
        /// </summary>
        /// <param name="Item">検査結果項目。</param>
        /// <param name="colKey">列キー。</param>
        /// <param name="rowKey">行キー。</param>
        public void SetItem(ExaminationItem Item, string colKey, string rowKey)
        {
            SetItem(Item, FindCol(colKey), FindRow(rowKey));
        }

        /// <summary>
        /// 検査結果表を更新します。
        /// 検査結果表に対する必要な操作を終えた後に実行してください。
        /// </summary>
        public void UpdateMatrix()
        {
            // 空の列を削除
            for (int x = ColCount - 1; x >= 0; x--)
            {
                if (_Items[x].FindIndex(i => !i.IsEmpty) < 0) RemoveCol(x);
            }

            // 空の行を削除
            if (ColCount > 0)
            {
                bool hasGroupValue = false;

                for (int y = RowCount - 1; y >= 0; y--)
                {
                    bool isGroupRow = (YAxis[y].Header2 == "groupRow");

                    if (isGroupRow)
                    {
                        if (!hasGroupValue) RemoveRow(y);
                        hasGroupValue = false;
                    }
                    else
                    {
                        bool hasValue = false;

                        for (int x = 0; x <= ColCount - 1; x++)
                        {
                            if (!_Items[x][y].IsEmpty)
                            {
                                hasValue = true;
                                hasGroupValue = true;
                                break;
                            }
                        }

                        if (!hasValue) RemoveRow(y);
                    }
                }
            }

            // 列項目と行項目の更新
            if (ColCount > 0 && RowCount > 0)
            {
                for (int y = 0; y <= RowCount - 1; y++)
                {
                    string defName = string.Empty;
                    string defUnit = string.Empty;
                    string defLow = string.Empty;
                    string defHigh = string.Empty;
                    string defRange = string.Empty;

                    bool hasDifferentUnit = false;
                    //Dim dicHealthAgeValues As New Dictionary(Of String, String)()
                    //Dim linkageno As Integer = 47008

                    for (int x = 0; x <= ColCount - 1; x++)
                    {
                        var item = _Items[x][y];

                        if (!item.IsEmpty)
                        {
                            if (string.IsNullOrWhiteSpace(defName) && !string.IsNullOrWhiteSpace(item.Name)) defName = item.Name;

                            if (item.IsPhysicalQuantity)
                            {
                                if (item.IsAbnormalValue)
                                {
                                    _xAxis[x].HasAbnormalValue = true;
                                    _yAxis[y].HasAbnormalValue = true;
                                }

                                if (string.IsNullOrWhiteSpace(defUnit) && !string.IsNullOrWhiteSpace(item.Unit)) defUnit = item.Unit;
                                if (string.IsNullOrWhiteSpace(defLow) && !string.IsNullOrWhiteSpace(item.Low)) defLow = item.Low;
                                if (string.IsNullOrWhiteSpace(defHigh) && !string.IsNullOrWhiteSpace(item.High)) defHigh = item.High;
                                if (string.IsNullOrWhiteSpace(defRange) && !string.IsNullOrWhiteSpace(item.ReferenceDisplayName)) defRange = item.ReferenceDisplayName;

                                item.IsDifferentUnit =
                                    string.Compare(item.Unit, defUnit, StringComparison.OrdinalIgnoreCase) != 0
                                    || string.Compare(item.Low, defLow, StringComparison.OrdinalIgnoreCase) != 0
                                    || string.Compare(item.High, defHigh, StringComparison.OrdinalIgnoreCase) != 0
                                    || string.Compare(item.ReferenceDisplayName, defRange, StringComparison.OrdinalIgnoreCase) != 0;

                                if (item.IsDifferentUnit)
                                {
                                    hasDifferentUnit = true;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(defName)) _yAxis[y].Header1 = defName;
                    if (!string.IsNullOrWhiteSpace(defUnit)) _yAxis[y].HeaderUnit = defUnit;
                    if (!string.IsNullOrWhiteSpace(defRange)) _yAxis[y].HeaderStandardValue = defRange;

                    if (hasDifferentUnit) _yAxis[y].HasDifferentUnit = hasDifferentUnit;

                    if (!string.IsNullOrWhiteSpace(defHigh) && !string.IsNullOrWhiteSpace(defLow))
                    {
                        _yAxis[y].HeaderStandardValue = defRange;
                    }
                    else if (!string.IsNullOrWhiteSpace(defHigh))
                    {
                        _yAxis[y].HeaderStandardValue = defHigh;
                    }
                    else if (!string.IsNullOrWhiteSpace(defLow))
                    {
                        _yAxis[y].HeaderStandardValue = defLow;
                    }
                    else if (!string.IsNullOrWhiteSpace(defRange))
                    {
                        _yAxis[y].HeaderStandardValue = defRange;
                    }
                    else
                    {
                        _yAxis[y].HeaderStandardValue = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 現在のインスタンスをコピーして、
        /// 特定の検査分類のみで構成される検査結果表を作成します。
        /// ※ひろしまでは、健診種別（特定健診等）、医誠会では、病院名＋受診科（内科等）を
        /// １件選択出来た。コルムスではどのように表示するか要検討。
        /// </summary>
        /// <param name="categoryId">検査分類ID。</param>
        /// <returns>
        /// 特定の検査分類のみで構成された検査結果表。
        /// 検査結果項目が存在しない場合はNothingを返却。
        /// </returns>
        public ExaminationMatrix NarrowInCategory(int categoryId)
        {
            ExaminationMatrix result = Copy();

            // 指定された検査分類IDの列のみ残す
            for (int x = result.ColCount - 1; x >= 0; x--)
            {
                string[] parts = result._xAxis[x].Key.Split(new[] { "_" }, StringSplitOptions.None);
                int id = 0;

                if (parts.Length == 3 && int.TryParse(parts.Last(), out id) && id != categoryId) result.RemoveCol(x);
            }

            // 検査結果表を更新
            if (result.ColCount > 0)
            {
                result.UpdateMatrix();

                if (result.ColCount > 0 && result.RowCount > 0) return result; // 検査結果項目が存在する
            }

            // 検査結果項目が存在しない
            return new ExaminationMatrix();
        }

        /// <summary>
        /// 現在のインスタンスをコピーして、
        /// 基準範囲外の結果のみで構成される検査結果表を作成します。
        /// </summary>
        /// <returns>
        /// 基準範囲外の結果のみで構成された検査結果表。
        /// 検査結果項目が存在しない場合はNothingを返却。
        /// </returns>
        public ExaminationMatrix NarrowInAbnormal()
        {
            ExaminationMatrix result = Copy();

            // 基準値範囲外の値を保持している列のみ残す
            for (int x = result.ColCount - 1; x >= 0; x--)
            {
                if (!result._xAxis[x].HasAbnormalValue) result.RemoveCol(x);
            }

            // 基準値範囲外の値を保持している行のみ残す
            if (result.ColCount > 0)
            {
                bool hasAbnormal = false;

                for (int y = result.RowCount - 1; y >= 0; y--)
                {
                    if (result._yAxis[y].HasAbnormalValue)
                    {
                        hasAbnormal = true;

                        for (int x = 0; x <= result.ColCount - 1; x++)
                        {
                            // 基準値内の値をクリア
                            if (!result._Items[x][y].IsAbnormalValue)
                            {
                                result._Items[x][y].Value = string.Empty;
                                result._Items[x][y].IsDifferentUnit = false;
                            }
                        }
                    }
                    else
                    {
                        // 基準値範囲外の値を1つも保持していないグループは削除
                        bool isGroupRow = (result.YAxis[y].Header2 == "groupRow");
                        if ((!isGroupRow) || (isGroupRow && hasAbnormal == false))
                        {
                            result.RemoveRow(y);
                        }

                        if (isGroupRow)
                        {
                            hasAbnormal = false;
                        }
                    }
                }
            }

            // 検査結果表を更新
            if (result.ColCount > 0 && result.RowCount > 0)
            {
                result.UpdateMatrix();

                if (result.ColCount > 0 && result.RowCount > 0) return result; // 検査結果項目が存在する
            }

            // 検査結果項目が存在しない
            return new ExaminationMatrix();
        }

        /// <summary>
        /// インスタンスのディープコピーを作成します。
        /// </summary>
        /// <returns>
        /// このインスタンスをディープコピーしたオブジェクト。
        /// </returns>
        public ExaminationMatrix Copy()
        {
            ExaminationMatrix result = null;

            using (var stream = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);

                stream.Position = 0;

                result = (ExaminationMatrix)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            }

            return result;
        }

        #endregion

        #region Debug

        /// <summary>
        /// 画面確認用テストデータを生成します。
        /// </summary>
        public static ExaminationMatrix CreateDebugMatrix()
        {
            var m = new ExaminationMatrix();

            // ============
            // 列（受診回）
            // key は "YYYYMMDD_連番_カテゴリID"
            // ============
            var col1Key = "20240115_1_1";
            var col2Key = "20240620_1_1";
            var col3Key = "20241105_1_2";
            var col4Key = "20250112_1_2";
            var col5Key = "20250703_2_3";

            var pdfRefJsonDummy = "{\"Accountkey\":\"DUMMY\",\"LoginAt\":\"2024-01-15\",\"RecordDate\":\"2024-01-15\",\"FacilityKey\":\"00000000-0000-0000-0000-000000000000\",\"LinkageSystemNo\":\"1\",\"LinkageSystemId\":\"SYS\",\"DataKey\":\"00000000-0000-0000-0000-000000000000\"}";

            // 判定辞書（グループ行と総合所見の表示用）
            Dictionary<string, ExaminationJudgementItem> MakeJudgement(
                string totalJudg, string totalText,
                string blood, string urine, string cardio, string imaging, string consult)
            {
                return new Dictionary<string, ExaminationJudgementItem>
                {
                    ["総合所見"] = new ExaminationJudgementItem { Name = "総合所見", Judgment1 = totalJudg, Value = totalText, IsTotalJudgment = true },
                    ["血液検査"] = new ExaminationJudgementItem { Name = "血液検査", Judgment1 = blood, Value = "" },
                    ["尿検査"] = new ExaminationJudgementItem { Name = "尿検査", Judgment1 = urine, Value = "" },
                    ["循環器"] = new ExaminationJudgementItem { Name = "循環器", Judgment1 = cardio, Value = "" },
                    ["画像・肺"] = new ExaminationJudgementItem { Name = "画像・肺", Judgment1 = imaging, Value = "" },
                    ["診察"] = new ExaminationJudgementItem { Name = "診察", Judgment1 = consult, Value = "" },
                };
            }

            // 列1
            m.AddCol(new ExaminationAxis(
                col1Key,
                "市民病院",
                "2024/1/15",
                "",
                "",
                42m,
                "",
                new List<AssociatedFileItem>
                {
            new AssociatedFileItem
            {
                DataType = QjExaminationDataTypeEnum.OverallAssessmentPdf,
                LinkageSystemNo = 1,
                LinkageSystemId = "SYS-A",
                RecordDate = new DateTime(2024,1,15),
                FacilityKey = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                DataKey = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                FileStorageReferenceJson = pdfRefJsonDummy
            }
                },
                MakeJudgement(
                    totalJudg: "A",
                    totalText: "概ね良好です。生活習慣（睡眠・運動）を継続してください。",
                    blood: "A",
                    urine: "A",
                    cardio: "A",
                    imaging: "A",
                    consult: "A"
                )
            ));

            // 列2
            m.AddCol(new ExaminationAxis(
                col2Key,
                "市民病院",
                "2024/6/20",
                "",
                "",
                45m,
                "",
                new List<AssociatedFileItem>(),
                MakeJudgement(
                    totalJudg: "B",
                    totalText: "脂質がやや高め。食事（油脂・間食）調整と運動を推奨。",
                    blood: "B",
                    urine: "A",
                    cardio: "A",
                    imaging: "A",
                    consult: "B"
                )
            ));

            // 列3
            m.AddCol(new ExaminationAxis(
                col3Key,
                "中央クリニック",
                "2024/11/5",
                "",
                "",
                47m,
                "",
                new List<AssociatedFileItem>(),
                MakeJudgement(
                    totalJudg: "C",
                    totalText: "血糖が高め。再検査または精密検査を検討してください。",
                    blood: "C",
                    urine: "B",
                    cardio: "A",
                    imaging: "A",
                    consult: "C"
                )
            ));

            // 列4
            m.AddCol(new ExaminationAxis(
                col4Key,
                "中央クリニック",
                "2025/1/12",
                "",
                "",
                48m,
                "",
                new List<AssociatedFileItem>(),
                MakeJudgement(
                    totalJudg: "B",
                    totalText: "尿酸が高め。水分摂取と食事内容の見直しを推奨。",
                    blood: "B",
                    urine: "A",
                    cardio: "A",
                    imaging: "A",
                    consult: "B"
                )
            ));

            // 列5
            m.AddCol(new ExaminationAxis(
                col5Key,
                "ハートライフ病院",
                "2025/7/3",
                "",
                "",
                41m,
                "",
                new List<AssociatedFileItem>
                {
            new AssociatedFileItem
            {
                DataType = QjExaminationDataTypeEnum.DicomData,
                AdditionalKey = "DICOM_URL_ACCESS_KEY_DUMMY",
                RecordDate = new DateTime(2025,7,3),
                FacilityKey = Guid.Parse("4c2fc0ea-705a-42d2-b32a-85e9a4eeccde")
            }
                },
                MakeJudgement(
                    totalJudg: "A",
                    totalText: "前回指摘事項は改善傾向。今後も継続。",
                    blood: "A",
                    urine: "A",
                    cardio: "A",
                    imaging: "A",
                    consult: "A"
                )
            ));

            // ============
            // 行（グループ + 項目）
            // ============
            m.AddGroupRow(new ExaminationGroupItem { GroupNo = 1, Name = "血液検査", Comment = "" });

            // 血液検査（PQ）
            m.AddRow(new ExaminationItem { Code = "100001", Name = "白血球数", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "個/μL", ReferenceDisplayName = "4000-9000" });
            m.AddRow(new ExaminationItem { Code = "100002", Name = "赤血球数", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "万/μL", ReferenceDisplayName = "430-570" });
            m.AddRow(new ExaminationItem { Code = "100003", Name = "ヘモグロビン", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "g/dL", ReferenceDisplayName = "13.5-17.6" });
            m.AddRow(new ExaminationItem { Code = "100004", Name = "血小板数", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "万/μL", ReferenceDisplayName = "15.0-35.0" });
            m.AddRow(new ExaminationItem { Code = "100005", Name = "AST(GOT)", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "U/L", ReferenceDisplayName = "13-30" });
            m.AddRow(new ExaminationItem { Code = "100006", Name = "ALT(GPT)", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "U/L", ReferenceDisplayName = "10-42" });
            m.AddRow(new ExaminationItem { Code = "100007", Name = "γ-GTP", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "U/L", ReferenceDisplayName = "13-64" });
            m.AddRow(new ExaminationItem { Code = "100008", Name = "中性脂肪", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mg/dL", ReferenceDisplayName = "30-149" });
            m.AddRow(new ExaminationItem { Code = "100009", Name = "HDLコレステロール", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mg/dL", ReferenceDisplayName = "40-" });
            m.AddRow(new ExaminationItem { Code = "100010", Name = "LDLコレステロール", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mg/dL", ReferenceDisplayName = "-119" });
            m.AddRow(new ExaminationItem { Code = "100011", Name = "空腹時血糖", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mg/dL", ReferenceDisplayName = "70-109" });
            m.AddRow(new ExaminationItem { Code = "100012", Name = "HbA1c(NGSP)", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "%", ReferenceDisplayName = "4.6-6.2" });
            m.AddRow(new ExaminationItem { Code = "100013", Name = "尿酸", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mg/dL", ReferenceDisplayName = "3.7-7.0" });
            m.AddRow(new ExaminationItem { Code = "100014", Name = "クレアチニン", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mg/dL", ReferenceDisplayName = "0.65-1.07" });

            m.AddGroupRow(new ExaminationGroupItem { GroupNo = 2, Name = "尿検査", Comment = "" });

            // 尿検査（ST/CO混在）
            m.AddRow(new ExaminationItem { Code = "200001", Name = "尿蛋白", ValueType = QjExaminationItemValueTypeEnum.ST, Unit = "", ReferenceDisplayName = "" });
            m.AddRow(new ExaminationItem { Code = "200002", Name = "尿糖", ValueType = QjExaminationItemValueTypeEnum.ST, Unit = "", ReferenceDisplayName = "" });
            m.AddRow(new ExaminationItem { Code = "200003", Name = "尿潜血", ValueType = QjExaminationItemValueTypeEnum.ST, Unit = "", ReferenceDisplayName = "" });
            m.AddRow(new ExaminationItem { Code = "200004", Name = "尿比重", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "", ReferenceDisplayName = "1.005-1.030" });
            m.AddRow(new ExaminationItem { Code = "200005", Name = "尿pH", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "", ReferenceDisplayName = "5.0-8.0" });

            m.AddGroupRow(new ExaminationGroupItem { GroupNo = 3, Name = "循環器", Comment = "" });

            m.AddRow(new ExaminationItem { Code = "300001", Name = "収縮期血圧", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mmHg", ReferenceDisplayName = "-129" });
            m.AddRow(new ExaminationItem { Code = "300002", Name = "拡張期血圧", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "mmHg", ReferenceDisplayName = "-84" });
            m.AddRow(new ExaminationItem { Code = "300003", Name = "心拍数", ValueType = QjExaminationItemValueTypeEnum.PQ, Unit = "回/分", ReferenceDisplayName = "50-100" });
            m.AddRow(new ExaminationItem { Code = "300004", Name = "心電図所見", ValueType = QjExaminationItemValueTypeEnum.ST, Unit = "", ReferenceDisplayName = "" });

            m.AddGroupRow(new ExaminationGroupItem { GroupNo = 4, Name = "画像・肺", Comment = "" });

            m.AddRow(new ExaminationItem { Code = "400001", Name = "胸部X線所見", ValueType = QjExaminationItemValueTypeEnum.ST, Unit = "", ReferenceDisplayName = "" });
            m.AddRow(new ExaminationItem { Code = "400002", Name = "腹部超音波所見", ValueType = QjExaminationItemValueTypeEnum.ST, Unit = "", ReferenceDisplayName = "" });

            m.AddGroupRow(new ExaminationGroupItem { GroupNo = 5, Name = "診察", Comment = "" });

            m.AddRow(new ExaminationItem { Code = "500001", Name = "医師コメント", ValueType = QjExaminationItemValueTypeEnum.ST, Unit = "", ReferenceDisplayName = "" });

            // ============
            // 値投入（ヘルパ）
            // ============
            void SetPQ(string colKey, string code, string value, string unit, string low, string high, string interp, string refDisp)
            {
                m.SetItem(
                    new ExaminationItem
                    {
                        ItemType = QjExaminationItemTypeEnum.Value,
                        Code = code,
                        ValueType = QjExaminationItemValueTypeEnum.PQ,
                        Value = value,
                        Unit = unit,
                        Low = low,
                        High = high,
                        Interpretation = interp,
                        ReferenceDisplayName = refDisp
                    },
                    colKey,
                    code
                );
            }

            void SetST(string colKey, string code, string value)
            {
                m.SetItem(
                    new ExaminationItem
                    {
                        ItemType = QjExaminationItemTypeEnum.Value,
                        Code = code,
                        ValueType = QjExaminationItemValueTypeEnum.ST,
                        Value = value,
                        Interpretation = ""
                    },
                    colKey,
                    code
                );
            }

            // ============
            // col1（概ね正常）
            // ============
            SetPQ(col1Key, "100001", "6200", "個/μL", "4000", "9000", "N", "4000-9000");
            SetPQ(col1Key, "100002", "480", "万/μL", "430", "570", "N", "430-570");
            SetPQ(col1Key, "100003", "15.2", "g/dL", "13.5", "17.6", "N", "13.5-17.6");
            SetPQ(col1Key, "100004", "24.1", "万/μL", "15.0", "35.0", "N", "15.0-35.0");
            SetPQ(col1Key, "100005", "22", "U/L", "13", "30", "N", "13-30");
            SetPQ(col1Key, "100006", "18", "U/L", "10", "42", "N", "10-42");
            SetPQ(col1Key, "100007", "28", "U/L", "13", "64", "N", "13-64");
            SetPQ(col1Key, "100008", "98", "mg/dL", "30", "149", "N", "30-149");
            SetPQ(col1Key, "100009", "56", "mg/dL", "40", "", "N", "40-");
            SetPQ(col1Key, "100010", "102", "mg/dL", "", "119", "N", "-119");
            SetPQ(col1Key, "100011", "96", "mg/dL", "70", "109", "N", "70-109");
            SetPQ(col1Key, "100012", "5.3", "%", "4.6", "6.2", "N", "4.6-6.2");
            SetPQ(col1Key, "100013", "6.1", "mg/dL", "3.7", "7.0", "N", "3.7-7.0");
            SetPQ(col1Key, "100014", "0.88", "mg/dL", "0.65", "1.07", "N", "0.65-1.07");

            SetST(col1Key, "200001", "(-)");
            SetST(col1Key, "200002", "(-)");
            SetST(col1Key, "200003", "(-)");
            SetPQ(col1Key, "200004", "1.015", "", "1.005", "1.030", "N", "1.005-1.030");
            SetPQ(col1Key, "200005", "6.0", "", "5.0", "8.0", "N", "5.0-8.0");

            SetPQ(col1Key, "300001", "118", "mmHg", "", "129", "N", "-129");
            SetPQ(col1Key, "300002", "76", "mmHg", "", "84", "N", "-84");
            SetPQ(col1Key, "300003", "62", "回/分", "50", "100", "N", "50-100");
            SetST(col1Key, "300004", "所見なし");

            SetST(col1Key, "400001", "異常所見なし");
            SetST(col1Key, "400002", "脂肪肝所見なし");

            SetST(col1Key, "500001", "特記事項なし。");

            // ============
            // col2（脂質だけH）
            // ============
            SetPQ(col2Key, "100001", "7100", "個/μL", "4000", "9000", "N", "4000-9000");
            SetPQ(col2Key, "100003", "15.0", "g/dL", "13.5", "17.6", "N", "13.5-17.6");
            SetPQ(col2Key, "100008", "168", "mg/dL", "30", "149", "H", "30-149");
            SetPQ(col2Key, "100010", "132", "mg/dL", "", "119", "H", "-119");
            SetPQ(col2Key, "100011", "101", "mg/dL", "70", "109", "N", "70-109");
            SetPQ(col2Key, "100012", "5.6", "%", "4.6", "6.2", "N", "4.6-6.2");
            SetPQ(col2Key, "100013", "6.8", "mg/dL", "3.7", "7.0", "N", "3.7-7.0");

            SetST(col2Key, "200001", "(-)");
            SetST(col2Key, "200002", "(-)");
            SetST(col2Key, "200003", "(-)");
            SetPQ(col2Key, "300001", "126", "mmHg", "", "129", "N", "-129");
            SetPQ(col2Key, "300002", "84", "mmHg", "", "84", "N", "-84");
            SetST(col2Key, "300004", "所見なし");
            SetST(col2Key, "400001", "異常所見なし");
            SetST(col2Key, "500001", "脂質管理（食事・運動）を推奨。");

            // ============
            // col3（血糖/HbA1c H）
            // ============
            SetPQ(col3Key, "100011", "135", "mg/dL", "70", "109", "H", "70-109");
            SetPQ(col3Key, "100012", "6.6", "%", "4.6", "6.2", "H", "4.6-6.2");
            SetPQ(col3Key, "100008", "142", "mg/dL", "30", "149", "N", "30-149");
            SetST(col3Key, "200002", "(±)");
            SetST(col3Key, "200001", "(-)");
            SetST(col3Key, "300004", "軽度ST-T変化の疑い（要経過観察）");
            SetST(col3Key, "500001", "血糖高値。再検査・生活改善を推奨。");

            // ============
            // col4（尿酸H）
            // ============
            SetPQ(col4Key, "100013", "7.6", "mg/dL", "3.7", "7.0", "H", "3.7-7.0");
            SetPQ(col4Key, "100011", "104", "mg/dL", "70", "109", "N", "70-109");
            SetST(col4Key, "200001", "(-)");
            SetST(col4Key, "200003", "(-)");
            SetST(col4Key, "500001", "尿酸高値。水分摂取・食事の見直し。");

            // ============
            // col5（“表示ボリューム”用にコメント長め/一部Lも混ぜる）
            // ============
            SetPQ(col5Key, "100003", "12.9", "g/dL", "13.5", "17.6", "L", "13.5-17.6");
            SetPQ(col5Key, "100008", "110", "mg/dL", "30", "149", "N", "30-149");
            SetPQ(col5Key, "100011", "92", "mg/dL", "70", "109", "N", "70-109");
            SetST(col5Key, "200001", "(-)");
            SetST(col5Key, "300004", "所見なし");
            SetST(col5Key, "400001", "異常所見なし");
            SetST(col5Key, "400002", "胆のうポリープ疑い（小、経過観察）。");
            SetST(col5Key, "500001", "ヘモグロビンがやや低め。食事（鉄）と体調で再評価。画像所見は経過観察。");

            // 最後に確定（XAxis/YAxis のヘッダ・異常フラグ等の再計算）
            m.UpdateMatrix();

            return m;
        }


        /// <summary>行定義ショートカット</summary>
        private static void AddRow(ExaminationMatrix m, string code, string name, string std, string unit)
        {
            m.AddRow(new ExaminationItem
            {
                Code = code,
                Name = name,
                ReferenceDisplayName = std,
                Unit = unit,
                ValueType = QjExaminationItemValueTypeEnum.PQ
            });
        }

        /// <summary>物理量値セット</summary>
        private static void SetPQ(ExaminationMatrix m, string colKey, string code, double val, double? low, double? high)
        {
            string interp = "N";
            if (low.HasValue && val < low.Value) interp = "L";
            if (high.HasValue && val > high.Value) interp = "H";

            m.SetItem(new ExaminationItem
            {
                ItemType = QjExaminationItemTypeEnum.Value,
                Code = code,
                ValueType = QjExaminationItemValueTypeEnum.PQ,
                Value = val.ToString(),
                Low = low?.ToString() ?? "",
                High = high?.ToString() ?? "",
                Interpretation = interp,
                ReferenceDisplayName = $"{low}-{high}"
            }, colKey, code);
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Configuration;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「沖縄セルラー Yappli」で使用する読み取り専用ディクショナリを表します。
    /// このクラスは継承できません。
    /// </summary>
    public sealed class QjDictionary
    {
        #region Private Method

        /// <summary>
        /// 年数と、西暦・和暦の文字列のディクショナリ―を作成します。
        /// </summary>
        private static Dictionary<string, string> GetCalendar()
        {
            var dic = new Dictionary<string, string>();

            string viewstr = string.Empty;
            int startNo = DateTime.Now.Year;

            DateTime nowYearFirstDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime nowYearLastDate = new DateTime(DateTime.Now.Year, 12, 31);

            string wareki = string.Empty;

            JapaneseCalendar jcalendar = new JapaneseCalendar();
            CultureInfo ci = new CultureInfo("ja-JP", true);

            ci.DateTimeFormat.Calendar = new JapaneseCalendar();

            //メモ：和暦カレンダー機能
            //jcalendar.GetEra(nowYear) = 1:明治、2:大正、3:昭和、4:平成
            //jcalendar.GetYear(nowYear) = 各年号における年数

            for (int i = startNo; i >= DateTime.Now.AddYears(-120).Year; i--)
            {
                //該当年の、元日と大晦日の年号を取得し、差異があれば２つとも表記する
                if (string.Compare(
                        nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci),
                        nowYearLastDate.ToString("yyyy") + nowYearLastDate.ToString("（ggy）年", ci),
                        StringComparison.Ordinal
                    ) == 0)
                {
                    dic.Add(i.ToString(), nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci));
                }
                else
                {
                    dic.Add(
                        i.ToString(),
                        nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy,", ci)
                        + nowYearLastDate.ToString("ggy）年", ci).Replace("1", "元")
                    );
                }

                nowYearFirstDate = nowYearFirstDate.AddYears(-1);
                nowYearLastDate = nowYearLastDate.AddYears(-1);
            }

            return dic;
        }

        /// <summary>
        /// 年数と、西暦・和暦の文字列のディクショナリ―を作成します。***特定健診対象者の生年選択用***
        /// </summary>
        private static Dictionary<string, string> GetTokuteiCalendar()
        {
            var dic = new Dictionary<string, string>();

            string viewstr = string.Empty;
            int startNo = DateTime.Now.Year;
            int endNo = DateTime.Now.AddYears(-120).Year;

            DateTime nowYearFirstDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime nowYearLastDate = new DateTime(DateTime.Now.Year, 12, 31);

            string wareki = string.Empty;

            JapaneseCalendar jcalendar = new JapaneseCalendar();
            CultureInfo ci = new CultureInfo("ja-JP", true);

            ci.DateTimeFormat.Calendar = new JapaneseCalendar();

            //メモ：和暦カレンダー機能
            //jcalendar.GetEra(nowYear) = 1:明治、2:大正、3:昭和、4:平成
            //jcalendar.GetYear(nowYear) = 各年号における年数

            int lower = int.MinValue;
            int upper = int.MinValue;

            try
            {
                lower = int.Parse(WebConfigurationManager.AppSettings["TokuteiKensinLower"].Trim());
                upper = int.Parse(WebConfigurationManager.AppSettings["TokuteiKensinUpper"].Trim());
            }
            catch
            {
            }

            if (lower == int.MinValue)
            {
                lower = 18;
            }
            if (upper == int.MinValue)
            {
                upper = 80;
            }

            //StartNo=Lower (今年39歳の人の生まれ年）
            startNo = DateTime.Now.Year - lower;
            //EndNo = Upper (今年75歳の人の生まれ年）
            endNo = DateTime.Now.Year - upper;

            nowYearFirstDate = new DateTime(startNo, 1, 1);
            nowYearLastDate = new DateTime(startNo, 12, 31);

            for (int i = startNo; i >= endNo; i--)
            {
                //該当年の、元日と大晦日の年号を取得し、差異があれば２つとも表記する
                if (string.Compare(
                        nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci),
                        nowYearLastDate.ToString("yyyy") + nowYearLastDate.ToString("（ggy）年", ci),
                        StringComparison.Ordinal
                    ) == 0)
                {
                    dic.Add(i.ToString(), nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci));
                }
                else
                {
                    dic.Add(
                        i.ToString(),
                        nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy,", ci)
                        + nowYearLastDate.ToString("ggy）年", ci).Replace("1", "元")
                    );
                }

                nowYearFirstDate = nowYearFirstDate.AddYears(-1);
                nowYearLastDate = nowYearLastDate.AddYears(-1);
            }

            return dic;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// 性別の種別のディクショナリを取得します。
        /// </summary>
        public static ReadOnlyDictionary<QjSexTypeEnum, string> SexType
        {
            get
            {
                return new ReadOnlyDictionary<QjSexTypeEnum, string>(
                    new Dictionary<QjSexTypeEnum, string>
                    {
                        { QjSexTypeEnum.Male, "男性" },
                        { QjSexTypeEnum.Female, "女性" }
                    }
                );
            }
        }

        /// <summary>
        /// バイタル情報の測定条件の種別のディクショナリを取得します。
        /// </summary>
        public static ReadOnlyDictionary<QjVitalConditionTypeEnum, string> VitalConditionType
        {
            get
            {
                return new ReadOnlyDictionary<QjVitalConditionTypeEnum, string>(
                    new Dictionary<QjVitalConditionTypeEnum, string>
                    {
                        { QjVitalConditionTypeEnum.None, "随時" },
                        { QjVitalConditionTypeEnum.Fasting, "空腹時" },
                        { QjVitalConditionTypeEnum.LessThanTwoHours, "食後2時間未満" },
                        { QjVitalConditionTypeEnum.NotLessThanTwoHours, "食後2時間以降" }
                    }
                );
            }
        }

        /// <summary>
        /// 都道府県のディクショナリを取得します。
        /// </summary>
        public static ReadOnlyDictionary<byte, string> Prefecture
        {
            get
            {
                return new ReadOnlyDictionary<byte, string>(
                    new Dictionary<byte, string>
                    {
                        { 1, "北海道" },
                        { 2, "青森県" },
                        { 3, "岩手県" },
                        { 4, "宮城県" },
                        { 5, "秋田県" },
                        { 6, "山形県" },
                        { 7, "福島県" },
                        { 8, "茨城県" },
                        { 9, "栃木県" },
                        { 10, "群馬県" },
                        { 11, "埼玉県" },
                        { 12, "千葉県" },
                        { 13, "東京都" },
                        { 14, "神奈川県" },
                        { 15, "新潟県" },
                        { 16, "富山県" },
                        { 17, "石川県" },
                        { 18, "福井県" },
                        { 19, "山梨県" },
                        { 20, "長野県" },
                        { 21, "岐阜県" },
                        { 22, "静岡県" },
                        { 23, "愛知県" },
                        { 24, "三重県" },
                        { 25, "滋賀県" },
                        { 26, "京都府" },
                        { 27, "大阪府" },
                        { 28, "兵庫県" },
                        { 29, "奈良県" },
                        { 30, "和歌山県" },
                        { 31, "鳥取県" },
                        { 32, "島根県" },
                        { 33, "岡山県" },
                        { 34, "広島県" },
                        { 35, "山口県" },
                        { 36, "徳島県" },
                        { 37, "香川県" },
                        { 38, "愛媛県" },
                        { 39, "高知県" },
                        { 40, "福岡県" },
                        { 41, "佐賀県" },
                        { 42, "長崎県" },
                        { 43, "熊本県" },
                        { 44, "大分県" },
                        { 45, "宮崎県" },
                        { 46, "鹿児島県" },
                        { 47, "沖縄県" }
                    }
                );
            }
        }

        /// <summary>
        /// AM/PM のディクショナリを表します。
        /// </summary>
        public static ReadOnlyDictionary<string, string> Meridiem
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(
                    new Dictionary<string, string>
                    {
                        { "am", "午前" },
                        { "pm", "午後" }
                    }
                );
            }
        }

        /// <summary>
        /// 年（西暦・和暦併記）のディクショナリを取得します。
        /// </summary>
        public static ReadOnlyDictionary<string, string> Year
        {
            get
            {
                Dictionary<string, string> dic = GetCalendar();
                return new ReadOnlyDictionary<string, string>(dic);
            }
        }

        /// <summary>
        /// 年（西暦・和暦併記）のディクショナリを取得します。***特定健診対象者の生年選択用***
        /// </summary>
        public static ReadOnlyDictionary<string, string> TokuteiYear
        {
            get
            {
                Dictionary<string, string> dic = GetTokuteiCalendar();
                return new ReadOnlyDictionary<string, string>(dic);
            }
        }

        /// <summary>
        /// 食事の種別のディクショナリを取得します。
        /// </summary>
        public static ReadOnlyDictionary<QjMealTypeEnum, string> MealType
        {
            get
            {
                return new ReadOnlyDictionary<QjMealTypeEnum, string>(
                    new Dictionary<QjMealTypeEnum, string>
                    {
                        { QjMealTypeEnum.Breakfast, "朝食" },
                        { QjMealTypeEnum.Lunch, "昼食" },
                        { QjMealTypeEnum.Dinner, "夕食" },
                        { QjMealTypeEnum.Snacking, "間食" }
                    }
                );
            }
        }

        /// <summary>
        /// 薬剤の剤型の種別のディクショナリを取得します。
        /// </summary>
        public static ReadOnlyDictionary<QjDosageFormTypeEnum, string> DosageFormType
        {
            get
            {
                return new ReadOnlyDictionary<QjDosageFormTypeEnum, string>(
                    new Dictionary<QjDosageFormTypeEnum, string>
                    {
                        { QjDosageFormTypeEnum.Oral, "内服" },
                        { QjDosageFormTypeEnum.Drip, "内滴" },
                        { QjDosageFormTypeEnum.DoseOfMedicine, "頓服" },
                        { QjDosageFormTypeEnum.InjectionDrug, "注射" },
                        { QjDosageFormTypeEnum.External, "外用" },
                        { QjDosageFormTypeEnum.DipFry, "浸煎" },
                        { QjDosageFormTypeEnum.Touzai, "湯" },
                        { QjDosageFormTypeEnum.Materials, "材料" },
                        { QjDosageFormTypeEnum.Other, "その他" }
                    }
                );
            }
        }

        /// <summary>
        /// 連携内容の種別のディクショナリを取得します。
        /// </summary>
        public static ReadOnlyDictionary<QjRelationContentTypeEnum, string> RelationContentType
        {
            get
            {
                return new ReadOnlyDictionary<QjRelationContentTypeEnum, string>(
                    new Dictionary<QjRelationContentTypeEnum, string>
                    {
                        { QjRelationContentTypeEnum.Information, "基本情報" },
                        { QjRelationContentTypeEnum.Vital, "バイタル情報" },
                        { QjRelationContentTypeEnum.Medicine, "お薬情報" },
                        { QjRelationContentTypeEnum.Examination, "検査・健診情報" },
                        { QjRelationContentTypeEnum.Contact, "連絡情報" },
                        { QjRelationContentTypeEnum.Dental, "歯科情報" },
                        { QjRelationContentTypeEnum.Assessment, "活動情報" },
                        { QjRelationContentTypeEnum.Meal, "食事情報" }
                    }
                );
            }
        }

        #endregion
    }
}
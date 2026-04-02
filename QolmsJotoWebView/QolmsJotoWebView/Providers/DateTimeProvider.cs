using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// DateTimeを抽象化したインターフェース
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// 現在のローカル時間を取得する
        /// </summary>
        DateTime Now { get; }
        /// <summary>
        /// 現在のUTC時間を取得する
        /// </summary>
        DateTime UtcNow { get; }
        /// <summary>
        /// 今日のDateTimeを取得する
        /// </summary>
        DateTime Today { get; }
        /// <summary>
        /// 今日の最終日時を取得する
        /// 今日の23:59:59.99999.....を返す
        /// </summary>
        DateTime TodayLast { get; }
        (DateTime first, DateTime last) GetMonthRange(int year, int month);
    }

    /// <summary>
    /// 日付の取得を提供します。
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeProvider()
        {
        }
        public DateTime UtcNow => DateTimeOffset.UtcNow.DateTime;
        public DateTime Now => DateTime.Now;
        public DateTime Today => DateTime.Today;
        public DateTime TodayLast => DateTime.Today.AddDays(1).AddMilliseconds(-1);

        public (DateTime first, DateTime last) GetMonthRange(int year, int month)
        {
            var days = DateTime.DaysInMonth(year, month);

            var first = new DateTime(year, month, 1, 0, 0, 0);
            var last = new DateTime(year, month, days, 23, 59, 59);

            return (first, last);
        }
    }
}
using System;
using System.Collections.Generic;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// バイタル警告対象者を表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbVitalAlertItem
    {


        /// <summary>
        /// アラート番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public long AlertNo { get; set; } = long.MinValue;

        /// <summary>
        /// 登録日を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// アカウントキーをを取得または設定します。　
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        public decimal Value1 { get; set; } = decimal.MinValue;

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        public decimal Value2 { get; set; } = decimal.MinValue;

        /// <summary>
        /// アラートメッセージを取得または設定します。
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 対象者名を取得または設定します。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// メールアドレスを取得または設定します。
        /// </summary>
        public string Mail { get; set; } = string.Empty;

        /// <summary>
        /// 電話番号を取得または設定します。　
        /// </summary>
        public string Tel { get; set; } = string.Empty;

        /// <summary>
        /// 住所を取得または設定します。
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 連絡先家族続柄を取得または設定します。
        /// </summary>
        public string FamilyRelationship { get; set; } = string.Empty;

        /// <summary>
        /// 家族の名前を取得または設定します。
        /// </summary>
        public string FamilyName { get; set; } = string.Empty;

        /// <summary>
        /// 家族の電話番号を取得または設定します。
        /// </summary>
        public string FamilyTel { get; set; } = string.Empty;


      
        /// <summary>
        /// <see cref="DbVitalAlertItem" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbVitalAlertItem()
        {
        }
    }


}
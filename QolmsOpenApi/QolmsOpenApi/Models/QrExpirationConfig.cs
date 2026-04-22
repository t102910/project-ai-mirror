using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// QRコードの有効期限設定
    /// </summary>
    public class QrExpirationConfig
    {
        /// <summary>
        /// 受付票の有効期限
        /// </summary>
        public int ReceptionExpirationHour { get; set; }
        /// <summary>
        /// 予約票の有効期限
        /// </summary>
        public int ReservationExpirationHour { get; set; }

        /// <summary>
        /// カンマ区切り文字列を渡してインスタンスを生成する
        /// </summary>
        /// <param name="value"></param>
        public QrExpirationConfig(string value)
        {
            try
            {
                var values = value.Split(',');

                ReceptionExpirationHour = values.ElementAtOrDefault(0).TryToValueType(24);
                ReservationExpirationHour = values.ElementAtOrDefault(1).TryToValueType(48);
            }
            catch
            {
                // 例外時はデフォルト値を設定
                ReceptionExpirationHour = 24;
                ReservationExpirationHour = 48;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public QrExpirationConfig()
        {
            // デフォルト値を設定
            ReceptionExpirationHour = 24;
            ReservationExpirationHour = 48;
        }
    }
}
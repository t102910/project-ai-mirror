using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_PATIENTCARD QH_FACILITY_MST QH_LINKAGE_DAT QH_LINKAGESYSTEM_MST
    /// を組みあわせたアプリから利用するための情報を抽出したVIEW
    /// (実際にDBにVIEWを作成しているわけではなくあくまでクエリ結果格納用）
    /// </summary>
    public class QH_PATIENTCARD_FACILITY_VIEW : QsDbEntityBase
    {
        /// <summary>
        /// アカウントキー
        /// </summary>
        public Guid ACCOUNTKEY { get; set; } = Guid.Empty;
        /// <summary>
        /// カード種別番号
        /// </summary>
        public int CARDCODE { get; set; } = int.MinValue;
        /// <summary>
        /// カード連番
        /// </summary>
        public int SEQUENCE { get; set; } = int.MinValue;
        /// <summary>
        /// 施設キー
        /// </summary>
        public Guid FACILITYKEY { get; set; } = Guid.Empty;
        /// <summary>
        /// 利用者カードの番号
        /// </summary>
        public string CARDNO { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CREATEDDATE { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 連携状態
        /// </summary>
        public byte STATUSTYPE { get; set; } = byte.MinValue;
        /// <summary>
        /// 施設名
        /// </summary>
        public string FACILITYNAME { get; set; } = string.Empty;
        /// <summary>
        /// 施設カナ
        /// </summary>
        public string FACILITYKANANAME { get; set; } = string.Empty;
        /// <summary>
        /// 郵便番号
        /// </summary>
        public string POSTALCODE { get; set; } = string.Empty;
        /// <summary>
        /// 住所1
        /// </summary>
        public string ADDRESS1 { get; set; } = string.Empty;
        /// <summary>
        /// 住所2
        /// </summary>
        public string ADDRESS2 { get; set; } = string.Empty;
        /// <summary>
        /// 都道府県コード
        /// </summary>
        public byte PREFNO { get; set; } = byte.MinValue;
        /// <summary>
        /// 地区コード
        /// </summary>
        public int CITYNO { get; set; } = int.MinValue;
        /// <summary>
        /// 電話番号
        /// </summary>
        public string TEL { get; set; } = string.Empty;
        /// <summary>
        /// FAX
        /// </summary>
        public string FAX { get; set; } = string.Empty;
        /// <summary>
        /// 正式名称
        /// </summary>
        public string OFFICIALNAME { get; set; } = string.Empty;
        /// <summary>
        /// 医療機関コード
        /// </summary>
        public string MEDICALFACILITYCODE { get; set; } = string.Empty;

        /// <summary>
        /// カスタムバーコード用の書式
        /// </summary>
        public string CustomCodeFormat { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                ACCOUNTKEY = reader.GetGuid(0);
                CARDCODE = reader.GetInt32(1);
                SEQUENCE = reader.GetInt32(2);
                FACILITYKEY = reader.GetGuid(3);
                CARDNO = reader.GetString(4);
                CREATEDDATE = reader.GetDateTime(5);
                STATUSTYPE = reader.IsDBNull(6) ? byte.MinValue : reader.GetByte(6);
                FACILITYNAME = reader.GetString(7);
                FACILITYKANANAME = reader.GetString(8);
                POSTALCODE = reader.GetString(9);
                ADDRESS1 = reader.GetString(10);
                ADDRESS2 = reader.GetString(11);
                PREFNO = reader.GetByte(12);
                CITYNO = reader.GetInt32(13);
                TEL = reader.GetString(14);
                FAX = reader.GetString(15);
                OFFICIALNAME = reader.GetString(16);
                MEDICALFACILITYCODE = reader.GetString(17);
                CustomCodeFormat = reader.GetString(18);

                KeyGuid = Guid.NewGuid();
                DataState = QsDbEntityStateTypeEnum.Unchanged;
                IsEmpty = false;
            }
            catch
            {
                throw;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsKeysValid()
        {
            return true;
        }
    }
}
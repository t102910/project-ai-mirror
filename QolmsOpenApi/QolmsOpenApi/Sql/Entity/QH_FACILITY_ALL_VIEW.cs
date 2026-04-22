using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_FACILITY_MST / QH_FACILITYADDITION_MST / QH_FACILITYFILE_MST
    /// QH_FACILITYMEDICALDEPARTMENT_DAT / QH_FACILITYCONTACTINFOMATION_DAT
    /// QH_FACILITYURL_DAT
    /// の情報を結合してアプリから利用するためのVIEW
    /// 1:Nのとなる情報はJSONとして返すようになっており、デシリアライズした情報も保持する
    /// (実際にDBにVIEWを作成しているわけではなくあくまでクエリ結果格納用）
    /// </summary>
    public class QH_FACILITY_ALL_VIEW : QsDbEntityBase
    {
        /// <summary>
        /// 施設キー
        /// </summary>
        public Guid FACILITYKEY { get; set; } = Guid.Empty;
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
        /// 正式名称
        /// </summary>
        public string OFFICIALNAME { get; set; } = string.Empty;
        /// <summary>
        /// 医療機関コード
        /// </summary>
        public string MEDICALFACILITYCODE { get; set; } = string.Empty;
        /// <summary>
        /// 表示順
        /// </summary>
        public int DISPORDER { get; set; } = 0;
        /// <summary>
        /// 更新日
        /// </summary>
        public DateTime UPDATEDDATE { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal LATITUDE { get; set; } = 0;
        /// <summary>
        /// 経度 
        /// </summary>
        public decimal LONGITUDE { get; set; } = 0;
        /// <summary>
        /// ファイルキーリスト (JSON)
        /// </summary>
        [JsonIgnore]
        public string FILEJSON { get; set; } = string.Empty;
        /// <summary>
        /// 診療科リスト(JSON)
        /// </summary>
        [JsonIgnore]
        public string DEPARTMENTJSON { get; set; } = string.Empty;
        /// <summary>
        /// 連絡先リスト(JSON)
        /// </summary>
        [JsonIgnore]
        public string CONTACTJSON { get; set; } = string.Empty;
        /// <summary>
        /// URLリスト(JSON)
        /// </summary>
        [JsonIgnore]
        public string URLJSON { get; set; } = string.Empty;

        /// <summary>
        /// ファイルリスト 
        /// </summary>
        public List<FileInfo> FileList { get; set; }
        /// <summary>
        /// 診療科リスト
        /// </summary>
        public List<DepertmentInfo> DepertmentList { get; set; }
        /// <summary>
        /// 連絡先リスト
        /// </summary>
        public List<ContactInfo> ContactList { get; set; }
        /// <summary>
        /// URLリスト
        /// </summary>
        public List<UrlInfo> UrlList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                FACILITYKEY = reader.GetGuid(0);
                FACILITYNAME = reader.GetString(1);
                FACILITYKANANAME = reader.GetString(2);
                POSTALCODE = reader.GetString(3);
                ADDRESS1 = reader.GetString(4);
                ADDRESS2 = reader.GetString(5);
                PREFNO = reader.GetByte(6);
                CITYNO = reader.GetInt32(7);
                OFFICIALNAME = reader.GetString(8);
                MEDICALFACILITYCODE = reader.GetString(9);
                DISPORDER = reader.GetInt32(10);
                UPDATEDDATE = reader.GetDateTime(11);
                LATITUDE = reader.IsDBNull(12) ? 0 : reader.GetDecimal(12);
                LONGITUDE = reader.IsDBNull(13) ? 0 : reader.GetDecimal(13);
                FILEJSON = reader.IsDBNull(14) ? string.Empty : reader.GetString(14);
                DEPARTMENTJSON = reader.IsDBNull(15) ? string.Empty : reader.GetString(15);
                CONTACTJSON = reader.IsDBNull(16) ? string.Empty : reader.GetString(16);
                URLJSON = reader.IsDBNull(17) ? string.Empty : reader.GetString(17);

                KeyGuid = Guid.NewGuid();
                DataState = QsDbEntityStateTypeEnum.Unchanged;
                IsEmpty = false;

                // Json項目の展開
                FileList = JsonConvert.DeserializeObject<List<FileInfo>>(FILEJSON) ?? new List<FileInfo>();
                DepertmentList = JsonConvert.DeserializeObject<List<DepertmentInfo>>(DEPARTMENTJSON) ?? new List<DepertmentInfo>();
                ContactList = JsonConvert.DeserializeObject<List<ContactInfo>>(CONTACTJSON) ?? new List<ContactInfo>();
                UrlList = JsonConvert.DeserializeObject<List<UrlInfo>>(URLJSON) ?? new List<UrlInfo>();

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

        /// <summary>
        /// 
        /// </summary>
        public class FileInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public int Sequence { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Guid FileKey { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DepertmentInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public int DepartmentNo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string DepartmentName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string LocalCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string LocalName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int DispOrder { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class ContactInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public int ContactInformationNo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public byte ContactInformationType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string TelAreaCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string TelCityCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string TelSubscriberNumber { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string TelFull { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string AcceptedStart { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string AcceptedEnd { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string CommentSet { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int DispOrder { get; set; }

            QhFacilityContactInformationCommentSetOfJson _comment;
            /// <summary>
            /// 
            /// </summary>
            public QhFacilityContactInformationCommentSetOfJson Comment
            {
                get
                {
                    if(_comment == null)
                    {
                        _comment = JsonConvert.DeserializeObject<QhFacilityContactInformationCommentSetOfJson>(CommentSet);
                    }
                    return _comment;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class UrlInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public int UrlNo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public byte UrlType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int DispOrder { get; set; }
        }
    }
}
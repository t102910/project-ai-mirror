using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_ETHDRUGMEIGARA_MST/QH_ETHDRUGYJHA_MST/QH_ETHDRUGYJHS_MST/QH_ETHDRUGFILE_MST
    /// を組み合わせた調剤薬詳細情報を抽出したVIEW
    /// (実際にDBにVIEWを作成しているわけではなくあくまでクエリ結果格納用）
    /// </summary>
    public class QH_ETHDRUG_DETAIL_VIEW : QsDbEntityBase
    {
        public string YJCODE { get; set; }              // YJコード
        public string PRODUCTNAME { get; set; }         // 製品名
        public string COMMONNAME { get; set; }          // 一般名
        public string APPROVALCOMPANYNAME { get; set; } // 製造販売承認取得会社名
        public string SALESCOMPANYNAME { get; set; }    // 販売会社名
        public string INGREDIENTS { get; set; }         // 配合成分
        public string GENERALCODE { get; set; }         // 一般項目 コード
        public string ACTIONA { get; set; }             // 作用 A
        public string ACTIONB { get; set; }             // 作用 B
        public string ACTIONC1 { get; set; }            // 作用 C-1
        public string ACTIONC2 { get; set; }            // 作用 C-2
        public string PRECAUTIONS { get; set; }         // 注意事項
        public string DRUGORFOOD { get; set; }          // 対象薬剤・食品
        public string INTERACTION { get; set; }         // 患者向け相互作用情報
        public List<QH_ETHDRUGFILE_MST> FileEntityN { get; set; } = new List<QH_ETHDRUGFILE_MST>(); // 医療用医薬品製剤写真マスタテーブル

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                YJCODE = reader.GetString(0).Trim();
                PRODUCTNAME = reader.GetString(1);
                COMMONNAME = reader.GetString(2);
                APPROVALCOMPANYNAME = reader.GetString(3);
                SALESCOMPANYNAME = reader.GetString(4);
                INGREDIENTS = reader.GetString(5);
                GENERALCODE = reader.GetString(6);
                ACTIONA = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                ACTIONB = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);
                ACTIONC1 = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
                ACTIONC2 = reader.IsDBNull(10) ? string.Empty : reader.GetString(10);
                PRECAUTIONS = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);
                DRUGORFOOD = reader.IsDBNull(12) ? string.Empty : reader.GetString(12);
                INTERACTION = reader.IsDBNull(13) ? string.Empty : reader.GetString(13);

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
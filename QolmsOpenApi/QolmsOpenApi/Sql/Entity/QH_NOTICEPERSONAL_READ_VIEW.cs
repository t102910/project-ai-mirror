using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Data.Common;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// QH_NOTICEPERSONAL_DAT と QH_NOTICEPERSONALALREADYREAD_DAT および QH_ACCOUNTINDEX_DAT
    /// を組みあわせた個人向けお知らせ取得用 VIEW
    /// （実際にDBにVIEWを作成しているわけではなくあくまでクエリ結果格納用）
    /// </summary>
    public class QH_NOTICEPERSONAL_READ_VIEW : QsDbEntityBase
    {
        /// <summary>
        /// お知らせ番号
        /// </summary>
        public long NOTICENO { get; set; } = long.MinValue;

        /// <summary>
        /// タイトル
        /// </summary>
        public string TITLE { get; set; } = string.Empty;

        /// <summary>
        /// 内容
        /// </summary>
        public string CONTENTS { get; set; } = string.Empty;

        /// <summary>
        /// カテゴリ番号
        /// </summary>
        public byte CATEGORYNO { get; set; } = byte.MinValue;

        /// <summary>
        /// 優先度
        /// </summary>
        public byte PRIORITYNO { get; set; } = byte.MinValue;

        /// <summary>
        /// 対象アカウントキー
        /// </summary>
        public Guid ACCOUNTKEY { get; set; } = Guid.Empty;

        /// <summary>
        /// 施設キー
        /// </summary>
        public Guid FACILITYKEY { get; set; } = Guid.Empty;

        /// <summary>
        /// 開始日時
        /// </summary>
        public DateTime STARTDATE { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 終了日時
        /// </summary>
        public DateTime ENDDATE { get; set; } = DateTime.MinValue;

        /// <summary>
        /// お知らせデータセット
        /// </summary>
        public string NOTICEDATASET { get; set; } = string.Empty;

        /// <summary>
        /// 既読フラグ
        /// </summary>
        public bool ALREADYREADFLAG { get; set; }

        /// <summary>
        /// 送信者姓
        /// </summary>
        public string SENDERFAMILYNAME { get; set; } = string.Empty;

        /// <summary>
        /// 送信者名
        /// </summary>
        public string SENDERGIVENNAME { get; set; } = string.Empty;

        /// <summary>
        /// 総件数（一覧取得時のみ設定）
        /// </summary>
        public int TOTAL_COUNT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
        {
            try
            {
                NOTICENO = reader.GetInt64(0);
                TITLE = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                CONTENTS = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                CATEGORYNO = reader.GetByte(3);
                PRIORITYNO = reader.GetByte(4);
                ACCOUNTKEY = reader.GetGuid(5);
                FACILITYKEY = reader.GetGuid(6);
                STARTDATE = reader.GetDateTime(7);
                ENDDATE = reader.GetDateTime(8);
                NOTICEDATASET = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
                ALREADYREADFLAG = reader.IsDBNull(10) ? false : Convert.ToBoolean(reader.GetValue(10));
                SENDERFAMILYNAME = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);
                SENDERGIVENNAME = reader.IsDBNull(12) ? string.Empty : reader.GetString(12);
                TOTAL_COUNT = reader.FieldCount > 14 ? reader.GetInt32(14) : 0;

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

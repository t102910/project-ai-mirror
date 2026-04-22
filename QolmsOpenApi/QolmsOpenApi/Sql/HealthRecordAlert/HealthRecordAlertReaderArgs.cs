using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// バイタル警告の情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class HealthRecordAlertReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 取得開始対象記録日を取得または設定します。
        /// </summary>
        public DateTime TargetStartRecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 取得終了対象記録日を取得または設定します。
        /// </summary>
        public DateTime TargetEndRecordDate { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// アラートキー番号を取得または設定します。
        /// </summary>
        public long AlertNo { get; set; } = long.MinValue;

        /// <summary>
        /// バイタル種別を取得または設定します。
        /// </summary>
        public byte VitalType { get; set; } = byte.MinValue;


        /// <summary>
        /// <see cref="HealthRecordAlertReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public HealthRecordAlertReaderArgs() : base()
        {
        }
    }


}
using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 施設の情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FacilityMasterReaderArgs : QsDbReaderArgsBase<QH_FACILITY_MST>
    {


        /// <summary>
        /// 医療機関番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>医療機関番号か、連携システム番号のどちらかを設定します。両方ある場合は医療機関番号を優先。</remarks>
        public string MedicalFacilityCode { get; set; } = string.Empty;


        /// <summary>
        /// 連携システム番号を取得または設定します。
        /// </summary>
        /// <remarks>医療機関番号か、連携システム番号のどちらかを設定します。両方ある場合は医療機関番号を優先。</remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// <see cref="FacilityMasterReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMasterReaderArgs() : base()
        {
        }
    }


}
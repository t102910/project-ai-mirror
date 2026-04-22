using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 医療施設検索の情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class MedicalFacilitySearchReaderArgs : QsDbReaderArgsBase<QH_FACILITY_MST>
    {


        /// <summary>
        /// 医療機関番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>医療機関番号が設定されている場合は医療機関番号を優先。</remarks>
        public string MedicalFacilityCode { get; set; } = string.Empty;

        /// <summary>
        /// 都道府県コードを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PrefectureCode { get; set; } = string.Empty;

        /// <summary>
        /// 検索文字を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string KeyWord { get; set; } = string.Empty;

        /// <summary>
        /// 取得ページインデックスを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PageIndex { get; set; } = int.MinValue;

        /// <summary>
        /// 1ページあたりの取得件数を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PageSize { get; set; } = int.MinValue;

        /// <summary>
        /// <see cref="FacilityMasterReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public MedicalFacilitySearchReaderArgs() : base()
        {
        }
    }


}
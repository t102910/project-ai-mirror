using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// 診療科設定情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FacilityMedicalDepartmentConfigReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// LinkageSystemNo を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// 診療科コード を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DepartmentCode { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="FacilityMedicalDepartmentConfigArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityMedicalDepartmentConfigReaderArgs() : base()
        {
        }
    }


}
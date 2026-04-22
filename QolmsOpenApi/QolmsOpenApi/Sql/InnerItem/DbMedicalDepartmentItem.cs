using System;
using System.Collections.Generic;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 診療科情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbMedicalDepartmentItem
    {


        /// <summary>
        /// 診療科番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int DepartmentNo { get; set; } = int.MinValue;


        /// <summary>
        /// 診療科名を取得または設定します。
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 診療科ローカルコードを取得または設定します。
        /// </summary>
        public string LocalCode { get; set; } = string.Empty;

        /// <summary>
        /// 診療科ローカル名を取得または設定します。
        /// </summary>
        public string LocalName { get; set; } = string.Empty;





        /// <summary>
        /// <see cref="DbMedicalDepartmentItem" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbMedicalDepartmentItem()
        {
        }
    }


}
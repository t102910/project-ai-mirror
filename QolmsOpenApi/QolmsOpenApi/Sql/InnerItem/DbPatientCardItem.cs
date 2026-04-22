using System;
using System.Collections.Generic;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    /// <summary>
    /// 利用者カードを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbPatientCardItem
    {


        /// <summary>
        /// カード種別番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int CardCode { get; set; } = int.MinValue;

        /// <summary>
        /// カード番号を取得または設定します。
        /// </summary>
        public string CardNo { get; set; } = string.Empty;

        /// <summary>
        /// カード連番を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Sequence { get; set; } = int.MinValue;

        /// <summary>
        /// 施設キーを取得または設定します。
        /// </summary>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// カード登録日を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime CreatedDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 利用者カードの情報を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PatientCardSet { get; set; } = string.Empty;

        /// <summary>
        /// 添付ファイル情報のリストを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<DbAttachedFileItem> AttachedFileN { get; set; } = new List<DbAttachedFileItem>();

        /// <summary>
        /// 連携状態を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte StatusType { get; set; } = byte.MinValue;

        /// <summary>
        /// 連携拒否理由を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DisapprovedReason { get; set; } = string.Empty;




        /// <summary>
        /// <see cref="DbPatientCardItem" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public DbPatientCardItem()
        {
        }
    }


}
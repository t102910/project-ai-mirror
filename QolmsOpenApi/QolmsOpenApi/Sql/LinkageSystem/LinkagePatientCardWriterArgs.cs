using System;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    ///   
    internal sealed class LinkagePatientCardWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// 所有者アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 対象者アカウントキーを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid ActorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// カード種別番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int CardCode { get; set; } = int.MinValue;

        /// <summary>
        /// カード連番を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>新規はint.MinValue</remarks>
        public int Sequence { get; set; } = int.MinValue;

        /// <summary>
        /// 施設番号を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid FacilityKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 利用者カードの情報を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PatientCardSet { get; set; } = string.Empty;

        /// <summary>
        /// 削除フラグを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DeleteFlag { get; set; } = false;

        /// <summary>
        /// 連携状態を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte StatusType { get; set; } = byte.MinValue;

        /// <summary>
        /// 連携情報のデータセットを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSet { get; set; } = string.Empty;



        /// <summary>
        /// <see cref="LinkagePatientCardWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkagePatientCardWriterArgs() : base()
        {
        }
    }


}
using System;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// 健診アップロードファイルシーケンス情報を、
    /// データベーステーブルから取得するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class CheckupFileSequenceReaderArgs : QsDbReaderArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// LinkageSystemNo を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// LinkageSystemId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// ServiceCode を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ServiceCode { get; set; } = string.Empty;

        /// <summary>
        /// 医師コード を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime EffectiveDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// OrganizationId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string OrganizationId { get; set; } = string.Empty;

        /// <summary>
        /// SystemType を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public QsApiSystemTypeEnum SystemType { get; set; } = QsApiSystemTypeEnum.None;


        /// <summary>
        /// <see cref="CheckupFileSequenceReaderArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupFileSequenceReaderArgs() : base()
        {
        }
    }


}
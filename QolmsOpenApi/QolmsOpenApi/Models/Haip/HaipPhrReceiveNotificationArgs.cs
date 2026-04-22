using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// HAIP データ収集完了通知受信 APIを実行するための情報を格納する引数クラスを表します。 このクラスは継承できません。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public sealed class HaipPhrReceiveNotificationArgs
    {
        #region "Public Property"

        /// <summary>
        /// XRequestId を取得または設定します。
        /// </summary>
        public string XRequestId { get; set; } = string.Empty;

        /// <summary>
        /// XApiKey を取得または設定します。
        /// </summary>
        public string XApiKey { get; set; } = string.Empty;

        /// <summary>
        /// result_status を取得または設定します。
        /// 20：完了通知受信(正常)　提供元へのデータ収集が正常に行われ、データチェックもすべて正常に行われた場合。
        /// 21：完了通知受信(データなし)　収集対象データがない場合や認可済の提供元がない場合。
        /// 22：完了通知受信(一部エラー)　提供元へのデータ収集もしくはデータチェック一部エラーがあり、収集できたデータのみ返却した場合。
        /// 23：完了通知受信(エラー)　提供元へのデータ収集もしくはデータチェックエラーのため、データ収集できなかった場合。
        /// </summary>
        [DataMember(Name = "result_status")]
        public string ResultStatus { get; set; } = string.Empty;
        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="HaipPhrReceiveNotificationArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HaipPhrReceiveNotificationArgs() : base() { }

        #endregion
    }
}
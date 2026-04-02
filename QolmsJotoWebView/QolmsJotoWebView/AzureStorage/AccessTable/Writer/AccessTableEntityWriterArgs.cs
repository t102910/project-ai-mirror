using MGF.QOLMS.QolmsAzureStorageCoreV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アクセス ログ テーブル ストレージ へ値を登録するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TTableEntity"></typeparam>
    internal sealed class AccessTableEntityWriterArgs<TTableEntity> 
        :QsAzureTableStorageWriterArgsBase<TTableEntity> where TTableEntity : QsAccessTableEntityBase
    {
        #region "Public Property"

        /// <summary>
        /// アカウント キー を取得または設定します。
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// アクセス 日時を取得または設定します。
        /// </summary>
        public DateTime AccessDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// アクセス ログ の種別を取得または設定します。
        /// </summary>
        public byte AccessType { get; set; } = byte.MinValue;

        /// <summary>
        /// アクセス URI を取得または設定します。
        /// </summary>
        public string AccessUri { get; set; } = string.Empty;

        /// <summary>
        /// 補足 コメント を取得または設定します。
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// ユーザー ホスト アドレス を取得または設定します。
        /// </summary>
        public string UserHostAddress { get; set; } = string.Empty;

        /// <summary>
        /// ユーザー ホスト 名を取得または設定します。
        /// </summary>
        public string UserHostName { get; set; } = string.Empty;

        /// <summary>
        /// ユーザー エージェント を取得または設定します。
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="AccessTableEntityWriterArgs" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AccessTableEntityWriterArgs() : base() { }

        #endregion
    }
}
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// Line事前登録情報 を、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class LinePreRegistWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// LineUserId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// LinkageSystemId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LinkageSystemId { get; set; } = string.Empty;

        /// <summary>
        /// 生年月日 を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime Birthday { get; set; } = DateTime.MinValue;

        /// <summary>
        /// DeleteFlag を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DeleteFlag { get; set; } = false;

        /// <summary>
        /// <see cref="LinePreRegistWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinePreRegistWriterArgs() : base()
        {
        }
    }


}
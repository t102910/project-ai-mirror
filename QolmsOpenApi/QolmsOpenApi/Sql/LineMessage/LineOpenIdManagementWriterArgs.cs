using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// OpenIdManagement情報 を、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class LineOpenIdManagementWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// LineUserId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// LinkageSystemNo を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LinkageSystemNo { get; set; } = int.MinValue;

        /// <summary>
        /// DeleteFlag を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DeleteFlag { get; set; } = false;

        /// <summary>
        /// <see cref="LineOpenIdManagementWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LineOpenIdManagementWriterArgs() : base()
        {
        }
    }


}
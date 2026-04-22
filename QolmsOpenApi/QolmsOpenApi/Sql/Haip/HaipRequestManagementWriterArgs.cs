using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// HAIPデータ取得依頼情報 を、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class HaipRequestManagementWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {


        /// <summary>
        /// HAIP を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid RequestId { get; set; } = Guid.Empty;

        /// <summary>
        /// LinkageSystemId を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ResponseStatus { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="HaipRequestManagementWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public HaipRequestManagementWriterArgs() : base()
        {
        }
    }


}
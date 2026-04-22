using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ReproPush通知登録情報 を、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class PushSendLogWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// 登録エンティティ を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<QH_PUSHSEND_LOG> Entities { get; set; }

        /// <summary>
        /// <see cref="PushSendLogWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public PushSendLogWriterArgs() : base()
        {
        }
    }


}
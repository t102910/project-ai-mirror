using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 学童健診Blob管理データ を、
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class CheckupAfterSchoolFileWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// UpsertEntity を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public QH_CHECKUPAFTERSCHOOLFILE_DAT Entity { get; set; }

        /// <summary>
        /// <see cref="CheckupCareFileWriterArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupAfterSchoolFileWriterArgs() : base()
        {
        }
    }


}
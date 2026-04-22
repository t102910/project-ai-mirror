using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// OpenApiからの運動データを
    /// データベース テーブルへ登録するための情報を格納する引数クラスを表します。
    /// </summary>
    public class ExerciseEventWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 所有者アカウント キーを取得または設定します。
        /// </summary>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 対象者アカウント キーを取得または設定します。
        /// </summary>
        public Guid ActorKey { get; set; } = Guid.Empty;


        /// <summary>
        /// バイタル情報のリストを取得または設定します。
        /// </summary>
        public List<DbVitalValueItem> VitalValueN { get; set; } = new List<DbVitalValueItem>();
    }
}
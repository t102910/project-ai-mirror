using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    internal class DailySequenceGenerateWriterArgs: QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 対象のSystem種別を設定。
        /// </summary>
        public QsApiSystemTypeEnum SystemType { get; set; } = QsApiSystemTypeEnum.None;


        /// <summary>
        /// 機能番号を設定
        /// </summary>
        public int FunctionNo { get; set; } = 0;

        /// <summary>
        /// 対象の日付を設定。
        /// </summary>
        public DateTime TargetDate { get; set; }

    }
}
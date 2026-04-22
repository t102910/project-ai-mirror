using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// QH_DAILYSEQUENCE_MSTの入出力
    /// </summary>
    public interface IDailySequenceRepository
    {
        /// <summary>
        /// 日別の新しい連番を取得する
        /// </summary>
        /// <param name="targetDate">対象日。省略時今日。</param>
        /// <param name="systemType">対象システム。省略時MGF共通。</param>
        /// <param name="functionNo">機能番号。省略時SystemType共通</param>
        /// <returns>新しく採番された値</returns>
        int GetDailySequence(DateTime? targetDate = null, QsApiSystemTypeEnum systemType = QsApiSystemTypeEnum.None, int functionNo = 0);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class DailySequenceRepository: QsDbReaderBase, IDailySequenceRepository
    {
        /// <inheritdoc/>
        public int GetDailySequence(DateTime? targetDate = null,  QsApiSystemTypeEnum systemType = QsApiSystemTypeEnum.None, int functionNo = 0)
        {
            var writerArgs = new DailySequenceGenerateWriterArgs
            {
                SystemType = systemType,
                FunctionNo = functionNo,
                TargetDate = targetDate ?? DateTime.Today
            };

            var result = new DailySequenceGenerateWriter().WriteByAuto(writerArgs);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_DAILYSEQUENCE_MST)}採番処理に失敗しました。");
            }

            return result.Sequence;
        }
    }
}
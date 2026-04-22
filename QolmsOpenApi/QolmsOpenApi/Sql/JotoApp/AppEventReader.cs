using System;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// Joto ネイティブアプリのアプリイベント一覧を取得します。
    /// </summary>
    internal sealed class AppEventReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, AppEventReaderArgs, AppEventReaderResults>
    {
        /// <summary>
        /// 分散トランザクションを使用してデータベーステーブルから値を取得します。
        /// </summary>
        public AppEventReaderResults ExecuteByDistributed(AppEventReaderArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            AppEventReaderResults result = new AppEventReaderResults() { IsSuccess = false };

            try
            {
                DbAppEventReaderCore reader = new DbAppEventReaderCore();
                // Core側で公開期間・アプリ種別・ページングを加味して一覧を取得する。
                result.AppEventItemN = reader.ReadAppEventList(args.PageIndex, args.PageSize, args.AccountKey);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
            }

            return result;
        }
    }
}

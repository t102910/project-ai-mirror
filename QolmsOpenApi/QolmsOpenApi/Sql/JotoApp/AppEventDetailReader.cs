using System;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// Joto ネイティブアプリのアプリイベント詳細を取得します。
    /// </summary>
    internal sealed class AppEventDetailReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, AppEventDetailReaderArgs, AppEventDetailReaderResults>
    {
        /// <summary>
        /// 分散トランザクションを使用してデータベーステーブルから値を取得します。
        /// </summary>
        public AppEventDetailReaderResults ExecuteByDistributed(AppEventDetailReaderArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            AppEventDetailReaderResults result = new AppEventDetailReaderResults() { IsSuccess = false };
            if (args.EventKey == Guid.Empty)
            {
                // キー未指定時はDBアクセスせず失敗結果を返す。
                return result;
            }

            try
            {
                DbAppEventReaderCore reader = new DbAppEventReaderCore();
                // 指定キーのイベント詳細を1件取得する。
                result.AppEventItem = reader.ReadAppEventEntity(args.EventKey);
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

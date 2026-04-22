using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// アプリイベント情報
    /// </summary>
    public interface IAppEventRepository
    {
        /// <summary>
        /// アプリイベント一覧を取得します。
        /// </summary>
        /// <param name="pageIndex">ページ番号</param>
        /// <param name="pageSize">ページサイズ</param>
        /// <param name="accountKey">対象者アカウントキー</param>
        List<DbAppEventItem> ReadAppEventList(int pageIndex, int pageSize, Guid accountKey);

        /// <summary>
        /// イベントキーからアプリイベント詳細を取得します。
        /// </summary>
        DbAppEventItem ReadAppEventDetail(Guid eventKey);

        /// <summary>
        /// イベントキーとファイルキーからアプリイベントファイル情報を取得します。
        /// </summary>
        QH_APPEVENTFILE_DAT ReadAppEventFile(Guid eventKey, Guid fileKey);
    }

    /// <summary>
    /// アプリイベント情報
    /// </summary>
    public class AppEventRepository : IAppEventRepository
    {
        /// <summary>
        /// アプリイベント一覧を取得します。
        /// </summary>
        /// <param name="pageIndex">ページ番号</param>
        /// <param name="pageSize">ページサイズ</param>
        /// <param name="accountKey">対象者アカウントキー</param>
        public List<DbAppEventItem> ReadAppEventList(int pageIndex, int pageSize, Guid accountKey)
        {
            // Reader層にページング条件をそのまま引き渡す。
            AppEventReaderResults result = QsDbManager.Read(new AppEventReader(), new AppEventReaderArgs()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                AccountKey = accountKey
            });
            if (result != null && result.IsSuccess && result.AppEventItemN != null)
            {
                return result.AppEventItemN;
            }

            // 取得失敗時は呼び出し側のnull判定を避けるため空リストを返す。
            return new List<DbAppEventItem>();
        }

        /// <summary>
        /// イベントキーからアプリイベント詳細を取得します。
        /// </summary>
        public DbAppEventItem ReadAppEventDetail(Guid eventKey)
        {
            // 詳細取得はイベントキーをキーにReaderへ委譲する。
            AppEventDetailReaderResults result = QsDbManager.Read(new AppEventDetailReader(), new AppEventDetailReaderArgs() { EventKey = eventKey });
            if (result != null && result.IsSuccess)
            {
                return result.AppEventItem;
            }

            return null;
        }

        /// <summary>
        /// イベントキーとファイルキーからアプリイベントファイル情報を取得します。
        /// </summary>
        public QH_APPEVENTFILE_DAT ReadAppEventFile(Guid eventKey, Guid fileKey)
        {
            // 画像参照時は EventKey と FileKey の両方一致を必須にする。
            AppEventFileReaderResults result = QsDbManager.Read(
                new AppEventFileReader(),
                new AppEventFileReaderArgs() { EventKey = eventKey, FileKey = fileKey });

            if (result != null && result.IsSuccess && result.Result != null && result.Result.Count > 0)
            {
                return result.Result[0];
            }

            return null;
        }
    }
}

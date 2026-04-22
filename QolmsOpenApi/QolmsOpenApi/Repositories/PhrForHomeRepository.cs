using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// JOTOホームPHR取得の入出力インターフェース
    /// </summary>
    internal interface IPhrForHomeRepository
    {
        /// <summary>
        /// ホーム画面用PHR情報を取得します
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="targetDate">対象日</param>
        /// <returns>Reader結果</returns>
        PhrForHomeReaderResults ReadForHome(Guid accountKey, DateTime targetDate);
    }

    /// <summary>
    /// JOTOホームPHR取得の実装
    /// </summary>
    internal class PhrForHomeRepository : IPhrForHomeRepository
    {
        /// <summary>
        /// ホーム画面用PHR情報を取得します
        /// </summary>
        /// <param name="accountKey">アカウントキー</param>
        /// <param name="targetDate">対象日</param>
        /// <returns>Reader結果</returns>
        public PhrForHomeReaderResults ReadForHome(Guid accountKey, DateTime targetDate)
        {
            var args = new PhrForHomeReaderArgs()
            {
                AccountKey = accountKey,
                TargetDate = targetDate,
            };

            return QsDbManager.Read(new PhrForHomeReader(), args);
        }
    }
}

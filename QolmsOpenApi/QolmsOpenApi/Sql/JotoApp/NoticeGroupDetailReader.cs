using System;
using System.Collections.Generic;
using System.Linq;

using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// Joto ネイティブアプリのお知らせ（グループ）詳細情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class NoticeGroupDetailReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, NoticeGroupDetailReaderArgs, NoticeGroupDetailReaderResults>
    {
        #region "Private Property"
        #endregion

        #region "Public Property"
        #endregion

        #region "Constructor"
        /// <summary>
        /// <see cref="NoticeGroupDetailReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoticeGroupDetailReader() : base()
        {
        }
        #endregion

        #region "Private Method"
        #endregion

        #region "Public Method"
        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public NoticeGroupDetailReaderResults ExecuteByDistributed(NoticeGroupDetailReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            NoticeGroupDetailReaderResults result = new NoticeGroupDetailReaderResults() { IsSuccess = false };

            Guid accountKey = args.AccountKey;
            long noticeNo = args.NoticeNo;
            var facilityKeyN = args.FacilityKeyN;

            if (accountKey == Guid.Empty || noticeNo == long.MinValue)
            {
                return result;
            }

            try
            {
                // DbNoticeGroupReaderCoreを呼び出してお知らせ詳細情報を取得
                DbNoticeGroupReaderCore noticeGroupReader = new DbNoticeGroupReaderCore(accountKey, facilityKeyN);
                DbNoticeGroupItem noticeGroupItem = noticeGroupReader.ReadNoticeGroupEntity(noticeNo);

                result.NoticeGroupItem = noticeGroupItem;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex.Message, Guid.Empty);
                return result;
            }

            return result;
        }
        #endregion
    }
}

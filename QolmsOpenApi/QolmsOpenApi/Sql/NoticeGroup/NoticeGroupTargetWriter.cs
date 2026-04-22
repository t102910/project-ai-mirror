using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ReproPush通知登録情報 を、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class NoticeGroupTargetWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, NoticeGroupTargetWriterArgs, NoticeGroupTargetWriterResults>
    {

        /// <summary>
        /// <see cref="NoticeGroupTargetWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoticeGroupTargetWriter() : base()
        {
        }

        private int WriteEntities(List<QH_NOTICEGROUPTARGET_DAT> entities)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MgfConnection"].ConnectionString;
            string useEncryptedConnectionStrings = ConfigurationManager.AppSettings["UseEncryptedConnectionStrings"];

            if (useEncryptedConnectionStrings.TryToValueType(false))
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    connectionString = crypt.DecryptString(connectionString);
                }
            }

            var noticeGroupTargets = entities;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var bulk = new SqlBulkCopy(conn))
                using (var reader = new ListDataReader<QH_NOTICEGROUPTARGET_DAT>(
                    noticeGroupTargets,
                    nameof(QH_NOTICEGROUPTARGET_DAT.NOTICENO),
                    nameof(QH_NOTICEGROUPTARGET_DAT.ACCOUNTKEY),
                    nameof(QH_NOTICEGROUPTARGET_DAT.ALREADYREADFLAG),
                    nameof(QH_NOTICEGROUPTARGET_DAT.ALREADYREADDATE),
                    nameof(QH_NOTICEGROUPTARGET_DAT.NOTICEDATASET),
                    nameof(QH_NOTICEGROUPTARGET_DAT.DELETEFLAG),
                    nameof(QH_NOTICEGROUPTARGET_DAT.CREATEDDATE),
                    nameof(QH_NOTICEGROUPTARGET_DAT.UPDATEDDATE)))
                {
                    bulk.DestinationTableName = nameof(QH_NOTICEGROUPTARGET_DAT);
                    bulk.BatchSize = 5000;

                    bulk.WriteToServer(reader);

                    return noticeGroupTargets.Count;
                }
            }
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public NoticeGroupTargetWriterResults ExecuteByDistributed(NoticeGroupTargetWriterArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            NoticeGroupTargetWriterResults result = new NoticeGroupTargetWriterResults() { IsSuccess = false };
            var count = WriteEntities(args.Entities);

            if (count > 0)
            {
                result.Result = count;
                // 成功
                result.IsSuccess = true;
            }

            return result;
        }
    }
}
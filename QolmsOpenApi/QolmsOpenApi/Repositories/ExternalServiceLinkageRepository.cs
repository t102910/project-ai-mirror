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
    /// 外部サービス連携情報の入出力インターフェース
    /// </summary>
    public interface IExternalServiceLinkageRepository
    {
        /// <summary>
        /// 外部サービス連携情報を書き込む
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        (bool isSuccess, List<string> errorMessages) WriteList(List<QH_EXTERNALSERVICELINKAGE_DAT> entities);

        /// <summary>
        /// 外部サービス連携情報を1件取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="externalServiceType"></param>
        /// <param name="targetServiceType"></param>
        /// <returns></returns>
        QH_EXTERNALSERVICELINKAGE_DAT ReadEntity(Guid accountKey, int externalServiceType, int targetServiceType);

        /// <summary>
        /// 外部サービス連携情報を追加または更新する
        /// </summary>
        /// <param name="entity"></param>
        void UpsertEntity(QH_EXTERNALSERVICELINKAGE_DAT entity);

        /// <summary>
        /// 外部サービス SPHR連携対象者情報 を取得する。
        /// </summary>
        /// <param name="linkageSystemNo">対象施設連携システム番号</param>
        /// <param name="communitySystemNo">連携対象連携システム番号</param>
        /// <returns></returns>
        List<QH_LINKAGE_DAT> SphrLinkageRead(Guid accountKeyReference, int linkageSystemNo, int communitySystemNo);

        /// <summary>
        /// 外部サービス 連携システム情報 を登録する。
        /// </summary>
        /// <param name="entity">登録対象レコード</param>
        /// <returns></returns>
        bool SphrLinkageWrite(QH_LINKAGE_DAT entity);
    }

    /// <summary>
    /// 外部サービス連携情報入出力実装
    /// </summary>
    public class ExternalServiceLinkageRepository : QsDbReaderBase, IExternalServiceLinkageRepository
    {
        /// <summary>
        /// 外部サービス連携情報を書き込む
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public (bool isSuccess, List<string> errorMessages) WriteList(List<QH_EXTERNALSERVICELINKAGE_DAT> entities)
        {
            var errorMessages = new List<string>();
            QoAccessLog.WriteInfoLog($"検証ログ:外部サービス書き込み開始");
            var isSuccess = QolmsLibraryV1.QolmsExternalServiceWorker.Write(entities, ref errorMessages);

            return (isSuccess, errorMessages);
        }
                
        /// <inheritdoc/>
        public QH_EXTERNALSERVICELINKAGE_DAT ReadEntity(Guid accountKey, int externalServiceType, int targetServiceType)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_APPSETTINGS_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", externalServiceType),
                    CreateParameter(con, "@p3", targetServiceType)
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_EXTERNALSERVICELINKAGE_DAT)}
                    WHERE {nameof(QH_EXTERNALSERVICELINKAGE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_EXTERNALSERVICELINKAGE_DAT.EXTERNALSERVICETYPE)} = @p2
                    AND   {nameof(QH_EXTERNALSERVICELINKAGE_DAT.TARGETSERVICETYPE)} = @p3
                    AND   {nameof(QH_EXTERNALSERVICELINKAGE_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_EXTERNALSERVICELINKAGE_DAT>(con, null, sql, paramList);
                var entity = result.FirstOrDefault();               

                return entity;
            }
        }

        /// <inheritdoc/>
        public void UpsertEntity(QH_EXTERNALSERVICELINKAGE_DAT entity)
        {
            var args = new ExternalServiceLinkageWriterArgs { Entity = entity };
            var results = new ExternalServiceLinkageWriter().WriteByAuto(args);

            if (!results.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_EXTERNALSERVICELINKAGE_DAT)}の更新に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public List<QH_LINKAGE_DAT> SphrLinkageRead(Guid accountKeyReference, int linkageSystemNo, int communitySystemNo)
        {
            var reader = new ExternalServiceSphrLinkageReader();
            var readerArgs = new ExternalServiceSphrLinkageReaderArgs() {AccountKey = accountKeyReference, LinkageSystemNo = linkageSystemNo, CommunitySystemNo = communitySystemNo };

            //読込
            var readerResults = QsDbManager.Read(reader, readerArgs);

            if (!readerResults.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_LINKAGE_DAT)}の取得に失敗しました。");
            }

            return readerResults.Result;
        }
        /// <inheritdoc/>
        public bool SphrLinkageWrite(QH_LINKAGE_DAT entity)
        {
            var args = new ExternalServiceSphrLinkageWriterArgs { Entity = entity };
            var results = new ExternalServiceSphrLinkageWriter().WriteByAuto(args);

            return results.IsSuccess;
            
        }
    }
}
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
    /// 連携システムデータの入出力インターフェース
    /// </summary>
    public interface ILinkageRepository
    {
        /// <summary>
        /// 施設キーと連携システムIDからQH_LINKAGE_DATのレコード更新日時を取得する
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="linkageSystemId"></param>
        /// <returns></returns>
        DateTime GetLinkageUpdated(Guid facilityKey, string linkageSystemId);

        /// <summary>
        /// 施設キーとカードNoから利用可能なカード番号かどうか確認する
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="cardNo"></param>
        /// <returns>重複無しでTrue, 重複していればFalse</returns>
        bool IsAvailableCard(Guid facilityKey, string cardNo);

        /// <summary>
        /// 診察券の書き込みを行う
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="actorkey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="facilityKey"></param>
        /// <param name="patientId"></param>
        /// <param name="Sequence"></param>
        /// <param name="delteFlag"></param>
        /// <returns></returns>
        (string errors, QH_PATIENTCARD_DAT entity) WriteLinkagePatientCard(Guid authorKey, Guid actorkey, int linkageSystemNo, Guid facilityKey, string patientId, int Sequence, bool delteFlag);

        /// <summary>
        /// FacilityKey から、LinkageSystemマスタ使って LinkageSystemNoを取得する
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        int GetLinkageNo(Guid facilityKey);

        /// <summary>
        /// 連携システムIDと施設キーからアカウントキーを取得する
        /// </summary>
        /// <param name="linkageSystemId"></param>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        Guid GetAccountKey(string linkageSystemId, Guid facilityKey);

        /// <summary>
        /// FacilityKeyから、施設マスタ・連携マスタを使って親のLinkageSystemMstを取得する
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns>存在した場合は該当マスタレコード、存在しなかった場合はnullが返る</returns>
        QH_LINKAGESYSTEM_MST GetParentLinkageMst(Guid facilityKey);

        /// <summary>
        /// 指定された施設の子施設の連携データリストを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="parentFacility"></param>
        /// <returns></returns>
        List<QH_LINKAGE_DAT> ReadChildLinkageList(Guid accountKey, Guid parentFacility);

        /// <summary>
        /// 親連携システム番号と子連携システム番号と連携IDリストから
        /// Push通知用のUserIdを含むVIEW取得する
        /// </summary>
        /// <param name="rootLinkageSystemNo">親連携システム番号(アプリと紐づいているもの)</param>
        /// <param name="linkageSystemNo">子連携システム番号(対象施設を示す連携番号)</param>
        /// <param name="linkageSystemIdList">対象連携IDリスト</param>
        /// <returns></returns>
        List<QH_LINKAGE_PUSHNOTIFICATION_VIEW> ReadPushNotificationUserView(int rootLinkageSystemNo, int linkageSystemNo, List<string> linkageSystemIdList);

        /// <summary>
        /// QH_LINKAGE_DATにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        void InsertEntity(QH_LINKAGE_DAT entity);
        /// <summary>
        /// QH_LINKAGE_DATのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEntity(QH_LINKAGE_DAT entity);
        /// <summary>
        /// QH_LINKAGE_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity(QH_LINKAGE_DAT entity);
        /// <summary>
        /// QH_LINKAGE_DATのレコードを1件取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        QH_LINKAGE_DAT ReadEntity(Guid accountKey, int linkageSystemNo);

        /// <summary>
        /// 新規連携ユーザー登録処理中のエラーによってアカウントを削除します。
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="authorKey"></param>
        /// <param name="actorKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="unsubscribeItemNo"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        bool WithdrawAccountOnRegister(Guid executor, Guid authorKey, Guid actorKey, int linkageSystemNo, byte unsubscribeItemNo, string comment);

        /// <summary>
        /// 連携システム番号と連携システムIDのリストからアカウントキーのリストを取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="linkageSystemIds"></param>
        /// <returns></returns>
        List<Guid> ReadAccountKeys(int linkageSystemNo,List<string> linkageSystemIds);

        /// <summary>
        /// アカウントキーから、StatusType=2、DeleteFlag=0 の条件で LinkageSystemNo リストを取得します。
        /// </summary>
        /// <param name="accountKey">対象者アカウントキー</param>
        /// <returns>LinkageSystemNo のリスト</returns>
        List<int> ReadLinkageSystemNoListByAccountKey(Guid accountKey);

        /// <summary>
        /// 連携システム番号から連携システムマスタを取得します。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        QH_LINKAGESYSTEM_MST GetFacilitykey(int linkageSystemNo);

    }

    /// <summary>
    /// 連携システムデータの入出力実装
    /// </summary>
    public class LinkageRepository: QsDbReaderBase, ILinkageRepository
    {
        /// <summary>
        /// 施設キーと連携システムIDからQH_LINKAGE_DATのレコード更新日時を取得する
        /// レコードが存在しない場合はDateTime.MinValueを返す
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="linkageSystemId"></param>
        /// <returns></returns>
        public DateTime GetLinkageUpdated(Guid facilityKey, string linkageSystemId)
        {
            var args = new LinkageUpdatedReaderArgs() 
            { 
                FacilityKey = facilityKey, 
                LinkageSystemId = linkageSystemId
            };
            var readerResults = QsDbManager.Read(new LinkageUpdatedReader(), args);

            if (!readerResults.IsSuccess)
            {
                // レコード無し
                return DateTime.MinValue;
            }

            return readerResults.UpdatedDate;
        }

        /// <summary>
        /// 施設キーとカードNoから利用可能なカード番号かどうか確認する
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="cardNo"></param>
        /// <returns>重複無しでTrue, 重複していればFalse</returns>
        public bool IsAvailableCard(Guid facilityKey, string cardNo)
        {
            var linkageSystemNo = GetLinkageNo(facilityKey);
            if(linkageSystemNo < 0)
            {
                throw new Exception("施設キーから連携システム番号への変換に失敗しました。");
            }

            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", linkageSystemNo),
                    CreateParameter(con,"@p2", cardNo)
                };

                var sql = $@"
                    Select count(*)
                    From {nameof(QH_LINKAGE_DAT)}
                    Where {nameof(QH_LINKAGE_DAT.LINKAGESYSTEMNO)} = @p1
                    And {nameof(QH_LINKAGE_DAT.LINKAGESYSTEMID)} = @p2
                    And {nameof(QH_LINKAGE_DAT.DELETEFLAG)} = 0;
                ";

                con.Open();

               return ExecuteScalar<int>(con, null, sql, parameters) == 0;
            }
        }

        /// <summary>
        /// 診察券の書き込みを行う
        /// </summary>
        /// <param name="authorKey"></param>
        /// <param name="actorkey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="facilityKey"></param>
        /// <param name="patientId"></param>
        /// <param name="Sequence"></param>
        /// <param name="delteFlag"></param>
        /// <returns></returns>
        public (string errors, QH_PATIENTCARD_DAT entity) WriteLinkagePatientCard(Guid authorKey, Guid actorkey, int linkageSystemNo, Guid facilityKey, string patientId, int Sequence, bool delteFlag)
        {
            var writerArgs = new LinkagePatientCardWriterArgs()
            {
                AuthorKey = authorKey,
                ActorKey = actorkey,
                CardCode = linkageSystemNo,
                Sequence = Sequence,
                FacilityKey = facilityKey,
                PatientCardSet = new QsJsonSerializer().Serialize(new QhPatientCardSetOfJson() { CardNo = patientId, FacilityKey = facilityKey.ToString() }),
                DeleteFlag = delteFlag,
                StatusType = 2
            };
            var writerResults = new LinkagePatientCardWriter().WriteByAuto(writerArgs);
            if (writerResults != null && !string.IsNullOrWhiteSpace(writerResults.ErrorMessage))
                return (writerResults.ErrorMessage,null);
            else
                return (string.Empty, writerResults.Entity);
        }

        /// <summary>
        /// FacilityKey から、LinkageSystemマスタ使って LinkageSystemNoを取得する
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        public int GetLinkageNo(Guid facilityKey)
        {
            //FacilityKey から、LinkageSystemマスタ使って LinkageSystemNoを取得する
            var readerArgs = new LinkageSystemMasterReaderArgs() { FacilityKey = facilityKey };
            var readerResults = QsDbManager.Read(new LinkageSystemMasterReader(), readerArgs);
            if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count > 0)
                return readerResults.Result.First().LINKAGESYSTEMNO;
            else
                QoAccessLog.WriteInfoLog(string.Format("GetLinkageNo Error :facilitykey={0}", facilityKey));
            return int.MinValue;
        }

        /// <summary>
        /// 連携システムIDと施設キーからアカウントキーを取得する
        /// </summary>
        /// <param name="linkageSystemId"></param>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        public Guid GetAccountKey(string linkageSystemId, Guid facilityKey)
        {
            var linkageSystemNo = GetLinkageNo(facilityKey);
            var readerArgs = new LinkageUserReaderArgs() { LinkageSystemId = linkageSystemId, LinkageSystemNo = linkageSystemNo };
            var accountKey = QsDbManager.Read(new LinkageUserReader(), readerArgs).AccountKey;

            return accountKey;
        }

        /// <summary>
        /// FacilityKeyから、施設マスタ・連携マスタを使って親のLinkageSystemMstを取得する
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns>存在した場合は該当マスタレコード、存在しなかった場合はnullが返る</returns>
        public QH_LINKAGESYSTEM_MST GetParentLinkageMst(Guid facilityKey)
        {
            var emptyFacility = Guid.Empty;

            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", facilityKey),
                    CreateParameter(con,"@p2", emptyFacility)
                };

                var sql = $@"
                    Select * 
                    From {nameof(QH_LINKAGESYSTEM_MST)}
                    Where {nameof(QH_LINKAGESYSTEM_MST.FACILITYKEY)} = (
                        Select {nameof(QH_FACILITY_MST.PARENTKEY)}
                        From {nameof(QH_FACILITY_MST)}
                        Where {nameof(QH_FACILITY_MST.FACILITYKEY)} = @p1
                        AND {nameof(QH_FACILITY_MST.PARENTKEY)} != @p2
                        AND {nameof(QH_FACILITY_MST.DELETEFLAG)} = 0
                    )
                    AND {nameof(QH_LINKAGESYSTEM_MST.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_LINKAGESYSTEM_MST>(con, null, sql, parameters);

                return result.FirstOrDefault();                
            }
        }

        /// <summary>
        /// 指定された施設の子施設の連携データリストを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="parentFacility"></param>
        /// <returns></returns>
        public List<QH_LINKAGE_DAT> ReadChildLinkageList(Guid accountKey, Guid parentFacility)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", parentFacility)
                };

                var sql = $@"
                    SELECT t1.*
                    FROM QH_LINKAGE_DAT t1
                    INNER JOIN QH_LINKAGESYSTEM_MST t2
                    ON t1.LINKAGESYSTEMNO = t2.LINKAGESYSTEMNO
                    AND t2.DELETEFLAG = 0
                    INNER JOIN QH_FACILITY_MST t3
                    On t3.FACILITYKEY = t2.FACILITYKEY
                    AND t3.PARENTKEY = @p2
                    AND t3.DELETEFLAG = 0
                    WHERE 
                    t1.ACCOUNTKEY = @p1
                    AND t1.DELETEFLAG = 0
                ";

                con.Open();

                return ExecuteReader<QH_LINKAGE_DAT>(con, null, sql, parameters);             
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootLinkageSystemNo"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="linkageSystemIdList"></param>
        /// <returns></returns>
        public List<QH_LINKAGE_PUSHNOTIFICATION_VIEW> ReadPushNotificationUserView(int rootLinkageSystemNo, int linkageSystemNo, List<string> linkageSystemIdList)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", rootLinkageSystemNo),
                    CreateParameter(con,"@p2", linkageSystemNo),
                    CreateParameter(con,"@p3", string.Join(",",linkageSystemIdList))
                };
                
                // システムを識別するための親LinkageSystemNoと子のLinkageSystemNoと
                // LinkageSystemIDから対応するPush通知用UserIdを取得する
                // t2はアカウントとそれに対する親アカウントのVIEW
                // 親アカウントの親は自身を指すようにしている
                // 子アカウントが存在しない場合はQH_ACCOUNTRELATION_DATにレコードが存在しないので
                // その場合は結合条件ORでQH_LINKAGE_DAT同士を結合させるようにしている
                // IN句はSTRING_SPLIT関数で安全にSELECTで取得するように
                // パフォーマンスを考慮してサブクエリ内ではUNION ALLしてメインでDISTINCTしている
                var sql = $@"
                SELECT DISTINCT t1.LINKAGESYSTEMID, t1.ACCOUNTKEY, t3.LINKAGESYSTEMID As USERID
                FROM QH_LINKAGE_DAT t1
                LEFT JOIN
                (
                    SELECT ACCOUNTKEY, RELATIONACCOUNTKEY
                    FROM QH_ACCOUNTRELATION_DAT
                    WHERE RELATIONDIRECTIONTYPE = 2
                    AND RELATIONTYPE = 1
                    AND DELETEFLAG = 0

                    UNION ALL

                    SELECT ACCOUNTKEY, ACCOUNTKEY as RELATIONACCOUNTKEY
                    FROM QH_ACCOUNTRELATION_DAT
                    WHERE RELATIONDIRECTIONTYPE = 1
                    AND RELATIONTYPE = 1
                    AND DELETEFLAG = 0
                ) t2
                ON t1.ACCOUNTKEY = t2.ACCOUNTKEY
                INNER JOIN QH_LINKAGE_DAT t3
                ON (t2.RELATIONACCOUNTKEY = t3.ACCOUNTKEY OR t1.ACCOUNTKEY = t3.ACCOUNTKEY)
                AND t3.LINKAGESYSTEMNO = @p1
                AND t3.DELETEFLAG = 0
                WHERE t1.LINKAGESYSTEMNO = @p2
                AND t1.LINKAGESYSTEMID IN (SELECT value FROM STRING_SPLIT(@p3,','))
                AND t1.STATUSTYPE = 2
                AND t1.DELETEFLAG = 0 ";

                con.Open();

                return ExecuteReader<QH_LINKAGE_PUSHNOTIFICATION_VIEW>(con, null, sql, parameters);
            }
        }

        /// <summary>
        /// QH_LINKAGE_DATのレコードを1件取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        public QH_LINKAGE_DAT ReadEntity(Guid accountKey, int linkageSystemNo)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", linkageSystemNo)
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_LINKAGE_DAT)}
                    WHERE {nameof(QH_LINKAGE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_LINKAGE_DAT.LINKAGESYSTEMNO)} = @p2
                    AND   {nameof(QH_LINKAGE_DAT.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_LINKAGE_DAT>(con, null, sql, paramList);
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// QH_LINKAGE_DATにレコードを挿入します。
        /// </summary>
        /// <param name="entity"></param>
        public void InsertEntity(QH_LINKAGE_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new LinkageWriterArgs { Entity = entity };
            var result = new LinkageWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_LINKAGE_DAT)}の挿入に失敗しました。");
            }
        }

        /// <summary>
        /// QH_LINKAGE_DATのレコードを更新します。
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEntity(QH_LINKAGE_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new LinkageWriterArgs { Entity = entity };
            var result = new LinkageWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_LINKAGE_DAT)}の更新に失敗しました。");
            }
        }

        /// <summary>
        /// QH_LINKAGE_DATのレコードを削除します。
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEntity(QH_LINKAGE_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new LinkageWriterArgs { Entity = entity };
            var result = new LinkageWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_LINKAGE_DAT)}の削除に失敗しました。");
            }
        }

        /// <summary>
        /// 新規連携ユーザー登録処理中のエラーによってアカウントを削除します。
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="authorKey"></param>
        /// <param name="actorKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <param name="unsubscribeItemNo"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool WithdrawAccountOnRegister(Guid executor, Guid authorKey, Guid actorKey, int linkageSystemNo, byte unsubscribeItemNo, string comment)
        {
            try
            {
                if(AccountWorker.ProcessWithdraw(authorKey, actorKey, linkageSystemNo, unsubscribeItemNo, comment))
                {
                    return true;
                }

                throw new Exception();
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, "登録トランザクション中止による退会処理に失敗しました。", executor);
                return false;
            }
        }

        /// <summary>
        /// アカウントキーから、StatusType=2、DeleteFlag=0 の条件で LinkageSystemNo リストを取得します。
        /// </summary>
        /// <param name="accountKey">対象者アカウントキー</param>
        /// <returns>LinkageSystemNo のリスト</returns>
        public List<int> ReadLinkageSystemNoListByAccountKey(Guid accountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", 2),  // StatusType = 2
                    CreateParameter(con, "@p3", 0)   // DeleteFlag = 0
                };

                var sql = $@"
                    SELECT *
                    FROM {nameof(QH_LINKAGE_DAT)}
                    WHERE {nameof(QH_LINKAGE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_LINKAGE_DAT.STATUSTYPE)} = @p2
                    AND   {nameof(QH_LINKAGE_DAT.DELETEFLAG)} = @p3
                    ORDER BY {nameof(QH_LINKAGE_DAT.LINKAGESYSTEMNO)}
                ";

                con.Open();

                var results = ExecuteReader<QH_LINKAGE_DAT>(con, null, sql, parameters);
                return results.Select(r => r.LINKAGESYSTEMNO).Distinct().ToList();
            }
        }

        /// <summary>
        /// 連携システム番号と連携システムIDのリストからアカウントキーのリストを取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="linkageSystemIds"></param>
        /// <returns></returns>
        public List<Guid> ReadAccountKeys(int linkageSystemNo, List<string> linkageSystemIds)
        {
                
            var strWhere = string.Empty;
            
            foreach (var id in linkageSystemIds) strWhere = string.IsNullOrWhiteSpace(strWhere) ? $"'{id}'" : $"{strWhere}, '{id}'";
            QoAccessLog.WriteInfoLog($"strWhere {strWhere}");

            var reader = new LinkageUserCheckReader();

            try
            {
                var readerArgs = new LinkageUserCheckReaderArgs() { LinkageSystemNo = linkageSystemNo, LinkageIds = strWhere };

                //読込
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.AccountKeys.Count > 0)
                {
                    //結果格納
                    return readerResults.AccountKeys;
                }
                else
                {
                    QoAccessLog.WriteInfoLog(readerResults.AccountKeys.Count.ToString());
                    QoAccessLog.WriteErrorLog(string.Format($"[ReadAccountKeys]QH_LINKAGE_DAT情報の取得に失敗しました。"), Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return new List<Guid>();
        }

        /// <summary>
        /// linkageSystemNokara、施設マスタ・連携マスタを使って親のLinkageSystemMstを取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns>存在した場合は該当マスタレコード、存在しなかった場合はnullが返る</returns>
        public QH_LINKAGESYSTEM_MST GetFacilitykey(int linkageSystemNo)
        {
            var emptyFacility = Guid.Empty;

            using (var con = QsDbManager.CreateDbConnection<QH_LINKAGESYSTEM_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", linkageSystemNo)
                };

                var sql = $@"
                    Select * 
                    From {nameof(QH_LINKAGESYSTEM_MST)}
                    Where {nameof(QH_LINKAGESYSTEM_MST.LINKAGESYSTEMNO)} = @p1 
                    AND {nameof(QH_LINKAGESYSTEM_MST.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_LINKAGESYSTEM_MST>(con, null, sql, parameters);

                return result.FirstOrDefault();
            }
        }
    }
}
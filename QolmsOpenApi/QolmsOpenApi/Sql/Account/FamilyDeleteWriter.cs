using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Transactions;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 家族削除処理Writer 
    /// IdentityApiのConnectFamilyWriterから一部移植
    /// IdentityApiの削除処理では関連テーブルから完全に削除しきれないため
    /// DbLibraryのUnsubscribeの処理と組み合わせて完全に削除するようにする
    /// </summary>
    public class FamilyDeleteWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, FamilyDeleteWriterArgs, FamilyDeleteWriterResults>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public FamilyDeleteWriterResults ExecuteByDistributed(FamilyDeleteWriterArgs args)
        {
            var result = new FamilyDeleteWriterResults { IsSuccess = false };
            try
            {
                var actionDate = DateTime.Now;
                var accountRelationWriter = new DbAccountRelationWriterCore(args.AuthorKey, args.AccountKey);              
                
                var accountWriter = new DbAccountRelationWriter();
                var accountWriterArgs = new DbAccountRelationWriterArgs
                {
                    ActionDate = actionDate,
                    ActorKey = args.ParentAccountKey,
                    AuthorKey = args.ParentAccountKey,
                    RelationAccountKey = args.AccountKey,
                    RelationDirectionType = QsDbAccountRelationDirectionTypeEnum.To,
                    RelationType = QsDbAccountRelationTypeEnum.Family,
                    WriteModeType = DbAccountRelationWriterCore.AccountRelationWriteModeTypeEnum.Delete
                };

                // 親のリレーションを更新
                var accountResult = QsDbManager.WriteByCurrent(accountWriter, accountWriterArgs);

                if (!accountResult.IsSuccess)
                {
                    throw new InvalidOperationException("QH_ACCOUNTRELATION_DATテーブルへの登録に失敗しました。");
                }

                // QH_ACCOUNTRELATION_DATを取得
                var relationEntity = ReadAccountRelation(args.AccountKey);

                // QH_ACCOUNTRELATIONCONTROL_DATから該当RelationItemKeyのデータを削除
                DeleteAccountRelationControl(relationEntity.RELATIONITEMKEY, actionDate);

                // QH_ACCOUNTRELATION_DATの削除
                DeleteAccountRelation(args.AccountKey, actionDate);
                
                var unsubscribeWriter = new DbUnsubscribeWriterCore(args.AuthorKey, args.AccountKey);
                // 子アカウントの残りの関連テーブルのレコードに削除フラグ(DbLibrary)
                // 子アカウントは連携システム番号は関係ないはずなので引数は0を指定
                if (!unsubscribeWriter.DeleteTableData(actionDate, args.AccountKey, 0))
                {
                    throw new InvalidOperationException("家族アカウントの関連テーブルの削除に失敗しました。");
                }

                result.IsSuccess = true;                

                return result;
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return result;
            }
        }

        QH_ACCOUNTRELATION_DAT ReadAccountRelation(Guid accountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNTRELATION_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1",accountKey)
                };

                var sql = @"
                    SELECT * 
                    FROM QH_ACCOUNTRELATION_DAT
                    WHERE ACCOUNTKEY = @p1
                    AND DELETEFLAG = 0
                ";

                con.Open();

                var entityList = ExecuteReader<QH_ACCOUNTRELATION_DAT>(con, null, sql, parameters);

                return entityList.FirstOrDefault();
            }
        }

        void DeleteAccountRelation(Guid accountKey, DateTime actionDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNTRELATION_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", actionDate)
                };

                var sql = @"
                    UPDATE QH_ACCOUNTRELATION_DAT
                    SET
                    DELETEFLAG = 1,
                    UPDATEDDATE = @p2
                    WHERE ACCOUNTKEY = @p1
                ";

                con.Open();

                ExecuteNonQuery(con, null, sql, parameters);
            }
        }

        void DeleteAccountRelationControl(Guid itemKey, DateTime actionDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNTRELATIONCONTROL_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", itemKey),
                    CreateParameter(con, "@p2", actionDate)
                };

                var sql = @"
                    UPDATE QH_ACCOUNTRELATIONCONTROL_DAT
                    SET
                    DELETEFLAG = 1,
                    UPDATEDDATE = @p2
                    WHERE RELATIONITEMKEY = @p1
                ";

                con.Open();

                ExecuteNonQuery(con, null, sql, parameters);
            }
        }
    }
}
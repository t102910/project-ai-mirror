using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 家族アカウント作成Writer
    /// IdentityApiより移植
    /// </summary>
    public class FamilyCreateWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, FamilyCreateWriterArgs, FamilyCreateWriterResults>
    {
        class RelationMaster
        {
            public long RelationId { get; set; } = long.MinValue;
            public string RelationName { get; set; } = string.Empty;
            public long ParentItemId { get; set; } = long.MinValue;
            public int Hierarchy { get; set; } = int.MinValue;
            public Dictionary<long, RelationMaster> RelationItemN { get; set; } = new Dictionary<long, RelationMaster>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public FamilyCreateWriterResults ExecuteByDistributed(FamilyCreateWriterArgs args)
        {
            var results = new FamilyCreateWriterResults
            {
                IsSuccess = false,
                Result = 0
            };

            var accountKey = Guid.NewGuid();
            var now = DateTime.Now;

            if(InsertAccountMst(accountKey, now) &&
                InsertAccountIndexDat(accountKey, now, args) &&
                InsertLoginmanagementDat(accountKey, now) &&
                InsertPasswordmanagementDat(accountKey, now) &&
                InsertTwoFactorAuthenticationDat(accountKey, now) &&
                InsertRelationDat(accountKey, args.ParentAccountKey, now, out var relationItemkeyN) &&
                InsertRelationControlDat(accountKey, args.ParentAccountKey, now, relationItemkeyN))
            {
                results.Result = 1;
            }

            if(results.Result == 1)
            {
                results.IsSuccess = true;
                results.AccountKey = accountKey;
            }
            else
            {
                throw new InvalidOperationException("家族アカウントの作成に失敗しました。");
            }

            return results;
        }

        // アカウントマスタ登録
        bool InsertAccountMst(Guid accountKey, DateTime now)
        {
            try
            {
                var entity = new QH_ACCOUNT_MST
                {
                    ACCOUNTKEY = accountKey,
                    ACCOUNTTYPE = 1,
                    PRIVATEACCOUNTFLAG = true,
                    TESTACCOUNTFLAG = false,
                    DELETEFLAG = false,
                    CREATEDDATE = now,
                    UPDATEDDATE = now
                };

                using(var con = QsDbManager.CreateDbConnection<QH_ACCOUNT_MST>())
                {
                    var parameters = new List<DbParameter>
                    {
                        CreateParameter(con, "@p1", entity.ACCOUNTKEY),
                        CreateParameter(con, "@p2", entity.ACCOUNTTYPE),
                        CreateParameter(con, "@p3", entity.PRIVATEACCOUNTFLAG),
                        CreateParameter(con, "@p4", entity.TESTACCOUNTFLAG),
                        CreateParameter(con, "@p5", entity.DELETEFLAG),
                        CreateParameter(con, "@p6", entity.CREATEDDATE),
                        CreateParameter(con, "@p7", entity.UPDATEDDATE),
                    };

                    var query = new StringBuilder();
                    query.Append("insert into QH_ACCOUNT_MST");
                    query.Append("(accountkey,accounttype,privateaccountflag,testaccountflag,deleteflag,createddate,updateddate)");
                    query.Append(" values (");
                    query.Append(" @p1,");
                    query.Append(" @p2,");
                    query.Append(" @p3,");
                    query.Append(" @p4,");
                    query.Append(" @p5,");
                    query.Append(" @p6,");
                    query.Append(" @p7");
                    query.Append(" )");
                    query.Append(";");

                    con.Open();

                    return ExecuteNonQuery(con, null, CreateCommandText(con, query.ToString()), parameters) == 1;
                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    accountKey,
                    DateTime.Now, 
                    QoAccessLog.AccessTypeEnum.Error, 
                    string.Empty, 
                    $"家族アカウント作成時エラー:例外: {ex.InnerException.Message} AccountKey: {accountKey}"
                );

                return false;
            }
        }

        // アカウントインデックス登録
        bool InsertAccountIndexDat(Guid accountKey, DateTime now, FamilyCreateWriterArgs args)
        {
            try 
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNTINDEX_DAT>())
                {
                    // 暗号化
                    var familyName = crypt.TryEncrypt(args.FamilyName);
                    var givenName = crypt.TryEncrypt(args.GivenName);
                    var familyKana = crypt.TryEncrypt(args.FamilyNameKana);
                    var givenKana = crypt.TryEncrypt(args.GivenNameKana);
                    var nickName = crypt.TryEncrypt(args.NickName);

                    var parameter = new List<DbParameter>
                    {
                        CreateParameter(con, "@p1", accountKey),
                        CreateParameter(con, "@p2", familyName),
                        CreateParameter(con, "@p3", string.Empty),
                        CreateParameter(con, "@p4", givenName),
                        CreateParameter(con, "@p5", familyKana),
                        CreateParameter(con, "@p6", string.Empty),
                        CreateParameter(con, "@p7", givenKana),
                        CreateParameter(con, "@p8", string.Empty),
                        CreateParameter(con, "@p9", string.Empty),
                        CreateParameter(con, "@p10", string.Empty),
                        CreateParameter(con, "@p11", nickName),
                        CreateParameter(con, "@p12", args.Sex),
                        CreateParameter(con, "@p13", args.BirthDate),
                        CreateParameter(con, "@p14", args.PhotoKey),
                        CreateParameter(con, "@p15", 1),
                        CreateParameter(con, "@p16", now),
                        CreateParameter(con, "@p17", 0),
                        CreateParameter(con, "@p18", now),
                        CreateParameter(con, "@p19", now),
                    };

                    con.Open();

                    var query = new StringBuilder();
                    query.Append("insert into qh_accountindex_dat");
                    query.Append(" (accountkey, familyname, middlename, givenname, familykananame, ");
                    query.Append("middlekananame,givenkananame,familyromanname,middleromanname,");
                    query.Append("givenromanname,nickname,sextype,birthday,photokey,acceptflag,acceptdate,");
                    query.Append("deleteflag,createddate,updateddate)");
                    query.Append(" values (");
                    query.Append(" @p1,");
                    query.Append(" @p2,");
                    query.Append(" @p3,");
                    query.Append(" @p4,");
                    query.Append(" @p5,");
                    query.Append(" @p6,");
                    query.Append(" @p7,");
                    query.Append(" @p8,");
                    query.Append(" @p9,");
                    query.Append(" @p10,");
                    query.Append(" @p11,");
                    query.Append(" @p12,");
                    query.Append(" @p13,");
                    query.Append(" @p14,");
                    query.Append(" @p15,");
                    query.Append(" @p16,");
                    query.Append(" @p17,");
                    query.Append(" @p18,");
                    query.Append(" @p19");
                    query.Append(" )");
                    query.Append(";");

                    return ExecuteNonQuery(con, null, CreateCommandText(con, query.ToString()), parameter) == 1;
                }

            }
            catch(Exception ex)
            {
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    accountKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"家族アカウント作成時エラー(ACCOUNTINDEX:例外: {ex.InnerException.Message} AccountKey: {accountKey}"
                );

                return false;
            }
        }

        // LOGINMANAGEMENT新規登録
        bool InsertLoginmanagementDat(Guid accountKey, DateTime now)
        {
            try
            {
                using(var con = QsDbManager.CreateDbConnection<QH_LOGINMANAGEMENT_DAT>())
                {
                    var parameters = new List<DbParameter>
                    {
                        CreateParameter(con, "@p1", accountKey),
                        CreateParameter(con, "@p2", 0),
                        CreateParameter(con, "@p3", 0),
                        CreateParameter(con, "@p4", 0),
                        CreateParameter(con, "@p5", DateTime.MinValue),
                        CreateParameter(con, "@p6", 0),
                        CreateParameter(con, "@p7", now),
                        CreateParameter(con, "@p8", now),
                    };

                    var query = new StringBuilder();

                    query.Append("insert into QH_LOGINMANAGEMENT_DAT");
                    query.Append("(accountkey,logincount,retrycount,lockcount,lastlogindate,");
                    query.Append("deleteflag,createddate,updateddate)");
                    query.Append(" values (");
                    query.Append(" @p1,");
                    query.Append(" @p2,");
                    query.Append(" @p3,");
                    query.Append(" @p4,");
                    query.Append(" @p5,");
                    query.Append(" @p6,");
                    query.Append(" @p7,");
                    query.Append(" @p8");
                    query.Append(" )");
                    query.Append(";");

                    con.Open();

                    return ExecuteNonQuery(con, null, CreateCommandText(con, query.ToString()), parameters) == 1;
                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    accountKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"家族アカウント作成時エラー(LOGINMANAGEMENT):例外: {ex.InnerException.Message} AccountKey: {accountKey}"
                );

                return false;
            }
        }

        // PASSWORDMANAGEMENT新規登録
        bool InsertPasswordmanagementDat(Guid accountKey, DateTime now)
        {
            try
            {
                using(var con = QsDbManager.CreateDbConnection<QH_PASSWORDMANAGEMENT_DAT>())
                {
                    var parameters = new List<DbParameter>
                    {
                        CreateParameter(con, "@p1", accountKey),
                        CreateParameter(con, "@p2", string.Empty),
                        CreateParameter(con, "@p3", string.Empty),
                        CreateParameter(con, "@p4", string.Empty),
                        CreateParameter(con, "@p5", string.Empty),
                        CreateParameter(con, "@p6", now),
                        CreateParameter(con, "@p7", 0),
                        CreateParameter(con, "@p8", now),
                        CreateParameter(con, "@p9", now),
                    };

                    var query = new StringBuilder();
                    query.Append("insert into QH_PASSWORDMANAGEMENT_DAT");
                    query.Append("(accountkey,userid,userpassword,passwordrecoveryset,");
                    query.Append(" passwordrecoverymailaddress,lastupdatepassworddate,");
                    query.Append(" deleteflag,createddate,updateddate)");
                    query.Append(" values (");
                    query.Append(" @p1,");
                    query.Append(" @p2,");
                    query.Append(" @p3,");
                    query.Append(" @p4,");
                    query.Append(" @p5,");
                    query.Append(" @p6,");
                    query.Append(" @p7,");
                    query.Append(" @p8,");
                    query.Append(" @p9");
                    query.Append(" )");
                    query.Append(";");

                    con.Open();

                    return ExecuteNonQuery(con, null, CreateCommandText(con, query.ToString()), parameters) == 1;
                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    accountKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"家族アカウント作成時エラー(PASSWORDMANAGEMENT):例外: {ex.InnerException.Message} AccountKey: {accountKey}"
                );

                return false;
            }
        }

        //  TWOFACTORAUTHENTICATION新規登録
        bool InsertTwoFactorAuthenticationDat(Guid accountKey, DateTime now)
        {
            try
            {
                using(var con = QsDbManager.CreateDbConnection<QH_TWOFACTORAUTHENTICATION_DAT>())
                {
                    var parameters = new List<DbParameter>
                    {
                        CreateParameter(con, "@p1", accountKey),
                        CreateParameter(con, "@p2", 0),
                        CreateParameter(con, "@p3", string.Empty),
                        CreateParameter(con, "@p4", string.Empty),
                        CreateParameter(con, "@p5", DateTime.MinValue),
                        CreateParameter(con, "@p6", DateTime.MinValue),
                        CreateParameter(con, "@p7", 0),
                        CreateParameter(con, "@p8", now),
                        CreateParameter(con, "@p9", now),
                    };

                    var query = new StringBuilder();
                    query.Append("insert into QH_TWOFACTORAUTHENTICATION_DAT");
                    query.Append("(accountkey,useauthenticationflag,authenticationset,authenticationtoken,");
                    query.Append(" authenticationexpires,lastauthenticationdate,");
                    query.Append(" deleteflag,createddate,updateddate)");
                    query.Append(" values (");
                    query.Append(" @p1,");
                    query.Append(" @p2,");
                    query.Append(" @p3,");
                    query.Append(" @p4,");
                    query.Append(" @p5,");
                    query.Append(" @p6,");
                    query.Append(" @p7,");
                    query.Append(" @p8,");
                    query.Append(" @p9");
                    query.Append(" )");
                    query.Append(";");

                    con.Open();

                    return ExecuteNonQuery(con, null, CreateCommandText(con, query.ToString()), parameters) == 1;
                }
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    accountKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"家族アカウント作成時エラー(TWOFACTORAUTHENTICATION):例外: {ex.InnerException.Message} AccountKey: {accountKey}"
                );

                return false;
            }
        }

        // アカウントリレーション新規登録
        bool InsertRelationDat(Guid accountKey, Guid parentAccountKey, DateTime now, out List<KeyValuePair<Guid, Guid>> relationItemkeyN)
        {
            relationItemkeyN = null;
            var result = false;

            // 申請
            var writer = new DbAccountRelationWriter();
            var writerArgs = new DbAccountRelationWriterArgs
            {
                AuthorKey = parentAccountKey,
                ActorKey = parentAccountKey,
                ActionDate = now,
                RelationAccountKey = accountKey,
                RelationDirectionType = QsDbAccountRelationDirectionTypeEnum.To,
                RelationType = QsDbAccountRelationTypeEnum.Family,
                WriteModeType = DbAccountRelationWriterCore.AccountRelationWriteModeTypeEnum.Request
            };

            var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess)
            {
                // 承認
                var updateWriterArgs = new DbAccountRelationWriterArgs
                {
                    AuthorKey = parentAccountKey,
                    ActorKey = parentAccountKey,
                    ActionDate = now,
                    RelationAccountKey = accountKey,
                    RelationDirectionType = QsDbAccountRelationDirectionTypeEnum.To,
                    RelationType = QsDbAccountRelationTypeEnum.Family,
                    WriteModeType = DbAccountRelationWriterCore.AccountRelationWriteModeTypeEnum.Approval
                };

                var updateWriterResults = QsDbManager.WriteByCurrent(writer, updateWriterArgs);
                relationItemkeyN = updateWriterResults.RelationItemKeyN;
                result = updateWriterResults.IsSuccess;
            }

            if (!result)
            {
                QoAccessLog.WriteAccessLog(
                    null,
                    QsApiSystemTypeEnum.QolmsOpenApi,
                    accountKey,
                    DateTime.Now,
                    QoAccessLog.AccessTypeEnum.Error,
                    string.Empty,
                    $"家族アカウント作成時エラー(QH_ACCOUNTRELATION_DAT): AccountKey: {accountKey}"
                );
            }

            return result;
        }

        bool InsertRelationControlDat(Guid accountKey, Guid parentAccountKey, DateTime now, List<KeyValuePair<Guid, Guid>> relationItemkeyN)
        {
            // リレーションマスタを取得
            var reader = new DbAccountRelationItemMasterReader();
            var readerArgs = new DbAccountRelationItemMasterReaderArgs
            {
                AuthorKey = parentAccountKey,
                ActorKey = parentAccountKey,
            };
            var readerResults = QsDbManager.Read(reader, readerArgs);

            if(!readerResults.IsSuccess || !readerResults.ItemN.Any())
            {
                return false;
            }

            var relationMaster = new Dictionary<long, RelationMaster>();

            // 3階層目までみる
            for(var count = 1; count <= 3; count++)
            {
                var hierarchy = count;
                foreach(var item in readerResults.ItemN.Where(x => x.Value.Item3 == hierarchy))
                {
                    var master = new RelationMaster();
                    master.RelationId = item.Key;
                    master.RelationName = item.Value.Item1;
                    master.ParentItemId = item.Value.Item2;
                    master.Hierarchy = item.Value.Item3;

                    // 1階層目
                    if (hierarchy == 1)
                    {
                        relationMaster.Add(item.Key, master);
                    }
                    // 2階層目
                    else if(hierarchy == 2)
                    {
                        if (relationMaster.ContainsKey(item.Value.Item2))
                        {
                            relationMaster[item.Value.Item2].RelationItemN.Add(item.Key, master);
                        }
                    }
                    // 3階層目
                    else
                    {
                        foreach(var parentItem in relationMaster)
                        {
                            if (parentItem.Value.RelationItemN.ContainsKey(item.Value.Item2))
                            {
                                parentItem.Value.RelationItemN[item.Value.Item2].RelationItemN.Add(item.Key, master);
                            }
                        }
                    }
                }
            }

            var relationIdList = new List<long>();
            foreach(var item1 in relationMaster)
            {
                if (!item1.Value.RelationItemN.Any())
                {
                    relationIdList.Add(item1.Key);
                }
                else
                {
                    foreach(var item2 in item1.Value.RelationItemN)
                    {
                        if (!item2.Value.RelationItemN.Any())
                        {
                            relationIdList.Add(item2.Key);
                        }
                        else
                        {
                            foreach(var item3 in item2.Value.RelationItemN)
                            {
                                if (!item3.Value.RelationItemN.Any())
                                {
                                    relationIdList.Add(item3.Key);
                                }
                            }
                        }
                    }
                }
            }

            // RelationIdList分の数だけtrue追加
            var relationFlags = new List<bool>();
            foreach(var _ in relationIdList)
            {
                relationFlags.Add(true);
            }

            foreach(var entity in relationItemkeyN)
            {
                var writer = new DbAccountRelationControlWriter();
                var writerArgs = new DbAccountRelationControlWriterArgs
                {
                    ActorKey = parentAccountKey,
                    AuthorKey = parentAccountKey,
                    ActionDate = now,
                    RelationItemId = relationIdList,
                    RelationItemKey = entity.Value,
                    RelationShowFlag = relationFlags,
                    RelationEditFlag = relationFlags
                };

                var writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

                if (!writerResults.IsSuccess)
                {
                    QoAccessLog.WriteAccessLog(
                        null,
                        QsApiSystemTypeEnum.QolmsOpenApi,
                        accountKey,
                        DateTime.Now,
                        QoAccessLog.AccessTypeEnum.Error,
                        string.Empty,
                        $"家族アカウント作成時エラー(QH_ACCOUNTRELATIONCONTROL_DAT): AccountKey: {entity.Key}"
                    );

                    return false;
                }
            }

            return true;
        }

        
    }
}
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    public class CalomealMealSyncRepository : QsDbReaderBase
    {
        public const int CalomealLinkageSystemNo = 47015;

        public CalomealTokenData ReadTokenData(Guid accountKey, int linkageSystemNo)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_CALOMEALTOKENMANAGEMENT_DAT>())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = new StringBuilder()
                        .Append("select top(1) accountkey, token, tokenexpires, refreshtoken, refreshtokenexpires")
                        .Append(" from qh_calomealtokenmanagement_dat")
                        .Append(" where accountkey = @p1")
                        .Append(" and linkagesystemno = @p2")
                        .Append(" and token <> ''")
                        .Append(" and tokenexpires > '0001-01-01'")
                        .Append(" and refreshtoken <> ''")
                        .Append(" and deleteflag = 0")
                        .Append(" order by updateddate desc;")
                        .ToString();
                    command.Parameters.Add(CreateParameter(connection, "@p1", accountKey));
                    command.Parameters.Add(CreateParameter(connection, "@p2", linkageSystemNo));

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new CalomealTokenData()
                        {
                            AccountKey = reader.GetGuid(0),
                            Token = reader.GetString(1),
                            TokenExpires = reader.GetDateTime(2),
                            RefreshToken = reader.GetString(3),
                            RefreshTokenExpires = reader.GetDateTime(4)
                        };
                    }
                }
            }
        }

        public void UpdateToken(Guid accountKey, int linkageSystemNo, string token, DateTime tokenExpires)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_CALOMEALTOKENMANAGEMENT_DAT>())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = new StringBuilder()
                        .Append("update qh_calomealtokenmanagement_dat")
                        .Append(" set token = @p1, tokenexpires = @p2, updateddate = @p3")
                        .Append(" where accountkey = @p4")
                        .Append(" and linkagesystemno = @p5")
                        .Append(" and deleteflag = 0;")
                        .ToString();
                    command.Parameters.Add(CreateParameter(connection, "@p1", token));
                    command.Parameters.Add(CreateParameter(connection, "@p2", tokenExpires));
                    command.Parameters.Add(CreateParameter(connection, "@p3", DateTime.Now));
                    command.Parameters.Add(CreateParameter(connection, "@p4", accountKey));
                    command.Parameters.Add(CreateParameter(connection, "@p5", linkageSystemNo));

                    if (command.ExecuteNonQuery() != 1)
                    {
                        throw new InvalidOperationException("QH_CALOMEALTOKENMANAGEMENT_DAT の更新に失敗しました。");
                    }
                }
            }
        }

        public CalomealMealSyncActionType ApplyMealHistory(Guid accountKey, CalomealMealSyncItem item, DateTime actionDate, Guid actionKey)
        {
            if (accountKey == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(accountKey));
            if (item == null) throw new ArgumentNullException(nameof(item));

            var current = ReadMealByHistoryId(accountKey, item.HistoryId);
            if (current == null && item.DeleteFlag)
            {
                return CalomealMealSyncActionType.None;
            }

            var targetRecordDate = new DateTime(item.RecordDate.Year, item.RecordDate.Month, item.RecordDate.Day, item.RecordDate.Hour, item.RecordDate.Minute, 0, 0);
            var actionType = CalomealMealSyncActionType.None;
            var entity = new QH_MEALEVENT2_DAT();

            if (current == null)
            {
                entity = CreateNewMealEvent(accountKey, targetRecordDate, item, actionDate);
                actionType = CalomealMealSyncActionType.Added;
            }
            else if (item.DeleteFlag)
            {
                entity = CloneMealEvent(current);
                entity.DELETEFLAG = true;
                entity.UPDATEDDATE = actionDate;
                actionType = CalomealMealSyncActionType.Deleted;
            }
            else if (current.ACCOUNTKEY == accountKey && current.RECORDDATE == targetRecordDate && current.MEALTYPE == item.MealType)
            {
                entity = CloneMealEvent(current);
                entity.STARTDATE = targetRecordDate;
                entity.ENDDATE = targetRecordDate;
                entity.ITEMNAME = item.ItemName;
                entity.CALORIE = item.Calorie;
                entity.PHOTOKEY = Guid.Empty;
                entity.ANALYSISTYPE = item.AnalysisType;
                entity.ANALYSISSET = item.AnalysisSet;
                entity.CHOOSEPOSITIONSET = string.Empty;
                entity.CHOOSESET = item.ChooseSet;
                entity.RATE = item.Rate;
                entity.DELETEFLAG = false;
                entity.UPDATEDDATE = actionDate;
                actionType = CalomealMealSyncActionType.Modified;
            }
            else
            {
                using (var connection = QsDbManager.CreateDbConnection<QH_MEALEVENT2_DAT>())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var deleteEntity = CloneMealEvent(current);
                        deleteEntity.DELETEFLAG = true;
                        deleteEntity.UPDATEDDATE = actionDate;

                        WriteMealEvent(connection, transaction, deleteEntity);
                        WriteMealEventHist(connection, transaction, CreateLogEntity(current, accountKey, QH_MEALEVENTHIST2_LOG.HistTypeEnum.Deleted, actionDate, actionKey));
                        transaction.Commit();
                    }
                }

                entity = CreateNewMealEvent(accountKey, targetRecordDate, item, actionDate);
                actionType = CalomealMealSyncActionType.Added;
            }

            using (var connection = QsDbManager.CreateDbConnection<QH_MEALEVENT2_DAT>())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    if (actionType == CalomealMealSyncActionType.Deleted)
                    {
                        WriteMealEvent(connection, transaction, entity);
                        WriteMealEventHist(connection, transaction, CreateLogEntity(current, accountKey, QH_MEALEVENTHIST2_LOG.HistTypeEnum.Deleted, actionDate, actionKey));
                        MarkCalomealMappingDeleted(connection, transaction, entity);
                        if (item.HasImage)
                        {
                            MarkCalomealImageDeleted(connection, transaction, entity);
                        }
                    }
                    else
                    {
                        if (entity.SEQUENCE <= 0)
                        {
                            entity.SEQUENCE = GetNextSequence(connection, transaction, accountKey, entity.RECORDDATE, entity.MEALTYPE) + 1;
                        }

                        WriteMealEvent(connection, transaction, entity);
                        var histType = current == null || actionType == CalomealMealSyncActionType.Added
                            ? QH_MEALEVENTHIST2_LOG.HistTypeEnum.Added
                            : QH_MEALEVENTHIST2_LOG.HistTypeEnum.Modified;
                        WriteMealEventHist(connection, transaction, CreateLogEntity(current ?? entity, accountKey, histType, actionDate, actionKey));
                        UpsertCalomealMapping(connection, transaction, entity, item.HistoryId, actionDate);
                        if (item.HasImage)
                        {
                            UpsertCalomealImageImport(connection, transaction, entity, item.HistoryId, actionDate);
                        }
                    }

                    transaction.Commit();
                }
            }

            return actionType;
        }

        private QH_MEALEVENT2_DAT ReadMealByHistoryId(Guid accountKey, int historyId)
        {
            using (var connection = QsDbManager.CreateDbConnection<QH_MEALEVENT2_DAT>())
            {
                var paramList = new List<DbParameter>()
                {
                    CreateParameter(connection, "@p1", accountKey),
                    CreateParameter(connection, "@p2", historyId)
                };

                var sql = new StringBuilder()
                    .Append("select m.* from qh_calomealtomealevent2_dat c")
                    .Append(" inner join qh_mealevent2_dat m")
                    .Append(" on m.accountkey = c.accountkey")
                    .Append(" and m.recorddate = c.recorddate")
                    .Append(" and m.mealtype = c.mealtype")
                    .Append(" and m.sequence = c.sequence")
                    .Append(" where c.accountkey = @p1")
                    .Append(" and c.historyid = @p2")
                    .Append(" order by c.createddate desc;")
                    .ToString();

                connection.Open();
                return ExecuteReader<QH_MEALEVENT2_DAT>(connection, null, sql, paramList).FirstOrDefault();
            }
        }

        private static int GetNextSequence(DbConnection connection, DbTransaction transaction, Guid accountKey, DateTime recordDate, byte mealType)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = new StringBuilder()
                    .Append("select top(1) sequence from qh_mealevent2_dat")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" order by sequence desc;")
                    .ToString();
                command.Parameters.Add(CreateParameter(connection, "@p1", accountKey));
                command.Parameters.Add(CreateParameter(connection, "@p2", recordDate));
                command.Parameters.Add(CreateParameter(connection, "@p3", mealType));
                var value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }

        private static void WriteMealEvent(DbConnection connection, DbTransaction transaction, QH_MEALEVENT2_DAT entity)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = new StringBuilder()
                    .Append("if(select count(accountkey)")
                    .Append(" from qh_mealevent2_dat")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4) = 1")
                    .Append(" begin")
                    .Append(" update qh_mealevent2_dat")
                    .Append(" set startdate = @p5,")
                    .Append(" enddate = @p6,")
                    .Append(" itemname = @p7,")
                    .Append(" calorie = @p8,")
                    .Append(" photokey = @p9,")
                    .Append(" analysistype = @p10,")
                    .Append(" analysisset = @p11,")
                    .Append(" choosepositionset = @p12,")
                    .Append(" chooseset = @p13,")
                    .Append(" rate = @p14,")
                    .Append(" deleteflag = @p15,")
                    .Append(" createddate = @p16,")
                    .Append(" updateddate = @p17")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4")
                    .Append(" end")
                    .Append(" else")
                    .Append(" begin")
                    .Append(" insert into qh_mealevent2_dat")
                    .Append(" values(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17)")
                    .Append(" end;")
                    .ToString();
                command.Parameters.Add(CreateParameter(connection, "@p1", entity.ACCOUNTKEY));
                command.Parameters.Add(CreateParameter(connection, "@p2", entity.RECORDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p3", entity.MEALTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p4", entity.SEQUENCE));
                command.Parameters.Add(CreateParameter(connection, "@p5", entity.STARTDATE));
                command.Parameters.Add(CreateParameter(connection, "@p6", entity.ENDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p7", entity.ITEMNAME));
                command.Parameters.Add(CreateParameter(connection, "@p8", entity.CALORIE));
                command.Parameters.Add(CreateParameter(connection, "@p9", entity.PHOTOKEY));
                command.Parameters.Add(CreateParameter(connection, "@p10", entity.ANALYSISTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p11", entity.ANALYSISSET));
                command.Parameters.Add(CreateParameter(connection, "@p12", entity.CHOOSEPOSITIONSET));
                command.Parameters.Add(CreateParameter(connection, "@p13", entity.CHOOSESET));
                command.Parameters.Add(CreateParameter(connection, "@p14", entity.RATE));
                command.Parameters.Add(CreateParameter(connection, "@p15", entity.DELETEFLAG));
                command.Parameters.Add(CreateParameter(connection, "@p16", entity.CREATEDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p17", entity.UPDATEDDATE));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("QH_MEALEVENT2_DAT の更新に失敗しました。");
                }
            }
        }

        private static void WriteMealEventHist(DbConnection connection, DbTransaction transaction, QH_MEALEVENTHIST2_LOG entity)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = new StringBuilder()
                    .Append("insert into qh_mealeventhist2_log")
                    .Append(" values(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17);")
                    .ToString();
                command.Parameters.Add(CreateParameter(connection, "@p1", entity.ACCOUNTKEY));
                command.Parameters.Add(CreateParameter(connection, "@p2", entity.RECORDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p3", entity.MEALTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p4", entity.SEQUENCE));
                command.Parameters.Add(CreateParameter(connection, "@p5", entity.ACTIONDATE));
                command.Parameters.Add(CreateParameter(connection, "@p6", entity.ACTIONKEY));
                command.Parameters.Add(CreateParameter(connection, "@p7", entity.HISTTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p8", entity.STARTDATE));
                command.Parameters.Add(CreateParameter(connection, "@p9", entity.ENDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p10", entity.ITEMNAME));
                command.Parameters.Add(CreateParameter(connection, "@p11", entity.CALORIE));
                command.Parameters.Add(CreateParameter(connection, "@p12", entity.PHOTOKEY));
                command.Parameters.Add(CreateParameter(connection, "@p13", entity.ANALYSISTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p14", entity.ANALYSISSET));
                command.Parameters.Add(CreateParameter(connection, "@p15", entity.CHOOSEPOSITIONSET));
                command.Parameters.Add(CreateParameter(connection, "@p16", entity.CHOOSESET));
                command.Parameters.Add(CreateParameter(connection, "@p17", entity.RATE));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("QH_MEALEVENTHIST2_LOG の登録に失敗しました。");
                }
            }
        }

        private static void UpsertCalomealMapping(DbConnection connection, DbTransaction transaction, QH_MEALEVENT2_DAT entity, int historyId, DateTime actionDate)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = new StringBuilder()
                    .Append("if(select count(accountkey)")
                    .Append(" from qh_calomealtomealevent2_dat")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4) = 1")
                    .Append(" begin")
                    .Append(" update qh_calomealtomealevent2_dat")
                    .Append(" set historyid = @p5,")
                    .Append(" deleteflag = @p6,")
                    .Append(" createddate = @p7,")
                    .Append(" updateddate = @p8")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4")
                    .Append(" end")
                    .Append(" else")
                    .Append(" begin")
                    .Append(" insert into qh_calomealtomealevent2_dat")
                    .Append(" values(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)")
                    .Append(" end;")
                    .ToString();
                command.Parameters.Add(CreateParameter(connection, "@p1", entity.ACCOUNTKEY));
                command.Parameters.Add(CreateParameter(connection, "@p2", entity.RECORDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p3", entity.MEALTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p4", entity.SEQUENCE));
                command.Parameters.Add(CreateParameter(connection, "@p5", historyId));
                command.Parameters.Add(CreateParameter(connection, "@p6", entity.DELETEFLAG));
                command.Parameters.Add(CreateParameter(connection, "@p7", actionDate));
                command.Parameters.Add(CreateParameter(connection, "@p8", actionDate));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("QH_CALOMEALTOMEALEVENT2_DAT の更新に失敗しました。");
                }
            }
        }

        private static void UpsertCalomealImageImport(DbConnection connection, DbTransaction transaction, QH_MEALEVENT2_DAT entity, int historyId, DateTime actionDate)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = new StringBuilder()
                    .Append("if(select count(accountkey)")
                    .Append(" from qh_calomealimageinport_dat")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4) = 1")
                    .Append(" begin")
                    .Append(" update qh_calomealimageinport_dat")
                    .Append(" set historyid = @p5,")
                    .Append(" statuscode = @p6,")
                    .Append(" response = @p7,")
                    .Append(" message = @p8,")
                    .Append(" retrycount = @p9,")
                    .Append(" deleteflag = @p10,")
                    .Append(" createddate = @p11,")
                    .Append(" updateddate = @p12")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4")
                    .Append(" end")
                    .Append(" else")
                    .Append(" begin")
                    .Append(" insert into qh_calomealimageinport_dat")
                    .Append(" values(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)")
                    .Append(" end;")
                    .ToString();
                command.Parameters.Add(CreateParameter(connection, "@p1", entity.ACCOUNTKEY));
                command.Parameters.Add(CreateParameter(connection, "@p2", entity.RECORDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p3", entity.MEALTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p4", entity.SEQUENCE));
                command.Parameters.Add(CreateParameter(connection, "@p5", historyId));
                command.Parameters.Add(CreateParameter(connection, "@p6", 0));
                command.Parameters.Add(CreateParameter(connection, "@p7", string.Empty));
                command.Parameters.Add(CreateParameter(connection, "@p8", string.Empty));
                command.Parameters.Add(CreateParameter(connection, "@p9", 0));
                command.Parameters.Add(CreateParameter(connection, "@p10", entity.DELETEFLAG));
                command.Parameters.Add(CreateParameter(connection, "@p11", actionDate));
                command.Parameters.Add(CreateParameter(connection, "@p12", actionDate));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("QH_CALOMEALIMAGEINPORT_DAT の更新に失敗しました。");
                }
            }
        }

        private static void MarkCalomealMappingDeleted(DbConnection connection, DbTransaction transaction, QH_MEALEVENT2_DAT entity)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = new StringBuilder()
                    .Append("update qh_calomealtomealevent2_dat")
                    .Append(" set deleteflag = 1, updateddate = @p5")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4;")
                    .ToString();
                command.Parameters.Add(CreateParameter(connection, "@p1", entity.ACCOUNTKEY));
                command.Parameters.Add(CreateParameter(connection, "@p2", entity.RECORDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p3", entity.MEALTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p4", entity.SEQUENCE));
                command.Parameters.Add(CreateParameter(connection, "@p5", entity.UPDATEDDATE));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("QH_CALOMEALTOMEALEVENT2_DAT の削除更新に失敗しました。");
                }
            }
        }

        private static void MarkCalomealImageDeleted(DbConnection connection, DbTransaction transaction, QH_MEALEVENT2_DAT entity)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = new StringBuilder()
                    .Append("update qh_calomealimageinport_dat")
                    .Append(" set deleteflag = 1, updateddate = @p5")
                    .Append(" where accountkey = @p1")
                    .Append(" and recorddate = @p2")
                    .Append(" and mealtype = @p3")
                    .Append(" and sequence = @p4;")
                    .ToString();
                command.Parameters.Add(CreateParameter(connection, "@p1", entity.ACCOUNTKEY));
                command.Parameters.Add(CreateParameter(connection, "@p2", entity.RECORDDATE));
                command.Parameters.Add(CreateParameter(connection, "@p3", entity.MEALTYPE));
                command.Parameters.Add(CreateParameter(connection, "@p4", entity.SEQUENCE));
                command.Parameters.Add(CreateParameter(connection, "@p5", entity.UPDATEDDATE));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("QH_CALOMEALIMAGEINPORT_DAT の削除更新に失敗しました。");
                }
            }
        }

        private static QH_MEALEVENT2_DAT CreateNewMealEvent(Guid accountKey, DateTime recordDate, CalomealMealSyncItem item, DateTime actionDate)
        {
            return new QH_MEALEVENT2_DAT()
            {
                ACCOUNTKEY = accountKey,
                RECORDDATE = recordDate,
                MEALTYPE = item.MealType,
                SEQUENCE = int.MinValue,
                STARTDATE = recordDate,
                ENDDATE = recordDate,
                ITEMNAME = item.ItemName,
                CALORIE = item.Calorie,
                PHOTOKEY = Guid.Empty,
                ANALYSISTYPE = item.AnalysisType,
                ANALYSISSET = item.AnalysisSet,
                CHOOSEPOSITIONSET = string.Empty,
                CHOOSESET = item.ChooseSet,
                RATE = item.Rate,
                DELETEFLAG = item.DeleteFlag,
                CREATEDDATE = actionDate,
                UPDATEDDATE = actionDate,
            };
        }

        private static QH_MEALEVENT2_DAT CloneMealEvent(QH_MEALEVENT2_DAT source)
        {
            return new QH_MEALEVENT2_DAT()
            {
                ACCOUNTKEY = source.ACCOUNTKEY,
                RECORDDATE = source.RECORDDATE,
                MEALTYPE = source.MEALTYPE,
                SEQUENCE = source.SEQUENCE,
                STARTDATE = source.STARTDATE,
                ENDDATE = source.ENDDATE,
                ITEMNAME = source.ITEMNAME,
                CALORIE = source.CALORIE,
                PHOTOKEY = source.PHOTOKEY,
                ANALYSISTYPE = source.ANALYSISTYPE,
                ANALYSISSET = source.ANALYSISSET,
                CHOOSEPOSITIONSET = source.CHOOSEPOSITIONSET,
                CHOOSESET = source.CHOOSESET,
                RATE = source.RATE,
                DELETEFLAG = source.DELETEFLAG,
                CREATEDDATE = source.CREATEDDATE,
                UPDATEDDATE = source.UPDATEDDATE,
            };
        }

        private static QH_MEALEVENTHIST2_LOG CreateLogEntity(
            QH_MEALEVENT2_DAT entity,
            Guid actorKey,
            QH_MEALEVENTHIST2_LOG.HistTypeEnum histType,
            DateTime actionDate,
            Guid actionKey)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.ACCOUNTKEY != actorKey) throw new ArgumentOutOfRangeException(nameof(entity.ACCOUNTKEY));

            var setEntity = CloneMealEvent(entity);
            if ((byte)histType == (byte)QsDbHistTypeEnum.Added)
            {
                setEntity.STARTDATE = DateTime.MinValue;
                setEntity.ENDDATE = DateTime.MinValue;
                setEntity.ITEMNAME = string.Empty;
                setEntity.CALORIE = short.MinValue;
                setEntity.PHOTOKEY = Guid.Empty;
                setEntity.ANALYSISTYPE = byte.MinValue;
                setEntity.ANALYSISSET = string.Empty;
                setEntity.CHOOSEPOSITIONSET = string.Empty;
                setEntity.CHOOSESET = string.Empty;
                setEntity.RATE = decimal.Zero;
            }

            return new QH_MEALEVENTHIST2_LOG()
            {
                ACCOUNTKEY = setEntity.ACCOUNTKEY,
                RECORDDATE = setEntity.RECORDDATE,
                MEALTYPE = setEntity.MEALTYPE,
                SEQUENCE = setEntity.SEQUENCE,
                ACTIONDATE = actionDate,
                ACTIONKEY = actionKey,
                HISTTYPE = (byte)histType,
                STARTDATE = setEntity.STARTDATE,
                ENDDATE = setEntity.ENDDATE,
                ITEMNAME = setEntity.ITEMNAME,
                CALORIE = setEntity.CALORIE,
                PHOTOKEY = setEntity.PHOTOKEY,
                ANALYSISTYPE = setEntity.ANALYSISTYPE,
                ANALYSISSET = setEntity.ANALYSISSET,
                CHOOSEPOSITIONSET = setEntity.CHOOSEPOSITIONSET,
                CHOOSESET = setEntity.CHOOSESET,
                RATE = setEntity.RATE,
            };
        }
    }

    public sealed class CalomealTokenData
    {
        public Guid AccountKey { get; set; } = Guid.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpires { get; set; } = DateTime.MinValue;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpires { get; set; } = DateTime.MinValue;
    }
}
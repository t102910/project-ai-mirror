using System;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.JAHISMedicineEntityV1;
using System.Collections.Generic;
using System.Linq;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System.Text;
using System.Configuration;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System.Data.Common;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// お薬手帳情報入出力インターフェース
    /// </summary>
    public interface INoteMedicineRepository
    {
        /// <summary>
        /// お薬手帳情報のリストを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <param name="refIsModified"></param>
        /// <param name="refLastAccessDate"></param>
        /// <param name="refPageIndex"></param>
        /// <param name="refMaxPageIndex"></param>
        /// <returns></returns>
        List<QH_MEDICINE_DAT> ReadMedicineList(QoNoteMedicineReadApiArgs args,out bool refIsModified,out DateTime refLastAccessDate,out int refPageIndex,out int refMaxPageIndex);

        /// <summary>
        /// お薬手帳情報を主キーで1件取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="recordDate"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        QH_MEDICINE_DAT ReadEntity(Guid accountKey, DateTime recordDate, int seq);

        /// <summary>
        /// お薬手帳用の履歴リストを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        List<QH_MEDICINE_DAT> ReadNoteHistory(Guid accountKey, DateTime startDate, DateTime endDate);

        /// <summary>
        /// お薬手帳用の履歴リストの次の候補を1件取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        QH_MEDICINE_DAT ReadNoteHistoryNext(Guid accountKey, DateTime startDate);

        /// <summary>
        /// お薬手帳用の更新(差分)リストを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        List<QH_MEDICINE_DAT> ReadNoteUpdate(Guid accountKey, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 指定日のお薬手帳情報のリストを取得します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="targetDate"></param>
        /// <param name="dataTypeList"></param>
        /// <param name="facilityKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        List<QH_MEDICINE_DAT> ReadMedicineDayList(Guid accountKey, DateTime targetDate, List<byte> dataTypeList, Guid facilityKey, int linkageSystemNo);

        /// <summary>
        /// お薬手帳用の利用規約を取得します。
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        QoApiNoteMedicineTermsItem ReadTermsItem(byte systemType);


        /// <summary>
        /// お薬手帳情報を登録します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="convertMedSet"></param>
        /// <param name="dataType"></param>
        /// <param name="recordDate"></param>
        /// <param name="linkageSysNo"></param>
        /// <param name="medicineSet"></param>
        /// <param name="ownerType"></param>
        /// <param name="pharmacyNo"></param>
        /// <param name="dataId"></param>
        /// <param name="refErrorMessage"></param>
        /// <returns></returns>
        (bool,bool, int) WriteMedicine(
            Guid accountKey,
            Guid authorKey,
            string convertMedSet,
            byte dataType,
            DateTime recordDate,
            int linkageSysNo,
            string medicineSet,
            int ownerType,
            int pharmacyNo,
            ref string dataId,
            ref string refErrorMessage);        

        /// <summary>
        /// お薬イベントを登録する
        /// </summary>
        /// <param name="args"></param>
        /// <param name="recordDate"></param>
        /// <param name="medSequence"></param>
        /// <returns></returns>
        bool RegisterMedicineEvent(QoNoteMedicineAddApiArgs args, DateTime recordDate, int medSequence);

        /// <summary>
        /// お薬情報を削除する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="dataIdReference"></param>
        /// <returns></returns>
        (bool, List<DbMedicineKeyItem>) DeleteMedicine(Guid accountKey, string dataIdReference);

        /// <summary>
        /// QH_MEDICINE_DATのレコードを論理削除する
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity(QH_MEDICINE_DAT entity);

        /// <summary>
        /// QH_MEDICINE_DATのレコードを物理削除する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="recordDate"></param>
        /// <param name="sequence"></param>
        void PhysicalDeleteEntity(Guid accountKey, DateTime recordDate, int sequence);

        /// <summary>
        /// QH_MEDICINE_DATのレコードを追加する
        /// CONVERTEDMEDICINESET / MEDICINESET は暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void InsertEntity(QH_MEDICINE_DAT entity);

        /// <summary>
        /// QH_MEDICINE_DATのレコードを更新する
        /// CONVERTEDMEDICINESET / MEDICINESET は暗号化されます。
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEntity(QH_MEDICINE_DAT entity);

        /// <summary>
        /// お薬イベントを削除する
        /// </summary>
        /// <param name="args"></param>
        /// <param name="deletedKeys"></param>
        /// <returns></returns>
        bool DeleteMedicineEvent(QoNoteMedicineDeleteApiArgs args, List<DbMedicineKeyItem> deletedKeys);


        /// <summary>
        /// ファイルキー取得
        /// </summary>
        /// <param name="yjCode"></param>
        /// <returns></returns>
        List<Guid> ReadEthicalDrugFileKey(string yjCode);

        /// <summary>
        /// 調剤薬ファイルをYJコードから一括抽出する
        /// </summary>
        /// <param name="yjCodeN"></param>
        /// <returns></returns>
        List<QH_ETHDRUGFILE_MST> ReadEthicalDrugFileListWithYjCode(List<string> yjCodeN);

        /// <summary>
        /// 市販薬ファイルをItemCodeから一括抽出
        /// </summary>
        /// <param name="itemCodeN">ItemCodeとItemCodeTypeを連結した文字列のリスト</param>
        /// <returns></returns>
        List<QH_OTCDRUGFILE_MST> ReadOtcDrugFileListWithItemCode(List<string> itemCodeN);

        /// <summary>
        /// 薬効分類取得
        /// </summary>
        /// <param name="yjCodeN"></param>
        /// <returns></returns>
        List<DbEthicalDrugCategoryItem> ReadYakkoList(List<string> yjCodeN);

    }

    /// <summary>
    /// お薬手帳情報入出力実装
    /// </summary>
    public class NoteMedicineRepository: QsDbReaderBase, INoteMedicineRepository
    {
        private const int MEDICINE_EVENT_CATEGORYNO = 4;

        /// <summary>
        /// お薬手帳情報のリストを取得する
        /// </summary>
        /// <param name="args"></param>
        /// <param name="refIsModified"></param>
        /// <param name="refLastAccessDate"></param>
        /// <param name="refPageIndex"></param>
        /// <param name="refMaxPageIndex"></param>
        /// <returns></returns>
        public List<QH_MEDICINE_DAT> ReadMedicineList(QoNoteMedicineReadApiArgs args, out bool refIsModified,out DateTime refLastAccessDate, out int refPageIndex,out int refMaxPageIndex)
        {
            List<QH_MEDICINE_DAT> result = null;
            refIsModified = false;
            refPageIndex = 0;
            refMaxPageIndex = 0;
            refLastAccessDate = DateTime.MinValue;

            var reader = new NoteMedicineReader();
            var readerArgs = new NoteMedicineReaderArgs()
            {
                AccountKey = args.ActorKey.ToValueType<Guid>(),
                DataType = args.DataType.ToValueType<byte>(),
                LastAccessDate = args.LastAccessDate.TryToValueType<DateTime>(DateTime.MinValue),
                PageIndex = args.PageIndex.ToValueType<int>(),
                PageSize = args.DaysPerPage.ToValueType<int>()
            };

            NoteMedicineReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults != null)
            {
                if (readerResults.IsSuccess)
                {
                    result = new List<QH_MEDICINE_DAT>();
                    if (readerResults.Result != null && readerResults.Result.Any())
                    {
                        result = readerResults.Result;
                    }
                    refIsModified = readerResults.IsModified;
                    refLastAccessDate = readerResults.LastAccessDate;
                    refPageIndex = readerResults.PageIndex;
                    refMaxPageIndex = readerResults.MaxPageIndex;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public QH_MEDICINE_DAT ReadEntity(Guid accountKey, DateTime recordDate, int seq)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", recordDate.Date),
                    CreateParameter(con,"@p3", seq),

                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_MEDICINE_DAT)}
                    WHERE {nameof(QH_MEDICINE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_MEDICINE_DAT.RECORDDATE)} = @p2
                    AND   {nameof(QH_MEDICINE_DAT.SEQUENCE)} = @p3
                    AND   {nameof(QH_MEDICINE_DAT.DELETEFLAG)} = 0                   
                ";

                con.Open();

                var result = ExecuteReader<QH_MEDICINE_DAT>(con, null, sql, paramList);

                return result.FirstOrDefault();
            }
        }

        /// <inheritdoc/>
        public List<QH_MEDICINE_DAT> ReadNoteHistory(Guid accountKey, DateTime startDate, DateTime endDate)
        {            
            using (var con = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", endDate.Date),
                    CreateParameter(con,"@p3", startDate.Date),

                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_MEDICINE_DAT)}
                    WHERE {nameof(QH_MEDICINE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_MEDICINE_DAT.RECORDDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_MEDICINE_DAT.DATATYPE)} IN (1,2,100)
                    AND   {nameof(QH_MEDICINE_DAT.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_MEDICINE_DAT.RECORDDATE)} DESC, {nameof(QH_MEDICINE_DAT.SEQUENCE)} ASC
                ";

                con.Open();

                var result = ExecuteReader<QH_MEDICINE_DAT>(con, null, sql, paramList);

                return result;
            }
        }

        /// <inheritdoc/>
        public QH_MEDICINE_DAT ReadNoteHistoryNext(Guid accountKey, DateTime startDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", startDate.Date),
                };

                var sql = $@"
                    SELECT TOP(1) *
                    FROM  {nameof(QH_MEDICINE_DAT)}
                    WHERE {nameof(QH_MEDICINE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_MEDICINE_DAT.RECORDDATE)} <= @p2
                    AND   {nameof(QH_MEDICINE_DAT.DATATYPE)} IN (1,2,100)
                    AND   {nameof(QH_MEDICINE_DAT.DELETEFLAG)} = 0
                    ORDER BY {nameof(QH_MEDICINE_DAT.RECORDDATE)} DESC, {nameof(QH_MEDICINE_DAT.SEQUENCE)} ASC
                ";

                con.Open();

                var result = ExecuteReader<QH_MEDICINE_DAT>(con, null, sql, paramList);

                return result.FirstOrDefault();
            }
        }

        /// <inheritdoc/>
        public List<QH_MEDICINE_DAT> ReadNoteUpdate(Guid accountKey, DateTime startDate, DateTime endDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", accountKey),
                    CreateParameter(con,"@p2", startDate),
                    CreateParameter(con,"@p3", endDate),

                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_MEDICINE_DAT)}
                    WHERE {nameof(QH_MEDICINE_DAT.ACCOUNTKEY)} = @p1
                    AND   {nameof(QH_MEDICINE_DAT.UPDATEDDATE)} BETWEEN @p2 AND @p3
                    AND   {nameof(QH_MEDICINE_DAT.DATATYPE)} IN (1,2,100)
                    ORDER BY {nameof(QH_MEDICINE_DAT.RECORDDATE)} DESC, {nameof(QH_MEDICINE_DAT.SEQUENCE)} ASC
                ";

                con.Open();

                var result = ExecuteReader<QH_MEDICINE_DAT>(con, null, sql, paramList);

                return result;
            }
        }

        /// <summary>
        /// 指定日のお薬手帳情報を取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="targetDate"></param>
        /// <param name="dataTypeList"></param>
        /// <param name="facilityKey"></param>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        public List<QH_MEDICINE_DAT> ReadMedicineDayList(Guid accountKey, DateTime targetDate, List<byte> dataTypeList, Guid facilityKey, int linkageSystemNo)
        {
            var reader = new DbNoteMedicineReaderCore(accountKey);

            return reader.ReadMedicineDayList(targetDate.Date, dataTypeList, facilityKey, linkageSystemNo);
        }

        /// <summary>
        /// お薬手帳用の利用規約を取得します。
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public QoApiNoteMedicineTermsItem ReadTermsItem(byte systemType)
        {
            var result = new QoApiNoteMedicineTermsItem();

            var reader = new NoteMedicineTermsReader();
            var readerArgs = new NoteMedicineTermsReaderArgs() { SystemType = systemType };
            NoteMedicineTermsReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults != null && readerResults.IsSuccess)
            {
                result.TermsNo = readerResults.TermsItem.TermsNo.ToString();
                result.UpdatedDate = readerResults.TermsItem.TermsUpdatedDate.ToApiDateString();
                result.Contents = readerResults.TermsItem.Contents;
            }

            return result;
        }


        /// <summary>
        /// お薬手帳情報を登録します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="authorKey"></param>
        /// <param name="convertMedSet"></param>
        /// <param name="dataType"></param>
        /// <param name="recordDate"></param>
        /// <param name="linkageSysNo"></param>
        /// <param name="medicineSet"></param>
        /// <param name="ownerType"></param>
        /// <param name="pharmacyNo"></param>
        /// <param name="dataId"></param>
        /// <param name="refErrorMessage"></param>
        /// <returns></returns>
        public (bool,bool,int) WriteMedicine(
            Guid accountKey,
            Guid authorKey,
            string convertMedSet, 
            byte dataType,
            DateTime recordDate,
            int linkageSysNo,
            string medicineSet, 
            int ownerType, 
            int pharmacyNo,
            ref string dataId,
            ref string refErrorMessage
            )
        {
            var wResult = false;
            var result = false;
            string tmpDataId = dataId;
            var sequence = int.MinValue;

            try
            {
                var writer = new NoteMedicineWriter();
                var writerArgs = new NoteMedicineWriterArgs()
                {
                    ActorKey = accountKey,
                    AuthorKey = authorKey,
                    CommentSet = string.Empty,
                    ConvertedMedicineSet = convertMedSet,
                    DataType = dataType,
                    DeleteFlag = false,
                    EndDate = recordDate,
                    FacilityKey = Guid.Empty,
                    LinkageSystemNo = linkageSysNo,
                    MedicineSet = medicineSet,
                    OriginalFileName = string.Empty,
                    OwnerType = ownerType,
                    PharmacyNo = pharmacyNo,
                    PrescriptionDate = DateTime.MinValue,
                    ReceiptNo = tmpDataId,
                    RecordDate = recordDate,
                    Sequence = int.MinValue,
                    StartDate = recordDate
                };
                NoteMedicineWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

                if (writerResults != null)
                {
                    wResult = true;
                    dataId = writerResults.DataId;                    
                    sequence = writerResults.Sequence;
                    result = writerResults.IsSuccess && writerResults.Result == 1;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
            return (wResult,result, sequence);
        }       

        /// <inheritdoc/>
        public void InsertEntity(QH_MEDICINE_DAT entity)
        {
            entity.CONVERTEDMEDICINESET = entity.CONVERTEDMEDICINESET.TryEncrypt();
            entity.MEDICINESET = entity.MEDICINESET.TryDecrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Added;

            var args = new MedicineWriterArgs { Entity = entity };
            var result = new MedicineWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_MEDICINE_DAT)}の挿入に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void UpdateEntity(QH_MEDICINE_DAT entity)
        {
            entity.CONVERTEDMEDICINESET = entity.CONVERTEDMEDICINESET.TryEncrypt();
            entity.MEDICINESET = entity.MEDICINESET.TryEncrypt();

            entity.DataState = QsDbEntityStateTypeEnum.Modified;

            var args = new MedicineWriterArgs { Entity = entity };
            var result = new MedicineWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_MEDICINE_DAT)}の更新に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void DeleteEntity(QH_MEDICINE_DAT entity)
        {
            entity.DataState = QsDbEntityStateTypeEnum.Deleted;
            entity.DELETEFLAG = true;

            var args = new MedicineWriterArgs { Entity = entity };
            var result = new MedicineWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_MEDICINE_DAT)}の削除に失敗しました。");
            }
        }

        /// <inheritdoc/>
        public void PhysicalDeleteEntity(Guid accountKey, DateTime recordDate, int sequence)
        {
            var entity = new QH_MEDICINE_DAT
            {
                ACCOUNTKEY = accountKey,
                RECORDDATE = recordDate,
                SEQUENCE = sequence
            };

            entity.DataState = QsDbEntityStateTypeEnum.Deleted;

            var args = new MedicineWriterArgs
            {
                Entity = entity,
                IsPhysicalDelete = true
            };
            var result = new MedicineWriter().WriteByAuto(args);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"{nameof(QH_MEDICINE_DAT)}の物理削除に失敗しました。");
            }
        }

        /// <summary>
        /// お薬手帳情報を削除します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="dataIdReference"></param>
        /// <returns></returns>
        public (bool, List<DbMedicineKeyItem>) DeleteMedicine(Guid accountKey, string dataIdReference)
        {
            var result = false;
            List<DbMedicineKeyItem> deletedKeys = null;

            if (string.IsNullOrWhiteSpace(dataIdReference)) throw new ArgumentNullException("データID参照文字列が不正です。");

            try
            {
                var deleter = new NoteMedicineDeleter();
                var deleterArgs = new NoteMedicineDeleterArgs() { ActorKey = accountKey, AuthorKey = accountKey, DataId = dataIdReference.ToDecrypedReference() };
                NoteMedicineDeleterResults deleterResults = QsDbManager.Write(deleter, deleterArgs);

                if (deleterResults != null)
                {
                    if (deleterResults.IsSuccess && deleterResults.Result > 0)
                    {
                        // 成功
                        result = true;
                        deletedKeys = deleterResults.DeletedKeys;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            
            return (result, deletedKeys);
        }

        /// <summary>
        /// お薬イベントを登録する
        /// </summary>
        /// <param name="args"></param>
        /// <param name="recordDate"></param>
        /// <param name="medSequence"></param>
        /// <returns></returns>
        public bool RegisterMedicineEvent(QoNoteMedicineAddApiArgs args, DateTime recordDate, int medSequence)
        {
            var result = false;

            var apiArgs = new QoCalendarEventWriteApiArgs()
            {
                ActorKey = args.ActorKey,
                ApiType = QoApiTypeEnum.CalendarEventWrite.ToString(),
                Event = new QoApiCalendarEventItem(),
                ExecuteSystemType = args.ExecuteSystemType,
                Executor = args.Executor,
                ExecutorName = args.ExecutorName
            };

            var item = new QoApiCalendarEventItem();

            item.LinkageSystemNo = args.LinkageSystemNo;
            item.Name = "お薬";
            item.EventDate = recordDate.ToApiDateString();
            item.StartDate = item.EventDate;
            item.EndDate = item.EventDate;
            item.CategoryNoN = new List<string>() { MEDICINE_EVENT_CATEGORYNO.ToString() };
            item.CustomTagNoN = new List<string>();
            item.SystemTagNoN = new List<string>();
            item.CustomStampNo = "";
            item.EventType = QsDbEventTypeEnum.System.ToString("d");
            item.AllDayFlag = bool.TrueString;
            item.DeleteFlag = bool.FalseString;
            item.FinishFlag = bool.FalseString;
            item.Importance = "1";
            item.Sequence = "-1";
            item.NoticeFlag = bool.FalseString;
            item.OpenFlag = bool.FalseString;
            item.EventSetTypeName = typeof(QhMedicineEventSetOfJson).Name;
            item.EventSet = new QoApiCalendarEventSetItem();

            //外部キーは作って渡す
            item.ForeignKey = new QsJsonSerializer().Serialize<QhEventForeignKeyOfJson>(
                this.CreateCalendarEventForeignKey(
                    new DbMedicineKeyItem()
                    {
                        AccountKey = args.ActorKey.ToValueType<Guid>(),
                        RecordDate = recordDate,
                        Sequence = medSequence,
                    }));

            apiArgs.Event = item;

            QoCalendarEventWriteApiResults apiResults = CalendarWorker.EventWrite(apiArgs);

            if (apiResults != null)
            {
                if (apiResults.IsSuccess == bool.TrueString &&
                    apiResults.Sequence.TryToValueType<int>(int.MinValue) >= 0)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// お薬イベントを削除する
        /// </summary>
        /// <param name="args"></param>
        /// <param name="deletedKeys"></param>
        /// <returns></returns>
        public bool DeleteMedicineEvent(QoNoteMedicineDeleteApiArgs args, List<DbMedicineKeyItem> deletedKeys)
        {
            var result = false;
            var ser = new QsJsonSerializer();
            var accountKey = args.ActorKey.ToValueType<Guid>();

            var reader = new DbCalendarEventReader();
            var readerResult = new DbCalendarEventReaderResults();

            // 基本的にQH_MEDICINE_DATとQH_EVENT_DATは１：１なので1件しか来ないはず
            foreach (DbMedicineKeyItem key in deletedKeys)
            {
                var readerArgs = new DbCalendarEventReaderArgs()
                {
                    ActorKey = accountKey,
                    AuthorKey = accountKey,
                    LinkageSystemNo = args.LinkageSystemNo.TryToValueType<int>(int.MinValue),
                    EventDate = key.RecordDate,
                    ForeignKey = ser.Serialize<QhEventForeignKeyOfJson>(this.CreateCalendarEventForeignKey(key))
                };

                readerResult = QsDbManager.Read(reader, readerArgs);

                var eventN = new List<DbEventItem>();

                if (readerResult != null)
                {
                    if (readerResult.IsSuccess && readerResult.EventN != null && readerResult.EventN.Any())
                    {
                        eventN = readerResult.EventN; // 同じForeignKeyで複数返ってくることも想定
                    }
                }

                if (eventN != null && eventN.Any())
                {
                    foreach (DbEventItem item in eventN)
                    {
                        var apiArgs = new QoCalendarEventWriteApiArgs()
                        {
                            ActorKey = args.ActorKey,
                            ApiType = QoApiTypeEnum.CalendarEventWrite.ToString(),
                            ExecuteSystemType = args.ExecuteSystemType,
                            Executor = args.Executor,
                            ExecutorName = args.ExecutorName,
                            Event = new QoApiCalendarEventItem()
                            {
                                LinkageSystemNo = item.LinkageSystemNo.ToString(),
                                EventDate = item.EventDate.ToApiDateString(),
                                Sequence = item.EventSequence.ToString(),
                                EventType = item.EventType.ToString("d"),
                                EventSetTypeName = item.EventSetTypeName,
                                DeleteFlag = bool.TrueString
                            }
                        };

                        QoCalendarEventWriteApiResults apiResults = CalendarWorker.EventWrite(apiArgs);

                        if (apiResults != null)
                        {
                            if (apiResults.IsSuccess == bool.TrueString &&
                                apiResults.Sequence.TryToValueType<int>(int.MinValue) >= 0)
                            {
                                result = true;
                            }
                        }
                    }
                }

            }
            return result;

        }

        private QhEventForeignKeyOfJson CreateCalendarEventForeignKey(DbMedicineKeyItem key)
        {
            // 外部キーは作って渡す
            var keyColumnN = new List<QhTableColumnReferenceOfJson>()
            {
                new QhTableColumnReferenceOfJson()  { Name = "AccountKey", Value = key.AccountKey.ToString("N")},
                new QhTableColumnReferenceOfJson()  { Name = "RecordDate", Value = key.RecordDate.ToApiDateString()},
                new QhTableColumnReferenceOfJson()  { Name = "Sequence", Value = key.Sequence.ToString()}
            };

            var tableReferenceN = new List<QhTableReferenceOfJson>()
            {
                new QhTableReferenceOfJson()  {Name = typeof(QH_MEDICINE_DAT).Name, KeyColumnN = keyColumnN}
            };

            return new QhEventForeignKeyOfJson() { TableReferenceN = tableReferenceN };
        }

        /// <summary>
        /// ファイルキー取得
        /// </summary>
        /// <param name="yjCode"></param>
        /// <returns></returns>
        public List<Guid> ReadEthicalDrugFileKey(string yjCode)
        {
            var result = new List<Guid>();

            if (!string.IsNullOrWhiteSpace(yjCode))
            {
                var reader = new DbEthicalDrugFileKeyReader();
                var readerArgs = new DbEthicalDrugFileKeyReaderArgs() { YjCode = yjCode };
                DbEthicalDrugFileKeyReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

                if (readerResults != null)
                {
                    if (readerResults.IsSuccess && readerResults.FileKeyN != null && readerResults.FileKeyN.Any())
                    {
                        result = readerResults.FileKeyN;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<QH_ETHDRUGFILE_MST> ReadEthicalDrugFileListWithYjCode(List<string> yjCodeN)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ETHDRUGFILE_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", string.Join(",",yjCodeN)),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_ETHDRUGFILE_MST)}
                    WHERE {nameof(QH_ETHDRUGFILE_MST.YJCODE)} IN (SELECT value FROM STRING_SPLIT(@p1,','))
                    AND   {nameof(QH_ETHDRUGFILE_MST.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_ETHDRUGFILE_MST>(con, null, sql, paramList);

                return result;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<QH_OTCDRUGFILE_MST> ReadOtcDrugFileListWithItemCode(List<string> itemCodeN)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_OTCDRUGFILE_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con,"@p1", string.Join(",",itemCodeN)),
                };

                var sql = $@"
                    SELECT *
                    FROM  {nameof(QH_OTCDRUGFILE_MST)}
                    WHERE {nameof(QH_OTCDRUGFILE_MST.ITEMCODE)} + {nameof(QH_OTCDRUGFILE_MST.ITEMCODETYPE)} IN (SELECT value FROM STRING_SPLIT(@p1,','))
                    AND   {nameof(QH_OTCDRUGFILE_MST.CONTENTTYPE)} = 'image/jpeg'
                    AND   {nameof(QH_OTCDRUGFILE_MST.DELETEFLAG)} = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_OTCDRUGFILE_MST>(con, null, sql, paramList);

                return result;
            }
        }

        /// <summary>
        /// 薬効分類取得
        /// </summary>
        /// <param name="yjCodeN"></param>
        /// <returns></returns>
        public List<DbEthicalDrugCategoryItem> ReadYakkoList(List<string> yjCodeN)
        {
            var result = new List<DbEthicalDrugCategoryItem>();

            if (yjCodeN != null && yjCodeN.Any())
            {
                var reader = new DbEthicalDrugCategoryReader();
                var readerArgs = new DbEthicalDrugCategoryReaderArgs() { YjCodeN = yjCodeN };
                DbEthicalDrugCategoryReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

                if (readerResults != null)
                {
                    if (readerResults.IsSuccess)
                    {
                        result = new List<DbEthicalDrugCategoryItem>();
                        if (readerResults.CategoryN != null && readerResults.CategoryN.Any()) result = readerResults.CategoryN;
                    }
                }
            }

            return result;
        }

        
    }
}
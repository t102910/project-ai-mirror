using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.JAHISMedicineEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// お薬手帳の情報を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbNoteMedicineWriterCore : QsDbWriterBase
    {
        /// <summary>
        /// アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _accountKey = Guid.Empty;

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbNoteMedicineWriterCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbNoteMedicineWriterCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">対象者アカウントキー</param>
        /// <remarks></remarks>
        public DbNoteMedicineWriterCore(Guid accountKey) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }

        #region "Private Method"

        /// <summary>
        /// ログテーブル用のエンティティを作成します。
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="histType"></param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        private QH_MEDICINEHIST_LOG CreateLogEntity(QH_MEDICINE_DAT entity, QH_MEDICINEHIST_LOG.HistTypeEnum histType, DateTime actionDate, Guid actionKey)
        {
            if (entity == null) throw new ArgumentNullException("entity", "登録対象のお薬情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY!= this._accountKey ) throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");

            return new QH_MEDICINEHIST_LOG()
            {
                ACCOUNTKEY = entity.ACCOUNTKEY,
                RECORDDATE = entity.RECORDDATE,
                SEQUENCE = entity.SEQUENCE,
                ACTIONDATE = actionDate,
                ACTIONKEY = actionKey,
                HISTTYPE = (byte)histType,
                PHARMACYNO = entity.PHARMACYNO,
                RECEIPTNO = entity.RECEIPTNO,
                FACILITYKEY = entity.FACILITYKEY,
                ORIGINALFILENAME = entity.ORIGINALFILENAME,
                MEDICINESET = entity.MEDICINESET,
                CONVERTEDMEDICINESET = entity.CONVERTEDMEDICINESET,
                COMMENTSET = entity.COMMENTSET
            };
        }

        /// <summary>
        /// お薬手帳情報データテーブルエンティティを登録します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <param name="refDataId"></param>
        /// <param name="refSequence"></param>
        /// <returns>成功なら True、失敗なら例外をスロー。</returns>
        private bool UpsertEntity(QH_MEDICINE_DAT entity, DateTime actionDate, Guid actionKey, ref string refDataId, ref int refSequence)
        {
            if (entity == null) throw new ArgumentNullException("entity", "登録対象のお薬手帳情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey) throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (string.IsNullOrWhiteSpace(entity.MEDICINESET)) throw new ArgumentNullException("entity.MedicineSET", "お薬情報がNull参照もしくは空白です。");

            // 日付け内連番を追加するため、日付けの最新連番を取得
            if (entity.SEQUENCE <= 0)
            {
                entity.SEQUENCE = GetSequenceNo(entity.ACCOUNTKEY, entity.RECORDDATE) + 1; // 日付け内で初レコードであれば連番は１開始

                // データID未採番のときに採番する
                if (string.IsNullOrWhiteSpace(entity.RECEIPTNO))
                {
                    entity.RECEIPTNO = string.Format("{0}{1}", entity.RECORDDATE.ToString("yyyyMMdd"), entity.SEQUENCE.ToString("d6"));
                }
            }

            var writer = new QhMedicineEntityWriter();
            var writerArgs = new QhMedicineEntityWriterArgs() { Data = new List<QH_MEDICINE_DAT>() { entity } };
            QhMedicineEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                // ログテーブル書き込み
                QH_MEDICINEHIST_LOG logEntity = this.CreateLogEntity(entity, QH_MEDICINEHIST_LOG.HistTypeEnum.Added, actionDate, actionKey);
                QsDbManager.WriteByCurrent(
                    new QhMedicineHistEntityWriter(),
                    new QhMedicineHistEntityWriterArgs()
                    {
                        Data = new List<QH_MEDICINEHIST_LOG>() { logEntity }
                    }
                );

                refDataId = entity.RECEIPTNO;
                refSequence = entity.SEQUENCE;
                return true;
            }
            else
            {
                throw new InvalidOperationException("QH_MEDICINE_DATテーブルへの登録に失敗しました。");
            }
        }

        private int GetSequenceNo(Guid accountkey, DateTime recordDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORD_DAT>())
            {
                var query = new StringBuilder();
                var @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@p1", accountkey),
                    this.CreateParameter(connection, "@p2", recordDate)
                };

                var refIsSuccess = false;

                query.Append("select top(1) sequence from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.Append(" and recorddate = @p2");
                query.Append(" order by sequence desc");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<int>(connection, null, query.ToString(), @params, 0,ref refIsSuccess);
            }
        }

        // 市販薬のQOLMSからのデータには画像ファイルキーが付いてくるが、アプリにはなくて消してしまうのを回避するために
        // どうにもならないので、ファイルが付いていたらコメントだけアップデートさせることにする。
        // 薬のキー値を持たないと順番とか削除とかいろいろ起きるのでJANこーどなりを持つようになったらここを妥協せず直す
        private string SetCommentAndMedicineName(string oldMedicineEncSet, string newMedicineEncSet, ref bool exsitAttachedFile)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(oldMedicineEncSet)) return newMedicineEncSet;
            var oldMedicineSet = string.Empty;
            var newMedicineSet = string.Empty;

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                oldMedicineSet = crypt.DecryptString(oldMedicineEncSet);
                newMedicineSet = crypt.DecryptString(newMedicineEncSet);

                var ser = new QsJsonSerializer();

                var oldMedicineSetObj = ser.Deserialize<QhMedicineSetOtcDrugOfJson>(oldMedicineSet);
                var newMedicineSetObj  = ser.Deserialize<QhMedicineSetOtcDrugOfJson>(newMedicineSet);

                exsitAttachedFile = false;
                if (oldMedicineSetObj != null && oldMedicineSetObj.MedicineN != null &&
                     newMedicineSetObj.MedicineN.Any(m => { return string.IsNullOrWhiteSpace(m.ItemCode); }))
                {
                    foreach (QhMedicineSetOtcDrugItemOfJson item in oldMedicineSetObj.MedicineN)
                    {
                        if(item.AttachedFileN != null && item.AttachedFileN.Any())
                        {
                            exsitAttachedFile = true;
                        }
                    }
                }

                if (exsitAttachedFile)
                {
                    // 添付ファイルが存在した上、新しいオブジェクト内にItemCodeがない(古いデータ)ときは、QOLMSから入れられたものなので、どうにもならないので薬品編集はスルーしてコメントだけ反映
                    oldMedicineSetObj.Comment = newMedicineSetObj.Comment;
                    result = crypt.EncryptString(ser.Serialize<QhMedicineSetOtcDrugOfJson>(oldMedicineSetObj));
                }
                else
                {
                    // 添付ファイルがないときはフル編集通す
                    //（薬品にキーがないので編集箇所を特定できない）
                    result = crypt.EncryptString(ser.Serialize<QhMedicineSetOtcDrugOfJson>(newMedicineSetObj));
                }
            }

            return result;
        }

        // 市販薬のQOLMSからのデータには画像ファイルキーが付いてくるが、アプリにはなくて消してしまうのを回避するために
        // どうにもならないので、ファイルが付いていたらコメントだけアップデートさせることにする。
        // 薬のキー値を持たないと順番とか削除とかいろいろ起きるのでJANこーどなりを持つようになったらここを妥協せず直す
        private string SetCommentOnlyOrAll(string oldMedicineEncSet, string newMedicineEncSet, ref bool exsitAttachedFile)
        {
            var result = string.Empty;
            var oldMedicineSet = string.Empty;
            var newMedicineSet = string.Empty;

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                oldMedicineSet = crypt.DecryptString(oldMedicineEncSet);
                newMedicineSet = crypt.DecryptString(newMedicineEncSet);
                var ser = new QsJsonSerializer();
                var oldJahis = ser.Deserialize<JM_Message>(oldMedicineSet);
                var newJahis = ser.Deserialize<JM_Message>(newMedicineSet);

                exsitAttachedFile = false;

                if (exsitAttachedFile)
                {
                    // 添付ファイルが存在したした上、新しいオブジェクト内にItemCodeがない(古いデータ)ときは、どうにもならないので薬品編集はスルーしてコメントだけ反映
                    oldJahis.No004_List = new List<JM_No004>(newJahis.No004_List);
                    result = crypt.EncryptString(ser.Serialize<JM_Message>(oldJahis));
                }
                else
                {
                    // 添付ファイルがないときもしくは新しい形式のデータはフル編集通す
                    //（薬品にキーがないので編集箇所を特定できない）
                    result = crypt.EncryptString(ser.Serialize<JM_Message>(newJahis));
                }
            }

            return result;
        }

        // 患者が入れる情報だけ値をコピーする
        private string SetNo601_2(string oldMedicineEncSet, string newMedicineEncSet)
        {
            var result = string.Empty;
            var oldMedicineSet = string.Empty;
            var newMedicineSet = string.Empty;

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                oldMedicineSet = crypt.DecryptString(oldMedicineEncSet);
                newMedicineSet = crypt.DecryptString(newMedicineEncSet);
                var ser = new QsJsonSerializer();
                var oldJahis = ser.Deserialize<JM_Message>(oldMedicineSet);
                var newJahis = ser.Deserialize<JM_Message>(newMedicineSet);

                for (int i = 0; i <= oldJahis.Prescription_List.Count - 1; i++)
                {
                    oldJahis.Prescription_List[i].No601_List = new List<JM_No601>(newJahis.Prescription_List[i].No601_List);
                }

                result = crypt.EncryptString(ser.Serialize<JM_Message>(oldJahis));
            }

            return result;
        }

        // Memoだけコピーする
        private string SetPatientMemo(string oldMedicineEncSet, string newMedicineEncSet, QH_MEDICINE_DAT.DataTypeEnum dataType )
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(oldMedicineEncSet)) return newMedicineEncSet;
            var oldMedicineSet = string.Empty;
            var newMedicineSet = string.Empty;

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                oldMedicineSet = crypt.DecryptString(oldMedicineEncSet);
                newMedicineSet = crypt.DecryptString(newMedicineEncSet);
                var ser = new QsJsonSerializer();

                switch (dataType)
                {
                    case QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug:
                        {
                            QhMedicineSetEthicalDrugOfJson oldMedicineSetObj = ser.Deserialize<QhMedicineSetEthicalDrugOfJson>(oldMedicineSet);
                            QhMedicineSetEthicalDrugOfJson newMedicineSetObj = ser.Deserialize<QhMedicineSetEthicalDrugOfJson>(newMedicineSet);
                            oldMedicineSetObj.Memo = newMedicineSetObj.Memo;
                            result = crypt.EncryptString(ser.Serialize<QhMedicineSetEthicalDrugOfJson>(oldMedicineSetObj));

                        }
                        break;
                    case QH_MEDICINE_DAT.DataTypeEnum.OtcDrug:
                        {
                            QhMedicineSetOtcDrugOfJson oldMedicineSetObj = ser.Deserialize<QhMedicineSetOtcDrugOfJson>(oldMedicineSet);
                            QhMedicineSetOtcDrugOfJson newMedicineSetObj = ser.Deserialize<QhMedicineSetOtcDrugOfJson>(newMedicineSet);
                            oldMedicineSetObj.Comment = newMedicineSetObj.Comment;
                            result = crypt.EncryptString(ser.Serialize<QhMedicineSetOtcDrugOfJson>(oldMedicineSetObj));
                        }
                        break;
                    case QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto:
                        {
                            // 市販薬写真（ファイル、その他文字情報）
                            QhMedicineSetOtcDrugPhotoOfJson oldMedicineSetObj = ser.Deserialize<QhMedicineSetOtcDrugPhotoOfJson>(oldMedicineSet);
                            QhMedicineSetOtcDrugPhotoOfJson newMedicineSetObj = ser.Deserialize<QhMedicineSetOtcDrugPhotoOfJson>(newMedicineSet);
                            oldMedicineSetObj.Memo = newMedicineSetObj.Memo;
                            result = crypt.EncryptString(ser.Serialize<QhMedicineSetOtcDrugPhotoOfJson>(oldMedicineSetObj));
                        }
                        break;
                    default:
                        result = oldMedicineEncSet;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// お薬手帳情報データテーブルエンティティを編集します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns>成功なら True、失敗なら例外をスロー。</returns>
        private bool UpdateEntity(QH_MEDICINE_DAT entity, DateTime actionDate, Guid actionKey)
        {
            if (entity == null) throw new ArgumentNullException("entity", "登録対象のお薬手帳情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey) throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (string.IsNullOrWhiteSpace(entity.MEDICINESET)) throw new ArgumentNullException("entity.MedicineSET", "お薬情報がNull参照もしくは空白です。");

            List<QH_MEDICINE_DAT> oldEntity = null;
            if(entity.SEQUENCE > int.MinValue)
            {
                oldEntity = this.SelectEntity(entity.RECORDDATE, entity.SEQUENCE);
            }
            else if(!string.IsNullOrWhiteSpace(entity.RECEIPTNO))
            {
                oldEntity = this.SelectEntityFromDataId(entity.RECORDDATE, entity.RECEIPTNO);
            }

            // 編集対象のデータエンティティを取得
            if(oldEntity != null && oldEntity.Count == 1)
            {
                switch ((QH_MEDICINE_DAT.OwnerTypeEnum)oldEntity[0].OWNERTYPE)
                {
                    case QH_MEDICINE_DAT.OwnerTypeEnum.Oneself:
                        // 市販薬のQOLMSからのデータには画像ファイルキーが付いてくるが、アプリにはなくて消してしまうのを回避
                        if (oldEntity[0].DATATYPE == (byte)QH_MEDICINE_DAT.DataTypeEnum.OtcDrug)
                        {
                            var exixtFile = false;
                            entity.MEDICINESET = SetCommentAndMedicineName(oldEntity[0].MEDICINESET, entity.MEDICINESET, ref exixtFile);
                            entity.CONVERTEDMEDICINESET = SetCommentOnlyOrAll(oldEntity[0].CONVERTEDMEDICINESET, entity.CONVERTEDMEDICINESET, ref exixtFile);
                        }
                        break;
                    default:
                        // コメントだけしか編集不可
                        //多分QH_MEDICINE_DAT.OwnerTypeEnum.Data, QH_MEDICINE_DAT.OwnerTypeEnum.QrCodeはコメント以外編集不可
                        //それ以外のはよくわからないのでとりあえずコメントのみとしとく。
                        entity.MEDICINESET = SetPatientMemo(oldEntity[0].MEDICINESET, entity.MEDICINESET, (QH_MEDICINE_DAT.DataTypeEnum)entity.DATATYPE);
                        entity.CONVERTEDMEDICINESET = SetNo601_2(oldEntity[0].CONVERTEDMEDICINESET, entity.CONVERTEDMEDICINESET);
                        entity.ENDDATE = oldEntity[0].ENDDATE;
                        entity.ORIGINALFILENAME = oldEntity[0].ORIGINALFILENAME;
                        entity.PRESCRIPTIONDATE = oldEntity[0].PRESCRIPTIONDATE;
                        entity.STARTDATE = oldEntity[0].STARTDATE;
                        break;
                }

                // この辺はキーなので間違っても変わらないように。
                entity.RECORDDATE = oldEntity[0].RECORDDATE;
                entity.ACCOUNTKEY = oldEntity[0].ACCOUNTKEY;
                entity.SEQUENCE = oldEntity[0].SEQUENCE;

                // この辺もとりあえず変えさせないでおくけど、実装かえる必要あるかも
                entity.DATATYPE = oldEntity[0].DATATYPE;
                entity.LINKAGESYSTEMNO = oldEntity[0].LINKAGESYSTEMNO;
                entity.FACILITYKEY = entity.FACILITYKEY == Guid.Empty ? oldEntity[0].FACILITYKEY : entity.FACILITYKEY;
                entity.OWNERTYPE = oldEntity[0].OWNERTYPE;

                // お薬情報を更新、更新日時＝現在時刻
                entity.UPDATEDDATE = actionDate;
                entity.LINKAGESYSTEMNO = oldEntity[0].LINKAGESYSTEMNO;

                var writer = new QhMedicineEntityWriter();
                var writerArgs = new QhMedicineEntityWriterArgs() { Data = { entity} };
                QhMedicineEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);
                
                if (writerResults.IsSuccess && writerResults.Result == 1)
                {
                    // ログテーブル書き込み
                    QH_MEDICINEHIST_LOG logEntity = this.CreateLogEntity(oldEntity[0], QH_MEDICINEHIST_LOG.HistTypeEnum.Modified, actionDate, actionKey);
                    QsDbManager.WriteByCurrent(
                        new QhMedicineHistEntityWriter(),
                        new QhMedicineHistEntityWriterArgs()
                        {
                            Data = new List<QH_MEDICINEHIST_LOG>() { logEntity }
                        }
                    );

                    return true;
                }
                else
                {
                    throw new InvalidOperationException("QH_MEDICINE_DATテーブルへの登録に失敗しました。");
                }
            }
            else
            {
                throw new InvalidOperationException("お薬情報の更新に失敗しました。");
            }
        }

        /// <summary>
        /// お薬手帳情報データテーブルエンティティを削除します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns>成功なら True、失敗なら例外をスロー。</returns>
        private bool DeleteEntity(QH_MEDICINE_DAT entity, DateTime actionDate,Guid actionKey)
        {

            if (entity == null) throw new ArgumentNullException("entity", "登録対象のお薬手帳情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey) throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");

            List<QH_MEDICINE_DAT> oldEntity = this.SelectEntity(entity.RECORDDATE, entity.SEQUENCE);
            // 削除対象のデータエンティティを取得
            if( oldEntity.Count == 1)
            {
                // 削除フラグを立てる、更新日時＝現在時刻
                oldEntity[0].DELETEFLAG = true;
                oldEntity[0].UPDATEDDATE = DateTime.Now;

                var writer = new QhMedicineEntityWriter();
                var writerArgs = new QhMedicineEntityWriterArgs() { Data = oldEntity };

                QhMedicineEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);
                if(writerResults.IsSuccess && writerResults.Result == 1)
                {
                    // ログテーブル書き込み
                    QH_MEDICINEHIST_LOG logEntity = this.CreateLogEntity(oldEntity[0], QH_MEDICINEHIST_LOG.HistTypeEnum.Deleted, actionDate, actionKey);
                    QsDbManager.WriteByCurrent(
                        new QhMedicineHistEntityWriter(),
                        new QhMedicineHistEntityWriterArgs() 
                        {
                            Data = new List<QH_MEDICINEHIST_LOG>() { logEntity }
                        }
                    );

                    return true;
                }
                else
                {
                    throw new InvalidOperationException("QH_MEDICINE_DATテーブルへの登録に失敗しました。");
                }
            }
            else
            {
                throw new InvalidOperationException("お薬情報の更新に失敗しました。");
            }
        }

        /// <summary>
        /// 評価日と日付け内連番を指定して、
        /// お薬情報データテーブルエンティティを取得します。
        /// </summary>
        /// <param name="recordDate">評価日。</param>
        /// <param name="sequence">日付け内連番。</param>
        /// <returns>該当するテーブルエンティティ。</returns>
        private List<QH_MEDICINE_DAT> SelectEntity(DateTime recordDate, int sequence)
        {
            if ( recordDate == DateTime.MinValue) throw new ArgumentOutOfRangeException("recordDate", "評価日が不正です。");
            if(sequence == int.MinValue ) throw new ArgumentOutOfRangeException("sequence", "日付け内連番が不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var query = new StringBuilder();
                var @params = new List<DbParameter>()  {
                    this.CreateParameter(connection, "@p1", this._accountKey),
                    this.CreateParameter(connection, "@p2", DateTime.ParseExact(recordDate.ToString("yyyyMMdd0000000000000"), "yyyyMMddHHmmssfffffff", null, System.Globalization.DateTimeStyles.None)),
                    this.CreateParameter(connection, "@p3", sequence)
                };

                // クエリ作成
                query.Append("select * from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.Append(" and recorddate = @p2");
                query.Append(" and sequence = @p3");
                query.Append(" and deleteflag = 0");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_MEDICINE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// データを指定して、
        /// お薬情報データテーブルエンティティを取得します。
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns>該当するテーブルエンティティ。</returns>
        private List<QH_MEDICINE_DAT> SelectEntityFromDataId(string dataId)
        {
            if (string.IsNullOrWhiteSpace(dataId)) throw new ArgumentOutOfRangeException("dataId", "データIDが不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var query = new StringBuilder();
                var @params = new List<DbParameter>()  {
                    this.CreateParameter(connection, "@p1", this._accountKey),
                    this.CreateParameter(connection, "@p2", dataId)
                };

                // クエリ作成
                query.Append("select * from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.Append(" and receiptno = @p2");
                query.Append(" and deleteflag = 0");
                query.Append(";"); 

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_MEDICINE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        /// <summary>
        /// データを指定して、
        /// お薬情報データテーブルエンティティを取得します。
        /// </summary>
        /// <param name="recordDate"></param>
        /// <param name="dataId"></param>
        /// <returns>該当するテーブルエンティティ。</returns>
        private List<QH_MEDICINE_DAT> SelectEntityFromDataId(DateTime recordDate, string dataId)
        {
            if (string.IsNullOrWhiteSpace(dataId)) throw new ArgumentOutOfRangeException("dataId", "データIDが不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var query = new StringBuilder();
                var @params = new List<DbParameter>()  {
                    this.CreateParameter(connection, "@p1", this._accountKey),
                    this.CreateParameter(connection, "@p2", dataId),
                    this.CreateParameter(connection, "@p3", recordDate)
                };

                // クエリ作成
                query.Append("select * from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.Append(" and receiptno = @p2");
                query.Append(" and recorddate= @p3");
                query.Append(" and deleteflag = 0");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_MEDICINE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }

        #endregion

        #region "public method"


        /// <summary>
        /// お薬情報を登録します。
        /// </summary>
        /// <param name="args"></param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <param name="refDataId"></param>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        public bool WriteMedicineSet(NoteMedicineWriterArgs args, DateTime actionDate, Guid actionKey, ref string refDataId, ref int refSequence)
        {
            var cMedSet = string.Empty;
            var cCovSet = string.Empty;

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                cMedSet = !string.IsNullOrWhiteSpace(args.MedicineSet) ? crypt.EncryptString(args.MedicineSet) : string.Empty;
                cCovSet = !string.IsNullOrWhiteSpace(args.ConvertedMedicineSet) ? crypt.EncryptString(args.ConvertedMedicineSet) : string.Empty;
            }

            var entity = new QH_MEDICINE_DAT()
            {
                ACCOUNTKEY = args.ActorKey,
                RECORDDATE = args.RecordDate,
                SEQUENCE = args.Sequence,
                DATATYPE = args.DataType,
                OWNERTYPE = args.OwnerType,
                LINKAGESYSTEMNO = args.LinkageSystemNo,
                PHARMACYNO = args.PharmacyNo,
                RECEIPTNO = args.ReceiptNo,
                FACILITYKEY = args.FacilityKey,
                PRESCRIPTIONDATE = args.PrescriptionDate,
                STARTDATE = args.StartDate,
                ENDDATE = args.EndDate,
                ORIGINALFILENAME = args.OriginalFileName,
                MEDICINESET = cMedSet,
                CONVERTEDMEDICINESET = cCovSet,
                COMMENTSET = args.CommentSet,
                DELETEFLAG = args.DeleteFlag,
                CREATEDDATE = actionDate,
                UPDATEDDATE = actionDate
            };

            if (entity.DELETEFLAG)
            {
                return this.DeleteEntity(entity, actionDate, actionKey);
            }
            else if(entity.SEQUENCE == int.MinValue && string.IsNullOrWhiteSpace(entity.RECEIPTNO))
            {
                return this.UpsertEntity(entity, actionDate, actionKey,ref refDataId,ref refSequence);
            }
            else
            {
                return this.UpdateEntity(entity, actionDate, actionKey);
            }
        }

        /// <summary>
        /// お薬情報を削除します。
        /// </summary>
        /// <param name="args"></param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <param name="deletedKeys"></param>
        /// <returns></returns>
        public int DeleteFromDataId(NoteMedicineDeleterArgs args, DateTime actionDate, Guid actionKey, ref List<DbMedicineKeyItem> deletedKeys)
        {
            // QOLMS本体とDataSync？で作られるデータはReceiptNoがちゃんと入っていないのでDataIdでの削除をサポートしない
            const int MIN_LENGTH = 10; // テーブル主キー（RecordDateとSequence）から生成する際に一意が保証されるであろう桁数
            const int MAX_LENGTH = 14; //JAHISでサポートされる分割制御データ固有IDの最大長（DBの最大長は20）

            if (args == null) throw new ArgumentNullException("args", "引数が不正です。");
            if (string.IsNullOrWhiteSpace(args.DataId)) throw new ArgumentNullException("DataId", "データIDが不正です。");
            if (args.DataId.Length < MIN_LENGTH || MAX_LENGTH < args.DataId.Length) throw new ArgumentOutOfRangeException("DataId", "データIDの桁数が不正です。");

            var result = 0;

            // DataId文字長に制限かけてるので1件しか取れないはず。複数取れたら全部削除
            List<QH_MEDICINE_DAT> entities = this.SelectEntityFromDataId(args.DataId);

            if( entities != null && entities.Any())
            {
                deletedKeys = new List<DbMedicineKeyItem>();
                foreach (QH_MEDICINE_DAT entity in entities)
                {
                    try
                    {
                        if (this.DeleteEntity(entity, actionDate, actionKey))
                        {
                            result += 1;
                            deletedKeys.Add(new DbMedicineKeyItem()
                            {
                                AccountKey = entity.ACCOUNTKEY,
                                RecordDate = entity.RECORDDATE,
                                Sequence = entity.SEQUENCE,
                                DataType = entity.DATATYPE
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
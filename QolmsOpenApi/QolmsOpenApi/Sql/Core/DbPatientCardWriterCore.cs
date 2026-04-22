using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsCryptV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{

    /// <summary>
    ///利用者カードの情報を、
    ///データベーステーブルへ登録するための機能を提供します。
    ///このクラスは継承できません。
    ///</summary>
    ///<remarks></remarks>
    internal sealed class DbPatientCardWriterCore : QsDbWriterBase
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
        private DbPatientCardWriterCore()
        {
        }

        #region "private Method"


        /// <summary>
        /// ログテーブル用のエンティティを作成します。
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="histType"></param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private QH_PATIENTCARDHIST_LOG CreateLogEntity(QH_PATIENTCARD_DAT entity, QH_PATIENTCARDHIST_LOG.HistTypeEnum histType, DateTime actionDate, Guid actionKey)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象の利用者カード情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");

            return new QH_PATIENTCARDHIST_LOG()
            {
                ACCOUNTKEY = entity.ACCOUNTKEY,
                CARDCODE = entity.CARDCODE,
                SEQUENCE = entity.SEQUENCE,
                ACTIONDATE = actionDate,
                ACTIONKEY = actionKey,
                HISTTYPE = (byte)histType,
                CARDNO = entity.CARDNO,
                PATIENTCARDSET = histType == QH_PATIENTCARDHIST_LOG.HistTypeEnum.Added ? string.Empty : entity.PATIENTCARDSET
            };
        }

        /// <summary>
        /// 利用者カード情報データテーブルエンティティを登録します。
        /// </summary>  
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <param name="errorMessage"></param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private bool UpsertEntity(QH_PATIENTCARD_DAT entity, DateTime actionDate, Guid actionKey, ref string errorMessage)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象の利用者カード情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (string.IsNullOrWhiteSpace(entity.PATIENTCARDSET))
                throw new ArgumentNullException("entity.PatientCardSET", "利用者カード情報がNull参照もしくは空白です。");

            // カード連番を追加するため、最新連番を取得
            if (entity.SEQUENCE <= 0)
            {
                if (entity.CARDCODE == 0)
                    // その他の場合は連番が増えていく
                    entity.SEQUENCE = GetCardSequence(entity.ACCOUNTKEY, entity.CARDCODE) + 1; // 初レコードであれば連番は１開始
                else
                    // 連携カードの場合は1枚のみ許可
                    entity.SEQUENCE = GetCardSequenceForLinkage(entity.ACCOUNTKEY, entity.CARDCODE) + 1;// 初レコードであれば連番は１開始
            }

            if (entity.CARDCODE > 0 && !this.IsRegisterCheckCard(entity.ACCOUNTKEY, entity.CARDCODE, entity.CARDNO) | entity.SEQUENCE > 1)
            {
                // 連携システムカードの場合、１枚dのみ登録を許可するので、既に登録があればエラーメッセージを返却。
                // エラー文章分ける？
                errorMessage = "指定された種類のカードは、すでに登録があります。";
                return false;
            }

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", entity.ACCOUNTKEY),
                this.CreateParameter(connection, "@p2", entity.CARDCODE),
                this.CreateParameter(connection, "@p3", entity.SEQUENCE),
                this.CreateParameter(connection, "@p4", entity.FACILITYKEY),
                this.CreateParameter(connection, "@p5", entity.CARDNO),
                this.CreateParameter(connection, "@p6", entity.PATIENTCARDSET),
                this.CreateParameter(connection, "@p7", entity.DELETEFLAG),
                this.CreateParameter(connection, "@p8", entity.CREATEDDATE),
                this.CreateParameter(connection, "@p9", entity.UPDATEDDATE)
            };

                // クエリを作成

                query.Append("update qh_patientcard_dat");
                query.Append(" set facilitykey = @p4, cardno = @p5, patientcardset = @p6, deleteflag = @p7, createddate = @p8, updateddate = @p9");
                query.Append(" where accountkey = @p1 and cardcode = @p2 and sequence = @p3");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行

                if (this.ExecuteNonQuery(connection, null/* TODO Change to default(_) if this is not a reference type */, query.ToString(), @params) == 1)
                {

                    // ログテーブル書き込み
                    QH_PATIENTCARDHIST_LOG logEntity = this.CreateLogEntity(entity, QH_PATIENTCARDHIST_LOG.HistTypeEnum.Modified, actionDate, actionKey);
                    QsDbManager.Write(new QhPatientCardHistEntityWriter(), new QhPatientCardHistEntityWriterArgs() { Data = new List<QH_PATIENTCARDHIST_LOG>() { logEntity } }
    );

                    return true;
                }
                else
                {
                }
            }

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", entity.ACCOUNTKEY),
                this.CreateParameter(connection, "@p2", entity.CARDCODE),
                this.CreateParameter(connection, "@p3", entity.SEQUENCE),
                this.CreateParameter(connection, "@p4", entity.FACILITYKEY),
                this.CreateParameter(connection, "@p5", entity.CARDNO),
                this.CreateParameter(connection, "@p6", entity.PATIENTCARDSET),
                this.CreateParameter(connection, "@p7", entity.DELETEFLAG),
                this.CreateParameter(connection, "@p8", entity.CREATEDDATE),
                this.CreateParameter(connection, "@p9", entity.UPDATEDDATE)
            };

                // クエリを作成
                query.Append("insert into qh_patientcard_dat");
                query.Append(" values(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9)");
                query.Append(";");


                // コネクションオープン
                connection.Open();

                if (this.ExecuteNonQuery(connection, null/* TODO Change to default(_) if this is not a reference type */, query.ToString(), @params) == 1)
                {
                    // ログテーブル書き込み
                    QH_PATIENTCARDHIST_LOG logEntity = this.CreateLogEntity(entity, QH_PATIENTCARDHIST_LOG.HistTypeEnum.Added, actionDate, actionKey);
                    QsDbManager.Write(new QhPatientCardHistEntityWriter(), new QhPatientCardHistEntityWriterArgs() { Data = new List<QH_PATIENTCARDHIST_LOG>() { logEntity } });

                    return true;
                }
                else
                    throw new InvalidOperationException("QH_PATIENTCARD_DATテーブルへの登録に失敗しました。");
            }
        }

        private bool IsRegisterCheckCard(Guid accountkey, int linkageSystemNo, string linkageSystemId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", accountkey),
                this.CreateParameter(connection, "@p2", linkageSystemNo),
                this.CreateParameter(connection, "@p3", linkageSystemId)
            };
                bool refIsSuccess = false;

                // クエリを作成
                query.Append("select count(*) from qh_linkage_dat");
                query.Append(" where ");
                query.Append(" (LINKAGESYSTEMNO = @p2  and  linkagesystemid = @p3 and deleteflag = 0)");
                query.Append(" or");
                query.Append(" ( accountkey = @p1 and LINKAGESYSTEMNO = @p2 and deleteflag = 0)");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<int>(connection, null, query.ToString(), @params, 0, ref refIsSuccess) == 0;
            }
        }


        private bool IsUpdateCheckCard(Guid accountkey, int linkageSystemNo, string linkageSystemId)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", accountkey),
                this.CreateParameter(connection, "@p2", linkageSystemNo),
                this.CreateParameter(connection, "@p3", linkageSystemId)
            };
                bool refIsSuccess = false;

                // クエリを作成

                query.Append("select count(*) from qh_linkage_dat");
                query.Append(" where ");
                query.Append(" (accountkey <> @p1 and LINKAGESYSTEMNO = @p2  and  linkagesystemid = @p3 and deleteflag = 0)");
                query.Append(";");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<int>(connection, null, query.ToString(), @params, 0, ref refIsSuccess) == 0;
            }
        }

        // 連番取得
        private int GetCardSequence(Guid accountkey, int cardcode)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", accountkey),
                this.CreateParameter(connection, "@p2", cardcode)
            };
                bool refIsSuccess = false;

                // クエリを作成

                query.Append("select top(1) sequence from qh_patientcard_dat");
                query.Append(" where accountkey = @p1");
                query.Append(" and cardcode = @p2");
                query.Append(" order by sequence desc");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<int>(connection, null, query.ToString(), @params, 0, ref refIsSuccess);
            }
        }

        // 連番取得(連携カード用）
        private int GetCardSequenceForLinkage(Guid accountkey, int cardcode)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", accountkey),
                this.CreateParameter(connection, "@p2", cardcode)
            };
                bool refIsSuccess = false;

                // クエリを作成

                query.Append("select top(1) sequence from qh_patientcard_dat");
                query.Append(" where accountkey = @p1");
                query.Append(" and cardcode = @p2");
                query.Append(" and deleteflag = 0");
                query.Append(" order by sequence desc");
                query.Append(";");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.TryExecuteScalar<int>(connection, null, query.ToString(), @params, 0, ref refIsSuccess);
            }
        }

        /// <summary>
        /// 利用者カード情報データテーブルエンティティを編集します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <param name="errorMessage"></param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private bool UpdateEntity(QH_PATIENTCARD_DAT entity, DateTime actionDate, Guid actionKey, ref string errorMessage)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象の利用者カード情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (string.IsNullOrWhiteSpace(entity.PATIENTCARDSET))
                throw new ArgumentNullException("entity.PatientCardSET", "歯の情報がNull参照もしくは空白です。");

            if (!this.IsUpdateCheckCard(entity.ACCOUNTKEY, entity.CARDCODE, entity.CARDNO))
            {
                // New InvalidOperationException("指定された種類のカードは、すでに登録があります。")
                errorMessage = "指定された種類のカードは、すでに登録があります。";
                return false;
            }

            List<QH_PATIENTCARD_DAT> oldEntity = this.SelectEntity(entity.CARDCODE, entity.SEQUENCE);
            // 編集対象のデータエンティティを取得
            if (oldEntity.Count == 1)
            {
                // 情報を更新、更新日時＝現在時刻
                entity.UPDATEDDATE = actionDate;
                QhPatientCardEntityWriter writer = new QhPatientCardEntityWriter();
                QhPatientCardEntityWriterArgs writerArgs = new QhPatientCardEntityWriterArgs() { Data = new List<QH_PATIENTCARD_DAT>() { entity } };
                QhPatientCardEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);


                if (writerResults.IsSuccess && writerResults.Result == 1)
                {

                    // ログテーブル書き込み
                    QH_PATIENTCARDHIST_LOG logEntity = this.CreateLogEntity(oldEntity[0], QH_PATIENTCARDHIST_LOG.HistTypeEnum.Modified, actionDate, actionKey);
                    QsDbManager.WriteByCurrent(new QhPatientCardHistEntityWriter(), new QhPatientCardHistEntityWriterArgs() { Data = new List<QH_PATIENTCARDHIST_LOG>() { logEntity } });

                    return true;
                }
                else
                    throw new InvalidOperationException("QH_PATIENTCARD_DATテーブルへの登録に失敗しました。");

            }
            else
                throw new InvalidOperationException("利用者カードの更新に失敗しました。");
        }

        /// <summary>
        /// 利用者カード情報データテーブルエンティティを削除します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private bool DeleteEntity(QH_PATIENTCARD_DAT entity, DateTime actionDate, Guid actionKey)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象の利用者カード情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (string.IsNullOrWhiteSpace(entity.PATIENTCARDSET))
                throw new ArgumentNullException("entity.PatientCardSET", "利用者カードの情報がNull参照もしくは空白です。");

            List<QH_PATIENTCARD_DAT> oldEntity = this.SelectEntity(entity.CARDCODE, entity.SEQUENCE);
            // 削除対象のデータエンティティを取得
            if (oldEntity.Count == 1)
            {
                // 削除フラグを立てる、更新日時＝現在時刻
                oldEntity[0].DELETEFLAG = true;
                oldEntity[0].UPDATEDDATE = DateTime.Now;
                QhPatientCardEntityWriter writer = new QhPatientCardEntityWriter();
                QhPatientCardEntityWriterArgs writerArgs = new QhPatientCardEntityWriterArgs() { Data = oldEntity };
                QhPatientCardEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);


                if (writerResults.IsSuccess && writerResults.Result == 1)
                {

                    // ログテーブル書き込み
                    QH_PATIENTCARDHIST_LOG logEntity = this.CreateLogEntity(oldEntity[0], QH_PATIENTCARDHIST_LOG.HistTypeEnum.Deleted, actionDate, actionKey);
                    QsDbManager.WriteByCurrent(new QhPatientCardHistEntityWriter(), new QhPatientCardHistEntityWriterArgs() { Data = new List<QH_PATIENTCARDHIST_LOG>() { logEntity } });

                    return true;
                }
                else
                    throw new InvalidOperationException("QH_PATIENTCARD_DATテーブルへの登録に失敗しました。");

            }
            else
                throw new InvalidOperationException("利用者カードの更新に失敗しました。");
        }

        /// <summary>
        /// カード種別番号、カード連番を指定して、
        /// 利用者カードテーブルエンティティを取得します。
        /// </summary>
        /// <param name="cardcode">カード種別番号。</param>
        /// <param name="sequence">カード連番。</param>
        /// <returns>
        /// 該当するテーブルエンティティ。
        /// </returns>
        /// <remarks></remarks>
        private List<QH_PATIENTCARD_DAT> SelectEntity(int cardcode, int sequence)
        {
            if (cardcode == int.MinValue)
                throw new ArgumentOutOfRangeException("cardcode", "カード種別番号が不正です。");
            if (sequence == int.MinValue)
                throw new ArgumentOutOfRangeException("sequence", "カード連番が不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", this._accountKey),
                this.CreateParameter(connection, "@p2", cardcode),
                this.CreateParameter(connection, "@p3", sequence)
            };

                // クエリを作成

                query.Append("select * from QH_PATIENTCARD_DAT");
                query.Append(" where accountkey = @p1");
                query.Append(" and cardcode = @p2");
                query.Append(" and sequence = @p3");
                query.Append(" and deleteflag = 0");
                query.Append(";");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_PATIENTCARD_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
            }
        }

        /// <summary>
        /// 連携システム番号を指定して、
        /// 連携システムテーブルエンティティを取得します。
        /// </summary>
        /// <returns>
        /// 該当するテーブルエンティティ。
        /// </returns>
        /// <remarks></remarks>
        private List<QH_LINKAGE_DAT> SelectLinkageSystemEntity(int linkagesystemno)
        {
            if (linkagesystemno == int.MinValue)
                throw new ArgumentOutOfRangeException("cardcode", "カード種別番号が不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", this._accountKey),
                this.CreateParameter(connection, "@p2", linkagesystemno)
            };

                // クエリを作成

                query.Append("select * from QH_LINKAGE_DAT");
                query.Append(" where accountkey = @p1");
                query.Append(" and linkagesystemno = @p2");
                query.Append(" and deleteflag = 0");
                query.Append(";");


                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_LINKAGE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
            }
        }

        /// <summary>
        /// 連携システム情報データテーブルエンティティを登録します。
        /// </summary>  
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private bool UpsertLinkageSystemEntity(QH_LINKAGE_DAT entity, DateTime actionDate, Guid actionKey)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象の利用者カード情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (entity.LINKAGESYSTEMNO == int.MinValue)
                throw new ArgumentNullException("entity.LINKAGESYSTEMNO", "連携システム番号が不正です。");

            QhLinkageEntityWriter writer = new QhLinkageEntityWriter();
            QhLinkageEntityWriterArgs writerArgs = new QhLinkageEntityWriterArgs() { Data = new List<QH_LINKAGE_DAT>() { entity } };
            QhLinkageEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);


            if (writerResults.IsSuccess && writerResults.Result == 1)

                // 'ログテーブル書き込み
                // Dim logEntity As QH_PATIENTCARDHIST_LOG = Me.CreateLogEntity(entity, QH_PATIENTCARDHIST_LOG.HistTypeEnum.Added, actionDate, actionKey)
                // QsDbManager.WriteByCurrent(
                // New QhPatientCardHistEntityWriter(),
                // New QhPatientCardHistEntityWriterArgs() With {.Data = {logEntity}.ToList()}
                // )

                return true;
            else
                throw new InvalidOperationException("QH_LINKAGE_DATテーブルへの登録に失敗しました。");

        }

        /// <summary>
        /// 連携システム情報データテーブルエンティティを編集します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private bool UpdateLinkageSystemEntity(QH_LINKAGE_DAT entity, DateTime actionDate, Guid actionKey)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象の利用者カード情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (entity.LINKAGESYSTEMNO == int.MinValue)
                throw new ArgumentNullException("entity.LINKAGESYSTEMNO", "連携システム番号が不正です。");

            List<QH_LINKAGE_DAT> oldEntity = this.SelectLinkageSystemEntity(entity.LINKAGESYSTEMNO);
            // 編集対象のデータエンティティを取得
            if (oldEntity.Count == 1)
            {
                // 情報を更新、更新日時＝現在時刻
                entity.UPDATEDDATE = actionDate;
                QhLinkageEntityWriter writer = new QhLinkageEntityWriter();
                QhLinkageEntityWriterArgs writerArgs = new QhLinkageEntityWriterArgs() { Data = new List<QH_LINKAGE_DAT>() { entity } };
                QhLinkageEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);


                if (writerResults.IsSuccess && writerResults.Result == 1)

                    // 'ログテーブル書き込み
                    // Dim logEntity As QH_PATIENTCARDHIST_LOG = Me.CreateLogEntity(oldEntity.Item(0), QH_PATIENTCARDHIST_LOG.HistTypeEnum.Modified, actionDate, actionKey)
                    // QsDbManager.WriteByCurrent(
                    // New QhPatientCardHistEntityWriter(),
                    // New QhPatientCardHistEntityWriterArgs() With {.Data = {logEntity}.ToList()}
                    // )

                    return true;
                else
                    throw new InvalidOperationException("QH_LINKAGE_DATテーブルへの登録に失敗しました。");

            }
            else
                throw new InvalidOperationException("連携システム情報の更新に失敗しました。");
        }

        /// <summary>
        /// 連携システム情報データテーブルエンティティを削除します。
        /// </summary>
        /// <param name="entity">登録するテーブルエンティティ。</param>
        /// <param name="actionDate"></param>
        /// <param name="actionKey"></param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        private bool DeleteLinkageSystemEntity(QH_LINKAGE_DAT entity, DateTime actionDate, Guid actionKey)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "登録対象の利用者カード情報データテーブルエンティティがNull参照です。");
            if (entity.ACCOUNTKEY != this._accountKey)
                throw new ArgumentOutOfRangeException("entity.ACCOUNTKEY", "アカウントキーが不正です。");
            if (entity.LINKAGESYSTEMNO == int.MinValue)
                throw new ArgumentNullException("entity.LINKAGESYSTEMNO", "連携システム番号が不正です。");

            List<QH_LINKAGE_DAT> oldEntity = this.SelectLinkageSystemEntity(entity.LINKAGESYSTEMNO);
            // 削除対象のデータエンティティを取得
            if (oldEntity.Count == 1)
            {
                // 削除フラグを立てる、更新日時＝現在時刻
                oldEntity[0].DELETEFLAG = true;
                oldEntity[0].UPDATEDDATE = DateTime.Now;
                QhLinkageEntityWriter writer = new QhLinkageEntityWriter();
                QhLinkageEntityWriterArgs writerArgs = new QhLinkageEntityWriterArgs() { Data = oldEntity };
                QhLinkageEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);


                if (writerResults.IsSuccess && writerResults.Result == 1)

                    // 'ログテーブル書き込み
                    // Dim logEntity As QH_PATIENTCARDHIST_LOG = Me.CreateLogEntity(oldEntity.Item(0), QH_PATIENTCARDHIST_LOG.HistTypeEnum.Deleted, actionDate, actionKey)
                    // QsDbManager.WriteByCurrent(
                    // New QhPatientCardHistEntityWriter(),
                    // New QhPatientCardHistEntityWriterArgs() With {.Data = {logEntity}.ToList()}
                    // )

                    return true;
                else
                    throw new InvalidOperationException("QH_LINKAGE_DATテーブルへの登録に失敗しました。");

            }
            else
                throw new InvalidOperationException("連携システム情報の更新に失敗しました。");
        }


        #endregion

        #region "Public Method"
        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbPatientCardWriterCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">アカウントキー。</param>
        /// <remarks></remarks>
        public DbPatientCardWriterCore(Guid accountKey)
        {
            this._accountKey = accountKey;

            // TODO: アカウントキーの有効性をチェック

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }

        /// <summary>
        /// 利用者カード情報を登録します。
        /// </summary>
        /// <returns>
        /// 成功なら True、
        /// 重複判定できる失敗はTrueでErrorMessageを返却、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public bool WritePatientCardSet(Guid accountKey, int cardcode, int seq, Guid facilityKey, string patientCardSet,
            byte statusType, bool deleteflag, DateTime actionDate, Guid actionKey, ref string errorMessage, out QH_PATIENTCARD_DAT entity)
        {
            DateTime now = DateTime.Now;
            QhPatientCardSetOfJson cardset = new QsJsonSerializer().Deserialize<QhPatientCardSetOfJson>(patientCardSet);
            string cPSet = string.Empty;

            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                // 暗号化
                cPSet = string.IsNullOrWhiteSpace(patientCardSet) ? string.Empty : crypt.EncryptString(patientCardSet);
            }

            entity = new QH_PATIENTCARD_DAT()
            {
                ACCOUNTKEY = accountKey,
                CARDCODE = cardcode,
                SEQUENCE = seq,
                FACILITYKEY = facilityKey,
                CARDNO = cardset.CardNo,
                PATIENTCARDSET = cPSet,
                DELETEFLAG = deleteflag,
                CREATEDDATE = actionDate,
                UPDATEDDATE = actionDate
            };


            if (entity.CARDCODE == 0)
            {
                // その他カードの場合
                if (entity.DELETEFLAG)
                    return this.DeleteEntity(entity, actionDate, actionKey);
                else if (entity.SEQUENCE == int.MinValue)
                    return this.UpsertEntity(entity, actionDate, actionKey, ref errorMessage);
                else
                    return this.UpdateEntity(entity, actionDate, actionKey, ref errorMessage);
            }
            else
            {
                // 連携システムマスタにあるカードの場合
                QhPatientCardSetOfJson patientcardset = new QhPatientCardSetOfJson();
                try
                {
                    // デシリアライズ
                    patientcardset = new QsJsonSerializer().Deserialize<QhPatientCardSetOfJson>(patientCardSet);
                }
                catch
                {
                }

                string linkageSetString = string.Empty;
                // 連携システムマスタにあるカードの場合
                //try
                //{
                //}
                // デシリアライズ
                // linkageSetString = If(args.LinkageSet.Devices.Count = 0 And args.LinkageSet.Tags.Count = 0, String.Empty, New QsJsonSerializer().Serialize(Of QhLinkageSetOfJson)(args.LinkageSet))
                //catch
                //{
                //}

                // TODO:FACILITYKEY＝施設マスタが出来たら該当キーを入れる
                // TODO:STATUSTYPE　Enum定義にする　（仮）1:申請中　2:承認済
                // TODO:DATASET = 各システムにおいて、連携キー以外に持つ情報がある場合、JSONのセットを登録
                QH_LINKAGE_DAT linkageEntity = new QH_LINKAGE_DAT()
                {
                    ACCOUNTKEY = accountKey,
                    LINKAGESYSTEMNO = cardcode,
                    LINKAGESYSTEMID = patientcardset.CardNo,
                    DATASET = linkageSetString,
                    STATUSTYPE = statusType,
                    DELETEFLAG = deleteflag,
                    CREATEDDATE = actionDate,
                    UPDATEDDATE = actionDate
                };

                if (entity.DELETEFLAG)
                {
                    // カード削除
                    if (this.DeleteEntity(entity, actionDate, actionKey))
                        // 連携削除
                        return this.DeleteLinkageSystemEntity(linkageEntity, actionDate, actionKey);
                    else
                        // メッセージが必要ないのでFalseで返却。
                        // エラーメッセージが必要な場合はメッセージを返却してTrueに変更してください。

                        return false;
                }
                else if (entity.SEQUENCE == int.MinValue)
                {
                    if (this.UpsertEntity(entity, actionDate, actionKey, ref errorMessage) && string.IsNullOrWhiteSpace(errorMessage))
                        // カード登録成功
                        // 連携システムデータを登録
                        return this.UpsertLinkageSystemEntity(linkageEntity, actionDate, actionKey);
                    else
                        // カード登録失敗（重複）
                        // エラーメッセージ返却
                        return true;
                }
                else
                {    
                    // カード更新
                    if (this.UpdateEntity(entity, actionDate, actionKey, ref errorMessage))
                        return this.UpdateLinkageSystemEntity(linkageEntity, actionDate, actionKey);
                    else
                        return true;
                }
            }
        }

        #endregion


    }


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsApiCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// 利用者カードの情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbPatientCardReaderCore : QsDbReaderBase
    {
        /// <summary>
        /// アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _accountKey = Guid.Empty;

        /// <summary>
        /// 利用者カードクラスを表します。
        /// </summary>
        /// <remarks></remarks>
        internal sealed class PatientCardResult : QsDbEntityBase
        {

            /// <summary>
            ///     アカウントキーを取得または設定します。ACCOUNTKEY
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public Guid Col1 { get; set; } = Guid.Empty;

            /// <summary>
            ///     カード種別番号を取得または設定します。CARDCODE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public int Col2 { get; set; } = int.MinValue;

            /// <summary>
            ///     カード連番を取得または設定します。SEQUENCE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public int Col3 { get; set; } = int.MinValue;

            /// <summary>
            ///     組織番号を取得または設定します。FACILITYKEY
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public Guid Col4 { get; set; } = Guid.Empty;

            /// <summary>
            ///     利用者カード情報を取得または設定します。PATIENTCARDSET
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string Col5 { get; set; } = string.Empty;

            /// <summary>
            ///     連携状態を取得または設定します。STATUSTYPE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public byte Col6 { get; set; } = byte.MinValue;

            /// <summary>
            ///     作成日時を取得または設定します。CREATEDDATE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public DateTime Col7 { get; set; } = DateTime.MinValue;

            /// <summary>
            /// カード番号を取得または設定します。
            /// </summary>
            public string Col8 { get; set; } = string.Empty;

            public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
            {
                try
                {
                   
                    this.Col1 = reader.GetGuid(0);
                    this.Col2 = reader.GetInt32(1);
                    this.Col3 = reader.GetInt32(2);
                    this.Col4 = reader.GetGuid(3);
                    this.Col5 = reader.GetString(4);
                    this.Col6 = reader.IsDBNull(5) ? byte.MinValue : reader.GetByte(5);
                    this.Col7 = reader.GetDateTime(6);
                    this.Col8 = reader.GetString(7);
                    this.KeyGuid = Guid.NewGuid();
                    this.DataState = QsDbEntityStateTypeEnum.Unchanged;
                    this.IsEmpty = false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return this;
            }

            public override bool IsKeysValid()
            {
                return true;
            }

            /// <summary>
            ///     DB 用の利用者カードクラスへ変換します。
            ///     </summary>
            ///     <param name="target">変換元クラス。</param>
            ///     <returns>
            ///     DB 用の利用者カードクラス。
            ///     </returns>
            ///     <remarks></remarks>
            public DbPatientCardItem ToDbPatientCardItemWithStatus(PatientCardResult target)
            {
                if (target == null)
                    throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

                string ucPSet = string.Empty;

                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    // 復号化
                    ucPSet = string.IsNullOrWhiteSpace(target.Col5) ? string.Empty : crypt.DecryptString(target.Col5);
                }

                QhPatientCardSetOfJson cardset = new QsJsonSerializer().Deserialize<QhPatientCardSetOfJson>(ucPSet);

                return new DbPatientCardItem()
                {
                    CardCode = target.Col2,
                    CardNo =target.Col8,
                    Sequence = target.Col3,
                    FacilityKey =target.Col4,
                    CreatedDate = target.Col7,
                    PatientCardSet = ucPSet,
                    AttachedFileN = cardset.AttachedFileN.ConvertAll(i => { return new DbAttachedFileItem() { FileKey = i.FileKey.TryToValueType(Guid.Empty), OriginalName = string.Empty }; }),
                    StatusType = target.Col6
                };
            }
        }

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbPatientCardReaderCore()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// <see cref="DbPatientCardReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">アカウントキー。</param>
        /// <remarks></remarks>
        public DbPatientCardReaderCore(Guid accountKey) : base()
        {
            this._accountKey = accountKey;

            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }

        /// <summary>
        /// 利用者カードテーブルエンティティのリストを取得します。
        /// </summary>
        /// <returns>
        /// 該当するテーブルエンティティのリスト。
        /// </returns>
        /// <remarks></remarks>
        private List<PatientCardResult> SelectEntities()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", this._accountKey)
            };

                // クエリを作成
                
                query.Append("select ct.accountkey,ct.cardcode,ct.sequence,ct.facilitykey,ct.patientcardset,lt.statustype,ct.createddate,ct.cardno ");
                query.Append(" from (select * from qh_patientcard_dat  where accountkey = @p1");
                query.Append(" and deleteflag = 0) as ct");
                query.Append(" left outer join qh_linkage_dat as lt");
                query.Append(" on ct.cardcode = lt.linkagesystemno and lt.accountkey = @p1 and lt.deleteflag = 0");
                query.Append(" order by ct.createddate desc");
                query.Append(";");
                
                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<PatientCardResult>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
            }
        }


        /// <summary>
        /// カード種別番号、カード連番を指定して、
        /// 利用者カードデータテーブルエンティティを取得します。
        ///  2018/05/31 連携状態を取得するように修正
        /// </summary>
        /// <param name="cardCode">カード種別番号。</param>
        /// <param name="sequence">カード連番。</param>
        /// <returns>
        /// 該当するテーブルエンティティ。
        /// </returns>
        /// <remarks></remarks>
        private List<PatientCardResult> SelectEntity(int cardCode, int sequence)
        {
            if (cardCode == int.MinValue)
                throw new ArgumentOutOfRangeException("cardCode", "カード種別番号が不正です。");
            if (sequence == int.MinValue)
                throw new ArgumentOutOfRangeException("sequence", "カード連番が不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
            {
                this.CreateParameter(connection, "@p1", this._accountKey),
                this.CreateParameter(connection, "@p2", cardCode),
                this.CreateParameter(connection, "@p3", sequence)
            };


                // クエリを作成
                query.Append("select ct.accountkey,ct.cardcode,ct.sequence,ct.facilitykey,ct.patientcardset,lt.statustype,ct.createddate, ct.cardno");
                query.Append(" from (select * from qh_patientcard_dat  where accountkey = @p1");
                query.Append(" and cardcode = @p2");
                query.Append(" and sequence = @p3");
                query.Append(" and deleteflag = 0) as ct");
                query.Append(" left outer join qh_linkage_dat as lt");
                query.Append(" on ct.cardcode = lt.linkagesystemno and lt.accountkey = @p1 and lt.deleteflag = 0");
                query.Append(";");
               

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<PatientCardResult>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
            }
        }

        ///// <summary>
        ///// 連携システムマスタのリストを取得します。
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //private List<DbLinkageSystemMasterItem> GetMasterList()
        //{
        //    List<DbLinkageSystemMasterItem> results = new List<DbLinkageSystemMasterItem>();

        //    using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGESYSTEM_MST>())
        //    {
        //        StringBuilder query = new StringBuilder();
        //        List<DbParameter> @params = new List<DbParameter>()
        //    {
        //        this.CreateParameter(connection, "@p1", 0)
        //    };

        //        // クエリを作成
        //        
        //            query.Append("select * from qh_linkagesystem_mst");
        //            query.Append(" where deleteflag = @p1");
        //            query.Append(" and displayflag = 1");
        //            query.Append(" and prefno <> 47");
        //            query.Append(" order by disporder ;");
        //       
        //        // コネクションオープン
        //        connection.Open();

        //        // クエリを実行
        //        List<QH_LINKAGESYSTEM_MST> masterData = this.ExecuteReader<QH_LINKAGESYSTEM_MST>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
        //        foreach (QH_LINKAGESYSTEM_MST item in masterData)
        //            results.Add(new DbLinkageSystemMasterItem()
        //            {
        //                LinkageSystemNo = item.LINKAGESYSTEMNO,
        //                PrefNo = item.PREFNO,
        //                LinkageName = item.LINKAGENAME,
        //                LinkageNote = item.LINKAGENOTE,
        //                LinkageSet = item.LINKAGESET,
        //                DispOrder = item.DISPORDER
        //            });
        //    }

        //    return results;
        //}

        /// <summary>
        /// DisplayFlagを考慮して、
        /// 利用者カードテーブルエンティティのリストを取得します。
        /// </summary>
        /// <returns>
        /// 該当するテーブルエンティティのリスト。
        /// </returns>
        /// <remarks></remarks>
        private List<PatientCardResult> SelectEntitiesConsideringDisplayFlag()
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@p1", this._accountKey)
                };

                // クエリを作成
                
                query.Append("select t4.accountkey, t4.cardcode, t4.sequence, t4.facilitykey,");
                query.Append(" t4.patientcardset, t4.statustype, t4.createddate,t4.cardno");
                query.Append(" from (");
                query.Append(" select t1.accountkey, t1.cardcode, t1.sequence, t1.facilitykey,");
                query.Append("  t1.patientcardset, t3.statustype, t1.createddate, t1.cardno, t2.displayflag");
                query.Append(" from qh_patientcard_dat as t1");
                query.Append(" left outer join qh_linkagesystem_mst as t2");
                query.Append("  on t1.cardcode = t2.linkagesystemno and t2.deleteflag = 0");
                query.Append(" left outer join qh_linkage_dat as t3");
                query.Append("  on t1.cardcode = t3.linkagesystemno");
                query.Append("  and t3.accountkey = @p1 and t3.deleteflag = 0");
                query.Append(" where t1.accountkey = @p1 and t1.deleteflag = 0");
                query.Append(" ) as t4");
                query.Append(" where (t4.cardcode = 0 and t4.displayflag is null) or (t4.cardcode > 0 and t4.displayflag = 1)");
                query.Append(" order by t4.createddate desc");
                query.Append(";");
                

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<PatientCardResult>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
            }
        }


        /// <summary>
        /// DB 用の利用者カードクラスへ変換します。
        /// </summary>
        /// <param name="target">変換元クラス。</param>
        /// <returns>
        /// DB 用の利用者カードクラス。
        /// </returns>
        /// <remarks></remarks>
        public DbPatientCardItem ToDbPatientCardItemWithStatus(PatientCardResult target)
        {
            if (target == null)
                throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            string ucPSet = string.Empty;

            // ucPSet = target.Col5

            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                // 復号化
                ucPSet = string.IsNullOrWhiteSpace(target.Col5) ? string.Empty : crypt.DecryptString(target.Col5);
            }

            QhPatientCardSetOfJson cardset = new QsJsonSerializer().Deserialize<QhPatientCardSetOfJson>(ucPSet);

            return new DbPatientCardItem()
            {
                CardCode = target.Col2,
                CardNo =target.Col8,
                Sequence = target.Col3,
                FacilityKey =target.Col4 ,
                CreatedDate = target.Col7,
                PatientCardSet = ucPSet,
                AttachedFileN = cardset.AttachedFileN.ConvertAll(i => {
                    return new DbAttachedFileItem()
                    {
                        FileKey = i.FileKey.TryToValueType(Guid.Empty),
                        OriginalName = string.Empty
                    };
                    }),
                StatusType = target.Col6
            };
        }


        /// <summary>
        /// 利用者カードのリストを取得します。
        /// </summary>
        /// <returns>
        /// 利用者カードのリスト。
        /// </returns>
        /// <remarks></remarks>
        public List<DbPatientCardItem> ReadPatientCardList()
        {
            return this.SelectEntities().ConvertAll(new Converter<PatientCardResult, DbPatientCardItem>(ToDbPatientCardItemWithStatus));
        }

        /// <summary>
        /// カード種別番号、カード連番を指定して、利用者カードの情報を取得します。
        /// </summary>
        /// <param name="cardCode">カード種別番号。</param>
        /// <param name="sequence">カード連番。</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DbPatientCardItem ReadPatientCardEntity(int cardCode, int sequence)
        {
            return this.SelectEntity(cardCode, sequence).ConvertAll(new Converter<PatientCardResult, DbPatientCardItem>(ToDbPatientCardItemWithStatus)).First();
        }

        ///// <summary>
        ///// 連携システムマスタのリストを取得します。
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public List<DbLinkageSystemMasterItem> ReadLinkageSystemMasterList()
        //{
        //    return this.GetMasterList();
        //}

        /// <summary>
        /// DisplayFlagを考慮して、
        /// 利用者カードのリストを取得します。
        /// </summary>
        /// <returns>
        /// 利用者カードのリスト。
        /// </returns>
        /// <remarks></remarks>
        public List<DbPatientCardItem> ReadPatientCardListConsideringDisplayFlag()
        {
            return this.SelectEntitiesConsideringDisplayFlag().ConvertAll(new Converter<PatientCardResult, DbPatientCardItem>(ToDbPatientCardItemWithStatus));
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsApiCoreV1;


namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// グループへのお知らせの情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbNoticeGroupReaderCore : QsDbReaderBase
    {
        /// <summary>
        /// アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _accountKey = Guid.Empty;

        /// <summary>
        /// 取得対象の施設キー一覧を保持します。
        /// </summary>
        private List<Guid> _facilityKeyN = new List<Guid>() { Guid.Empty };

        /// <summary>
        /// グループへのお知らせクラスを表します。
        /// </summary>
        /// <remarks></remarks>
        internal sealed class NoticeGroupResult : QsDbEntityBase
        {

            /// <summary>
            ///     お知らせ番号を取得または設定します。NOTICENO
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public long Col1 { get; set; } = long.MinValue;

            /// <summary>
            ///     タイトルを取得または設定します。TITLE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string Col2 { get; set; } = string.Empty;

            /// <summary>
            ///     内容を取得または設定します。CONTENTS
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string Col3 { get; set; } = string.Empty;

            /// <summary>
            ///     お知らせカテゴリを取得または設定します。CATEGORYNO
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public byte Col4 { get; set; } = byte.MinValue;

            /// <summary>
            ///     優先度を取得または設定します。PRIORITYNO
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public byte Col5 { get; set; } = byte.MinValue;

            /// <summary>
            ///     対象タイプを取得または設定します。TARGETTYPE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public byte Col6 { get; set; } = byte.MinValue;

            /// <summary>
            ///     開始日を取得または設定します。STARTDATE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public DateTime Col7 { get; set; } = DateTime.MinValue;

            /// <summary>
            ///     終了日を取得または設定します。ENDDATE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public DateTime Col8 { get; set; } = DateTime.MinValue;

            /// <summary>
            ///     お知らせデータセットを取得または設定します。NOTICEDATASET
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string Col9 { get; set; } = string.Empty;

            /// <summary>
            ///     既読フラグを取得または設定します。ALREADYREADFLAG
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public bool Col10 { get; set; } = false;

            /// <summary>
            ///     既読日時を取得または設定します。ALREADYREADDATE
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public DateTime Col11 { get; set; } = DateTime.MinValue;

            public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
            {
                try
                {
                    // SQLのSELECT列順に対応して結果を取り込む。
                    this.Col1 = reader.GetInt64(0);
                    this.Col2 = reader.GetString(1);
                    this.Col3 = reader.GetString(2);
                    this.Col4 = reader.GetByte(3);
                    this.Col5 = reader.GetByte(4);
                    this.Col6 = reader.GetByte(5);
                    this.Col7 = reader.GetDateTime(6);
                    this.Col8 = reader.GetDateTime(7);
                    this.Col9 = reader.GetString(8);
                    // 左外部結合先は未登録の可能性があるためNULLを考慮する。
                    this.Col10 = reader.IsDBNull(9) ? false : reader.GetBoolean(9);
                    this.Col11 = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10);
                    // 取得結果としてのエンティティ状態へ初期化する。
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
        }

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbNoticeGroupReaderCore()
        {
        }

        /// <summary>
        /// お知らせ番号とアカウントキーを指定して、
        /// <see cref="DbNoticeGroupReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="accountKey">アカウントキー。</param>
        /// <param name="facilityKeyN">施設キー一覧。</param>
        /// <remarks></remarks>
        public DbNoticeGroupReaderCore(Guid accountKey, IEnumerable<Guid> facilityKeyN = null) : base()
        {
            this._accountKey = accountKey;
            this._facilityKeyN = (facilityKeyN ?? Enumerable.Empty<Guid>())
                .Where(x => x != Guid.Empty)
                .Distinct()
                .Prepend(Guid.Empty)
                .ToList();

            // 既読情報の結合に使用するため、空Guidは受け付けない。
            if (this._accountKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("accountKey", "アカウントキーが不正です。");
        }

        /// <summary>
        /// グループへのお知らせテーブルエンティティを取得します。
        /// </summary>
        /// <returns>
        /// 該当するテーブルエンティティ。
        /// </returns>
        /// <remarks></remarks>
        private NoticeGroupResult SelectEntity(long noticeNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_NOTICEGROUP_DAT>())
            {
                StringBuilder query = new StringBuilder();
                // 取得対象は Joto Native 系（iOS/Android/共通）かつ全体向け/連携施設向け通知に限定する。
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@p1", noticeNo),
                    this.CreateParameter(connection, "@p2", this._accountKey),
                    this.CreateParameter(connection, "@p3", (byte)QsApiSystemTypeEnum.JotoNativeiOSApp),
                    this.CreateParameter(connection, "@p4", (byte)QsApiSystemTypeEnum.JotoNativeAndroidApp),
                    this.CreateParameter(connection, "@p5", (byte)QsApiSystemTypeEnum.JotoNative),
                    this.CreateParameter(connection, "@p6", DateTime.Now)
                };

                var facilityParamN = new List<string>();
                for (int i = 0; i < this._facilityKeyN.Count; i++)
                {
                    string facilityParamName = string.Format("@f{0}", i);
                    facilityParamN.Add(facilityParamName);
                    @params.Add(this.CreateParameter(connection, facilityParamName, this._facilityKeyN[i]));
                }

                // クエリを作成
                query.Append("select ");
                query.Append("  t1.noticeno, t1.title, t1.contents, t1.categoryno, t1.priorityno,");
                query.Append("  t1.targettype, t1.startdate, t1.enddate, t1.noticedataset,");
                query.Append("  t2.alreadyreadflag, t2.alreadyreaddate");
                query.Append(" from qh_noticegroup_dat as t1");
                query.Append(" left outer join qh_noticegrouptarget_dat as t2");
                query.Append("  on t1.noticeno = t2.noticeno and t2.accountkey = @p2 and t2.deleteflag = 0");
                // 単体取得でも一覧取得と同じ配信対象条件を適用する。
                query.Append(" where t1.noticeno = @p1 and t1.deleteflag = 0");
                query.Append("  and t1.tosystemtype in (@p3, @p4, @p5)");
                query.AppendFormat("  and t1.facilitykey in ({0})", string.Join(",", facilityParamN));
                query.Append("  and t1.startdate <= @p6 and @p6 <= t1.enddate");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                List<NoticeGroupResult> results = this.ExecuteReader<NoticeGroupResult>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
                return results.Count > 0 ? results.First() : null;
            }
        }

        /// <summary>
        /// 対象タイプとカテゴリ番号を指定してグループへのお知らせテーブルエンティティのリストを取得します。
        /// </summary>
        /// <param name="targetType">対象タイプ。</param>
        /// <param name="categoryNo">カテゴリ番号。</param>
        /// <param name="alreadyRead">既読状態。</param>
        /// <param name="pageIndex">ページ番号。</param>
        /// <param name="pageSize">ページサイズ。</param>
        /// <returns>
        /// 該当するテーブルエンティティのリスト。
        /// </returns>
        /// <remarks></remarks>
        private List<NoticeGroupResult> SelectEntities(byte targetType, byte categoryNo, byte alreadyRead, int pageIndex, int pageSize)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_NOTICEGROUP_DAT>())
            {
                StringBuilder query = new StringBuilder();
                // 取得対象は Joto Native 系（iOS/Android/共通）かつ全体向け/連携施設向け通知に限定する。
                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@p2", this._accountKey),
                    this.CreateParameter(connection, "@p7", (byte)QsApiSystemTypeEnum.JotoNativeiOSApp),
                    this.CreateParameter(connection, "@p8", (byte)QsApiSystemTypeEnum.JotoNativeAndroidApp),
                    this.CreateParameter(connection, "@p9", (byte)QsApiSystemTypeEnum.JotoNative),
                    this.CreateParameter(connection, "@p10", DateTime.Now)
                };

                var facilityParamN = new List<string>();
                for (int i = 0; i < this._facilityKeyN.Count; i++)
                {
                    string facilityParamName = string.Format("@f{0}", i);
                    facilityParamN.Add(facilityParamName);
                    @params.Add(this.CreateParameter(connection, facilityParamName, this._facilityKeyN[i]));
                }

                bool hasTargetType = targetType != (byte)QsDbNoticeGroupTargetTypeEnum.None;
                if (hasTargetType)
                {
                    // 指定時のみ対象タイプ条件を追加する。
                    @params.Add(this.CreateParameter(connection, "@p1", targetType));
                }

                // API引数のカテゴリ値をDB値へ変換する。
                // 0=すべて(条件なし), 1=お知らせ(DB:0), 2=メンテナンス(DB:1)
                byte dbCategoryNo = byte.MinValue;
                bool hasCategoryNo = false;
                switch (categoryNo)
                {
                    case 1:
                        dbCategoryNo = 0;
                        hasCategoryNo = true;
                        break;
                    case 2:
                        dbCategoryNo = 1;
                        hasCategoryNo = true;
                        break;
                }
                if (hasCategoryNo)
                {
                    @params.Add(this.CreateParameter(connection, "@p3", dbCategoryNo));
                }

                bool hasAlreadyRead = alreadyRead != byte.MinValue;
                if (hasAlreadyRead)
                {
                    // 既読管理は個人通知(targettype=Individual)のみ対象。
                    @params.Add(this.CreateParameter(connection, "@p4", (byte)QsDbNoticeGroupTargetTypeEnum.Individual));
                }

                bool hasPaging = pageIndex > 0 && pageSize > 0;
                if (hasPaging)
                {
                    // row_number用の開始/終了行番号を算出する。
                    int startRowNo = ((pageIndex - 1) * pageSize) + 1;
                    int endRowNo = pageIndex * pageSize;
                    @params.Add(this.CreateParameter(connection, "@p5", startRowNo));
                    @params.Add(this.CreateParameter(connection, "@p6", endRowNo));
                }

                // クエリを作成
                if (hasPaging)
                {
                    // ページング時はrow_numberで範囲抽出する。
                    query.Append("select ");
                    query.Append("  t.noticeno, t.title, t.contents, t.categoryno, t.priorityno,");
                    query.Append("  t.targettype, t.startdate, t.enddate, t.noticedataset,");
                    query.Append("  t.alreadyreadflag, t.alreadyreaddate");
                    query.Append(" from (");
                    query.Append("  select ");
                    query.Append("    t1.noticeno, t1.title, t1.contents, t1.categoryno, t1.priorityno,");
                    query.Append("    t1.targettype, t1.startdate, t1.enddate, t1.noticedataset,");
                    query.Append("    t2.alreadyreadflag, t2.alreadyreaddate,");
                    query.Append("    row_number() over (order by t1.startdate desc, t1.noticeno desc) as rowno");
                    query.Append("  from qh_noticegroup_dat as t1");
                    query.Append("  left outer join qh_noticegrouptarget_dat as t2");
                    query.Append("   on t1.noticeno = t2.noticeno and t2.accountkey = @p2 and t2.deleteflag = 0");
                    query.Append("  where t1.deleteflag = 0");
                    query.Append("   and t1.tosystemtype in (@p7, @p8, @p9)");
                    query.AppendFormat("   and t1.facilitykey in ({0})", string.Join(",", facilityParamN));
                    query.Append("   and t1.startdate <= @p10 and @p10 <= t1.enddate");
                }
                else
                {
                    // 非ページング時は通常の並び順で全件取得する。
                    query.Append("select ");
                    query.Append("  t1.noticeno, t1.title, t1.contents, t1.categoryno, t1.priorityno,");
                    query.Append("  t1.targettype, t1.startdate, t1.enddate, t1.noticedataset,");
                    query.Append("  t2.alreadyreadflag, t2.alreadyreaddate");
                    query.Append(" from qh_noticegroup_dat as t1");
                    query.Append(" left outer join qh_noticegrouptarget_dat as t2");
                    query.Append("  on t1.noticeno = t2.noticeno and t2.accountkey = @p2 and t2.deleteflag = 0");
                    query.Append(" where t1.deleteflag = 0");
                    query.Append("  and t1.tosystemtype in (@p7, @p8, @p9)");
                    query.AppendFormat("  and t1.facilitykey in ({0})", string.Join(",", facilityParamN));
                    query.Append("  and t1.startdate <= @p10 and @p10 <= t1.enddate");
                }
                if (hasTargetType)
                {
                    query.Append(" and t1.targettype = @p1");
                }
                if (hasCategoryNo)
                {
                    // 変換後カテゴリで絞り込む。
                    query.Append(" and t1.categoryno = @p3");
                }
                if (hasAlreadyRead)
                {
                    // alreadyRead=1:未読のみ、alreadyRead=2:既読のみを抽出する。
                    // targettype=All(1) は既読管理対象外のため、未読/既読どちらにも含めない。
                    // レコード未作成(null)も未読/既読のどちらにも含めない。
                    if (alreadyRead == 1)
                    {
                        query.Append(" and t1.targettype = @p4 and t2.alreadyreadflag = 0");
                    }
                    else if (alreadyRead == 2)
                    {
                        query.Append(" and t1.targettype = @p4 and t2.alreadyreadflag = 1");
                    }
                }
                if (hasPaging)
                {
                    // row_numberの範囲指定でページングし、rowno順で返す。
                    query.Append(" ) as t");
                    query.Append(" where t.rowno between @p5 and @p6");
                    query.Append(" order by t.rowno");
                }
                else
                {
                    // 最新日付・通知番号の降順で返す。
                    query.Append(" order by t1.startdate desc, t1.noticeno desc");
                }
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<NoticeGroupResult>(connection, null, this.CreateCommandText(connection, query.ToString()), @params).ToList();
            }
        }

        /// <summary>
        /// DB 用のグループへのお知らせクラスへ変換します。
        /// </summary>
        /// <param name="target">変換元クラス。</param>
        /// <returns>
        /// DB 用のグループへのお知らせクラス。
        /// </returns>
        /// <remarks></remarks>
        public DbNoticeGroupItem ToDbNoticeGroupItem(NoticeGroupResult target)
        {
            if (target == null)
                throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            // 読み取り用の中間結果を公開用モデルへ詰め替える。
            return new DbNoticeGroupItem()
            {
                NoticeNo = target.Col1,
                Title = target.Col2,
                Contents = target.Col3,
                CategoryNo = target.Col4,
                PriorityNo = target.Col5,
                TargetType = target.Col6,
                StartDate = target.Col7,
                EndDate = target.Col8,
                NoticeDataSet = target.Col9,
                AlreadyReadFlag = target.Col10,
                AlreadyReadDate = target.Col11
            };
        }

        /// <summary>
        /// お知らせ（グループ）の情報を取得します。
        /// </summary>
        /// <returns>
        /// グループへのお知らせの情報。
        /// </returns>
        /// <remarks></remarks>
        public DbNoticeGroupItem ReadNoticeGroupEntity(long noticeNo)
        {
            // 単体取得は該当がなければnullを返す。
            NoticeGroupResult result = this.SelectEntity(noticeNo);
            return result != null ? this.ToDbNoticeGroupItem(result) : null;
        }

        /// <summary>
        /// 対象タイプとカテゴリ番号を指定してお知らせ（グループ）のリストを取得します。
        /// </summary>
        /// <param name="targetType">対象タイプ。</param>
        /// <param name="categoryNo">カテゴリ番号。</param>
        /// <param name="alreadyRead">既読状態。</param>
        /// <param name="pageIndex">ページ番号。</param>
        /// <param name="pageSize">ページサイズ。</param>
        /// <returns>
        /// グループへのお知らせのリスト。
        /// </returns>
        /// <remarks></remarks>
        public List<DbNoticeGroupItem> ReadNoticeGroupList(byte targetType, byte categoryNo, byte alreadyRead, int pageIndex, int pageSize)
        {
            // 一覧取得結果を公開用モデルへ変換して返す。
            return this.SelectEntities(targetType, categoryNo, alreadyRead, pageIndex, pageSize).ConvertAll(x => this.ToDbNoticeGroupItem(x));
        }
    }

}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsOpenApi.Repositories;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// アプリイベント情報をデータベースから取得する機能を提供します。
    /// </summary>
    internal sealed class DbAppEventReaderCore : QsDbReaderBase
    {
        /// <summary>
        /// SQL結果を受けるための内部クラスです。
        /// </summary>
        internal sealed class AppEventResult : QsDbEntityBase
        {
            public Guid Col1 { get; set; } = Guid.Empty;
            public byte Col2 { get; set; } = byte.MinValue;
            public string Col3 { get; set; } = string.Empty;
            public int Col4 { get; set; } = int.MinValue;
            public string Col5 { get; set; } = string.Empty;
            public string Col6 { get; set; } = string.Empty;
            public DateTime Col7 { get; set; } = DateTime.MinValue;
            public DateTime Col8 { get; set; } = DateTime.MinValue;
            public DateTime Col9 { get; set; } = DateTime.MinValue;
            public DateTime Col10 { get; set; } = DateTime.MinValue;
            public string Col11 { get; set; } = string.Empty;

            public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
            {
                this.Col1 = reader.GetGuid(0);
                this.Col2 = reader.GetByte(1);
                this.Col3 = reader.GetString(2);
                this.Col4 = reader.GetInt32(3);
                this.Col5 = reader.GetString(4);
                this.Col6 = reader.GetString(5);
                this.Col7 = reader.GetDateTime(6);
                this.Col8 = reader.GetDateTime(7);
                this.Col9 = reader.GetDateTime(8);
                this.Col10 = reader.GetDateTime(9);
                this.Col11 = reader.GetString(10);
                this.KeyGuid = Guid.NewGuid();
                this.DataState = QsDbEntityStateTypeEnum.Unchanged;
                this.IsEmpty = false;
                return this;
            }

            public override bool IsKeysValid()
            {
                return true;
            }
        }

        /// <summary>
        /// アプリイベント一覧を取得します。
        /// </summary>
        public List<DbAppEventItem> ReadAppEventList(int pageIndex, int pageSize, Guid accountKey)
        {
            // 現在時刻時点で公開中のイベントを取得する。
            List<AppEventResult> data = this.SelectEntities(DateTime.Now, pageIndex, pageSize, accountKey);
            return data.ConvertAll(this.ToDbAppEventItem);
        }

        /// <summary>
        /// イベントキーでアプリイベント詳細を取得します。
        /// </summary>
        public DbAppEventItem ReadAppEventEntity(Guid eventKey)
        {
            AppEventResult data = this.SelectEntity(eventKey);
            return data == null ? null : this.ToDbAppEventItem(data);
        }

        private AppEventResult SelectEntity(Guid eventKey)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_APPEVENT_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> parameters = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@p1", eventKey)
                };

                query.Append("select ");
                query.Append("  t1.eventkey, t1.apptype, t1.eventcode, t1.linkagesystemno, t1.title,");
                query.Append("  t1.contents, t1.startdate, t1.enddate, t1.publishstartdate, t1.publishenddate, t1.appeventset");
                query.Append(" from qh_appevent_dat as t1");
                query.Append(" where t1.eventkey = @p1");
                query.Append("  and t1.deleteflag = 0;");

                connection.Open();
                List<AppEventResult> results = this.ExecuteReader<AppEventResult>(connection, null, this.CreateCommandText(connection, query.ToString()), parameters).ToList();
                return results.Count > 0 ? results.First() : null;
            }
        }

        private List<AppEventResult> SelectEntities(DateTime now, int pageIndex, int pageSize, Guid accountKey)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_APPEVENT_DAT>())
            {
                List<int> linkageSystemNoList = new List<int>();
                try
                {
                    var linkageRepo = new LinkageRepository();
                    linkageSystemNoList = linkageRepo.ReadLinkageSystemNoListByAccountKey(accountKey);
                }
                catch (Exception ex)
                {
                    QoAccessLog.WriteErrorLog(ex.Message, accountKey);
                    linkageSystemNoList = new List<int>();
                }

                // 共通イベント(linkagesystemno=0)は常に対象に含める。
                if (!linkageSystemNoList.Contains(0))
                {
                    linkageSystemNoList.Add(0);
                }

                linkageSystemNoList = linkageSystemNoList.Distinct().ToList();

                if (linkageSystemNoList.Count == 0)
                {
                    return new List<AppEventResult>();
                }

                StringBuilder query = new StringBuilder();
                List<DbParameter> parameters = new List<DbParameter>();

                string linkageSystemNoInClause = string.Join(",", linkageSystemNoList);

                parameters.Add(this.CreateParameter(connection, "@p1", (byte)QsApiSystemTypeEnum.JotoNativeiOSApp));
                parameters.Add(this.CreateParameter(connection, "@p2", (byte)QsApiSystemTypeEnum.JotoNativeAndroidApp));
                parameters.Add(this.CreateParameter(connection, "@p3", (byte)QsApiSystemTypeEnum.JotoNative));
                parameters.Add(this.CreateParameter(connection, "@p4", now));

                bool hasPaging = pageIndex > 0 && pageSize > 0;
                if (hasPaging)
                {
                    // row_numberの範囲指定に使用する開始/終了行を算出する。
                    int startRowNo = ((pageIndex - 1) * pageSize) + 1;
                    int endRowNo = pageIndex * pageSize;
                    parameters.Add(this.CreateParameter(connection, "@p5", startRowNo));
                    parameters.Add(this.CreateParameter(connection, "@p6", endRowNo));
                }

                if (hasPaging)
                {
                    // ページング時はrow_numberで対象範囲のみ抽出する。
                    query.Append("select ");
                    query.Append("  t.eventkey, t.apptype, t.eventcode, t.linkagesystemno, t.title,");
                    query.Append("  t.contents, t.startdate, t.enddate, t.publishstartdate, t.publishenddate, t.appeventset");
                    query.Append(" from (");
                    query.Append("  select ");
                    query.Append("    t1.eventkey, t1.apptype, t1.eventcode, t1.linkagesystemno, t1.title,");
                    query.Append("    t1.contents, t1.startdate, t1.enddate, t1.publishstartdate, t1.publishenddate, t1.appeventset,");
                    query.Append("    row_number() over (order by t1.publishstartdate desc, t1.startdate desc, t1.eventkey desc) as rowno");
                    query.Append("  from qh_appevent_dat as t1");
                    query.Append("  where t1.deleteflag = 0");
                    query.Append($"   and t1.linkagesystemno in ({linkageSystemNoInClause})");
                    query.Append("   and t1.apptype in (@p1, @p2, @p3)");
                    query.Append("   and t1.publishstartdate <= @p4");
                    query.Append("   and t1.publishenddate >= @p4");
                    query.Append(" ) as t");
                    query.Append(" where t.rowno between @p5 and @p6");
                    query.Append(" order by t.rowno;");
                }
                else
                {
                    // 非ページング時は従来どおり全件を並び順付きで取得する。
                    query.Append("select ");
                    query.Append("  t1.eventkey, t1.apptype, t1.eventcode, t1.linkagesystemno, t1.title,");
                    query.Append("  t1.contents, t1.startdate, t1.enddate, t1.publishstartdate, t1.publishenddate, t1.appeventset");
                    query.Append(" from qh_appevent_dat as t1");
                    query.Append(" where t1.deleteflag = 0");
                    query.Append($"  and t1.linkagesystemno in ({linkageSystemNoInClause})");
                    query.Append("  and t1.apptype in (@p1, @p2, @p3)");
                    query.Append("  and t1.publishstartdate <= @p4");
                    query.Append("  and t1.publishenddate >= @p4");
                    query.Append(" order by t1.publishstartdate desc, t1.startdate desc, t1.eventkey desc;");
                }

                connection.Open();
                return this.ExecuteReader<AppEventResult>(connection, null, this.CreateCommandText(connection, query.ToString()), parameters).ToList();
            }
        }

        private DbAppEventItem ToDbAppEventItem(AppEventResult target)
        {
            // DB読み取り用の中間結果を公開用モデルに詰め替える。
            return new DbAppEventItem()
            {
                EventKey = target.Col1,
                AppType = target.Col2,
                EventCode = target.Col3,
                LinkageSystemNo = target.Col4,
                Title = target.Col5,
                Contents = target.Col6,
                StartDate = target.Col7,
                EndDate = target.Col8,
                PublishStartDate = target.Col9,
                PublishEndDate = target.Col10,
                AppEventSet = target.Col11
            };
        }
    }
}

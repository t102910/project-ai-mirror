using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// バイタル警告の内容を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class HealthRecordAlertReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, HealthRecordAlertReaderArgs, HealthRecordAlertReaderResults>
    {
        /// <summary>
        /// バイタル警告の情報取得用Entity
        /// </summary>
        /// <remarks></remarks>
        internal sealed class AlertTargetResult : QsDbEntityBase
        {
            public Guid ACCOUNTKEY { get; set; } = Guid.Empty;
            
            public string ENTRYADDITION { get; set; } = string.Empty ;

            public DateTime RECORDDATE { get; set; } = DateTime.MinValue;

            public decimal VALUE1 { get; set; } = decimal.MinValue;

            public decimal VALUE2 { get; set; } = decimal.MinValue;

            public long ALERTNO { get; set; }=long.MinValue ;

            public string MESSAGE { get; set; } = string.Empty;

            
            public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
            {
                try
                {

                    this.ACCOUNTKEY  = reader.GetGuid(0);
                    this.ENTRYADDITION  = reader.GetString(1);
                    this.RECORDDATE  = reader.GetDateTime(2);
                    this.VALUE1  = reader.GetDecimal(3);
                    this.VALUE2 = reader.GetDecimal(4);
                    this.ALERTNO = reader.GetInt64(5);
                    this.MESSAGE = reader.GetString(6);
                    
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
        /// <see cref="HealthRecordAlertReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public HealthRecordAlertReader() : base()
        {
        }


        /// <summary>
        /// アラート情報を返す。（リストだが１件しか返らない想定）
        /// </summary>
        /// <returns></returns>
        private List<AlertTargetResult> Select(int linkageSystemNo, DateTime startDate, DateTime endDate, long alertNo ,byte vitalType)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_HEALTHRECORDALERT_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@p1", linkageSystemNo ),
                    this.CreateParameter(connection, "@p2", startDate ),
                    this.CreateParameter(connection, "@p3", endDate ),
                    this.CreateParameter(connection, "@p4", alertNo ),
                    this.CreateParameter(connection, "@p5", vitalType ),
                    this.CreateParameter(connection, "@p6", Guid.Parse("CDF50EC6-DA20-4D47-84DE-6F14BF9CEC1F")) /*心臓見守り*/
                };

                // クエリを作成                
                query.Append("SELECT et.ACCOUNTKEY, et.ENTRYADDITION, hra.RECORDDATE, hra.VALUE1, hra.VALUE2, hra.ALERTNO, hra.MESSAGE ");
                query.Append(" FROM QH_ENTRYCHALLENGE_DAT AS et INNER JOIN QH_HEALTHRECORDALERT_DAT AS hra ON et.ACCOUNTKEY = hra.ACCOUNTKEY ");
                query.Append(" WHERE (et.CHALLENGEKEY = @p6 ) AND (et.ENDDATE > @p3) AND (et.DELETEFLAG = 0) AND (hra.DELETEFLAG = 0)");
                if(alertNo >0)
                    query.Append(" AND (hra.ALERTNO=@p4) ;");
                else
                    query.Append(" AND (hra.LINKAGESYSTEMNO = @p1 ) AND (hra.UPDATEDDATE BETWEEN @p2 AND @p3) AND (hra.VITALTYPE=@p5);");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<AlertTargetResult>(connection, null, query.ToString(), @params);

            }
        }

        /// <summary>
        ///     DB 用のDbVitalAlertItemクラスへ変換します。
        ///     </summary>
        ///     <param name="target">変換元クラス。</param>
        ///     <returns>
        ///     DB 用の利用者カードクラス。
        ///     </returns>
        ///     <remarks></remarks>
        private DbVitalAlertItem ToDbVitalAlertItem(AlertTargetResult target)
        {
            if (target == null)
                throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            QhChallengeEntryAdditionOfJson dataset = new QsJsonSerializer().Deserialize<QhChallengeEntryAdditionOfJson>(target.ENTRYADDITION);
            Dictionary<string, string> dic = dataset.Required.ToDictionary(s => s.Key, s => s.Value);
            return new DbVitalAlertItem()
            {
                AlertNo = target.ALERTNO,
                AccountKey = target.ACCOUNTKEY,
                Name = dic["Name"],
                FamilyTel = dic["FamilyPhoneNumber"],
                Mail = dic["Mail"],
                Message = target.MESSAGE,
                RecordDate = target.RECORDDATE,
                Tel = dic["PhoneNumber"],
                Value1 = target.VALUE1,
                Value2 = target.VALUE2,
                Address =dic["Address"],
                FamilyName=dic["Family"],
                FamilyRelationship=dic["FamilyRelationship"]
            };
        }
        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public HealthRecordAlertReaderResults ExecuteByDistributed(HealthRecordAlertReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            HealthRecordAlertReaderResults result = new HealthRecordAlertReaderResults() { IsSuccess = false };
            result.AlertList = this.Select(args.LinkageSystemNo,args.TargetStartRecordDate,args.TargetEndRecordDate,args.AlertNo ,args.VitalType)
                .ConvertAll(new Converter<AlertTargetResult, DbVitalAlertItem>(ToDbVitalAlertItem));

            result.IsSuccess = true;
            return result;
        }
    }


}

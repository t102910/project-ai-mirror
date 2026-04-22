using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// プッシュ通知対象アカウントキーと通知元施設キーを、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class NotificationTargetFromPatientIdReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, NotificationTargetFromPatientIdReaderArgs, NotificationTargetFromPatientIdReaderResults>
    {
        /// <summary>
        /// プッシュ通知対象アカウントキーと通知元施設キーを表します。
        /// </summary>
        /// <remarks></remarks>
        internal sealed class TargetFromPatientId : QsDbEntityBase
        {

            /// <summary>
            ///     ACCOUNTKEY を取得または設定します。
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public Guid ACCOUNTKEY { get; set; } = Guid.Empty;

            /// <summary>
            ///     FACILITYKEY を取得または設定します。
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public Guid FACILITYKEY { get; set; } = Guid.Empty;

            public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
            {
                try
                {
                    this.ACCOUNTKEY = reader.GetGuid(0);
                    this.FACILITYKEY = reader.GetGuid(1);

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
        /// <see cref="NotificationTargetFromPatientIdReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NotificationTargetFromPatientIdReader() : base()
        {
        }

        /// <summary>
        /// 患者ID、LinkageSystemNo から プッシュ通知対象アカウントキー と 通知元施設キー を返す。
        /// </summary>
        /// <param name="patientId">患者ID。</param>
        /// <param name="linkageSystemNo">LinkageSystemNo。</param>
        /// <returns></returns>
        private TargetFromPatientId SelectTargetFromPatientId(string patientId, string linkageSystemNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", patientId),
                    this.CreateParameter(connection, "@P2", linkageSystemNo),
                    this.CreateParameter(connection, "@P3", false)
                };

                // クエリを作成                
                query.Append(" SELECT T1.ACCOUNTKEY, T2.FACILITYKEY");
                query.Append(" FROM QH_LINKAGE_DAT AS T1, QH_LINKAGESYSTEM_MST AS T2");
                query.Append(" WHERE T1.LINKAGESYSTEMNO = T2.LINKAGESYSTEMNO");
                query.Append(" AND   T1.LINKAGESYSTEMNO = @P2");
                query.Append(" AND   T1.LINKAGESYSTEMID = @P1");
                query.Append(" AND   T1.DELETEFLAG = @P3");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<TargetFromPatientId>(connection, null, query.ToString(), @params).FirstOrDefault();
            }
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public NotificationTargetFromPatientIdReaderResults ExecuteByDistributed(NotificationTargetFromPatientIdReaderArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            NotificationTargetFromPatientIdReaderResults result = new NotificationTargetFromPatientIdReaderResults() { IsSuccess = false };
            TargetFromPatientId ret = this.SelectTargetFromPatientId(args.PatientId, args.LinkageSystemNo);
            //結果が格納されている場合。
            if (ret != null)
            {
                result.TargetAccountKey = ret.ACCOUNTKEY;
                result.FromFacilityKey = ret.FACILITYKEY;
                result.IsSuccess = true;
            }
            return result;
        }
    }


}

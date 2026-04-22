using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// 施設の内容を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class ExaminationGroupNameReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, ExaminationGroupNameReaderArgs, ExaminationGroupNameReaderResults>
    {
        /// <summary>
        /// 検査グループ名・項目名クラスを表します。
        /// </summary>
        /// <remarks></remarks>
        internal sealed class ExaminationGroupName : QsDbEntityBase
        {

            /// <summary>
            ///     グループNoを取得または設定します。
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public int GROUPNO { get; set; } = int.MinValue;

            /// <summary>
            ///     グループ名を取得または設定します。
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string GROUPNAME { get; set; } = string.Empty;

            /// <summary>
            ///     項目コードを取得または設定します。
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string ITEMCODE { get; set; } = string.Empty;

            /// <summary>
            ///     項目名を取得または設定します。
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string ITEMNAME { get; set; } = string.Empty;

            /// <summary>
            ///     コメントを取得または設定します。
            ///     </summary>
            ///     <value></value>
            ///     <returns></returns>
            ///     <remarks></remarks>
            public string COMMENT { get; set; } = string.Empty;

            public override QsDbEntityBase InitializeByDbDataReader(DbDataReader reader)
            {
                try
                {

                    this.GROUPNO = reader.GetInt32(0);
                    this.GROUPNAME = reader.GetString(1);
                    this.ITEMCODE = reader.GetString(2);
                    this.ITEMNAME = reader.GetString(3);
                    this.COMMENT = reader.GetString(4);

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
        /// <see cref="ExaminationGroupNameReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public ExaminationGroupNameReader() : base()
        {
        }

        /// <summary>
        /// 施設キーから検査項目グループ情報を返す。
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        private List<ExaminationGroupName> SelectExaminationGroupName(Guid facilityKey )
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_EXAMINATIONGROUP_MST>())
            {
                StringBuilder query = new StringBuilder();
                List<DbParameter> @params = new List<DbParameter>() {
                    this.CreateParameter(connection, "@P1", facilityKey),
                    this.CreateParameter(connection, "@P2", false)};

                // クエリを作成                
                query.Append(" SELECT T2.GROUPNO, T2.GROUPNAME, T1.ITEMCODE, T1.ITEMNAME, T1.COMMENT");
                query.Append(" FROM QH_EXAMINATIONITEM_MST AS T1, QH_EXAMINATIONGROUP_MST AS T2");
                query.Append(" WHERE FACILITYKEY = @P1");
                query.Append(" AND T1.DISPLAYFLAG = 1 AND T1.DELETEFLAG = @P2");
                query.Append(" AND T1.GROUPNO = T2.GROUPNO");
                query.Append(" AND T2.DISPLAYFLAG = 1 AND T2.DELETEFLAG = @P2");
                query.Append(" ORDER BY T2.DISPORDER, T1.DISPORDER");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<ExaminationGroupName>(connection, null, query.ToString(), @params);
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
        public ExaminationGroupNameReaderResults ExecuteByDistributed(ExaminationGroupNameReaderArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            ExaminationGroupNameReaderResults result = new ExaminationGroupNameReaderResults() { IsSuccess = false };
            List<ExaminationGroupName> ret = this.SelectExaminationGroupName(args.FacilityKey);
            result.GroupNameList = new Dictionary<string, Tuple<int, string, string, string>>();
            //結果が格納されている場合。
            if (ret != null)
            {
                //取得結果分処理を行う。
                foreach (ExaminationGroupName target in ret)
                {
                    result.GroupNameList.Add(target.ITEMCODE, Tuple.Create(target.GROUPNO, target.GROUPNAME, target.ITEMNAME, target.COMMENT));
                }
            }

            if (result.GroupNameList != null && result.GroupNameList.Count >= 0)
                result.IsSuccess = true;
            return result;
        }
    }


}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// お薬手帳の情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbNoteMedicineReaderCore : QsDbReaderBase
    {
        /// <summary>
        /// 対象者アカウントキーを保持します。
        /// </summary>
        /// <remarks></remarks>
        private Guid _actorKey = Guid.Empty;

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private DbNoteMedicineReaderCore()
        {
        }
        
        /// <summary>
        /// 所有者アカウントキーおよび対象者アカウントキーを指定して、
        /// <see cref="DbNoteMedicineReaderCore" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="actorKey">対象者アカウントキー</param>
        /// <remarks></remarks>
        public DbNoteMedicineReaderCore(Guid actorKey) : base()
        {
            this._actorKey = actorKey;

            if (this._actorKey == Guid.Empty)
                throw new ArgumentOutOfRangeException("actorKey", "対象者アカウントキーが不正です。");
        }

        /// <summary>
        /// データの取得範囲を算出します。
        /// </summary>
        /// <param name="totalCount">総件数</param>
        /// <param name="pageSize">ページサイズ</param>
        /// <param name="pageIndex">ページインデックス</param>
        /// <param name="startPosition">取得開始位置が格納される変数</param>
        /// <param name="endPosition">取得終了位置が格納される変数</param>
        /// <param name="returnPageIndex">実際のページインデックスが格納される変数</param>
        /// <param name="maxPageIndex">最大ページインデックスが格納される変数</param>
        /// <returns>
        /// 成功ならTrue、失敗ならFalse
        /// </returns>
        private bool GetRange(
            int totalCount,
            int pageSize,
            int pageIndex,
            ref int startPosition,
            ref int endPosition,
            ref int returnPageIndex,
            ref int maxPageIndex)
        {
            var result = false;

            startPosition = int.MinValue;
            endPosition = int.MinValue;
            returnPageIndex = pageIndex < 0 ? int.MinValue : pageIndex;
            maxPageIndex = int.MinValue;

            if ( totalCount >= 0  && pageSize > 0 && pageIndex >= 0)
            {
                maxPageIndex = (totalCount / pageSize) + ((totalCount % pageSize) > 0 ? 0 : -1);
                startPosition = pageSize * pageIndex;

                if (startPosition < totalCount)
                {
                    endPosition = startPosition +  pageSize - 1;
                }
                else
                {
                    while(startPosition >= totalCount)
                    {
                        startPosition -= pageSize;
                        returnPageIndex -= 1;
                    }

                    if(startPosition < 0 || returnPageIndex < 0)
                    {
                        startPosition = 0;
                        returnPageIndex = 0;
                    }

                    endPosition = startPosition + pageSize - 1;
                }

                result = true;

            }

            return result;
        }


        /// <summary>
        /// 表示期間内のデータの総件数を取得します。
        /// </summary>
        /// <param name="dataTypeList">データ種別のリスト</param>
        /// <returns>総件数</returns>
        private int GetCount( List<byte> dataTypeList)
        {
            var result = int.MinValue;

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var query = new StringBuilder();
                var paramList = new List<DbParameter>() { 
                    this.CreateParameter(connection,"@p1", this._actorKey),
                    this.CreateParameter(connection,"@p2", string.Join(", ", dataTypeList))
                };

                // クエリを作成
                query.Append("select count(year(recorddate) * 10000 + month(recorddate) * 100 + day(recorddate))");
                query.Append(" from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.AppendFormat(" and datatype in ({0})", string.Join(", ", dataTypeList));
                query.Append(" and ownertype in (1, 2, 3, 101)");
                query.Append(" and deleteflag = 0");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                result = this.ExecuteScalar<int>(connection, null, this.CreateCommandText(connection, query.ToString()), paramList);
            }

            return result;
        }


        private List<QH_MEDICINE_DAT> SelectEntitiesByRange(List<byte> dataTypeList, int startPos, int endPos)
        {
            if (startPos < 0) throw new ArgumentOutOfRangeException("startPos", "取得開始位置が不正です。");
            if (endPos < 0) throw new ArgumentOutOfRangeException("endPos", "取得終了位置が不正です。");
            if (startPos > endPos) throw new ArgumentOutOfRangeException("startPos、endPos", "取得開始位置と終了位置が逆転しています。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var query = new StringBuilder();
                var paramList = new List<DbParameter>() {
                    this.CreateParameter(connection,"@p1", this._actorKey),
                    this.CreateParameter(connection,"@p2", string.Join(", ", dataTypeList)),
                    this.CreateParameter(connection, "@p3", startPos),
                    this.CreateParameter(connection, "@p4", endPos)
                };

                // クエリを作成
                query.Append("select accountkey, recorddate, sequence, datatype, ownertype, linkagesystemno, pharmacyno, receiptno, facilitykey, prescriptiondate, startdate, enddate, originalfilename, medicineset, convertedmedicineset, commentset, deleteflag, createddate, updateddate");
                query.Append(" from (");
                query.Append(" select row_number() over (order by recorddate desc , sequence desc) as rownum, *");
                query.Append(" from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.AppendFormat(" and datatype in ({0})", string.Join(", ", dataTypeList));
                query.Append(" and ownertype in (1, 2, 3, 101)");  //余計なものを取りたくないので絞ってるが意味はない
                query.Append(" and deleteflag = 0");
                query.Append(" ) as t1");
                query.Append(" where t1.rownum between @p3 and @p4");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return  this.ExecuteReader<QH_MEDICINE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), paramList);
            }
        }

        /// <summary>
        /// 表示期間内のページ位置を指定して、
        /// お薬手帳の情報テーブルエンティティのリストを取得します。
        /// </summary>
        /// <param name="dataTypeList">データ種別のリスト</param>
        /// <param name="pageSize">ページサイズ</param>
        /// <param name="pageIndex">ページインデックス</param>
        /// <param name="refPageIndex">実際のページインデックスが格納される変数</param>
        /// <param name="refMaxPageIndex">実際の最大ページインデックスが格納される変数</param>
        /// <returns></returns>
        private List<QH_MEDICINE_DAT> ReadMedicineEntityList (
            List<byte> dataTypeList,
            int pageSize,
            int pageIndex,
            ref int refPageIndex,
            ref int refMaxPageIndex)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize", "ページサイズが不正です。");
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex", "ページインデックスが不正です。");
            if (dataTypeList == null || !dataTypeList.Any()) throw new ArgumentNullException("dataTypeList", "対象のデータ種別が不正です。");

            var result = new List<QH_MEDICINE_DAT>();

            int total = this.GetCount(dataTypeList); // 総件数取得
            int startPos = int.MinValue;
            int endPos = int.MinValue;
            int retIndex = int.MinValue;
            int maxIndex = int.MinValue;

            if( this.GetRange(total, pageSize, pageIndex, ref startPos, ref endPos, ref retIndex, ref maxIndex))
            {
                result = this.SelectEntitiesByRange(dataTypeList, startPos + 1, endPos + 1);
                refPageIndex = pageIndex;
                refMaxPageIndex = maxIndex;
            }
            else
            {
                // 該当するデータなし
                refPageIndex = 0;
                refMaxPageIndex = 0;
            }

            return result;
        }

        /// <summary>
        /// データ種別、最終アクセス日時を指定して、
        /// 最終アクセス日時以降に追加・修正・削除されたデータ件数を取得します。
        /// </summary>
        /// <param name="dataTypeList">データ種別のリスト</param>
        /// <param name="lastAccessDate">最終アクセス日時</param>
        /// <returns></returns>
        private int GetCountModified(List<byte> dataTypeList,DateTime lastAccessDate)
        {
            if (lastAccessDate == DateTime.MinValue) return 0;

            if (dataTypeList == null || !dataTypeList.Any()) throw new ArgumentNullException("dataTypeList", "対象のデータ種別が不正です。");

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var query = new StringBuilder();
                var paramList = new List<DbParameter>() {
                    this.CreateParameter(connection,"@p1", this._actorKey),
                    this.CreateParameter(connection,"@p2", string.Join(", ", dataTypeList)),
                    this.CreateParameter(connection,"@p3", lastAccessDate)
                };

                // クエリを作成
                query.Append("select count(*)");
                query.Append(" from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.AppendFormat(" and datatype in ({0})", string.Join(", ", dataTypeList));
                query.Append(" and updateddate >= @p3");
                // query.Append(" and deleteflag = 0"); // 削除も検知する必要がある
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteScalar<int>(connection, null, this.CreateCommandText(connection, query.ToString()), paramList);
            }
        }

        private int GetMaxSequence(DateTime recordDate)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {
                var query = new StringBuilder();
                var paramList = new List<DbParameter>() {
                    this.CreateParameter(connection,"@p1", this._actorKey),
                    this.CreateParameter(connection,"@p2", recordDate)
                };

                // クエリを作成
                query.Append("select isunll(max(sequence), 0)");
                query.Append(" from qh_medicine_dat");
                query.Append(" where accountkey = @p1");
                query.Append(" and recorddate = @p2");
                query.Append(";");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteScalar<int>(connection, null, this.CreateCommandText(connection, query.ToString()), paramList);
            }
        }

        #region "public method"

        /// <summary>
        /// ページ位置を指定して、お薬手帳の情報のリストを取得します。
        /// </summary>
        /// <param name="dataTypeList">ページサイズ</param>
        /// <param name="pageSize">ページインデックス</param>
        /// <param name="pageIndex"></param>
        /// <param name="refPageIndex">実際のページインデックスが格納される変数</param>
        /// <param name="refMaxPageIndex">実際の最大ページインデックスが格納される変数</param>
        /// <returns>該当するお薬手帳情報のリスト。</returns>
        public List<QH_MEDICINE_DAT> ReadMedicineList(List<byte> dataTypeList, int pageSize, int pageIndex,ref int refPageIndex,ref int refMaxPageIndex)
        {
            return this.ReadMedicineEntityList(dataTypeList, pageSize, pageIndex, ref refPageIndex, ref refMaxPageIndex);
        }

        /// <summary>
        /// 最終アクセス日時よりも新しく追加・修正・削除されたデータが存在するかどうかを判定します。
        /// </summary>
        /// <param name="dataTypeList">データ種別のリスト。</param>
        /// <param name="lastAccessDate">最終アクセス日時。</param>
        /// <returns></returns>
        public bool IsModified(List<byte> dataTypeList, DateTime lastAccessDate)
        {
            return this.GetCountModified(dataTypeList, lastAccessDate) > 0;
        }

        /// <summary>
        /// 指定日のお薬手帳の情報を返す
        /// </summary>
        /// <param name="targetDate">指定日</param>
        /// <param name="dataTypeList">データタイプリスト</param>
        /// <param name="facilityKey">施設キー</param>
        /// <param name="linkageSystemNo">連携システム番号</param>
        /// <returns></returns>
        public List<QH_MEDICINE_DAT> ReadMedicineDayList(DateTime targetDate, List<byte> dataTypeList, Guid facilityKey, int linkageSystemNo)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_MEDICINE_DAT>())
            {                
                var paramList = new List<DbParameter>() {
                    this.CreateParameter(connection,"@p1", this._actorKey),
                    this.CreateParameter(connection,"@p2", targetDate),
                    this.CreateParameter(connection, "@p3", facilityKey),
                    this.CreateParameter(connection, "@p4", linkageSystemNo)
                };

                var query = new StringBuilder();
                query.Append($@"
                    Select *
                    From  {nameof(QH_MEDICINE_DAT)}
                    Where {nameof(QH_MEDICINE_DAT.ACCOUNTKEY)} = @p1
                    And   {nameof(QH_MEDICINE_DAT.RECORDDATE)} = @p2
                    And   {nameof(QH_MEDICINE_DAT.DATATYPE)} In ({string.Join(",",dataTypeList)})
                    And   {nameof(QH_MEDICINE_DAT.DELETEFLAG)} = 0
                ");

                if(facilityKey != Guid.Empty)
                {
                    query.Append($@"
                    And   {nameof(QH_MEDICINE_DAT.FACILITYKEY)} = @p3
                    ");
                }
                if(linkageSystemNo > 0)
                {
                    query.Append($@"
                    And   {nameof(QH_MEDICINE_DAT.LINKAGESYSTEMNO)} = @p4
                    ");
                }

                query.Append($@"
                    Order By {nameof(QH_MEDICINE_DAT.SEQUENCE)}
                ");

                // コネクションオープン
                connection.Open();

                // クエリを実行
                return this.ExecuteReader<QH_MEDICINE_DAT>(connection, null, this.CreateCommandText(connection, query.ToString()), paramList);
            }
        }


        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System.Linq;
using MGF.QOLMS.QolmsCryptV1;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class LinkageUserMemberShipReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, LinkageUserMemberShipReaderArgs, LinkageUserMemberShipReaderResults>
    {

        #region "Private Method"

        private List<DbLinkageUserMembershipItem> GetLinkageUserMemberShip(string openId, DateTime targetDate, int linkageSystemNo)
        {

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var query = new StringBuilder();
                var encryptOpenId = openId;

                using (var crypt =  new QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem))
                {
                    encryptOpenId = crypt.EncryptString(openId);
                }

                var param = new List<DbParameter>() {
                    this.CreateParameter(connection, "@openid", encryptOpenId),
                    this.CreateParameter(connection, "@idtype",  (byte)QsDbOpenIdTypeEnum.AuId),
                    this.CreateParameter(connection, "@statustype", (byte)QsDbLinkageStatusTypeEnum.Approved),
                    this.CreateParameter(connection, "@linkagesystemno", linkageSystemNo),
                    this.CreateParameter(connection, "@targetdate", targetDate)
                };

                query
                    .Append("SELECT QH_OPENIDMANAGEMENT_DAT.ACCOUNTKEY, QH_LINKAGE_DAT.LINKAGESYSTEMID, QY_MEMBERSHIP_DAT.MEMBERSHIPTYPE ")
                    .Append(" FROM QH_OPENIDMANAGEMENT_DAT LEFT OUTER JOIN QY_MEMBERSHIP_DAT ON QH_OPENIDMANAGEMENT_DAT.ACCOUNTKEY = QY_MEMBERSHIP_DAT.ACCOUNTKEY AND CONVERT(varchar, @targetdate, 112) BETWEEN CONVERT(varchar, dbo.QY_MEMBERSHIP_DAT.STARTDATE, 112) AND CONVERT(varchar, dbo.QY_MEMBERSHIP_DAT.ENDDATE, 112) AND QY_MEMBERSHIP_DAT.STATUSNO IN (-2, 3, 10,11) ")
                    .Append(" LEFT OUTER JOIN QH_LINKAGE_DAT ON QH_OPENIDMANAGEMENT_DAT.ACCOUNTKEY = QH_LINKAGE_DAT.ACCOUNTKEY AND QH_LINKAGE_DAT.DELETEFLAG = 0 AND QH_LINKAGE_DAT.STATUSTYPE = @statustype AND QH_LINKAGE_DAT.LINKAGESYSTEMNO = @linkagesystemno ")
                    .Append(" WHERE  (QH_OPENIDMANAGEMENT_DAT.OPENID = @openid) AND (QH_OPENIDMANAGEMENT_DAT.IDTYPE = @idtype) AND (QH_OPENIDMANAGEMENT_DAT.DELETEFLAG = 0) AND  (CONVERT(varchar, QH_OPENIDMANAGEMENT_DAT.createddate, 112) <= CONVERT(varchar, @targetdate, 112)) ")
                    .Append(" ORDER  BY QY_MEMBERSHIP_DAT.MEMBERMANAGENO DESC;");

                var log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "testlog.txt");
                System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}{3}{2}{4}{2}", DateTime.Now, query.ToString(), Environment.NewLine, encryptOpenId, targetDate));

                //コネクションオープン
                connection.Open();

                List<DbLinkageUserMembershipItem> resultList = this.ExecuteReader<DbLinkageUserMembershipItem>(connection, null, this.CreateCommandText(connection, query.ToString()), param);
                return resultList;
            }
        }


        private List<DbLinkageUserMembershipItem> GetLinkageUserMemberShip(Guid accountKey, DateTime targetDate, int linkageSystemNo)
        {

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var query = new StringBuilder();

                var param = new List<DbParameter>() {

                    this.CreateParameter(connection, "@accountkey", accountKey),
                    this.CreateParameter(connection, "@idtype", (byte)(QsDbOpenIdTypeEnum.AuId)),
                    this.CreateParameter(connection, "@statustype", (byte)(QsDbLinkageStatusTypeEnum.Approved)),
                    this.CreateParameter(connection, "@linkagesystemno", linkageSystemNo),
                    this.CreateParameter(connection, "@targetdate", targetDate)
                };

                query
                    .Append("SELECT QH_OPENIDMANAGEMENT_DAT.ACCOUNTKEY, QH_LINKAGE_DAT.LINKAGESYSTEMID, QY_MEMBERSHIP_DAT.MEMBERSHIPTYPE ")
                    .Append(" FROM QH_OPENIDMANAGEMENT_DAT LEFT OUTER JOIN QY_MEMBERSHIP_DAT ON QH_OPENIDMANAGEMENT_DAT.ACCOUNTKEY = QY_MEMBERSHIP_DAT.ACCOUNTKEY AND CONVERT(varchar, @targetdate, 112) BETWEEN CONVERT(varchar, dbo.QY_MEMBERSHIP_DAT.STARTDATE, 112) AND CONVERT(varchar, dbo.QY_MEMBERSHIP_DAT.ENDDATE, 112) AND QY_MEMBERSHIP_DAT.STATUSNO IN (-2, 3, 10,11) ")
                    .Append(" LEFT OUTER JOIN QH_LINKAGE_DAT ON QH_OPENIDMANAGEMENT_DAT.ACCOUNTKEY = QH_LINKAGE_DAT.ACCOUNTKEY AND QH_LINKAGE_DAT.DELETEFLAG = 0 AND QH_LINKAGE_DAT.STATUSTYPE = @statustype AND QH_LINKAGE_DAT.LINKAGESYSTEMNO = @linkagesystemno ")
                    .Append(" WHERE  (QH_OPENIDMANAGEMENT_DAT.ACCOUNTKEY = @accountkey) AND (QH_OPENIDMANAGEMENT_DAT.IDTYPE = @idtype) AND (QH_OPENIDMANAGEMENT_DAT.DELETEFLAG = 0) AND  (CONVERT(varchar, QH_OPENIDMANAGEMENT_DAT.createddate, 112) <= CONVERT(varchar, @targetdate, 112)) ")
                    .Append(" ORDER  BY QY_MEMBERSHIP_DAT.MEMBERMANAGENO DESC;");

                //var log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "testlog.txt");
                //System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}{3}{2}{4}{2}", DateTime.Now, query.ToString(), Environment.NewLine, encryptOpenId, targetDate));

                //コネクションオープン
                connection.Open();

                List<DbLinkageUserMembershipItem> resultList = this.ExecuteReader<DbLinkageUserMembershipItem>(connection, null, this.CreateCommandText(connection, query.ToString()), param);
                return resultList;
            }
        }


        private List<DbLinkageUserMembershipItem> GetLinkageUserMemberShipFromLinkageId(string linkageId ,DateTime targetDate,int linkageSystemNo )
        {

            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_LINKAGE_DAT>())
            {
                var query = new StringBuilder();

                var param = new List<DbParameter>() {
                    this.CreateParameter(connection, "@LinkageSystemId", linkageId),
                    this.CreateParameter(connection, "@idtype", (byte)QsDbOpenIdTypeEnum.AuId),
                    this.CreateParameter(connection, "@statustype",  (byte)QsDbLinkageStatusTypeEnum.Approved),
                    this.CreateParameter(connection, "@linkagesystemno", linkageSystemNo),
                    this.CreateParameter(connection, "@targetdate", targetDate)
                };

                query
                    .Append("SELECT QH_LINKAGE_DAT.ACCOUNTKEY, QH_LINKAGE_DAT.LINKAGESYSTEMID, QY_MEMBERSHIP_DAT.MEMBERSHIPTYPE ")
                    .Append(" FROM QH_LINKAGE_DAT LEFT OUTER JOIN QY_MEMBERSHIP_DAT ON QH_LINKAGE_DAT.ACCOUNTKEY = QY_MEMBERSHIP_DAT.ACCOUNTKEY AND CONVERT(varchar, @targetdate, 112) BETWEEN CONVERT(varchar, QY_MEMBERSHIP_DAT.STARTDATE, 112) AND CONVERT(varchar, QY_MEMBERSHIP_DAT.ENDDATE, 112) AND QY_MEMBERSHIP_DAT.STATUSNO IN (- 2, 3, 10, 11) ")
                    .Append(" WHERE (QH_LINKAGE_DAT.LINKAGESYSTEMID = @LinkageSystemId) AND (QH_LINKAGE_DAT.DELETEFLAG = 0) AND (QH_LINKAGE_DAT.STATUSTYPE = @statustype) AND (QH_LINKAGE_DAT.LINKAGESYSTEMNO = @linkagesystemno) ")
                    .Append(" ORDER BY QY_MEMBERSHIP_DAT.MEMBERMANAGENO DESC;");

                //var log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "testlog.txt");
                //System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}{3}{2}{4}{2}", DateTime.Now, query.ToString(), Environment.NewLine, encryptOpenId, targetDate));

                //コネクションオープン
                connection.Open();

                List<DbLinkageUserMembershipItem> resultList = this.ExecuteReader<DbLinkageUserMembershipItem>(connection, null, this.CreateCommandText(connection, query.ToString()), param);
                return resultList;
            }
        }

        #endregion


        #region "Public Method"

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public LinkageUserMemberShipReaderResults ExecuteByDistributed(LinkageUserMemberShipReaderArgs args)
        {

            LinkageUserMemberShipReaderResults result = new LinkageUserMemberShipReaderResults() { IsSuccess = false };
            List<DbLinkageUserMembershipItem> resultList = null;
            AccessLogWorker.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, AccessLogWorker.AccessTypeEnum.Api, string.Empty, $"{args.LinkageSystemNo} {args.LinkageId} {args.TargetDate}");

            //AccountKeyがわかっている場合
            if (args.AccountKey != Guid.Empty)
            {
                resultList = GetLinkageUserMemberShip(args.AccountKey, args.TargetDate, args.LinkageSystemNo);
            }

            //AuSystemID（OpenID)がわかっている場合
            if (string.IsNullOrEmpty(args.AuSystemId) == false)
            {
                resultList = GetLinkageUserMemberShip(args.AuSystemId, args.TargetDate, args.LinkageSystemNo);
            }

            //LinkageId がわかってる場合
            if (string.IsNullOrEmpty(args.LinkageId) == false)
            {
                resultList = GetLinkageUserMemberShipFromLinkageId(args.LinkageId, args.TargetDate, args.LinkageSystemNo);
            }

            if (resultList != null && resultList.Count > 0)
            {
                DbLinkageUserMembershipItem item = resultList.FirstOrDefault();
                result.AccountKey = item.AccountKey;
                result.LinkageId = item.LinkageId;
                result.MemberShipType = item.MembershipType;

                result.IsSuccess = true;
            }

            return result;
        }

        #endregion
    }
}

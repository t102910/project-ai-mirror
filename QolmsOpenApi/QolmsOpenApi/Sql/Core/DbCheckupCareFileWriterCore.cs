using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql.Core
{
    /// <summary>
    /// 介護情報Blob管理データ情報 を、
    /// データベーステーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class DbCheckupCareFileWriterCore : QsDbWriterBase
    {

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        public DbCheckupCareFileWriterCore() : base()
        {
        }


        #region "Private Method"

        #endregion

        #region "Public Method"

        /// <summary>
        /// 介護情報Blob書込み情報を登録、更新します。
        /// </summary>
        /// <returns>
        /// 成功ならUpdate件数が1件の場合 True 否の場合 False、
        /// 失敗なら例外をスロー。
        /// </returns>
        /// <remarks></remarks>
        public int UpsertCheckupCareFile(QH_CHECKUPCAREFILE_DAT entity)
        {
            using (DbConnection connection = QsDbManager.CreateDbConnection<QH_CHECKUPCAREFILE_DAT>())
            {

                List<DbParameter> @params = new List<DbParameter>()
                {
                    this.CreateParameter(connection, "@P1", entity.LINKAGESYSTEMNO),
                    this.CreateParameter(connection, "@P2", entity.LINKAGESYSTEMID),
                    this.CreateParameter(connection, "@P3", entity.SERVICECODE),
                    this.CreateParameter(connection, "@P4", entity.EFFECTIVEDATE),
                    this.CreateParameter(connection, "@P5", entity.ORGANIZATIONID),
                    this.CreateParameter(connection, "@P6", entity.SEQUENCE),
                    this.CreateParameter(connection, "@P7", entity.STATUSNO),
                    this.CreateParameter(connection, "@P8", entity.FILEKEY),
                    this.CreateParameter(connection, "@P9", entity.ORIGINALNAME),
                    this.CreateParameter(connection, "@P10", entity.CONTENTTYPE),
                    this.CreateParameter(connection, "@P11", entity.CONTENTENCODING),
                    this.CreateParameter(connection, "@P12", entity.HASHVALUE),
                    this.CreateParameter(connection, "@P13", entity.DELETEFLAG),
                    this.CreateParameter(connection, "@P14", entity.CREATEDDATE),
                    this.CreateParameter(connection, "@P15", entity.UPDATEDDATE),
                };
                var query = new StringBuilder();
                query.Append("BEGIN");
                query.Append(" IF (SELECT COUNT(*) FROM QH_CHECKUPCAREFILE_DAT WHERE LINKAGESYSTEMNO = @P1 AND LINKAGESYSTEMID = @P2 AND SERVICECODE = @P3 AND EFFECTIVEDATE = @P4 AND ORGANIZATIONID = @P5 AND SEQUENCE = @P6) <> 0");
                query.Append(" BEGIN");
                query.Append("  UPDATE QH_CHECKUPCAREFILE_DAT SET");
                query.Append("  STATUSNO = @P7, FILEKEY = @P8, ORIGINALNAME = @P9, CONTENTTYPE = @P10, CONTENTENCODING = @P11, HASHVALUE = @P12, DELETEFLAG = @P13, CREATEDDATE = @P14");
                query.Append("  WHERE LINKAGESYSTEMNO = @P1 AND LINKAGESYSTEMID = @P2 AND SERVICECODE = @P3 AND EFFECTIVEDATE = @P4 AND ORGANIZATIONID = @P5 AND SEQUENCE = @P6");
                query.Append(" END");
                query.Append(" ELSE");
                query.Append(" BEGIN");
                query.Append("  INSERT INTO QH_CHECKUPCAREFILE_DAT");
                query.Append("  (LINKAGESYSTEMNO, LINKAGESYSTEMID, SERVICECODE, EFFECTIVEDATE, ORGANIZATIONID, SEQUENCE, STATUSNO, FILEKEY, ORIGINALNAME, CONTENTTYPE, CONTENTENCODING, HASHVALUE, DELETEFLAG, CREATEDDATE, UPDATEDDATE)");
                query.Append("  VALUES");
                query.Append("  (@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10, @P11, @P12, @P13, @P14, @P15)");
                query.Append(" END ");
                query.Append("END");

                connection.Open();

                return this.ExecuteNonQuery(connection, null, this.CreateCommandText(connection, query.ToString()), @params);
            }
        }
        #endregion
    }
}
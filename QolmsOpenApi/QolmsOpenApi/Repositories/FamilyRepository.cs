using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// 家族アカウントの入出力インターフェース
    /// </summary>
    public interface IFamilyRepository
    {
        /// <summary>
        /// 本人を含む家族アカウントリストを取得
        /// </summary>
        /// <param name="publicAccountKey"></param>
        /// <returns></returns>
        List<QH_ACCOUNTINDEX_DAT> ReadFamilyList(Guid publicAccountKey);

        /// <summary>
        /// 家族アカウントの削除
        /// </summary>
        /// <param name="parentAccountKey"></param>
        /// <param name="childAccountKey"></param>
        /// <param name="authorKey"></param>
        void DeleteFamily(Guid parentAccountKey, Guid childAccountKey, Guid authorKey);

        /// <summary>
        /// 家族アカウントの追加
        /// </summary>
        /// <param name="parentAccountKey">親アカウントキー</param>
        /// <param name="user"></param>
        /// <param name="birthDate"></param>
        /// <param name="photoKey"></param>
        /// <returns>追加された家族のアカウントキー</returns>
        Guid AddFamily(Guid parentAccountKey, QoApiUserItem user, DateTime birthDate, Guid photoKey);

        /// <summary>
        /// 親子関係が成立しているかを確認する。
        /// </summary>
        /// <param name="parentAccountKey"></param>
        /// <param name="childAccountKey"></param>
        /// <returns>親子関係である True / 親子ではない False</returns>
        bool IsParentChildRelation(Guid parentAccountKey, Guid childAccountKey);
    }

    /// <summary>
    /// 家族アカウントの入出力実装
    /// </summary>
    public class FamilyRepository:QsDbReaderBase, IFamilyRepository
    {
        /// <summary>
        /// 本人を含む家族アカウントリストを取得
        /// </summary>
        /// <param name="publicAccountKey"></param>
        /// <returns></returns>
        public List<QH_ACCOUNTINDEX_DAT> ReadFamilyList(Guid publicAccountKey)
        {
            var reader = new AccountFamilyReader();
            var readerArgs = new AccountFamilyReaderArgs() { AccountKey = publicAccountKey };
            AccountFamilyReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if(readerResults == null || !readerResults.IsSuccess)
            {
                throw new InvalidOperationException();
            }

            return readerResults.Result ?? new List<QH_ACCOUNTINDEX_DAT>();            
        }

        /// <inheritdoc/>
        public void DeleteFamily(Guid parentAccountKey, Guid childAccountKey, Guid authorKey)
        {
            var writer = new FamilyDeleteWriter();
            var writerArgs = new FamilyDeleteWriterArgs
            {
                ParentAccountKey = parentAccountKey,
                AccountKey = childAccountKey,
                AuthorKey = authorKey
            };

            var result = writer.WriteByAuto(writerArgs);

            if(result == null || !result.IsSuccess)
            {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc/>
        public Guid AddFamily(Guid parentAccountKey, QoApiUserItem user, DateTime birthDate, Guid photoKey)
        {
            var writer = new FamilyCreateWriter();
            var writerArgs = new FamilyCreateWriterArgs
            {
                FamilyName = user.FamilyName,
                FamilyNameKana = user.FamilyNameKana,
                GivenName = user.GivenName,
                GivenNameKana = user.GivenNameKana,
                NickName = user.NickName,
                BirthDate = birthDate,
                Sex = user.Sex,
                ParentAccountKey = parentAccountKey,
                PhotoKey = photoKey
            };

            var result = writer.WriteByAuto(writerArgs);

            if (result == null || !result.IsSuccess)
            {
                throw new InvalidOperationException();
            }

            return result.AccountKey;
        }

        /// <inheritdoc/>
        public bool IsParentChildRelation(Guid parentAccountKey, Guid childAccountKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_ACCOUNTRELATION_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", parentAccountKey),
                    CreateParameter(con,"@p2", childAccountKey)
                };

                var sql = $@"
                    SELECT *
                    FROM QH_ACCOUNTRELATION_DAT
                    WHERE ACCOUNTKEY = @p1
                    AND RELATIONACCOUNTKEY = @p2
                    AND RELATIONDIRECTIONTYPE = 1
                    AND RELATIONTYPE = 1
                    AND DELETEFLAG = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_ACCOUNTRELATION_DAT>(con, null, sql, parameters);

                return result.FirstOrDefault() != null;
            }
        }
    }
}
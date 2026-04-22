using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ワンタイムコードから利用者の情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class OnetimeCodeReader : QsDbReaderBase, IQsDbDistributedReader<QH_ELINKONETIMECODE_DAT, OnetimeCodeReaderArgs, OnetimeCodeReaderResults>
    {
        /// <summary>
        /// <see cref="OnetimeCodeReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public OnetimeCodeReader() : base()
        {
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public OnetimeCodeReaderResults ExecuteByDistributed(OnetimeCodeReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new OnetimeCodeReaderResults() { IsSuccess = false };

            result.Result = new DbOnetimeCodeReaderCore().ReadEntities(args.OnetimeCode); ;
            result.IsSuccess = true;

            return result;

        }
    }

 }
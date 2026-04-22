using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// OpenIdManagement情報 を、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class LineOpenIdManagementWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, LineOpenIdManagementWriterArgs, LineOpenIdManagementWriterResults>
    {

        /// <summary>
        /// <see cref="LineOpenIdManagementWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LineOpenIdManagementWriter() : base()
        {
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public LineOpenIdManagementWriterResults ExecuteByDistributed(LineOpenIdManagementWriterArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            //UPSERT実行
            LineOpenIdManagementWriterResults result = new LineOpenIdManagementWriterResults() { IsSuccess = false };
            DbLineOpenIdManagementWriterCore accountWriter = new DbLineOpenIdManagementWriterCore(args.UserId, args.LinkageSystemNo, args.DeleteFlag);
            
            if (accountWriter.InsertLineOpenIdManagement())
            {
                // 成功
                result.IsSuccess = true;
            }

            return result;
        }
    }


}
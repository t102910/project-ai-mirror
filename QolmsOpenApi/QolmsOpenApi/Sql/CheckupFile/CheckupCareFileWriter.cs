using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 介護情報Blob管理データ を、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class CheckupCareFileWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, CheckupCareFileWriterArgs, CheckupCareFileWriterResults>
    {

        /// <summary>
        /// <see cref="CheckupCareFileWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public CheckupCareFileWriter() : base()
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
        public CheckupCareFileWriterResults ExecuteByDistributed(CheckupCareFileWriterArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }
            var result = new CheckupCareFileWriterResults() { IsSuccess = false };

            //UPSERT実行
            var writer = new DbCheckupCareFileWriterCore();

            result.Result = writer.UpsertCheckupCareFile(args.Entity);
            result.IsSuccess = true;

            return result;
        }
    }


}
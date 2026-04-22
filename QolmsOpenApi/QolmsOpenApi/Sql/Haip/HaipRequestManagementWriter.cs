using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// HAIPデータ取得依頼情報 を、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class HaipRequestManagementWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, HaipRequestManagementWriterArgs, HaipRequestManagementWriterResults>
    {

        /// <summary>
        /// <see cref="HaipRequestManagementWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public HaipRequestManagementWriter() : base()
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
        public HaipRequestManagementWriterResults ExecuteByDistributed(HaipRequestManagementWriterArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            //UPSERT実行
            HaipRequestManagementWriterResults result = new HaipRequestManagementWriterResults() { IsSuccess = false };
            DbHaipRequestManagementWriterCore writerCore = new DbHaipRequestManagementWriterCore(args.RequestId, args.ResponseStatus);
            
            if (writerCore.UpsertHaipRequestManagement())
            {
                // 成功
                result.IsSuccess = true;
            }

            return result;
        }
    }


}
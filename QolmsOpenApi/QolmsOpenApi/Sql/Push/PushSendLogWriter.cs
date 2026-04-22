using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using System;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ReproPush通知登録情報 を、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class PushSendLogWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, PushSendLogWriterArgs, PushSendLogWriterResults>
    {

        /// <summary>
        /// <see cref="PushSendLogWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public PushSendLogWriter() : base()
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
        public PushSendLogWriterResults ExecuteByDistributed(PushSendLogWriterArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }

            //UPSERT実行
            PushSendLogWriterResults result = new PushSendLogWriterResults() { IsSuccess = false };
            DbPushWriterCore writerCore = new DbPushWriterCore(args.Entities);
            
            if (writerCore.InsertPushSendLog())
            {
                // 成功
                result.IsSuccess = true;
            }

            return result;
        }
    }


}
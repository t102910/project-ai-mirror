using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;


namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// ログイン日時 を、
    /// データベース テーブルへ登録するための機能を提供します。
    ///  このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class WaitingOrderListWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, WaitingOrderListWriterArgs, WaitingOrderListWriterResults>
    {

        /// <summary>
        /// <see cref="WaitingOrderListWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public WaitingOrderListWriter() : base()
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
        public WaitingOrderListWriterResults ExecuteByDistributed(WaitingOrderListWriterArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");
            }
            var result = new WaitingOrderListWriterResults() { IsSuccess = false };

            //UPSERT実行
            var writer = new DbWaitingOrderListWriterCore();
            if (args.Entity != null)
            {
                // 南部徳洲会で不具合あったので暫定チューニング
                // 本対応では一本の処理で解決する
                // 千葉徳洲会で同様の問題再現したため、暫定対応
                if ((args.Entity.LINKAGESYSTEMNO == 47001005 || args.Entity.LINKAGESYSTEMNO == 12001017) && args.Entity.PUSHSENDFLAG == false)
                {
                    QoAccessLog.WriteInfoLog($"検証ログ:南部・千葉プッシュ通知時以外用分岐作動");
                    result.Result = writer.UpsertWaitingOrderListNanbu(args.Entity);
                }
                else
                {
                    result.Result = writer.UpsertWaitingOrderList(args.Entity);
                }
                
            }
            //DELETE実行
            else
            {
                result.Result = writer.DeleteWaitingOrderList(args.LinkageSystemNo, args.DepartmentCode, args.DoctorCode, args.LinkageSystemId, args.DjKbn);
            }
            result.IsSuccess = true;

            return result;
        }
    }


}
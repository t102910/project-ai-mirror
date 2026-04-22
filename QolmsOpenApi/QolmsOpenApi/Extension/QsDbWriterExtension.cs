using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{
    /// <summary>
    /// DbWriterに関する拡張メソッド
    /// </summary>
    public static class QsDbWriterExtension
    {
        /// <summary>
        /// トランザクションの有無に応じて自動的に分岐して書き込みを行う
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <typeparam name="TResults"></typeparam>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TResults WriteByAuto<TEntity,TArgs,TResults>(this IQsDbDistributedWriter<TEntity, TArgs, TResults> writer ,TArgs args)
            where TEntity : QsDbEntityBase
            where TArgs : QsDbWriterArgsBase<TEntity>
            where TResults : QsDbWriterResultsBase
        {
            // トランザクションが存在しない場合は新規トランザクションで実行
            if(Transaction.Current is null)
            {
                return QsDbManager.Write(writer, args);
            }
            // トランザクションがある場合は現在のトランザクションに参加して実行
            else
            {
                return QsDbManager.WriteByCurrent(writer, args);
            }            
        }
    }
}
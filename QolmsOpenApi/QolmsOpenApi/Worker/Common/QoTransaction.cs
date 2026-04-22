using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// DBトランザクションを扱うクラス
    /// 内部でQsDbManager.WriteByCurrentを使用している場合と
    /// QsDbWriterBaseの拡張メソッドWriteByAutoを使用している場合のみ使用可。
    /// Workerで使用してもTest時に影響は受けない
    /// QsDbManager.Readメソッド使用している場合はトランザクション中の
    /// レコードは読みだせないので注意する
    /// </summary>
    public class QoTransaction : IDisposable
    {
        TransactionScope _scope;
        /// <summary>
        /// インスタンスの生成と同時にトランザクションスコープを生成し
        /// トランザクションを開始する
        /// 必ずUsing句とともに使用すること。
        /// Commitを行わずにUsing句を抜けるとロールバックされる。
        /// </summary>
        public QoTransaction()
        {
            _scope = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable
                }
            );
        }

        /// <summary>
        /// トランザクションを確定する
        /// </summary>
        public void Commit()
        {
            _scope?.Complete();
        }

        /// <summary>
        /// トランザクションスコープを破棄する
        /// </summary>
        public void Dispose()
        {
            _scope.Dispose();
            _scope = null;
        }
    }
}
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// SignUpデータへの入出力インターフェース
    /// </summary>
    public interface ISignUpRepository
    {
        /// <summary>
        /// 対象のアカウントキーの仮登録データを削除する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        bool DeleteSignUpData(Guid accountKey);
    }

    /// <summary>
    /// SignUpデータへの入出力実装
    /// </summary>
    public class SignUpRepository: ISignUpRepository
    {
        /// <summary>
        /// 対象のアカウントキーの仮登録データを削除する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public bool DeleteSignUpData(Guid accountKey)
        {
            try
            {
                var writer = new DbSignUpWriterCore();
                writer.DeleteSignUpData(accountKey);

                return true;
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return false;
            }
        }
    }
}
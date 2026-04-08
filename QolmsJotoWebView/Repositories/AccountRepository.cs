namespace MGF.QOLMS.QolmsJotoWebView
{

    ///// <summary>
    ///// アカウント情報への入出力インターフェース
    ///// </summary>
    //public interface IAccountRepository
    //{
    //    /// <summary>
    //    /// QH_ACCOUNT_MSTから主キーでレコードを取得する
    //    /// </summary>
    //    /// <param name="accountKey"></param>
    //    /// <returns></returns>
    //    Guid SsoAccountExists(Guid executer, Guid accountKey);

    //}

    //public class AccountRepository : IAccountRepository
    //{
    //    /// <summary>
    //    /// ダミーのセッションIDを現します。
    //    /// </summary>
    //    readonly string DUMMY_SESSION_ID = new string('Z', 100);

    //    /// <summary>
    //    /// ダミーのAPI認証キーを表します
    //    /// </summary>
    //    readonly Guid DUMMY_API_AUTHORIZE_KEY = new Guid(new string('F', 32));


    //    /// <summary>
    //    /// QH_ACCOUNT_MSTから主キーでレコードを取得する
    //    /// </summary>
    //    /// <param name="accountKey"></param>
    //    /// <returns></returns>
    //    public Guid SsoAccountExists(Guid executer, Guid accountKey)
    //    {
    //        //Identityを呼び出してJWT用の存在チェック
    //        var apiArgs = new QiQolmsJotoSsoAccountExistsApiArgs(executer, string.Empty)
    //        { 
    //            ActorKey = accountKey.ToApiGuidString() 
    //        };

    //        var apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsJotoSsoAccountExistsApiResults>(
    //            apiArgs,
    //            DUMMY_SESSION_ID,
    //            DUMMY_API_AUTHORIZE_KEY
    //            );

    //        if (apiResults.IsSuccess.TryToValueType(false))
    //        {
    //            return apiResults.Accountkey.TryToValueType(Guid.Empty);
    //        }

    //        return Guid.Empty;
    //    }
    //}
}
namespace MGF.QOLMS.QolmsJotoWebView
{
    public class AccessKeyUseCase
    {

        ///// <summary>
        ///// 暗号化したExecutorを生成
        ///// </summary>
        ///// <param name="executor"></param>
        ///// <returns></returns>
        //public static string GetEncExeCutor(string executor)
        //{
        //    string encExeCutor = string.Empty;
        //    using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
        //    {
        //        try
        //        {
        //            encExeCutor = crypt.EncryptString(executor.TryToValueType(Guid.Empty).ToString("N"));
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //    return encExeCutor;
        //}

        ///// <summary>
        ///// アクセスキーを発行
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        //public QoAccessKeyGenerateApiResults Generate(QoAccessKeyGenerateApiArgs args)
        //{
        //    QoAccessKeyGenerateApiResults result = new QoAccessKeyGenerateApiResults() { IsSuccess = bool.FalseString };

        //    DateTime? expiry = null; // null指定でデフォルト14日
        //    if (args.TokenExpiryHours > 0)
        //    {
        //        // 有効期限が指定されていたら指定の期限を設定する
        //        expiry = DateTime.Now.AddHours(args.TokenExpiryHours);
        //    }

        //    int role = (int)QoApiFunctionTypeEnum.All; // デフォルト全権限
        //    if (args.TokenRoles != QoApiRoleTypeEnum.None)
        //    {
        //        // ロールが指定されていれば指定のロールを設定
        //        role = (int)args.TokenRoles;
        //    }

        //    result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(GetEncExeCutor(args.Executor), args.ActorKey.TryToValueType(Guid.Empty), Guid.Empty, role, expiry);
        //    if (!string.IsNullOrEmpty(result.AccessKey))
        //        result.IsSuccess = bool.TrueString;

        //    //ログイン日時更新 現状TISAPPのみ
        //    //string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
        //    //if (p == AppWorker.UrlParam_TisApp)
        //    //{
        //    //    AccessKeyWorker.UpdateLogInManagement(args.ActorKey.TryToValueType(Guid.Empty), DateTime.Now);
        //    //}

        //    return result;
        //}

        ///// <summary>
        ///// アクセスキーを更新
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        //public static QoAccessKeyRefreshApiResults Refresh(QoAccessKeyRefreshApiArgs args)
        //{
        //    QoAccessKeyRefreshApiResults result = new QoAccessKeyRefreshApiResults() { IsSuccess = bool.FalseString };


        //    DateTime? expiry = null; // null指定でデフォルト14日
        //    if (args.TokenExpiryHours > 0)
        //    {
        //        // 有効期限が指定されていたら指定の期限を設定する
        //        expiry = DateTime.Now.AddHours(args.TokenExpiryHours);
        //    }

        //    int role = (int)QjApiFunctionTypeEnum.All; // デフォルト全権限
        //    if (args.TokenRoles != QjApiRoleTypeEnum.None)
        //    {
        //        // ロールが指定されていれば指定のロールを設定
        //        role = (int)args.TokenRoles;
        //    }

        //    result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(GetEncExeCutor(args.Executor), args.ActorKey.TryToValueType(Guid.Empty), Guid.Empty, role, expiry);
        //    if (!string.IsNullOrEmpty(result.AccessKey))
        //        result.IsSuccess = bool.TrueString;

        //    ////ログイン日時更新 現状TISAPPのみ
        //    //string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
        //    //if (p == AppWorker.UrlParam_TisApp)
        //    //{
        //    //    AccessKeyWorker.UpdateLogInManagement(args.ActorKey.TryToValueType(Guid.Empty), DateTime.Now);
        //    //}

        //    return result;
        //}

    }
}
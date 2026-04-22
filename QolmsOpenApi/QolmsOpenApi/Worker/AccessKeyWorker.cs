using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsDbCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessKeyWorker
    {


        #region "Private Meshod"

        /// <summary>
        /// ログインマネジメント の ログイン日時 を更新する。
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        private static void UpdateLogInManagement(Guid accountKey, DateTime actionDate)
        {
            //Writerクラス定義
            var writer = new AccountLogInManagementWriter();
            var writerArgs = new AccountLogInManagementWriterArgs()
            {
                AuthorKey = accountKey,
                ActionDate = actionDate
            };
            AccountLogInManagementWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

        }


        #endregion

        #region "Public Meshod"

        /// <summary>
        /// 暗号化したExecutorを生成
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        public static string GetEncExeCutor(string executor)
        {
            string encExeCutor = string.Empty;
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                try
                {
                    encExeCutor = crypt.EncryptString(executor.TryToValueType(Guid.Empty).ToString("N"));
                }
                catch (Exception)
                {
                }
            }
            return encExeCutor;
        }

        /// <summary>
        /// アクセスキーを発行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccessKeyGenerateApiResults Generate(QoAccessKeyGenerateApiArgs args)
        {
            QoAccessKeyGenerateApiResults result = new QoAccessKeyGenerateApiResults() { IsSuccess = bool.FalseString };
            
            DateTime? expiry = null; // null指定でデフォルト14日
            if(args.TokenExpiryHours > 0)
            {
                // 有効期限が指定されていたら指定の期限を設定する
                expiry = DateTime.Now.AddHours(args.TokenExpiryHours);
            }

            int role = (int)QoApiFunctionTypeEnum.All; // デフォルト全権限
            if(args.TokenRoles != QoApiRoleTypeEnum.None)
            {
                // ロールが指定されていれば指定のロールを設定
                role = (int)args.TokenRoles;
            }

            result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(GetEncExeCutor(args.Executor), args.ActorKey.TryToValueType(Guid.Empty),Guid.Empty, role, expiry);
            if(!string.IsNullOrEmpty(result.AccessKey) )
                result.IsSuccess = bool.TrueString;

            //ログイン日時更新 現状TISAPPのみ
            string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
            if (p == AppWorker.UrlParam_TisApp)
            {
                AccessKeyWorker.UpdateLogInManagement(args.ActorKey.TryToValueType(Guid.Empty), DateTime.Now);
            }

            return result;
        }

        /// <summary>
        /// アクセスキーを更新
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccessKeyRefreshApiResults Refresh(QoAccessKeyRefreshApiArgs args)
        {
            QoAccessKeyRefreshApiResults result = new QoAccessKeyRefreshApiResults() { IsSuccess = bool.FalseString };


            DateTime? expiry = null; // null指定でデフォルト14日
            if (args.TokenExpiryHours > 0)
            {
                // 有効期限が指定されていたら指定の期限を設定する
                expiry = DateTime.Now.AddHours(args.TokenExpiryHours);
            }

            int role = (int)QoApiFunctionTypeEnum.All; // デフォルト全権限
            if (args.TokenRoles != QoApiRoleTypeEnum.None)
            {
                // ロールが指定されていれば指定のロールを設定
                role = (int)args.TokenRoles;
            }

            result.AccessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(GetEncExeCutor(args.Executor), args.ActorKey.TryToValueType(Guid.Empty), Guid.Empty, role, expiry);
            if (!string.IsNullOrEmpty(result.AccessKey))
                result.IsSuccess = bool.TrueString;

            //ログイン日時更新 現状TISAPPのみ
            string p = AppWorker.GetUrlParam(args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None));
            if (p == AppWorker.UrlParam_TisApp)
            {
                AccessKeyWorker.UpdateLogInManagement(args.ActorKey.TryToValueType(Guid.Empty), DateTime.Now);
            }

            return result;
        }

        #endregion

    }

}
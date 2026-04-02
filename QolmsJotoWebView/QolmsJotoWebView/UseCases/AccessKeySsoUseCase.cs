using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{

    public class AccessKeySsoUseCase
    {

        ILoginRepository _accountRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accountRepository"></param>
        public AccessKeySsoUseCase(ILoginRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }


        private bool TryGetAccount(Guid executer,Guid accountKey, QjApiResultsBase results, out Guid outAccountkey)
        {
            try
            {
                // アカウントマスタを取得
                outAccountkey = _accountRepository.SsoAccountExists(executer,accountKey);
                if (outAccountkey == Guid.Empty)
                {
                    results.Result = QjApiResult.Build(QjApiResultCodeTypeEnum.OperationError, "アカウントが存在しませんでした。");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                outAccountkey = Guid.Empty;
                results.Result = QjApiResult.Build(ex, "アカウント情報の取得に失敗しました。");
                return false;
            }
        }

        public QjAccessKeySsoApiResults Generate(QjAccessKeySsoApiArgs args)
        {
            var results = new QjAccessKeySsoApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーに変換
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            var executor = args.Executor.TryToValueType(Guid.Empty);

            // 該当アカウントをマスタより取得
            if (!this.TryGetAccount(executor, accountKey, results, out var user))
            {
                return results;
            }

            try
            {
                // SSOの生成
                var tokenProvider = new QsJwtTokenProvider();
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    var encExeCutor = crypt.EncryptString(args.Executor.TryToValueType(Guid.Empty).ToString("N"));
                    // 有効期限15分
                    results.SsoKey = tokenProvider.CreateQolmsJwtSsoKey(encExeCutor, accountKey, Guid.Empty, args.QolmsJotoJwtPageNo.TryToValueType(int.MinValue), DateTime.Now.AddMinutes(15));
                }
            }
            catch (Exception ex)
            {
                results.Result = QjApiResult.Build(ex, "シングルサインオンキーの生成に失敗しました。");
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QjApiResult.Build(QjApiResultCodeTypeEnum.Success);

            return results;
        }
    }
}
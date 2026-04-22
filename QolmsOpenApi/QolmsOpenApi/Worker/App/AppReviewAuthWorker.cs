using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// アプリ審査用認証処理
    /// </summary>
    public class AppReviewAuthWorker
    {
        /// <summary>
        /// 審査用ID
        /// </summary>
        const string ReviewID = "95521654";
        /// <summary>
        /// 審査用パスワード(暗号化)
        /// </summary>
        const string ReviewPass = "4mIWAt9ctsdxwHLV599ySw==";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AppReviewAuthWorker()
        {

        }

        /// <summary>
        /// 認証処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAppReviewAuthApiResults Auth(QoAppReviewAuthApiArgs args)
        {
            var results = new QoAppReviewAuthApiResults
            {
                IsSuccess = bool.FalseString
            };

            // ID照合
            if(args.Id != ReviewID)
            {
                return results;
            }
            
            var password = args.Password.TryEncrypt();
            // パスワード照合
            if(password != ReviewPass)
            {
                return results;
            }

            // 成功
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }
    }
}
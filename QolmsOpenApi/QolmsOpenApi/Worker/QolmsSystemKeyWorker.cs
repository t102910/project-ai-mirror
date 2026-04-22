using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// QOLMS 内部利用用のAPIキーを生成
    /// </summary>
    public class QolmsSystemKeyWorker
    {
        /// <summary>
        /// Api キーを生成して返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoQolmsSystemKeyGenerateApiResults Generate(QoQolmsSystemKeyGenerateApiArgs args)
        {
            var result = new QoQolmsSystemKeyGenerateApiResults() { IsSuccess = bool.FalseString };
            // カレンダーとポイントと待ち受けが使えるキー
            var func = QoApiFunctionTypeEnum.Calendar | QoApiFunctionTypeEnum.Point | QoApiFunctionTypeEnum.Tis | QoApiFunctionTypeEnum.Waiting | QoApiFunctionTypeEnum.HealthRecord;
            //var func = QoApiFunctionTypeEnum.Calendar | QoApiFunctionTypeEnum.Point | QoApiFunctionTypeEnum.Tis | QoApiFunctionTypeEnum.Waiting;
            var encExecutor = args.Executor;
            using (var crypt=new QsCrypt ( QsCryptTypeEnum.QolmsSystem ))
            {
                encExecutor = crypt.EncryptString(args.Executor);
            }
            result.QolmsSystemKey = new QsJwtTokenProvider().CreateOpenApiJwtApiKey(encExecutor, (int)func );    //（Default）有効期限３日のキーができる

            if (!string.IsNullOrWhiteSpace(result.QolmsSystemKey))
                result.IsSuccess = bool.TrueString;

            return result;

        }
    }
}
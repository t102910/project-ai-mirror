using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// バイタル情報のバッチ登録を行います
    /// </summary>
    public class HealthRecordBatchWorker
    {
        readonly IHealthRecordQueueRepository _repo;
        readonly IHealthRecordValidator _validator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="validator"></param>
        public HealthRecordBatchWorker(IHealthRecordQueueRepository repository ,IHealthRecordValidator validator)
        {
            _repo = repository;
            _validator = validator;
        }

        /// <summary>
        /// バイタル情報をDBに登録するためのバッチ登録処理を行います。
        /// ここではDBには登録しません。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoHealthRecordImportApiResults ImportBatch(QoHealthRecordImportApiArgs args)
        {
            var result = new QoHealthRecordImportApiResults
            {
                IsSuccess = bool.FalseString
            };

            // 引数チェック
            if (args == null)
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrWhiteSpace(args.ActorKey))
            {
                throw new ArgumentException();
            }
            if (args.VitalValueN == null || !args.VitalValueN.Any())
            {
                throw new ArgumentException();
            }
            if (string.IsNullOrWhiteSpace(args.Executor))
            {
                throw new ArgumentException();
            }

            // バイタル値チェック
            var (isValid, error) = _validator.Validate(args.VitalValueN);
            if (!isValid)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, error);
                return result;
            }

            var accountKey = Guid.Parse(args.ActorKey);

            // QueueStorageへの登録
            if (!_repo.Enqueue(accountKey, args.VitalValueN))
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.StorageError, "キューの登録に失敗しました。");
                return result;
            }

            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            result.IsSuccess = bool.TrueString;
            return result;
        }
    }
}
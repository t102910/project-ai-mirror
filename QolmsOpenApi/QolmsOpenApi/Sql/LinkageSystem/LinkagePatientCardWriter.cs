using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class LinkagePatientCardWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, LinkagePatientCardWriterArgs, LinkagePatientCardWriterResults>
    {


        /// <summary>
        /// <see cref="LinkagePatientCardWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkagePatientCardWriter() : base()
        {
        }



        /// <summary>
        /// 分散トランザクションを使用してデータベーステーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public LinkagePatientCardWriterResults ExecuteByDistributed(LinkagePatientCardWriterArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinkagePatientCardWriterResults result = new LinkagePatientCardWriterResults() { IsSuccess = false };
            DbPatientCardWriterCore patientCardWriter = new DbPatientCardWriterCore(args.ActorKey);
            DateTime actionDate = DateTime.Now;
            Guid actionKey = Guid.NewGuid();

            string errorMessage = string.Empty;

            if (patientCardWriter.WritePatientCardSet(args.ActorKey,args.CardCode,args.Sequence,args.
                FacilityKey,args.PatientCardSet,(byte)args.StatusType,args.DeleteFlag, actionDate, actionKey, ref errorMessage, out var entity))
            {
                // 処理件数
                result.Result = 1;

                // 操作日時
                result.ActionDate = actionDate;

                // 操作キー
                result.ActionKey = actionKey;

                // エラーメッセージ
                result.ErrorMessage = errorMessage;

                result.Entity = entity;

                // 成功
                result.IsSuccess = true;
            }
            else
                // 失敗
                throw new InvalidOperationException();
           
            return result;
        }
    }


}
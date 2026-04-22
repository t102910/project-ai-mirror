using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// お薬手帳情報を、
    /// データベーステーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class NoteMedicineWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, NoteMedicineWriterArgs, NoteMedicineWriterResults>
    {
        /// <summary>
        /// <see cref="NoteMedicineWriter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineWriter() : base()
        {
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public NoteMedicineWriterResults ExecuteByDistributed(NoteMedicineWriterArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new NoteMedicineWriterResults() { IsSuccess = false };
            var medicineWriter = new DbNoteMedicineWriterCore(args.ActorKey);
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();

            string refDataId = string.Empty;
            int refSeq = 0;
            if (medicineWriter.WriteMedicineSet(args,actionDate,actionKey,ref refDataId, ref refSeq))
            {
                // 処理件数
                result.Result = 1;

                // 操作日時
                result.ActionDate = actionDate;

                // 操作キー
                result.ActionKey = actionKey;

                // 成功
                result.IsSuccess = true;

                result.DataId = refDataId;
                result.Sequence = refSeq;
            }
            else
            {
                throw new InvalidOperationException();
            }

            return result;

        }
    }

 }
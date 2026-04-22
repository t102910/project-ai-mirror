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
    /// データベーステーブルから削除するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class NoteMedicineDeleter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, NoteMedicineDeleterArgs, NoteMedicineDeleterResults>
    {
        /// <summary>
        /// <see cref="NoteMedicineDeleter" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineDeleter() : base()
        {
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public NoteMedicineDeleterResults ExecuteByDistributed(NoteMedicineDeleterArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new NoteMedicineDeleterResults() { IsSuccess = false };
            var medicineWriter = new DbNoteMedicineWriterCore(args.ActorKey);
            var actionDate = DateTime.Now;
            var actionKey = Guid.NewGuid();
            List<DbMedicineKeyItem> refDeletedKeys = null;

            int count = medicineWriter.DeleteFromDataId(args, actionDate, actionKey, ref refDeletedKeys);
            if (count > 0)
            {
                // 処理件数
                result.Result = count;

                // 操作日時
                result.ActionDate = actionDate;

                // 操作キー
                result.ActionKey = actionKey;

                // 成功
                result.IsSuccess = true;

                result.DeletedKeys = refDeletedKeys;

            }
            else
            {
                // 失敗
                throw new InvalidOperationException();
            }

            return result;

        }
    }

 }
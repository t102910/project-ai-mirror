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
    internal sealed class NoteMedicineReader : QsDbReaderBase, IQsDbDistributedReader<QH_MEDICINE_DAT, NoteMedicineReaderArgs, NoteMedicineReaderResults>
    {
        /// <summary>
        /// <see cref="NoteMedicineReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoteMedicineReader() : base()
        {
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public NoteMedicineReaderResults ExecuteByDistributed(NoteMedicineReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            var result = new NoteMedicineReaderResults() { IsSuccess = false };
            var medicineReader = new DbNoteMedicineReaderCore(args.AccountKey);

            var dataTypeList = new List<byte>();

            dataTypeList.Add(args.DataType);
            //dataTypeList.Add(DirectCast(args.DataType, QH_MEDICINE_DAT.DataTypeEnum))

            switch ((QH_MEDICINE_DAT.DataTypeEnum)args.DataType)
            {
                case QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug:
                    // 調剤薬ならSSMIXから取り込んだデータも対象に
                    dataTypeList.Add((byte)QH_MEDICINE_DAT.DataTypeEnum.Ssmix);
                    break;
                case QH_MEDICINE_DAT.DataTypeEnum.OtcDrug:
                    // 市販薬写真も取得対象に
                    dataTypeList.Add((byte)QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto);
                    break;
            }

            if (args.LastAccessDate != DateTime.MinValue && medicineReader.IsModified(dataTypeList, args.LastAccessDate))
            {
                //追加・削除されたデータによってページングがずれるため、全データをリロードしてもらう
                result.IsModified = true;
                result.IsSuccess = true;
            }
            else
            {
                var pageIndex = 0;
                var maxPageIndex = 0;
                result.IsModified = false;
                result.LastAccessDate = DateTime.Now;

                // お薬手帳の情報のリスト
                result.Result = medicineReader.ReadMedicineList(dataTypeList, args.PageSize, args.PageIndex,ref pageIndex, ref maxPageIndex);
                result.PageIndex = pageIndex;
                result.MaxPageIndex = maxPageIndex;

                // 成功
                result.IsSuccess = true;
            }
            return result;

        }
    }

 }
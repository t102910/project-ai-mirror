using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{

    /// <summary>
    /// 利用者カードの一覧を、取得するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class LinkagePatientCardReader : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, LinkagePatientCardReaderArgs, LinkagePatientCardReaderResults>
    {


        /// <summary>
        /// <see cref="LinkagePatientCardReader" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkagePatientCardReader() : base()
        {
        }



        /// <summary>
        /// 分散トランザクションを使用してデータベーステーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public LinkagePatientCardReaderResults ExecuteByDistributed(LinkagePatientCardReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            LinkagePatientCardReaderResults result = new LinkagePatientCardReaderResults() { IsSuccess = false };
            DbPatientCardReaderCore patientCardReader = new DbPatientCardReaderCore(args.ActorKey); // TODO: AuthorKeyも渡す
            var storageReader = new DbFileStorageReaderCore<QH_UPLOADFILE_DAT>(args.AuthorKey, args.ActorKey);

            List<DbPatientCardItem> cardList = new List<DbPatientCardItem>();
            cardList = patientCardReader.ReadPatientCardListConsideringDisplayFlag();
           
            // 添付ファイルのリスト
            foreach (DbPatientCardItem item in cardList)
            {
                if (item.AttachedFileN.Count > 0)
                    item.AttachedFileN.ForEach(i => { i.OriginalName = storageReader.GetFileStorageItem(i.FileKey).OriginalName; });
            }

            // 利用者カードのリスト
            result.PatientCardItemN = cardList;

            // 成功
            result.IsSuccess = true;
            

            return result;
        }
    }


}
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    
    internal sealed class LinkagePatientCardReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        /// <summary>
        /// 利用者カードのリストを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<DbPatientCardItem> PatientCardItemN { get; set; } = new List<DbPatientCardItem>();



        /// <summary>
        /// <see cref="LinkagePatientCardReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public LinkagePatientCardReaderResults() : base()
        {
        }
    }


}
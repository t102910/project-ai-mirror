using System;
using System.Collections.Generic;

using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 施設画像の内容を、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FacilityFileReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        /// ファイルキー情報のリストを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<Guid> FileKeyN { get; set; } = new List<Guid>();



        /// <summary>
        /// <see cref="FacilityFileReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FacilityFileReaderResults() : base()
        {
        }
    }


}
using System;
using System.Collections.Generic;
using System.Data.Common;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// プッシュ通知対象アカウントキーと通知元施設キーを、
    /// データベーステーブルから取得した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class NotificationTargetFromPatientIdReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {

        /// <summary>
        ///     プッシュ通知対象アカウントキーを取得または設定します。
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        public Guid TargetAccountKey { get; set; }

        /// <summary>
        ///     通知元施設キーを取得または設定します。
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        public Guid FromFacilityKey { get; set; }

        /// <summary>
        /// <see cref="NotificationTargetFromPatientIdReaderResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NotificationTargetFromPatientIdReaderResults() : base()
        {
        }
    }


}
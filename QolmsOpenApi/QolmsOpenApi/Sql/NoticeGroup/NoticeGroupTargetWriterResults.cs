using MGF.QOLMS.QolmsDbCoreV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi
{
    /// <summary>
    /// お知らせグループ対象者情報 を、
    /// データベース テーブルへ登録した結果を格納する戻り値クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    public sealed class NoticeGroupTargetWriterResults : QsDbWriterResultsBase
    {

        /// <summary>
        /// <see cref="NoticeGroupTargetWriterResults" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public NoticeGroupTargetWriterResults() : base()
        {
        }
    }
}
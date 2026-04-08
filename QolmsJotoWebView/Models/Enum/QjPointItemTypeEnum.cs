using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Models
{
    /// <summary>
    /// 食事の種別を表します。
    /// </summary>
    public enum QjPointItemTypeEnum : byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = 0,

        /// <summary>
        /// 初回プレミアム登録ポイントです。
        /// </summary>
        InitialRegistration = 1,

        /// <summary>
        /// ログインポイントです。
        /// </summary>
        Login = 2,

        /// <summary>
        /// 歩数のポイントです。
        /// </summary>
        Walk5k = 3,
        Walk6k = 4,
        Walk7k = 5,
        Walk8k = 6,
        Walk9k = 7,
        Walk10k = 8,

        /// <summary>
        /// 運動のポイントです。
        /// </summary>
        Exercise = 9,

        /// <summary>
        /// 朝食登録のポイントです。
        /// </summary>
        Breakfast = 10,

        /// <summary>
        /// 昼食登録のポイントです。
        /// </summary>
        Lunch = 11,

        /// <summary>
        /// 夕食登録のポイントです。
        /// </summary>
        Dinner = 12,

        /// <summary>
        /// 間食登録のポイントです。
        /// </summary>
        Snack = 13,

        /// <summary>
        /// バイタル登録のポイントです。
        /// </summary>
        Vital = 14,

        /// <summary>
        /// 健診データ登録のポイントです。
        /// </summary>
        Examination = 15,

        /// <summary>
        /// データチャージ登録のポイントです。
        /// </summary>
        Datacharge = 16,

        /// <summary>
        /// ポイント交換登録のポイントです。
        /// </summary>
        PointExchange = 17,

        /// <summary>
        /// タニタ連携初回登録のポイントです。
        /// </summary>
        TanitaConnection = 18,

        /// <summary>
        /// auポイント交換登録のポイントです。
        /// </summary>
        AuPoint = 19,

        /// <summary>
        /// auポイント交換登録のポイントです。
        /// </summary>
        AmazonPoint = 20,

        /// <summary>
        /// ポイント修正登録のポイントです。
        /// </summary>
        RecoveryPoint = 21,

        /// <summary>
        /// 健診結果登録のポイントです。
        /// </summary>
        ExaminationPoint = 22,

        /// <summary>
        /// チャレンジ達成ポイントのポイントです。
        /// </summary>
        ChallengeCompleted = 23,

        /// <summary>
        /// 食事登録の ポイント です（2022/02/28 からの仕様）
        /// </summary>
        Meal = 24,

        /// <summary>
        /// ローカルポイント変換のポイントです。
        /// </summary>
        LocalPointRedeem = 25
    }
}
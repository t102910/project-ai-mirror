using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{ 
    /// <summary>
    /// JotoWebViewで使用する画面番号の種別を表します。
    /// </summary>
    public enum QjPageNoTypeEnum:byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        /// <remarks></remarks>
        /// SSOする画面番号と合わせてください。
        /// 
        None = 0,

        NoteExamination = 2,

        /// <summary>
        /// ポイント履歴 画面 
        /// </summary>
        PointHistory = 3,

        /// <summary>
        /// 地域ポイント履歴 画面
        /// </summary>
        PointLocalHistory = 4,

        /// <summary>
        /// 市民確認(ぎのわん) 画面
        /// </summary>
        PortalLocalIdVerification = 5,

        /// <summary>
        /// 健康年齢 画面
        /// </summary>
        HealthAge = 6,

        /// <summary>
        /// 連携設定 画面
        /// </summary>
        PortalConnectionSetting = 7,//暫定

        /// <summary>
        /// 医療費後払い 画面
        /// </summary>
        PortalMedicalPayment = 8,

        /// <summary>
        /// プレミアム 画面
        /// </summary>
        PremiumIndex = 9,

        /// <summary>
        /// カロミル食事 画面
        /// </summary>
        NoteCalomeal = 10,

        /// <summary>
        /// 問診(すながわ) 画面
        /// </summary>
        NoteMonshin = 11,

        /// <summary>
        /// auIDログイン
        /// </summary>
        LoginByAuId = 12, //暫定

        //ここよリ↑画面番号提供済みのため変更不可

        //使っていい空き番号です。,
        //13,

        //14,

        PortalSearchDetail = 15,
        
        /// <summary>
        /// 健康年齢 測定入力 画面
        /// </summary>
        HealthAgeEdit = 16,

        /// <summary>
        /// アマゾンギフト券交換 画面
        /// </summary>
        PointAmazonGiftCard = 17,

        /// <summary>
        /// 沖縄クリップマルシェ交換 画面
        /// </summary>
        PointCouponForOcm = 18,

        /// <summary>
        /// Popntaポイント交換 画面
        /// </summary>
        PointPonta = 19,

        /// <summary>
        /// オンラインストアクーポン交換 画面
        /// </summary>
        PointOnlineStore = 20,

        /// <summary>
        /// データチャージ 画面
        /// </summary>
        PointDatacharge = 21,

        //PortalSearchDetail,
        //HealthAgeEdit = 4,
        //PortalHome = 5,
        PortalHome = 254, //現状ないので後で修正して削除
        Demo = byte.MaxValue
    }
}
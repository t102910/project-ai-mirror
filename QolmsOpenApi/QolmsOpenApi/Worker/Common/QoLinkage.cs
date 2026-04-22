using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    internal class QoLinkage
    {
        /// <summary>
        /// Tisの連携システム番号
        /// </summary>
        public const int TIS_LINKAGE_SYSTEM_NO = 27003; //TISのLinkageSystemNo
        public const int JOTO_LINKAGE_SYSTEM_NO = 47003;    //Joto
        public const int JOTO_LINKAGE_SYSTEM_NO_FITBIT = 47011;    //Joto fitbit        
        public const int JOTO_LINKAGE_SYSTEM_NO_NAVITIME = 47006;    //Joto Navitime        
        public const int JOTO_LINKAGE_SYSTEM_NO_CALOMEAL = 47015;    //Joto Calomeal        
        /// <summary>
        /// JOTO 心拍見守りアプリ
        /// </summary>
        public const int JOTO_HEARTMONITOR_SYSTEM_NO = 47016;

        public const int QOLMS_MEDICINE_APP_LINKAGE_SYSTEM_NO = 99000;  //お薬手帳
        public const int QOLMS_NAVI_LINKAGE_SYSTEM_NO = 99001; // 医療ナビ

        /// <summary>
        /// JOTO ぎのわん PJ データ 基盤から
        /// </summary>
        public const int JOTO_GINOWAN_SYSTEM_NO = 47900021;

        /// <summary>
        /// 健康DIARYの連携システム番号
        /// </summary>
        public const int HEALTHDIARY_LINKAGE_SYSTEM_NO = 99002;

        /// <summary>
        /// MEIナビの連携システム番号
        /// </summary>
        public const int MEINAVI_LINKAGE_SYSTEM_NO = 99004;

        /// <summary>
        /// QOLMS共通システム番号(KAGAMINO/お薬手帳など連携システムなしのMGFアプリ)
        /// </summary>
        public const int QOLMS_COMMON_LINKAGE_SYSTEM_NO = 99999;
    }
}

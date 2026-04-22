using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Enums
{
    /// <summary>
    /// 認証種別
    /// </summary>
    [Flags()]
    public enum QoApiAuthorizeTypeEnum:byte
    {
        /// <summary>
        /// なし
        /// </summary>
        None = 0,
        /// <summary>
        /// Jwtトークン
        /// </summary>
        JwtToken = 1,
        /// <summary>
        /// Jwtアクセスキー
        /// </summary>
        JwtAccessKey = 2,
        /// <summary>
        /// Basic認証
        /// </summary>
        Basic = 4,
        /// <summary>
        /// QOLMSAPIきー
        /// </summary>
        JwtQolmsApiKey = 8,
        /// <summary>
        /// JOTOAPIキー
        /// </summary>
        JwtJotoApiKey = 16,
        /// <summary>
        /// 無効
        /// </summary>
        Invalid = 255,
    }
}
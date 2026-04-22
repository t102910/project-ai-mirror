using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// IdentityApiのアカウント登録系APIのRepository用の戻り値
    /// </summary>
    public class IdentityRegisterApiResults
    {
        /// <summary>
        /// 成否
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 生成されたアカウントキー
        /// </summary>
        public string AccountKey { get; set; }
        /// <summary>
        /// エラーリスト
        /// </summary>
        public List<string> ErrorList { get; set; } = new List<string>();
    }
}
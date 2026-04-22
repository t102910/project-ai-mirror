using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Models
{
    /// <summary>
    /// Azure Notification Hubs へのリクエスト用モデル
    /// </summary>
    public class NotificationRequest
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 内容
        /// </summary>
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// カスタムデータJsonを入れる
        /// </summary>
        public string Extra { get; set; } = string.Empty;
        
        //public string[] Tags { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 送信日
        /// </summary>
        public DateTime ScheduleDate { get; set; } = DateTime.Now;

        
        /// <summary>
        /// タグ式文字列を取得または設定します。
        /// </summary>
        /// <remarks>
        /// A tag expression is any boolean expression constructed using the logical operators AND (&&), OR (||), NOT (!), 
        /// and round parentheses. For example: (A || B) && !C. If an expression uses only ORs, 
        /// it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</remarks>
        public string TagExpression { get; private set; } = string.Empty;

        /// <summary>
        /// TagsをAndでつないだタグ式文字列を設定します。
        /// </summary>
        public void SetTagExpressionJoinAllAnd(string[] tags){
            if (tags.Length > 6)
                throw new ArgumentOutOfRangeException("tags","tagは、6個までしか指定できない");

            TagExpression = string.Join(" && ",tags); 
        }
        /// <summary>
        /// TagsをOrでつないだタグ式文字列を設定します。
        /// </summary>
        public void SetTagExpressionJoinAllOr(string[] tags)
        {
            if (tags.Length > 6)
                throw new ArgumentOutOfRangeException("tags", "tagは、All ORでも20個までしか指定できない");

            TagExpression = string.Join(" || ", tags); 
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Silent { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int Badge { get; set; } = -1;
    }
}
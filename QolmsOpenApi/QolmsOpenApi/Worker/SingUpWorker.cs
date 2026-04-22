using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 「アカウント仮登録」「本登録」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SignUpWorker
    {


        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private SignUpWorker() { }

        
        /// <summary>
        /// 文字列のエンコードを変換する
        /// キャリアメール受信時に、件名が文字化けする対応
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string EncodeMailHeader(string title)
        {
            // 「=?iso-2022-jp?B?<エンコード文字列>?=」形式に変換
            Encoding enc = Encoding.GetEncoding("iso-2022-jp");
            string str = Convert.ToBase64String(enc.GetBytes(title));

            return string.Format("=?{0}?B?{1}?=", "iso-2022-jp", str);
        }


        /// <summary>
        /// アカウント仮登録前の表示機能を提供します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int SignUpRead(string mailAddress)
        {
            if (!string.IsNullOrWhiteSpace(mailAddress))
            {
                // API呼び出し テーブルセレクト   
                return QoIdentityClient.ExecuteSignUpReadApi(mailAddress).MailAddressCount;
            }
            else
                return int.MinValue;
        }

        /// <summary>
        /// アカウント仮登録の機能を提供します。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Guid SignUpWrite(string mailAddress)
        {
            Guid key = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(mailAddress))
            {
                // API呼び出し API側で、DBテーブル登録　登録が成功すれば、仮アカウントキーを返却    
                key = QoIdentityClient.ExecuteSignUpWriteApi(mailAddress).Accountkey;
            }
            return key;
        }

        /// <summary>
        /// アカウント本登録の機能を提供します。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorList"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool RegisterWrite(QoApiAccountRegisterItem model, ref List<string> errorList)
        {
            if (model != null)
            {
                // IdentityAPI呼び出し API側で、DBテーブル登録
                QiQolmsAccountRegisterWriteApiResults result = QoIdentityClient.ExecuteRegisterWriteApi(model);
                 errorList = result.ErrorList;

                if (result.IsSuccess == bool.TrueString && (errorList == null || errorList.Count == 0))
                    return true;                
            }

            return false;
        }

        /// <summary>
        /// URLに含まれたキーの値をチェックします。
        /// </summary>
        /// <param name="accountkey"></param>
        /// <param name="mailaddress"></param>
        /// <param name="expiresOk"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool CheckTheKey(Guid accountkey, out string mailaddress, out bool expiresOk)
        {
            mailaddress = string.Empty;
            expiresOk = false;

            // URLから受け取ったkeyの値を復号化
            // API呼び出し API側で、DBテーブル検索　検索が成功すれば、メールアドレスを返却  
            QiQolmsAccountRegisterReadApiResults result = QoIdentityClient.ExecuteRegisterReadApi(accountkey);

            if (!string.IsNullOrWhiteSpace(result.MailAddress) && !string.IsNullOrWhiteSpace(result.Expires))
            {
                mailaddress = result.MailAddress;
                // 有効期限が切れていないか
                if (DateTime.Parse(result.Expires) >= DateTime.Now)
                    expiresOk = true;
                else
                    expiresOk = false;
                return true;
            }
            else
                return false;
        }
    }

}
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class NoteMonshinWorker
    {

        public static string CreateRedirectUrl(QolmsJotoModel mainModel)
        {
            //連携がある状態でしか呼び出されない
            //ボタンの表示制御　連携がある場合のみ表示される
            //47000020 すながわ内科
            //アカウント情報取得
            var args = new QjCommonLinkageReadApiArgs(QjApiTypeEnum.CommonLinkageRead,
                                                      QsApiSystemTypeEnum.QolmsJoto,
                                                      mainModel.ApiExecutor,
                                                      mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                LinkageSystemNo = $"47000020"
            };
            var linkageDat = ExecuteLinkageReadApi(mainModel, args);
            
            
            //・診察券番号
            //・氏名（JOTO登録情報）※全角
            //・カナ姓名（JOTO登録情報）※全角
            //・性別（JOTO登録情報）※半角数値（1=男、2=女、3=不明）
            //・生年月日（JOTO登録情報）※半角（YYYY-MM-DD）
            //・呼び出し日時※半角（yyyyMMddHHmmss）
            //JSON形式の文字列をACS暗号化してGETパラメータとして受け渡してください。
            //※プロパティ名と文字列は、ダブルクオーテーションで囲む

            var jsonParam = new MonshinArgsJsonParamater()
            {
                patientID = linkageDat.LinkageSystemId,
                patientName = mainModel.AuthorAccount.Name,
                patientNameKana = mainModel.AuthorAccount.KanaName,
                gendar = $"{(byte)mainModel.AuthorAccount.SexType}",
                birthday = mainModel.AuthorAccount.Birthday.ToString("yyyy-MM-dd"),
                timestamp = DateTime.Now.ToString("yyyyMMddHHmmss")
            };

            //　パラメータ例）{"patientID": "0000001", "patientName": "砂川　太郎", "patientNameKana": "スナガワ　タロウ", "gendar": 1, "birthday": "2000-01-01", "timestamp": "20240913123456" }
            var param = jsonParam.ToJsonString();
            var basicPass = HttpUtility.UrlEncode(QjConfiguration.BasicPass);

            //※パラメータは ACS 暗号化して受け渡しを行う
            var crptPrm = AesEncrypt(QjConfiguration.AesKey, QjConfiguration.AesIv, param);
            crptPrm = HttpUtility.UrlEncode(crptPrm);
            var u = new Uri(QjConfiguration.MonshinUrl);
            return $"{u.Scheme}://{QjConfiguration.BasicUserId}:{basicPass}@{u.Host}{u.PathAndQuery}{crptPrm}";
       
        }

        #region Constant

        
        #endregion

        #region PrivateMethod

        private static QjCommonLinkageReadApiResults ExecuteLinkageReadApi(QolmsJotoModel mainModel, QjCommonLinkageReadApiArgs args)
        {

            return QsApiManager.ExecuteQolmsJotoApi<QjCommonLinkageReadApiResults>(args, mainModel.SessionId, mainModel.ApiAuthorizeKey2);
        }

        private static string AesEncrypt(string key, string iv, string param)
        {

            try
            {
                // キーと IV のサイズを設定
                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = Encoding.UTF8.GetBytes(iv);

                    // 暗号化用のトランスフォームを作成
                    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(param);
                            }
                        }
                        // 暗号化されたデータを Base64 に変換
                        return Convert.ToBase64String(ms.ToArray());
                    }

                }
            }
            catch
            {
                return string.Empty;
            }            


        }
        
        #endregion
    }
}













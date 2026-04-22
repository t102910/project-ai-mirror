using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Linq;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsLineMessageApiCoreV1;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.IO;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 検査結果関連の機能を提供します。
    /// </summary>
    public class LineMessageWorker
    {

        #region "Private Method"

        //lineUserIdからLinePreRegistを取得。
        private static bool IsLinePreRegistDelete(string lineUserId)
        {
            var reader = new LinePreRegistReader();

            try
            {
                var readerArgs = new LinePreRegistReaderArgs() { LineUserId = lineUserId };

                //読込
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
                {
                    //結果格納
                    QH_LINEPREREGIST_DAT target = readerResults.Result.First();
                    return target.DELETEFLAG;
                }
                else
                {
                    QoAccessLog.WriteInfoLog(readerResults.Result.Count.ToString());
                    QoAccessLog.WriteErrorLog(string.Format($"[IsLinePreRegistDelete]QH_LINEPREREGIST_DAT情報の取得に失敗しました。"), Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return true;
        }

        //lineUserIdからLinePreRegistを取得。
        private static QH_LINEPREREGIST_DAT ReadLinePreRegist(string lineUserId)
        {
            var reader = new LinePreRegistReader();

            try
            {
                var readerArgs = new LinePreRegistReaderArgs() { LineUserId = lineUserId };

                //読込
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
                {
                    //結果格納
                    QH_LINEPREREGIST_DAT target = readerResults.Result.First();
                    return target;
                }
                else
                {
                    QoAccessLog.WriteInfoLog(readerResults.Result.Count.ToString());
                    QoAccessLog.WriteErrorLog(string.Format($"[IsLinePreRegistDelete]QH_LINEPREREGIST_DAT情報の取得に失敗しました。"), Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return new QH_LINEPREREGIST_DAT();
        }

        //LinkageSystemNoとlineUserIdからLineOpenIdを取得。
        private static string GetLineOpenId(int linkageSystemNo, string lineUserId)
        {
            var reader = new LineOpenIdManagementReader();

            try
            {
                var readerArgs = new LineOpenIdManagementReaderArgs() { LinkageSystemNo = linkageSystemNo, LineUserId = lineUserId };

                //読込
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
                {
                    //結果格納
                    QH_OPENIDMANAGEMENT_DAT target = readerResults.Result.First();
                    return target.OPENID;
                }
                else
                {
                    QoAccessLog.WriteInfoLog(readerResults.Result.Count.ToString());
                    QoAccessLog.WriteErrorLog(string.Format($"[GetLineOpenId]QH_OPENIDMANAGEMENT_DAT情報の取得に失敗しました。"), Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return string.Empty;
        }

        //LinkageSystemNoとLinkageSystemIdからLineUserIdを取得。
        private static string GetLineUserId(int linkageSystemNo, string linkageSystemId)
        {
            var reader = new LineUserIdReader();
            
            try
            {
                var readerArgs = new LineUserIdReaderArgs() { LinkageSystemNo = linkageSystemNo, LinkageSystemId = linkageSystemId };

                //読込
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
                {
                    //結果格納
                    QH_OPENIDMANAGEMENT_DAT target = readerResults.Result.First();
                    return target.OPENID;
                }
                else
                {
                    QoAccessLog.WriteInfoLog(readerResults.Result.Count.ToString());
                    QoAccessLog.WriteErrorLog(string.Format($"[GetLineUserId]QH_OPENIDMANAGEMENT_DAT情報の取得に失敗しました。"), Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return string.Empty;
        }

        //LinkageSystemNoとLinkageSystemIdからLineUserIdを取得。
        private static List<string> GetLineUserId(int linkageSystemNo)
        {
            var reader = new LineUserIdReader();

            try
            {
                var readerArgs = new LineUserIdReaderArgs() { LinkageSystemNo = linkageSystemNo };

                //読込
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count > 0)
                {
                    List<string> ret = new List<string>();
                    //結果格納
                    foreach (var id in readerResults.Result)
                    {
                        ret.Add(id.OPENID);
                    }
                    return ret;
                }
                else
                {
                    QoAccessLog.WriteInfoLog(readerResults.Result.Count.ToString());
                    QoAccessLog.WriteErrorLog(string.Format($"[GetLineUserId]QH_OPENIDMANAGEMENT_DAT情報の取得に失敗しました。"), Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return new List<string>();
        }

        //LinkageSystemNoを基にLINKAGESYSTEM_MSTと照合。存在した場合、QhLinkageSystemLinkageSetOfJsonを取得
        private static QhLinkageSystemLinkageSetOfJson GetChannelAccessToken(int linkageSystemNo)
        {
            var reader = new LineLinkageSetReader();
            try
            {
                //読込
                var readerArgs = new LineLinkageSetReaderArgs() { LinkageSystemNo = linkageSystemNo };
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1)
                {
                    return new QolmsDbEntityV1.QsJsonSerializer().Deserialize<QhLinkageSystemLinkageSetOfJson>(readerResults.Result.First().LINKAGESET);
                }

                else
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[GetChannelAccessToken]LINKAGESYSTEMMST情報の取得に失敗しました。"), Guid.Empty);
                }
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }

            return new QhLinkageSystemLinkageSetOfJson();
        }

        //QH_LINEPREREGIST_DATへのLinkageSystemId、Birthdayの登録。
        private static bool WriteLinePreRegist(string userId, string linkageSystemId, DateTime birthday, bool deleteFlag)
        {

            try
            {
                var writer = new LinePreRegistWriter();

                //QH_LINEPREREGIST_DATに登録
                var writerArgs = new LinePreRegistWriterArgs() { UserId = userId, LinkageSystemId = linkageSystemId, Birthday = birthday, DeleteFlag = deleteFlag };
                var writerResults = QsDbManager.Write(writer, writerArgs);

                //登録失敗時
                if (!(writerResults != null && writerResults.IsSuccess))
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[WriteLinePreRegist]QH_LINEPREREGIST_DATへの登録に失敗しました。"), Guid.Empty);
                }
                return writerResults.IsSuccess;
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return false;
            }


        }

        //QH_OPENIDMANAGEMENT_DATへのUserId紐づけ。
        private static bool WriteLineOpenIdManagement(string userId, int linkageSystemNo, bool deleteFlag)
        {
            try
            {
                var writer = new LineOpenIdManagementWriter();

                //QH_OPENIDMANAGEMENT_DATに登録
                var writerArgs = new LineOpenIdManagementWriterArgs() { UserId = userId, LinkageSystemNo = linkageSystemNo, DeleteFlag = deleteFlag };
                var writerResults = QsDbManager.Write(writer, writerArgs);


                //登録更新失敗時
                if (!(writerResults != null && writerResults.IsSuccess))
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[WriteLineOpenIdManagement]QH_OPENIDMANAGEMENT_DATへの登録更新に失敗しました。"), Guid.Empty);
                }

                return writerResults.IsSuccess;
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return false;
            }

        }

        //署名検証
        private static bool ValidSignature(string body, string xLineSignature, string channelSecret)
        {

            byte[] secretBytes = Encoding.UTF8.GetBytes(channelSecret);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

            using (HMACSHA256 hmac = new HMACSHA256(secretBytes))
            {
                byte[] hash = hmac.ComputeHash(bodyBytes);
                string signature = Convert.ToBase64String(hash);
                return signature == xLineSignature;
            }
        }

        //フォローイベント処理。
        private static void FollowEvent1(string accessToken, string replyToken, string userId)
        {
            try
            {
                //挨拶メッセージより先にメッセージが送信されてしまうため2秒後に送信
                Thread.Sleep(2000);

                QoAccessLog.WriteInfoLog("FollowEvent1");
                //var message1 = $"女性の美と健康を応援する、セントラルクリニックグループの公式アカウントへようこそ！\r\n" +
                //               $"健診、人間ドッグの予約や、結果の確認ができる、便利な機能をお届けします。";
                var message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFollowEventMessage1.txt"));

                var message2 = $"はい";
                var message3 = $"いいえ";

                LineMessageApiArgs<LineTemplateMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineTemplateMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message1,
                        template = new LineTemplateOfJson()
                        {
                            type = "confirm",
                            text = message1,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "message",
                                    label = message3,
                                    text = message3
                                },
                                new LineActionOfJson()
                                {
                                    type = "message",
                                    label = message2,
                                    text = message2
                                }
                            }
                        }
                    }
                );

                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }
            
        }

        //追加フォローイベント処理。
        private static void FollowEvent2(string accessToken, string replyToken, string userId, bool root)
        {
            try
            {
                QoAccessLog.WriteInfoLog("FollowEvent2");
                var message1 = string.Empty;
                var message2 = string.Empty;
                var message3 = string.Empty;
                if (root)
                {
                    message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFollowEventMessage2.txt"));
                    message2 = $"登録する";
                    message3 = $"登録しない";
                }
                else
                {
                    message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFollowEventMessage3.txt"));
                    message2 = $"予約する";
                    message3 = $"予約しない";
                }

                LineMessageApiArgs<LineTemplateMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineTemplateMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message1,
                        template = new LineTemplateOfJson()
                        {
                            type = "confirm",
                            text = message1,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "message",
                                    label = "いいえ",
                                    text = message3
                                },
                                new LineActionOfJson()
                                {
                                    type = "message",
                                    label = "はい",
                                    text = message2
                                }
                            }
                        }
                    }
                );
                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //メッセージイベント処理。
        private static void MessageEvent(string accessToken, string replyToken, string message, string userId)
        {
            try
            {
                QoAccessLog.WriteInfoLog("MessageEvent");
                switch (message)
                {
                    //"連携する"
                    case "はい":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            //個人ID連携ルート
                            FollowEvent2(accessToken, replyToken, userId, true);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //"連携しない"
                    case "いいえ":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            //健診、ドック新規予約ルート
                            FollowEvent2(accessToken, replyToken, userId, false);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //"予約しない"
                    case "予約しない":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            //コラムへ飛ばす
                            SecondMessageEvent(accessToken, replyToken);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //"予約する"
                    case "予約する":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            //健診、ドック新規予約
                            FirstMessageEvent2(accessToken, replyToken, userId);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //"登録しない"
                    case "登録しない":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            //コラムへ飛ばす
                            SecondMessageEvent(accessToken, replyToken);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //"登録する"
                    case "登録する":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            //健診、ドック新規予約
                            FirstMessageEvent1(accessToken, replyToken, userId);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //"同意しない"
                    case "同意しない":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            //コラムへ飛ばす
                            SecondMessageEvent(accessToken, replyToken);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //"同意する"
                    case "同意する":
                        //PreRegistデータが作成状態の場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            WriteLinePreRegist(userId, string.Empty, DateTime.MinValue, false);
                            ThirdMessageEvent(accessToken, replyToken, userId);
                        }
                        else
                        {
                            ErrorEvent(accessToken,
                            userId,
                            System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                        }
                        break;
                    //上記以外
                    default:
                        //PreRegistデータが削除状態、または作成されていない場合応答しない
                        if (IsLinePreRegistDelete(userId))
                        {
                            ErrorEvent(accessToken,
                                userId,
                                System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineBadRequestMessage1.txt")));
                            FollowEvent1(accessToken, replyToken, userId);
                        }
                        else
                        {
                            //個人ID
                            if (long.TryParse(message, out long id))
                            {
                                WriteLinePreRegist(userId, message, DateTime.MinValue, false);
                                FourthMessageEvent(accessToken, replyToken, userId);
                            }
                            else
                            {
                                ErrorEvent(accessToken, userId);
                                ThirdMessageEvent(accessToken, replyToken, userId);
                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //ポストバックイベント処理。
        private static void PostBackEvent(string accessToken, string replyToken, string userId, int linkageSystemNo, QoApiLineMessageEventPostBackItem postBack, string menuId)
        {
            try
            {
                QoAccessLog.WriteInfoLog("PostBackEvent");

                switch (postBack.Data)
                {
                    case QsLineMessageApiConfiguration.LINE_DATETIMEPICKER_DATANAME:
                        if (DateTime.TryParse(postBack.Params.ParamDate, out DateTime birthday))
                        {
                            WriteLinePreRegist(userId, string.Empty, birthday, false);
                        }
                        else
                        {
                            ErrorEvent(accessToken, userId);
                            FourthMessageEvent(accessToken, replyToken, userId);
                            break;
                        }

                        //生年月日リマインドメッセージ送信
                        ErrorEvent(accessToken, userId, string.Format(System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineDateTimeRemindMessage.txt")), birthday.ToString("yyyy年MM月dd日")));

                        if (WriteLineOpenIdManagement(userId, linkageSystemNo, false))
                        {
                            //RichMenuSet(accessToken, userId, menuId);
                            FifthPostBackEvent(accessToken, replyToken, userId);
                        }
                        else
                        {
                            ErrorEvent(accessToken, userId);
                            ThirdMessageEvent(accessToken, replyToken, userId);
                        }
                        break;
                    //上記以外
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //アンフォローイベント処理。
        private static void UnfollowEvent(string userId, int linkageSystemNo)
        {
            try
            {
                QoAccessLog.WriteInfoLog("UnfollowEvent");
                //OpenIdにDELETEFLAG「1」設定
                if (!WriteLineOpenIdManagement(userId, linkageSystemNo, true))
                {
                    QoAccessLog.WriteInfoLog("OpenId解除失敗");
                }
                //PreRegistDatにDELETEFLAG「1」設定
                if (!WriteLinePreRegist(userId, string.Empty, DateTime.MinValue, true))
                {
                    QoAccessLog.WriteInfoLog("PreRegistDat解除失敗");
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //Errorイベント処理。
        private static void ErrorEvent(string accessToken, string userId, string message = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = $"登録に失敗しました。\r\nもう一度入力してください。";
                }

                LinePushApiArgs<LineMessageOfJson> args = QsLineMessageApiHelper.BuildLinePushApiArgs<LineMessageOfJson>(accessToken, userId);
                //イベント追加
                args.messages.Add(
                    new LineMessageOfJson()
                    {
                        type = "text",
                        text = message
                    }
                );

                //APIリクエスト
                var pushResult = QsLineMessageApiManager.Execute<LinePushApiArgs<LineMessageOfJson>, LinePushApiResults>(args);
                QoAccessLog.WriteInfoLog(pushResult.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //「登録する」メッセージイベント処理。
        private static void FirstMessageEvent1(string accessToken, string replyToken, string userId)
        {
            try
            {
                //var message1 = $"連携は簡単2ステップ。\r\n" +
                //               $"まずは利用規約をご確認ください。";
                var message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFirstEventMessage1.txt"));
                var label = $"利用規約を見る";

                var message2 = $"利用規約に";

                var message3 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFirstEventMessage3.txt"));

                LinePushApiArgs<LineMessageOfJson> pushArgs = QsLineMessageApiHelper.BuildLinePushApiArgs<LineMessageOfJson>(accessToken, userId);
                //イベント追加
                pushArgs.messages.Add(
                    new LineMessageOfJson()
                    {
                        type = "text",
                        text = message3
                    }
                );

                //APIリクエスト
                var pushResult = QsLineMessageApiManager.Execute<LinePushApiArgs<LineMessageOfJson>, LinePushApiResults>(pushArgs);

                LineMessageApiArgs<LineTemplateMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineTemplateMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message1,
                        template = new LineTemplateOfJson()
                        {
                            type = "buttons",
                            text = message1,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "uri",
                                    label = label,
                                    uri = QoApiConfiguration.LineFirstEventUrl
                                }
                            }
                        }
                    }
                );
                args.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message2,
                        template = new LineTemplateOfJson()
                        {
                            type = "confirm",
                            text = message2,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "message",
                                    label = "同意しない",
                                    text = "同意しない"
                                },
                                new LineActionOfJson()
                                {
                                    type = "message",
                                    label = "同意する",
                                    text = "同意する"
                                }
                            }
                        }
                    }
                );

                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //「予約する」メッセージイベント処理。
        private static void FirstMessageEvent2(string accessToken, string replyToken, string userId)
        {
            try
            {
                QoAccessLog.WriteInfoLog("FirstMessageEvent2");

                var message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFirstEventMessage2.txt"));
                var label = "新規予約";

                LineMessageApiArgs<LineTemplateMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineTemplateMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message1,
                        template = new LineTemplateOfJson()
                        {
                            type = "buttons",
                            text = message1,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "uri",
                                    label = label,
                                    uri = QoApiConfiguration.LineFollowEventUrl
                                }
                            }
                        }
                    }
                );

                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }


        //「同意しない」メッセージイベント処理。
        private static void SecondMessageEvent(string accessToken, string replyToken)
        {
            try
            {

                //var message1 = $"この度はセントラルF公式アカウントに友だち登録いただきありがとうございました。\r\n" +
                //               $"今後ともセントラルグループをよろしくお願いいたします。";
                var message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineSecondEventMessage1.txt"));
                var label = $"セントラルクリニック";

                LineMessageApiArgs<LineTemplateMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineTemplateMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message1,
                        template = new LineTemplateOfJson()
                        {
                            type = "buttons",
                            text = message1,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "uri",
                                    label = label,
                                    uri = QoApiConfiguration.LineSecondEventUrl
                                }
                            }
                        }
                    }
                );

                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //「同意する」メッセージイベント処理。
        private static void ThirdMessageEvent(string accessToken, string replyToken, string userId)
        {
            try
            {
                //イベント1
                QoAccessLog.WriteInfoLog("イベント開始");
                //var message1 = $"【ステップ１/２】\r\nセントラルクリニックの「個人ID」を入力してください。\r\n" +
                //               $"ハイフン（-）は入力必要ありません。";
                var message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineThirdEventMessage1.txt"));

                LineMessageApiArgs<LineMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineMessageOfJson()
                    {
                        type = "text",
                        text = message1
                    }
                );

                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineMessageOfJson>, LineMessageApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);

                //string url = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineThirdEventImage.jpg");
                string url = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineThirdEventImageUrl.txt"));
                QoAccessLog.WriteInfoLog(url);
                LinePushApiArgs<LineImageOfJson> pushArgs = QsLineMessageApiHelper.BuildLinePushApiArgs<LineImageOfJson>(accessToken, userId);
                //イベント追加
                pushArgs.messages.Add(
                    new LineImageOfJson()
                    {
                        type = "image",
                        originalContentUrl = url,
                        previewImageUrl = url
                    }
                );

                //APIリクエスト
                var pushResult = QsLineMessageApiManager.Execute<LinePushApiArgs<LineImageOfJson>, LinePushApiResults>(pushArgs);
                QoAccessLog.WriteInfoLog(pushResult.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //「個人ID」メッセージイベント処理。
        private static void FourthMessageEvent(string accessToken, string replyToken, string userId)
        {
            try
            {

                string url = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFourthEventImageUrl.txt"));
                QoAccessLog.WriteInfoLog(url);
                LinePushApiArgs<LineImageOfJson> pushArgs = QsLineMessageApiHelper.BuildLinePushApiArgs<LineImageOfJson>(accessToken, userId);
                //イベント追加
                pushArgs.messages.Add(
                    new LineImageOfJson()
                    {
                        type = "image",
                        originalContentUrl = url,
                        previewImageUrl = url
                    }
                );

                //APIリクエスト
                var pushResult = QsLineMessageApiManager.Execute<LinePushApiArgs<LineImageOfJson>, LinePushApiResults>(pushArgs);
                QoAccessLog.WriteInfoLog(pushResult.ResponseString);

                //var message1 = $"【ステップ２/２】\r\n生年月日を選択してください。";
                var message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFourthEventMessage1.txt"));

                LineMessageApiArgs<LineQuickReplyMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineQuickReplyMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineQuickReplyMessageOfJson()
                    {
                        type = "text",
                        text = message1,
                        quickReply = new LineQuickReplyOfJson()
                        {
                            items = new List<LineQuickReplyItemOfJson>()
                            {
                                new LineQuickReplyItemOfJson()
                                {
                                    type = "action",
                                    action = new LineActionOfJson()
                                    {
                                        type = "datetimepicker",
                                        label = QsLineMessageApiActionEnum.生年月日選択.ToString(),
                                        data = QsLineMessageApiConfiguration.LINE_DATETIMEPICKER_DATANAME,
                                        initial = "2000-01-01",
                                        mode = QsLineMessageApiDateTimePickerModeEnum.date.ToString()
                                    }
                                },
                            }
                        }
                        
                    }
                );

                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineQuickReplyMessageOfJson>, LineMessageApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);

            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //「生年月日選択」ポストバックイベント処理。
        private static void FifthPostBackEvent(string accessToken, string replyToken, string userId)
        {
            try
            {
                //イベント1
                QoAccessLog.WriteInfoLog("イベント開始");
                //var message1 = $"連携が完了しました！\r\n" +
                //               $"今後、このLINE公式アカウントから、予約や健診、人間ドックの結果が確認できます！\r\n" +
                //               $"あなたの健康のためにぜひご利用ください！";
                var message1 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFifthEventMessage1.txt"));

                var label = $"最終ステップに進む";

                var target = ReadLinePreRegist(userId);
                var id = string.Empty;
                var pass = string.Empty;
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    id = System.Web.HttpUtility.UrlEncode(crypt.EncryptString(target.LINKAGESYSTEMID));
                    pass = System.Web.HttpUtility.UrlEncode(crypt.EncryptString(target.BIRTHDAY.ToString("yyyyMMdd")));
                }

                LineMessageApiArgs<LineTemplateMessageOfJson> args = QsLineMessageApiHelper.BuildLineMessageApiArgs<LineTemplateMessageOfJson>(accessToken, replyToken);
                //イベント追加
                args.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message1,
                        template = new LineTemplateOfJson()
                        {
                            type = "buttons",
                            text = message1,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "uri",
                                    label = label,
                                    uri = string.Format(QoApiConfiguration.LineFifthEventUrl, id, pass)
                                }
                            }
                        }
                    }
                );

                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<LineMessageApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(args);

                //イベント1
                //QoAccessLog.WriteInfoLog("イベント開始");
                ////var message2 = $"このままでも便利にお使いいただけますが…\r\n" +
                ////               $"パスワードを登録するだけで、あなたの情報がバックアップできる、「セントラルサービス」に登録しませんか？\r\n" +
                ////               $"もちろんご利用は無料です！\r\n" +
                ////               $"便利なだけでなく、あなたに役立つ情報がきっと見つかる！そんなサービスとなっております♪";
                //var message2 = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineFifthEventMessage2.txt"));

                //var label = $"セントラルクリニック女性";

                //LinePushApiArgs<LineTemplateMessageOfJson> pushArgs = QsLineMessageApiHelper.BuildLinePushApiArgs<LineTemplateMessageOfJson>(accessToken, userId);
                ////イベント追加
                //pushArgs.messages.Add(
                //    new LineTemplateMessageOfJson()
                //    {
                //        type = "template",
                //        altText = message2,
                //        template = new LineTemplateOfJson()
                //        {
                //            type = "buttons",
                //            text = message2,
                //            actions = new List<LineActionOfJson>()
                //            {
                //                new LineActionOfJson()
                //                {
                //                    type = "uri",
                //                    label = label,
                //                    uri = QoApiConfiguration.LineFifthEventUrl
                //                }
                //            }
                //        }
                //    }
                //);

                ////APIリクエスト
                //var pushResult = QsLineMessageApiManager.Execute<LinePushApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(pushArgs);
                //QoAccessLog.WriteInfoLog(pushResult.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //RichMenuSet処理。
        private static void RichMenuSet(string accessToken, string userId, string menuId)
        {
            try
            {
                var args = new SetRichMenuApiArgs()
                {
                    AccessToken = accessToken,
                    LineUserId = userId,
                    RichMenuId = menuId
                };
                //APIリクエスト
                var result = QsLineMessageApiManager.Execute<SetRichMenuApiArgs, SetRichMenuApiResults>(args);
                QoAccessLog.WriteInfoLog(result.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //TemplatePushSend処理。
        private static void TemplatePushSend(string accessToken, string userId, string message, string label, string uri)
        {
            try
            {
                LinePushApiArgs<LineTemplateMessageOfJson> pushArgs = QsLineMessageApiHelper.BuildLinePushApiArgs<LineTemplateMessageOfJson>(accessToken, userId);
                //イベント追加
                pushArgs.messages.Add(
                    new LineTemplateMessageOfJson()
                    {
                        type = "template",
                        altText = message,
                        template = new LineTemplateOfJson()
                        {
                            type = "buttons",
                            text = message,
                            actions = new List<LineActionOfJson>()
                            {
                                new LineActionOfJson()
                                {
                                    type = "uri",
                                    label = label,
                                    uri = uri
                                }
                            }
                        }
                    }
                );
                //APIリクエスト
                var pushResult = QsLineMessageApiManager.Execute<LinePushApiArgs<LineTemplateMessageOfJson>, LineMessageApiResults>(pushArgs);
                QoAccessLog.WriteInfoLog(pushResult.ResponseString);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        //CustomPushSend処理。
        private static void CustomPushSend(string accessToken, List<string> toList, string altText ,string contents)
        {
            try
            {
                var serializer = new QolmsLineMessageApiCoreV1.QsJsonSerializer();
                var content = string.Empty;
                switch (toList.Count)
                {
                    //指定なし
                    case 0:
                        var bcArgs = new LineBroadcastCustomPushApiArgs<LineFlexMessageOfJson>()
                        {
                            AccessToken = accessToken,
                            messages = new List<LineFlexMessageOfJson>()
                            {
                                new LineFlexMessageOfJson()
                                {
                                    type = "flex",
                                    altText = altText,
                                    contents = contents
                                }
                            }
                        };
                        //整形
                        content = serializer.Serialize<LineBroadcastCustomPushApiArgs<LineFlexMessageOfJson>>(bcArgs);
                        content = content.Replace("\\", "");
                        content = content.Replace("rn", "");
                        content = content.Replace("\"{", "{");
                        bcArgs.Contents = content.Replace("}\"", "}");
                        QoAccessLog.WriteInfoLog(bcArgs.Contents);
                        var bcResult = QsLineMessageApiManager.Execute<LineBroadcastCustomPushApiArgs<LineFlexMessageOfJson>, LineBroadcastCustomPushApiResults>(bcArgs);
                        break;
                    //個人
                    case 1:
                        var psArgs = new LineCustomPushApiArgs<LineFlexMessageOfJson>()
                        {
                            AccessToken = accessToken,
                            to = toList.First(),
                            messages = new List<LineFlexMessageOfJson>()
                            {
                                new LineFlexMessageOfJson()
                                {
                                    type = "flex",
                                    altText = altText,
                                    contents = contents
                                }
                            }
                        };
                        //整形
                        content = serializer.Serialize<LineCustomPushApiArgs<LineFlexMessageOfJson>>(psArgs);
                        content = content.Replace("\\", "");
                        content = content.Replace("rn", "");
                        content = content.Replace("\"{", "{");
                        psArgs.Contents = content.Replace("}\"", "}");
                        QoAccessLog.WriteInfoLog(psArgs.Contents);
                        var psResult = QsLineMessageApiManager.Execute<LineCustomPushApiArgs<LineFlexMessageOfJson>, LineCustomPushApiResults>(psArgs);
                        break;
                    //複数指定
                    default:
                        var mcArgs = new LineMulticastCustomPushApiArgs<LineFlexMessageOfJson>()
                        {
                            AccessToken = accessToken,
                            to = toList,
                            messages = new List<LineFlexMessageOfJson>()
                            {
                                new LineFlexMessageOfJson()
                                {
                                    type = "flex",
                                    altText = altText,
                                    contents = contents
                                }
                            }
                        };
                        //整形
                        content = serializer.Serialize<LineMulticastCustomPushApiArgs<LineFlexMessageOfJson>>(mcArgs);
                        content = content.Replace("\\", "");
                        content = content.Replace("rn", "");
                        content = content.Replace("\"{", "{");
                        mcArgs.Contents = content.Replace("}\"", "}");
                        var mcResult = QsLineMessageApiManager.Execute<LineMulticastCustomPushApiArgs<LineFlexMessageOfJson>, LineMulticastCustomPushApiResults>(mcArgs);
                        break;
                }

            }
            catch (Exception ex)
            {
                QoAccessLog.WriteInfoLog(ex.Message);
            }

        }

        #endregion
        #region "Public Method"


        /// <summary>
        /// Webhook処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static LineWebhookEventResults Webhook(LineWebhookEventArgs args)
        {
            QoAccessLog.WriteInfoLog("Webhook受信");
            var result = new LineWebhookEventResults() {};

            //引数チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                QoAccessLog.WriteInfoLog("LinkageSystemNoチェック");
                return result;
            }
            
            //署名チェック
            QhLinkageSystemLinkageSetOfJson linkageSet = GetChannelAccessToken(linkageSystemNo);
            if (ValidSignature(args.body, args.XLineSignature, linkageSet.LineBotChannelSecret))
            {
                QoAccessLog.WriteInfoLog("署名チェック");
                return result;
            }
            
            //アクセストークンチェック
            if (string.IsNullOrWhiteSpace(linkageSet.LineBotChannelAccessToken))
            {
                QoAccessLog.WriteInfoLog("アクセストークンチェック");
                return result;
            }
            
            //Webhookイベント数分処理
            foreach (QoApiLineMessageEventItem item in args.events)
            {
                QoAccessLog.WriteInfoLog("イベント分岐");
                //イベントタイプによって処理
                switch ((QsLineMessageApiEventTypeEnum)Enum.Parse(typeof(QsLineMessageApiEventTypeEnum), item.Type))
                {
                    //フォローイベント
                    case QsLineMessageApiEventTypeEnum.follow:

                        FollowEvent1(linkageSet.LineBotChannelAccessToken, item.ReplyToken, item.Source.UserId);
                        break;

                    //メッセージイベント
                    case QsLineMessageApiEventTypeEnum.message:
                        //既にOpenIdが登録されている場合応答不可とする
                        if (!string.IsNullOrWhiteSpace(GetLineOpenId(linkageSystemNo, item.Source.UserId)))
                        {
                            ErrorEvent(linkageSet.LineBotChannelAccessToken,
                                item.Source.UserId, 
                                System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/Line/LineRegisteredMessage1.txt")));
                            break;
                        }

                        MessageEvent(linkageSet.LineBotChannelAccessToken, item.ReplyToken, item.Message.Text, item.Source.UserId);
                        break;

                    //ポストバックイベント
                    case QsLineMessageApiEventTypeEnum.postback:
                        //既にOpenIdが登録されている場合応答不可とする
                        if (!string.IsNullOrWhiteSpace(GetLineOpenId(linkageSystemNo, item.Source.UserId)))
                        {
                            ErrorEvent(linkageSet.LineBotChannelAccessToken,
                                item.Source.UserId,
                                System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Line/LineRegisteredMessage1.txt")));
                            break;
                        }

                        PostBackEvent(linkageSet.LineBotChannelAccessToken, item.ReplyToken, item.Source.UserId, linkageSystemNo, item.Postback, linkageSet.LineRichMenuId);
                        break;

                    //アンフォローイベント
                    case QsLineMessageApiEventTypeEnum.unfollow:

                        UnfollowEvent(item.Source.UserId, linkageSystemNo);
                        break;

                    //上記以外
                    default:
                        break;
                }

            }


            return result;
        }

        /// <summary>
        /// TemplatePushメッセージ送信処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoLineTemplatePushResults TemplatePushSend(QoLineTemplatePushArgs args)
        {
            var result = new QoLineTemplatePushResults() { };

            //引数チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNo");
                return result;
            }

            if (string.IsNullOrWhiteSpace(args.LinkageSystemId))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemId");
                return result;
            }

            if (string.IsNullOrWhiteSpace(args.Message))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Message");
                return result;
            }

            if (string.IsNullOrWhiteSpace(args.Label))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Label");
                return result;
            }

            if (string.IsNullOrWhiteSpace(args.Url))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Url");
                return result;
            }

            string lineUserId = GetLineUserId(linkageSystemNo, args.LinkageSystemId);
            if (string.IsNullOrWhiteSpace(lineUserId))
            {

                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LineUserId");
                return result;
            }

            //アクセストークンチェック
            QhLinkageSystemLinkageSetOfJson linkageSet = GetChannelAccessToken(linkageSystemNo);
            if (string.IsNullOrWhiteSpace(linkageSet.LineBotChannelAccessToken))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AccessToken");
                return result;
            }

            //Pushメッセージ送信
            TemplatePushSend(linkageSet.LineBotChannelAccessToken, lineUserId, args.Message, args.Label, args.Url);

            return result;
        }

        /// <summary>
        /// CustomPushメッセージ送信処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoLineCustomPushResults CustomPushSend(QoLineCustomPushArgs args)
        {
            var result = new QoLineCustomPushResults() { };

            //引数チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNo");
　                return result;
            }

            if (string.IsNullOrWhiteSpace(args.AltText))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AltText");
                return result;
            }

            QoAccessLog.WriteInfoLog(args.Contents);
            if (string.IsNullOrWhiteSpace(args.Contents))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "Contents");
                return result;
            }

            //アクセストークンチェック
            QhLinkageSystemLinkageSetOfJson linkageSet = GetChannelAccessToken(linkageSystemNo);
            if (string.IsNullOrWhiteSpace(linkageSet.LineBotChannelAccessToken))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AccessToken");
                return result;
            }

            var toList = new List<string>();

            //ID取得
            if (args.LinkageSystemId != null && args.LinkageSystemId.Count > 0)
            {
                foreach (var id in args.LinkageSystemId)
                {
                    string lineUserId = GetLineUserId(linkageSystemNo, id);
                    if (!string.IsNullOrWhiteSpace(lineUserId))
                    {
                        toList.Add(lineUserId);
                    }
                }
                //ID取得失敗
                if (toList.Count <= 0)
                {
                    result.IsSuccess = bool.FalseString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LineUserId");
                    return result;
                }
                
            }

            //Pushメッセージ送信
            CustomPushSend(linkageSet.LineBotChannelAccessToken, toList, args.AltText, args.Contents) ;

            return result;
        }

        /// <summary>
        /// メニュー更新処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoLineUpdateMenuIdResults UpdateMenuId(QoLineUpdateMenuIdArgs args)
        {
            var result = new QoLineUpdateMenuIdResults() { };

            //引数チェック
            if (!int.TryParse(args.LinkageSystemNo, out int linkageSystemNo))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LinkageSystemNo");
                return result;
            }

            //アクセストークンチェック
            QhLinkageSystemLinkageSetOfJson linkageSet = GetChannelAccessToken(linkageSystemNo);
            if (string.IsNullOrWhiteSpace(linkageSet.LineBotChannelAccessToken))
            {
                result.IsSuccess = bool.FalseString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "AccessToken");
                return result;
            }

            if (string.IsNullOrWhiteSpace(args.LinkageSystemId))
            {
                List<string> lineUserId = GetLineUserId(linkageSystemNo);
                if (lineUserId.Count <= 0)
                {
                    result.IsSuccess = bool.FalseString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LineUserId");
                    return result;
                }
                foreach (var userId in lineUserId)
                {
                    //Menu設定
                    RichMenuSet(linkageSet.LineBotChannelAccessToken, userId, linkageSet.LineRichMenuId);
                }
            }
            else
            {
                string lineUserId = GetLineUserId(linkageSystemNo, args.LinkageSystemId);
                if (string.IsNullOrWhiteSpace(lineUserId))
                {
                    result.IsSuccess = bool.FalseString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "LineUserId");
                    return result;
                }
                //Menu設定
                RichMenuSet(linkageSet.LineBotChannelAccessToken, lineUserId, linkageSet.LineRichMenuId);
            }

            return result;
        }

        #endregion
    }

}
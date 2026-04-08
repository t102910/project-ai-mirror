using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsNoticeApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView.Workers
{
    internal sealed class NoticeMailWorker
    {
        #region Constant

        private const string SETTING_NOTICE_SETTINGS_NAME = "SysNoticeSettingsName";

        private const string SETTING_NOTICE_SUBJECT = "SysNoticeSubject";

        private const string SETTING_NOTICE_TO = "SysNoticeTo";

        #endregion

        #region Constructor

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        private NoticeMailWorker()
        {
        }

        #endregion

        #region Private Method

        private static Dictionary<string, string> GetAppSettings()
        {
            var result = new Dictionary<string, string>();

            var keys = new[]
            {
            SETTING_NOTICE_SETTINGS_NAME,
            SETTING_NOTICE_SUBJECT,
            SETTING_NOTICE_TO
        };

            foreach (var key in keys)
            {
                var value = string.Empty;

                try
                {
                    value = System.Configuration.ConfigurationManager.AppSettings[key]?.Trim() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    result.Add(key, value);
                }
            }

            return result;
        }

        private static QnMailSendApiResults ExecuteQolmsNoticeApi(QnMailSendApiArgs args)
        {
            var result = new QnMailSendApiResults { IsSuccess = bool.FalseString };

            try
            {
                result = QsNoticeApiManager.ExecuteQolmsNoticeApi<QnMailSendApiResults>(args);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return result;
        }

        #endregion

        #region Public Method

        //[Obsolete("実装中")]
        public static bool Send(string body)
        {
            var result = false;

            if (!string.IsNullOrWhiteSpace(body))
            {
                var settings = GetAppSettings();

                if (settings.ContainsKey(SETTING_NOTICE_SETTINGS_NAME) &&
                    settings.ContainsKey(SETTING_NOTICE_SUBJECT) &&
                    settings.ContainsKey(SETTING_NOTICE_TO))
                {
                    var toN = settings[SETTING_NOTICE_TO]
                        .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .Select(i => i.Trim())
                        .ToList();

                    if (toN.Any())
                    {
                        var args = new QnMailSendApiArgs
                        {
                            ApiType = QnApiTypeEnum.MailSend.ToString(),
                            ExecuteSystemType = QsApiSystemTypeEnum.Qolms.ToString(),
                            Executor = Guid.Empty.ToApiGuidString(),
                            ExecutorName = "JOTOホームドクター",
                            ExecuteApplicationType = "None",
                            NoticeSettingsName = settings[SETTING_NOTICE_SETTINGS_NAME],
                            ToN = toN,
                            Subject = settings[SETTING_NOTICE_SUBJECT],
                            Body = body.Trim()
                        };

                        result = ExecuteQolmsNoticeApi(args).IsSuccess.TryToValueType(false);
                    }
                }
            }

            return result;
        }

        //[Obsolete("実装中")]
        public static async Task<bool> SendAsync(string body)
        {
            return await Task.Run(() => Send(body));
        }

        /// <summary>
        /// メールアドレスとタイトルと本文を指定して、メールを送信します。
        /// </summary>
        /// <param name="toMailAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        //[Obsolete("実装中")]
        public static bool SendUser(string toMailAddress, string subject, string body)
        {
            var result = false;

            if (!string.IsNullOrWhiteSpace(body))
            {
                var settings = GetAppSettings();

                if (settings.ContainsKey(SETTING_NOTICE_SETTINGS_NAME) &&
                    settings.ContainsKey(SETTING_NOTICE_SUBJECT) &&
                    settings.ContainsKey(SETTING_NOTICE_TO))
                {
                    var toN = settings[SETTING_NOTICE_TO]
                        .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .Select(i => i.Trim())
                        .ToList();

                    if (toN.Any())
                    {
                        var args = new QnMailSendApiArgs
                        {
                            ApiType = QnApiTypeEnum.MailSend.ToString(),
                            ExecuteSystemType = QsApiSystemTypeEnum.Qolms.ToString(),
                            Executor = Guid.Empty.ToApiGuidString(),
                            ExecutorName = "JOTOホームドクター",
                            ExecuteApplicationType = "None",
                            NoticeSettingsName = settings[SETTING_NOTICE_SETTINGS_NAME],
                            ToN = new List<string> { toMailAddress },
                            Subject = subject,
                            Body = body.Trim()
                        };

                        result = ExecuteQolmsNoticeApi(args).IsSuccess.TryToValueType(false);
                    }
                }
            }

            return result;
        }

        [Obsolete("実装中")]
        public static async Task<bool> SendUserAsync(string toMailAddress, string subject, string body)
        {
            return await Task.Run(() => SendUser(toMailAddress, subject, body));
        }

        #endregion
    }

}
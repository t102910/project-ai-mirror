using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// NoticeApi呼び出し引数ベースクラス
    /// </summary>
    internal abstract class QoNoticeClientArgsBase
    {
        /// <summary>
        /// 送信設定名を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SettingsName { get; set; } = string.Empty;

        /// <summary>
        /// 件名を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// 受信者のリストを取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<string> ToN { get; set; } = new List<string>();

        /// <summary>
        /// 本文を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Body { get; set; } = string.Empty;



        /// <summary>
        /// <see cref="QoNoticeClientArgsBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        protected QoNoticeClientArgsBase()
        {
            this.InitializeBase();
        }

        /// <summary>
        /// 受信者のリストを指定して、
        /// <see cref="QoNoticeClientArgsBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="toN">受信者のリスト。</param>
        /// <remarks></remarks>
        protected QoNoticeClientArgsBase(List<string> toN)
        {
            if (toN != null && toN.Any())
                this.ToN = toN.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToList();

            // 初期化
            this.InitializeBase();
        }

        /// <summary>
        /// 受信者のリスト、
        /// 本文テンプレート ファイルのパス、
        /// フッター テンプレート ファイルのパスを指定して、
        /// <see cref="QoNoticeClientArgsBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="toN">受信者のリスト。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected QoNoticeClientArgsBase(List<string> toN, string bodyTemplatePath, string footerTemplatePath)
        {
            if (toN != null && toN.Any())
                this.ToN = toN.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToList();

            // 初期化
            this.InitializeBase(bodyTemplatePath, footerTemplatePath);
        }
        /// <summary>
        /// NoticeApi用の設定名、
        /// 受信者のリスト、
        /// 本文テンプレート ファイルのパス、
        /// フッター テンプレート ファイルのパスを指定して、
        /// <see cref="QoNoticeClientArgsBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="toN">受信者のリスト。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected QoNoticeClientArgsBase(string settingsName, List<string> toN, string bodyTemplatePath, string footerTemplatePath)
        {
            if (toN != null && toN.Any())
                this.ToN = toN.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToList();

            // 初期化
            this.InitializeBase(settingsName, bodyTemplatePath, footerTemplatePath);
        }

        /// <summary>
        /// NoticeApi用の設定名、
        /// 件名の設定名、
        /// 受信者のリスト、
        /// 本文テンプレート ファイルのパス、
        /// フッター テンプレート ファイルのパスを指定して、
        /// <see cref="QoNoticeClientArgsBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="subject">件名の設定名。</param>
        /// <param name="toN">受信者のリスト。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected QoNoticeClientArgsBase(string settingsName, string subject, List<string> toN, string bodyTemplatePath, string footerTemplatePath)
        {
            if (toN != null && toN.Any())
                this.ToN = toN.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToList();

            // 初期化
            this.InitializeBase(settingsName, subject, bodyTemplatePath, footerTemplatePath);
        }

        /// <summary>
        /// NoticeApi用の設定名、
        /// 送信先メールアドレス（受信者）の設定名、
        /// 本文テンプレート ファイルのパス、
        /// フッター テンプレート ファイルのパスを指定して、
        /// <see cref="QoNoticeClientArgsBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="mailToSettingsName">送信先メールアドレス（受信者）の設定名。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected QoNoticeClientArgsBase(string settingsName, string mailToSettingsName, string bodyTemplatePath, string footerTemplatePath)
        {
            // 初期化
            this.InitializeBase(settingsName, mailToSettingsName, bodyTemplatePath, footerTemplatePath);
        }

        /// <summary>
        /// NoticeApi用の設定名、
        /// 送信先メールアドレス（受信者）の設定名、
        /// 本文テンプレート ファイルのパス、
        /// フッター テンプレート ファイルのパスを指定して、
        /// <see cref="QoNoticeClientArgsBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="subject">件名の設定名。</param>
        /// <param name="mailToSettingsName">送信先メールアドレス（受信者）の設定名。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected QoNoticeClientArgsBase(string settingsName, string subject, string mailToSettingsName, string bodyTemplatePath, string footerTemplatePath)
        {
            // 初期化
            this.InitializeBase(settingsName, subject, mailToSettingsName, bodyTemplatePath, footerTemplatePath);
        }

        /// <summary>
        /// アプリケーション構成ファイルから、
        /// アプリケーション設定を取得します。
        /// </summary>
        /// <param name="key">エントリのキー。</param>
        /// <returns>
        /// エントリの値。
        /// </returns>
        /// <remarks></remarks>
        protected string GetAppSettings(string key)
        {
            string result = string.Empty;

            try
            {
                result = ConfigurationManager.AppSettings[key].Trim();
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// アプリケーション構成ファイルから送信設定名を取得し、
        /// このインスタンスの <see cref="SettingsName" /> プロパティに設定します。
        /// </summary>
        /// <param name="key">エントリのキー。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        /// <remarks></remarks>
        protected bool SetSettingsNameToAppSettings(string key)
        {
            string toAddr = this.GetAppSettings(key);
            if (!string.IsNullOrWhiteSpace(toAddr))
                this.ToN = toAddr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return this.ToN != null && this.ToN.Any();
        }

        /// <summary>
        /// アプリケーション構成ファイルから送信設定名を取得し、
        /// このインスタンスの <see cref="SettingsName" /> プロパティに設定します。
        /// </summary>
        /// <param name="key">エントリのキー。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        /// <remarks></remarks>
        protected bool SetSettingsNameFromAppSettings(string key)
        {
            this.SettingsName = this.GetAppSettings(key);

            return !string.IsNullOrWhiteSpace(this.SettingsName);
        }

        /// <summary>
        /// アプリケーション構成ファイルから件名を取得し、
        /// このインスタンスの <see cref="Subject" /> プロパティに設定します。
        /// </summary>
        /// <param name="key">エントリのキー。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        /// <remarks></remarks>
        protected bool SetSubjectFromAppSettings(string key)
        {
            this.Subject = this.GetAppSettings(key);

            return !string.IsNullOrWhiteSpace(this.Subject);
        }

        /// <summary>
        /// テンプレート ファイルから本文を取得し、
        /// このインスタンスの <see cref="Body" /> プロパティに設定します。
        /// </summary>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス（オプショナル、デフォルト = ""）。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        /// <remarks></remarks>
        protected bool SetBodyFromTemplate(string bodyTemplatePath, string footerTemplatePath = "")
        {
            if (!string.IsNullOrWhiteSpace(bodyTemplatePath) && System.IO.File.Exists(bodyTemplatePath))
                this.Body = System.IO.File.ReadAllText(bodyTemplatePath);

            if (!string.IsNullOrWhiteSpace(footerTemplatePath) && System.IO.File.Exists(footerTemplatePath))
            {
                if (string.IsNullOrWhiteSpace(this.Body))
                    this.Body = System.IO.File.ReadAllText(footerTemplatePath);
                else
                    this.Body = this.Body + Environment.NewLine + System.IO.File.ReadAllText(footerTemplatePath);
            }

            return !string.IsNullOrWhiteSpace(this.Body);
        }

        /// <summary>
        /// <see cref="QoNoticeClientArgsBase" /> クラスのインスタンスを初期化します。
        /// このメソッドは引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected abstract void InitializeBase();

        /// <summary>
        /// <see cref="QoNoticeClientArgsBase" /> クラスのインスタンスを初期化します。
        /// このメソッドは引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected virtual void InitializeBase(string bodyTemplatePath, string footerTemplatePath)
        {
        }

        /// <summary>
        /// <see cref="QoNoticeClientArgsBase" /> クラスのインスタンスを初期化します。
        /// このメソッドは引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected virtual void InitializeBase(string settingsName, string bodyTemplatePath, string footerTemplatePath)
        {
        }

        /// <summary>
        /// <see cref="QoNoticeClientArgsBase" /> クラスのインスタンスを初期化します。
        /// このメソッドは引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="mailToSettingsName">送信先メールアドレス（受信者）の設定名。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected virtual void InitializeBase(string settingsName, string mailToSettingsName, string bodyTemplatePath, string footerTemplatePath)
        {
        }

        /// <summary>
        /// <see cref="QoNoticeClientArgsBase" /> クラスのインスタンスを初期化します。
        /// このメソッドは引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="subject">件名の設定名。</param>
        /// <param name="mailToSettingsName">送信先メールアドレス（受信者）の設定名。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <remarks></remarks>
        protected virtual void InitializeBase(string settingsName, string subject, string mailToSettingsName, string bodyTemplatePath, string footerTemplatePath)
        {
        }
    }
}
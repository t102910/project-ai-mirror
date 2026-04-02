using CefSharp;
using CefSharp.Internals;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace _TestJotoWebView
{
    public class CustomRequestHandler : IRequestHandler
    {
        private readonly string _authToken;

        public CustomRequestHandler(string authToken)
        {
            _authToken = authToken;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            // ナビゲーション前の処理が必要な場合はここに記述
            // 通常は false を返して処理を続行
            return false;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            // ドキュメントが利用可能になった時の処理
            // 必要に応じて実装
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            // リソースリクエストハンドラを返す
            // ここで Authorizationヘッダーを追加する処理を行う
            return new CustomResourceRequestHandler(_authToken);
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            // 認証情報が必要な場合は true を返してコールバックで設定
            // 通常は false でシステムのデフォルト認証ダイアログを使用
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            // SSL証明書エラーの処理
            // false を返すとエラーで停止、true を返すと続行
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            // クライアント証明書の選択処理
            // 通常は false で処理
            return false;
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            // レンダリングビューが準備完了した時の処理
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status, int errorCode, string errorMessage)
        {
            // レンダラープロセスが終了した時の処理
            // ログ出力やリソース解放など必要に応じて実装
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            // タブ
            return false;
        }
    }

    public class CustomResourceRequestHandler : IResourceRequestHandler
    {
        private readonly string _authToken;

        public CustomResourceRequestHandler(string authToken)
        {
            _authToken = authToken;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            try
            {
                // ✅ HttpRequestHeaders を使用してヘッダーを追加
                var headers = request.Headers;

                // 既存のヘッダーをコピー
                var newHeaders = new NameValueCollection(headers);

                // Authorizationヘッダーを追加
                newHeaders["Authorization"] = $"Bearer {_authToken}";

                // 新しいヘッダーを設定
                request.Headers = newHeaders;

                return CefReturnValue.Continue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnBeforeResourceLoad: {ex.Message}");
                return CefReturnValue.Continue;
            }
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            // リソース読み込み完了時の処理
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, string url)
        {
            return false;
        }

        public ICookieAccessFilter GetCookieAccessFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            return;
        }

    }

}

using CefSharp;
using CefSharp.WinForms;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace _TestJotoWebView
{
    public partial class Form1 : Form
    {
		public enum EnvironmentType
		{
			/// <summary>
			/// ローカル環境
			/// </summary>
			Local,

			/// <summary>
			/// Azure検証環境
			/// </summary>
			AzureDev,

			/// <summary>
			/// Azure本番環境
			/// </summary>
			AzurePro,

			/// <summary>
			/// Azure新井用検証環境
			/// </summary>
			AzureArai,

			/// <summary>
			/// Azure本番環境　スロット
			/// </summary>
			AzureProSlot,

		}
		private Dictionary<EnvironmentType, string> AuthHeaderKeys { get; set; } = new Dictionary<EnvironmentType, string>()
		{
			{
				EnvironmentType.Local,
				"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJwYXlsb2FkIjoie1wiYWNja2V5XCI6XCJQa2xBOFdtYlo2QmRNOE5Vcm1RaUdTbFxcL01HTmlEVk9Oc3VZN3lPc0cyS05xdjRRYk1yNE1MZkxTb1dDXFwvVk5KdGczVnVaRjZBbmdzUFV5RlBoalFaT1E9PVwiLFwicG5cIjpcIjFcIixcInBhcktleVwiOlwiXCIsXCJlY3RcIjpcInN5U3hkeHVDMzFKN1hma0I0cGhENmFlTFlVY0VDOUYrRTFLZlQ2M21ZNnFSRnJUdWJcXC9PZUVZSTNLY01kTmxkcFwifSIsIm5iZiI6MTY4OTMyMjQxNSwiZXhwIjozMzI0NjIzMTUxNSwiaWF0IjoxNjg5MzIyNzE1LCJpc3MiOiJRT0xNUyIsImF1ZCI6IlFvbG1zIn0.wRIXsyIxRxLwvjzJ8e-qzFiG1UYbjUKGsrzLkM2qK4Y\r\n" +
				"User-Agent: tisphrapp_ios/\r\n"
			},
			{
				EnvironmentType.AzureDev,
				"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJwYXlsb2FkIjoie1wiYWNja2V5XCI6XCJQa2xBOFdtYlo2QmRNOE5Vcm1RaUdTbFxcL01HTmlEVk9Oc3VZN3lPc0cyS05xdjRRYk1yNE1MZkxTb1dDXFwvVk5KdGczVnVaRjZBbmdzUFV5RlBoalFaT1E9PVwiLFwicG5cIjpcIjFcIixcInBhcktleVwiOlwiXCIsXCJlY3RcIjpcInN5U3hkeHVDMzFKN1hma0I0cGhENmFlTFlVY0VDOUYrRTFLZlQ2M21ZNnFSRnJUdWJcXC9PZUVZSTNLY01kTmxkcFwifSIsIm5iZiI6MTY4OTMyMjQxNSwiZXhwIjozMzI0NjIzMTUxNSwiaWF0IjoxNjg5MzIyNzE1LCJpc3MiOiJRT0xNUyIsImF1ZCI6IlFvbG1zIn0.wRIXsyIxRxLwvjzJ8e-qzFiG1UYbjUKGsrzLkM2qK4Y\r\n" +
				"User-Agent: tisphrapp_ios/\r\n"
			},
			//{
			//	EnvironmentType.AzurePro,
			//	"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJwYXlsb2FkIjoie1wiYWNja2V5XCI6XCJLcHhSYkp0WmZ4TW56TTBYclVHb0Y4Vlh6ODZ0QkVYOTR2dFQ4c0doSTNPT2xCdit0ZXd1dHB3bXYrUWtrYzZxK0k5bnJKaUw5RnpueFArU2RTdzlUUT09XCIsXCJwblwiOlwiMVwiLFwicGFyS2V5XCI6XCJcIixcImVjdFwiOlwieXJSNWNhbmk2Q2VGeURQTnBSOWxHVGFzY2JEdlZYNUFJRDlvNTRaaWo5QXJUVFk3SEZCWmdEQnErXFwvVVhCdWJ6XCJ9IiwibmJmIjoxNjg3MjUyMzIxLCJleHAiOjMzMjQ0MTYxNDIxLCJpYXQiOjE2ODcyNTI2MjEsImlzcyI6IlFPTE1TIiwiYXVkIjoiUW9sbXMifQ.-Qy3N_SWVFvP1CsFfkfWrBVhHcLHVYO_SftsyvOCqAw\r\n" +
			//	"User-Agent: tisphrapp_ios/\r\n"
			//},
			//{
			//	EnvironmentType.AzureArai,
			//	"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJwYXlsb2FkIjoie1wiYWNja2V5XCI6XCJrMDBsSFRmNGZacklBcUtpQlxcL1RKSXBFV2t3MjRHRXJoZXFWZjY4MGd6UDJcXC85alNxY29FTmM5ZGxCbGpBbFMyXFwvMWUrNW5JcnlMTWNNK20zdTZGejRPQT09XCIsXCJwblwiOlwiMVwiLFwicGFyS2V5XCI6XCJcIixcImVjdFwiOlwibkpZWTlSajZMdXRVNDlkTjl6VXpBc2RLZHU3QXFRZTNJUXJUb3pTSEJWMFVrcVlSODYzNGhYSkNQYWg3MnhKTlwifSIsIm5iZiI6MTY4NzE0NDQ1MiwiZXhwIjozMzI0NDA1MzU1MiwiaWF0IjoxNjg3MTQ0NzUyLCJpc3MiOiJRT0xNUyIsImF1ZCI6IlFvbG1zIn0.h2UmQvikU4ecvCi3D8Ib1b-VZHFTQ_iyxJx9gExSTUU\r\n" +
			//	"User-Agent: tisphrapp_ios/\r\n"
			//},
			//{
			//	EnvironmentType.AzureProSlot,
			//	"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJwYXlsb2FkIjoie1wiYWNja2V5XCI6XCJLcHhSYkp0WmZ4TW56TTBYclVHb0Y4Vlh6ODZ0QkVYOTR2dFQ4c0doSTNPT2xCdit0ZXd1dHB3bXYrUWtrYzZxK0k5bnJKaUw5RnpueFArU2RTdzlUUT09XCIsXCJwblwiOlwiMVwiLFwicGFyS2V5XCI6XCJcIixcImVjdFwiOlwieXJSNWNhbmk2Q2VGeURQTnBSOWxHVGFzY2JEdlZYNUFJRDlvNTRaaWo5QXJUVFk3SEZCWmdEQnErXFwvVVhCdWJ6XCJ9IiwibmJmIjoxNjg3MjUyMzIxLCJleHAiOjMzMjQ0MTYxNDIxLCJpYXQiOjE2ODcyNTI2MjEsImlzcyI6IlFPTE1TIiwiYXVkIjoiUW9sbXMifQ.-Qy3N_SWVFvP1CsFfkfWrBVhHcLHVYO_SftsyvOCqAw\r\n" +
			//	"User-Agent: tisphrapp_ios/\r\n"
			//},
		};

		private Dictionary<EnvironmentType, string> ExecutorKeys { get; set; } = new Dictionary<EnvironmentType, string>()
		{
			{
				EnvironmentType.Local, "FF62E7EA-0035-2901-0000-000000047003"
			},
			{
				EnvironmentType.AzureDev, "FF62E7EA-0035-2901-0000-000000047003"
			},
			//{
			//	EnvironmentType.AzurePro, "FFB5190F-0038-4101-0000-000000000001"
			//},
			//{
			//	EnvironmentType.AzureArai, "30E53385-5194-4A2B-ADBE-E68573BDEFEC"
			//},
			//{
			//	EnvironmentType.AzureProSlot, "FFB5190F-0038-4101-0000-000000000001"
			//},

		};
		private Dictionary<EnvironmentType, string> URLs { get; set; } = new Dictionary<EnvironmentType, string>()
		{
			{
				EnvironmentType.Local,
				"https://localhost:44384/start/sso"
			},
			{
				EnvironmentType.AzureDev,
				"https://devjotov2.qolms.com/start/sso"
			},
			//{
			//	EnvironmentType.AzurePro,
			//	"https://qolms-prod-core-east-app05.azurewebsites.net/start/sso"
			//},
			//{
			//	EnvironmentType.AzureArai,
			//	"https://app-healthcheckup.azurewebsites.net/start/sso"
			//},
			//{
			//	EnvironmentType.AzureProSlot,
			//	"https://qolms-prod-core-east-app05-slot01.azurewebsites.net/start/sso"
			//},
		};

		public Form1()
        {
            InitializeComponent();

			cboPath.SelectedIndex = 0;
			cboEv.SelectedIndex = 0;
			cboPage.SelectedIndex = 0;

			txtAccount.Text = "81417A6F-1BAC-48A5-B691-750B003529A8";
		}

		private void ClearCache()
		{
			if (web.Document != null)
			{
				web.Document.ExecCommand("ClearAuthenticationCache", false, null);
				web.Document.ExecCommand("ClearCache", false, null);
			}
		}

		private async void Show()
		{
			ClearCache();
			var myUrl = cboPath.Text;
			System.Uri uri = new Uri(myUrl);

			//Azure検証環境
			string authHeader =
				"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJwYXlsb2FkIjoie1wiYWNja2V5XCI6XCJOaHFWT2IxXFwvSDExQ09UUlFKdFd0bGRBWTMwMWtCZDJQYXU0c25QVjVwWk5PV0JTV0ZscldBVVkweDRcXC9Jc0Q3WVdMTjhEdHF1NVlrM1l4RkZvRGhxYlE9PVwiLFwicG5cIjpcIjFcIixcInBhcktleVwiOlwiXCIsXCJlY3RcIjpcInN5U3hkeHVDMzFKN1hma0I0cGhENmFlTFlVY0VDOUYrRTFLZlQ2M21ZNnFSRnJUdWJcXC9PZUVZSTNLY01kTmxkcFwifSIsIm5iZiI6MTY4NzE3NjAzNCwiZXhwIjozMzI0NDA4NTEzNCwiaWF0IjoxNjg3MTc2MzM0LCJpc3MiOiJRT0xNUyIsImF1ZCI6IlFvbG1zIn0.iSIE6Ciyg3uL1q7BYfK9A5G7JyZ9UkE7tKXOa27yP5g\r\n" +
				"User-Agent: tisphrapp_ios/\r\n";

			web.Navigate(uri, "", null, authHeader);

		}


		private void ShowEdge()
		{

			// Edgeを起動するためのプロセスを作成
			ProcessStartInfo startInfo = new ProcessStartInfo();

			// Edgeの実行ファイルのパス
			string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
			string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";


			//startInfo.FileName = edgePath;
			startInfo.FileName = chromePath;

			// HTTPヘッダーに値を設定
			startInfo.Arguments = "http://localhost:16910/start/sso?pageno=2 \r\n --extra-http-headers \"" +
				"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJwYXlsb2FkIjoie1wiYWNja2V5XCI6XCJDMkdsZlBzNWxHVHg3QTFcXC9TK2MwekFvMnpNNE10XFwvQU9DTWtteVdMRHhGZHAwYmRIcjlzTStTSHNqbzJcXC85UnZhbjhEaDFFMDV6MFM4TzdMREtLRHpuZz09XCIsXCJwblwiOlwiMVwiLFwicGFyS2V5XCI6XCJcIixcImVjdFwiOlwibkpZWTlSajZMdXRVNDlkTjl6VXpBc2RLZHU3QXFRZTNJUXJUb3pTSEJWMFVrcVlSODYzNGhYSkNQYWg3MnhKTlwifSIsIm5iZiI6MTY4NDQ2MTU4MCwiZXhwIjozMzI0MTM3MDY4MCwiaWF0IjoxNjg0NDYxODgwLCJpc3MiOiJRT0xNUyIsImF1ZCI6IlFvbG1zIn0.xvJPYz3s8NHOOTXzfMJY0tKPUdVbMNWLv4usi4lZAjQ\r\n" +
				"User-Agent: tisphrapp_ios/\r\n";



			// Edgeプロセスを開始
			Process.Start(startInfo);

		}

		private string ToJWT(EnvironmentType key)
		{
			if (ExecutorKeys.ContainsKey(key))
			{
				var executor = ExecutorKeys[key];


				using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
				{
					var encryptExecutor = crypt.EncryptString(executor);
					var accountKey = Guid.Parse(txtAccount.Text);

					pageno pageno = (pageno)Enum.Parse(typeof(pageno), cboPage.Text);

					return new QsJwtTokenProvider().CreateQolmsJwtSsoKey(encryptExecutor, accountKey, Guid.Empty, (int)pageno, DateTime.Now.AddYears(1000));

				}
			}
			return string.Empty;
		}

        private void btnshow_Click(object sender, EventArgs e)
        {
			var key = (EnvironmentType)cboEv.SelectedIndex;

			if (this.AuthHeaderKeys.ContainsKey(key))
			{
				var myUrl = this.URLs[key];
				var pageNo = (pageno)Enum.Parse(typeof(pageno), cboPage.Text);

				if (pageNo == pageno.LocalHistory)
				{
					// DEBUG SODA
					int? year = 2026;
					int? month = 1;

					// string linkageSystemId = "fTLk7ivH";
					// string linkageSystemId = "M28eRwhL";
					
					var uriBuilder = new UriBuilder(myUrl);
					var query = HttpUtility.ParseQueryString(uriBuilder.Query);

					if (year.HasValue)
					{
						query["year"] = year.Value.ToString();
					}

					if (month.HasValue)
					{
						query["month"] = month.Value.ToString();
					}

					uriBuilder.Query = query.ToString();
					myUrl = uriBuilder.ToString();
				}

				if (pageNo == pageno.Calomeal)
				{
					int? selectdate = 20260301;
					int? meal = 1;

					// string linkageSystemId = "fTLk7ivH";
					// string linkageSystemId = "M28eRwhL";

					var uriBuilder = new UriBuilder(myUrl);
					var query = HttpUtility.ParseQueryString(uriBuilder.Query);


					if (meal.HasValue)
					{
						query["meal"] = meal.ToString();
					}

					if (selectdate.HasValue)
					{
						query["selectdate"] = selectdate.Value.ToString();
					}

					uriBuilder.Query = query.ToString();
					myUrl = uriBuilder.ToString();
				}

				//myUrl += "?pageno=" + (cboPage.SelectedIndex + 1).ToString();
				//myUrl += "&redirecturl=www.google.co.jp%2F";
				//myUrl += "&redirecturlname=LINE";

				System.Uri uri = new Uri(myUrl);
				var authHeader = string.Empty;
				var jwt = string.Empty;
				if (string.IsNullOrEmpty(txtAccount.Text))
				{
					authHeader = this.AuthHeaderKeys[key];
				}
				else
				{
					jwt = ToJWT(key);
					string a = $"{myUrl}&jwt={jwt}";
					authHeader = $"Authorization: Bearer {jwt}\r\n" + "User-Agent: tisphrapp_ios/\r\n";
				}

				txtURL.Text = myUrl + " " + authHeader;

				web.Navigate(uri, "", null, authHeader);

				Form form = new Form();

				ChromiumWebBrowser cefBrowser;
				
                if (Cef.IsInitialized == false)
                {
					CefSettings settings = new CefSettings();
					Cef.Initialize(settings);
				}
				// フォーム初期化時など
				cefBrowser = new ChromiumWebBrowser();
				cefBrowser.RequestHandler = new CustomRequestHandler(jwt);
				// URLをロード
				cefBrowser.Load(uri.AbsoluteUri);

				form.Controls.Add(cefBrowser);
				cefBrowser.Dock = DockStyle.Fill;
				//cefBrowser.ShowDevTools();

				cefBrowser.KeyDown += new KeyEventHandler(doKeyDownEvent);
				form.KeyDown += new KeyEventHandler(doKeyDownEvent2);

				form.Show();

			}
		}

        private void btnback_Click(object sender, EventArgs e)
        {
			if (web.CanGoBack)
			{
				web.GoBack();
			}
		}

		enum pageno : int
		{
			PortalLocalIdVerification = 5,
			HealthAge = 6,
			NoteExamination =2,
			NoteMonshin = 11,
			PointHistory = 3,
			LocalHistory = 4,
			Calomeal = 10,

		}
		public void doKeyDownEvent(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F12)
			{

				((ChromiumWebBrowser)sender).ShowDevTools();
				// イベントを処理済みとしてマーク（他の処理を防ぐ）
				e.Handled = true;
			}
		}

		public void doKeyDownEvent2(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F12)
			{
				var browser = FindChromiumWebBrowser();

				if (browser != null && browser.IsBrowserInitialized)
				{
					browser.ShowDevTools();
				}
				e.Handled = true;
			}
		}

		public ChromiumWebBrowser FindChromiumWebBrowser(Control parent = null)
		{
			// parentが指定されていない場合はフォーム自身を対象
			if (parent == null)
			{
				parent = this;
			}

			// 指定されたコントロールとその子要素を再帰的に探索
			foreach (Control control in parent.Controls)
			{
				// ChromiumWebBrowserのインスタンスか確認
				if (control is ChromiumWebBrowser browser)
				{
					return browser;
				}

				// 子要素がある場合は再帰的に探索
				ChromiumWebBrowser foundBrowser = FindChromiumWebBrowser(control);
				if (foundBrowser != null)
				{
					return foundBrowser;
				}
			}

			return null;
		}
	}
}

Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1

Friend NotInheritable Class PortalMonshinWorker

#Region "Constant"

    '''' <summary>
    '''' <see cref="HttpClient" /> のインスタンスを保持します。
    '''' </summary>
    'Private Shared httpClientInstance As HttpClient = PortalMonshinWorker.CreateHttpClientInstance()

    Private Shared BASIC_USERID As New Lazy(Of String)(Function() PortalMonshinWorker.GetCryptSetting("SunagawaBasicUserId"))

    Private Shared BASIC_PASSWORD As New Lazy(Of String)(Function() PortalMonshinWorker.GetCryptSetting("SunagawaBasicPass"))

    Private Shared AES_KEY As New Lazy(Of String)(Function() PortalMonshinWorker.GetCryptSetting("SunagawaAesKey"))
    Private Shared AES_IV As New Lazy(Of String)(Function() PortalMonshinWorker.GetCryptSetting("SunagawaAesIv"))

    Private Shared SETTING_URL As New Lazy(Of String)(Function() PortalMonshinWorker.GetSetting("SunagawaMonshinUrl"))

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 設定を取得します。
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetCryptSetting(settingName As String) As String

        Dim result As String = String.Empty
        Dim value As String = ConfigurationManager.AppSettings(settingName)

        If value IsNot Nothing And Not String.IsNullOrWhiteSpace(value) Then

            Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
                result = crypt.DecryptString(value)

            End Using

        End If

        Return result

    End Function
    ''' <summary>
    ''' 設定を取得します。
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetSetting(settingName As String) As String

        Dim result As String = String.Empty
        Dim value As String = ConfigurationManager.AppSettings(settingName)

        If value IsNot Nothing And Not String.IsNullOrWhiteSpace(value) Then

            result = value
        End If

        Return result

    End Function

    '''' <summary>
    '''' <see cref="HttpClient" /> の新しいインスタンスを作成します。
    '''' </summary>
    '''' <returns><see cref="HttpClient" /> の新しいインスタンス。</returns>
    'Private Shared Function CreateHttpClientInstance() As HttpClient

    '    Dim url As String = "https://iakum.com/"

    '    Dim ifUri As New Uri(url)
    '    Dim sp As ServicePoint = ServicePointManager.FindServicePoint(New Uri(ifUri.Scheme + Uri.SchemeDelimiter + ifUri.Host))
    '    sp.ConnectionLeaseTimeout = 60 * 1000

    '    Return New HttpClient(New HttpClientHandler() With {.UseCookies = False})

    'End Function



    'Private Shared Function GetContent(url As String) As String

    '    ' 認証情報 (ユーザー名とパスワード)
    '    Dim username As String = "Sunagawamc2525"
    '    Dim password As String = "T1#V9pU&k3@Xl7FqZ"

    '    ' 認証情報をBase64形式にエンコード
    '    ' Dim credentials As String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"))

    '    ' AuthorizationヘッダーにBasic認証情報を追加
    '    httpClientInstance.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", "U3VuYWdhd2FtYzI1MjU6VDEjVjlwVSZrM0BYbDdGcVo=")

    '    ' リクエスト先のURL
    '    'Dim url As String = "https://example.com/api/resource"

    '    ' 非同期でリクエストを送信し、レスポンスを取得
    '    Dim response As HttpResponseMessage = httpClientInstance.GetAsync(url).Result




    '    ' レスポンスのステータスコードを確認
    '    Console.WriteLine($"Response Status Code: {response.StatusCode}")

    '    ' レスポンスのコンテンツを表示
    '    Dim content As String = response.Content.ReadAsStringAsync().Result
    '    Console.WriteLine($"Response Content: {content}")

    '    ' HttpClientを破棄

    '    Return content

    'End Function

#End Region

#Region "Public Method"

    Public Shared Function CreateRedirectUrl(mainModel As QolmsYappliModel) As String

        '連携がある状態でしか呼び出されない
        'ボタンの表示制御　連携がある場合のみ表示される
        Dim linkageSystemNo As Integer = 47000020 'すながわ内科
        Dim linkagelist As LinkageItem = LinkageWorker.GetLinkageItem(mainModel, linkageSystemNo)

        '・診察券番号
        '・氏名（JOTO登録情報）※全角
        '・カナ姓名（JOTO登録情報）※全角
        '・性別（JOTO登録情報）※半角数値（1=男、2=女、3=不明）
        '・生年月日（JOTO登録情報）※半角（YYYY-MM-DD）
        '・呼び出し日時※半角（yyyyMMddHHmmss）
        'JSON形式の文字列をACS暗号化してGETパラメータとして受け渡してください。
        '※プロパティ名と文字列は、ダブルクオーテーションで囲む

        Dim json As New MonshinArgsJsonParamater() With {
            .patientID = linkagelist.LinkageSystemId,
            .patientName = mainModel.AuthorAccount.Name,
            .patientNameKana = mainModel.AuthorAccount.KanaName,
            .gendar = CByte(mainModel.AuthorAccount.SexType),
            .birthday = mainModel.AuthorAccount.Birthday.ToString("yyyy-MM-dd"),
            .timestamp = Date.Now.ToString("yyyyMMddHHmmss")
        }
        '　パラメータ例）{"patientID": "0000001", "patientName": "砂川　太郎", "patientNameKana": "スナガワ　タロウ", "gendar": 1, "birthday": "2000-01-01", "timestamp": "20240913123456" }
        Dim prm As String = json.ToJsonString()
        Dim key As String = AES_KEY.Value '設定
        Dim iv As String = AES_IV.Value '設定

        Dim basicPass As String = HttpUtility.UrlEncode(BASIC_PASSWORD.Value)

        '※パラメータは ACS 暗号化して受け渡しを行う
        Dim aes As New AESCrypt(key, iv)
        Dim crptPrm As String = aes.Encrypt(prm)
        crptPrm = HttpUtility.UrlEncode(crptPrm)

        Dim u As New Uri(SETTING_URL.Value)

        Dim url As String = $"{u.Scheme}://{BASIC_USERID.Value}:{basicPass}@{u.Host}{u.PathAndQuery}{crptPrm}" '設定

        Return url

    End Function
#End Region

End Class

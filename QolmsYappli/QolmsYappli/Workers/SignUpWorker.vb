Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports System.Net
Imports System.Net.Mail
Imports System.Text
Imports System.Web.Configuration
Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 「アカウント仮登録」「本登録」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class SignUpWorker

#Region "Constant"

    ''' <summary>
    ''' ダミーのセッションIDを現します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

    ''' <summary>
    ''' ダミーのAPI認証キーを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    ''' <summary>
    ''' 連携システムNOを決め打ち。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const OKINAWAAULINKAGESYSTEMNO As Integer = 47003
#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    'Private Shared Function ExecuteLinkageWriteApi(mainModel As QolmsYappliModel, Optional ByVal linkageSystemNo As Integer = OKINAWAAULINKAGESYSTEMNO) As QhYappliLinkagePatientCardWriteApiResults


    '    'API実行
    '    Dim apiArgs As New QhYappliLinkagePatientCardWriteApiArgs(
    '        QhApiTypeEnum.LinkagePatientCardEditWrite,
    '        QsApiSystemTypeEnum.Qolms,
    '        mainModel.ApiExecutor,
    '        mainModel.ApiExecutorName
    '    ) With {
    '        .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
    '        .LinkageSystemNo = linkageSystemNo.ToString()
    '    }

    '    Dim apiResults As QhYappliLinkagePatientCardWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliLinkagePatientCardWriteApiResults)(
    '        apiArgs,
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) Then

    '            Return apiResults
    '        Else
    '            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '        End If
    '    End With

    'End Function

    Private Shared Function ExecuteRegisterReadApi(accountkey As String) As QiQolmsAccountRegisterReadApiResults

        Dim apiArgs As New QiQolmsAccountRegisterReadApiArgs(accountkey, Guid.Empty, String.Empty)
        Dim apiResults As QiQolmsAccountRegisterReadApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsAccountRegisterReadApiResults)(
            apiArgs,
            SignUpWorker.DUMMY_SESSION_ID,
            SignUpWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecuteRegisterWriteApi(model As StartRegisterInputModel) As QiQolmsAccountRegisterWriteApiResults

        Dim apiArgs As New QiQolmsAccountRegisterWriteApiArgs(Guid.Empty, String.Empty) With {
            .Accountkey = model.Accountkey.ToString,
            .FamilyName = model.FamilyName,
            .GivenName = model.GivenName,
            .FamilyKanaName = model.FamilyKanaName,
            .GivenKanaName = model.GivenKanaName,
            .Sex = model.Sex.ToString,
            .BirthYear = model.BirthYear,
            .BirthMonth = model.BirthMonth,
            .BirthDay = model.BirthDay,
            .MailAddress = model.MailAddress,
            .OpenId = model.OpenId,
            .OpenIdType = model.OpenIdType.ToString(),
            .AccountType = "1",
            .PrivateAccountFlag = Boolean.FalseString,
            .LinkageID = "",
            .LinkageSystemNo = SignUpWorker.OKINAWAAULINKAGESYSTEMNO.ToString()
        }
        Dim apiResults As QiQolmsAccountRegisterWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsAccountRegisterWriteApiResults)(
            apiArgs,
            SignUpWorker.DUMMY_SESSION_ID,
            SignUpWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function


    Private Shared Function ExecuteRegisterUserIdWriteApi(model As StartRegisterUserIdInputModel) As QiQolmsYappliAccountRegisterWriteApiResults

        Dim apiArgs As New QiQolmsYappliAccountRegisterWriteApiArgs(Guid.Empty, String.Empty) With {
            .Accountkey = model.Accountkey.ToString,
            .FamilyName = model.FamilyName,
            .GivenName = model.GivenName,
            .FamilyKanaName = model.FamilyKanaName,
            .GivenKanaName = model.GivenKanaName,
            .Sex = model.Sex.ToString,
            .BirthYear = model.BirthYear,
            .BirthMonth = model.BirthMonth,
            .BirthDay = model.BirthDay,
            .MailAddress = model.MailAddress,
            .UserId = String.Format("JOTO{0}", model.UserId),
            .Password = model.Password,
            .AccountType = "1",
            .PrivateAccountFlag = Boolean.FalseString,
            .LinkageID = "",
            .LinkageSystemNo = SignUpWorker.OKINAWAAULINKAGESYSTEMNO.ToString()
        }
        Dim apiResults As QiQolmsYappliAccountRegisterWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliAccountRegisterWriteApiResults)(
            apiArgs,
            SignUpWorker.DUMMY_SESSION_ID,
            SignUpWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function

    ' ''' <summary>
    ' ''' 文字列のエンコードを変換する
    ' ''' キャリアメール受信時に、件名が文字化けする対応
    ' ''' </summary>
    ' ''' <param name="title"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Private Shared Function EncodeMailHeader(title As String) As String

    '    '「=?iso-2022-jp?B?<エンコード文字列>?=」形式に変換
    '    Dim enc As Encoding = Encoding.GetEncoding("iso-2022-jp")
    '    Dim str As String = String.Empty
    '    str = Convert.ToBase64String(enc.GetBytes(title))

    '    Return String.Format("=?{0}?B?{1}?=", "iso-2022-jp", str)
    '    '参考ＵＲＬ
    '    'http://blogs.technet.com/b/exchangeteamjp/archive/2012/10/05/3524293.aspx

    'End Function

#End Region

#Region "Public Method"

    ' ''' <summary>
    ' ''' アカウント仮登録前の表示機能を提供します。
    ' ''' </summary>
    ' ''' <param name="model"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared Function SignUpRead(model As StartSignUpInputModel) As Integer

    '    If model IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(model.Mail1) Then

    '        'API呼び出し テーブルセレクト   
    '        With SignUpWorker.ExecuteSignUpReadApi(model)
    '            Return .MailAddressCount
    '        End With
    '    Else
    '        Return Integer.MinValue
    '    End If

    'End Function

    ' ''' <summary>
    ' ''' アカウント仮登録の機能を提供します。
    ' ''' </summary>
    ' ''' <param name="model"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared Function SignUpWrite(model As StartSignUpInputModel) As Guid

    '    Dim key As Guid = Guid.Empty

    '    If model IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(model.Mail1) Then

    '        'API呼び出し API側で、DBテーブル登録　登録が成功すれば、仮アカウントキーを返却    
    '        With SignUpWorker.ExecuteSignUpWriteApi(model)
    '            key = .Accountkey
    '        End With

    '    End If

    '    Return key

    'End Function

    ''' <summary>
    ''' アカウント本登録の機能を提供します。
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RegisterWrite(model As StartRegisterInputModel, ByRef errorList As List(Of String)) As Boolean

        If model IsNot Nothing Then
            Dim result As QiQolmsAccountRegisterWriteApiResults = SignUpWorker.ExecuteRegisterWriteApi(model)

            'API呼び出し API側で、DBテーブル登録 
            errorList = result.ErrorList
            Return Boolean.Parse(result.IsSuccess) 'AndAlso SignUpWorker.ExecuteLinkageWriteApi(mainmodel).IsSuccess.TryToValueType(False)

        Else
            Return False
        End If

    End Function


    ''' <summary>
    ''' アカウント本登録の機能を提供します。
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RegisterUserIdWrite(model As StartRegisterUserIdInputModel, ByRef errorList As List(Of String)) As Boolean

        If model IsNot Nothing Then
            Dim result As QiQolmsYappliAccountRegisterWriteApiResults = SignUpWorker.ExecuteRegisterUserIdWriteApi(model)

            'API呼び出し API側で、DBテーブル登録 
            errorList = result.ErrorList
            Return Boolean.Parse(result.IsSuccess) 'AndAlso SignUpWorker.ExecuteLinkageWriteApi(mainmodel).IsSuccess.TryToValueType(False)

        Else
            Return False
        End If

    End Function
    ' ''' <summary>
    ' ''' URLに含まれたキーの値をチェックします。
    ' ''' </summary>
    ' ''' <param name="accountkey"></param>
    ' ''' <param name="mailaddress"></param>
    ' ''' <param name="expiresOk"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared Function CheckTheKey(accountkey As String, ByRef mailaddress As String, ByRef expiresOk As Boolean) As Boolean

    '    'URLから受け取ったkeyの値を復号化
    '    'API呼び出し API側で、DBテーブル検索　検索が成功すれば、メールアドレスを返却  
    '    Dim result As QiQolmsAccountRegisterReadApiResults = SignUpWorker.ExecuteRegisterReadApi(accountkey)

    '    If Not String.IsNullOrWhiteSpace(result.MailAddress) AndAlso Not String.IsNullOrWhiteSpace(result.Expires) Then
    '        mailaddress = result.MailAddress
    '        '有効期限が切れていないか
    '        If Date.Parse(result.Expires) >= Date.Now Then
    '            expiresOk = True
    '        Else
    '            expiresOk = False
    '        End If
    '        Return True
    '    Else
    '        Return False
    '    End If

    '    Return True

    'End Function

#End Region

End Class
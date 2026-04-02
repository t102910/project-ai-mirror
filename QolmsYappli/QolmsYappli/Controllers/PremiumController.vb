Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1

Public NotInheritable Class PremiumController
    Inherits QyMvcControllerBase
#Region "Constant"

    Private Const AU_ERROR_PAYCERTFORCONTBILL As String = "決済認可要求時のエラーです。"

    Private Const TIMEOUT_ERROR_PAYCERTFORCONTBILL As String = "タイムアウトしました。"

    Private Const AU_ERROR_CONTBILLREQUEST As String = "継続課金情報登録要求時のエラーです。"

    Private Const AU_ERROR_CONTBILLCANCELREQUEST As String = "継続課金情報解除登録要求時のエラーです。"

    ''' <summary>
    ''' 予期せぬエラーが発生したことを表すエラーメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const UNEXPECTED_ERROR_MESSAGE As String = "予期せぬエラーが発生しました。"

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PremiumController" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "index"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function Index() As ActionResult
        Dim auPaymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
        'セッションに保存
        QySessionHelper.SetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel
        mainModel.AuthorAccount.MembershipType = CType(auPaymentModel.MemberShipType, QyMemberShipTypeEnum)

        Select Case mainModel.AuthorAccount.MembershipType
            Case QyMemberShipTypeEnum.Business, QyMemberShipTypeEnum.BusinessFree
                Return RedirectToAction("CompanyConnectionHome", "Portal")

            Case Else
                Return View("Index", auPaymentModel)

        End Select

    End Function

#End Region

#Region "決済方法の選択 画面"

    ''' <summary>
    ''' 決済方法の選択画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function Regist() As ActionResult
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        If auPaymentModel Is Nothing Then Return RedirectToAction("index")
        Dim viewModel As New PremiumRegistViewModel(Me.GetQolmsYappliModel())
        viewModel.PaymentStartDate = auPaymentModel.ContinueAccountStartDate
        Return View(viewModel)

    End Function

    ''' <summary>
    ''' プレミアム会員登録開始。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>決済認可要求をだして成功したらユーザ認証へリダイレクト</remarks>
    <HttpPost()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function RegistResult(PaymentType As Byte) As ActionResult
        If PaymentType = 2 Then
            Dim page As Byte = QyPageNoTypeEnum.PremiumPayJpRegist
            Return RedirectToAction("PayJpAgree", New With {.fromPageNo = page})
        End If

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        If auPaymentModel Is Nothing Then Return RedirectToAction("index")

        If PremiumWorker.InsertPremiumRecord(mainModel, auPaymentModel) = True AndAlso auPaymentModel.MemberManageNo > 0 Then

            Dim redirectUri As String = AuPaymentAccessWorker.RequestPayCertAndGetRedirectUrl(auPaymentModel)
            'DBに保存
            PremiumWorker.UpdatePremiumRecordToPayCertRequested(mainModel, auPaymentModel)
            If String.IsNullOrWhiteSpace(redirectUri) = False Then
                'セッションに保存
                QySessionHelper.SetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
                '成功
                Return Redirect(redirectUri)
            Else
                '失敗
                '既に課金中の場合は状態チェックして一致してたらエラーを返す必要がない。
                If auPaymentModel.AuResultCode = AuPaymentAccessWorker.AUREGISTEREDERRORCODE Then
                    'DBと状態が一致しているかチェック
                    Dim dbMemberShipType As Byte = PremiumWorker.GetMemberShipType(Me.GetQolmsYappliModel)
                    If dbMemberShipType = QyMemberShipTypeEnum.LimitedTime OrElse dbMemberShipType = QyMemberShipTypeEnum.Premium _
                        OrElse dbMemberShipType = QyMemberShipTypeEnum.Business OrElse dbMemberShipType = QyMemberShipTypeEnum.BusinessFree Then 'ビジネスプレミアムの場合のプレミアム画面の表示？

                        Return RedirectToAction("index")
                    End If
                End If
                'セッション削除
                QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                Me.SetErrorMessage("決済認可要求に失敗しました。") ' エラー画面に表示するメッセージ
                Throw New InvalidOperationException(String.Format("UserId={0}：{1}", auPaymentModel.AuId, AU_ERROR_PAYCERTFORCONTBILL))
            End If
        End If
        Return RedirectToAction("Index") '例外発生してエラーページに行くのでここにはこないはず。
    End Function

#End Region

#Region "au簡単決済"

    ''' <summary>
    ''' セッションタイムアウト対策
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    Public Function AuPaymentAuth() As ActionResult
        Return RedirectToAction("Regist")
    End Function

    ''' <summary>
    ''' ユーザ認証に成功したときにAUからリダイレクトされてくる
    ''' </summary>
    ''' <param name="transactionId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyApiAuthorize()>
    Public Function AuPaymentAuthResult(transactionId As String) As ActionResult
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        If mainModel IsNot Nothing OrElse auPaymentModel IsNot Nothing Then
            auPaymentModel.TransctionId = transactionId
            '状態をDB保存
            PremiumWorker.UpdatePremiumRecordToUserPermitBridge(Me.GetQolmsYappliModel, auPaymentModel)
            '継続課金情報登録要求をPostする
            If AuPaymentAccessWorker.SetContinuedBilling(auPaymentModel) Then
                '課金情報を会員情報テーブルに保存する
                If PremiumWorker.UpdatePremiumRecordToSuccessPremiumRegist(Me.GetQolmsYappliModel, auPaymentModel) Then
                    Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime
                    'Me.GetQolmsYappliModel.AuthorAccount.MembershipExpirationDate = auPaymentModel.ContinueAccountStartDate.AddDays(-1)
                    ' プレミアム会員初登録時にポイント付与
                    'Dim actionDate As Date = Now
                    ''Dim limit As Date = Now.Date.AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                    'Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                    'Dim pointItemList As New List(Of QolmsPointGrantItem)
                    'pointItemList.Add(New QolmsPointGrantItem(QyMemberShipTypeEnum.LimitedTime, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.InitialRegistration, limit)) '初回プレミアム登録
                    'Task.Run(
                    '    Sub()
                    '        QolmsPointWorker.AddQolmsPoints(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey,
                    '        mainModel.AuthorAccount.AccountKey, pointItemList)
                    '    End Sub
                    ')
                End If
            Else
                '継続課金情報登録に失敗
                PremiumWorker.UpdatePremiumRecordToFailedPremiumRegist(Me.GetQolmsYappliModel, auPaymentModel)
                Me.SetErrorMessage("継続課金情報登録に失敗しました。恐れ入りますが、最初からやり直してください。")
                Throw New InvalidOperationException(String.Format("TransactionId={0}：{1}", transactionId, AU_ERROR_CONTBILLREQUEST))
            End If
            'セッション削除
            QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
            Return RedirectToAction("Index")
        Else
            Me.SetErrorMessage("タイムアウトしました。恐れ入りますが、最初からやり直してください。")
            Throw New InvalidOperationException(String.Format("TransactionId={0}：{1}", transactionId, TIMEOUT_ERROR_PAYCERTFORCONTBILL))
        End If
    End Function

    ''' <summary>
    ''' 失敗したときにAuからリダイレクトされてくる
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyApiAuthorize()>
    Public Function AuPaymentAuthError() As ActionResult
        Dim errorCode As String = ""
        Dim transactionId As String = ""
        Dim errorMessage As String = ""

        'エラーコードを取得
        If Request.Params.AllKeys.Contains("X-ResultCd") Then
            errorCode = Request.Params("X-ResultCd")
        ElseIf Request.Headers.AllKeys.Contains("X-ResultCd") Then
            errorCode = Request.Headers("X-ResultCd")
        End If
        'トランザクションIDを取得
        If Request.Params.AllKeys.Contains("transactionId") Then
            transactionId = Request.Params("transactionId")
        End If

        Select Case errorCode
            Case "MPL40200"
                errorMessage = String.Format("購入がキャンセルされました。({0})", errorCode)
            Case Else
                errorMessage = String.Format("不明なエラーです。({0})", errorCode)
        End Select

        'セッションが残ってたらDBをUpdate
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        If auPaymentModel IsNot Nothing Then
            PremiumWorker.UpdatePremiumRecordToFailedPremiumRegist(Me.GetQolmsYappliModel, auPaymentModel)
            QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
        End If


        Me.SetErrorMessage(errorMessage)
        Throw New InvalidOperationException(String.Format("TransactionId={0}：{1}", transactionId, errorMessage))

    End Function

#End Region

#Region "pay.jp 個人情報同意画面"

    ''' <summary>
    ''' Pay.jpの個人情報同意画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyApiAuthorize()>
    Public Function PayJpAgree(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Return View(PremiumPayJPWorker.CreateAgreeViewModel(Me.GetQolmsYappliModel(), fromPageNoType))

    End Function

#End Region

#Region "Pay.jp 新規登録"

    ''' <summary>
    ''' Pay.jpのカード登録画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function PayJpCardRegister() As ActionResult

        Dim mainmodel As QolmsYappliModel = Me.GetQolmsYappliModel
        Dim paymentModel As PaymentInf = Nothing

        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
        If paymentModel Is Nothing Then
            paymentModel = PremiumWorker.GetPremiumRecord(mainmodel)
        End If

        Return View(PremiumPayJPWorker.CreateViewModel(mainmodel, paymentModel))

    End Function

    ''' <summary>
    ''' Pay.jpのカード登録3Dセキュア
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("tds")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    Public Function PayJpCardRegisterResult(token As String, dummy As String) As ActionResult

        If String.IsNullOrWhiteSpace(token) Then

            Return New PremiumPayjpJsonResult() With {
                        .IsSuccess = Boolean.FalseString,
                        .Message = "クレジットカード決済の登録が失敗しました。カード情報を入力しなおして登録してください。"
                    }.ToJsonResult()

        Else
            'Tokenをセッションに保存
            QySessionHelper.SetItem(Me.Session, "payjptoken", token)

            Return New PremiumPayjpJsonResult() With {
                        .IsSuccess = Boolean.TrueString
                    }.ToJsonResult()
        End If


    End Function

    ''' <summary>
    ''' Pay.jpのカード登録画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function PayJpCardRegisterTokenRedirect() As ActionResult

        Dim token As String = String.Empty
        QySessionHelper.GetItem(Of String)(Me.Session, "payjptoken", token)

        Dim url As String = PremiumPayJPWorker.RedirectTds(token, Me.Url.Action("payjpcardregisterresult", "premium"))
        Return Redirect(url)

    End Function

    ''' <summary>
    ''' Pay.jpのカード登録3Dセキュア
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize(True)>
    <QyApiAuthorize()>
    Public Function PayJpCardRegisterResult() As ActionResult

        Dim token As String = String.Empty
        QySessionHelper.GetItem(Of String)(Me.Session, "payjptoken", token)
        '3Dセキュア完了の確認
        Dim idtoken As String = PremiumPayJPWorker.TdsFinish(token)

        If String.IsNullOrWhiteSpace(idtoken) Then
            Throw New InvalidOperationException(String.Format("カード情報の登録が失敗しました。カード情報を入力しなおして登録してください。"))

            'Return New PremiumPayjpJsonResult() With {
            '    .IsSuccess = Boolean.FalseString,
            '    .Message = HttpUtility.HtmlEncode("クレジットカードの情報が不正です。")
            '}.ToJsonResult()

        End If

        Dim paymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
        If paymentModel Is Nothing Then
            paymentModel = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
        End If
        ' 課金開始日を初期化
        paymentModel.ContinueAccountStartDate = Date.MinValue
        Dim result As Integer = PremiumPayJPWorker.Register(Me.GetQolmsYappliModel, idtoken, paymentModel)
        If result = 1 OrElse result = 2 Then
            Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime
            'Return New PremiumPayjpJsonResult() With {
            '        .IsSuccess = Boolean.TrueString,
            '        .Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が完了しました。"),
            '        .IsExpiration = IIf(result = 1, Boolean.FalseString, Boolean.TrueString).ToString(),
            '        .ExpDate = paymentModel.ContinueAccountStartDate.AddDays(-1).ToShortDateString()
            '    }.ToJsonResult()
            Return RedirectToAction("index", "premium")
        Else
            'Return New PremiumPayjpJsonResult() With {
            '        .IsSuccess = Boolean.FalseString,
            '        .Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が失敗しました。カード情報を入力しなおして登録してください。")
            '    }.ToJsonResult()
            Return RedirectToAction("PayJpCardRegister")

        End If

    End Function

    '''' <summary>
    '''' Pay.jpのカード登録画面。
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks></remarks>
    '<HttpPost()>
    '<QyAjaxOnly()>
    '<QyAuthorize(True)>
    '<QyActionMethodSelector("Card")>
    '<ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    '<QyApiAuthorize()>
    'Public Function PayJpCardRegisterResult(token As String) As ActionResult

    '    If String.IsNullOrWhiteSpace(token) Then
    '        Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.FalseString,
    '            .Message = HttpUtility.HtmlEncode("クレジットカードの情報が不正です。")
    '        }.ToJsonResult()

    '    End If

    '    Dim paymentModel As PaymentInf = Nothing
    '    QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
    '    If paymentModel Is Nothing Then
    '        paymentModel = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
    '    End If
    '    ' 課金開始日を初期化
    '    paymentModel.ContinueAccountStartDate = Date.MinValue
    '    Dim result As Integer = PremiumPayJPWorker.Register(Me.GetQolmsYappliModel, token, paymentModel)
    '    If result = 1 OrElse result = 2 Then
    '        Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime
    '        Return New PremiumPayjpJsonResult() With {
    '                .IsSuccess = Boolean.TrueString,
    '                .Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が完了しました。"),
    '                .IsExpiration = IIf(result = 1, Boolean.FalseString, Boolean.TrueString).ToString(),
    '                .ExpDate = paymentModel.ContinueAccountStartDate.AddDays(-1).ToShortDateString()
    '            }.ToJsonResult()
    '    Else
    '        Return New PremiumPayjpJsonResult() With {
    '                .IsSuccess = Boolean.FalseString,
    '                .Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が失敗しました。カード情報を入力しなおして登録してください。")
    '            }.ToJsonResult()

    '    End If

    'End Function

    '''' <summary>
    '''' Pay.jpのカード登録画面。
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks></remarks>
    '<HttpPost()>
    '<QyAjaxOnly()>
    '<QyAuthorize(True)>
    '<QyActionMethodSelector("Reserve")>
    '<ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    '<QyApiAuthorize()>
    'Public Function PayJpCardRegisterResult() As JsonResult

    '    If PremiumPayJPWorker.Reserve(Me.GetQolmsYappliModel()) Then
    '        Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime

    '        Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.TrueString,
    '            .Message = HttpUtility.HtmlEncode("クレジットカード決済の予約が完了しました。")
    '        }.ToJsonResult()
    '    Else

    '        Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.FalseString,
    '            .Message = HttpUtility.HtmlEncode("予約の登録が失敗しました。")
    '        }.ToJsonResult()

    '    End If
    'End Function
#End Region

#Region "支払い方法変更"

    ''' <summary>
    ''' 支払い方法変更画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function MethodChange() As ActionResult

        Dim paymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
        If paymentModel Is Nothing Then
            paymentModel = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
        End If

        Return View(paymentModel)

    End Function

    ''' <summary>
    ''' 支払い方法変更画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAuthorize()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    Public Function MethodChangeResult(paymentType As Byte) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel
        If paymentType = 2 Then
            Dim page As Byte = QyPageNoTypeEnum.PremiumPayJpEdit
            Return RedirectToAction("PayJpAgree", New With {.fromPageNo = page})
        End If

        Dim paymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
        If paymentModel Is Nothing Then
            Return RedirectToAction("index")
        End If

        If paymentModel.PaymentType = 1 Then
            'すでにauかんたん決済
            Return RedirectToAction("index")
        End If

        '①pay.JP→au
        ' payJpの解約処理
        Dim isDemo As Boolean = False
        Dim settingStr As String = ConfigurationManager.AppSettings("DemoMode")
        If String.IsNullOrEmpty(settingStr) = False AndAlso settingStr.ToLower() = "true" Then
            isDemo = True
        End If

        If PremiumPayJPWorker.RequestContBillCancel(mainModel, paymentModel) Then
            PremiumPayJPWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, paymentModel)
            paymentModel.MemberManageNo = Long.MinValue
        Else
            '解約失敗
            Me.SetErrorMessage("クレジットカード決済の解約に失敗しました。") ' エラー画面に表示するメッセージ
            Throw New InvalidOperationException(String.Format("UserId={0}：{1}", paymentModel.AuId, AU_ERROR_PAYCERTFORCONTBILL))

        End If
        If isDemo Then
            If PremiumWorker.InsertPremiumRecord(mainModel, paymentModel) = True AndAlso paymentModel.MemberManageNo > 0 Then
                paymentModel.EndDate = Date.MaxValue
                If PremiumWorker.UpdatePremiumRecordToSuccessPremiumRegist(mainModel, paymentModel) Then
                    Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime
                    Return RedirectToAction("Index")
                End If
            End If
        Else
            ' au決済の登録処理
            If PremiumWorker.InsertPremiumRecord(mainModel, paymentModel) = True AndAlso paymentModel.MemberManageNo > 0 Then

                Dim redirectUri As String = AuPaymentAccessWorker.RequestPayCertAndGetRedirectUrl(paymentModel)
                'DBに保存
                PremiumWorker.UpdatePremiumRecordToPayCertRequested(mainModel, paymentModel)
                If String.IsNullOrEmpty(redirectUri) = False Then
                    'セッションに保存
                    QySessionHelper.SetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
                    '成功
                    Return Redirect(redirectUri)
                Else
                    '失敗
                    '既に課金中の場合は状態チェックして一致してたらエラーを返す必要がない。
                    If paymentModel.AuResultCode = AuPaymentAccessWorker.AUREGISTEREDERRORCODE Then
                        'DBと状態が一致しているかチェック
                        Dim dbMemberShipType As Byte = PremiumWorker.GetMemberShipType(Me.GetQolmsYappliModel)
                        If dbMemberShipType = QyMemberShipTypeEnum.LimitedTime OrElse dbMemberShipType = QyMemberShipTypeEnum.Premium _
                            OrElse dbMemberShipType = QyMemberShipTypeEnum.Business OrElse dbMemberShipType = QyMemberShipTypeEnum.BusinessFree Then
                            Return RedirectToAction("index")
                        End If
                    End If
                    'セッション削除
                    QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                    Me.SetErrorMessage("決済認可要求に失敗しました。") ' エラー画面に表示するメッセージ
                    Throw New InvalidOperationException(String.Format("UserId={0}：{1}", paymentModel.AuId, AU_ERROR_PAYCERTFORCONTBILL))
                End If
            End If

        End If

    End Function


#End Region

#Region "pay.Jp カード更新"

    ''' <summary>
    ''' pay.Jpのカード更新画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function PayJpCardUpdate() As ActionResult

        Dim mainmodel As QolmsYappliModel = Me.GetQolmsYappliModel
        Dim paymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
        If paymentModel Is Nothing Then
            paymentModel = PremiumWorker.GetPremiumRecord(mainmodel)
        End If

        Return View(PremiumPayJPWorker.CreateViewModel(mainmodel, paymentModel))

    End Function

    ''' <summary>
    ''' Pay.jpのカード登録3Dセキュア
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("tds")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    Public Function PayJpCardUpdateResult(token As String, dummy As String) As ActionResult

        If String.IsNullOrWhiteSpace(token) Then

            Return New PremiumPayjpJsonResult() With {
                        .IsSuccess = Boolean.FalseString,
                        .Message = "クレジットカード決済の登録が失敗しました。カード情報を入力しなおして登録してください。"
                    }.ToJsonResult()

        Else
            'Tokenをセッションに保存
            QySessionHelper.SetItem(Me.Session, "payjptoken", token)

            Return New PremiumPayjpJsonResult() With {
                        .IsSuccess = Boolean.TrueString
                    }.ToJsonResult()
        End If
    End Function

    ''' <summary>
    ''' Pay.jpのカード登録画面。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function PayJpCardUpdateTokenRedirect() As ActionResult

        Dim token As String = String.Empty

        QySessionHelper.GetItem(Of String)(Me.Session, "payjptoken", token)
        Dim url As String = PremiumPayJPWorker.RedirectTds(token, Me.Url.Action("payjpcardupdateresult", "premium"))
        Return Redirect(url)

    End Function

    ''' <summary>
    ''' Pay.jpのカード登録3Dセキュア
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize(True)>
    <QyApiAuthorize()>
    Public Function PayJpCardUpdateResult() As ActionResult

        Dim token As String = String.Empty
        QySessionHelper.GetItem(Of String)(Me.Session, "payjptoken", token)
        '3Dセキュア完了の確認
        Dim idtoken As String = PremiumPayJPWorker.TdsFinish(token)


        If String.IsNullOrWhiteSpace(idtoken) Then
            Throw New InvalidOperationException(String.Format("カード情報の登録が失敗しました。カード情報を入力しなおして登録してください。"))
            'Return New PremiumPayjpJsonResult() With {
            '    .IsSuccess = Boolean.FalseString,
            '    .Message = HttpUtility.HtmlEncode("クレジットカードの情報が不正です。")
            '}.ToJsonResult()

        End If

        Dim mainmodel As QolmsYappliModel = Me.GetQolmsYappliModel
        Dim paymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
        If paymentModel Is Nothing Then
            paymentModel = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
        End If

        Dim menbershipType As QyMemberShipTypeEnum = Me.GetQolmsYappliModel.AuthorAccount.MembershipType
        If (menbershipType = QyMemberShipTypeEnum.LimitedTime OrElse menbershipType = QyMemberShipTypeEnum.Premium) AndAlso paymentModel.PaymentType = 1 Then
            'プレミアム会員で、au決済の場合
            '②au → pay.jp
            ' au決済の解約処理
            Dim isDemo As Boolean = False
            Dim settingStr As String = ConfigurationManager.AppSettings("DemoMode")
            If String.IsNullOrEmpty(settingStr) = False AndAlso settingStr.ToLower() = "true" Then
                isDemo = True
            End If
            If isDemo Then
                Dim auPaymentModel As PaymentInf = Nothing
                QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
                If PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainmodel, auPaymentModel) Then

                    Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Free
                    QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                Else
                    'Log
                    AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(String.Format("auid={0}：{1}", auPaymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST)))
                    Throw New InvalidOperationException(String.Format("auid={0}：{1}", auPaymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST))
                    'Return New PremiumPayjpJsonResult() With {
                    '    .IsSuccess = Boolean.FalseString,
                    '    .Message = HttpUtility.HtmlEncode("退会処理に失敗しました。サポート窓口へご連絡ください。")
                    '}.ToJsonResult()
                End If
            Else
                If AuPaymentAccessWorker.RequestContBillCancel(paymentModel) Then

                    Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Free

                    PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainmodel, paymentModel)
                    QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                Else
                    If paymentModel.AuResultCode = "MPL40011" Then    'すでに解約済みエラー
                        'もう一度ステータス確認し、ステータスをそろえる
                        Dim oldAuPaymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(mainmodel)
                        If oldAuPaymentModel.MemberShipType <> QyMemberShipTypeEnum.Free Then
                            If paymentModel.EndDate = Date.MaxValue Then paymentModel.EndDate = Now.Date
                            PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainmodel, paymentModel)
                        End If
                        QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                    Else
                        PremiumWorker.UpdatePremiumRecordToCancelPremiumRegistFailed(mainmodel, paymentModel)
                        'Log
                        AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(String.Format("auid={0}：{1}", paymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST)))
                        Throw New InvalidOperationException(String.Format("auid={0}：{1}", paymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST))
                    End If
                End If
            End If

            paymentModel.MemberManageNo = Long.MinValue
            ' pay.jpの登録処置
            Dim result As Integer = PremiumPayJPWorker.Register(mainmodel, idtoken, paymentModel)
            If result = 1 OrElse result = 2 Then
                Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime
                Return RedirectToAction("index", "premium")
                'Return New PremiumPayjpJsonResult() With {
                '    .IsSuccess = Boolean.TrueString,
                '    .Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が完了しました。"),
                '    .IsExpiration = IIf(result = 1, Boolean.FalseString, Boolean.TrueString).ToString(),
                '    .ExpDate = paymentModel.ContinueAccountStartDate.AddDays(-1).ToShortDateString()
                '    }.ToJsonResult()
            Else
                Throw New InvalidOperationException(String.Format("カード情報の登録が失敗しました。カード情報を入力しなおして登録してください。"))
                'Return New PremiumPayjpJsonResult() With {
                '.IsSuccess = Boolean.FalseString,
                '.Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が失敗しました。カード情報を入力しなおして登録してください。")
                '}.ToJsonResult()
            End If
        ElseIf menbershipType = QyMemberShipTypeEnum.Business OrElse menbershipType = QyMemberShipTypeEnum.BusinessFree Then

            Return RedirectToAction("index", "premium")
            'Return New PremiumPayjpJsonResult() With {
            '    .IsSuccess = Boolean.FalseString,
            '    .Message = HttpUtility.HtmlEncode("ビジネス版ではプレミアムを利用できません。")
            '}.ToJsonResult()

        Else
            '③payjp→payjp(カードの変更) or 予約カード登録
            Dim result As Integer = PremiumPayJPWorker.Register(mainmodel, idtoken, paymentModel)
            If result = 1 OrElse result = 2 Then
                '    Return New PremiumPayjpJsonResult() With {
                '    .IsSuccess = Boolean.TrueString,
                '    .Message = HttpUtility.HtmlEncode("カード情報の登録が成功しました。"),
                '    .IsExpiration = IIf(result = 1, Boolean.FalseString, Boolean.TrueString).ToString(),
                '    .ExpDate = paymentModel.ContinueAccountStartDate.AddDays(-1).ToShortDateString()
                '}.ToJsonResult()
                Return RedirectToAction("index", "premium")

            Else
                Throw New InvalidOperationException(String.Format("カード情報の登録が失敗しました。カード情報を入力しなおして登録してください。"))
                'Return New PremiumPayjpJsonResult() With {
                '.IsSuccess = Boolean.FalseString,
                '.Message = HttpUtility.HtmlEncode("カード情報の登録が失敗しました。カード情報を入力しなおして登録してください。")
                '}.ToJsonResult()
            End If

        End If


    End Function


    '''' <summary>
    '''' pay.Jpのカード更新画面。
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks></remarks>
    '<HttpPost()>
    '<QyAjaxOnly()>
    '<QyAuthorize(True)>
    '<ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    '<QyApiAuthorize()>
    'Public Function PayJpCardUpdateResult(token As String) As ActionResult

    '    If String.IsNullOrWhiteSpace(token) Then
    '        Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.FalseString,
    '            .Message = HttpUtility.HtmlEncode("クレジットカードの情報が不正です。")
    '        }.ToJsonResult()

    '    End If

    '    Dim mainmodel As QolmsYappliModel = Me.GetQolmsYappliModel
    '    Dim paymentModel As PaymentInf = Nothing
    '    QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
    '    If paymentModel Is Nothing Then
    '        paymentModel = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
    '    End If

    '    Dim menbershipType As QyMemberShipTypeEnum = Me.GetQolmsYappliModel.AuthorAccount.MembershipType
    '    If (menbershipType = QyMemberShipTypeEnum.LimitedTime OrElse menbershipType = QyMemberShipTypeEnum.Premium) AndAlso paymentModel.PaymentType = 1 Then
    '        'プレミアム会員で、au決済の場合
    '        '②au → pay.jp
    '        ' au決済の解約処理
    '        Dim isDemo As Boolean = False
    '        Dim settingStr As String = ConfigurationManager.AppSettings("DemoMode")
    '        If String.IsNullOrEmpty(settingStr) = False AndAlso settingStr.ToLower() = "true" Then
    '            isDemo = True
    '        End If
    '        If isDemo Then
    '            Dim auPaymentModel As PaymentInf = Nothing
    '            QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
    '            If PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainmodel, auPaymentModel) Then

    '                Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Free
    '                QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
    '            Else
    '                'Log
    '                AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(String.Format("auid={0}：{1}", auPaymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST)))
    '                Return New PremiumPayjpJsonResult() With {
    '                    .IsSuccess = Boolean.FalseString,
    '                    .Message = HttpUtility.HtmlEncode("退会処理に失敗しました。サポート窓口へご連絡ください。")
    '                }.ToJsonResult()
    '            End If
    '        Else
    '            If AuPaymentAccessWorker.RequestContBillCancel(paymentModel) Then

    '                Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Free

    '                PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainmodel, paymentModel)
    '                QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
    '            Else
    '                If paymentModel.AuResultCode = "MPL40011" Then    'すでに解約済みエラー
    '                    'もう一度ステータス確認し、ステータスをそろえる
    '                    Dim oldAuPaymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(mainmodel)
    '                    If oldAuPaymentModel.MemberShipType <> QyMemberShipTypeEnum.Free Then
    '                        If paymentModel.EndDate = Date.MaxValue Then paymentModel.EndDate = Now.Date
    '                        PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainmodel, paymentModel)
    '                    End If
    '                    QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
    '                Else
    '                    PremiumWorker.UpdatePremiumRecordToCancelPremiumRegistFailed(mainmodel, paymentModel)
    '                    'Log
    '                    AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(String.Format("auid={0}：{1}", paymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST)))
    '                    Return New PremiumPayjpJsonResult() With {
    '                        .IsSuccess = Boolean.FalseString,
    '                        .Message = HttpUtility.HtmlEncode("退会処理に失敗しました。サポート窓口へご連絡ください。")
    '                    }.ToJsonResult()
    '                End If
    '            End If
    '        End If

    '        paymentModel.MemberManageNo = Long.MinValue
    '        ' pay.jpの登録処置
    '        Dim result As Integer = PremiumPayJPWorker.Register(mainmodel, token, paymentModel)
    '        If result = 1 OrElse result = 2 Then
    '            Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime
    '            Return New PremiumPayjpJsonResult() With {
    '                .IsSuccess = Boolean.TrueString,
    '                .Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が完了しました。"),
    '                .IsExpiration = IIf(result = 1, Boolean.FalseString, Boolean.TrueString).ToString(),
    '                .ExpDate = paymentModel.ContinueAccountStartDate.AddDays(-1).ToShortDateString()
    '                }.ToJsonResult()
    '        Else
    '            Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.FalseString,
    '            .Message = HttpUtility.HtmlEncode("クレジットカード決済の登録が失敗しました。カード情報を入力しなおして登録してください。")
    '            }.ToJsonResult()
    '        End If
    '    ElseIf menbershipType = QyMemberShipTypeEnum.Business OrElse menbershipType = QyMemberShipTypeEnum.BusinessFree Then

    '        Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.FalseString,
    '            .Message = HttpUtility.HtmlEncode("ビジネス版ではプレミアムを利用できません。")
    '        }.ToJsonResult()

    '    Else
    '        '③payjp→payjp(カードの変更) or 予約カード登録
    '        Dim result As Integer = PremiumPayJPWorker.Register(mainmodel, token, paymentModel)
    '        If result = 1 OrElse result = 2 Then
    '            Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.TrueString,
    '            .Message = HttpUtility.HtmlEncode("カード情報の登録が成功しました。"),
    '            .IsExpiration = IIf(result = 1, Boolean.FalseString, Boolean.TrueString).ToString(),
    '            .ExpDate = paymentModel.ContinueAccountStartDate.AddDays(-1).ToShortDateString()
    '        }.ToJsonResult()
    '        Else
    '            Return New PremiumPayjpJsonResult() With {
    '            .IsSuccess = Boolean.FalseString,
    '            .Message = HttpUtility.HtmlEncode("カード情報の登録が失敗しました。カード情報を入力しなおして登録してください。")
    '            }.ToJsonResult()
    '        End If

    '    End If

    'End Function

#End Region

#Region "お支払い履歴"

    ''' <summary>
    ''' 「お支払い履歴」画面
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function History() As ActionResult

        ' ビューを返却
        Return View(PremiumHistoryWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function


#End Region

#Region "プレミアム会員退会"

    ''' <summary>
    ''' プレミアム会員退会処理
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function Leave() As ActionResult
        'TODO: とりあえずのアクションに退会処理書いとく、最終的にはなにか確認表示とかしてからではないかと。そのとき-ResultというPostアクションにする。

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        If auPaymentModel IsNot Nothing Then
            If AuPaymentAccessWorker.RequestContBillCancel(auPaymentModel) Then

                Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Free

                PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel)
                QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                Return RedirectToAction("Index")
            Else
                If auPaymentModel.AuResultCode = "MPL40011" Then    'すでに解約済みエラー
                    'もう一度ステータス確認し、ステータスをそろえる
                    Dim oldAuPaymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(mainModel)
                    If oldAuPaymentModel.MemberShipType <> QyMemberShipTypeEnum.Free Then
                        If auPaymentModel.EndDate = Date.MaxValue Then auPaymentModel.EndDate = Now.Date
                        PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel)
                    End If
                    QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                    Return RedirectToAction("Index")
                End If
                PremiumWorker.UpdatePremiumRecordToCancelPremiumRegistFailed(mainModel, auPaymentModel)
                Me.SetErrorMessage("課金登録解除に失敗しました。サポート窓口へご連絡ください。")
                Throw New InvalidOperationException(String.Format("auid={0}：{1}", auPaymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST))
            End If
        End If

    End Function

#End Region


#Region "Dummy"
    ''' <summary>
    ''' プレミアム会員登録したふり。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>決済認可要求しない</remarks>
    <HttpPost()>
    <QyAuthorize()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    Public Function DummyRegistResult() As ActionResult
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        If PremiumWorker.InsertPremiumRecord(mainModel, auPaymentModel) = True AndAlso auPaymentModel.MemberManageNo > 0 Then
            auPaymentModel.StartDate = Now.Date
            auPaymentModel.EndDate = Date.MaxValue
            If PremiumWorker.UpdatePremiumRecordToSuccessPremiumRegist(mainModel, auPaymentModel) Then
                Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.LimitedTime
                Return RedirectToAction("Index")
            End If
        End If
        Return RedirectToAction("Index") '例外発生してエラーページに行くのでここにはこないはず。
    End Function

    <HttpGet()>
    <QyAuthorize()>
    Public Function DummyRegist() As ActionResult
        Return View("Regist")
    End Function

    ''' <summary>
    ''' プレミアム会員退会処理
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    Public Function DummyLeave() As ActionResult
        'TODO: とりあえずのアクションに退会処理書いとく、最終的にはなにか確認表示とかしてからではないかと。そのとき-ResultというPostアクションにする。

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)
        If PremiumWorker.UpdatePremiumRecordToCancelPremiumRegist(mainModel, auPaymentModel) Then

            Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Free

            QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
            Return RedirectToAction("Index")
        Else
            Me.SetErrorMessage("退会処理に失敗しました。サポート窓口へご連絡ください。")
            Throw New InvalidOperationException(String.Format("auid={0}：{1}", auPaymentModel.AuId, AU_ERROR_CONTBILLCANCELREQUEST))
        End If
    End Function
#End Region

End Class
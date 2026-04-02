Imports System.Net
Imports System.Web.Routing
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Public NotInheritable Class HealthController
    Inherits QyMvcControllerBase

#Region "Private Property"

    ''' <summary>
    ''' Yappli から呼び出されているかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property IsYappli As Boolean

        Get
            Return Me.Request.UserAgent.ToLower().Contains("yappli")
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthController" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "ページ ビュー アクション"

#Region "「健康年齢」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Age(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None

        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Return View(HealthAgeWorker.CreateViewModel(Me.GetQolmsYappliModel(), fromPageNoType))

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Report")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AgeResult(healthAgeReportType As QyHealthAgeReportTypeEnum) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As HealthAgeViewModel = mainModel.GetInputModelCache(Of HealthAgeViewModel)()
        Dim partialViewName As String = String.Empty
        Dim partialViewModel As QyHealthAgeReportPartialViewModelBase = HealthAgeWorker.CreateReportPartialViewModel(mainModel, pageViewModel, healthAgeReportType, partialViewName)

        ' パーシャルビューを返却
        Return PartialView(partialViewName, partialViewModel)

    End Function

#End Region

#Region "「健康年齢入力」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AgeEdit(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 編集対象のモデルを作成
        Dim inputModel As HealthAgeEditInputModel = HealthAgeEditWorker.CreateInputModel(mainModel, fromPageNoType)

        ' モデルをキャッシュへ格納（入力検証エラー時の再展開用）
        mainModel.SetInputModelCache(inputModel)

        ' ビューを返却
        Return View("AgeEdit", inputModel)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Edit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AgeEditResult(model As HealthAgeEditInputModel) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As HealthAgeEditInputModel = mainModel.GetInputModelCache(Of HealthAgeEditInputModel)()

        ' モデルへ入力値をを反映
        inputModel.UpdateByInput(model)

        ' モデルの検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功
            Dim errorMessage As New StringBuilder()

            ' 健診受信日の時点での年齢が 18 歳以上かつ 74 歳以下かチェック
            errorMessage.Append(HealthAgeEditWorker.CheckRecordDate(inputModel.AuthorBirthday, inputModel.RecordDate))

            If errorMessage.Length = 0 Then
                ' 健康年齢 Web API を実行
                Dim responseN As List(Of QhApiHealthAgeResponseItem) = HealthAgeEditWorker.ExecuteJmdcHealthAgeApi(mainModel, inputModel)

                responseN.ForEach(
                    Sub(i)
                        If i.StatusCode.TryToValueType(500) <> 200 Then errorMessage.AppendFormat("{0} {1}{2}", i.StatusCode, i.Message, Environment.NewLine)
                    End Sub
                )

                If errorMessage.Length = 0 Then
                    ' 登録
                    If HealthAgeEditWorker.Edit(mainModel, inputModel, responseN) Then
                        ' モデルをキャッシュからクリア
                        mainModel.RemoveInputModelCache(Of HealthAgeEditInputModel)()
                    End If

                    Dim dict As New RouteValueDictionary()
                    dict.Add("fromPageNo", Convert.ToByte(inputModel.FromPageNoType))

                    ' 「健康年齢」画面へ遷移
                    Return RedirectToAction("Age", "Health", dict)
                Else
                    ' 健康年齢 Web APIエラー
                    AccessLogWorker.WriteErrorLog(Me.HttpContext, String.Empty, errorMessage.ToString())

                    ' 独自にエラーメッセージを用意しビューに渡す
                    Me.TempData("ErrorMessage") = New Dictionary(Of String, String) From {{"HealthAgeApi", "健康年齢を測定出来ませんでした。健康年齢WEBAPIがメンテナンス中の可能性があります。"}}

                    ' ビューを返却
                    Return View("AgeEdit", inputModel)
                End If
            Else
                ' 年齢チェックエラー

                ' 独自にエラーメッセージを用意しビューに渡す
                Me.TempData("ErrorMessage") = New Dictionary(Of String, String) From {{"model.RecordDate", errorMessage.ToString()}}

                ' ビューを返却
                Return View("AgeEdit", inputModel)
            End If
        Else
            ' 検証失敗

            ' 独自にエラーメッセージを用意しビューに渡す
            Dim errorMessage As New Dictionary(Of String, String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(key, e.ErrorMessage)
                Next
            Next

            Me.TempData("ErrorMessage") = errorMessage

            ' ビューを返却
            Return View("AgeEdit", inputModel)
        End If

        Return View()

    End Function

#End Region

    '#Region "「健康相談」画面"

    '    <HttpGet()>
    '    Public Function Consult() As ActionResult

    '        Return View()

    '    End Function

    '#End Region

#End Region

#Region "パーシャル ビュー アクション"

#Region "共通パーツ"

    ''' <summary>
    ''' 「ヘッダー」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function HealthHeaderPartialView() As ActionResult

        ' パーシャル ビューを返却
        If Me.IsYappli Then
            Return New EmptyResult()
        Else
            Return PartialView("_HealthHeaderPartialView")
        End If

    End Function

    ''' <summary>
    ''' 「フッター」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    <OutputCache(Duration:=600)>
    Public Function HealthFooterPartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_HealthFooterPartialView")

    End Function

#End Region

#Region "「健康年齢」画面用 パーシャル ビュー"

    ''' <summary>
    ''' 「健康年齢」画面用
    ''' 「健康年齢改善アドバイス」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function HealthAgeAdviceAreaPartialView() As ActionResult

        Dim model As HealthAgeViewModel = Me.GetPageViewModel(Of HealthAgeViewModel)()

        ' パーシャル ビューを返却
        Return PartialView("_HealthAgeAdviceAreaPartialView", New HealthAgeAdviceAreaPartialViewModel(model))

    End Function

    ''' <summary>
    ''' 「健康年齢」画面用
    ''' 「健康年齢の推移」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function HealthAgeTransitionAreaPartialView() As ActionResult

        Dim model As HealthAgeViewModel = Me.GetPageViewModel(Of HealthAgeViewModel)()

        ' パーシャル ビューを返却
        Return PartialView("_HealthAgeTransitionAreaPartialView", New HealthAgeTransitionAreaPartialViewModel(model))

    End Function

#End Region

#End Region

End Class

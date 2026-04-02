Imports System.Threading.Tasks
Imports System.Web.Mvc
Imports MGF.QOLMS
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsCryptV1


Public NotInheritable Class PortalController
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
    ''' <see cref="PortalController" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region


#Region "ページ ビュー アクション"

#Region "TEST用画面"

    '<HttpGet()>
    'Public Function Test() As ActionResult

    '    ' TODO: test
    '    PayJpTestWorker.Test()

    '    Return View("Test")

    'End Function

#End Region

#Region "「ホーム」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    <QyViewCount(PageNo:=QyPageNoTypeEnum.PortalHome)>
    Public Function Home() As ActionResult
        
        'TODO: Test code
        TempData("s") = String.Format("debug情報>useragent:{0},session:{1}", Me.Request.UserAgent, Me.Session.SessionID)
        'Return View(New PortalHomeViewModel() With {
        '            .PointMax = 6500,
        '            .CalBreakfast = 400,
        '            .CalLunch = 800,
        '            .CalDinner = 750,
        '            .TargetCalorieIn = 3250,
        '            .TargetCalorieOut = 3250,
        '            .Steps = 8000,
        '            .ExerciseCal = 300
        '            })
        '
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As PortalHomeViewModel = PortalHomeWorker.CreateViewModel(mainModel, Me.IsYappli)
        'viewModelをキャッシュ
        mainModel.SetInputModelCache(viewModel)

        Return View(viewModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Left,Right,Monthly,Today,Daily,Weekly")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult(monthly As Boolean, nowDay As Date) As ActionResult

        'モードと表示日（月）を指定してページを再取得
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As PortalHomeViewModel = mainModel.GetInputModelCache(Of PortalHomeViewModel)() 'mainModel.GetInputModelCache(Of PortalHomeViewModel)()
       
        Dim Mode As String 
        Dim showDay As Date 
        
        If pageViewModel Is Nothing Then
            'キャッシュのモデルがなかったら取り直し
            pageViewModel = PortalHomeWorker.CreateViewModel(mainModel, Me.IsYappli)

        End If

        Mode= pageViewModel.PartialViewModel.Mode
        showDay = pageViewModel.PartialViewModel.ShowDay
        If nowDay <> Date.MinValue Then pageViewModel.PartialViewModel.ShowDay = nowDay ' POST された日付に置き換え


        'Todo : weeklyの追加
        Select Case ActionSource
            Case "Today"
                'TODAY　ボタンを押して本日の情報を取得
                Mode = "Daily"
                showDay = Date.Now
            Case "Daily"
                '日(Daily)ボタンを押して本日の情報を取得
                Mode = "Daily"
                showDay = Date.Now
            Case "Weekly"
                '週(weekly)ボタンを押して今週の情報を取得
                Mode = "Weekly"
                showDay = Date.Now
            Case "Monthly"
                '月(monthly)ボタンを押して今月の情報を取得
                Mode = "Monthly"
                showDay = Date.Now
            Case "Right"
                '日付遷移 右(後)
                If Mode = "Daily" Then
                    showDay = pageViewModel.PartialViewModel.ShowDay.AddDays(1)

                ElseIf Mode = "Weekly" Then
                    showDay = pageViewModel.PartialViewModel.ShowDay.AddDays(7)

                ElseIf Mode = "Monthly" Then
                    showDay = pageViewModel.PartialViewModel.ShowDay.AddMonths(1)

                End If
            Case "Left"
                '日付遷移 左(前)
                If Mode = "Daily" Then
                    showDay = pageViewModel.PartialViewModel.ShowDay.AddDays(-1)

                ElseIf Mode = "Weekly" Then
                    showDay = pageViewModel.PartialViewModel.ShowDay.AddDays(-7)

                ElseIf Mode = "Monthly" Then
                    showDay = pageViewModel.PartialViewModel.ShowDay.AddMonths(-1)

                End If

        End Select

        'PartialViewModelを取得
        Dim partialViewModel As PortalHomeDataAreaPartialViewModel = PortalHomeWorker.CreatePartialViewModel(mainModel, pageViewModel, Mode, showDay)
        pageViewModel.PartialViewModel = partialViewModel
        'viewModelをキャッシュ
        mainModel.SetInputModelCache(pageViewModel)

        ' パーシャル ビューを返却
        Return PartialView("_PortalHomeDataAreaPartialView", partialViewModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Alkoo")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult() As ActionResult

        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} Start", ActionSource))

        Dim sw As New System.Diagnostics.Stopwatch()
        sw.Start()
        'ALKOOAPIから歩数の取得、1歩以上なら更新、ビューへ返却
        Dim steps As Integer = PortalHomeWorker.UpdateSteps(Me.GetQolmsYappliModel())

        sw.Stop()
        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

        Return New PortalHomeWalkUpdateJsonResult() With {
            .IsSuccess = Boolean.TrueString,
            .Steps = steps.ToString()
        }.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("News")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult(dummy As String) As ActionResult

        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} Start", ActionSource))

        Dim sw As New System.Diagnostics.Stopwatch()
        sw.Start()
        ' Newsを取得します。
        Dim news As List(Of String) = PortalHomeWorker.GetNews(Me.GetQolmsYappliModel())


        sw.Stop()
        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

        Return New PortalHomeNewsJsonResult() With {
            .IsSuccess = Boolean.TrueString,
            .News = news.ConvertAll(Function(i) HttpUtility.HtmlEncode(i))
        }.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Examination")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult(dummy As String, dummy2 As String) As ActionResult

        'AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} Start", ActionSource))

        'Dim sw As New System.Diagnostics.Stopwatch()
        'sw.Start()
        '対象の医療機関連携が成立しているかどうかを確認
        If PortalHomeWorker.IsExaminationShow(Me.GetQolmsYappliModel()) Then


            'sw.Stop()
            'AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

            Return New PortalHomeNewsJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        Else


            'sw.Stop()
            'AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

            Return New PortalHomeNewsJsonResult() With {
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()

        End If
    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Fitbit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult(dummy As String, dummy2 As String, dummy3 As String) As ActionResult

        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} Start", ActionSource))

        Dim sw As New System.Diagnostics.Stopwatch()
        sw.Start()

        '対象のFitbit連携が成立しているかどうかを確認
        If PortalHomeWorker.IsFitbitConnected(Me.GetQolmsYappliModel()) Then

            sw.Stop()
            AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

            Return New PortalHomeNewsJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        Else
            sw.Stop()
            AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

            Return New PortalHomeNewsJsonResult() With {
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()

        End If
    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("ChallengeTargetData")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult(targetDay As Integer) As ActionResult

        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} Start", ActionSource))

        Dim sw As New System.Diagnostics.Stopwatch()
        sw.Start()

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As PortalHomeViewModel = mainModel.GetInputModelCache(Of PortalHomeViewModel)() 'mainModel.GetInputModelCache(Of PortalHomeViewModel)()

        Dim startDate As Date = pageViewModel.ChallengeAreaPartialViewModel.ChallengeStartDate
        Dim showDay As Date = startDate.AddDays(targetDay - 1)

        'PartialViewModelを取得
        Dim partialViewModel As PortalHomeDataAreaPartialViewModel = PortalHomeWorker.CreatePartialViewModel(mainModel, pageViewModel, "Daily", showDay)
        pageViewModel.PartialViewModel = partialViewModel
        'viewModelをキャッシュ
        mainModel.SetInputModelCache(pageViewModel)

        sw.Stop()
        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

        ' パーシャル ビューを返却
        Return PartialView("_PortalHomeDataAreaPartialView", partialViewModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Task")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult(showDay As Date) As ActionResult

        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} Start", ActionSource))

        Dim sw As New System.Diagnostics.Stopwatch()
        sw.Start()

        Dim taskJson As PortalHomeTaskJsonResult = PortalHomeWorker.GetTasks(Me.GetQolmsYappliModel(), showDay)

        sw.Stop()
        AccessLogWorker.DebugLog(String.Format("Get HomeResult {0} End : {1}", ActionSource, sw.Elapsed))

        Return taskJson.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Url")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HomeResult(urlType As Byte) As ActionResult

        Return PortalHomeWorker.CreateUrl(Me.GetQolmsYappliModel(), urlType).ToJsonResult()

    End Function

#End Region

#Region "「医療機関検索」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Search() As ActionResult

        ' ビューを返却
        Return View(PortalSearchWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Search")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SearchResult(
                                SearchText As String,
                                SearchCity As String,
                                Optional SearchDepartment As Integer = Integer.MinValue,
                                Optional Index As Integer = 0,
                                Optional Latitude As Decimal = Decimal.MinValue,
                                Optional Longitude As Decimal = Decimal.MinValue,
                                Optional OptionFlags As Integer = Integer.MinValue,
                                Optional OpenFlag As Boolean = False) _
                            As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim resultPageIndex As Integer = Integer.MinValue
        Dim resultMaxPageIndex As Integer = Integer.MinValue
        Dim medicalN As New List(Of MedicalInstitutionItem)()
        Dim openDatetime As DateTime = DateTime.MinValue

        ' API を実行
        With PortalSearchWorker.Search(mainModel, SearchText, SearchDepartment, SearchCity, Index, Latitude, Longitude, OptionFlags, OpenFlag, resultPageIndex, resultMaxPageIndex, medicalN)

            ' パーシャルビューを返却
            Return PartialView("_PortalSearchResultPartialView", New PortalSearchViewModel(mainModel, SearchText, resultPageIndex, resultMaxPageIndex, medicalN))
        End With

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Detail")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SearchResult(CodeNo As String) As ActionResult

        Return View("_PortalSearchDetailResultPartialView", PortalSearchDetailWorker.CreateViewModel(Me.GetQolmsYappliModel(), CodeNo))

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Request")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SearchResult(CodeNo As String, RequestFlag As Boolean) As JsonResult

        Return PortalSearchDetailWorker.PostPayRequest(Me.GetQolmsYappliModel(), CodeNo, RequestFlag).ToJsonResult()

    End Function

#End Region

#Region "「情報（メニュー）」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Information() As ActionResult

        Return View(PortalInformationWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function

#End Region

#Region "「目標設定」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TargetSetting2() As ActionResult

        'Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        '' 編集対象のモデルを作成
        'Dim inputModel As PortalTargetSettingInputModel = PortalTargetSettingWorker.CreateInputModel(mainModel)

        '' モデルをキャッシュへ格納（入力検証エラー時の再展開用）
        'mainModel.SetInputModelCache(inputModel)

        '' ビューを返却
        'Return View("TargetSetting", inputModel)

        ' ビューを返却
        Return View(PortalTargetSettingWorker2.CreateInputModel(Me.GetQolmsYappliModel()))

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("StandardValue")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TargetSettingResult2(vitalType As QyVitalTypeEnum) As JsonResult

        Dim result As New PortalTargetSettingStandardValueJsonResult() With {
            .VitalType = QyVitalTypeEnum.None.ToString(),
            .Message = HttpUtility.HtmlEncode("標準値の取得に失敗しました。"),
            .IsSuccess = Boolean.FalseString
        }
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim lower1 As Decimal = Decimal.MinusOne
        Dim upper1 As Decimal = Decimal.MinusOne
        Dim lower2 As Decimal = Decimal.MinusOne
        Dim upper2 As Decimal = Decimal.MinusOne

        PortalTargetSettingWorker2.GetInputModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

        Select Case vitalType
            Case QyVitalTypeEnum.Steps
                ' 歩数
                TargetValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.None, lower1, upper1)
                result.Lower1 = If(lower1 >= 0, String.Format("{0:0.####}", lower1), String.Empty)
                result.Upper1 = If(upper1 >= 0, String.Format("{0:0.####}", upper1), String.Empty)

                result.VitalType = vitalType.ToString()
                result.Message = String.Empty
                result.IsSuccess = Boolean.TrueString

            Case QyVitalTypeEnum.BodyWeight
                ' 体重
                TargetValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.None, lower1, upper1)
                result.Lower1 = If(lower1 >= 0, String.Format("{0:0.####}", lower1), String.Empty)
                result.Upper1 = If(upper1 >= 0, String.Format("{0:0.####}", upper1), String.Empty)

                result.VitalType = vitalType.ToString()
                result.Message = String.Empty
                result.IsSuccess = Boolean.TrueString

            Case QyVitalTypeEnum.BloodPressure
                ' 血圧（上）
                TargetValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureUpper, lower1, upper1)
                result.Lower1 = If(lower1 >= 0, String.Format("{0:0.####}", lower1), String.Empty)
                result.Upper1 = If(upper1 >= 0, String.Format("{0:0.####}", upper1), String.Empty)

                ' 血圧（下）
                TargetValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodPressureLower, lower2, upper2)
                result.Lower2 = If(lower2 >= 0, String.Format("{0:0.####}", lower2), String.Empty)
                result.Upper2 = If(upper2 >= 0, String.Format("{0:0.####}", upper2), String.Empty)

                result.VitalType = vitalType.ToString()
                result.Message = String.Empty
                result.IsSuccess = Boolean.TrueString

            Case QyVitalTypeEnum.BloodSugar
                ' 血糖値（空腹時）
                TargetValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarFasting, lower1, upper1)
                result.Lower1 = If(lower1 >= 0, String.Format("{0:0.####}", lower1), String.Empty)
                result.Upper1 = If(upper1 >= 0, String.Format("{0:0.####}", upper1), String.Empty)

                ' 血糖値（その他）
                TargetValueWorker.GetStandardValue(mainModel, vitalType, QyStandardValueTypeEnum.BloodSugarOther, lower2, upper2)
                result.Lower2 = If(lower2 >= 0, String.Format("{0:0.####}", lower2), String.Empty)
                result.Upper2 = If(upper2 >= 0, String.Format("{0:0.####}", upper2), String.Empty)

                result.VitalType = vitalType.ToString()
                result.Message = String.Empty
                result.IsSuccess = Boolean.TrueString

        End Select

        ' JSON を返却
        Return result.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Edit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TargetSettingResult2(model As PortalTargetSettingInputModel2) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As PortalTargetSettingInputModel2 = PortalTargetSettingWorker2.GetInputModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策 mainModel.GetInputModelCache(Of PortalTargetSettingInputModel)()

        ' モデルへ入力値をを反映
        inputModel.UpdateByInput(model)

        Dim returnVal As New Dictionary(Of String, String)
        Dim success As String = Boolean.FalseString

        ' モデルの検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功

            ' データ登録
            If PortalTargetSettingWorker2.Edit(mainModel, inputModel) Then
                ' 登録成功

                ' モデルをキャッシュから削除
                mainModel.RemoveInputModelCache(Of PortalTargetSettingInputModel2)()
            Else
                ' 登録失敗
                Throw New InvalidOperationException("目標値の登録に失敗しました。")
            End If

            success = Boolean.TrueString

            ' ビューを返却
            If Me.IsYappli Then
                ' Yappli の場合は「登録完了」ダイアログを出す
                Me.TempData("IsFinish") = True

                'Return RedirectToAction("TargetSetting2", "Portal")
                returnVal.Add("Url", Url.Action("TargetSetting2", "Portal"))
            Else
                ' 「情報（メニュー）」画面へ遷移
                'Return RedirectToAction("Information", "Portal")
                returnVal.Add("Url", Url.Action("Information", "Portal"))
            End If
        Else
            ' 検証失敗

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    returnVal.Add(key, e.ErrorMessage)
                Next
            Next

        End If

        Return New PortalTargetSettingCalculationJsonResult() With {
            .IsSuccess = success,
            .Values = returnVal
        }.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Calculation")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TargetSettingResult2(height As String, weight As Decimal, physicalActivityLevel As Byte, type As String) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As PortalTargetSettingInputModel2 = PortalTargetSettingWorker2.GetInputModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策 mainModel.GetInputModelCache(Of PortalTargetSettingInputModel)()

        inputModel.Height = height
        inputModel.Weight = weight.ToString("#.0")
        inputModel.PhysicalActivityLevel = physicalActivityLevel

        Dim returnVal As New Dictionary(Of String, String)
        returnVal.Add("StdWeight", inputModel.StdWeight.ToString("#.0"))
        returnVal.Add("StdBasalMetabolism", inputModel.StdBasalMetabolism.ToString("#,###"))
        returnVal.Add("NowBasalMetabolism", inputModel.NowBasalMetabolism.ToString("#,###"))
        returnVal.Add("StdEstimatedEnergyRequirement", inputModel.StdEstimatedEnergyRequirement.ToString("#,###"))
        returnVal.Add("NowEstimatedEnergyRequirement", inputModel.NowEstimatedEnergyRequirement.ToString("#,###"))

        Return New PortalTargetSettingCalculationJsonResult() With {
                    .IsSuccess = Boolean.TrueString,
                    .Values = returnVal
                }.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("CalcTargetCalorieIn")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TargetSettingResult2(model As PortalTargetSettingInputModel2, buttonType As String) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As PortalTargetSettingInputModel2 = PortalTargetSettingWorker2.GetInputModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策 mainModel.GetInputModelCache(Of PortalTargetSettingInputModel)()

        ' モデルへ入力値をを反映
        inputModel.UpdateByInput(model)

        Dim returnVal As New Dictionary(Of String, String)
        Dim success As String = Boolean.FalseString

        ' モデルの検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功

            returnVal.Add("NowTargetCalorieIn", inputModel.NowTargetCalorieIn.ToString())
            success = Boolean.TrueString

        Else
            ' 検証失敗

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    returnVal.Add(key, e.ErrorMessage)
                Next
            Next

        End If

        Return New PortalTargetSettingCalculationJsonResult() With {
            .IsSuccess = success,
            .Values = returnVal
        }.ToJsonResult()

    End Function

#End Region

#Region "「利用規約」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Terms() As ActionResult

        Dim viewModel As PortalTermsViewModel = PortalTermsWorker.CreateViewModel(Me.GetQolmsYappliModel())
        Dim str As String = String.Empty
        Dim path As String = Me.HttpContext.Server.MapPath("~/App_Data/TermsOfUse.txt")

        If Not String.IsNullOrWhiteSpace(path) AndAlso IO.File.Exists(path) Then
            str = IO.File.ReadAllText(path)
        End If
        viewModel.TermsText = str

        Return View(viewModel)

    End Function
#End Region

#Region "「退会」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Unsubscribe() As ActionResult

        'プレミアム会員情報を取得
        Dim paymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
        'セッションに保存
        QySessionHelper.SetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)

        Dim inputModel As PortalUnsubscribeInputModel = PortalUnsubscribeWorker.CreateViewModel(Me.GetQolmsYappliModel())
        '退会説明
        Dim path As String = Me.HttpContext.Server.MapPath("~/App_Data/Unsubscribe.txt")
        Dim str As String = String.Empty
        If Not String.IsNullOrWhiteSpace(path) AndAlso IO.File.Exists(path) Then
            str = IO.File.ReadAllText(path)
        End If
        inputModel.Description = str
        Dim premiumPath As String = Me.HttpContext.Server.MapPath("~/App_Data/PremiumUnsubscribe.txt")
        Dim premiumStr As String = String.Empty
        If Not String.IsNullOrWhiteSpace(premiumPath) AndAlso IO.File.Exists(premiumPath) Then
            premiumStr = IO.File.ReadAllText(premiumPath)
        End If
        inputModel.PremiumDescription = premiumStr
        Return View(inputModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("All")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function UnsubscribeResult(model As PortalUnsubscribeInputModel) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        If Me.ModelState.IsValid Then
            Dim auPaymentModel As PaymentInf = Nothing
            QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)

            If auPaymentModel.MemberShipType = QyMemberShipTypeEnum.Business Or auPaymentModel.MemberShipType = QyMemberShipTypeEnum.BusinessFree Then
                'ビジネスプレミアムの場合は、画面に先に解約するエラーを返却
                Return New PortalUnsubscribeJsonResult() With {
                    .Message = HttpUtility.HtmlEncode("退会するには企業連携を先に解除してください。"),
                    .IsSuccess = Boolean.FalseString
                }.ToJsonResult()

            End If

            Try
                Select Case PortalUnsubscribeWorker.Register(mainModel, model, auPaymentModel)
                    Case 1
                        '成功
                        '退会ログ
                        Dim message As String = If(mainModel Is Nothing, String.Empty, String.Format("AccountKey={0}：Name={1}：{2}", mainModel.AuthorAccount.AccountKey, mainModel.AuthorAccount.Name, "退会しました"))
                        AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Delete, message)

                        ' ログアウト
                        'Dim logoutMessage As String = If(mainModel Is Nothing, String.Empty, String.Format("AccountKey={0}：Name={1}：{2}", mainModel.AuthorAccount.AccountKey, mainModel.AuthorAccount.Name, "ログアウトしました。"))
                        'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Logout, logoutMessage)
                        'QyLoginHelper.ToLogout(Me.Session, Me.Response, True)

                        'autoログイン関連設定のcookieを削除
                        QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                        '
                        QyCookieHelper.DisableRememberLoginCookie(Response)
                        QyCookieHelper.DisableRememberIdCookie(Response)

                        Return New PortalUnsubscribeJsonResult() With {
                        .IsLogout = Boolean.TrueString,
                        .Message = HttpUtility.HtmlEncode("退会処理が完了しました。"),
                        .IsSuccess = Boolean.TrueString}.ToJsonResult()
                    Case 2
                        '会員情報更新失敗
                        Return New PortalUnsubscribeJsonResult() With {
                        .Message = HttpUtility.HtmlEncode("退会処理に失敗しました。会員情報の削除に失敗しました。"),
                        .IsSuccess = Boolean.FalseString}.ToJsonResult()
                    Case Else
                        'プレミアム会員解約失敗
                        Return New PortalUnsubscribeJsonResult() With {
                        .Message = HttpUtility.HtmlEncode("退会処理に失敗しました。課金登録解除に失敗しました。サポート窓口へご連絡ください。"),
                        .IsSuccess = Boolean.FalseString}.ToJsonResult()
                End Select

            Catch ex As Exception
                AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, ex.Message)

                Return New PortalUnsubscribeJsonResult() With {
                    .Message = HttpUtility.HtmlEncode("退会処理に失敗しました。サポート窓口へご連絡ください。"),
                    .IsSuccess = Boolean.FalseString}.ToJsonResult()
            End Try
        Else
            ' inputModel invalid = false
            Dim errorMessage As New List(Of String)()
            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            Return New PortalUnsubscribeJsonResult() With {
                .Message = HttpUtility.HtmlEncode(errorMessage.First()),
                .IsSuccess = Boolean.FalseString}.ToJsonResult()

        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Premium")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function UnsubscribeResult() As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim auPaymentModel As PaymentInf = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(PaymentInf).Name, auPaymentModel)

        Try
            Select Case PortalUnsubscribeWorker.PremiumCancel(mainModel, auPaymentModel)
                Case 1
                    '成功
                    Me.GetQolmsYappliModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Free
                    'mainmodel の更新
                    QySessionHelper.RemoveItem(Me.Session, GetType(PaymentInf).Name)
                    'プレミアム会員情報を取得
                    Dim paymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)
                    QySessionHelper.SetItem(Me.Session, GetType(PaymentInf).Name, paymentModel)
                    'log
                    Dim message As String = If(mainModel Is Nothing, String.Empty, String.Format("AccountKey={0}：Name={1}：{2}", mainModel.AuthorAccount.AccountKey, mainModel.AuthorAccount.Name, "プレミアム会員を解約しました。"))
                    AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Delete, message)
                    Return New PortalUnsubscribeJsonResult() With {
                     .Message = HttpUtility.HtmlEncode("解約が完了しました。"),
                     .IsSuccess = Boolean.TrueString}.ToJsonResult()

                Case 2
                    Return New PortalUnsubscribeJsonResult() With {
                    .Message = HttpUtility.HtmlEncode("すでに解約済みです。"),
                    .IsSuccess = Boolean.FalseString}.ToJsonResult()

                Case Else
                    Return New PortalUnsubscribeJsonResult() With {
                    .Message = HttpUtility.HtmlEncode("課金登録解除に失敗しました。サポート窓口へご連絡ください。"),
                    .IsSuccess = Boolean.FalseString}.ToJsonResult()

            End Select

        Catch ex As Exception
            'エラーログ
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, ex.Message)

            Return New PortalUnsubscribeJsonResult() With {
             .Message = HttpUtility.HtmlEncode("課金登録解除に失敗しました。サポート窓口へご連絡ください。"),
             .IsSuccess = Boolean.FalseString}.ToJsonResult()

        End Try


    End Function

#End Region

#Region "「データチャージ」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Datacharge(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim viewModel As PortalDatachargeViewModel = PortalDatachargeWorker.CreateViewModel(Me.GetQolmsYappliModel(), fromPageNoType)
        Dim path As String = Me.HttpContext.Server.MapPath("~/App_Data/DatachargeDescription.txt")
        Dim str As String = String.Empty
        If Not String.IsNullOrWhiteSpace(path) AndAlso IO.File.Exists(path) Then
            str = IO.File.ReadAllText(path)
        End If
        viewModel.Description = str

        Return View(viewModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function DatachargeResult(capacity As Integer) As JsonResult
        Try
            'データチャージ呼び出す
            If PortalDatachargeWorker.Charge(Me.GetQolmsYappliModel(), capacity) Then
                Return New PortalDatachargeJsonResult() With {
                 .Message = HttpUtility.HtmlEncode("チャージが完了しました。"),
                 .IsSuccess = Boolean.TrueString}.ToJsonResult()
            Else
                'ここに返ってくることはないはず
                Return New PortalDatachargeJsonResult() With {
                 .Message = HttpUtility.HtmlEncode("チャージが失敗しました。"),
                 .IsSuccess = Boolean.FalseString}.ToJsonResult()
            End If

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))
            Return New PortalDatachargeJsonResult() With {
              .Message = HttpUtility.HtmlEncode("チャージが失敗しました。"),
              .IsSuccess = Boolean.FalseString}.ToJsonResult()
        End Try

    End Function

#End Region

#Region "「ポイント交換」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function PointExchange(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim viewModel As PortalPointExchangeViewModel = PortalPointExchangeWorker.CreateViewModel(Me.GetQolmsYappliModel(), fromPageNoType)
        Dim path As String = Me.HttpContext.Server.MapPath("~/App_Data/PointExchangeDescription.txt")
        Dim str As String = String.Empty
        If Not String.IsNullOrWhiteSpace(path) AndAlso IO.File.Exists(path) Then
            str = IO.File.ReadAllText(path)
        End If
        viewModel.Description = str
        'Return View(PortalPointExchangeWorker.CreateViewModel(Me.GetQolmsYappliModel))
        'Me.TempData("IsFinish") = False
        Return View(viewModel)
    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function PointExchangeResult(couponType As Byte) As ActionResult
        Try
            'データチャージ呼び出す
            If PortalPointExchangeWorker.Exchange(Me.GetQolmsYappliModel(), couponType) Then
                Me.TempData("IsFinish") = True

                Return New PortalDatachargeJsonResult() With {
               .Message = HttpUtility.HtmlEncode("ポイント交換が完了しました。"),
               .IsSuccess = Boolean.TrueString}.ToJsonResult()
            Else
                Return New PortalDatachargeJsonResult() With {
               .Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
               .IsSuccess = Boolean.FalseString}.ToJsonResult()
            End If

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))
            Return New PortalDatachargeJsonResult() With {
                .Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
                .IsSuccess = Boolean.FalseString}.ToJsonResult()
        End Try

    End Function

#End Region

#Region "「Auポイント交換」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AuPoint(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Return View(AuWalletPointExchangeWorker.CreateViewModel(Me.GetQolmsYappliModel(), fromPageNoType))
    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AuPointResult(itemid As Integer) As ActionResult
        Try
            'データチャージ呼び出す
            If AuWalletPointExchangeWorker.Exchange(Me.GetQolmsYappliModel(), itemid) Then
                Me.TempData("IsFinish") = True

                Return New PortalAuWalletJsonResult() With {
               .Message = HttpUtility.HtmlEncode("Pontaポイント交換が成功しました。"),
               .IsSuccess = Boolean.TrueString}.ToJsonResult()
            Else
                Return New PortalAuWalletJsonResult() With {
               .Message = HttpUtility.HtmlEncode("Pontaポイント交換が失敗しました。"),
               .IsSuccess = Boolean.FalseString}.ToJsonResult()
            End If

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))
            Return New PortalAuWalletJsonResult() With {
                .Message = HttpUtility.HtmlEncode("Pontaポイント交換が失敗しました。"),
                .IsSuccess = Boolean.FalseString}.ToJsonResult()
        End Try

    End Function

#End Region

#Region "「Amazonポイント交換」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AmazonPoint(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Return View(PortalAmazonGiftCardWorker.CreateViewModel(Me.GetQolmsYappliModel(), fromPageNoType))
    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AmazonPointResult(itemid As Byte) As ActionResult
        Try
            'データチャージ呼び出す
            If PortalAmazonGiftCardWorker.Exchange(Me.GetQolmsYappliModel(), itemid) Then
                Me.TempData("IsFinish") = True

                Return New PortalAmazonGiftCardJsonResult() With {
               .Message = HttpUtility.HtmlEncode("Amazonギフト券の交換が完了しました。"),
               .IsSuccess = Boolean.TrueString}.ToJsonResult()
            Else
                Return New PortalAmazonGiftCardJsonResult() With {
               .Message = HttpUtility.HtmlEncode("Amazonギフト券の交換が失敗しました。"),
               .IsSuccess = Boolean.FalseString}.ToJsonResult()
            End If

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))

            Return New PortalAmazonGiftCardJsonResult() With {
                .Message = HttpUtility.HtmlEncode("Amazonギフト券の交換が失敗しました。"),
                .IsSuccess = Boolean.FalseString}.ToJsonResult()
        End Try

    End Function

#End Region

#Region "「JOTO ポイント履歴」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function History(year As Nullable(Of Integer), month As Nullable(Of Integer), fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        If year.HasValue AndAlso month.HasValue Then
            Return View(PortalHistoryWorker.CreateViewModel(Me.GetQolmsYappliModel(), year.Value, month.Value, fromPageNoType))
        Else
            ' ビューを返却
            With Date.Now
                Return View(PortalHistoryWorker.CreateViewModel(Me.GetQolmsYappliModel(), .Year, .Month, fromPageNoType))
            End With
        End If
    End Function

    <HttpPost()>
    <QyAuthorize()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function HistoryResult(year As Integer, month As Integer, fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoValue As Byte = Byte.MinValue
        If fromPageNo.HasValue Then
            fromPageNoValue = fromPageNo.Value
        End If

        ' ビューを返却
        'Return View("History", PortalHistoryWorker.CreateViewModel(Me.GetQolmsYappliModel(), year, month))
        Return RedirectToAction("History", "Portal", New With {.year = year, .month = month, .fromPageNo = fromPageNoValue})

    End Function

#End Region

#Region "「連携設定」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ConnectionSetting(fromPageNo As Nullable(Of Byte), tabNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        Dim tabNoType As Byte = Byte.MinValue

        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)
        If tabNo.HasValue AndAlso tabNo.Value > 0 AndAlso tabNo.Value <= 4 Then tabNoType = tabNo.Value

        Return View(PortalConnectionSettiongWorker.CreateViewModel(Me.GetQolmsYappliModel, fromPageNoType, tabNoType))

    End Function


#End Region

#Region "「タニタ連携」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TanitaConnection() As ActionResult

        Return View(PortalTanitaConnectionWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyAjaxOnly()>
    <QyActionMethodSelector("Register")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TanitaConnectionResult(model As PortalTanitaConnectionInputModel, alkooCancelFlag As Boolean) As ActionResult

        '更新のフローと画面反映処理とかエラー処理とか
        If Me.ModelState.IsValid Then

            Dim messageAlkoo As String = String.Empty
            If alkooCancelFlag Then
                If Not PortalAlkooConnectionWorker.Cancel(Me.GetQolmsYappliModel(), messageAlkoo) Then
                    Return New PortalTanitaConnectionJsonResult() With {
                        .IsSuccess = Boolean.FalseString,
                        .Message = HttpUtility.HtmlEncode("Alkoo連携の解除に失敗しました。")
                    }.ToJsonResult()
                End If
            End If

            Dim errorMessage As New Dictionary(Of String, String)()

            If ModelState.IsValid Then
                Dim message As String = String.Empty
                If PortalTanitaConnectionWorker.ConnectionRegister(Me.GetQolmsYappliModel(), model, message) Then

                    Return New PortalTanitaConnectionJsonResult() With {
                        .IsSuccess = Boolean.TrueString,
                        .Message = String.Empty
                    }.ToJsonResult()
                Else
                    Return New PortalTanitaConnectionJsonResult() With {
                        .IsSuccess = Boolean.FalseString,
                        .Message = HttpUtility.HtmlEncode("登録に失敗しました。")
                    }.ToJsonResult()
                End If
            End If

            Return New PortalTanitaConnectionJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Message = HttpUtility.HtmlEncode("IDまたはパスワードが不正です。")
                }.ToJsonResult()
        Else

            Dim errorMessage As String = String.Empty

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage += e.ErrorMessage
                Next
            Next

            Return New PortalTanitaConnectionJsonResult() With {
                .IsSuccess = Boolean.FalseString,
                .Message = HttpUtility.HtmlEncode(errorMessage)
            }.ToJsonResult()
        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Update")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TanitaConnectionResult(data As String, checked As Boolean, alkooCancelFlag As Boolean) As JsonResult

        'ALKOO
        Dim messageAlkoo As String = String.Empty
        If alkooCancelFlag Then
            If Not PortalAlkooConnectionWorker.Cancel(Me.GetQolmsYappliModel(), messageAlkoo) Then
                Return New PortalTanitaConnectionJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Message = HttpUtility.HtmlEncode("Alkoo連携の解除に失敗しました。")
                }.ToJsonResult()
            End If
        End If

        Try
            Dim devices As List(Of Byte) = PortalTanitaConnectionWorker.DeviceRegister(Me.GetQolmsYappliModel(), data, checked)
            Return New PortalTanitaConnectionJsonResult() With {
                .IsSuccess = Boolean.TrueString,
                .Devises = devices,
                .Message = String.Empty
            }.ToJsonResult()
        Catch ex As Exception
            'アクセスログ
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))

            Return New PortalTanitaConnectionJsonResult() With {
                .IsSuccess = Boolean.FalseString,
                .Message = HttpUtility.HtmlEncode("更新に失敗しました")
            }.ToJsonResult()
        End Try

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Cancel")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function TanitaConnectionResult() As ActionResult

        Dim message As String = String.Empty
        If PortalTanitaConnectionWorker.Cancel(Me.GetQolmsYappliModel(), message) Then
            Return New PortalTanitaConnectionJsonResult() With {
                .IsSuccess = Boolean.TrueString,
                .Message = String.Empty
            }.ToJsonResult()
        Else
            Return New PortalTanitaConnectionJsonResult() With {
                .IsSuccess = Boolean.FalseString,
                .Message = HttpUtility.HtmlEncode("連携解除に失敗しました。")
            }.ToJsonResult()
        End If

    End Function
#End Region

#Region "「ALKOO」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AlkooConnection() As ActionResult

        Return View(PortalAlkooConnectionWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyAjaxOnly()>
    <QyActionMethodSelector("Register")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AlkooConnectionResult(tanitaCancelFlag As Boolean) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim message As String = String.Empty
        If tanitaCancelFlag Then
            Try
                If PortalTanitaConnectionWorker.DeviceRegister(mainModel, "Pedometer", False).Contains(DirectCast(QolmsKaradaKaruteApiCoreV1.QsKaradaKaruteApiDeviceTypeEnum.Pedometer, Byte)) Then
                    '含まれている場合は解除失敗
                    Return New PortalAlkooConnectionJsonResult() With {
                        .IsSuccess = Boolean.FalseString,
                        .Message = HttpUtility.HtmlEncode("タニタ連携解除に失敗しました。")
                    }.ToJsonResult()
                End If
            Catch ex As Exception
                Return New PortalAlkooConnectionJsonResult() With {
                     .IsSuccess = Boolean.FalseString,
                     .Message = HttpUtility.HtmlEncode("タニタ連携解除でエラーが発生しました。")
                 }.ToJsonResult()
            End Try

        End If

        Dim id As String = PortalAlkooConnectionWorker.ConnectionRegister(mainModel)
        Dim url As String = ConfigurationManager.AppSettings("AlkooConnectUrl")

        If String.IsNullOrWhiteSpace(id) Then
            Return New PortalAlkooConnectionJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Message = HttpUtility.HtmlEncode("登録に失敗しました。")
                }.ToJsonResult()
        Else
            '成功
            Return New PortalAlkooConnectionJsonResult() With {
                .IsSuccess = Boolean.TrueString,
                .ID = id,
                .URL = url,
                .Message = HttpUtility.HtmlEncode("登録成功しました。")
            }.ToJsonResult()

        End If

    End Function


    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Cancel")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AlkooConnectionResult() As ActionResult

        Dim message As String = String.Empty
        If PortalAlkooConnectionWorker.Cancel(Me.GetQolmsYappliModel(), message) Then
            Return New PortalAlkooConnectionJsonResult() With {
                .IsSuccess = Boolean.TrueString,
                .Message = String.Empty
            }.ToJsonResult()
        Else
            Return New PortalAlkooConnectionJsonResult() With {
                .IsSuccess = Boolean.FalseString,
                .Message = HttpUtility.HtmlEncode(message)
            }.ToJsonResult()
        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Reconnection")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AlkooConnectionResult(dummy As String) As ActionResult

        Dim message As String = "IDの取得に失敗しました。"
        Dim id As String = PortalAlkooConnectionWorker.GetAlkooId(Me.GetQolmsYappliModel())
        Dim url As String = ConfigurationManager.AppSettings("AlkooConnectUrl")
        If Not String.IsNullOrWhiteSpace(id) Then
            Return New PortalAlkooConnectionJsonResult() With {
                .IsSuccess = Boolean.TrueString,
                .ID = id,
                .URL = url,
                .Message = String.Empty
            }.ToJsonResult()
        Else
            Return New PortalAlkooConnectionJsonResult() With {
                .IsSuccess = Boolean.FalseString,
                .Message = HttpUtility.HtmlEncode(message)
            }.ToJsonResult()
        End If

    End Function

#End Region

#Region "「企業連携」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnection(fromPageNo As Nullable(Of Byte), Optional LinkageSystemNo As Integer = Integer.MinValue) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        If LinkageSystemNo > 0 Then
            Dim viewModel As PortalCompanyConnectionViewModel = PortalCompanyConnectionWorker.CreateViewModel(Me.GetQolmsYappliModel(), LinkageSystemNo, fromPageNoType)
            If Not viewModel Is Nothing Then
                Return View(viewModel)

            End If
        End If

        Return RedirectToAction("ConnectionSetting", New With {.fromPageNo = fromPageNo, .tabNo = 2})

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Delete")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnectionResult(LinkageSystemNo As Integer) As ActionResult

        Dim errorMessage As String = String.Empty
        If LinkageSystemNo > 0 Then

            If PortalCompanyConnectionWorker.Delete(Me.GetQolmsYappliModel(), LinkageSystemNo, errorMessage) Then

                Return New PortalMedicineConnectionJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()

            End If

        End If

        Return New PortalMedicineConnectionJsonResult() With {
            .Massage = "",
            .IsSuccess = Boolean.FalseString
        }.ToJsonResult()

    End Function

    '<HttpPost()>
    '<QyAuthorize()>
    '<QyActionMethodSelector("Edit")>
    '<ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    '<QyApiAuthorize()>
    '<QyLogging()>
    'Public Function CompanyConnectionResult(LinkageSystemNo As Integer, dummy As String) As ActionResult

    '    Dim errorMessage As String = String.Empty
    '    If LinkageSystemNo > 0 Then

    '        'Edit画面遷移
    '        Return View("CompanyConnectionEdit")
    '    End If

    'End Function

#End Region

#Region "「企業連携登録」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnectionRequest(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As PortalCompanyConnectionRequestInputModel = PortalCompanyConnectionRequestWorker.CreateViewModel(mainModel, fromPageNoType)

        'inputModelのキャッシュ（メールアドレスの変更判定のため）
        mainModel.SetInputModelCache(viewModel)
        Return View(viewModel)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Request")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnectionRequestResult(model As PortalCompanyConnectionRequestInputModel) As ActionResult

        Dim message As String = String.Empty
        Dim linkageSystemNo As Integer = Integer.MinValue
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel
        Dim auPaymentModel As PaymentInf = PremiumWorker.GetPremiumRecord(Me.GetQolmsYappliModel)

        If Me.ModelState.IsValid Then
            Select Case PortalUnsubscribeWorker.PremiumCancel(mainModel, auPaymentModel)

                Case 1, 2 ' プレミアム会員解約成功、解約済み
                    'プレミアム会員
                    Dim premiumMessage As String = If(mainModel Is Nothing, String.Empty, String.Format("AccountKey={0}：Name={1}：{2}", mainModel.AuthorAccount.AccountKey, mainModel.AuthorAccount.Name, "プレミアム会員を解約しました。"))
                    AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Delete, premiumMessage)

                    If PortalCompanyConnectionRequestWorker.Request(mainModel, model, linkageSystemNo, message) Then

                        Return New PortalCompanyConnectionRequestJsonResult() With {.IsSuccess = Boolean.TrueString,
                                                                                  .LinkageSystemNo = linkageSystemNo.ToString(),
                                                                                    .Massages = New Dictionary(Of String, String)()
                                                                                   }.ToJsonResult()

                    End If

                Case Else
                    message = HttpUtility.HtmlEncode("課金登録解除に失敗しました。サポート窓口へご連絡ください。")

            End Select
        End If

        ' 検証に失敗
        Dim errorMessage As New Dictionary(Of String, String)()

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode("," + e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        If String.IsNullOrWhiteSpace(message) Then
            errorMessage.Add("summary", "入力内容を確認してください。")

        Else
            errorMessage.Add("summary", message)
        End If


        Return New PortalCompanyConnectionRequestJsonResult() With {.IsSuccess = Boolean.FalseString,
                                                            .Massages = errorMessage
                                                           }.ToJsonResult()

    End Function


    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("IsIdentityChecked")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnectionRequestResult(model As PortalCompanyConnectionRequestInputModel, dummy As String) As ActionResult

        Dim message As String = String.Empty
        Dim errorMessage As New Dictionary(Of String, String)()

        If Me.ModelState.IsValid Then

            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
            Dim account As AuthorAccountItem = mainModel.AuthorAccount

            Dim birthday As Date = Date.MinValue
            Try
                birthday = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))
            Catch ex As Exception

            End Try

            Dim cache As PortalCompanyConnectionRequestInputModel = mainModel.GetInputModelCache(Of PortalCompanyConnectionRequestInputModel)()

            '本人確認
            If account.FamilyName = model.FamilyName AndAlso
                account.GivenName = model.GivenName AndAlso
                account.FamilyKanaName = model.FamilyKanaName AndAlso
                account.GivenKanaName = model.GivenKanaName AndAlso
                account.Birthday = birthday AndAlso
                account.SexType = model.SexType Then

                '全部一致(そのまま更新)
                Return New PortalCompanyConnectionRequestJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()

            Else

                If account.Birthday = birthday AndAlso account.SexType = model.SexType Then
                    errorMessage.Add("summary", "入力された個人情報が登録と一致しませんでした。個人情報更新しますか？")
                    '成功で返却してメッセージで確認

                    Return New PortalCompanyConnectionRequestJsonResult() With {
                        .IsSuccess = Boolean.TrueString,
                        .Massages = errorMessage
                    }.ToJsonResult()

                Else
                    '//名前しか変更の許可をしない
                    '//性別、生年月日の変更があった場合はエラーとして処理
                    errorMessage.Add("summary", "性別、生年月日の変更はできません。")

                    Return New PortalCompanyConnectionRequestJsonResult() With {
                        .IsSuccess = Boolean.FalseString,
                        .Massages = errorMessage
                    }.ToJsonResult()

                End If

            End If
        End If

        ' 検証に失敗

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode("," + e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        If String.IsNullOrWhiteSpace(message) Then
            errorMessage.Add("summary", "入力内容を確認してください。")
        End If

        Return New PortalCompanyConnectionRequestJsonResult() With {.IsSuccess = Boolean.FalseString,
                                                            .Massages = errorMessage
                                                           }.ToJsonResult()

    End Function

#End Region

#Region "「企業連携編集」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnectionEdit(fromPageNo As Nullable(Of Byte), Optional LinkageSystemNo As Integer = Integer.MinValue) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        If LinkageSystemNo > 0 Then

            Return View(PortalCompanyConnectionEditWorker.CreateViewModel(Me.GetQolmsYappliModel(), LinkageSystemNo, fromPageNoType))
        End If

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnectionEditResult(model As PortalCompanyConnectionEditInputModel) As ActionResult

        Dim message As String = String.Empty

        If Me.ModelState.IsValid Then
            Dim linkageSystemNo As Integer = Integer.MinValue

            If PortalCompanyConnectionEditWorker.Edit(Me.GetQolmsYappliModel(), model, linkageSystemNo, message) Then

                Return New PortalCompanyConnectionEditJsonResult() With {.IsSuccess = Boolean.TrueString,
                                                    .LinkageSystemNo = linkageSystemNo.ToString()
                                           }.ToJsonResult()

            End If

        End If

        ' 検証に失敗
        Dim errorMessage As New Dictionary(Of String, String)()

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode("," + e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        If String.IsNullOrWhiteSpace(message) Then
            errorMessage.Add("summary", "入力内容を確認してください。")

        Else
            errorMessage.Add("summary", message)
        End If


        Return New PortalCompanyConnectionEditJsonResult() With {.IsSuccess = Boolean.FalseString,
                                            .Massages = errorMessage
                                           }.ToJsonResult()
    End Function

#End Region

#Region "「企業連携メニュー」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CompanyConnectionHome(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel
        If mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.Business Or mainModel.AuthorAccount.MembershipType = QyMemberShipTypeEnum.BusinessFree Then
            Dim viewModel As New PortalCompanyConnectionHomeViewModel() With {
                .FromPageNoType = fromPageNoType
            }

            Return View(viewModel)

        End If

        Return RedirectToAction("ConnectionSetting", New With {.fromPageNo = fromPageNo, .tabNo = 2})


    End Function

#End Region

#Region "「病院連携」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HospitalConnection(fromPageNo As Nullable(Of Byte), Optional LinkageSystemNo As Integer = Integer.MinValue) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        If LinkageSystemNo < 0 Then
            Return RedirectToAction("ConnectionSetting")
        End If

        Return View(PortalHospitalConnectionWorker.CreateViewModel(Me.GetQolmsYappliModel, LinkageSystemNo, fromPageNoType))

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Cancel")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HospitalConnectionResult(LinkageSystemNo As Integer) As ActionResult

        Dim message As String = String.Empty

        If Me.ModelState.IsValid Then

            If PortalHospitalConnectionWorker.Cancel(Me.GetQolmsYappliModel, LinkageSystemNo) Then

                Return New PortalHospitalConnectionJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()

            End If

        End If

        Return New PortalHospitalConnectionJsonResult() With {
            .Massage = "",
            .IsSuccess = Boolean.FalseString
        }.ToJsonResult()

    End Function

#End Region

#Region "「病院連携登録」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HospitalConnectionRequest(fromPageNo As Nullable(Of Byte), Optional LinkageSystemNo As Integer = Integer.MinValue) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel
        Dim viewModell As PortalHospitalConnectionRequestInputModel = PortalHospitalConnectionRequestWorker.CreateViewModel(mainModel, LinkageSystemNo, fromPageNoType)

        'inputModelのキャッシュ（メールアドレスの変更判定のため）
        mainModel.SetInputModelCache(viewModell)
        Return View(viewModell)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Request")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HospitalConnectionRequestResult(model As PortalHospitalConnectionRequestInputModel) As ActionResult

        Dim message As String = String.Empty

        If Me.ModelState.IsValid Then
            Try
                If PortalHospitalConnectionRequestWorker.Request(Me.GetQolmsYappliModel, model, message) AndAlso String.IsNullOrWhiteSpace(message) Then

                    Return New PortalHospitalConnectionRequestJsonResult() With {
                        .IsSuccess = Boolean.TrueString
                    }.ToJsonResult()

                Else
                    Dim summary As New Dictionary(Of String, String)()

                    If Not String.IsNullOrWhiteSpace(message) Then
                        summary.Add("summary", message)
                    Else
                        summary.Add("summary", "登録に失敗しました。")
                    End If

                    Return New PortalHospitalConnectionRequestJsonResult() With {
                        .Massages = summary,
                        .IsSuccess = Boolean.FalseString
                    }.ToJsonResult()
                End If
            Catch ex As Exception

                AccessLogWorker.WriteErrorLog(Me.HttpContext, "", ex.Message)

                Dim summary As New Dictionary(Of String, String)()
                summary.Add("summary", "サーバーでエラーが発生しました。")

                Return New PortalHospitalConnectionRequestJsonResult() With {
                        .Massages = summary,
                        .IsSuccess = Boolean.FalseString
                    }.ToJsonResult()

            End Try



        End If

        ' 検証に失敗
        Dim errorMessage As New Dictionary(Of String, String)()

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode(e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        Return New PortalHospitalConnectionRequestJsonResult() With {
            .Massages = errorMessage,
            .IsSuccess = Boolean.FalseString
        }.ToJsonResult()


    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("IsIdentityChecked")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function HospitalConnectionRequestResult(model As PortalHospitalConnectionRequestInputModel, dummy As String) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim account As AuthorAccountItem = mainModel.AuthorAccount

        Dim birthday As Date = Date.MinValue
        Try
            birthday = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))
        Catch ex As Exception

        End Try

        Dim cache As PortalHospitalConnectionRequestInputModel = mainModel.GetInputModelCache(Of PortalHospitalConnectionRequestInputModel)()

        Dim mailAddress As String = String.Empty
        If cache IsNot Nothing Then
            mailAddress = cache.MailAddress
        End If

        '本人確認
        If account.FamilyName = model.FamilyName AndAlso
            account.GivenName = model.GivenName AndAlso
            account.FamilyKanaName = model.FamilyKanaName AndAlso
            account.GivenKanaName = model.GivenKanaName AndAlso
            account.Birthday = birthday AndAlso
            account.SexType = model.SexType AndAlso
            mailAddress = model.MailAddress Then

            '全部一致(そのまま更新)
            Return New PortalHospitalConnectionRequestJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        Else
            Dim errorMessage As New Dictionary(Of String, String)()

            If account.Birthday = birthday AndAlso account.SexType = model.SexType Then
                errorMessage.Add("summary", "入力された個人情報が登録と一致しませんでした。個人情報更新しますか？")
                '成功で返却してメッセージで確認

                Return New PortalHospitalConnectionRequestJsonResult() With {
                    .IsSuccess = Boolean.TrueString,
                    .Massages = errorMessage
                }.ToJsonResult()

            Else
                '//名前しか変更の許可をしない
                '//性別、生年月日の変更があった場合はエラーとして処理
                errorMessage.Add("summary", "性別、生年月日の変更はできません。")

                Return New PortalHospitalConnectionRequestJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Massages = errorMessage
                }.ToJsonResult()

            End If

        End If
    End Function

#End Region

#Region "「チャレンジ一覧」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Challenge() As ActionResult

        Return View(PortalChallengeWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function

    '<HttpPost()>
    '<QyAuthorize()>
    '<QyApiAuthorize()>
    '<QyLogging()>
    'Public Function ChallengeRersult() As ActionResult

    '    Return View()

    'End Function

#End Region

#Region "「チャレンジエントリー」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEntry(key As String) As ActionResult

        Dim challengekey As Guid = Guid.Empty
        If Not String.IsNullOrWhiteSpace(key) Then

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                    challengekey = Guid.Parse(crypt.DecryptString(key))

                End Using
            Catch ex As Exception
                Me.TempData("Nodata") = True
                Return RedirectToAction("Challenge")

            End Try

            If challengekey <> Guid.Empty Then

                Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                'ViewModelを取得
                Dim model As PortalChallengeEntryInputModel = PortalChallengeEntryWorker.CreateViewModel(mainModel, challengekey)
                'キャッシュ
                mainModel.SetInputModelCache(model)

                'エントリー済みならリダイレクト
                If model.ChallengeItem.UserStartDate > Date.MinValue Then
                    Return RedirectToAction("ChallengeDetail", New With {.key = key})
                End If

                Return View(model)

            End If

        End If

        'Me.TempData("Nodata") = True
        Return RedirectToAction("Challenge")

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Entry")>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEntryResult() As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim preaentModel As PortalChallengeEntryInputModel = mainModel.GetInputModelCache(Of PortalChallengeEntryInputModel)()

        Dim model As PortalChallengeEntryAgreeResultPartialViewModel = PortalChallengeEntryWorker.CreateAgreePartialViewModel(mainModel, preaentModel)

        If String.IsNullOrWhiteSpace(model.Terms) Then
            'todo:規約がない場合(今回はある場合しかないのでスルー？)
        Else
            'モデルの更新
            preaentModel.AgreePartialViewModel = model
            'モデルをキャッシュ
            mainModel.SetInputModelCache(preaentModel)
        End If

        Return PartialView("_PortalChallengeEntryAgreeResultPartialView", model)

    End Function


    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("Agree")>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEntryResult(Agreement As Boolean) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        '同意チェック
        If Agreement Then
            'OK
            Dim model As PortalChallengeEntryInputModel = mainModel.GetInputModelCache(Of PortalChallengeEntryInputModel)()
            'Dim  As PortalChallengeEntryInputModel = Me.GetPageViewModel(Of PortalChallengeEntryInputModel)()
            model.AgreeChecked = True

            '参加資格確認に必要な条件を取得
            Dim partialViewModel As PortalChallengeEntryPassResultPartialViewModel = PortalChallengeEntryWorker.CreatePartialPassViewModel(mainModel, model)

            'モデルの更新
            model.PassPartialViewModel = partialViewModel

            PortalChallengeEntryWorker.GetInitialValue(mainModel, model)

            'モデルをキャッシュ
            mainModel.SetInputModelCache(model)

            '参加資格確認へ
            Return PartialView("_PortalChallengeEntryPassResultPartialView", partialViewModel)

        Else
            '同意を確認してください
        End If

        Return View()


    End Function


    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("Pass")>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEntryResult(pass As String, values As Dictionary(Of String, String), checked As Byte) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim preaentModel As PortalChallengeEntryInputModel = mainModel.GetInputModelCache(Of PortalChallengeEntryInputModel)()

        preaentModel.UpdateByInput(pass, values, checked)

        Dim message As String = String.Empty
        Dim errorMessage As New Dictionary(Of String, String)()

        If preaentModel.IsValid(errorMessage) Then

            ''エントリーコード
            'If preaentModel.PassPartialViewModel.PassCodeVisible Then
            '    If preaentModel.PassPartialViewModel.PassCodes.IndexOf(model.Pass) >= 0 Then
            '        'パスOK
            '    Else
            '        errorMessage.Add("Pass", "エントリーコードが間違っています。")
            '    End If

            'End If

            '入力項目
            'For Each item As KeyValuePair(Of String, String) In preaentModel.PassPartialViewModel.RequiredN

            '    If values.ContainsKey(item.Key) Then
            '        If PortalChallengeEntryWorker.RequiredItemValidate(item.Key, values(item.Key), message) Then

            '            preaentModel.RequiredN.Add(item.Key, values(item.Key))
            '        Else

            '            errorMessage.AppendLine(message)
            '        End If
            '    Else

            '        errorMessage.AppendLine("必須入力です。")
            '    End If
            'Next

            '参加条件を確認
            If preaentModel.PassPartialViewModel.RequiredN.Count = preaentModel.RequiredN.Count Then

                '入力内容を登録
                If PortalChallengeEntryWorker.Entry(mainModel, preaentModel) Then
                    Dim key As String = String.Empty

                    Try
                        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                            key = crypt.EncryptString(preaentModel.ChallengeItem.Challengekey.ToString())

                        End Using
                    Catch ex As Exception
                    End Try

                    Me.TempData("first") = True

                    '参加資格OKならエントリーを登録して詳細画面へ
                    Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.TrueString, .Key = key}.ToJsonResult()

                End If

            End If

        End If

        ' 検証に失敗
        If errorMessage.Count = 0 Then
            errorMessage.Add("Summary", "登録に失敗しました。")

        End If

        'For Each key As String In Me.ModelState.Keys
        '    For Each e As ModelError In Me.ModelState(key).Errors
        '        If errorMessage.ContainsKey(key) Then
        '            errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode("," + e.ErrorMessage)
        '        Else
        '            errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

        '        End If
        '    Next
        'Next

        Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.FalseString, .Messages = errorMessage}.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("PostCode")>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEntryResult(PostCode As String) As ActionResult

        If Not String.IsNullOrWhiteSpace(PostCode) Then
            '入力検証

            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

            '住所検索
            Dim address As String = PortalChallengeEntryWorker.PostalCodeToAddress(mainModel, PostCode)
            If Not String.IsNullOrWhiteSpace(address) Then

                Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.FalseString, .Address = address}.ToJsonResult()
            End If

        Else

            '入力が空

        End If

        Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.FalseString}.ToJsonResult()

    End Function



#End Region

#Region "「チャレンジ詳細」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeDetail(key As String) As ActionResult

        Dim challengekey As Guid = Guid.Empty
        If Not String.IsNullOrWhiteSpace(key) Then

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                    challengekey = Guid.Parse(crypt.DecryptString(key))

                End Using
            Catch ex As Exception
                'Me.TempData("Nodata") = True
                Return RedirectToAction("Challenge")

            End Try

            If challengekey <> Guid.Empty Then
                Return View(PortalChallengeDetailWorker.CreateViewModel(Me.GetQolmsYappliModel(), challengekey))
            End If

        End If

        'Me.TempData("Nodata") = True
        Return RedirectToAction("Challenge")

    End Function

#End Region

#Region "「チャレンジ編集」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEdit(key As String) As ActionResult

        Dim challengekey As Guid = Guid.Empty
        If Not String.IsNullOrWhiteSpace(key) Then

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                    challengekey = Guid.Parse(crypt.DecryptString(key))

                End Using
            Catch ex As Exception
                'Me.TempData("Nodata") = True
                Return RedirectToAction("Challenge")

            End Try

            If challengekey <> Guid.Empty Then

                Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                Dim model As PortalChallengeEditInputModel = PortalChallengeEditWorker.CreateViewModel(mainModel, challengekey)

                'モデルをキャッシュ
                mainModel.SetInputModelCache(model)

                Return View(model)

            End If

        End If

        Return RedirectToAction("Challenge")

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("Edit")>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEditResult(values As Dictionary(Of String, String), checked As Byte) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim preaentModel As PortalChallengeEditInputModel = mainModel.GetInputModelCache(Of PortalChallengeEditInputModel)()

        preaentModel.UpdateByInput(values, checked)

        Dim message As String = String.Empty
        Dim errorMessage As New Dictionary(Of String, String)()

        If preaentModel.IsValid(errorMessage) Then

            '入力内容を登録
            If PortalChallengeEditWorker.Edit(mainModel, preaentModel) Then
                Dim key As String = String.Empty

                Try
                    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                        key = crypt.EncryptString(preaentModel.ChallengeItem.Challengekey.ToString())

                    End Using
                Catch ex As Exception
                End Try

                Me.TempData("first") = True

                '参加資格OKならエントリーを登録して詳細画面へ
                Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.TrueString, .Key = key}.ToJsonResult()

            End If
        End If

        ' 検証に失敗
        If errorMessage.Count = 0 Then
            errorMessage.Add("Summary", "登録に失敗しました。")

        End If

        Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.FalseString, .Messages = errorMessage}.ToJsonResult()

    End Function


    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("Cancel")>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeEditResult() As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim preaentModel As PortalChallengeEditInputModel = mainModel.GetInputModelCache(Of PortalChallengeEditInputModel)()

        Dim errorMessage As New Dictionary(Of String, String)()

        '入力内容を登録
        If PortalChallengeEditWorker.Cancel(mainModel, preaentModel.ChallengeItem.Challengekey) Then

            Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()

        End If

        ' 検証に失敗
        If errorMessage.Count = 0 Then
            errorMessage.Add("Summary", "登録に失敗しました。")

        End If

        Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.FalseString, .Messages = errorMessage}.ToJsonResult()

    End Function

#End Region

#Region "「コラム一覧」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeColumn(key As String, Optional targetday As Integer = Integer.MinValue, Optional fromPageNo As Byte = Byte.MinValue) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim challengekey As Guid = Guid.Empty
        If Not String.IsNullOrWhiteSpace(key) Then

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                    challengekey = Guid.Parse(crypt.DecryptString(key))

                End Using
            Catch ex As Exception
                'Me.TempData("Nodata") = True
                Return RedirectToAction("Challenge")

            End Try

            If challengekey <> Guid.Empty Then

                Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                Dim model As PortalChallengeColumnViewModel = PortalChallengeColumnWorker.CreateViewModel(mainModel, challengekey, fromPageNoType)

                'targetdayは表示する経過日数（多分ある日しか来ないけど確認はする）
                If targetday > 0 Then

                    Dim target As List(Of ChallengeColumnItem) = model.ChallengeColumnItemN.Where(Function(i) i.Days = targetday).ToList()
                    If target.Any Then model.TargetColumnNo = target.First().ColumnNo

                End If
                mainModel.SetInputModelCache(model)

                Return View(model)
            End If

        End If

        'Me.TempData("Nodata") = True
        Return RedirectToAction("Challenge")

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("Detail")>
    <QyLogging()>
    Public Function ChallengeColumnResult(key As String, no As String) As ActionResult
        'readはほぼダミー

        Dim challengekey As Guid = Guid.Empty
        If Not String.IsNullOrWhiteSpace(key) Then

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                    challengekey = Guid.Parse(crypt.DecryptString(key))

                End Using
            Catch ex As Exception
                'Me.TempData("Nodata") = True
                'Return RedirectToAction("Challenge")

            End Try

            Dim columnNo As Integer = Integer.MinValue
            If challengekey <> Guid.Empty And Integer.TryParse(no, columnNo) Then

                Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

                Dim model As PortalChallengeColumnPartialViewModel = PortalChallengeColumnWorker.GetColumnItem(mainModel, challengekey, columnNo)
                Return PartialView("_PortalChallengeColumnPartialView", model)
            End If

        End If

        Return New PortalChallengeEntryJsonResult() With {.IsSuccess = Boolean.FalseString}.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("Read")>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeColumnResult(key As String, no As String, read As Boolean) As ActionResult

        Dim challengekey As Guid = Guid.Empty
        If Not String.IsNullOrWhiteSpace(key) Then

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                    challengekey = Guid.Parse(crypt.DecryptString(key))

                End Using
            Catch ex As Exception
                'Me.TempData("Nodata") = True
                'Return RedirectToAction("Challenge")
            End Try

            Dim columnNo As Integer = Integer.MinValue
            If challengekey <> Guid.Empty And Integer.TryParse(no, columnNo) Then

                Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

                Dim model As Boolean = PortalChallengeColumnWorker.ReadColumn(mainModel, challengekey, columnNo)
                Return New PortalChallengeColumnJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()
            End If

        End If

        Return New PortalChallengeColumnJsonResult() With {.IsSuccess = Boolean.FalseString}.ToJsonResult()

    End Function

    <HttpGet()>
    <QyApiAuthorize()>
    <QyAuthorize()>
    <QyLogging()>
    Public Function GetColumnImage(reference As String) As ActionResult

        Dim imagekey As Guid = Guid.Empty
        If Not String.IsNullOrWhiteSpace(reference) Then

            Try

                Dim jsonString As String = String.Empty

                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    jsonString = crypt.DecryptString(reference)
                End Using

                Dim parameter As FileStorageReferenceJsonParameter = QyJsonParameterBase.FromJsonString(Of FileStorageReferenceJsonParameter)(jsonString)
                imagekey = parameter.FileKey

            Catch ex As Exception
                'Me.TempData("Nodata") = True
                'Return RedirectToAction("Challenge")
            End Try

            Dim data As Byte()
            Dim contentType As String = String.Empty

            If imagekey <> Guid.Empty AndAlso PortalChallengeColumnWorker.GetImage(Me.GetQolmsYappliModel(), imagekey, contentType, data) Then

                Return New FileContentResult(data, contentType)

            End If

        End If

        Return New EmptyResult()

    End Function


#End Region

#Region "「チャレンジアンケート完了」画面"

    ''' <summary>
    ''' チャレンジアンケート完了画面を取得します
    ''' </summary>
    ''' <param name="key">アンケート画面からキーを指定される</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ChallengeCompleted(key As String) As ActionResult

        If PortalChallengeCompletedWorker.Complete(Me.GetQolmsYappliModel(), key) Then
            Return View()
        Else
            'false
            Return View()
        End If

    End Function

#End Region

#Region "「薬局連携」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicineConnection(LinkageSystemNo As Nullable(Of Integer), facilitykey As Nullable(Of Guid), fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        'prescription 処方薬
        'pharmacy 薬局
        If LinkageSystemNo.HasValue AndAlso LinkageSystemNo.Value > 0 _
            AndAlso facilitykey.HasValue AndAlso facilitykey.Value <> Guid.Empty Then

            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
            Return View(PortalMedicineConnectionWorker.CreateViewModel(mainModel, LinkageSystemNo.Value, facilitykey.Value, fromPageNoType))

        Else

            Return RedirectToAction("ConnectionSetting", New With {.fromPageNo = fromPageNo, .tabNo = 4})

        End If

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Cancel")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicineConnectionResult(LinkageSystemNo As Nullable(Of Integer), facilitykey As Nullable(Of Guid)) As ActionResult

        Dim message As String = String.Empty
        If LinkageSystemNo.HasValue AndAlso LinkageSystemNo.Value > 0 _
                AndAlso facilitykey.HasValue AndAlso facilitykey.Value <> Guid.Empty Then

            If PortalMedicineConnectionWorker.Cancel(Me.GetQolmsYappliModel(), LinkageSystemNo.Value, facilitykey.Value, message) Then

                Return New PortalMedicineConnectionJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()

            End If

        End If

        Return New PortalMedicineConnectionJsonResult() With {
            .Massage = message,
            .IsSuccess = Boolean.FalseString
        }.ToJsonResult()

    End Function

#End Region

#Region "「薬局連携リクエスト」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicineConnectionRequest(linkageSystemNo As Nullable(Of Integer), facilitykey As Nullable(Of Guid), fromPageNo As Nullable(Of Integer)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        If linkageSystemNo.HasValue AndAlso linkageSystemNo.Value > 0 _
            AndAlso facilitykey.HasValue AndAlso facilitykey.Value <> Guid.Empty Then

            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

            Dim viewModel As PortalMedicineConnectionRequestInputModel = PortalMedicineConnectionRequestWorker.CreateViewModel(mainModel, linkageSystemNo.Value, facilitykey.Value, fromPageNoType)
            If viewModel.LinkageSystemNo > 0 AndAlso viewModel.FacilityKey <> Guid.Empty Then
                Return View(viewModel)
            End If
        End If

        'ConnectionSettingにリダイレクトさせる
        Return RedirectToAction("ConnectionSetting", New With {.fromPageNo = fromPageNo, .tabNo = 4})

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Request")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicineConnectionRequestResult(model As PortalMedicineConnectionRequestInputModel) As ActionResult

        Dim message As String = String.Empty

        If Me.ModelState.IsValid Then

            If PortalMedicineConnectionRequestWorker.Request(Me.GetQolmsYappliModel, model, message) Then

                Return New PortalMedicineConnectionRequestJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()

            Else
                Dim summary As New Dictionary(Of String, String)()
                summary.Add("summary", String.Format("登録に失敗しました。({0})", message))

                Return New PortalMedicineConnectionRequestJsonResult() With {
                    .Massages = summary,
                    .IsSuccess = Boolean.FalseString
                }.ToJsonResult()
            End If

        End If

        ' 検証に失敗
        Dim errorMessage As New Dictionary(Of String, String)()

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode("," + e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        Return New PortalMedicineConnectionRequestJsonResult() With {
            .Massages = errorMessage,
            .IsSuccess = Boolean.FalseString
        }.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("IsIdentityChecked")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicineConnectionRequestResult(model As PortalMedicineConnectionRequestInputModel, dummy As String) As ActionResult


        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim account As AuthorAccountItem = mainModel.AuthorAccount

        Dim birthday As Date = Date.MinValue
        Try
            birthday = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))
        Catch ex As Exception

        End Try

        Dim cache As PortalMedicineConnectionRequestInputModel = mainModel.GetInputModelCache(Of PortalMedicineConnectionRequestInputModel)()

        '本人確認
        If account.FamilyName = model.FamilyName AndAlso
            account.GivenName = model.GivenName AndAlso
            account.Birthday = birthday AndAlso
            account.SexType = model.SexType Then

            '全部一致(そのまま更新)
            Return New PortalMedicineConnectionRequestJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        Else
            Dim errorMessage As New Dictionary(Of String, String)()

            If account.Birthday = birthday AndAlso account.SexType = model.SexType Then
                errorMessage.Add("summary", "入力された個人情報が登録と一致しませんでした。個人情報更新しますか？")
                '成功で返却してメッセージで確認

                Return New PortalMedicineConnectionRequestJsonResult() With {
                    .IsSuccess = Boolean.TrueString,
                    .Massages = errorMessage
                }.ToJsonResult()

            Else
                '//名前しか変更の許可をしない
                '//性別、生年月日の変更があった場合はエラーとして処理
                errorMessage.Add("summary", "性別、生年月日の変更はできません。")

                Return New PortalMedicineConnectionRequestJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Massages = errorMessage
                }.ToJsonResult()

            End If

        End If

    End Function

#End Region

#Region "「連携先薬局選択」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicineConnectionSearch(fromPageNo As Nullable(Of Integer), Optional pageIndex As Integer = Integer.MinValue) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        'Index１ページ目が0

        If pageIndex < 0 Then
            pageIndex = 0
        End If

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        Return View(PortalMedicineConnectionSearchWorker.CreateViewModel(mainModel, pageIndex, fromPageNoType))

    End Function

#End Region

#Region "「連携先薬局規約画面」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicineConnectionAgreement(linkageSystemNo As Nullable(Of Integer), facilitykey As Nullable(Of Guid), fromPageNo As Nullable(Of Integer)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        If linkageSystemNo.HasValue AndAlso linkageSystemNo.Value > 0 _
            AndAlso facilitykey.HasValue AndAlso facilitykey.Value <> Guid.Empty Then

            Dim str As String = String.Empty
            Dim path As String = Me.HttpContext.Server.MapPath("~/App_Data/PharumoTerm.txt")

            If Not String.IsNullOrWhiteSpace(path) AndAlso IO.File.Exists(path) Then
                str = IO.File.ReadAllText(path)
            End If

            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
            Dim viewModel As New PortalMedicineConnectionAgreementViewModel() With {
                .FromPageNoType = fromPageNoType,
                .Facilitykey = facilitykey.Value,
                .LinkageSystemNo = linkageSystemNo.Value,
                .TermsString = str
            }

            Return View(viewModel)
        End If

        'ConnectionSettingにリダイレクトさせる
        Return RedirectToAction("ConnectionSetting", New With {.fromPageNo = fromPageNo, .tabNo = 4})

    End Function

#End Region

    '#Region "「通いの場連携」画面"

    '    <HttpGet()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize()>
    '    <QyLogging()>
    '    Public Function KayoinobaConnection(fromPageNo As Nullable(Of Byte)) As ActionResult

    '        'HOMEと連携設定に戻る

    '        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
    '        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

    '        '連携がなければ同意画面へ遷移
    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim model As PortalKayoinobaConnectionViewModel = PortalKayoinobaConnectionWorker.CreateViewModel(mainModel, fromPageNoType)

    '        If model.KyoinobaConnectedFlag Then
    '            Return View(model)

    '        Else

    '            If fromPageNoType = QyPageNoTypeEnum.PortalHome Then
    '                Return RedirectToAction("Kayoinoba", New With {.fromPageNo = 1})

    '            Else
    '                Return RedirectToAction("Kayoinoba")

    '            End If

    '        End If

    '    End Function


    '    <HttpPost()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize()>
    '    <QyLogging()>
    '    Public Function KayoinobaConnectionResult() As ActionResult

    '        'Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
    '        'If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

    '        '連携がなければ同意画面へ遷移
    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        If PortalKayoinobaConnectionWorker.Cancel(mainModel) Then
    '            Return New PortalKayoinobaJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()

    '        End If

    '        Return New PortalKayoinobaJsonResult() With {.IsSuccess = Boolean.FalseString}.ToJsonResult()

    '    End Function

    '#End Region

    '#Region "「通いの場連携規約、ログイン呼びだし」画面"

    '    <HttpGet()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize()>
    '    <QyLogging()>
    '    Public Function Kayoinoba(fromPageNo As Nullable(Of Byte)) As ActionResult

    '        'HOMEと連携設定に戻る
    '        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
    '        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

    '        Dim model As New PortalKayoinobaInputModel() With {.fromPageNoType = fromPageNoType}
    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        mainModel.SetInputModelCache(model)
    '        '同意文章を取得（どこにおくかは検討。Viewでもいい）
    '        'HOMEか連携設定画面に戻る
    '        Return View(model)

    '    End Function

    '    <HttpPost()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize()>
    '    <QyLogging()>
    '    Public Function KayoinobaResult(model As PortalKayoinobaInputModel) As ActionResult

    '        Dim errorMessage As String = String.Empty
    '        'String.Empty か ７桁（R除いてるので6桁）のコード
    '        Dim code As Integer = Integer.MinValue
    '        If String.IsNullOrWhiteSpace(model.ShopCode) OrElse (model.ShopCode.Length = 6 AndAlso Integer.TryParse(model.ShopCode, code)) Then

    '            '店舗コードを入力できるようにする
    '            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '            Dim cacheModel As PortalKayoinobaInputModel = mainModel.GetInputModelCache(Of PortalKayoinobaInputModel)()
    '            model.FromPageNoType = cacheModel.FromPageNoType
    '            mainModel.SetInputModelCache(model)

    '            Dim scheme As String = HttpContext.Request.Url.Scheme
    '            Dim authority As String = HttpContext.Request.Url.Authority
    '            Dim path As String = HttpRuntime.AppDomainAppVirtualPath

    '            Dim returnUrl As String = String.Format("{0}://{1}{2}/{3}", scheme, authority, path, "Portal/KayoinobaLoginResult")

    '            Dim url As String = KayoinobaWorker.GetLoginUrl(returnUrl)
    '            If Not String.IsNullOrWhiteSpace(url) Then

    '                '同意OKで通いの場APIの結果ログインが返ってきたらログイン画面のURLを渡す
    '                Return New PortalKayoinobaJsonResult() With {.IsSuccess = Boolean.TrueString, .url = url}.ToJsonResult()

    '            Else

    '                errorMessage = "接続できません。時間をおいて試してください。"
    '            End If
    '        Else

    '            errorMessage = "コードの形式が間違っています。"
    '        End If

    '        Return New PortalKayoinobaJsonResult() With {.IsSuccess = Boolean.FalseString, .Message = errorMessage}.ToJsonResult()

    '    End Function

    '    <HttpGet()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize()>
    '    <QyLogging()>
    '    Public Function KayoinobaLoginResult(kayoinoba_code As String) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim cacheModel As PortalKayoinobaInputModel = mainModel.GetInputModelCache(Of PortalKayoinobaInputModel)()

    '        Dim scheme As String = HttpContext.Request.Url.Scheme
    '        Dim authority As String = HttpContext.Request.Url.Authority
    '        Dim path As String = HttpRuntime.AppDomainAppVirtualPath

    '        Dim returnUrl As String = String.Format("{0}://{1}{2}/{3}", scheme, authority, path, "Portal/KayoinobaLoginResult")

    '        Dim errorMessage As String = String.Empty
    '        If KayoinobaWorker.KeyoinobaLogin(kayoinoba_code, returnUrl) AndAlso PortalKayoinobaConnectionWorker.Register(mainModel, kayoinoba_code, cacheModel.ShopCode, errorMessage) Then
    '            If cacheModel.FromPageNoType = QyPageNoTypeEnum.PortalHome Then

    '                Return RedirectToAction("KayoinobaConnection", New With {.fromPageNo = 1}) '戻り先も指定できるようにする

    '            Else

    '                Return RedirectToAction("KayoinobaConnection") '戻り先も指定できるようにする
    '            End If

    '        End If

    '        Me.ViewData("ErrorMessage") = errorMessage
    '        '失敗した場合はどこに戻すか？一旦連携設定画面に飛ばす
    '        Return View("kayoinoba", cacheModel)

    '    End Function



    '#End Region

    '#Region "「チェックリスト」画面"

    '    <HttpGet()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize()>
    '    <QyLogging()>
    '    Public Function KayoinobaCheckList(fromPageNo As Nullable(Of Byte), questionnaireId As Integer) As ActionResult

    '        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
    '        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

    '        '連携がなければ同意画面へ遷移
    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim model As PortalKayoinobaCheckListInputModel = PortalKayoinobaCheckListWorker.CreateViewModel(mainModel, questionnaireId)

    '        model.FromPageNoType = fromPageNoType

    '        If model IsNot Nothing Then

    '            mainModel.SetInputModelCache(model)

    '            Return View(model)
    '        End If

    '        Return RedirectToAction("KayoinobaConnection")

    '    End Function

    '    <HttpPost()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize()>
    '    <ValidateInput(False)>
    '    <QyLogging()>
    '    Public Function KayoinobaCheckListResult(model As PortalKayoinobaCheckListInputModel) As ActionResult

    '        '連携がなければ同意画面へ遷移
    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim cacheModel As PortalKayoinobaCheckListInputModel = mainModel.GetInputModelCache(Of PortalKayoinobaCheckListInputModel)()

    '        model.CheckList = cacheModel.CheckList

    '        Me.ModelState.Clear()

    '        If TryValidateModel(model) AndAlso PortalKayoinobaCheckListWorker.SendAnswer(mainModel, model) Then

    '            Return New PortalKayoinobaCheckListJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()

    '        End If

    '        ' 検証に失敗
    '        Dim errorMessage As New Dictionary(Of String, String)()

    '        For Each key As String In Me.ModelState.Keys
    '            For Each e As ModelError In Me.ModelState(key).Errors
    '                If errorMessage.ContainsKey(key) Then
    '                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode("," + e.ErrorMessage)
    '                Else
    '                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

    '                End If
    '            Next
    '        Next

    '        Return New PortalKayoinobaCheckListJsonResult() With {
    '            .IsSuccess = Boolean.FalseString,
    '            .Massages = errorMessage
    '        }.ToJsonResult()

    '    End Function



    '#End Region

#Region "「薬局連携促進」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function MedicienConnectionPromote(fromPageNo As Nullable(Of Byte)) As ActionResult

        Return View()

    End Function

#End Region

#Region "「Fitbit連携」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function FitbitConnection(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As PortalFitbitConnectionViewModel = PortalFitbitConnectionWorker.CreateViewModel(mainModel, fromPageNoType)

        ' キャッシュ
        mainModel.SetInputModelCache(viewModel)

        Return View(viewModel)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Update")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function FitbitConnectionResult() As ActionResult

        Dim url As String = PortalFitbitConnectionWorker.GetAuthorizationUrl()

        Return New PortalFitbitConnectionJsonResult() With {.IsSuccess = Boolean.TrueString, .Url = url}.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Cancel")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function FitbitConnectionResult(cancel As String, dummy As String) As ActionResult

        Dim message As String = String.Empty

        If PortalFitbitConnectionWorker.Cancel(Me.GetQolmsYappliModel(), message) Then
            Return New PortalFitbitConnectionJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        Else

            Return New PortalFitbitConnectionJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        End If

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function FitbitConnectionResult(code As String) As ActionResult

        Dim message As String = String.Empty

        If code <> String.Empty Then

            Dim redirectUrl As String = Url.Action("fitbitconnectionresult", "portal")
            Dim token As FitbitTokenSet = FitbitWorker.GetFitbitToken(code, redirectUrl)

            If Not String.IsNullOrWhiteSpace(token.Token) Then

                Dim result As Boolean = PortalFitbitConnectionWorker.Register(Me.GetQolmsYappliModel(), token, message)
            End If

        End If

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As PortalFitbitConnectionViewModel = mainModel.GetInputModelCache(Of PortalFitbitConnectionViewModel)()
        Dim fromPageNo As Byte = viewModel.FromPageNoType

        Return RedirectToAction("FitbitConnection", New With {.fromPageNo = fromPageNo})


    End Function



#End Region

#Region "「女性の健康サポート」画面"

    <HttpGet()>
    <QyLogging()>
    Public Function MovForFemale() As ActionResult

        Return View()

    End Function
#End Region

#Region "「ユーザー情報」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function UserInfomation(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel
        Dim viewModel As PortalUserInfomationInputModel = PortalUserInfomationWorker.CreateViewModel(mainModel, fromPageNoType)

        'キャッシュが必要かどうかは検討
        mainModel.SetInputModelCache(viewModel)
        Return View(viewModel)

    End Function

#End Region

#Region "「ユーザー情報編集」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function UserInfomationEdit(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel
        Dim viewModel As PortalUserInfomationEditInputModel = PortalUserInfomationEditWorker.CreateViewModel(mainModel, fromPageNoType)

        'キャッシュが必要かどうかは検討
        mainModel.SetInputModelCache(viewModel)
        Return View(viewModel)

    End Function

    <HttpPost()>
    <ValidateInput(False)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function UserInfomationEditResult(inputModel As PortalUserInfomationEditInputModel) As ActionResult

        ' モデルの検証
        If Me.ModelState.IsValid() Then

            ' 登録処理
            If PortalUserInfomationEditWorker.Resister(Me.GetQolmsYappliModel(), inputModel) Then
                Return New PortalUserInfomationEditJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()
            End If 

        End If
                
        ' 検証に失敗
        Dim errorMessage As New Dictionary(Of String, String)()

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode(e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        Return New PortalUserInfomationEditJsonResult() With {
            .Massages = errorMessage,
            .IsSuccess = Boolean.FalseString
        }.ToJsonResult()

        Return View()

    End Function

#End Region

#Region "「Fitbitクーポン」画面"

    <HttpGet()>    
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function CouponForFitbit(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Return View(PortalCouponForFitbitWorker.CreateViewModel(Me.GetQolmsYappliModel(),fromPageNoType))

    End Function
        
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CouponForFitbitResult(couponType As Byte) As ActionResult
        
        Try
            If PortalCouponForFitbitWorker.Exchange(Me.GetQolmsYappliModel(), couponType) Then
                Me.TempData("IsFinish") = True

                Return New PortalCouponForFitbitJsonResult() With {
                .Message = HttpUtility.HtmlEncode("ポイント交換が完了しました。"),
                .IsSuccess = Boolean.TrueString}.ToJsonResult()
            Else
                Return New PortalCouponForFitbitJsonResult() With {
                .Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
                .IsSuccess = Boolean.FalseString}.ToJsonResult()
            End If

        Catch ex As Exception
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format(ex.Message))
            Return New PortalCouponForFitbitJsonResult() With {
                .Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
                .IsSuccess = Boolean.FalseString}.ToJsonResult()
        End Try

    End Function

#End Region

#Region "「SMS認証」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function SmsAuthentication(fromPageNo As Nullable(Of Byte)) As ActionResult

        ' 表示のみ
        Return View()

    End Function


    <HttpPost()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyActionMethodSelector("Send")>
    Public Function SmsAuthenticationResult(phoneNumber As String) As JsonResult

        If Me.ModelState.IsValid Then

            If PortalSmsAuthenticationWorker.SendPassCode(Me.GetQolmsYappliModel(), phoneNumber) Then
                '電話番号を確認してパスコードの発行、SMSの送信
                '成功ならパスコードを表示
                Return New PasswordResetRecoverSMSJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()
                'End If
            End If

        End If
        '検証の結果を返却
        Return New PasswordResetRecoverSMSJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Messages = New Dictionary(Of String, String)() From {{"error", "エラーメッセージ"}}
                }.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyActionMethodSelector("Success")>
    Public Function SmsAuthenticationResult(PhoneNumber As String, dummy As String) As ActionResult

        Dim str As String = String.Empty
        '電話番号を画面に返す、パスコード入力画面の表示
        If Not String.IsNullOrWhiteSpace(PhoneNumber) Then
            Using resource As New QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsWeb)
                str = resource.EncryptString(PhoneNumber)
            End Using
        End If

        Dim disp As String = PhoneNumber.Substring((PhoneNumber.Length - 4), 4).PadLeft(PhoneNumber.Length, "*"c)
        Dim viewModel As New PasswordResetRecoverSMSPassCodeViewModel() With {
            .CryptPhoneNumber = str,
            .DispPhoneNumber = disp}
        Return PartialView("_PasswordResetRecoverSMSPassCodePartialView", viewModel)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyActionMethodSelector("PassCode")>
    Public Function SmsAuthenticationResult(PhoneNumber As String, PassCode As String, dummy As String) As ActionResult

        Dim str As String = String.Empty

        If Not String.IsNullOrWhiteSpace(PhoneNumber) Then
            Using resource As New QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsWeb)
                str = resource.DecryptString(PhoneNumber)
            End Using
        End If

        'パスコードの認証
        '成功ならSMS認証OKにして登録、/portal/UserInfomation 戻る(もしかしたら戻る画面は指定しなきゃかも）
        Return New PasswordResetRecoverSMSJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()

    End Function

#End Region

#Region "「連携システムID登録」画面"

    ''' <summary>
    ''' QR または 外部リンクから呼び出される 連携システムID登録 の表示要求を処理します。
    ''' </summary>
    ''' <param name="linkageSystemNo"></param>
    ''' <param name="linkageSystemId"></param>
    ''' <returns></returns>
    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function AppendLinkageId(linkageSystemNo As String, linkageSystemId As String) As ActionResult

        Dim errorMessage As String = String.Empty
        PortalAppendLinkageIdWorker.Append(Me.GetQolmsYappliModel(), linkageSystemNo, linkageSystemId, errorMessage)
        Me.TempData("ErrorMessage") = errorMessage
        Return View()

    End Function

#End Region

#Region "「問診システム呼び出し」画面"

    ''' <summary>
    ''' すながわ内科用問診システム呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpPost()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function MonshinUrl() As ActionResult

        Dim url As String = PortalMonshinWorker.CreateRedirectUrl(Me.GetQolmsYappliModel())

        Return New PortalHomeUrlResult() With {.IsSuccess = Boolean.TrueString, .Url = url}.ToJsonResult()

    End Function

#End Region

#Region "「市民認証」画面"

    ''' <summary>
    ''' 市民認証画面呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function LocalIdVerification() As ActionResult

        If PortalLocalIdVerificationWorker.IsEntered(GetQolmsYappliModel()) Then
            'すでにエントリー済みなら確認画面へ
            Return RedirectToAction("LocalIdVerificationDetail")

        End If

        Return View()

    End Function

    ''' <summary>
    ''' 市民認証規約画面呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function LocalIdVerificationAgreement() As ActionResult

        Return View(PortalLocalIdVerificationAgreementWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function

    ''' <summary>
    ''' 市民認証入力画面呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function LocalIdVerificationRegister() As ActionResult

        '画面の入力項目を取得
        Dim viewmodel As PortalLocalIdVerificationRegisterInputModel = PortalLocalIdVerificationRegisterWorker.CreateViewModel(GetQolmsYappliModel())

        Return View(viewmodel)
    End Function

    ''' <summary>
    ''' 市民認証入力画面呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpPost()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function LocalIdVerificationRegisterResult(inputModel As PortalLocalIdVerificationRegisterInputModel) As ActionResult

        Dim errorMessage As New Dictionary(Of String, String)()
        Dim message As String = String.Empty
        '入力項目の更新
        'IDを発行して連携を登録
        If Me.ModelState.IsValid Then
            If PortalLocalIdVerificationRegisterWorker.Register(GetQolmsYappliModel(), inputModel, message) Then

                Return New PortalLocalIdVerificationJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()
            Else

                errorMessage.Add("summary", If(String.IsNullOrWhiteSpace(message), "登録に失敗しました。", message))
            End If

        End If

        ' 検証失敗
        ' 検証失敗
        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode(e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next
        'If errorMessage.Count = 0 Then
        '    errorMessage.Add("Summary", "登録に失敗しました。")

        'End If
        Return New PortalLocalIdVerificationJsonResult() With {.IsSuccess = Boolean.FalseString, .Messages = errorMessage}.ToJsonResult()

    End Function


    ''' <summary>
    ''' 市民認証入力画面呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpPost()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function LocalIdVerificationRegisterCancelResult() As ActionResult

        Dim errorMessage As New Dictionary(Of String, String)()
        Dim message As String = String.Empty
        '入力項目の更新
        'IDを発行して連携を登録
        If Me.ModelState.IsValid Then
            If PortalLocalIdVerificationRegisterWorker.Cancel(GetQolmsYappliModel(), message) Then

                Return New PortalLocalIdVerificationJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()
            Else

                errorMessage.Add("summary", If(String.IsNullOrWhiteSpace(message), "登録に失敗しました。", message))
            End If
        End If

        ' 検証失敗
        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode(e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        'If errorMessage.Count = 0 Then
        '    errorMessage.Add("Summary", "登録に失敗しました。")

        'End If
        Return New PortalLocalIdVerificationJsonResult() With {.IsSuccess = Boolean.FalseString, .Messages = errorMessage}.ToJsonResult()

    End Function

    ''' <summary>
    ''' 市民認証申請画面呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function LocalIdVerificationRequest() As ActionResult

        '発行したIDを取得
        'ステータスを取得
        'リンクを設定
        Dim ViewModel As PortalLocalIdVerificationRequestViewModel = PortalLocalIdVerificationRequestWorker.CreateViewModel(Me.GetQolmsYappliModel())

        If ViewModel.LinkageSystemNo > 0 Then
            Return View(ViewModel)
        Else
            Return RedirectToAction("LocalIdVerification")
        End If
        Return View()
    End Function

    ''' <summary>
    ''' 市民認証確認画面呼び出し
    ''' </summary>
    ''' <returns></returns>
    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function LocalIdVerificationDetail() As ActionResult

        'ステータスを取得
        '拒否の場合のメッセージを取得
        Dim ViewModel As PortalLocalIdVerificationDetailViewModel = PortalLocalIdVerificationDetailWorker.CreateViewModel(Me.GetQolmsYappliModel())

        If ViewModel.LinkageSystemNo > 0 Then
            Return View(ViewModel)
        Else
            Return RedirectToAction("LocalIdVerification")
        End If

    End Function


#End Region

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
    Public Function PortalHeaderPartialView() As ActionResult

        ' パーシャル ビューを返却
        If Me.IsYappli Then
            Return New EmptyResult()
        Else
            Return PartialView("_PortalHeaderPartialView")
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
    Public Function PortalFooterPartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_PortalFooterPartialView")

    End Function

#End Region

#Region "「ホーム」画面用 パーシャル ビュー"

    ''' <summary>
    ''' 「ホーム」画面用
    ''' データ領域パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function PortalHomeDataAreaPartialView() As ActionResult

        Dim model As PortalHomeViewModel = Me.GetPageViewModel(Of PortalHomeViewModel)()

        ' パーシャル ビューを返却
        Return PartialView("_PortalHomeDataAreaPartialView", model.PartialViewModel)

    End Function


    ''' <summary>
    ''' 「ホーム」画面用
    ''' データ領域パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function PortalHomeChallengeAreaPartialView() As ActionResult

        Dim model As PortalHomeViewModel = Me.GetPageViewModel(Of PortalHomeViewModel)()

        ' パーシャル ビューを返却
        Return PartialView("_PortalHomeChallengeAreaPartialView", model.ChallengeAreaPartialViewModel)
    End Function



#End Region

#Region "「医療機関検索」画面用 パーシャル ビュー"

    ''' <summary>
    ''' 「医療機関検索」画面用
    ''' 検索結果パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function PortalSearchResultPartialView() As ActionResult

        Dim model As PortalSearchViewModel = Me.GetPageViewModel(Of PortalSearchViewModel)()

        ' パーシャル ビューを返却
        Return PartialView("_PortalSearchResultPartialView", model)

    End Function
    ''' <summary>
    ''' 「医療機関詳細」画面用
    ''' 検索結果パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function PortalSearchDetailResultPartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_PortalSearchDetailResultPartialView", New PortalSearchDetailViewModel)

    End Function

#End Region

#Region "「チャレンジ詳細」画面用 パーシャル ビュー"

    ''' <summary>
    ''' 「チャレンジ詳細」画面用
    ''' 検索結果パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function PortalChallengeDetailPartialView() As ActionResult

        Dim model As PortalChallengeDetailViewModel = Me.GetPageViewModel(Of PortalChallengeDetailViewModel)()

        ' パーシャル ビューを返却
        If model.ChallengeItem.Challengekey = Guid.Parse("ded05070-8718-4313-924a-25233e35e218") Then
            Return PartialView("_PortalChallengeDetail1PartialView", model)

        ElseIf model.ChallengeItem.Challengekey = Guid.Parse("cdf50ec6-da20-4d47-84de-6f14bf9cec1f") Then
            Return PartialView("_PortalChallengeDetail2PartialView", model)

        ElseIf model.ChallengeItem.Challengekey = Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1") OrElse model.ChallengeItem.Challengekey = Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c") Then

            Return PartialView("_PortalChallengeDetail3PartialView", model)
        ElseIf model.ChallengeItem.Challengekey = Guid.Parse("3ab347fb-cfdf-4ff9-ba5e-a2a607b7ec29") Then

            Return PartialView("_PortalChallengeDetail5PartialView", model)

        Else
            Return PartialView("_PortalChallengeDetailDefaltPartialView", model)

        End If


    End Function

#End Region

#End Region

#Region "検証用"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    <Obsolete("検証用です。検証が終われば削除します。")>
    Public Function Test1() As ActionResult

        Return New EmptyResult()

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    <Obsolete("検証用です。検証が終われば削除します。")>
    Public Function Test2() As ActionResult

        Return New EmptyResult()

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    <Obsolete("検証用です。検証が終われば削除します。")>
    Public Function Test3() As ActionResult

        Return New EmptyResult()

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    <Obsolete("検証用です。検証が終われば削除します。")>
    Public Function TestPushApi() As ActionResult

        Dim res As String = CalomealWebViewWorker.TestOpenPushApi(Me.GetQolmsYappliModel())
        Me.TempData("ErrorMessage") = res
        Return View()

    End Function


#End Region

End Class

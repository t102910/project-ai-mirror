Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Globalization
Imports System.Threading.Tasks
Imports System.Web.Routing

''' <summary>
''' Note 系画面への HTTP 要求に応答する機能を提供します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class NoteController
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
    ''' <see cref="NoteController" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 暗号化された JSON 形式の パラメータ を復号化します。
    ''' </summary>
    ''' <typeparam name="T">復号化する パラメータ クラス の型。</typeparam>
    ''' <param name="reference">暗号化された JSON 形式の パラメータ。</param>
    ''' <returns>
    ''' 復号化された パラメータ クラス の インスタンス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function DecryptReference(Of T As QyJsonParameterBase)(reference As String) As T

        Dim result As T = Nothing

        Try
            Dim jsonString As String = String.Empty

            Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                jsonString = crypt.DecryptString(reference)
            End Using

            result = QyJsonParameterBase.FromJsonString(Of T)(jsonString)
        Catch
        End Try

        Return result

    End Function

#End Region

#Region "ページ ビュー アクション"

#Region "「歩く」画面"

    ''' <summary>
    ''' 「歩く」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Walk(Optional selectdate As String = "") As ActionResult

        '日付
        Dim recordDate As Date = Date.MinValue
        If Not String.IsNullOrWhiteSpace(selectdate) AndAlso selectdate.Length = 8 Then

            Dim tryRecordDate As Date = Date.MinValue
            Dim ci As CultureInfo = CultureInfo.CurrentCulture
            Dim dts As DateTimeStyles = DateTimeStyles.None
            If Date.TryParseExact(selectdate, "yyyyMMdd", ci, dts, tryRecordDate) AndAlso tryRecordDate < Date.Now.Date Then
                recordDate = tryRecordDate
            End If
        End If

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim redirecturl As String = PortalAlkooConnectionWorker.CreateWalkRedirectUrl(mainModel)

        If Not String.IsNullOrWhiteSpace(redirecturl) Then
            Return Redirect(redirecturl)
        End If

        ' ビュー を返却
        Return View(NoteWalkWorker.CreateViewModel(mainModel, recordDate))

    End Function

    ''' <summary>
    ''' 「歩く」画面の「グラフ」の表示要求を処理します。
    ''' </summary>
    ''' <param name="vitalType">バイタル 情報の種別。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Graph")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function WalkResult(vitalType As QyVitalTypeEnum) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteWalkViewModel = NoteWalkWorker.GetViewModelCache(mainModel, False) ' mainModel.GetInputModelCache(Of NoteWalkViewModel)()
        Dim partialViewName As String = String.Empty
        Dim partialViewModel As QyVitalGraphPartialViewModelBase = NoteWalkWorker.CreateGraphPartialViewModel(mainModel, pageViewModel, vitalType, partialViewName, True) ' TODO: 暫定、グラフの情報を強制的に再取得

        ' パーシャル ビュー を返却
        Return PartialView(partialViewName, partialViewModel)

    End Function

    ''' <summary>
    ''' 「歩く」画面の「詳細」の表示要求を処理します。
    ''' </summary>
    ''' <param name="vitalType">バイタル 情報の種別。</param>
    ''' <param name="recordDate">測定日。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Detail")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function WalkResult(vitalType As QyVitalTypeEnum, recordDate As Date) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteWalkViewModel = NoteWalkWorker.GetViewModelCache(mainModel) ' セッション タイム アウト 時の再 ログイン 後対策 mainModel.GetInputModelCache(Of NoteWalkViewModel)()
        Dim partialViewName As String = String.Empty

        NoteWalkWorker.CreateGraphPartialViewModel(mainModel, pageViewModel, vitalType, partialViewName, True) ' TODO: 暫定、グラフ の情報を強制的に再取得

        Dim partialViewModel As QyVitalDetailPartialViewModelBase = NoteWalkWorker.CreateDetailPartialViewModel(mainModel, pageViewModel, vitalType, recordDate, partialViewName, True) ' TODO: 暫定、詳細情報を強制的に再取得

        ' パーシャル ビュー を返却
        Return PartialView(partialViewName, partialViewModel)

    End Function

    ''' <summary>
    ''' 「歩く」画面の削除要求を処理します。
    ''' </summary>
    ''' <param name="vitalType">バイタル 情報の種別。</param>
    ''' <param name="reference">
    ''' 削除対象の日の リスト。
    ''' </param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Delete")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function WalkResult(vitalType As QyVitalTypeEnum, reference As List(Of Date)) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteWalkViewModel = NoteWalkWorker.GetViewModelCache(mainModel) ' セッション タイム アウト 時の再 ログイン 後対策 mainModel.GetInputModelCache(Of NoteWalkViewModel)()

        ' JSON を返却
        Return NoteWalkWorker.Delete(mainModel, pageViewModel, vitalType, reference).ToJsonResult()

    End Function

    ''' <summary>
    ''' 「歩く」画面の登録要求を処理します。
    ''' </summary>
    ''' <param name="model">「歩く」画面 インプット モデル。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Edit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function WalkResult(model As NoteVitalEditInputModel) As JsonResult

        Dim result As New NoteVitalEditJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .VitalTypeN = New List(Of String)(),
            .Messages = New List(Of String)()
        }

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteWalkViewModel = NoteWalkWorker.GetViewModelCache(mainModel) ' セッション タイム アウト 時の再 ログイン 後対策 mainModel.GetInputModelCache(Of NoteWalkViewModel)()

        ' モデル の検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功

            ' 登録処理
            Dim vitalTypeN As New List(Of QyVitalTypeEnum)()
            Dim messageN As New List(Of String)()

            If NoteWalkWorker.Edit(mainModel, pageViewModel, model, vitalTypeN, messageN) Then
                ' 登録成功
                result.VitalTypeN.AddRange(vitalTypeN.ConvertAll(Function(i) i.ToString()))
                result.IsSuccess = Boolean.TrueString
            Else
                ' 登録失敗
                result.Messages.AddRange(messageN)
            End If
        Else
            ' 検証失敗
            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    result.Messages.Add(e.ErrorMessage)
                Next
            Next
        End If

        ' JSON を返却
        Return result.ToJsonResultWithSanitize()

    End Function

#End Region

#Region "「運動」画面"

    ''' <summary>
    ''' 「運動」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Exercise(Optional selectdate As String = "") As ActionResult

        '日付
        Dim recordDate As Date = Date.MinValue
        If Not String.IsNullOrWhiteSpace(selectdate) AndAlso selectdate.Length = 8 Then

            Dim tryRecordDate As Date = Date.MinValue
            Dim ci As CultureInfo = CultureInfo.CurrentCulture
            Dim dts As DateTimeStyles = DateTimeStyles.None
            If Date.TryParseExact(selectdate, "yyyyMMdd", ci, dts, tryRecordDate) AndAlso tryRecordDate < Date.Now.Date Then
                recordDate = tryRecordDate
            End If
        End If

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 表示対象の モデル を作成
        Dim viewModel As NoteExerciseViewModel = NoteExerciseWorker.CreateViewModel(mainModel, recordDate)

        ' 編集対象の モデル を作成
        Dim inputModel As NoteExerciseInputModel = New NoteExerciseInputModel()

        'モデル を キャッシュ へ格納
        mainModel.SetInputModelCache(viewModel)
        mainModel.SetInputModelCache(inputModel)

        ' ビュー を返却
        Return View(viewModel)

    End Function

    ''' <summary>
    ''' 「運動」画面の登録要求を処理します。
    ''' </summary>
    ''' <param name="model">「運動」画面 インプット モデル。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Edit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ExerciseResult(model As NoteExerciseInputModel) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 編集対象の モデル を キャッシュ から取得
        Dim inputModel As NoteExerciseInputModel = NoteExerciseWorker.GetInputModelCache(mainModel) ' セッション タイム アウト 時の再 ログイン 後対策 mainModel.GetInputModelCache(Of NoteExerciseInputModel)()

        ' モデル へ入力値をを反映
        inputModel.UpdateByInput(model)

        ' モデル の検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功

            ' データ 登録
            If NoteExerciseWorker.Edit(mainModel, inputModel) Then
                ' 登録成功

                ' 表示対象の モデル を作成
                Dim viewModel As NoteExerciseViewModel = NoteExerciseWorker.CreateViewModel(mainModel)

                ' 編集対象の モデル を作成
                inputModel = New NoteExerciseInputModel()

                ' モデル を キャッシュ へ格納
                mainModel.SetInputModelCache(viewModel)
                mainModel.SetInputModelCache(inputModel)

                ' 「ホーム」画面用の キャッシュ を クリア しておく
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                ' 「最近の運動」パーシャル ビュー を更新
                Return PartialView("_NoteExerciseCardPartialView", viewModel)
            Else
                ' 登録失敗
                Throw New InvalidOperationException("運動情報の登録に失敗しました。")
            End If
        Else
            ' 検証失敗

            ' 独自に エラー メッセージ を用意し ビュー に渡す
            Dim errorMessage As New List(Of String)

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' 失敗なら JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage,
                .IsSuccess = Boolean.FalseString
            }.ToJsonResultWithSanitize()

        End If

    End Function

    ''' <summary>
    ''' 「運動」画面の削除要求を処理します。
    ''' </summary>
    ''' <param name="RecordDate">運動日。</param>
    ''' <param name="ExerciseType">運動の種別。</param>
    ''' <param name="Sequence">日付内連番。</param>
    ''' <param name="Calorie">カロリー。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Delete")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function ExerciseResult(RecordDate As Date, ExerciseType As Byte, Sequence As Integer, Calorie As String) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 登録 API を実行
        ' 登録処理（削除 フラグ を ON）
        If NoteExerciseWorker.Delete(mainModel, RecordDate, ExerciseType, Sequence, Calorie) Then
            ' 登録成功

            ' 表示対象の モデル を作成
            Dim viewModel As NoteExerciseViewModel = NoteExerciseWorker.CreateViewModel(mainModel)

            ' 編集対象の モデル を作成
            Dim inputModel As NoteExerciseInputModel = New NoteExerciseInputModel()

            ' モデル を キャッシュ へ格納
            mainModel.SetInputModelCache(viewModel)
            mainModel.SetInputModelCache(inputModel)

            ' 「ホーム」画面用の キャッシュ を クリアし ておく
            mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

            ' 「最近の運動」パーシャル ビュー を更新
            Return PartialView("_NoteExerciseCardPartialView", viewModel)
        Else
            ' 登録失敗
            Throw New InvalidOperationException("「運動」の削除に失敗しました。")
        End If

        ' 「運動」画面へ リダイレクト
        Return RedirectToAction("Exercise", "Note")

    End Function

#End Region

    '#Region "「食事」画面"

    '    <HttpGet()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal() As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

    '        ' 表示対象のモデルを作成
    '        Dim viewModel As NoteMealViewModel = NoteMealWorker.CreateViewModel(mainModel)

    '        ' 編集対象のモデルを作成
    '        Dim inputModel As NoteMealInputModel = New NoteMealInputModel() With {
    '            .RecordDate = Date.Now,
    '            .MealType = QyMealTypeEnum.Breakfast
    '        }

    '        viewModel.InputModel = inputModel

    '        'モデルをキャッシュへ格納()
    '        mainModel.SetInputModelCache(viewModel)

    '        ' ビューを返却
    '        Return View(viewModel)

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Delete")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function MealResult(RecordDate As Date, MealType As Byte, Sequence As Integer) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

    '        'Dim recorddate As Date = model.RecordDate
    '        'Dim mealtype As QyMealTypeEnum = model.MealType
    '        'Dim sequence As Integer = model.Sequence

    '        ' 登録APIを実行
    '        ' 登録処理（削除フラグをON）
    '        If NoteMealWorker.Delete(mainModel, RecordDate, MealType, Sequence) Then
    '            ' 登録成功
    '            'InputModelは覚えておく
    '            Dim inputModel As NoteMealInputModel = mainModel.GetInputModelCache(Of NoteMealViewModel)().InputModel

    '            ' 表示対象のモデルを作成
    '            Dim viewModel As NoteMealViewModel = NoteMealWorker.CreateViewModel(mainModel)

    '            'モデルをキャッシュへ格納()
    '            viewModel.InputModel = inputModel
    '            mainModel.SetInputModelCache(viewModel)

    '            'Home画面用のキャッシュをクリアしておく
    '            mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

    '            ' 「最近の食事」パーシャルビューを更新
    '            Return PartialView("_NoteMealCardPartialView", viewModel)

    '        Else
    '            ' 登録失敗
    '            Throw New InvalidOperationException("「食事」の登録に失敗しました。")
    '            ' 「食事」画面へリダイレクト
    '            Return RedirectToAction("Meal", "Note")
    '        End If

    '        ' 「食事」画面へリダイレクト
    '        Return RedirectToAction("Meal", "Note")

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Add,Edit,Next,AddS,NextS")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function MealResult(model As NoteMealInputModel) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

    '        ' 編集対象のモデル
    '        Dim inputModel As NoteMealInputModel = mainModel.GetInputModelCache(Of NoteMealViewModel)().InputModel

    '        Select Case Me.ActionSource

    '            Case "Add"
    '                ' モデルへ入力値をを反映
    '                inputModel.UpdateByInput(model)

    '                ' モデルの検証状態を確認
    '                If Me.ModelState.IsValid Then
    '                    ' 検証成功
    '                    '
    '                    If inputModel.AttachedFileN.Any Then
    '                        ' データ登録
    '                        If NoteMealWorker.Add(mainModel, inputModel) Then
    '                            ' 登録成功
    '                            'InputModelは覚えておく
    '                            inputModel.AttachedFileN = New List(Of AttachedFileItem)()

    '                            ' 表示対象のモデルを作成
    '                            Dim viewModel As NoteMealViewModel = NoteMealWorker.CreateViewModel(mainModel)

    '                            'モデルをキャッシュへ格納()
    '                            viewModel.InputModel = inputModel
    '                            mainModel.SetInputModelCache(viewModel)

    '                            'Home画面用のキャッシュをクリアしておく
    '                            mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

    '                            ' 「最近の食事」パーシャルビューを更新
    '                            Return PartialView("_NoteMealCardPartialView", viewModel)

    '                        Else
    '                            ' 登録失敗
    '                            Throw New InvalidOperationException("食事情報の登録に失敗しました。")

    '                            ' 「食事」画面へリダイレクト
    '                            Return RedirectToAction("Meal", "Note")
    '                        End If
    '                    Else
    '                        ' 検証失敗

    '                        ' 独自にエラーメッセージを用意しビューに渡す
    '                        Dim errorMessage As New List(Of String)
    '                        errorMessage.Add("ファイルを選択してください。")
    '                        ' 失敗ならJSONを返却
    '                        Return New NoteEditJsonResult() With {
    '                            .Messages = errorMessage,
    '                            .IsSuccess = Boolean.FalseString}.ToJsonResult()

    '                    End If

    '                Else
    '                    ' 検証失敗

    '                    ' 独自にエラーメッセージを用意しビューに渡す
    '                    Dim errorMessage As New List(Of String)()
    '                    For Each key As String In Me.ModelState.Keys
    '                        For Each e As ModelError In Me.ModelState(key).Errors
    '                            errorMessage.Add(e.ErrorMessage)
    '                        Next
    '                    Next

    '                    'Dim errorMessage As New Dictionary(Of String, String)()

    '                    'For Each key As String In Me.ModelState.Keys
    '                    '    For Each e As ModelError In Me.ModelState(key).Errors
    '                    '        errorMessage.Add(key, e.ErrorMessage)
    '                    '    Next
    '                    'Next

    '                    'Me.TempData("ErrorMessage") = errorMessage

    '                    ' 失敗ならJSONを返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = errorMessage,
    '                        .IsSuccess = Boolean.FalseString}.ToJsonResult()

    '                End If

    '            Case "Edit"

    '                ' モデルへ入力値をを反映
    '                'inputModel = New NoteMealInputModel()
    '                inputModel.UpdateByInput(model)

    '                '入力検証　TODO

    '                ' モデルの検証状態を確認
    '                If Me.ModelState.IsValid Then
    '                    ' データ登録
    '                    If NoteMealWorker.Edit(mainModel, inputModel) Then
    '                        ' 登録成功
    '                        ' 表示対象のモデルを作成
    '                        Dim viewModel As NoteMealViewModel = NoteMealWorker.CreateViewModel(mainModel)

    '                        ' 編集対象のモデルを作成
    '                        inputModel = mainModel.GetInputModelCache(Of NoteMealViewModel)().InputModel
    '                        inputModel.AttachedFileN = New List(Of AttachedFileItem)()

    '                        '覚えておく
    '                        viewModel.InputModel = inputModel

    '                        'モデルをキャッシュへ格納()
    '                        mainModel.SetInputModelCache(viewModel)

    '                        'Home画面用のキャッシュをクリアしておく
    '                        mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

    '                        ' 「最近の食事」パーシャルビューを更新
    '                        Return PartialView("_NoteMealCardPartialView", viewModel)

    '                    Else
    '                        ' 登録失敗
    '                        Throw New InvalidOperationException("食事情報の登録に失敗しました。")

    '                        ' 「食事」画面へリダイレクト
    '                        Return RedirectToAction("Meal", "Note")
    '                    End If
    '                Else
    '                    ' 検証失敗

    '                    ' 独自にエラーメッセージを用意しビューに渡す
    '                    Dim errorMessage As New List(Of String)
    '                    For Each key As String In Me.ModelState.Keys
    '                        For Each e As ModelError In Me.ModelState(key).Errors
    '                            errorMessage.Add(e.ErrorMessage)
    '                        Next
    '                    Next

    '                    'Dim errorMessage As New Dictionary(Of String, String)()

    '                    'For Each key As String In Me.ModelState.Keys
    '                    '    For Each e As ModelError In Me.ModelState(key).Errors
    '                    '        errorMessage.Add(key, e.ErrorMessage)
    '                    '    Next
    '                    'Next

    '                    'Me.TempData("ErrorMessage") = errorMessage

    '                    ' 失敗ならJSONを返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = errorMessage,
    '                        .IsSuccess = Boolean.FalseString}.ToJsonResult()
    '                End If

    '            Case "Next"

    '                If Me.ModelState.IsValid Then
    '                    ' モデルの検証状態を確認
    '                    If inputModel.AttachedFileN.Any Then
    '                        ' 検証成功
    '                        ' データ登録
    '                        If NoteMealWorker.Add(mainModel, inputModel) Then
    '                            ' 登録成功
    '                            'InputModelは覚えておく
    '                            inputModel = mainModel.GetInputModelCache(Of NoteMealViewModel)().InputModel
    '                            inputModel.AttachedFileN = New List(Of AttachedFileItem)()

    '                            ' 表示対象のモデルを作成
    '                            Dim viewModel As NoteMealViewModel = NoteMealWorker.CreateViewModel(mainModel)

    '                            'モデルをキャッシュへ格納()
    '                            viewModel.InputModel = inputModel
    '                            mainModel.SetInputModelCache(viewModel)

    '                            'Home画面用のキャッシュをクリアしておく
    '                            mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

    '                            ' 「最近の食事」パーシャルビューを更新
    '                            Return PartialView("_NoteMealCardPartialView", viewModel)

    '                        Else
    '                            ' 登録失敗
    '                            Throw New InvalidOperationException("食事情報の登録に失敗しました。")

    '                            ' 「食事」画面へリダイレクト
    '                            Return RedirectToAction("Meal", "Note")
    '                        End If


    '                    Else
    '                        ' 検証失敗

    '                        ' 独自にエラーメッセージを用意しビューに渡す
    '                        Dim errorMessage As New List(Of String)
    '                        For Each key As String In Me.ModelState.Keys
    '                            For Each e As ModelError In Me.ModelState(key).Errors
    '                                errorMessage.Add(e.ErrorMessage)
    '                            Next
    '                        Next

    '                        'Dim errorMessage As New Dictionary(Of String, String)()

    '                        'For Each key As String In Me.ModelState.Keys
    '                        '    For Each e As ModelError In Me.ModelState(key).Errors
    '                        '        errorMessage.Add(key, e.ErrorMessage)
    '                        '    Next
    '                        'Next

    '                        'Me.TempData("ErrorMessage") = errorMessage

    '                        ' 失敗ならJSONを返却
    '                        Return New NoteEditJsonResult() With {
    '                            .Messages = errorMessage,
    '                            .IsSuccess = Boolean.FalseString}.ToJsonResult()

    '                    End If
    '                Else
    '                    ' 検証失敗

    '                    ' 独自にエラーメッセージを用意しビューに渡す
    '                    Dim errorMessage As New List(Of String)
    '                    For Each key As String In Me.ModelState.Keys
    '                        For Each e As ModelError In Me.ModelState(key).Errors
    '                            errorMessage.Add(e.ErrorMessage)
    '                        Next
    '                    Next

    '                    'Dim errorMessage As New Dictionary(Of String, String)()

    '                    'For Each key As String In Me.ModelState.Keys
    '                    '    For Each e As ModelError In Me.ModelState(key).Errors
    '                    '        errorMessage.Add(key, e.ErrorMessage)
    '                    '    Next
    '                    'Next

    '                    'Me.TempData("ErrorMessage") = errorMessage

    '                    ' 失敗ならJSONを返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = errorMessage,
    '                        .IsSuccess = Boolean.FalseString}.ToJsonResult()
    '                End If
    '            Case "AddS"

    '                ' PalString -> FoodItem へ変換して格納しておく

    '                ' モデルへ入力値をを反映
    '                inputModel.UpdateByInput(model)

    '                ' モデルの検証状態を確認
    '                If Me.ModelState.IsValid Then
    '                    ' 検証成功

    '                    ' データ登録
    '                    If NoteMealWorker.AddS(mainModel, inputModel) Then
    '                        ' 登録成功
    '                        'InputModelは覚えておく
    '                        inputModel.AttachedFileN = New List(Of AttachedFileItem)()

    '                        ' 表示対象のモデルを作成
    '                        Dim viewModel As NoteMealViewModel = NoteMealWorker.CreateViewModel(mainModel)

    '                        'モデルをキャッシュへ格納()
    '                        viewModel.InputModel = inputModel
    '                        mainModel.SetInputModelCache(viewModel)

    '                        'Home画面用のキャッシュをクリアしておく
    '                        mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

    '                        ' 「最近の食事」パーシャルビューを更新
    '                        Return PartialView("_NoteMealCardPartialView", viewModel)

    '                    Else
    '                        ' 登録失敗
    '                        Throw New InvalidOperationException("食事情報の登録に失敗しました。")

    '                        ' 「食事」画面へリダイレクト
    '                        Return RedirectToAction("Meal", "Note")
    '                    End If


    '                Else
    '                    ' 検証失敗

    '                    ' 独自にエラーメッセージを用意しビューに渡す
    '                    Dim errorMessage As New List(Of String)
    '                    For Each key As String In Me.ModelState.Keys
    '                        For Each e As ModelError In Me.ModelState(key).Errors
    '                            errorMessage.Add(e.ErrorMessage)
    '                        Next
    '                    Next

    '                    'Dim errorMessage As New Dictionary(Of String, String)()

    '                    'For Each key As String In Me.ModelState.Keys
    '                    '    For Each e As ModelError In Me.ModelState(key).Errors
    '                    '        errorMessage.Add(key, e.ErrorMessage)
    '                    '    Next
    '                    'Next

    '                    'Me.TempData("ErrorMessage") = errorMessage

    '                    ' 失敗ならJSONを返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = errorMessage,
    '                        .IsSuccess = Boolean.FalseString}.ToJsonResult()

    '                End If

    '            Case "NextS"

    '                ' 検証成功
    '                If Me.ModelState.IsValid Then
    '                    ' モデルへ入力値をを反映
    '                    inputModel.ItemName = model.ItemName
    '                    inputModel.Calorie = model.Calorie
    '                    inputModel.PalString = model.PalString

    '                    ' データ登録
    '                    If NoteMealWorker.AddS(mainModel, inputModel) Then
    '                        ' 登録成功
    '                        'InputModelは覚えておく
    '                        inputModel.AttachedFileN = New List(Of AttachedFileItem)()
    '                        inputModel = mainModel.GetInputModelCache(Of NoteMealViewModel)().InputModel
    '                        inputModel.ItemName = String.Empty
    '                        inputModel.Calorie = String.Empty 'Short.MinValue
    '                        inputModel.PalString = String.Empty
    '                        inputModel.FoodN = New List(Of FoodItem)()

    '                        ' 表示対象のモデルを作成
    '                        Dim viewModel As NoteMealViewModel = NoteMealWorker.CreateViewModel(mainModel)

    '                        'モデルをキャッシュへ格納()
    '                        viewModel.InputModel = inputModel
    '                        mainModel.SetInputModelCache(viewModel)

    '                        'Home画面用のキャッシュをクリアしておく
    '                        mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

    '                        ' 「最近の食事」パーシャルビューを更新
    '                        Return PartialView("_NoteMealCardPartialView", viewModel)

    '                    Else
    '                        ' 登録失敗
    '                        Throw New InvalidOperationException("食事情報の登録に失敗しました。")

    '                        ' 「食事」画面へリダイレクト
    '                        Return RedirectToAction("Meal", "Note")
    '                    End If
    '                Else
    '                    ' 検証失敗

    '                    ' 独自にエラーメッセージを用意しビューに渡す
    '                    Dim errorMessage As New List(Of String)
    '                    For Each key As String In Me.ModelState.Keys
    '                        For Each e As ModelError In Me.ModelState(key).Errors
    '                            errorMessage.Add(e.ErrorMessage)
    '                        Next
    '                    Next

    '                    'Dim errorMessage As New Dictionary(Of String, String)()

    '                    'For Each key As String In Me.ModelState.Keys
    '                    '    For Each e As ModelError In Me.ModelState(key).Errors
    '                    '        errorMessage.Add(key, e.ErrorMessage)
    '                    '    Next
    '                    'Next

    '                    'Me.TempData("ErrorMessage") = errorMessage

    '                    ' 失敗ならJSONを返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = errorMessage,
    '                        .IsSuccess = Boolean.FalseString}.ToJsonResult()

    '                End If
    '        End Select

    '    End Function

    '    ''' <summary>
    '    ''' 「食事」画面　文字検索機能
    '    ''' </summary>
    '    ''' <param name="searchText"></param>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Search")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function MealResult(searchText As String) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

    '        Dim model As NoteMealViewModel = mainModel.GetInputModelCache(Of NoteMealViewModel)()
    '        'APIを実行
    '        Dim foodList As List(Of FoodItem) = NoteMealWorker.Search(searchText)

    '        If foodList.Count = 0 Then
    '            model.SearchedMaxPage = 0
    '        Else
    '            Dim pageList As New List(Of List(Of FoodItem))()
    '            Dim pageCount As Integer = 0

    '            pageCount = Integer.Parse(Math.Ceiling(foodList.Count / 20).ToString)

    '            For i As Integer = 0 To pageCount - 1
    '                Dim fList As New List(Of FoodItem)()

    '                For j As Integer = i * 20 To i * 20 + 19
    '                    fList.Add(foodList.Item(j))

    '                    If j = foodList.Count - 1 Then
    '                        Exit For
    '                    End If
    '                Next

    '                pageList.Add(fList)

    '            Next

    '            model.SearchedMealItemN = pageList
    '            model.SearchedMaxPage = pageCount

    '        End If

    '        ' パーシャル ビューを返却
    '        Return PartialView("_NoteMealSearchResultPartialView", model)


    '    End Function
    '    ''' <summary>
    '    ''' 「食事」画面　一覧フィルタ
    '    ''' </summary>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Filter")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function MealResult(Optional FilterDate As Date = Nothing) As ActionResult

    '        If IsNothing(FilterDate) Then
    '            FilterDate = Date.MinValue
    '        End If

    '        'Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim model As NoteMealViewModel = NoteMealWorker.Filter(Me.GetQolmsYappliModel(), FilterDate)

    '        ' パーシャル ビューを返却
    '        Return PartialView("_NoteMealCardPartialView", model)


    '    End Function
    '#End Region

    '#Region "「食事（2）」画面"

    '    <HttpGet()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal2() As ActionResult

    '        ' ビューを返却
    '        Return View(NoteMealWorker2.CreateViewModel(Me.GetQolmsYappliModel()))

    '    End Function

    '    '<HttpPost()>
    '    '<QyAjaxOnly()>
    '    '<QyAuthorize(True)>
    '    '<QyActionMethodSelector("Search")>
    '    '<QyApiAuthorize>
    '    '<QyLogging()>
    '    'Public Function Meal2Result(searchText As String) As ActionResult

    '    '    ' パーシャル ビューを返却
    '    '    Return PartialView("_NoteMealSearchResultPartialView", NoteMealWorker2.Search(Me.GetQolmsYappliModel(), searchText))

    '    'End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Search")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal2Result(searchText As String, Optional dummy As String = Nothing) As ActionResult

    '        ' パーシャル ビューを返却
    '        Return PartialView("_NoteMealSearchAreaPartialView", NoteMealWorker2.Search(Me.GetQolmsYappliModel(), searchText))

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("AddPhoto, AddNextPhoto")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal2Result(model As NoteMealInputModel, photoData As String, photoName As String) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel = NoteMealWorker2.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If Me.ModelState.IsValid Then
    '            ' 検証に成功
    '            Dim data() As Byte = Nothing
    '            Dim name As String = String.Empty
    '            Dim message As String = String.Empty

    '            If NoteMealWorker2.ToPhotoData(photoData, photoName, data, name, message) Then
    '                ' 画像ファイルが有効

    '                ' 登録
    '                If NoteMealWorker2.AddPhoto(mainModel, model, data, name) Then
    '                    ' 登録に成功

    '                    ' 「最近の食事」パーシャル ビューを返却
    '                    Return PartialView("_NoteMealCardPartialView", NoteMealWorker2.Filter(mainModel, Date.MinValue, True))
    '                Else
    '                    ' 登録に失敗（エラー）
    '                    Throw New InvalidOperationException("「食事」の登録に失敗しました。")
    '                End If
    '            Else
    '                ' 画像ファイルが無効（エラー）

    '                ' JSON を返却
    '                Return New NoteEditJsonResult() With {
    '                    .Messages = {message}.ToList(),
    '                    .IsSuccess = Boolean.FalseString
    '                }.ToJsonResult()
    '            End If
    '        Else
    '            ' 検証に失敗（エラー）
    '            Dim errorMessage As New List(Of String)()

    '            For Each key As String In Me.ModelState.Keys
    '                For Each e As ModelError In Me.ModelState(key).Errors
    '                    errorMessage.Add(e.ErrorMessage)
    '                Next
    '            Next

    '            ' JSON を返却
    '            Return New NoteEditJsonResult() With {
    '                .Messages = errorMessage,
    '                .IsSuccess = Boolean.FalseString
    '            }.ToJsonResult()
    '        End If

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("AddMenu, AddNextMenu")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal2Result(model As NoteMealInputModel, photoData As String, photoName As String, Optional dummy As String = Nothing) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel = NoteMealWorker2.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If Me.ModelState.IsValid Then
    '            ' 検証に成功
    '            Dim data() As Byte = Nothing
    '            Dim name As String = String.Empty
    '            Dim message As String = String.Empty

    '            ' 画像ファイルがある場合はチェック
    '            If Not String.IsNullOrWhiteSpace(photoData) AndAlso Not String.IsNullOrWhiteSpace(photoName) Then
    '                If Not NoteMealWorker2.ToPhotoData(photoData, photoName, data, name, message) Then
    '                    ' 画像ファイルが無効（エラー）

    '                    ' JSON を返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = {message}.ToList(),
    '                        .IsSuccess = Boolean.FalseString
    '                    }.ToJsonResult()
    '                End If
    '            End If

    '            ' 登録
    '            If NoteMealWorker2.AddMenu(mainModel, model, data, name) Then
    '                ' 登録に成功

    '                ' 「最近の食事」パーシャル ビューを返却
    '                Return PartialView("_NoteMealCardPartialView", NoteMealWorker2.Filter(mainModel, Date.MinValue, True))
    '            Else
    '                ' 登録に失敗（エラー）
    '                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
    '            End If
    '        Else
    '            ' 検証に失敗
    '            Dim errorMessage As New List(Of String)()

    '            For Each key As String In Me.ModelState.Keys
    '                For Each e As ModelError In Me.ModelState(key).Errors
    '                    errorMessage.Add(e.ErrorMessage)
    '                Next
    '            Next

    '            ' JSON を返却
    '            Return New NoteEditJsonResult() With {
    '                .Messages = errorMessage,
    '                .IsSuccess = Boolean.FalseString
    '            }.ToJsonResult()
    '        End If

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Edit")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal2Result(model As NoteMealInputModel, photoData As String, photoName As String, Optional dummy1 As String = Nothing, Optional dummy2 As String = Nothing) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel = NoteMealWorker2.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If Me.ModelState.IsValid Then
    '            ' 検証に成功
    '            Dim data() As Byte = Nothing
    '            Dim name As String = String.Empty
    '            Dim message As String = String.Empty

    '            ' 画像ファイルがある場合はチェック
    '            If Not String.IsNullOrWhiteSpace(photoData) AndAlso Not String.IsNullOrWhiteSpace(photoName) Then
    '                If Not NoteMealWorker2.ToPhotoData(photoData, photoName, data, name, message) Then
    '                    ' 画像ファイルが無効（エラー）

    '                    ' JSON を返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = {message}.ToList(),
    '                        .IsSuccess = Boolean.FalseString
    '                    }.ToJsonResult()
    '                End If
    '            End If

    '            ' 登録
    '            If NoteMealWorker2.Edit(mainModel, model, data, name) Then
    '                ' 登録に成功

    '                ' 「最近の食事」パーシャル ビューを返却
    '                Return PartialView("_NoteMealCardPartialView", NoteMealWorker2.Filter(mainModel, Date.MinValue, True))
    '            Else
    '                ' 登録に失敗（エラー）
    '                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
    '            End If
    '        Else
    '            ' 検証に失敗
    '            Dim errorMessage As New List(Of String)()

    '            For Each key As String In Me.ModelState.Keys
    '                For Each e As ModelError In Me.ModelState(key).Errors
    '                    errorMessage.Add(e.ErrorMessage)
    '                Next
    '            Next

    '            ' JSON を返却
    '            Return New NoteEditJsonResult() With {
    '                .Messages = errorMessage,
    '                .IsSuccess = Boolean.FalseString
    '            }.ToJsonResult()
    '        End If

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Delete")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal2Result(recordDate As Date, mealType As Byte, sequence As Integer) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel = NoteMealWorker2.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If NoteMealWorker.Delete(mainModel, recordDate, mealType, sequence) Then
    '            ' 削除成功

    '            ' 「最近の食事」パーシャル ビューを返却
    '            Return PartialView("_NoteMealCardPartialView", NoteMealWorker2.Filter(mainModel, Date.MinValue, True))
    '        End If

    '        ' 登録失敗
    '        Throw New InvalidOperationException("「食事」の削除に失敗しました。")

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Filter")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal2Result(filterDate As Nullable(Of Date)) As ActionResult

    '        ' パーシャル ビューを返却
    '        Return PartialView("_NoteMealCardPartialView", NoteMealWorker2.Filter(Me.GetQolmsYappliModel(), If(filterDate.HasValue, filterDate.Value, Date.MinValue)))

    '    End Function

    '#End Region

    '#Region "「食事（3）」画面"

    '    <HttpGet()>
    '    <QyAuthorize()>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal3() As ActionResult

    '        ' ビューを返却
    '        Return View(NoteMealWorker3.CreateViewModel(Me.GetQolmsYappliModel()))

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Search")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal3Result(searchText As String, Optional dummy As String = Nothing) As ActionResult

    '        ' パーシャル ビューを返却
    '        Return PartialView("_NoteMealSearchAreaPartialView", NoteMealWorker3.Search(Me.GetQolmsYappliModel(), searchText))

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("AddPhoto, AddNextPhoto")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal3Result(model As NoteMealInputModel, photoData As String, photoName As String) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel3 = NoteMealWorker3.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If Me.ModelState.IsValid Then
    '            ' 検証に成功
    '            Dim data() As Byte = Nothing
    '            Dim name As String = String.Empty
    '            Dim message As String = String.Empty

    '            If NoteMealWorker3.ToPhotoData(photoData, photoName, data, name, message) Then
    '                ' 画像ファイルが有効

    '                ' 登録
    '                If NoteMealWorker3.AddPhoto(mainModel, model, data, name) Then
    '                    ' 登録に成功

    '                    ' 「最近の食事」パーシャル ビューを返却
    '                    Return PartialView("_NoteMealCardAreaPartialView", NoteMealWorker3.Filter(mainModel, Date.MinValue, True))
    '                Else
    '                    ' 登録に失敗（エラー）
    '                    Throw New InvalidOperationException("「食事」の登録に失敗しました。")
    '                End If
    '            Else
    '                ' 画像ファイルが無効（エラー）

    '                ' JSON を返却
    '                Return New NoteEditJsonResult() With {
    '                    .Messages = {message}.ToList(),
    '                    .IsSuccess = Boolean.FalseString
    '                }.ToJsonResult()
    '            End If
    '        Else
    '            ' 検証に失敗（エラー）
    '            Dim errorMessage As New List(Of String)()

    '            For Each key As String In Me.ModelState.Keys
    '                For Each e As ModelError In Me.ModelState(key).Errors
    '                    errorMessage.Add(e.ErrorMessage)
    '                Next
    '            Next

    '            ' JSON を返却
    '            Return New NoteEditJsonResult() With {
    '                .Messages = errorMessage,
    '                .IsSuccess = Boolean.FalseString
    '            }.ToJsonResult()
    '        End If

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("AddMenu, AddNextMenu")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal3Result(model As NoteMealInputModel, photoData As String, photoName As String, Optional dummy As String = Nothing) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel3 = NoteMealWorker3.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If Me.ModelState.IsValid Then
    '            ' 検証に成功
    '            Dim data() As Byte = Nothing
    '            Dim name As String = String.Empty
    '            Dim message As String = String.Empty

    '            ' 画像ファイルがある場合はチェック
    '            If Not String.IsNullOrWhiteSpace(photoData) AndAlso Not String.IsNullOrWhiteSpace(photoName) Then
    '                If Not NoteMealWorker3.ToPhotoData(photoData, photoName, data, name, message) Then
    '                    ' 画像ファイルが無効（エラー）

    '                    ' JSON を返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = {message}.ToList(),
    '                        .IsSuccess = Boolean.FalseString
    '                    }.ToJsonResult()
    '                End If
    '            End If

    '            ' 登録
    '            If NoteMealWorker3.AddMenu(mainModel, model, data, name) Then
    '                ' 登録に成功

    '                ' 「最近の食事」パーシャル ビューを返却
    '                Return PartialView("_NoteMealCardAreaPartialView", NoteMealWorker3.Filter(mainModel, Date.MinValue, True))
    '            Else
    '                ' 登録に失敗（エラー）
    '                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
    '            End If
    '        Else
    '            ' 検証に失敗
    '            Dim errorMessage As New List(Of String)()

    '            For Each key As String In Me.ModelState.Keys
    '                For Each e As ModelError In Me.ModelState(key).Errors
    '                    errorMessage.Add(e.ErrorMessage)
    '                Next
    '            Next

    '            ' JSON を返却
    '            Return New NoteEditJsonResult() With {
    '                .Messages = errorMessage,
    '                .IsSuccess = Boolean.FalseString
    '            }.ToJsonResult()
    '        End If

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Edit")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal3Result(model As NoteMealInputModel, photoData As String, photoName As String, Optional dummy1 As String = Nothing, Optional dummy2 As String = Nothing) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel3 = NoteMealWorker3.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If Me.ModelState.IsValid Then
    '            ' 検証に成功
    '            Dim data() As Byte = Nothing
    '            Dim name As String = String.Empty
    '            Dim message As String = String.Empty

    '            ' 画像ファイルがある場合はチェック
    '            If Not String.IsNullOrWhiteSpace(photoData) AndAlso Not String.IsNullOrWhiteSpace(photoName) Then
    '                If Not NoteMealWorker3.ToPhotoData(photoData, photoName, data, name, message) Then
    '                    ' 画像ファイルが無効（エラー）

    '                    ' JSON を返却
    '                    Return New NoteEditJsonResult() With {
    '                        .Messages = {message}.ToList(),
    '                        .IsSuccess = Boolean.FalseString
    '                    }.ToJsonResult()
    '                End If
    '            End If

    '            ' 登録
    '            If NoteMealWorker3.Edit(mainModel, model, data, name) Then
    '                ' 登録に成功

    '                ' 「最近の食事」パーシャル ビューを返却
    '                Return PartialView("_NoteMealCardAreaPartialView", NoteMealWorker3.Filter(mainModel, Date.MinValue, True))
    '            Else
    '                ' 登録に失敗（エラー）
    '                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
    '            End If
    '        Else
    '            ' 検証に失敗
    '            Dim errorMessage As New List(Of String)()

    '            For Each key As String In Me.ModelState.Keys
    '                For Each e As ModelError In Me.ModelState(key).Errors
    '                    errorMessage.Add(e.ErrorMessage)
    '                Next
    '            Next

    '            ' JSON を返却
    '            Return New NoteEditJsonResult() With {
    '                .Messages = errorMessage,
    '                .IsSuccess = Boolean.FalseString
    '            }.ToJsonResult()
    '        End If

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Delete")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal3Result(recordDate As Date, mealType As Byte, sequence As Integer) As ActionResult

    '        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
    '        Dim viewModel As NoteMealViewModel3 = NoteMealWorker3.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

    '        If NoteMealWorker3.Delete(mainModel, recordDate, mealType, sequence) Then
    '            ' 削除成功

    '            ' 「最近の食事」パーシャル ビューを返却
    '            Return PartialView("_NoteMealCardAreaPartialView", NoteMealWorker3.Filter(mainModel, Date.MinValue, True))
    '        End If

    '        ' 登録失敗
    '        Throw New InvalidOperationException("「食事」の削除に失敗しました。")

    '    End Function

    '    <HttpPost()>
    '    <QyAjaxOnly()>
    '    <QyAuthorize(True)>
    '    <QyActionMethodSelector("Filter")>
    '    <QyApiAuthorize>
    '    <QyLogging()>
    '    Public Function Meal3Result(filterDate As Nullable(Of Date)) As ActionResult

    '        ' パーシャル ビューを返却
    '        Return PartialView("_NoteMealCardAreaPartialView", NoteMealWorker3.Filter(Me.GetQolmsYappliModel(), If(filterDate.HasValue, filterDate.Value, Date.MinValue)))

    '    End Function

    '    ' TODO:

    '#End Region

#Region "「食事（4）」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4(Optional meal As Byte = Byte.MinValue, Optional selectdate As String = "") As ActionResult

        '食事種別
        Dim mealType As QyMealTypeEnum = QyMealTypeEnum.None
        If meal > 0 Then
            mealType = DirectCast(meal, QyMealTypeEnum)
        End If

        '日付
        Dim recordDate As Date = Date.MinValue
        If Not String.IsNullOrWhiteSpace(selectdate) AndAlso selectdate.Length = 8 Then

            Dim tryRecordDate As Date = Date.MinValue
            Dim ci As CultureInfo = CultureInfo.CurrentCulture
            Dim dts As DateTimeStyles = DateTimeStyles.None
            If Date.TryParseExact(selectdate, "yyyyMMdd", ci, dts, tryRecordDate) AndAlso tryRecordDate < Date.Now.Date Then
                recordDate = tryRecordDate
            End If
        End If

        'モデルを削除
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        mainModel.RemoveInputModelCache(Of NoteMealViewModel4)()

        ' ビューを返却
        Return View(NoteMealWorker4.CreateViewModel(Me.GetQolmsYappliModel(), mealType, recordDate))

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result() As ActionResult

        ' ビューを返却
        Return RedirectToAction("Meal4", "Note")

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Search")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(searchText As String, model As NoteMealListInputModel, PageType As String) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        Dim inputModel As New NoteMealListInputModel()

        If Me.ModelState.IsValid Then
            ' 検証に成功
            '写真から登録 または編集 の場合は現在の画面用入力モデルをキャッシュから取得
            If PageType = "Photo" OrElse PageType = "Edit" Then
                inputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()
                inputModel.SearchFlag = True
            End If

            '日時の更新を取得
            inputModel.InputNext = model.InputNext
            inputModel.MealType = model.MealType
            inputModel.RecordDate = model.RecordDate
            inputModel.Meridiem = model.Meridiem
            inputModel.Hour = model.Hour
            inputModel.Minute = model.Minute
            inputModel.PhotoData = model.PhotoData
            inputModel.PhotoName = model.PhotoName

            'モデルをキャッシュへ格納
            mainModel.SetInputModelCache(inputModel)

            ' 表示対象のモデルを作成
            Dim viewModel As NoteMealSearchResultViewModel = NoteMealWorker4.Search(Me.GetQolmsYappliModel(), searchText)
            viewModel.SearchText = searchText
            viewModel.PageType = PageType

            'モデルをキャッシュへ格納
            mainModel.SetInputModelCache(viewModel)

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()
        Else
            ' 検証に失敗（エラー）
            Dim errorMessage As New List(Of String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("SearchAgain")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(searchText As String, pageType As String) As JsonResult

        Try
            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

            ' 表示対象のモデルを作成
            Dim viewModel As NoteMealSearchResultViewModel = NoteMealWorker4.Search(Me.GetQolmsYappliModel(), searchText)
            viewModel.SearchText = searchText
            viewModel.PageType = pageType

            'モデルをキャッシュへ格納
            mainModel.SetInputModelCache(viewModel)

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()
        Catch ex As Exception

            Dim errorMessage As New List(Of String)
            errorMessage.Add(ex.Message)

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End Try

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("ShowMore")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(page As Integer) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        'モデルをキャッシュから取得
        Dim viewModel As NoteMealSearchResultViewModel = mainModel.GetInputModelCache(Of NoteMealSearchResultViewModel)()
        '次のページを取得
        Dim foodItemList As List(Of FoodItem) = NoteMealWorker4.GetFoodItemList(Me.GetQolmsYappliModel(), viewModel.SearchText, page + 1)
        viewModel.DispedPage = page + 1
        viewModel.SearchedMealItemN = foodItemList
        Dim partialViewModel As New NoteMealSearchResultAreaPartialViewModel(viewModel)

        'モデルをキャッシュへ格納
        mainModel.SetInputModelCache(viewModel)

        'ビューを返却
        Return View("_NoteMealSearchResultAreaPartialView", partialViewModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("SearchResult")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(Item As String, Calorie As String, OtherParameter As String) As JsonResult

        Dim message As New List(Of String)

        Try
            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
            'モデルをキャッシュから取得
            Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()
            Dim viewModel As NoteMealSearchResultViewModel = mainModel.GetInputModelCache(Of NoteMealSearchResultViewModel)()

            If inputModel.ItemName Is Nothing Then
                inputModel.ItemName = New List(Of String)
            Else
                inputModel.ItemName.Clear()
            End If

            If inputModel.Calorie Is Nothing Then
                inputModel.Calorie = New List(Of String)
            Else
                inputModel.Calorie.Clear()
            End If

            If inputModel.PalString Is Nothing Then
                inputModel.PalString = New List(Of String)
            Else
                inputModel.PalString.Clear()
            End If

            inputModel.ItemName.Add(Item)
            inputModel.Calorie.Add(Calorie)
            inputModel.PalString.Add(OtherParameter)

            'モデルをキャッシュへ格納
            mainModel.SetInputModelCache(inputModel)

            'ビューを返却
            If viewModel.PageType = "Search" Then
                message.Add("MealRegisterFromSearch")
            ElseIf viewModel.PageType = "Edit" Then
                message.Add("MealEdit")
            Else
                message.Add("MealRegisterFromPhoto")
            End If

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = message.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()
        Catch ex As Exception
            message.Add(ex.Message)

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = message.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End Try

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("History")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(recordDate As Date, mealType As QyMealTypeEnum, meridiem As String, hour As Integer, minute As Integer) As JsonResult

        Try
            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
            Dim viewModel As NoteMealViewModel4 = NoteMealWorker4.CreateViewModel(mainModel)

            '日時等を取得
            If meridiem = "pm" Then
                hour += 12
            End If
            Dim setDate As New Date(recordDate.Year, recordDate.Month, recordDate.Day, hour, minute, 0)
            viewModel.RecordDate = setDate
            viewModel.MealType = mealType

            'モデルをキャッシュへ格納
            mainModel.SetInputModelCache(viewModel)

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        Catch ex As Exception
            ' JSON を返却
            Dim errorMessage As New List(Of String)()
            errorMessage.Add(ex.Message)

            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End Try

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Analysis")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(model As NoteMealListInputModel, photoData As String, photoName As String, Optional dummy As String = Nothing) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

        If Me.ModelState.IsValid Then
            ' 検証に成功
            Dim data() As Byte = Nothing
            Dim name As String = String.Empty
            Dim message As String = String.Empty

            If NoteMealWorker4.ToPhotoData(photoData, photoName, data, name, message) Then
                ' 画像ファイルが有効
                Dim inputModel As NoteMealListInputModel = NoteMealWorker4.AnalyzePhoto(mainModel, model, data, name)
                inputModel.InputNext = model.InputNext
                inputModel.RecordDate = model.RecordDate
                inputModel.MealType = model.MealType
                inputModel.Meridiem = model.Meridiem
                inputModel.Hour = model.Hour
                inputModel.Minute = model.Minute

                '検索画面用の入力モデルキャッシュ格納
                mainModel.SetInputModelCache(inputModel)

                ' JSON を返却
                Return New NoteEditJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()

            Else
                ' 画像ファイルが無効（エラー）

                ' JSON を返却
                Return New NoteEditJsonResult() With {
                    .Messages = {message}.ToList().ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                    .IsSuccess = Boolean.FalseString
                }.ToJsonResult()
            End If
        Else
            ' 検証に失敗（エラー）
            Dim errorMessage As New List(Of String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Edit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(model As NoteMealListInputModel) As JsonResult

        If Me.ModelState.IsValid Then
            ' PalStringをFoodNに変換
            Const COUNT As Integer = 8

            model.FoodN.Clear()

            Dim foodN As New List(Of FoodItem)()

            For Each palStr As String In model.PalString
                Dim pals() As String = palStr.Split({","}, StringSplitOptions.None)
                For i As Integer = 0 To pals.Count - 1 Step COUNT
                    Dim pal() As String = pals.Skip(i).Take(COUNT).ToArray()
                    If pal.Length < COUNT Then
                        Exit For
                    End If
                    foodN.Add(
                                New FoodItem() With {
                                    .label = pal(0),
                                    .calorie = pal(1),
                                    .protein = pal(2),
                                    .lipid = pal(3),
                                    .carbohydrate = pal(4),
                                    .salt_amount = pal(5),
                                    .available_carbohydrate = pal(6),
                                    .fiber = pal(7)
                                }
                            )
                Next
                model.FoodN.Add(foodN)
            Next

            ' ページ単位へ分割
            Dim pageList As New List(Of List(Of FoodItem))()
            Dim pageCount As Integer = 0
            Dim dispedPage As Integer = 0

            If foodN.Any() Then

                pageCount = Convert.ToInt32(Math.Ceiling(foodN.Count / 10D))

                For i As Integer = 0 To pageCount - 1
                    Dim fList As New List(Of FoodItem)()

                    For j As Integer = i * 10 To i * 10 + 9
                        fList.Add(foodN.Item(j))

                        If j = foodN.Count - 1 Then
                            Exit For
                        End If
                    Next

                    pageList.Add(fList)
                Next

                dispedPage = 1
            End If

            model.FoodN = pageList
            model.SearchedMaxPage = pageCount
            model.DispedPage = dispedPage

            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
            Dim inputModel As NoteMealListInputModel = New NoteMealListInputModel()

            'InputModelに反映
            inputModel.UpdateByInput(model)

            'モデルをキャッシュへ格納
            mainModel.SetInputModelCache(inputModel)

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()

        Else
            ' 検証に失敗（エラー）
            Dim errorMessage As New List(Of String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Delete")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(recordDate As Date, mealType As Byte, sequence As Integer) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

        If NoteMealWorker4.Delete(mainModel, recordDate, mealType, sequence) Then
            ' 削除成功

            ' 「最近の食事」パーシャル ビューを返却
            Return PartialView("_NoteMealCardAreaPartialView4", NoteMealWorker4.Filter(mainModel, Date.MinValue, True))
        End If

        ' 登録失敗
        Throw New InvalidOperationException("「食事」の削除に失敗しました。")

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Filter")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function Meal4Result(filterDate As Nullable(Of Date)) As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealCardAreaPartialView4", NoteMealWorker4.Filter(Me.GetQolmsYappliModel(), If(filterDate.HasValue, filterDate.Value, Date.MinValue)))

    End Function

#End Region

#Region "「食事（4）検索結果」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealSearchResult() As ActionResult

        'モデルをキャッシュから取得
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteMealSearchResultViewModel = mainModel.GetInputModelCache(Of NoteMealSearchResultViewModel)()

        ' ビューを返却
        Return View("MealSearchResult", viewModel)

    End Function

#End Region

#Region "「食事（4）検索から登録」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromSearch() As ActionResult

        'モデルをキャッシュから取得
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()

        ' ビューを返却
        Return View("MealRegisterFromSearch", inputModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Add, AddNext")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromSearchResult(model As NoteMealListInputModel, photoData As String, photoName As String, Optional dummy As String = Nothing) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("メインモデル取得"))
        Dim viewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策
        'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("キャッシュ取得"))

        If Me.ModelState.IsValid Then
            ' 検証に成功
            'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("検証に成功"))

            Dim data() As Byte = Nothing
            Dim name As String = String.Empty
            Dim message As String = String.Empty

            ' 画像ファイルがある場合はチェック
            If Not String.IsNullOrWhiteSpace(photoData) AndAlso Not String.IsNullOrWhiteSpace(photoName) Then
                If Not NoteMealWorker4.ToPhotoData(photoData, photoName, data, name, message) Then
                    ' 画像ファイルが無効（エラー）
                    'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("画像ファイルが無効"))

                    ' JSON を返却
                    Return New NoteEditJsonResult() With {
                        .Messages = {message}.ToList().ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                        .IsSuccess = Boolean.FalseString
                    }.ToJsonResult()
                End If
            End If

            ' 登録
            If NoteMealWorker4.AddSearch(mainModel, model, data, name) Then
                ' 登録に成功

                ' InputModelをキャッシュから削除
                mainModel.RemoveInputModelCache(Of NoteMealListInputModel)()

                'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("登録に成功"))
                ' JSON を返却
                Return New NoteEditJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()
            Else
                ' 登録に失敗（エラー）
                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
            End If
        Else
            ' 検証に失敗
            'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("検証に失敗"))

            Dim errorMessage As New List(Of String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End If

    End Function

#End Region

#Region "「食事（4）写真から登録」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromPhoto() As ActionResult

        'モデルをキャッシュから取得
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()

        ' ビューを返却
        Return View("MealRegisterFromPhoto", inputModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("ShowMore")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromPhotoResult(page As Integer) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        'モデルをキャッシュから取得
        Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()

        inputModel.DispedPage = page + 1

        'ビューを返却
        Return View("_NoteMealAnalyzeResultAreaPartialView", New NoteMealAnalyzeResultPartialViewModel(inputModel))

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Add, AddNext")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromPhotoResult(model As NoteMealListInputModel) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("メインモデル取得"))
        Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()
        'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("キャッシュからデータ取得"))

        If Me.ModelState.IsValid Then
            ' 検証に成功
            'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("検証に成功"))

            model.ForeignKey = inputModel.ForeignKey

            model.FoodN.Add(New List(Of FoodItem))
            For i As Integer = 0 To inputModel.FoodN.Count - 1
                For Each food As FoodItem In inputModel.FoodN(i)
                    model.FoodN(0).Add(food)
                Next
            Next

            ' 登録
            If NoteMealWorker4.AddPhoto(mainModel, model) Then
                ' 登録に成功
                'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("登録に成功"))

                ' InputModelをキャッシュから削除
                mainModel.RemoveInputModelCache(Of NoteMealListInputModel)()

                ' JSON を返却
                Return New NoteEditJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()
            Else
                ' 登録に失敗（エラー）
                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
            End If
        Else
            ' 検証に失敗
            'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("検証に失敗"))
            Dim errorMessage As New List(Of String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End If

    End Function

#End Region

#Region "「食事（4）履歴から登録」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromHistory() As ActionResult

        'モデルをキャッシュから取得
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteMealViewModel4 = mainModel.GetInputModelCache(Of NoteMealViewModel4)()

        ' ビューを返却
        Return View("MealRegisterFromHistory", viewModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Add")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromHistoryResult(model As NoteMealListInputModel) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

        'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("メインモデル取得"))
        If Me.ModelState.IsValid Then
            ' 検証に成功
            'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("検証に成功"))

            ' 登録
            If NoteMealWorker4.AddHistory(mainModel, model) Then
                ' 登録に成功
                'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("登録成功"))

                ' JSON を返却
                Return New NoteEditJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()
            Else

                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
            End If
        Else

            ' 検証に失敗
            'AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("検証に失敗"))
            Dim errorMessage As New List(Of String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Filter")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealRegisterFromHistoryResult(filterDate As Nullable(Of Date)) As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealRegisterFromHistoryCardAreaPartialView", NoteMealWorker4.Filter(Me.GetQolmsYappliModel(), If(filterDate.HasValue, filterDate.Value, Date.MinValue)))

    End Function

#End Region

#Region "「食事（4）編集」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealEdit() As ActionResult

        'モデルをキャッシュから取得
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()

        ' ビューを返却
        Return View("MealEdit", inputModel)

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("ShowMore")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealEditResult(page As Integer) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        'モデルをキャッシュから取得
        Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()

        inputModel.DispedPage = page + 1

        'ビューを返却
        Return View("_NoteMealAnalyzeResultAreaPartialView", New NoteMealAnalyzeResultPartialViewModel(inputModel))

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Edit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function MealEditResult(model As NoteMealListInputModel) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel) ' セッション タイム アウト時の再ログイン後対策

        If Me.ModelState.IsValid Then
            ' 検証に成功

            ' キャッシュから取得
            Dim inputModel As NoteMealListInputModel = mainModel.GetInputModelCache(Of NoteMealListInputModel)()
            model.BeforeRecordDate = inputModel.BeforeRecordDate
            model.BeforeMealType = inputModel.BeforeMealType
            model.Sequence = inputModel.Sequence

            Dim data() As Byte = Nothing
            Dim name As String = String.Empty
            Dim message As String = String.Empty

            ' 画像ファイルがある場合はチェック
            If Not String.IsNullOrWhiteSpace(model.PhotoData) AndAlso Not String.IsNullOrWhiteSpace(model.PhotoName) Then
                If Not NoteMealWorker4.ToPhotoData(model.PhotoData, model.PhotoName, data, name, message) Then
                    ' 画像ファイルが無効（エラー）

                    ' JSON を返却
                    Return New NoteEditJsonResult() With {
                        .Messages = {message}.ToList().ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                        .IsSuccess = Boolean.FalseString
                    }.ToJsonResult()
                End If
            End If

            ' 登録
            If NoteMealWorker4.Edit(mainModel, model, data, name) Then
                ' 登録に成功

                ' JSON を返却
                Return New NoteEditJsonResult() With {
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()
            Else
                ' 登録に失敗（エラー）
                Throw New InvalidOperationException("「食事」の登録に失敗しました。")
            End If
        Else
            ' 検証に失敗
            Dim errorMessage As New List(Of String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    errorMessage.Add(e.ErrorMessage)
                Next
            Next

            ' JSON を返却
            Return New NoteEditJsonResult() With {
                .Messages = errorMessage.ConvertAll(Function(i) HttpUtility.HtmlEncode(i)),
                .IsSuccess = Boolean.FalseString
            }.ToJsonResult()
        End If

    End Function

#End Region

#Region "「バイタル」画面"

    ''' <summary>
    ''' 「バイタル」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Vital(Optional tabNo As Byte = Byte.MinValue, Optional selectdate As String = "") As ActionResult

        Dim model As NoteVitalViewModel = NoteVitalWorker.CreateViewModel(Me.GetQolmsYappliModel())

        'タブ
        If 1 <= tabNo AndAlso tabNo <= 3 Then
            model.Tab = tabNo
        Else
            model.Tab = 1
        End If

        '日付
        Dim recordDate As Date = Date.MinValue
        If Not String.IsNullOrWhiteSpace(selectdate) AndAlso selectdate.Length = 8 Then

            Dim tryRecordDate As Date = Date.MinValue
            Dim now As Date = Date.Now
            Dim ci As CultureInfo = CultureInfo.CurrentCulture
            Dim dts As DateTimeStyles = DateTimeStyles.None
            If Date.TryParseExact(selectdate, "yyyyMMdd", ci, dts, tryRecordDate) AndAlso tryRecordDate < Date.Now.Date Then
                model.RecordDate =
                New Date(tryRecordDate.Year, tryRecordDate.Month, tryRecordDate.Day, now.Hour, now.Minute, 0)
            End If
        End If

        ' ビュー を返却
        Return View(model)

    End Function

    ''' <summary>
    ''' 「バイタル」画面の「グラフ」の表示要求を処理します。
    ''' </summary>
    ''' <param name="vitalType">バイタル 情報の種別。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Graph")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function VitalResult(vitalType As QyVitalTypeEnum) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteVitalViewModel = NoteVitalWorker.GetViewModelCache(mainModel, False) ' mainModel.GetInputModelCache(Of NoteVitalViewModel)()
        Dim partialViewName As String = String.Empty
        Dim partialViewModel As QyVitalGraphPartialViewModelBase = NoteVitalWorker.CreateGraphPartialViewModel(mainModel, pageViewModel, vitalType, partialViewName, True) ' TODO: 暫定、グラフ の情報を強制的に再取得

        ' パーシャル ビュー を返却
        Return PartialView(partialViewName, partialViewModel)

    End Function

    ''' <summary>
    ''' 「バイタル」画面の「詳細」の表示要求を処理します。
    ''' </summary>
    ''' <param name="vitalType">バイタル 情報の種別。</param>
    ''' <param name="recordDate">測定日。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Detail")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function VitalResult(vitalType As QyVitalTypeEnum, recordDate As Date) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteVitalViewModel = NoteVitalWorker.GetViewModelCache(mainModel) ' セッション タイム アウト 時の再 ログイン 後対策 mainModel.GetInputModelCache(Of NoteVitalViewModel)()
        Dim partialViewName As String = String.Empty

        NoteVitalWorker.CreateGraphPartialViewModel(mainModel, pageViewModel, vitalType, partialViewName, True) ' TODO: 暫定、グラフ の情報を強制的に再取得

        Dim partialViewModel As QyVitalDetailPartialViewModelBase = NoteVitalWorker.CreateDetailPartialViewModel(mainModel, pageViewModel, vitalType, recordDate, partialViewName, True) ' TODO: 暫定、詳細情報を強制的に再取得

        ' パーシャル ビュー を返却
        Return PartialView(partialViewName, partialViewModel)

    End Function

    ''' <summary>
    ''' 「バイタル」画面の歩数の削除要求を処理します。
    ''' </summary>
    ''' <param name="vitalType">バイタル 情報の種別。</param>
    ''' <param name="reference">
    ''' 削除対象の日時の リスト。
    ''' </param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Delete")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function VitalResult(vitalType As QyVitalTypeEnum, reference As List(Of Date)) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteVitalViewModel = NoteVitalWorker.GetViewModelCache(mainModel) ' セッション タイム アウト 時の再 ログイン 後対策 mainModel.GetInputModelCache(Of NoteVitalViewModel)()

        ' JSON を返却
        Return NoteVitalWorker.Delete(mainModel, pageViewModel, vitalType, reference).ToJsonResult()

    End Function

    ''' <summary>
    ''' 「バイタル」画面の登録要求を処理します。
    ''' </summary>
    ''' <param name="model">「バイタル」画面 インプット モデル。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Edit")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function VitalResult(model As NoteVitalEditInputModel) As JsonResult

        Dim result As New NoteVitalEditJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .VitalTypeN = New List(Of String)(),
            .Messages = New List(Of String)(),
            .Height = String.Empty
        }

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim pageViewModel As NoteVitalViewModel = NoteVitalWorker.GetViewModelCache(mainModel) ' セッション タイム アウト 時の再 ログイン 後対策 mainModel.GetInputModelCache(Of NoteVitalViewModel)()

        ' モデル の検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功

            ' 登録処理
            Dim vitalTypeN As New List(Of QyVitalTypeEnum)()
            Dim messageN As New List(Of String)()
            Dim height As Decimal = Decimal.MinValue

            If NoteVitalWorker.Edit(mainModel, pageViewModel, model, vitalTypeN, messageN, height) Then
                ' 登録成功
                result.VitalTypeN.AddRange(vitalTypeN.ConvertAll(Function(i) i.ToString()))
                If height > Decimal.Zero Then result.Height = height.ToString("0.####")
                result.IsSuccess = Boolean.TrueString
            Else
                ' 登録失敗
                result.Messages.AddRange(messageN)
            End If
        Else
            ' 検証失敗
            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    result.Messages.Add(e.ErrorMessage)
                Next
            Next
        End If

        ' JSON を返却
        Return result.ToJsonResultWithSanitize()

    End Function

    ''' <summary>
    '''  「バイタル」画面の「タニタ 会員 QR コード 情報」の表示要求を処理します。
    ''' </summary>
    ''' <param name="reference">タニタ 会員 QR コード 情報への参照 パラメータ。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function VitalTanitaQr(reference As String) As JsonResult

        Dim result As New NoteVitalTanitaQrJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .QrCode = String.Empty,
            .Experies = String.Empty
        }
        Dim jsonObject As NoteVitalTanitaQrReferenceJsonParameter = Me.DecryptReference(Of NoteVitalTanitaQrReferenceJsonParameter)(reference) ' 参照 パラメータ を復号化
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        If jsonObject IsNot Nothing Then
            Dim accountkey As Guid = jsonObject.Accountkey.TryToValueType(Guid.Empty)
            Dim loginAt As Date = jsonObject.LoginAt.TryToValueType(Date.MinValue)
            Dim memberNo As String = jsonObject.MemberNo

            If accountkey = mainModel.AuthorAccount.AccountKey _
                AndAlso loginAt = mainModel.AuthorAccount.LoginAt _
                AndAlso Not String.IsNullOrWhiteSpace(memberNo) Then

                ' タニタ の API から QR コード 情報を取得
                result = NoteVitalWorker.GetTanitaQrCode(mainModel, memberNo)
            End If
        End If

        Return result.ToJsonResult()

    End Function

#End Region

#Region "「ガルフ スポーツ 動画」画面"

    ''' <summary>
    ''' 「ガルフ スポーツ トップ」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function GulfSportsMovieIndex() As ActionResult

        ' ビュー を返却
        Return View()

    End Function

    ''' <summary>
    ''' 「ガルフ スポーツ 動画」画面の表示要求を処理します。
    ''' </summary>
    ''' <param name="movieType">動画の種別。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function GulfSportsMovie(movieType As String) As ActionResult

        Dim movieTypeValue As Byte = movieType.TryToValueType(Byte.MinValue)

        If movieTypeValue < 1 OrElse movieTypeValue > 3 Then
            Return RedirectToAction("GulfSportsMovieIndex")
        End If

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        Dim viewModel As NoteGulfSportsMovieViewModel = NoteGulfSportsMovieWorker.CreateViewModel(mainModel, movieTypeValue)

        ' ビュー を返却
        Return View(viewModel)

    End Function

    ''' <summary>
    ''' 「ガルフ スポーツ 動画」画面の登録要求を処理します。
    ''' </summary>
    ''' <param name="exerciseType">運動の種別。</param>
    ''' <param name="calorie">カロリー。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QyLogging()>
    Public Function GulfSportsMovieResult(exerciseType As Byte, calorie As Integer) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        If exerciseType > 0 AndAlso calorie > 0 Then
            If NoteGulfSportsMovieWorker.ExerciseRegister(mainModel, exerciseType, calorie) Then
                ' JSON を返却
                Return New NoteGulfSportsMovieJsonResult() With {
                    .IsSuccess = Boolean.TrueString,
                    .Messages = New List(Of String)() From {"運動の登録に成功しました。"}
                }.ToJsonResultWithSanitize()
            Else
                ' JSON を返却
                Return New NoteGulfSportsMovieJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Messages = New List(Of String)() From {"運動の登録に失敗しました"}
                }.ToJsonResultWithSanitize()
            End If

        Else
            ' JSON を返却
            Return New NoteGulfSportsMovieJsonResult() With {
                   .IsSuccess = Boolean.FalseString,
                   .Messages = New List(Of String)() From {"不正なデータです。画面を更新してやり直してください。"}
               }.ToJsonResultWithSanitize()
        End If

    End Function


#End Region

#Region "「健診結果画面」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function ExaminationSso(fromPageNo As Nullable(Of Byte)) As ActionResult

        Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
        If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        Dim heartlife As Integer = 47008
        If PortalHomeWorker.GetMedicalLinkageList(mainModel).Where(Function(i) i.LinkageSystemNo= heartlife.ToString()).Any() Then
            Return RedirectToAction("Examination")
        End If

        Return Redirect(NoteExaminationWorker.GetExaminationPage(mainModel,fromPageNoType))


    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyLogging()>
    Public Function Examination() As ActionResult
                
        'JOTOの健診ページ表示は廃止
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteExaminationViewModel = NoteExaminationWorker.CreateViewModel(mainModel)
        ' モデルをキャッシュへ格納
        mainModel.SetInputModelCache(viewModel)

        Return View(viewModel)
    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyApiAuthorize>
    <QjApiAuthorize()>
    <QyActionMethodSelector("HealthAge")>
    <QyLogging()>
    Public Function ExaminationResult(reference As String) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim message As New Dictionary(Of String, String)()

        Dim result As Boolean = False

        If Not String.IsNullOrWhiteSpace(reference) Then

            Try

                Dim paramater As NoteExaminationHelthAgeJsonParamater = Me.DecryptReference(Of NoteExaminationHelthAgeJsonParamater)(reference)

                Dim healthAgeEditInputModelN As List(Of HealthAgeEditInputModel) = NoteExaminationWorker.CreateHealthAgeEditInputModel(mainModel, paramater)

                For Each item As HealthAgeEditInputModel In healthAgeEditInputModelN

                    Dim valid As List(Of ComponentModel.DataAnnotations.ValidationResult) = HealthAgeEditWorker.ValidateByInputModelByExamination(item).ToList()

                    If Not valid.Any() Then 'モデルの検証成功

                        '健康年齢の測定
                        Dim errorMessage As New StringBuilder()

                        ' 健診受信日の時点での年齢が 18 歳以上かつ 74 歳以下かチェック
                        errorMessage.Append(HealthAgeEditWorker.CheckRecordDate(item.AuthorBirthday, item.RecordDate))

                        If errorMessage.Length = 0 Then
                            ' 健康年齢 Web API を実行
                            Dim responseN As List(Of QhApiHealthAgeResponseItem) = HealthAgeEditWorker.ExecuteJmdcHealthAgeApi(mainModel, item)

                            responseN.ForEach(
                                Sub(i)
                                    If i.StatusCode.TryToValueType(500) <> 200 Then errorMessage.AppendFormat("{0} {1}{2}", i.StatusCode, i.Message, Environment.NewLine)
                                End Sub
                            )

                            If errorMessage.Length = 0 Then
                                ' 登録
                                If HealthAgeEditWorker.Edit(mainModel, item, responseN) Then

                                    ' 成功
                                    message.Add(item.RecordDate.ToString("yyyy/MM/dd"), "成功")
                                    '一つでも成功したら成功を返却
                                    result = True

                                End If

                            Else
                                ' 健康年齢 Web APIエラー
                                AccessLogWorker.WriteErrorLog(Me.HttpContext, String.Empty, errorMessage.ToString())

                                message.Add(item.RecordDate.ToString("yyyy/MM/dd"), "健康年齢を測定出来ませんでした。健康年齢WEBAPIがメンテナンス中の可能性があります。")

                            End If
                        Else
                            ' 年齢チェックエラー
                            message.Add(item.RecordDate.ToString("yyyy/MM/dd"), errorMessage.ToString())

                        End If
                    End If
                Next

            Catch ex As Exception
                ' 暫定ログ書き込み
                message.Add("Exception", ex.Message)
                AccessLogWorker.WriteErrorLog(Me.HttpContext, String.Empty, String.Format("{0}:{1}", "ExaminationResult", ex.Message))

            End Try

        End If

        Return New NoteExaminationJsonResult() With {
            .IsSuccess = IIf(result, Boolean.TrueString, Boolean.FalseString).ToString(),
            .Messages = message
        }.ToJsonResult()

    End Function

    ''' <summary>
    ''' 「健診結果」画面の PDF ファイル の取得要求を処理します。
    ''' </summary>
    ''' <param name="reference">
    ''' 取得対象の ファイル 情報を表す、
    ''' 暗号化された JSON 文字列（<see cref="AssociatedFileStorageReferenceJsonParameter" /> クラス）。
    ''' </param>
    ''' <returns> </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    <QyAuthorize()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    <QyLogging()>
    Public Function ExaminationPdf(reference As String) As ActionResult

        'Using fs As New IO.FileStream(Me.HttpContext.Server.MapPath("~/App_Data/kensindummy.pdf"), IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)

        '    Dim length As Integer = CInt(fs.Length)
        '    Dim ar(length - 1) As Byte
        '    fs.Read(ar, 0, length)

        '    Return New FileContentResult(
        '                    ar,
        '                    "application/pdf"
        '                ) With {.FileDownloadName = String.Format("{0:yyyyMMddHHmmssfffffff}.pdf", Date.Now)}
        'End Using

        ' ファイル 情報を復号化
        Dim jsonObject As AssociatedFileStorageReferenceJsonParameter = Me.DecryptReference(Of AssociatedFileStorageReferenceJsonParameter)(reference)

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        If jsonObject IsNot Nothing Then
            Dim accountkey As Guid = jsonObject.Accountkey.TryToValueType(Guid.Empty)
            Dim loginAt As Date = jsonObject.LoginAt.TryToValueType(Date.MinValue)
            Dim linkageSystemNo As Integer = jsonObject.LinkageSystemNo.TryToValueType(Integer.MinValue)
            Dim linkageSystemId As String = jsonObject.LinkageSystemId
            Dim recordDate As Date = jsonObject.RecordDate.TryToValueType(Date.MinValue)
            Dim facilityKey As Guid = jsonObject.FacilityKey.TryToValueType(Guid.Empty)
            Dim dataKey As Guid = jsonObject.DataKey.TryToValueType(Guid.Empty)



            'AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("accountkey:{0}={1}", accountkey, mainModel.AuthorAccount.AccountKey))
            'AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("loginAt:{0}={1}", loginAt.ToString("yyyyMMddhhmmss"), mainModel.AuthorAccount.LoginAt.ToString("yyyyMMddhhmmss")))
            'AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("linkageSystemNo:{0}", linkageSystemNo))
            'AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("linkageSystemId:{0}", linkageSystemId))
            'AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("recordDate:{0}", recordDate))
            'AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("facilityKey:{0}", facilityKey))
            'AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("dataKey:{0}", dataKey))

            If accountkey = mainModel.AuthorAccount.AccountKey _
                AndAlso loginAt.ToString("yyyyMMddhhmmss") = mainModel.AuthorAccount.LoginAt.ToString("yyyyMMddhhmmss") _
                AndAlso linkageSystemNo <> Integer.MinValue _
                AndAlso Not String.IsNullOrWhiteSpace(linkageSystemId) _
                AndAlso recordDate <> Date.MinValue _
                AndAlso facilityKey <> Guid.Empty _
                AndAlso dataKey <> Guid.Empty Then

                Try
                    Dim originalName As String = String.Empty
                    Dim contentType As String = String.Empty

                    AccessLogWorker.WriteAccessLog(Nothing, "/Examination/ExaminationPdf", AccessLogWorker.AccessTypeEnum.None, String.Format("ファイル取得"))

                    ' ファイル を返却（ダウンロード として扱う）https://joto-hdrsub.qolms.com/
                    Return New FileContentResult(
                        NoteExaminationWorker.GetPdfFile(
                            mainModel,
                            dataKey,
                            linkageSystemNo,
                            linkageSystemId,
                            recordDate,
                            facilityKey,
                            originalName,
                            contentType
                        ),
                        contentType
                    ) With {.FileDownloadName = String.Format("{0:yyyyMMddHHmmssfffffff}{1}", Date.Now, IO.Path.GetExtension(originalName))}

                    'Dim binary As Byte() = NoteExaminationWorker.GetPdfFile(mainModel, dataKey, linkageSystemNo, linkageSystemId, recordDate, facilityKey, originalName, contentType)


                    ''Return New NoteExaminationPdfJsonResult() With {
                    ''    .IsSuccess = Boolean.TrueString,
                    ''    .PdfBinary = binary,
                    ''    .PdfFileName = String.Format("{0:yyyyMMddHHmmssfffffff}{1}", Date.Now, IO.Path.GetExtension(originalName))
                    ''    }.ToJsonResult(True)

                Catch ex As Exception
                End Try
            End If
        End If

        Return New NoteExaminationPdfJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .Message = "ファイルの取得に失敗しました。"
        }.ToJsonResult(True)

    End Function

    ''' <summary>
    ''' 「検査手帳」画面の検査結果表の表示要求を処理します。
    ''' </summary>
    ''' <param name="narrowInAbnormal">基準範囲外の結果のみへ絞り込むかのフラグ。</param>
    ''' <returns>
    ''' 成功ならパーシャルビュー、
    ''' 失敗ならJSON形式のコンテンツ。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly>
    <QyAuthorize()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyActionMethodSelector("Narrow")>
    <QyApiAuthorize()>
    <QjApiAuthorize()>
    Public Function ExaminationResult(narrowInFacility As String(), narrowInGroup() As String, narrowInAbnormal As Nullable(Of Boolean)) As ActionResult

        Dim boolValue As Boolean = If(narrowInAbnormal.HasValue, narrowInAbnormal.Value, False)

        ' 再展開用のモデルをセッションから取得
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim viewModel As NoteExaminationViewModel = Nothing

        Dim narrowInFacilityList As String() = Nothing

        If narrowInFacility IsNot Nothing Then
            narrowInFacilityList = narrowInFacility
        End If

        Dim narrowInGroupList As String() = Nothing

        If narrowInGroup IsNot Nothing Then
            narrowInGroupList = narrowInGroup
        End If

        ' 編集対象のモデルをキャッシュから取得
        viewModel = mainModel.GetInputModelCache(Of NoteExaminationViewModel)()

        If viewModel IsNot Nothing Then
            ' 成功
            'ViewModelの中の、ExaminationSetNから、表示対象となる病院の検査結果をリストアップ
            Dim selectedExaminationList As New List(Of ExaminationSetItem)
            If narrowInFacilityList IsNot Nothing AndAlso narrowInFacilityList.Any Then
                For Each eitem As ExaminationSetItem In viewModel.ExaminationSetN
                    For Each fitem As String In narrowInFacilityList
                        If Guid.Parse(eitem.OrganizationKey) = Guid.Parse(fitem) Then
                            selectedExaminationList.Add(eitem)

                        End If
                    Next
                Next
            Else
                selectedExaminationList = viewModel.ExaminationSetN
            End If

            'ViewModelの中の、ExaminationGroupNから、表示対象となる検査グループをリストアップ
            Dim selectedGroupList As New List(Of ExaminationGroupItem)
            If narrowInGroupList IsNot Nothing AndAlso narrowInGroupList.Any Then
                For Each gitem As ExaminationGroupItem In viewModel.ExaminationGroupN
                    For Each gritem As String In narrowInGroupList
                        If gitem.GroupNo.ToString = gritem Then
                            selectedGroupList.Add(gitem)

                        End If
                    Next
                Next
            Else
                selectedGroupList = viewModel.ExaminationGroupN
            End If

            '表（ExaminationMatrix）を再作成
            viewModel.MatrixN = NoteExaminationWorker.CreateExaminationMatrixFromItems(mainModel, selectedExaminationList, selectedGroupList)

            viewModel.NarrowInAbnormal = boolValue

            'キャッシュ内のモデルを更新
            mainModel.SetInputModelCache(viewModel)

            ' パーシャルビューを返却
            Return PartialView("_NoteExaminationResultPartialView", New NoteExaminationResultPartialViewModel(viewModel))
        Else
            ' 失敗ならJSONを返却
            Return New NoteExaminationJsonResult() With {.IsSuccess = Boolean.FalseString}.ToJsonResult()
        End If

    End Function


#End Region

#Region "「レシピ 動画」画面"

    ''' <summary>
    ''' 「レシピ トップ」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function RecipeMovieIndex() As ActionResult

        ' ビュー を返却
        Return View()

    End Function

    ''' <summary>
    ''' 「レシピ 動画」画面の表示要求を処理します。
    ''' </summary>
    ''' <param name="movieType">動画の種別。</param>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function RecipeMovie(movieType As String) As ActionResult

        Dim movieTypeValue As Byte = movieType.TryToValueType(Byte.MinValue)

        If movieTypeValue <> 1 Then
            Return RedirectToAction("RecipeMovieIndex")
        End If

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        Dim viewModel As NoteRecipeMovieViewModel = NoteRecipeMovieWorker.CreateViewModel(mainModel, movieTypeValue)

        ' ビュー を返却
        Return View(viewModel)

    End Function

#End Region

#Region "「おくすり」画面"

    ''' <summary>
    ''' 「おくすり」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Medicine() As ActionResult

        ' ビュー を返却
        Return View(NoteMedicineWorker.CreateViewModel(Me.GetQolmsYappliModel()))

    End Function

#End Region

#Region "「心拍」画面"

    ''' <summary>
    ''' 「心拍」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function HeartRate(Optional selectdate As String = "", Optional periodType As QyPeriodTypeEnum = QyPeriodTypeEnum.OneDay, Optional initDate As Boolean = False) As ActionResult

        '対象のFitbit連携が成立しているかどうかを確認
        If Not PortalHomeWorker.IsFitbitConnected(Me.GetQolmsYappliModel()) Then
            ' 「Fitbit 連携」画面へ遷移
            Dim dict As New RouteValueDictionary()
            dict.Add("FromPageNo", 1)

            Return RedirectToAction("FitbitConnection", "Portal", dict)
        End If

        '日付
        Dim endDate As Date = Date.MinValue
        If Not String.IsNullOrWhiteSpace(selectdate) AndAlso selectdate.Length = 8 Then

            Dim tryStartDate As Date = Date.MinValue
            Dim ci As CultureInfo = CultureInfo.CurrentCulture
            Dim dts As DateTimeStyles = DateTimeStyles.None
            If Date.TryParseExact(selectdate, "yyyyMMdd", ci, dts, tryStartDate) AndAlso tryStartDate < Date.Now.Date Then
                endDate = tryStartDate
            End If
        End If

        Dim startDate As Date = Date.MinValue

        ' 日付未指定(デフォルト)
        If endDate = Date.MinValue Then
            endDate = Date.Now
            endDate = New Date(endDate.Year, endDate.Month, endDate.Day)
        End If

        If initDate = True Then
            ' 期間指定変更時など今日を起点に初期化する場合
            endDate = Date.Now
            endDate = New Date(endDate.Year, endDate.Month, endDate.Day)
        End If

        Select Case periodType
            Case QyPeriodTypeEnum.OneDay
                startDate = endDate
            Case QyPeriodTypeEnum.OneWeek
                ' 指定日を含む週
                While Not endDate.DayOfWeek = DayOfWeek.Saturday
                    endDate = endDate.AddDays(1)
                End While
                startDate = endDate.AddDays(-6)
            Case QyPeriodTypeEnum.OneMonth
                ' 指定日を含む月
                endDate = New Date(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month))
                startDate = New Date(endDate.Year, endDate.Month, 1)
            Case QyPeriodTypeEnum.ThreeMonths
                ' 指定日を含む月から3ヶ月
                endDate = New Date(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month))
                startDate = New Date(endDate.AddMonths(-2).Year, endDate.AddMonths(-2).Month, 1)
        End Select

        ' 日数分移動させる場合
        'Select Case periodType
        '    Case QyPeriodTypeEnum.OneDay
        '        startDate = endDate
        '    Case QyPeriodTypeEnum.OneWeek
        '        startDate = endDate.AddDays(-6)
        '    Case QyPeriodTypeEnum.OneMonth
        '        startDate = endDate.AddMonths(-1).AddDays(1)
        '    Case QyPeriodTypeEnum.ThreeMonths
        '        startDate = endDate.AddMonths(-3).AddDays(1)
        'End Select

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 表示対象の モデル を作成
        Dim viewModel As NoteHeartRateViewModel = NoteHeartRateWorker.CreateViewModel(mainModel, periodType, startDate, endDate)

        ' ビュー を返却
        Return View(viewModel)

    End Function

#End Region

#Region "「運動強度」画面"

    ''' <summary>
    ''' 「運動強度」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Mets(Optional selectdate As String = "", Optional periodType As QyPeriodTypeEnum = QyPeriodTypeEnum.OneDay, Optional initDate As Boolean = False) As ActionResult

        '対象のFitbit連携が成立しているかどうかを確認
        If Not PortalHomeWorker.IsFitbitConnected(Me.GetQolmsYappliModel()) Then
            ' 「Fitbit 連携」画面へ遷移
            Dim dict As New RouteValueDictionary()
            dict.Add("FromPageNo", 1)

            Return RedirectToAction("FitbitConnection", "Portal", dict)
        End If

        '日付
        Dim endDate As Date = Date.MinValue
        If Not String.IsNullOrWhiteSpace(selectdate) AndAlso selectdate.Length = 8 Then

            Dim tryRecordDate As Date = Date.MinValue
            Dim ci As CultureInfo = CultureInfo.CurrentCulture
            Dim dts As DateTimeStyles = DateTimeStyles.None
            If Date.TryParseExact(selectdate, "yyyyMMdd", ci, dts, tryRecordDate) AndAlso tryRecordDate < Date.Now.Date Then
                endDate = tryRecordDate
            End If
        End If

        Dim startDate As Date = Date.MinValue
        ' 日付未指定(デフォルト)
        If endDate = Date.MinValue Then
            endDate = Date.Now
            endDate = New Date(endDate.Year, endDate.Month, endDate.Day)
        End If

        ' 期間指定変更時など今日を起点に初期化する場合
        If initDate = True Then
            endDate = Date.Now
            endDate = New Date(endDate.Year, endDate.Month, endDate.Day)
        End If

        Select Case periodType
            Case QyPeriodTypeEnum.OneDay
                startDate = endDate
            Case QyPeriodTypeEnum.OneWeek
                ' 指定日を含む週
                While Not endDate.DayOfWeek = DayOfWeek.Saturday
                    endDate = endDate.AddDays(1)
                End While
                startDate = endDate.AddDays(-6)
            Case QyPeriodTypeEnum.OneMonth
                ' 指定日を含む月
                endDate = New Date(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month))
                startDate = New Date(endDate.Year, endDate.Month, 1)
            Case QyPeriodTypeEnum.ThreeMonths
                ' 指定日を含む月から3ヶ月
                endDate = New Date(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month))
                startDate = New Date(endDate.AddMonths(-2).Year, endDate.AddMonths(-2).Month, 1)
        End Select

        ' 日数分移動させる場合
        'Select Case periodType
        '    Case QyPeriodTypeEnum.OneDay
        '        startDate = endDate
        '    Case QyPeriodTypeEnum.OneWeek
        '        startDate = endDate.AddDays(-6)
        '    Case QyPeriodTypeEnum.OneMonth
        '        startDate = endDate.AddMonths(-1).AddDays(1)
        '    Case QyPeriodTypeEnum.ThreeMonths
        '        startDate = endDate.AddMonths(-3).AddDays(1)
        'End Select

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 表示対象の モデル を作成
        Dim viewModel As NoteMetsViewModel = NoteMetsWorker.CreateViewModel(mainModel, periodType, startDate, endDate)

        ' ビュー を返却
        Return View(viewModel)

    End Function

#End Region

#Region "「カロミル」画面"

    ''' <summary>
    ''' 「カロミル」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Calomeal(Optional meal As String = "", Optional selectdate As String = "") As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim challengekey As Guid? = Guid.Empty
        Dim linkage As Integer = Integer.MinValue

        If NoteCalomealWorker.IsCallDynamicLink(mainModel, challengekey,linkage) Then

            Return RedirectToAction("DynamicLink", New With {.challengekey = challengekey,.linkage = linkage})

        End If

        '日付指定
        Dim recordDate As Date = Date.MinValue

        Dim tryRecordDate As Date = Date.MinValue
        Dim ci As CultureInfo = CultureInfo.CurrentCulture
        Dim dts As DateTimeStyles = DateTimeStyles.None
        If Not String.IsNullOrWhiteSpace(selectdate) AndAlso selectdate.Length = 8 AndAlso Date.TryParseExact(selectdate, "yyyyMMdd", ci, dts, tryRecordDate) AndAlso tryRecordDate < Date.Now.Date Then

            recordDate = tryRecordDate

        Else

            recordDate = Date.Now.Date

        End If
        'CalomealWebViewWorker.DebugLog(recordDate.ToString)

        Dim token As String = NoteCalomealWorker.TokenRead(Me.GetQolmsYappliModel())
        'NoteCalomealWorker.DebugLog(token)

        Return Redirect(NoteCalomealWorker.GetWebViewAuthUrl(token, recordDate ,meal.TryToValueType(Byte.MinValue)))

    End Function

    ''' <summary>
    ''' 「カロミル」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CalomealResult(result As String, state As String, code As String, [error] As String) As ActionResult

        Dim selectDate As Date = Date.Now
        Dim token As String = NoteCalomealWorker.GetToken(Me.GetQolmsYappliModel(), code)

        'カロミル側のプロフィールの登録を済ませておく。
        'NoteCalomealWorker.SetProfile(Me.GetQolmsYappliModel(), token)

        Return RedirectToAction("CalomealDiscription","start",New With {.isFirst = true})

    End Function

    ''' <summary>
    ''' 「カロミル」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CalomealError() As ActionResult

        ' ビュー を返却
        Return RedirectToAction("home","portal")

    End Function
    ''' <summary>
    ''' 「カロミル」画面の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CalomealErrorResult(page_id As String ,status_code As String ,[error] As String,retry As String ) As ActionResult

        If status_code = "200" andalso retry = "1" Then
            NoteCalomealWorker.RefreshToken(Me.GetQolmsYappliModel())
            Return RedirectToAction("calomeal")
        End If

        'If status_code = "400" AndAlso [error] = "Unset profile." Then
        '    'page_id=meal,status_code=400,error=Unset profile.
        '    NoteCalomealWorker.SetProfile(Me.GetQolmsYappliModel())
        'End If

        If Not String.IsNullOrWhiteSpace([error]) Then
           me.SetErrorMessage([error])
        End If
        Throw New InvalidOperationException(String.Format("カロミルエラーです。page_id={0},status_code={1},error={2}",page_id,status_code,[error]))

        ' ビュー を返却
        'Return View()

    End Function

    ''' <summary>
    ''' 「カロミル履歴の同期（検証）」
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CalomealHistory() As ActionResult
        NoteCalomealWorker.History(Me.GetQolmsYappliModel())
        Return View()

    End Function


    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function DynamicLink(challengekey As guid?, linkage As Integer) As ActionResult

        Return Redirect(NoteCalomealWorker.DynamicLink(Me.GetQolmsYappliModel(), challengekey, linkage))

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CalomealJwtDynamicLink(jwt As String) As ActionResult

        'CalomealWebViewWorker.DebugLog(jwt)

        Return Redirect(NoteCalomealWorker.DynamicLink(Me.GetQolmsYappliModel(), jwt))

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function CalomealHokenshido() As ActionResult

        Return Redirect(NoteCalomealWorker.Hokenshido(Me.GetQolmsYappliModel()))

    End Function

    '''' <summary>
    '''' 「カロミル」画面の表示要求を処理します。
    '''' </summary>
    '''' <returns>
    '''' アクション の結果。
    '''' </returns>
    '''' <remarks></remarks>
    '<HttpGet()>
    '<QyAuthorize()>
    '<QyApiAuthorize()>
    '<QyLogging()>
    'Public Function CalomealFitst(Optional selectdate As String = "") As ActionResult

    '    ViewData("First") = True
    '    Return View()

    'End Function

#End Region

#Region "「ALKOO DynamicLink」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QjApiAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function AlkooDynamicLink(urlSettingName As String, code As String, ginowanjoin As String) As ActionResult

        Dim str As String = PortalAlkooConnectionWorker.DynamicLink(Me.GetQolmsYappliModel(), urlSettingName, code, ginowanjoin)

        Return Redirect(str)

    End Function


#End Region


#End Region

#Region "パーシャル ビュー アクション"

#Region "共通 パーツ"

    ''' <summary>
    ''' 「ヘッダー」パーシャル ビュー の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteHeaderPartialView() As ActionResult

        ' パーシャル ビュー を返却
        If Me.IsYappli Then
            Return New EmptyResult()
        Else
            Return PartialView("_NoteHeaderPartialView")
        End If

    End Function

    ''' <summary>
    ''' 「フッター」パーシャル ビュー の表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    <OutputCache(Duration:=600)>
    Public Function NoteFooterPartialView() As ActionResult

        ' パーシャル ビュー を返却
        Return PartialView("_NoteFooterPartialView")

    End Function

#End Region

#Region "「食事」画面用 パーシャル ビュー"

    '''' <summary>
    '''' 「食事」画面用
    '''' 「最近の食事」パーシャル ビューの表示要求を処理します。
    '''' </summary>
    '''' <returns>
    '''' アクションの結果。
    '''' </returns>
    '''' <remarks></remarks>
    '<ChildActionOnly()>
    'Public Function NoteMealCardPartialView() As ActionResult

    '    Dim model As NoteMealViewModel = Me.GetPageViewModel(Of NoteMealViewModel)()

    '    ' パーシャル ビューを返却
    '    Return PartialView("_NoteMealCardPartialView", model)

    'End Function

    '''' <summary>
    '''' 「食事」画面用
    '''' 「文字検索結果」パーシャル ビューの表示要求を処理します。
    '''' </summary>
    '''' <returns>
    '''' アクションの結果。
    '''' </returns>
    '''' <remarks></remarks>
    '<ChildActionOnly()>
    'Public Function NoteMealSearchResultPartialView() As ActionResult

    '    Dim model As NoteMealViewModel = Me.GetPageViewModel(Of NoteMealViewModel)()

    '    ' パーシャル ビューを返却
    '    Return PartialView("_NoteMealSearchResultPartialView", model)

    'End Function

    '''' <summary>
    '''' 「食事（2）（3）」画面用
    '''' 「文字検索結果」パーシャル ビューの表示要求を処理します。
    '''' </summary>
    '''' <returns>
    '''' アクションの結果。
    '''' </returns>
    '''' <remarks></remarks>
    '<ChildActionOnly()>
    'Public Function NoteMealSearchAreaPartialView() As ActionResult

    '    Dim model As NoteMealViewModel = Me.GetPageViewModel(Of NoteMealViewModel)()

    '    ' パーシャル ビューを返却
    '    Return PartialView("_NoteMealSearchAreaPartialView", New NoteMealSearchAreaPartialViewModel(model))

    'End Function

    '''' <summary>
    '''' 「食事（3）」画面用
    '''' 「最近の食事」パーシャル ビューの表示要求を処理します。
    '''' </summary>
    '''' <returns>
    '''' アクションの結果。
    '''' </returns>
    '''' <remarks></remarks>
    '<ChildActionOnly()>
    'Public Function NoteMealCardAreaPartialView() As ActionResult

    '    Dim model As NoteMealViewModel3 = Me.GetPageViewModel(Of NoteMealViewModel3)()

    '    ' パーシャル ビューを返却
    '    Return PartialView("_NoteMealCardAreaPartialView", New NoteMealCardAreaPartialViewModel(model))

    'End Function

    ''' <summary>
    ''' 「食事」画面共通
    ''' 「日付、食事種別、時間」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <param name="displayTime">True:時間を表示する</param>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealEditDatePartialView(Optional displayTime As Nullable(Of Boolean) = Nothing) As ActionResult

        Dim model As QyNotePageViewModelBase = Me.GetPageViewModel(Of QyNotePageViewModelBase)()

        If TryCast(model, NoteMealViewModel4) Is Nothing AndAlso TryCast(model, NoteMealListInputModel) Is Nothing Then
            Throw New InvalidOperationException("親モデルが対象外です。")
        End If

        If displayTime Is Nothing Then
            displayTime = True
        End If

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealEditDatePartialView", New NoteMealEditDatePartialViewModel(model) With {.DisplayTime = displayTime.Value})

    End Function

    ''' <summary>
    ''' 「食事」画面共通
    ''' 「検索エリア」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealEditSearchAreaPartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealEditSearchAreaPartialView")

    End Function

    ''' <summary>
    ''' 「食事（4）」画面用
    ''' 「最近の食事」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealCardAreaPartialView4() As ActionResult

        Dim model As NoteMealViewModel4 = Me.GetPageViewModel(Of NoteMealViewModel4)()

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealCardAreaPartialView4", New NoteMealCardAreaPartialViewModel4(model))

    End Function

    ''' <summary>
    ''' 「食事」画面共通
    ''' 「品目、カロリー編集」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealEditHinmokuCalPartialView() As ActionResult

        Dim model As NoteMealListInputModel = Me.GetPageViewModel(Of NoteMealListInputModel)()

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealEditHinmokuCalPartialView", New NoteMealEditHinmokuCalPartialViewModel(model))

    End Function

    ''' <summary>
    ''' 「食事」画面共通
    ''' 「サムネイル編集」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealEditThumbnailPartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealEditThumbnailPartialView")

    End Function

    ''' <summary>
    ''' 「食事」画面共通
    ''' 「解析結果」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealAnalyzeResultPartialView(pageType As String) As ActionResult

        Dim model As NoteMealListInputModel = Me.GetPageViewModel(Of NoteMealListInputModel)()
        Dim viewModel As New NoteMealAnalyzeResultPartialViewModel(model)
        viewModel.PageType = pageType

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealAnalyzeResultPartialView", viewModel)

    End Function

    ''' <summary>
    ''' 「食事」画面共通
    ''' 「検索結果」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealSearchResultAreaPartialView() As ActionResult

        Dim model As NoteMealSearchResultViewModel = Me.GetPageViewModel(Of NoteMealSearchResultViewModel)()
        Dim viewModel As New NoteMealSearchResultAreaPartialViewModel(model)

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealSearchResultAreaPartialView", viewModel)

    End Function

    ''' <summary>
    ''' 「食事（4）履歴から登録」画面用
    ''' 「最近の食事」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMealRegisterFromHistoryCardAreaPartialView() As ActionResult

        Dim model As NoteMealViewModel4 = Me.GetPageViewModel(Of NoteMealViewModel4)()

        ' パーシャル ビューを返却
        Return PartialView("_NoteMealRegisterFromHistoryCardAreaPartialView", New NoteMealCardAreaPartialViewModel4(model))

    End Function

#End Region

#Region "「運動」画面用 パーシャル ビュー"

    ''' <summary>
    ''' 「運動」画面用
    ''' 「最近の運動」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteExerciseCardPartialView() As ActionResult

        Dim model As NoteExerciseViewModel = Me.GetPageViewModel(Of NoteExerciseViewModel)()

        ' パーシャル ビューを返却
        Return PartialView("_NoteExerciseCardPartialView", model)

    End Function

#End Region

#Region "「健診結果」画面用パーシャルビュー"

    ''' <summary>
    ''' 「健診結果」画面
    ''' 健診結果表パーシャルビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteExaminationResultPartialView() As ActionResult

        Dim model As NoteExaminationResultPartialViewModel = Nothing

        Try
            model = New NoteExaminationResultPartialViewModel(Me.GetPageViewModel(Of NoteExaminationViewModel)())
        Catch
        End Try

        ' パーシャルビューを返却
        If model IsNot Nothing Then
            Return PartialView("_NoteExaminationResultPartialView", model)
        Else
            Return New EmptyResult()
        End If

    End Function

    ''' <summary>
    ''' 「健診結果」画面
    ''' 「絞り込み」ボックスパーシャルビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteExaminationFilterPartialView() As ActionResult

        Dim model As NoteExaminationResultPartialViewModel = Nothing

        Try
            model = New NoteExaminationResultPartialViewModel(Me.GetPageViewModel(Of NoteExaminationViewModel)())
        Catch
        End Try

        ' パーシャルビューを返却
        If model IsNot Nothing Then
            Return PartialView("_NoteExaminationFilterPartialView", model)

        End If

        Return New EmptyResult()

    End Function

#End Region

#Region "「おくすり」画面用パーシャルビュー"

    ''' <summary>
    ''' 「健診結果」画面
    ''' 健診結果表パーシャルビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function NoteMedicineTablePartialView() As ActionResult

        Dim model As NoteMedicineTablePartialViewModel = Nothing

        Try

            Dim viewModel As NoteMedicineViewModel = Me.GetPageViewModel(Of NoteMedicineViewModel)()
            model = Me.GetPageViewModel(Of NoteMedicineViewModel)().MedicineTablePartialViewModel()
        Catch
        End Try

        ' パーシャルビューを返却
        If model IsNot Nothing Then
            Return PartialView("_NoteMedicineTablePartialView", model)
        Else
            Return New EmptyResult()
        End If

    End Function

#End Region

#End Region

End Class

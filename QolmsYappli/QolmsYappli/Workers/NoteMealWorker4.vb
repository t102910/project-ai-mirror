Imports System.Threading
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCalomealApiCoreV1

''' <summary>
''' 「食事（4）」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class NoteMealWorker4

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

#Region "ライフログテクノロジー カロミル Web API"

    ''' <summary>
    ''' 食事写真識別 Web API を実行します。
    ''' </summary>
    ''' <param name="fileName">ファイル名。</param>
    ''' <param name="data">ファイルのバイト配列（jpg 形式固定）。</param>
    ''' <returns>
    ''' 品目情報のリスト（品目である度合の高い順）。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteImageAnalysisSend2Api(fileName As String, data() As Byte) As List(Of FoodItem)

        If String.IsNullOrWhiteSpace(fileName) Then Throw New ArgumentNullException("fileName", "ファイル名が Null 参照もしくは空白です。")
        If data Is Nothing OrElse Not data.Any() Then Throw New ArgumentNullException("data", "ファイルのバイナリ データが Null 参照です。")

        Dim result As New List(Of FoodItem)()
        Dim apiArgs As New imageAnalysisSend2ApiArgs() With {
            .FileName = fileName,
            .Data = data,
            .Limit = 10
        }
        Dim apiResults As imageAnalysisSend2ApiResults = QsCalomealApiManager.Execute(Of imageAnalysisSend2ApiResults)(apiArgs)

        With apiResults
            If .IsSuccess AndAlso .root.Any() Then
                For Each item As foodOfJson In .root
                    If Not String.IsNullOrWhiteSpace(item.label) AndAlso item.menu IsNot Nothing Then
                        result.Add(
                            New FoodItem() With {
                                .label = item.label.Trim(),
                                .probability = item.probability,
                                .calorie = item.menu.calorie,
                                .protein = item.menu.protein,
                                .lipid = item.menu.lipid,
                                .carbohydrate = item.menu.carbohydrate,
                                .salt_amount = item.menu.salt_amount,
                                .available_carbohydrate = item.menu.available_carbohydrate,
                                .fiber = item.menu.fiber
                            }
                        )
                    End If
                Next
            End If
        End With

        Return result

    End Function

    ''' <summary>
    ''' 食事メニュー検索 Web API を実行します。
    ''' </summary>
    ''' <param name="keyword">検索キーワード。</param>
    ''' <param name="page">ページ数。</param>
    ''' <param name="limit">ページあたりの件数。</param>
    ''' <param name="refTotalPage">総ページ数が格納される変数。</param>
    ''' <returns>
    ''' 品目情報のリスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteMealSearchApi(keyword As String, page As Integer, limit As Integer, ByRef refTotalPage As Integer) As List(Of FoodItem)

        refTotalPage = 0

        If String.IsNullOrWhiteSpace(keyword) Then Throw New ArgumentNullException("keyword", "検索キーワードが Null 参照もしくは空白です。")
        If page < 1 Then Throw New ArgumentOutOfRangeException("page", "page", "ページ数が不正です。")
        If limit < 1 Then Throw New ArgumentOutOfRangeException("limit", "limit", "ページあたりの件数が不正です。")

        Dim result As New List(Of FoodItem)()
        Dim apiArgs As New mealSearchApiArgs() With {
            .keyword = keyword,
            .page = page,
            .limit = limit
        }
        Dim apiResults As mealSearchApiResults = QsCalomealApiManager.Execute(Of mealSearchApiResults)(apiArgs)

        With apiResults
            If .IsSuccess AndAlso .data.Any() Then
                For Each item As dataOfJson In .data
                    If Not String.IsNullOrWhiteSpace(item.menu_name) Then
                        result.Add(
                            New FoodItem() With {
                                .label = item.menu_name.Trim(),
                                .probability = String.Empty,
                                .calorie = item.calorie,
                                .protein = item.protein,
                                .lipid = item.lipid,
                                .carbohydrate = item.carbohydrate,
                                .salt_amount = item.salt_amount,
                                .available_carbohydrate = item.available_carbohydrate,
                                .fiber = item.fiber
                            }
                        )
                    End If
                Next

                refTotalPage = Convert.ToInt32(.total_page)
            End If
        End With

        Return result

    End Function

#End Region

    ''' <summary>
    ''' 食事の情報を作成します。
    ''' </summary>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <returns>
    ''' API 用の食事情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function MakeMealItem(inputModel As NoteMealListInputModel) As QhApiMealListItem

        Dim mealDate As Date = Date.MinValue

        If Not Date.TryParse(String.Format("#{0} {1}:{2}:00 {3}#", inputModel.RecordDate.ToShortDateString, inputModel.Hour, inputModel.Minute, inputModel.Meridiem), mealDate) Then
            mealDate = inputModel.RecordDate
        End If

        Dim mealNames As New List(Of String)
        For Each item As String In inputModel.ItemName
            If String.IsNullOrWhiteSpace(item) Then
                mealNames.Add("不明")
            Else
                mealNames.Add(item)
            End If
        Next

        Dim cals As New List(Of String)
        For Each cal As String In inputModel.Calorie
            If String.IsNullOrWhiteSpace(cal) Then
                cals.Add("0")
            Else
                cals.Add(cal)
            End If
        Next

        Dim photoKey As New List(Of String)
        For Each id As Guid In inputModel.ForeignKey
            photoKey.Add(id.ToString())
        Next

        Dim setFoodN As New List(Of List(Of QhApiFoodItem))
        For i As Integer = 0 To inputModel.FoodN.Count - 1
            Dim apiFood As New List(Of QhApiFoodItem)
            apiFood = inputModel.FoodN(i).ToList().ConvertAll(
                Function(j) New QhApiFoodItem() With {
                    .label = j.label,
                    .probability = j.probability,
                    .calorie = j.calorie,
                    .protein = j.protein,
                    .lipid = j.lipid,
                    .carbohydrate = j.carbohydrate,
                    .salt_amount = j.salt_amount,
                    .available_carbohydrate = j.available_carbohydrate,
                    .fiber = j.fiber
                }
            )
            setFoodN.Add(apiFood)
        Next

        Dim setBeforeRecordDate As New List(Of String)
        For Each recordDate As Date In inputModel.BeforeRecordDate
            setBeforeRecordDate.Add(recordDate.ToApiDateString())
        Next

        Dim setBeforeMealType As New List(Of String)
        For Each mealType As QyMealTypeEnum In inputModel.BeforeMealType
            setBeforeMealType.Add(mealType.ToString())
        Next

        Dim setSequence As New List(Of String)
        For Each seq As Integer In inputModel.Sequence
            setSequence.Add(seq.ToString())
        Next

        Return New QhApiMealListItem() With {
            .BeforeRecordDate = setBeforeRecordDate,
            .BeforeMealType = setBeforeMealType,
            .MealDate = mealDate.ToApiDateString(),
            .MealType = inputModel.MealType.ToString(),
            .MealName = mealNames,
            .Calorie = cals,
            .Sequence = setSequence,
            .PhotoKey = photoKey,
            .FoodN = setFoodN
        }

    End Function

    ''' <summary>
    ''' 「食事」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="filterDate">
    ''' 絞り込み日付。
    ''' 絞り込みが不要なら Date.MinValue を指定（直近 30 件の取得）。
    ''' </param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteMealReadApi(mainModel As QolmsYappliModel, filterDate As Date) As QhYappliNoteMealReadApiResults

        Dim apiArgs As New QhYappliNoteMealReadApiArgs(
            QhApiTypeEnum.YappliNoteMealRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString,
            .FilterDate = filterDate.ToApiDateString()
        }
        Dim apiResults As QhYappliNoteMealReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteMealReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    ''' <summary>
    ''' 「食事」画面登録 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteMealWriteApi(mainModel As QolmsYappliModel, inputModel As NoteMealListInputModel, deleteFlag As Boolean, copyFlag As Boolean) As QhYappliNoteMealListWriteApiResults

        Dim result As Boolean = False
        Dim mealItem As QhApiMealListItem = NoteMealWorker4.MakeMealItem(inputModel)

        Dim apiArgs As New QhYappliNoteMealListWriteApiArgs(
            QhApiTypeEnum.YappliNoteMealListWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .MealItem = mealItem,
            .DeleteFlag = deleteFlag,
            .CopyFlag = copyFlag
        }
        Dim apiResults As QhYappliNoteMealListWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteMealListWriteApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 「食事」画面ビュー モデルを作成します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <returns>
    ''' 成功なら「食事」画面ビュー モデル、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, Optional mealType As QyMealTypeEnum = QyMealTypeEnum.None, Optional targetDate As Nullable(Of Date) = Nothing) As NoteMealViewModel4

        Dim result As NoteMealViewModel4 = mainModel.GetInputModelCache(Of NoteMealViewModel4)() ' キャッシュから取得

        Dim recordDate As Date = Date.Now
        If targetDate.HasValue AndAlso targetDate > Date.MinValue Then
            recordDate = New Date(targetDate.Value.Year, targetDate.Value.Month, targetDate.Value.Day, recordDate.Hour, recordDate.Minute, 0)
        End If

        If result Is Nothing OrElse result.FilterDate <> Date.MinValue Then
            ' キャッシュが無ければ API を実行
            With NoteMealWorker4.ExecuteNoteMealReadApi(mainModel, Date.MinValue)
                result = New NoteMealViewModel4(
                    mainModel,
                    recordDate,
                    .MealItemN.ConvertAll(Function(i) i.ToMealItem),
                    mealType
                )

                'result.InputModel = New NoteMealInputModel() With {
                '    .RecordDate = result.RecordDate,
                '    .MealType = QyMealTypeEnum.Breakfast
                '}

                ' キャッシュへ追加
                mainModel.SetInputModelCache(result)
            End With
        End If

        Return result

    End Function

    ''' <summary>
    ''' 「食事」画面ビュー モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <returns>
    ''' 成功なら「食事」画面ビュー モデル、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetViewModelCache(mainModel As QolmsYappliModel) As NoteMealViewModel4

        Dim result As NoteMealViewModel4 = mainModel.GetInputModelCache(Of NoteMealViewModel4)()

        ' キャッシュが無ければ再作成
        If result Is Nothing Then
            ' ページ ビュー モデルを生成しキャッシュへ追加
            result = NoteMealWorker4.CreateViewModel(mainModel)
        End If

        Return result

    End Function

    ''' <summary>
    ''' 食事メニューを検索します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="searchText">検索文字列。</param>
    ''' <returns>
    ''' 「食事メニュー検索結果」パーシャル ビュー モデル。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function Search(mainModel As QolmsYappliModel, searchText As String) As NoteMealSearchResultViewModel

        Dim totalPage As Integer = 0
        Dim pageViewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel)
        Dim foodN As New List(Of FoodItem)()

        If Not String.IsNullOrWhiteSpace(searchText) Then
            Try
                ' 食事メニュー検索 Web API を実行

                ' 1 ページ分取得
                foodN = GetFoodItemList(mainModel, searchText, 1, totalPage)
            Catch
                foodN = New List(Of FoodItem)()
            End Try
        End If

        If foodN.Any() Then
            Return New NoteMealSearchResultViewModel() With {
                .SearchedMealItemN = foodN,
                .SearchedMaxPage = totalPage,
                .DispedPage = 1
            }
        Else
            Return New NoteMealSearchResultViewModel() With {
                .SearchedMealItemN = foodN,
                .SearchedMaxPage = 0,
                .DispedPage = 0
            }
        End If

    End Function

    ''' <summary>
    ''' 食事メニューの検索し、検索結果を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="searchText">検索文字列。</param>
    ''' <param name="page">ページ番号。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFoodItemList(mainModel As QolmsYappliModel, searchText As String, page As Integer) As List(Of FoodItem)
        Dim totalPage As Integer = 0
        Return GetFoodItemList(mainModel, searchText, page, totalPage)
    End Function

    ''' <summary>
    ''' 食事メニューの検索し、検索結果を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="searchText">検索文字列。</param>
    ''' <param name="page">ページ番号。</param>
    ''' <param name="totalPage">総ページ数。</param>
    ''' <returns>
    ''' 「食事メニュー検索結果」パーシャル ビュー モデル。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function GetFoodItemList(mainModel As QolmsYappliModel, searchText As String, page As Integer, ByRef totalPage As Integer) As List(Of FoodItem)

        Dim pageViewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel)
        Dim foodN As New List(Of FoodItem)()

        If Not String.IsNullOrWhiteSpace(searchText) Then
            Try
                ' 食事メニュー検索 Web API を実行

                ' 1 ページ分取得
                foodN = NoteMealWorker4.ExecuteMealSearchApi(searchText, page, 20, totalPage)
            Catch
                foodN = New List(Of FoodItem)()
            End Try
        End If

        If foodN.Any() Then
            Return foodN
        Else
            Return New List(Of FoodItem)
        End If
    End Function

    ' ''' <summary>
    ' ''' 食事メニューを検索します。(編集領域の検索用)
    ' ''' </summary>
    ' ''' <param name="mainModel">メイン モデル。</param>
    ' ''' <param name="searchText">検索文字列。</param>
    ' ''' <returns>
    ' ''' 「食事メニュー検索結果」パーシャル ビュー モデル。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Shared Function EditSearch(mainModel As QolmsYappliModel, searchText As String) As NoteMealSearchAreaPartialViewModel

    '    Dim pageViewModel As NoteMealViewModel = NoteMealWorker2.GetViewModelCache(mainModel)
    '    Dim foodN As New List(Of FoodItem)()

    '    If Not String.IsNullOrWhiteSpace(searchText) Then
    '        Try
    '            ' 食事メニュー検索 Web API を実行
    '            Dim totalPage As Integer = 0

    '            ' 1 ページ分取得
    '            foodN = NoteMealWorker4.ExecuteMealSearchApi(searchText, 1, 20, totalPage)

    '            ' 総ページ分取得
    '            If totalPage > 1 Then foodN = NoteMealWorker4.ExecuteMealSearchApi(searchText, 1, 20 * totalPage, totalPage)
    '        Catch
    '            foodN = New List(Of FoodItem)()
    '        End Try
    '    End If

    '    ' ページ単位へ分割
    '    If foodN.Any() Then
    '        Dim pageList As New List(Of List(Of FoodItem))()
    '        Dim pageCount As Integer = 0

    '        pageCount = Convert.ToInt32(Math.Ceiling(foodN.Count / 20D))

    '        For i As Integer = 0 To pageCount - 1
    '            Dim fList As New List(Of FoodItem)()

    '            For j As Integer = i * 20 To i * 20 + 19
    '                fList.Add(foodN.Item(j))

    '                If j = foodN.Count - 1 Then
    '                    Exit For
    '                End If
    '            Next

    '            pageList.Add(fList)
    '        Next

    '        Return New NoteMealSearchAreaPartialViewModel(pageViewModel) With {
    '            .SearchedMealItemN = pageList,
    '            .SearchedMaxPage = pageCount
    '        }
    '    Else
    '        Return New NoteMealSearchAreaPartialViewModel(pageViewModel) With {
    '            .SearchedMealItemN = New List(Of List(Of FoodItem))(),
    '            .SearchedMaxPage = 0
    '        }
    '    End If

    'End Function

    ''' <summary>
    ''' DataURL 形式の画像データを検証し、
    ''' 画像のバイト配列へ変換します。
    ''' </summary>
    ''' <param name="fileString">DataURL 形式の画像データ。</param>
    ''' <param name="fileName">画像ファイル名。</param>
    ''' <param name="refData">変換結果の画像のバイト配列が格納される変数。</param>
    ''' <param name="refName">変換結果の画像を表すファイル名が格納される変数。</param>
    ''' <param name="refMessage">エラーメッセージが格納される変数。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function ToPhotoData(fileString As String, fileName As String, ByRef refData() As Byte, ByRef refName As String, ByRef refMessage As String) As Boolean

        refData = Nothing
        refName = String.Empty
        refMessage = "jpg、jpeg、png、bmpファイルを選択してください。"

        Const mimeType As String = "data:image/jpeg;base64,"

        Dim extensions As New HashSet(Of String)({".jpg", ".jpeg", ".png", ".bmp"})
        Dim result As Boolean = False

        If Not String.IsNullOrWhiteSpace(fileString) AndAlso fileString.StartsWith(mimeType) AndAlso Not String.IsNullOrWhiteSpace(fileName) Then
            Try
                ' ファイルの拡張子をチェック
                If extensions.Contains(IO.Path.GetExtension(fileName).ToLower()) Then
                    refData = Convert.FromBase64String(fileString.Remove(0, mimeType.Length))

                    ' マジック ナンバーをチェック（データは jpg のはず）
                    If refData.Length > 2 AndAlso refData(0) = &HFF AndAlso refData(1) = &HD8 Then
                        refName = IO.Path.GetFileNameWithoutExtension(fileName) + ".jpg"

                        If refName.Length <= 100 Then
                            refMessage = String.Empty
                            result = True
                        Else
                            refMessage = "ファイル名が100文字以下のファイルを選択してください。"
                        End If
                    End If
                End If
            Catch
            End Try
        End If

        If Not result Then
            refData = Nothing
            refName = String.Empty
        End If

        Return result

    End Function

    ''' <summary>
    ''' 食事情報を登録します（検索登録）。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <param name="photoData">画像データ。</param>
    ''' <param name="photoName">画像ファイル名。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function AddSearch(mainModel As QolmsYappliModel, inputModel As NoteMealListInputModel, photoData() As Byte, photoName As String) As Boolean

        Dim result As Boolean = False

        inputModel.ForeignKey.Clear()

        'AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("AddSearch"))
        ' 画像があれば登録（画像の有効性は事前にチェック）
        If photoData IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(photoName) Then
            ' 画像有り
            Dim fileKey As Guid = Guid.Empty

            If AttachedFileWorker.SetMealFile(mainModel, photoName, photoData, fileKey) Then
                ' 成功
                inputModel.ForeignKey.Add(fileKey)
            Else
                ' 失敗
                inputModel.ForeignKey.Add(Guid.Empty)
            End If
        Else
            ' 画像無し
            inputModel.ForeignKey.Add(Guid.Empty)
        End If

        ' 検索されてクリックされた品目の情報を FoodItem として 1 つのみ持つ可能性がある
        inputModel.FoodN.Clear()
        inputModel.FoodN.Add(New List(Of FoodItem))

        If inputModel.PalString.Any() Then
            If Not String.IsNullOrWhiteSpace(inputModel.PalString(0)) Then
                Dim palStr() As String = inputModel.PalString(0).Split({","}, StringSplitOptions.None)

                If palStr IsNot Nothing AndAlso palStr.Count() >= 6 Then
                    inputModel.FoodN(0).Add(
                        New FoodItem() With {
                            .label = inputModel.ItemName(0),
                            .calorie = inputModel.Calorie(0),
                            .protein = palStr(0),
                            .lipid = palStr(1),
                            .carbohydrate = palStr(2),
                            .salt_amount = palStr(3),
                            .available_carbohydrate = palStr(4),
                            .fiber = palStr(5)
                        }
                    )
                End If
            End If
        End If

        ' 食事イベントを登録
        With NoteMealWorker4.ExecuteNoteMealWriteApi(mainModel, inputModel, False, False)
            ' 成功
            If .IsSuccess.TryToValueType(False) Then
                ' 「ホーム」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                ' ポイント 付与
                If .CanGivePoint.TryToValueType(False) Then

                    Dim limit As Date = New Date(inputModel.RecordDate.Year, inputModel.RecordDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                    Dim pointMaxDay As Date = Date.Now.Date ' ポイント付与範囲終了日（今日）
                    Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）
                    Dim pointType As QyPointItemTypeEnum = QyPointItemTypeEnum.None

                    ' ポイント付与（対象は測定日）
                    Dim key As Date = inputModel.RecordDate

                    If (key >= pointMinDay And key <= pointMaxDay) Then
                        'Select Case inputModel.MealType
                        '    Case QyMealTypeEnum.Breakfast
                        '        pointType = QyPointItemTypeEnum.Breakfast

                        '    Case QyMealTypeEnum.Lunch
                        '        pointType = QyPointItemTypeEnum.Lunch

                        '    Case QyMealTypeEnum.Dinner
                        '        pointType = QyPointItemTypeEnum.Dinner

                        '    Case QyMealTypeEnum.Snacking
                        '        pointType = QyPointItemTypeEnum.Snack

                        '    Case Else
                        '        pointType = QyPointItemTypeEnum.None

                        'End Select

                        ' 2022/2/28から食事は種別に関わらず1日1ポイントに変更
                        pointType = QyPointItemTypeEnum.Meal

                        ' 非同期で付与
                        Task.Run(
                            Sub()
                                QolmsPointWorker.AddQolmsPoints(
                                    mainModel.ApiExecutor,
                                    mainModel.ApiExecutorName,
                                    mainModel.SessionId,
                                    mainModel.ApiAuthorizeKey,
                                    mainModel.AuthorAccount.AccountKey,
                                    {
                                        New QolmsPointGrantItem(
                                            mainModel.AuthorAccount.MembershipType,
                                            Date.Now,
                                            Guid.NewGuid().ToApiGuidString(),
                                            pointType,
                                            limit,
                                            inputModel.RecordDate
                                        )
                                    }.ToList()
                                )
                            End Sub
                        )
                    End If

                End If

            End If

            result = True
        End With

        Return result

    End Function

    ''' <summary>
    ''' 食事情報を登録します（写真から登録）。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function AddPhoto(mainModel As QolmsYappliModel, inputModel As NoteMealListInputModel) As Boolean

        Dim result As Boolean = False

        'AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("AddPhoto"))
        ' 食事イベントを登録
        With NoteMealWorker4.ExecuteNoteMealWriteApi(mainModel, inputModel, False, False)
            ' 成功
            If .IsSuccess.TryToValueType(False) Then
                ' 「ホーム」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                ' ポイント 付与
                If .CanGivePoint.TryToValueType(False) Then

                    Dim limit As Date = New Date(inputModel.RecordDate.Year, inputModel.RecordDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                    Dim pointMaxDay As Date = Date.Now.Date ' ポイント付与範囲終了日（今日）
                    Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）
                    Dim pointType As QyPointItemTypeEnum = QyPointItemTypeEnum.None

                    ' ポイント付与（対象は測定日）
                    Dim key As Date = inputModel.RecordDate

                    If (key >= pointMinDay And key <= pointMaxDay) Then
                        'Select Case inputModel.MealType
                        '    Case QyMealTypeEnum.Breakfast
                        '        pointType = QyPointItemTypeEnum.Breakfast

                        '    Case QyMealTypeEnum.Lunch
                        '        pointType = QyPointItemTypeEnum.Lunch

                        '    Case QyMealTypeEnum.Dinner
                        '        pointType = QyPointItemTypeEnum.Dinner

                        '    Case QyMealTypeEnum.Snacking
                        '        pointType = QyPointItemTypeEnum.Snack

                        '    Case Else
                        '        pointType = QyPointItemTypeEnum.None

                        'End Select

                        ' 2022/2/28から食事は種別に関わらず1日1ポイントに変更
                        pointType = QyPointItemTypeEnum.Meal

                        ' 非同期で付与
                        Task.Run(
                            Sub()
                            QolmsPointWorker.AddQolmsPoints(
                                mainModel.ApiExecutor,
                                mainModel.ApiExecutorName,
                                mainModel.SessionId,
                                mainModel.ApiAuthorizeKey,
                                mainModel.AuthorAccount.AccountKey,
                                {
                                    New QolmsPointGrantItem(
                                        mainModel.AuthorAccount.MembershipType,
                                        Date.Now,
                                        Guid.NewGuid().ToApiGuidString(),
                                        pointType,
                                        limit,
                                        inputModel.RecordDate
                                    )
                                }.ToList()
                            )
                        End Sub
                        )
                    End If

                End If

            End If

            result = True
        End With

        Return result

    End Function

    ''' <summary>
    ''' 食事情報を登録します（履歴から登録）。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function AddHistory(mainModel As QolmsYappliModel, inputModel As NoteMealListInputModel) As Boolean

        Dim result As Boolean = False

        ' 選択したデータの画像を取得し、再登録を行う
        Dim foreignKeys As New List(Of Guid)()

        For Each photoKey As Guid In inputModel.ForeignKey
            Dim photoData() As Byte = Nothing
            Dim photoName As String = String.Empty
            Dim fileKey As Guid = Guid.Empty

            ' 画像がある場合は画像を取得して登録を行う
            If photoKey <> Guid.Empty Then
                ' 画像を取得
                photoData = AttachedFileWorker.GetMealFile(mainModel, photoKey, QyFileTypeEnum.Original, photoName)

                If photoData IsNot Nothing Then
                    ' 画像を登録
                    AttachedFileWorker.SetMealFile(mainModel, photoName, photoData, fileKey)
                End If
            End If

            foreignKeys.Add(fileKey)
        Next

        inputModel.ForeignKey.Clear()
        inputModel.ForeignKey = foreignKeys

        'AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("データ作成"))

        ' 食事イベントを登録
        With NoteMealWorker4.ExecuteNoteMealWriteApi(mainModel, inputModel, False, True)
            ' 成功
            If .IsSuccess.TryToValueType(False) Then
                ' 「ホーム」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                ' ポイント 付与
                If .CanGivePoint.TryToValueType(False) Then

                    Dim limit As Date = New Date(inputModel.RecordDate.Year, inputModel.RecordDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                    Dim pointMaxDay As Date = Date.Now.Date ' ポイント付与範囲終了日（今日）
                    Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）
                    Dim pointType As QyPointItemTypeEnum = QyPointItemTypeEnum.None

                    ' ポイント付与（対象は測定日）
                    Dim key As Date = inputModel.RecordDate

                    If (key >= pointMinDay And key <= pointMaxDay) Then
                        'Select Case inputModel.MealType
                        '    Case QyMealTypeEnum.Breakfast
                        '        pointType = QyPointItemTypeEnum.Breakfast

                        '    Case QyMealTypeEnum.Lunch
                        '        pointType = QyPointItemTypeEnum.Lunch

                        '    Case QyMealTypeEnum.Dinner
                        '        pointType = QyPointItemTypeEnum.Dinner

                        '    Case QyMealTypeEnum.Snacking
                        '        pointType = QyPointItemTypeEnum.Snack

                        '    Case Else
                        '        pointType = QyPointItemTypeEnum.None

                        'End Select

                        ' 2022/2/28から食事は種別に関わらず1日1ポイントに変更
                        pointType = QyPointItemTypeEnum.Meal

                        ' 非同期で付与
                        Task.Run(
                            Sub()
                            QolmsPointWorker.AddQolmsPoints(
                                mainModel.ApiExecutor,
                                mainModel.ApiExecutorName,
                                mainModel.SessionId,
                                mainModel.ApiAuthorizeKey,
                                mainModel.AuthorAccount.AccountKey,
                                {
                                    New QolmsPointGrantItem(
                                        mainModel.AuthorAccount.MembershipType,
                                        Date.Now,
                                        Guid.NewGuid().ToApiGuidString(),
                                        pointType,
                                        limit,
                                        inputModel.RecordDate
                                    )
                                }.ToList()
                            )
                        End Sub
                        )
                    End If

                End If

            End If

            result = True
        End With

        Return result

    End Function

    ''' <summary>
    ''' 食事情報を登録します（「編集」領域）。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <param name="photoData">画像データ。</param>
    ''' <param name="photoName">画像ファイル名。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function Edit(mainModel As QolmsYappliModel, inputModel As NoteMealListInputModel, photoData() As Byte, photoName As String) As Boolean

        Dim result As Boolean = False

        inputModel.ForeignKey.Clear()

        ' 画像があれば登録（画像の有効性は事前にチェック）
        If photoData IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(photoName) Then
            ' 画像有り
            Dim fileKey As Guid = Guid.Empty

            If AttachedFileWorker.SetMealFile(mainModel, photoName, photoData, fileKey) Then
                ' 成功
                inputModel.ForeignKey.Add(fileKey)
            Else
                ' 失敗
                inputModel.ForeignKey.Add(Guid.Empty)
            End If
        Else
            ' 画像無し
            inputModel.ForeignKey.Add(Guid.Empty)
        End If

        ' 食事イベントを登録
        With NoteMealWorker4.ExecuteNoteMealWriteApi(mainModel, inputModel, False, False)
            ' 成功
            If .IsSuccess.TryToValueType(False) Then
                ' 「ホーム」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                result = True
            End If
        End With

        Return result

    End Function

    ''' <summary>
    ''' 食事情報を削除します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="recordDate">削除対象の記録日時。</param>
    ''' <param name="mealType">削除対象の食事種別。</param>
    ''' <param name="sequence">削除対象の日時・種別内連番。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function Delete(mainModel As QolmsYappliModel, recordDate As Date, mealType As Byte, sequence As Integer) As Boolean

        If recordDate = Date.MinValue Then Throw New ArgumentOutOfRangeException("recordDate", "記録日時が不正です。")
        If mealType = Byte.MinValue Then Throw New ArgumentOutOfRangeException("mealType", "食事種別が不正です。")
        If sequence < 1 Then Throw New ArgumentOutOfRangeException("sequence", "日時・種別内連番が不正です。")

        Dim result As Boolean = False
        Dim inputModel As New NoteMealListInputModel() With {
            .RecordDate = recordDate,
            .MealType = mealType.ToString().TryToValueType(QyMealTypeEnum.None)
        }
        inputModel.Sequence.Add(sequence)

        ' API を実行
        With NoteMealWorker4.ExecuteNoteMealWriteApi(mainModel, inputModel, True, False)
            If .IsSuccess.TryToValueType(False) Then
                ' 「ホーム」画面のキャッシュをクリア
                mainModel.RemoveInputModelCache(Of PortalHomeViewModel)()

                result = True
            End If
        End With

        Return result

    End Function

    ''' <summary>
    ''' 食事情報の表示を絞り込みます。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="filterDate">絞り込み対象の日付。</param>
    ''' <param name="forceRead">
    ''' キャッシュを使用せずに、
    ''' ビューモデルを取得し直すかのフラグ（オプショナル）。
    ''' 取得し直すなら True、
    ''' 取得し直さないなら False（デフォルト）を指定します。
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Filter(mainModel As QolmsYappliModel, filterDate As Date, Optional forceRead As Boolean = False) As NoteMealCardAreaPartialViewModel4

        Dim pageViewModel As NoteMealViewModel4 = NoteMealWorker4.GetViewModelCache(mainModel)

        If forceRead OrElse pageViewModel.FilterDate <> filterDate Then
            ' API を実行
            With NoteMealWorker4.ExecuteNoteMealReadApi(mainModel, filterDate)
                pageViewModel.FilterDate = filterDate
                pageViewModel.MealItemN = .MealItemN.ConvertAll(Function(i) i.ToMealItem())

                ' キャッシュへ追加
                mainModel.SetInputModelCache(pageViewModel)
            End With
        End If

        Return New NoteMealCardAreaPartialViewModel4(pageViewModel)

    End Function

    ''' <summary>
    ''' 写真を登録して解析します（写真から登録）。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="inputModel">インプット モデル。</param>
    ''' <param name="photoData">画像データ。</param>
    ''' <param name="photoName">画像ファイル名。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function AnalyzePhoto(mainModel As QolmsYappliModel, inputModel As NoteMealListInputModel, photoData() As Byte, photoName As String) As NoteMealListInputModel

        Dim result As New NoteMealListInputModel()
        Dim foodItemN As New List(Of FoodItem)()
        Dim fileKey As Guid = Guid.Empty

        ' 食事写真識別 API を実行
        Try
            foodItemN = NoteMealWorker4.ExecuteImageAnalysisSend2Api(photoName, photoData)
        Catch
        End Try

        ' 画像を登録（画像の有効性は事前にチェック）
        If AttachedFileWorker.SetMealFile(mainModel, photoName, photoData, fileKey) Then
            ' 成功
            result.ForeignKey.Add(fileKey)

            Dim pageList As New List(Of List(Of FoodItem))()
            Dim pageCount As Integer = 0
            Dim dispedPage As Integer = 0

            If foodItemN.Any() Then
                result.ItemName.Add(foodItemN.Item(0).label)

                Dim decimalValue As Decimal = Decimal.MinValue

                If Not String.IsNullOrWhiteSpace(foodItemN.Item(0).calorie) _
                    AndAlso Decimal.TryParse(foodItemN.Item(0).calorie.Trim(), decimalValue) _
                    AndAlso decimalValue >= 0 Then

                    result.Calorie.Add(decimalValue.ToString())
                End If

                ' ページ単位へ分割
                pageCount = Convert.ToInt32(Math.Ceiling(foodItemN.Count / 10D))

                For i As Integer = 0 To pageCount - 1
                    Dim fList As New List(Of FoodItem)()

                    For j As Integer = i * 10 To i * 10 + 9
                        fList.Add(foodItemN.Item(j))

                        If j = foodItemN.Count - 1 Then
                            Exit For
                        End If
                    Next

                    pageList.Add(fList)
                Next

                dispedPage = 1
            Else
                result.ItemName.Add("不明")
                result.Calorie.Add("0")
            End If

            result.FoodN = pageList
            result.SearchedMaxPage = pageCount
            result.DispedPage = dispedPage
        End If

        Return result

    End Function

    ''' <summary>
    ''' 添付ファイルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="fileType">ファイルの種別。</param>
    ''' <param name="refOriginalName">取得したファイルのオリジナル ファイル名が格納される変数。</param>
    ''' <param name="refContentType">取得したファイルの MIME タイプが格納される変数。</param>
    ''' <returns>
    ''' ファイルのバイト配列。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetFile(mainModel As QolmsYappliModel, fileKey As Guid, fileType As QyFileTypeEnum, ByRef refOriginalName As String, ByRef refContentType As String) As Byte()

        Return AttachedFileWorker.GetFile(mainModel, fileKey, fileType, QyFileRelationTypeEnum.MealPhoto, refOriginalName, refContentType)

    End Function

    ''' <summary>
    ''' 仮アップロードされた添付ファイルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="tempFileKey">仮アップロード時のファイル キー。</param>
    ''' <param name="fileType">ファイルの種別。</param>
    ''' <param name="refOriginalName">取得したファイルのオリジナル ファイル名が格納される変数。</param>
    ''' <param name="refContentType">取得したファイルの MIME タイプが格納される変数。</param>
    ''' <returns>
    ''' ファイルのバイト配列。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetTempFile(mainModel As QolmsYappliModel, tempFileKey As String, fileType As QyFileTypeEnum, ByRef refOriginalName As String, ByRef refContentType As String) As Byte()

        Return AttachedFileWorker.GetTempFile(mainModel, tempFileKey, fileType, refOriginalName, refContentType)

    End Function


#End Region

End Class

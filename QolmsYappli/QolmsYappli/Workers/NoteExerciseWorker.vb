'Imports System.Threading
Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
'Imports MGF.QOLMS.QolmsAzureStorageCoreV1

''' <summary>
''' 「運動」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class NoteExerciseWorker

    '#Region "Constant"

    '    ''' <summary>
    '    ''' ダミーのセッションIDを現します。
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

    '    ''' <summary>
    '    ''' ダミーのAPI認証キーを表します。
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    '#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 運動の情報を作成します。
    ''' </summary>
    ''' <param name="inputModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function MakeExerciseItem(inputModel As NoteExerciseInputModel) As QhApiExerciseItem

        Dim result() As Byte = Nothing

        Dim exdate As Date = Date.MinValue

        If Date.TryParse(String.Format("#{0} {1}:{2}:00 {3}#", inputModel.RecordDate.ToShortDateString(), inputModel.Hour, inputModel.Minute, inputModel.Meridiem), exdate) Then
        Else
            exdate = inputModel.RecordDate
        End If

        Return New QhApiExerciseItem() With {
            .ExerciseDate = exdate.ToApiDateString(),
            .ExerciseType = inputModel.ExerciseType.ToString(),
            .ExerciseName = inputModel.ItemName,
            .Calorie = inputModel.Calorie,
            .Sequence = inputModel.Sequence.ToString(),
            .PhotoKey = inputModel.ForeignKey
        }

    End Function

    ''' <summary>
    ''' 「運動」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteExerciseReadApi(mainModel As QolmsYappliModel) As QhYappliNoteExerciseReadApiResults

        Dim apiArgs As New QhYappliNoteExerciseReadApiArgs(
        QhApiTypeEnum.YappliNoteExerciseRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString
        }

        Dim apiResults As QhYappliNoteExerciseReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteExerciseReadApiResults)(
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
    ''' 「運動」画面登録 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <param name="inputModel">インプットモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteExerciseEditWriteApi(mainModel As QolmsYappliModel, inputModel As NoteExerciseInputModel, deleteFlag As Boolean) As QhYappliNoteExerciseWriteApiResults

        Dim result As Boolean = False

        Dim exitem As QhApiExerciseItem = MakeExerciseItem(inputModel)

        Dim apiArgs As New QhYappliNoteExerciseWriteApiArgs(
            QhApiTypeEnum.YappliNoteExerciseWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .ExcerciseItem = exitem,
            .DeleteFlag = deleteFlag
        }
        Dim apiResults As QhYappliNoteExerciseWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteExerciseWriteApiResults)(
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

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, Optional targetDate As Nullable(Of Date) = Nothing) As NoteExerciseViewModel

        Dim recordDate As Date = Date.Now
        If targetDate.HasValue AndAlso targetDate.Value > Date.MinValue Then
            recordDate = New Date(targetDate.Value.Year, targetDate.Value.Month, targetDate.Value.Day, recordDate.Hour, recordDate.Minute, 0)
        End If


        ' API を実行
        With NoteExerciseWorker.ExecuteNoteExerciseReadApi(mainModel)

            Return New NoteExerciseViewModel(
                mainModel,
                recordDate,
                .ExerciseItemN.ConvertAll(Function(i) i.ToExerciseItem),
                .ExerciseStampN.ConvertAll(Function(i) i.ToExerciseItem),
                .ExerciseStringN.ConvertAll(Function(i) i.ToExerciseItem)
                )

        End With

    End Function

    ''' <summary>
    ''' 画面からの入力内容を登録します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <param name="inputModel">インプットモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function Edit(mainModel As QolmsYappliModel, inputModel As NoteExerciseInputModel) As Boolean

        If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプットモデルがNull参照です。")

        Dim recordDate As Date = Date.MinValue

        ' API を実行
        With NoteExerciseWorker.ExecuteNoteExerciseEditWriteApi(mainModel, inputModel, False)
            If .IsSuccess.TryToValueType(False) Then
                ' ポイント 付与
                If .CanGivePoint.TryToValueType(False) Then
                    Dim limit As Date = New Date(inputModel.RecordDate.Year, inputModel.RecordDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント 有効期限は 6 ヶ月後の月末（起点は測定日時）
                    Dim pointMaxDay As Date = Date.Now.Date ' ポイント付与範囲終了日（今日）
                    Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）

                    ' ポイント付与（対象は測定日）
                    Dim key As Date = inputModel.RecordDate
                    If (key >= pointMinDay And key <= pointMaxDay) Then
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
                                            Guid.NewGuid().ToApiGuidString,
                                            QyPointItemTypeEnum.Exercise,
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
        End With

        Return True

    End Function

    ''' <summary>
    ''' 運動の情報を削除します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function Delete(mainModel As QolmsYappliModel, RecordDate As Date, ExerciseType As Byte, Sequence As Integer, Calorie As String) As Boolean

        If RecordDate = Date.MinValue Then Throw New ArgumentNullException("recordDate", "評価日が不正です。")
        If ExerciseType = Byte.MinValue Then Throw New ArgumentNullException("exerciseType", "運動種別が不正です。")
        If Sequence = Integer.MinValue Then Throw New ArgumentNullException("sequence", "日付内連番が不正です。")
        If String.IsNullOrWhiteSpace(Calorie) Then Throw New ArgumentNullException("calorie", "カロリーが不正です。")

        Dim inputModel As New NoteExerciseInputModel() With {
            .RecordDate = RecordDate,
            .ExerciseType = ExerciseType,
            .Sequence = Sequence,
            .Calorie = Calorie
        }

        ' API を実行
        With NoteExerciseWorker.ExecuteNoteExerciseEditWriteApi(mainModel, inputModel, True)
            If .IsSuccess.TryToValueType(False) Then
                'AndAlso .IsChanged.TryToValueType(False) Then

            End If
        End With

        Return True

    End Function

    ' TODO: 「運動」画面はモデルの構造を見直したほうが良い
    ' パーシャル ビュー モデルとインプット モデルをページ ビューモデルに含める
    Public Shared Function GetInputModelCache(mainModel As QolmsYappliModel) As NoteExerciseInputModel

        ' キャッシュから取得
        Dim result As NoteExerciseInputModel = mainModel.GetInputModelCache(Of NoteExerciseInputModel)()

        If result Is Nothing Then
            ' インプット モデルを生成しキャッシュへ追加
            mainModel.SetInputModelCache(New NoteExerciseInputModel())

            ' キャッシュから取得
            result = mainModel.GetInputModelCache(Of NoteExerciseInputModel)()
        End If

        Return result

    End Function

#End Region

End Class

Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「ガルフ動画」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class NoteGulfSportsMovieWorker


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
    ''' 「運動」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteGulfMovieReadApi(mainModel As QolmsYappliModel, movieType As Byte) As QhYappliNoteGulfSportsMovieReadApiResults

        Dim apiArgs As New QhYappliNoteGulfSportsMovieReadApiArgs(
        QhApiTypeEnum.YappliNoteGulfSportsMovieRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
                   .MovieType = movieType.ToString()
               }

        Dim apiResults As QhYappliNoteGulfSportsMovieReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteGulfSportsMovieReadApiResults)(
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
    ''' 「ガルフスポーツ」画面登録 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteGulfMovieWriteApi(mainModel As QolmsYappliModel, rercordDate As Date, exerciseType As Byte, calorie As Integer) As QhYappliNoteGulfSportsMovieWriteApiResults

        Dim result As Boolean = False

        Dim apiArgs As New QhYappliNoteGulfSportsMovieWriteApiArgs(
            QhApiTypeEnum.YappliNoteGulfSportsMovieWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .RercordDate = rercordDate.ToApiDateString(),
            .ExerciseType = exerciseType.ToString(),
            .Calorie = calorie.ToString()
        }
        Dim apiResults As QhYappliNoteGulfSportsMovieWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteGulfSportsMovieWriteApiResults)(
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

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, movieType As Byte) As NoteGulfSportsMovieViewModel

        ' API を実行
        With NoteGulfSportsMovieWorker.ExecuteNoteGulfMovieReadApi(mainModel, movieType)

            Dim list As List(Of MovieItem) = .MovieItemN.ConvertAll(Function(i) New MovieItem() With {
                                                                        .Id = i.Id,
                                                                        .ExerciseType = Byte.Parse(i.ExerciseType),
                                                                        .Calorie = Integer.Parse(i.Calorie),
                                                                        .Discription = i.Discription,
                                                                        .Time = i.Time
                                                                    })

            Return New NoteGulfSportsMovieViewModel() With {
                .MovieType = movieType,
                .MovieItemN = list
                }

        End With

    End Function

    Shared Function ExerciseRegister(mainModel As QolmsYappliModel, exerciseType As Byte, calorie As Integer) As Boolean

        Dim result As Boolean = False
        Dim recordDate As Date = Date.Now

        Dim apiresult As QhYappliNoteGulfSportsMovieWriteApiResults = NoteGulfSportsMovieWorker.ExecuteNoteGulfMovieWriteApi(mainModel, recordDate, exerciseType, calorie)
        With apiresult

            If .IsSuccess.TryToValueType(False) Then
                result = True

                If .CanGivePoint.TryToValueType(False) Then
                    ' ポイント付与（対象は操作日時）
                    Dim limit As Date = New Date(recordDate.Year, recordDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                    Dim pointMaxDay As Date = Date.Now.Date ' ポイント付与範囲終了日（今日）
                    Dim pointMinDay As Date = pointMaxDay.AddDays(-6) ' ポイント付与範囲開始日（過去 1 週間）

                    ' ポイント付与（対象は測定日）
                    Dim key As Date = recordDate.Date
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
                                            recordDate
                                        )
                                    }.ToList()
                                )
                            End Sub
                        )
                    End If
                End If
            End If

        End With

        Return result


    End Function

    ' ''' <summary>
    ' ''' 画面からの入力内容を登録します。
    ' ''' </summary>
    ' ''' <param name="mainModel">メインモデル。</param>
    ' ''' <param name="inputModel">インプットモデル。</param>
    ' ''' <returns>
    ' ''' 成功なら True、
    ' ''' 失敗なら例外をスロー。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Shared Function Edit(mainModel As QolmsYappliModel, inputModel As NoteExerciseInputModel) As Boolean

    '    If inputModel Is Nothing Then Throw New ArgumentNullException("inputModel", "インプットモデルがNull参照です。")

    '    Dim recordDate As Date = Date.MinValue

    '    ' API を実行
    '    With NoteGulfSportsMovieWorker.ExecuteNoteGulfMovieWriteApi(mainModel, inputModel, False)


    '    End With

    '    Return True

    'End Function


    '' TODO: 「運動」画面はモデルの構造を見直したほうが良い
    '' パーシャル ビュー モデルとインプット モデルをページ ビューモデルに含める
    'Public Shared Function GetInputModelCache(mainModel As QolmsYappliModel) As NoteExerciseInputModel

    '    ' キャッシュから取得
    '    Dim result As NoteExerciseInputModel = mainModel.GetInputModelCache(Of NoteExerciseInputModel)()

    '    If result Is Nothing Then
    '        ' インプット モデルを生成しキャッシュへ追加
    '        mainModel.SetInputModelCache(New NoteExerciseInputModel())

    '        ' キャッシュから取得
    '        result = mainModel.GetInputModelCache(Of NoteExerciseInputModel)()
    '    End If

    '    Return result

    'End Function

#End Region



End Class

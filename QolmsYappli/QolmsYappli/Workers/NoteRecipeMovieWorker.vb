Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「レシピ動画」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class NoteRecipeMovieWorker


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
    ''' 「レシピ動画」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteRecipeMovieReadApi(mainModel As QolmsYappliModel, movieType As Byte) As QhYappliNoteRecipeMovieReadApiResults

        Dim apiArgs As New QhYappliNoteRecipeMovieReadApiArgs(
        QhApiTypeEnum.YappliNoteRecipeMovieRead,
        QsApiSystemTypeEnum.Qolms,
        mainModel.ApiExecutor,
        mainModel.ApiExecutorName
        ) With {
                   .MovieType = movieType.ToString()
               }

        Dim apiResults As QhYappliNoteRecipeMovieReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteRecipeMovieReadApiResults)(
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

    '    ''' <summary>
    '    ''' 「レシピ動画」画面登録 API を実行します。
    '    ''' </summary>
    '    ''' <param name="mainModel">ログイン済みモデル。</param>
    '    ''' <returns>
    '    ''' Web API 戻り値クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    Private Shared Function ExecuteNoteRecipeMovieWriteApi(mainModel As QolmsYappliModel, rercordDate As Date, exerciseType As Byte, calorie As Integer) As QhYappliNoteRecipeMovieWriteApiResults

    '        Dim result As Boolean = False

    '        Dim apiArgs As New QhYappliNoteRecipeMovieWriteApiArgs(
    '            QhApiTypeEnum.YappliNoteRecipeMovieWrite,
    '            QsApiSystemTypeEnum.Qolms,
    '            mainModel.ApiExecutor,
    '            mainModel.ApiExecutorName
    '        ) With {
    '            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
    '            .RercordDate = rercordDate.ToApiDateString(),
    '            .ExerciseType = exerciseType.ToString(),
    '            .Calorie = calorie.ToString()
    '        }
    '        Dim apiResults As QhYappliNoteRecipeMovieWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteRecipeMovieWriteApiResults)(
    '            apiArgs,
    '            mainModel.SessionId,
    '            mainModel.ApiAuthorizeKey
    '        )

    '        With apiResults
    '            If .IsSuccess.TryToValueType(False) Then
    '                Return apiResults
    '            Else
    '                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '            End If
    '        End With

    '    End Function
#End Region

#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, movieType As Byte) As NoteRecipeMovieViewModel

        ' API を実行
        With NoteRecipeMovieWorker.ExecuteNoteRecipeMovieReadApi(mainModel, movieType)

            Dim list As List(Of RecipeMovieItem) = .MovieItemN.ConvertAll(Function(i) New RecipeMovieItem() With {
                                                                        .Id = i.Id,
                                                                        .Calorie = Integer.Parse(i.Calorie),
                                                                        .Discription = i.Discription,
                                                                        .Time = i.Time,
                                                                        .Imagekey = i.Imagekey.TryToValueType(Guid.Empty),
                                                                        .ItemName = i.ItemName,
                                                                        .MealValue = i.MealValue
                                                                    })
            'Todo : MealValueには登録時に使用するJson文字列を入れます。
            'Imagekeyは今のところブロブのキー（GUID）が入ってる予定。

            Return New NoteRecipeMovieViewModel() With {
                .MovieType = movieType,
                .MovieItemN = list
                }

        End With

    End Function

#End Region



End Class

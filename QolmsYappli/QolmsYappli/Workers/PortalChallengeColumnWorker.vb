Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1


''' <summary>
''' 「チャレンジコラム」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalChallengeColumnWorker

#Region "Constant"

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

    ''' <summary>
    ''' 「チャレンジコラム」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeColumnReadApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeColumnReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeColumnReadApiArgs(
         QhApiTypeEnum.YappliPortalChallengeColumnRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .ChallengeKey = challengekey.ToApiGuidString()
         }

        Dim apiResults As QhYappliPortalChallengeColumnReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeColumnReadApiResults)(
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
    ''' 「チャレンジコラム」画面登録 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeColumnWriteApi(mainModel As QolmsYappliModel, challengekey As Guid, columnNo As Integer, readFlag As Boolean) As QhYappliPortalChallengeColumnWriteApiResults

        Dim apiArgs As New QhYappliPortalChallengeColumnWriteApiArgs(
         QhApiTypeEnum.YappliPortalChallengeColumnWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .Challengekey = challengekey.ToApiGuidString(),
             .ColumnNo = columnNo.ToString(),
             .ReadFlag = readFlag.ToString()
         }

        Dim apiResults As QhYappliPortalChallengeColumnWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeColumnWriteApiResults)(
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

    Private Shared Function ExecutePortalChallengeColumnImageReadApi(mainModel As QolmsYappliModel, challengekey As Guid, imagekey As Guid, p4 As Boolean) As QhYappliPortalChallengeColumnImageReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeColumnImageReadApiArgs(
             QhApiTypeEnum.YappliPortalChallengeColumnImageRead,
             QsApiSystemTypeEnum.Qolms,
             mainModel.ApiExecutor,
             mainModel.ApiExecutorName
             ) With {
                 .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                 .ChallengeKey = challengekey.ToApiGuidString(),
                 .ImegeKey = imagekey.ToString()
             }

        Dim apiResults As QhYappliPortalChallengeColumnImageReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeColumnImageReadApiResults)(
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
    ''' チャレンジコラム画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel, challengekey As Guid, fromPageNo As QyPageNoTypeEnum) As PortalChallengeColumnViewModel

        Dim result As New PortalChallengeColumnViewModel()
        ' API を実行
        With PortalChallengeColumnWorker.ExecutePortalChallengeColumnReadApi(mainModel, challengekey)

            For Each item As QhApiChallengeColumnItem In .ChallengeColumnItemN

                Dim challenge As New ChallengeColumnItem() With {
                .Challengekey = item.Challengekey.TryToValueType(Guid.Empty),
                .ColumnNo = item.ColumnNo.TryToValueType(Integer.MinValue),
                .Content = item.Content.Replace(vbCrLf, "<br/>"),
                .Days = item.Days.TryToValueType(Integer.MinValue),
                .DispOrder = item.DispOrder.TryToValueType(Integer.MinValue),
                .ImageKey = item.ImageKey.TryToValueType(Guid.Empty),
                .ThumbnailKey = item.ThumbnailKey.TryToValueType(Guid.Empty),
                .Title = item.Title,
                .UserDispDate = item.UserDispDate.TryToValueType(Date.MinValue),
                .UserReadDate = item.UserReadDate.TryToValueType(Date.MinValue),
                .UserReadFlag = item.UserReadFlag.TryToValueType(False)
               }

                If challenge.Challengekey <> Guid.Empty AndAlso challenge.ColumnNo > 0 Then
                    result.ChallengeColumnItemN.Add(challenge)
                End If

            Next

            result.FromPageNoType = fromPageNo

            Return result

        End With

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <param name="Challengekey">チャレンジキー。</param>
    ''' <param name="ColumnNo">コラムNO。</param>
    ''' <remarks>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </remarks>
    Public Shared Function GetColumnItem(mainModel As QolmsYappliModel, Challengekey As Guid, ColumnNo As Integer) As PortalChallengeColumnPartialViewModel

        Dim result As New PortalChallengeColumnPartialViewModel()

        Dim model As PortalChallengeColumnViewModel = mainModel.GetInputModelCache(Of PortalChallengeColumnViewModel)()
        Dim columnItem As ChallengeColumnItem = model.ChallengeColumnItemN.Where(Function(i) i.Challengekey = Challengekey AndAlso i.ColumnNo = ColumnNo).FirstOrDefault()

        result.PageViewModel = model
        result.ChallengeColumnItem = columnItem

        result.ExistsNext = True
        result.ExistsBefore = True

        Return result

    End Function

    ''' <summary>
    ''' コラムを既読にします。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="challengekey"></param>
    ''' <param name="columnNo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function ReadColumn(mainModel As QolmsYappliModel, challengekey As Guid, columnNo As Integer) As Boolean

        '既読にするのでReadFlagはTrue固定にする（検討）
        With PortalChallengeColumnWorker.ExecutePortalChallengeColumnWriteApi(mainModel, challengekey, columnNo, True)

            'キャッシュしたViewModelの既読フラグを更新する
            Dim model As PortalChallengeColumnViewModel = mainModel.GetInputModelCache(Of PortalChallengeColumnViewModel)()
            With model.ChallengeColumnItemN.Where(Function(i) i.ColumnNo = columnNo).First()
                .UserReadDate = Date.Now
                .UserReadFlag = True
            End With

            mainModel.SetInputModelCache(model)

            Return .IsSuccess.TryToValueType(False)
        End With

    End Function

    ''' <summary>
    ''' キーを指定して画像を取得します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <param name="imagekey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetImage(mainModel As QolmsYappliModel, imagekey As Guid, ByRef contentType As String, ByRef data As Byte()) As Boolean

        Dim model As PortalChallengeColumnViewModel = mainModel.GetInputModelCache(Of PortalChallengeColumnViewModel)()
        Dim challengekey As Guid = model.ChallengeColumnItemN.First().Challengekey

        Dim res As QhYappliPortalChallengeColumnImageReadApiResults = PortalChallengeColumnWorker.ExecutePortalChallengeColumnImageReadApi(mainModel, challengekey, imagekey, True)

        If (String.Compare(res.ContentType, "Image/jpeg", True) = 0 OrElse String.Compare(res.ContentType, "Image/png", True) = 0) _
            AndAlso Not String.IsNullOrWhiteSpace(res.Data) Then

            contentType = res.ContentType
            Data = Convert.FromBase64String(res.Data)

            Return True
        End If

        Return False
    End Function

#End Region



End Class

Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1


''' <summary>
''' 「チャレンジ」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalChallengeWorker

#Region "Constant"

    ''' <summary>
    ''' 画像のリンク先フォルダの連番を指定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property imageDic As Dictionary(Of Guid, Integer)
        Get
            'チャレンジ画像のリスト連番を保持します（暫定）。
            Return New Dictionary(Of Guid, Integer)() From {
                {Guid.Parse("ded05070-8718-4313-924a-25233e35e218"), 1},
                {Guid.Parse("cdf50ec6-da20-4d47-84de-6f14bf9cec1f"), 2},
                {Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1"), 3},
                {Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c"), 4}
            }
        End Get

    End Property

    ''' <summary>
    ''' 設定値からチャレンジに紐づくフォルダを取得します。
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property ChallengeFolder As New Lazy(Of Dictionary(Of Guid, String))(Function() GetChallengeFolderSetting())

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

    Private Shared Function GetChallengeFolderSetting() As Dictionary(Of Guid, String)

        Dim result As New Dictionary(Of Guid, String)()
        For i As Integer = 1 To 10

            Dim key As String = $"challenge-{i}"

            Try
                Dim value As String = ConfigurationManager.AppSettings(key)
                If Not result.ContainsKey(Guid.Parse(value)) Then
                    result.Add(Guid.Parse(value), key)

                End If

            Catch ex As Exception
            End Try
        Next

        Return result

    End Function

    ''' <summary>
    ''' 「医療機関検索」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeReadApi(mainModel As QolmsYappliModel) As QhYappliPortalChallengeReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeReadApiArgs(
         QhApiTypeEnum.YappliPortalChallengeRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString
         }

        Dim apiResults As QhYappliPortalChallengeReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeReadApiResults)(
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
    ''' チャレンジ画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalChallengeViewModel


        Dim result As New PortalChallengeViewModel()
        ' API を実行

        With PortalChallengeWorker.ExecutePortalChallengeReadApi(mainModel)

            For Each item As QhApiChallengeItem In .ChallengeItemN
                Dim imageStr As String = String.Empty
                'If PortalChallengeWorker.imageDic.ContainsKey(item.Challengekey.TryToValueType(Guid.Empty)) Then
                If ChallengeFolder.Value.ContainsKey(item.Challengekey.TryToValueType(Guid.Empty)) Then
                    imageStr = String.Format("../dist/img/challenge/{0}/", PortalChallengeWorker.ChallengeFolder.Value(item.Challengekey.TryToValueType(Guid.Empty)))
                End If

                Dim challenge As New ChallengeItem() With {
                .Challengekey = item.Challengekey.TryToValueType(Guid.Empty),
                .Description = item.Description,
                .EntryEndDate = item.EntryEndDate.TryToValueType(Date.MinValue),
                .EntryStartDate = item.EntryStartDate.TryToValueType(Date.MinValue),
                .Name = item.Name,
                .Period = item.Period.TryToValueType(Integer.MinValue),
                .StartDate = item.StartDate.TryToValueType(Date.MinValue),
                .StatusType = item.StatusType.TryToValueType(Byte.MinValue),
                .StatusTypeMaster = item.StatusTypeMaster.ConvertAll(Function(i) New ChallengeStatusItem() With {
                                                                         .Challengekey = i.Challengekey.TryToValueType(Guid.Empty),
                                                                         .StatusType = i.StatusType.TryToValueType(Byte.MinValue),
                                                                         .StatusName = i.StatusName
                                                                     }),
                .UserEndDate = item.UserEndDate.TryToValueType(Date.MinValue),
                .UserStartDate = item.UserStartDate.TryToValueType(Date.MinValue),
                .ImageSrc = imageStr
               }
                If challenge.Challengekey <> Guid.Empty Then
                    result.ChallengeItemN.Add(challenge)
                End If

            Next

            Return result

        End With

    End Function

    ''' <summary>
    ''' チャレンジ画面モデルを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら例外をスロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function IsChallengeEntry(mainModel As QolmsYappliModel, challengekey As Guid) As Boolean

        With PortalChallengeWorker.ExecutePortalChallengeReadApi(mainModel)

            Return .IsSuccess.TryToValueType(False) AndAlso _
                .ChallengeItemN.Where(Function(i) Guid.Parse(i.Challengekey) = challengekey AndAlso i.StatusType = "2").Count > 0

        End With

    End Function

    ''' <summary>
    ''' 参加している実施中のチャレンジを取得します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' <returns>
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetChallengeEntryList(mainModel As QolmsYappliModel) As Dictionary(Of Guid, String)

        Dim actionDate As Date = Date.Now
        Dim result As new Dictionary(Of Guid, String)()

        With PortalChallengeWorker.ExecutePortalChallengeReadApi(mainModel)

            If .IsSuccess.TryToValueType(False) Then
                Dim challengeList As List(Of QhApiChallengeItem) = .ChallengeItemN.Where(Function(i) _
                                                                        i.StatusType = "2" AndAlso _
                                                                        i.UserStartDate.TryToValueType(Date.Now) <= actionDate _
                                                                        AndAlso i.UserEndDate.TryToValueType(Date.MinValue) >= actionDate ).ToList()

                For Each item As QhApiChallengeItem In challengeList

                    If not result.ContainsKey(item.Challengekey.TryToValueType(Guid.Empty)) Then

                        result.Add(item.Challengekey.TryToValueType(Guid.Empty),item.Name)
                    End If

                Next

            End If

        End With
                        
        Return  result
            
    End Function

#End Region

End Class

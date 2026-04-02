Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports System.Threading.Tasks


''' <summary>
''' 「チャレンジ完了」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalChallengeCompletedWorker

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
    ''' 「チャレンジ完了」画面取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">ログイン済みモデル。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecutePortalChallengeCompletedReadApi(mainModel As QolmsYappliModel, externalId As String) As QhYappliPortalChallengeCompletedReadApiResults

        Dim apiArgs As New QhYappliPortalChallengeCompletedReadApiArgs(
         QhApiTypeEnum.YappliPortalChallengeCompletedRead,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .AccountKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .ExternalId = externalId
         }

        Dim apiResults As QhYappliPortalChallengeCompletedReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeCompletedReadApiResults)(
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
    Private Shared Function ExecutePortalChallengeCompletedWriteApi(mainModel As QolmsYappliModel, challengekey As Guid) As QhYappliPortalChallengeCompletedWriteApiResults

        Dim apiArgs As New QhYappliPortalChallengeCompletedWriteApiArgs(
         QhApiTypeEnum.YappliPortalChallengeCompletedWrite,
         QsApiSystemTypeEnum.Qolms,
         mainModel.ApiExecutor,
         mainModel.ApiExecutorName
         ) With {
             .AccountKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
             .ChallengeKey = challengekey.ToApiGuidString()
         }

        Dim apiResults As QhYappliPortalChallengeCompletedWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalChallengeCompletedWriteApiResults)(
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
    ''' アンケート完了処理をします。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function Complete(mainModel As QolmsYappliModel, key As String) As Boolean

        'キーを複合化
        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

            Dim externalid As String = crypt.DecryptString(key)

            If Not String.IsNullOrWhiteSpace(externalid) Then

                'キーからアカウントのチャレンジ情報を取得

                With PortalChallengeCompletedWorker.ExecutePortalChallengeCompletedReadApi(mainModel, externalid)

                    If .IsSuccess.TryToValueType(False) AndAlso .ChallengeItem.Challengekey.TryToValueType(Guid.Empty) <> Guid.Empty Then

                        'アカウントキーにポイントをつける（固定）
                        Dim pointActionDate As Date = Date.Now
                        ' ポイント付与（対象は操作日時）
                        Dim limit As Date = New Date(pointActionDate.Year, pointActionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末

                        Dim reason As String = String.Empty
                        Dim pointList As List(Of QolmsPointGrantItem) = {New QolmsPointGrantItem(
                                                mainModel.AuthorAccount.MembershipType,
                                                pointActionDate,
                                                Guid.NewGuid().ToApiGuidString(),
                                                QyPointItemTypeEnum.ChallengeCompleted,
                                                 limit,
                                                reason
                                            )
                                    }.ToList()

                        ' 非同期でポイント付与
                        Task.Run(
                            Sub()
                                QolmsPointWorker.AddQolmsPoints(
                                    mainModel.ApiExecutor,
                                    mainModel.ApiExecutorName,
                                    mainModel.SessionId,
                                    mainModel.ApiAuthorizeKey,
                                    mainModel.AuthorAccount.AccountKey,
                                    pointList)
                            End Sub
                        )
                        'チャレンジ完了へ変更
                        Dim apiResult As QhYappliPortalChallengeCompletedWriteApiResults = PortalChallengeCompletedWorker.ExecutePortalChallengeCompletedWriteApi(mainModel, .ChallengeItem.Challengekey.TryToValueType(Guid.Empty))

                        'ポイント付与が成功したら完了画面を返却
                        Return apiResult.IsSuccess.TryToValueType(False)

                    End If

                End With

            End If

        End Using






        Return False
    End Function

#End Region



End Class

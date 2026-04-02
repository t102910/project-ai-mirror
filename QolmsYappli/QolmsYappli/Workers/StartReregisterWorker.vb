Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

Friend NotInheritable Class StartReregisterWorker

#Region "Constant"

    ''' <summary>
    ''' ダミーのセッションIDを現します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

    ''' <summary>
    ''' ダミーのAPI認証キーを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    ' ''' <summary>
    ' ''' JWTのライフタイム検証をするかどうかを表します。（検証環境は2038/1/19(int32.MaxVale)を超える可能性があるのでOFFできるように）デフォルトはTrue
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Shared JWT_VALIDATE_LIFETIME_FLAG As Boolean = ConfigurationManager.AppSettings("JwtValidateLifetimeFlag").TryToValueType(True)

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    Private Shared Function ExecuteStartRereregisterReadApi(sessionId As String, openId As String, openIdType As Byte) As QiQolmsYappliStartReregisterReadApiResults


        Dim apiArgs As New QiQolmsYappliStartReregisterReadApiArgs(
            QiApiTypeEnum.QolmsYappliStartReregisterRead,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .SessionId = sessionId,
            .OpenId = openId,
            .OpenIdType = openIdType.ToString()
        }
        Dim apiResults As QiQolmsYappliStartReregisterReadApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliStartReregisterReadApiResults)(
            apiArgs,
            StartReregisterWorker.DUMMY_SESSION_ID,
            StartReregisterWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With


    End Function

    Private Shared Function ExecuteStartRereregisterWriteApi(sessionId As String, openId As String, openIdType As Byte, accountkey As Guid) As QiQolmsYappliStartReregisterWriteApiResults

        Dim apiArgs As New QiQolmsYappliStartReregisterWriteApiArgs(
            QiApiTypeEnum.QolmsYappliStartReregisterWrite,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .SessionId = sessionId,
            .OpenId = openId,
            .OpenIdType = openIdType.ToString(),
            .Accountkey = accountkey.ToApiGuidString()
        }
        Dim apiResults As QiQolmsYappliStartReregisterWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliStartReregisterWriteApiResults)(
            apiArgs,
            StartReregisterWorker.DUMMY_SESSION_ID,
            StartReregisterWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function
#End Region

#Region "Public Method"

    Public Shared Function RegisteredOpenId(session As String, openId As String, openIdType As Byte, ByRef accountkey As Guid) As Boolean

        '退会済みユーザーを含めてOpneIDの重複がないかチェックする
        With StartReregisterWorker.ExecuteStartRereregisterReadApi(session, openId, openIdType)

            If .IsSuccess.TryToValueType(False) AndAlso .IsReregister.TryToValueType(False) AndAlso .AccountKey.TryToValueType(Guid.Empty) <> Guid.Empty Then
                '過去に登録があるユーザー
                accountkey = .AccountKey.TryToValueType(Guid.Empty)

            End If

            Return .IsReregister.TryToValueType(False)

        End With

    End Function

    Public Shared Function Reregister(session As String, openId As String, openIdType As Byte, accountkey As Guid) As Boolean

        'アカウントを再開する（deleteFlagを戻す）
        With StartReregisterWorker.ExecuteStartRereregisterWriteApi(session, openId, openIdType, accountkey)

            Return .IsSuccess.TryToValueType(False)

        End With

    End Function

#End Region

End Class

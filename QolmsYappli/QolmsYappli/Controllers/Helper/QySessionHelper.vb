Imports System.Web.Configuration

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' セッション状態オブジェクトに関する補助機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class QySessionHelper

#Region "Constant"

    ''' <summary>
    ''' セッション ID を保持する HTTP クッキー名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly COOKIE_NAME As String = DirectCast(WebConfigurationManager.GetSection("system.web/sessionState"), System.Web.Configuration.SessionStateSection).CookieName.Trim()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' セッションをキャンセルし、
    ''' 次回 HTTP 要求時に新規セッションを開始します。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <param name="response">HTTP 応答。</param>
    ''' <remarks></remarks>
    Public Shared Sub NewSession(session As HttpSessionStateBase, response As HttpResponseBase)

        session.Abandon()
        response.Cookies.Add(New HttpCookie(QySessionHelper.COOKIE_NAME, String.Empty))

    End Sub

    ''' <summary>
    ''' セッション状態オブジェクトから値を取得します。
    ''' </summary>
    ''' <typeparam name="T">取得するオブジェクトの型。</typeparam>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <param name="key">キー名。</param>
    ''' <param name="refValue">取得したオブジェクトが格納される変数。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetItem(Of T)(session As HttpSessionStateBase, key As String, ByRef refValue As T) As Boolean

        Dim result As Boolean = False

        If session(key) IsNot Nothing Then
            Try
                refValue = DirectCast(session(key), T)
                result = True
            Catch
            End Try
        End If

        Return result

    End Function

    ''' <summary>
    ''' セッション状態オブジェクトへ値を追加します。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <param name="key">キー名。</param>
    ''' <param name="value">追加するオブジェクト。</param>
    ''' <remarks></remarks>
    Public Shared Sub SetItem(session As HttpSessionStateBase, key As String, value As Object)

        session.Add(key, value)

    End Sub

    ''' <summary>
    ''' セッション状態オブジェクトから値を削除します。
    ''' </summary>
    ''' <param name="session">セッション状態オブジェクト。</param>
    ''' <param name="key">キー名。</param>
    ''' <remarks></remarks>
    Public Shared Sub RemoveItem(session As HttpSessionStateBase, key As String)

        session.Remove(key)

    End Sub

#End Region

End Class

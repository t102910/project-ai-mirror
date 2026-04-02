Imports System.Collections.ObjectModel
Imports System.Linq.Expressions
Imports System.Reflection
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' メイン モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class QolmsYappliModel

#Region "Constant"

    ''' <summary>
    ''' キャッシュ機能を提供します。
    ''' </summary>
    ''' <remarks></remarks>
    Private ReadOnly cacheManager As New QyCacheManager()

#End Region

#Region "Variable"

#Region "検証用情報"

    ''' <summary>
    ''' JavaScript が有効かを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _enableJavaScript As Boolean = False

    ''' <summary>
    ''' クッキーが有効かを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _enableCookies As Boolean = False

    ''' <summary>
    ''' デバッグ ビルドかを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _isDebug As Boolean = False

    ''' <summary>
    ''' デバッグログのパスを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _debugLogPath As String = String.Empty

#End Region

#Region "Web API 認証情報"

    ''' <summary>
    ''' セッション ID を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _sessionId As String = String.Empty

    ''' <summary>
    ''' QolmsApi 用 API 認証 キー を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _apiAuthorizeKey As Guid = Guid.Empty

    ''' <summary>
    ''' QolmsApi 用 API 認証有効期限を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _apiAuthorizeExpires As Date = Date.MinValue

    ''' <summary>
    ''' QolmsJotoApi 用 API 認証 キー を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _apiAuthorizeKey2 As Guid = Guid.Empty

    ''' <summary>
    ''' QolmsJotoApi 用 API 認証有効期限を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _apiAuthorizeExpires2 As Date = Date.MinValue

#End Region

#Region "アカウント情報"

    ''' <summary>
    ''' 所有者アカウント情報を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _authorAccount As AuthorAccountItem = Nothing

#End Region

#Region "分割アップロードされたファイル"

    ''' <summary>
    ''' 分割アップロードされたファイルを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _uploadedFile As ChunkUploadFile = Nothing

#End Region

#Region "その他"

    ''' <summary>
    ''' セッション内での各画面の表示回数を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _pageViewCounts As New Dictionary(Of QyPageNoTypeEnum, Integer)()

#End Region

#End Region

#Region "Public Property"

    ''' <summary>
    ''' JavaScript が有効かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EnableJavaScript As Boolean

        Get
            Return Me._enableJavaScript
        End Get

    End Property

    ''' <summary>
    ''' クッキーが有効かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EnableCookies As Boolean

        Get
            Return Me._enableCookies
        End Get

    End Property

    ''' <summary>
    ''' デバッグ ビルドかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsDebug As Boolean

        Get
            Return Me._isDebug
        End Get

    End Property

    ''' <summary>
    ''' デバッグ ログのフォルダパスを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DebugLogPath As String

        Get
            Return Me._debugLogPath
        End Get

    End Property
    
    ''' <summary>
    ''' QolmsApi 用 API 認証 キー を取得します。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ApiAuthorizeKey As Guid

        Get
            Return Me._apiAuthorizeKey
        End Get

    End Property

    ''' <summary>
    ''' QolmsApi 用 API 認証有効期限を取得します。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ApiAuthorizeExpires As Date

        Get
            Return Me._apiAuthorizeExpires
        End Get

    End Property

    ''' <summary>
    ''' QolmsJotoApi 用 API 認証 キー を取得します。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ApiAuthorizeKey2 As Guid

        Get
            Return Me._apiAuthorizeKey2
        End Get

    End Property

    ''' <summary>
    ''' QolmsJotoApi 用 API 認証有効期限を取得します。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ApiAuthorizeExpires2 As Date

        Get
            Return Me._apiAuthorizeExpires2
        End Get

    End Property

    ''' <summary>
    ''' セッション ID を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SessionId As String

        Get
            Return Me._sessionId
        End Get

    End Property

    ''' <summary>
    ''' 所有者アカウント情報を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AuthorAccount As AuthorAccountItem

        Get
            Return Me._authorAccount
        End Get

    End Property

    ''' <summary>
    ''' Web API の実行者アカウント キーを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ApiExecutor As Guid

        Get
            Return Me._authorAccount.AccountKey
        End Get

    End Property

    ''' <summary>
    ''' Web API の実行者名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ApiExecutorName As String

        Get
            Return Me._authorAccount.Name
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタ は使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="QolmsYappliModel" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <param name="authorAccount">所有者 アカウント 情報。</param>
    ''' <param name="sessionId">セッション ID。</param>
    ''' <param name="apiAuthorizeKey">QolmsApi 用 API 認証 キー。</param>
    ''' <param name="apiAuthorizeExpires">QolmsApi 用 API 認証期限。</param>
    ''' <param name="apiAuthorizeKey2">QolmsJotoApi 用 API 認証 キー。</param>
    ''' <param name="apiAuthorizeExpires2">QolmsJotoApi 用 API 認証期限。</param>
    ''' <remarks></remarks>
    Public Sub New(authorAccount As AuthorAccountItem, sessionId As String, apiAuthorizeKey As Guid, apiAuthorizeExpires As Date, apiAuthorizeKey2 As Guid, apiAuthorizeExpires2 As Date)

        If authorAccount Is Nothing Then Throw New ArgumentNullException("authorAccount", "所有者アカウント情報が Null 参照です。")
        If authorAccount.AccountKey = Guid.Empty Then Throw New ArgumentOutOfRangeException("authorAccount.AccountKey", "アカウント キーが不正です。")
        If String.IsNullOrWhiteSpace(authorAccount.UserId) Then Throw New ArgumentNullException("authorAccount.UserId", "ユーザー ID が Null 参照もしくは空白です。")
        'If String.IsNullOrWhiteSpace(authorAccount.PasswordHash) Then Throw New ArgumentNullException("authorAccount.PasswordHash", "パスワード ハッシュが Null 参照もしくは空白です。")
        If String.IsNullOrWhiteSpace(sessionId) Then Throw New ArgumentNullException("sessionId", "セッション ID が Null 参照もしくは空白です。")
        If apiAuthorizeKey = Guid.Empty Then Throw New ArgumentOutOfRangeException("apiAuthorizeKey", "API 認証キーが不正です。")
        If apiAuthorizeExpires = Date.MinValue Then Throw New ArgumentOutOfRangeException("apiAuthorizeExpires", "API 認証期限が不正です。")

        Me._authorAccount = authorAccount

        Me._sessionId = sessionId

        ' QolmsApi 用
        Me._apiAuthorizeKey = apiAuthorizeKey
        Me._apiAuthorizeExpires = apiAuthorizeExpires

        ' QolmsJotoApi 用
        Me._apiAuthorizeKey2 = apiAuthorizeKey2
        Me._apiAuthorizeExpires2 = apiAuthorizeExpires2

        'TODO: 各種初期化

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' JavaScript が有効かを設定します。
    ''' </summary>
    ''' <param name="enable"></param>
    ''' <remarks></remarks>
    Public Sub SetEnableJavaScript(enable As Boolean)

        Me._enableJavaScript = enable

    End Sub

    ''' <summary>
    ''' クッキーが有効かを設定します。
    ''' </summary>
    ''' <param name="enable"></param>
    ''' <remarks></remarks>
    Public Sub SetEnableCookies(enable As Boolean)

        Me._enableCookies = enable

    End Sub

    ''' <summary>
    ''' デバッグ ビルドかを設定します。
    ''' </summary>
    ''' <param name="isDebug"></param>
    ''' <remarks></remarks>
    Public Sub SetIsDebug(isDebug As Boolean)

        Me._isDebug = isDebug

    End Sub

    ''' <summary>
    ''' デバッグ ログのフォルダパスを設定します。
    ''' </summary>
    ''' <param name="debugLogPath"></param>
    ''' <remarks></remarks>
    Public Sub SetDebugLogPath(debugLogPath As String)

        Me._debugLogPath = debugLogPath

    End Sub

    ''' <summary>
    ''' セッション ID を設定します。
    ''' </summary>
    ''' <param name="sessionId"></param>
    ''' <remarks></remarks>
    Public Sub SetSessionId(sessionId As String)

        Me._sessionId = sessionId

    End Sub

    ''' <summary>
    ''' QolmsApi 用 API 認証 キー を設定します。
    ''' </summary>
    ''' <param name="key">QolmsApi 用 API 認証 キー。</param>
    Public Sub SetApiAuthorizeKey(key As Guid)

        Me._apiAuthorizeKey = key

    End Sub

    ''' <summary>
    ''' QolmsApi 用 API 認証有効期限を設定します。
    ''' </summary>
    ''' <param name="expires">QolmsApi 用 API 認証有効期限。</param>
    Public Sub SetApiAuthorizeExpires(expires As Date)

        Me._apiAuthorizeExpires = expires

    End Sub

    ''' <summary>
    ''' QolmsJotoApi 用 API 認証 キー を設定します。
    ''' </summary>
    ''' <param name="key">QolmsJotoApi 用 API 認証 キー。</param>
    Public Sub SetApiAuthorizeKey2(key As Guid)

        Me._apiAuthorizeKey2 = key

    End Sub

    ''' <summary>
    ''' QolmsJotoApi 用 API 認証有効期限を設定します。
    ''' </summary>
    ''' <param name="expires">QolmsJotoApi 用 API 認証有効期限。</param>
    Public Sub SetApiAuthorizeExpires2(expires As Date)

        Me._apiAuthorizeExpires2 = expires

    End Sub

    ''' <summary>
    ''' 自動ログインかを設定します。
    ''' </summary>
    ''' <param name="isAutoLogin"></param>
    ''' <remarks></remarks>
    Public Sub SetIsAutoLogin(isAutoLogin As Boolean)

        Me._authorAccount.IsAutoLogin = isAutoLogin

    End Sub

    ''' <summary>
    ''' ビュー モデル内のプロパティ値をキャッシュへ追加します。
    ''' キャッシュ内の既存の値は上書きされます。
    ''' </summary>
    ''' <typeparam name="TModel">ビュー モデルの型。</typeparam>
    ''' <typeparam name="TProperty">プロパティの型。</typeparam>
    ''' <param name="model">プロパティ値が設定されたビュー モデルのインスタンス。</param>
    ''' <param name="expression">キャッシュへ追加するプロパティを格納しているオブジェクトを識別する式。</param>
    ''' <remarks></remarks>
    Public Sub SetModelPropertyCache(Of TModel As QyPageViewModelBase, TProperty)(model As TModel, expression As Expression(Of Func(Of TModel, TProperty)))

        Dim modelName As String = model.GetType().Name
        Dim propertyName As String = DirectCast(expression.Body.GetType().GetProperty("Member").GetValue(expression.Body, Nothing), PropertyInfo).Name
        Dim key As String = String.Format("{0}.{1}", modelName, propertyName)
        Dim value As Object = model.GetType().GetProperty(propertyName).GetValue(model, Nothing)

        Me.cacheManager.RemoveCache(QyCacheManager.QyCacheTypeEnum.ModelProperty, key)
        Me.cacheManager.SetCache(QyCacheManager.QyCacheTypeEnum.ModelProperty, key, value)

    End Sub

    ''' <summary>
    ''' ビュー モデルのプロパティ値をキャッシュから取得します。
    ''' 取得値が Nothing で無ければ、
    ''' その値をビュー モデルのプロパティへ設定します。
    ''' </summary>
    ''' <typeparam name="TModel">ビュー モデルの型。</typeparam>
    ''' <typeparam name="TProperty">プロパティの型。</typeparam>
    ''' <param name="model">プロパティ値が設定されるビュー モデルのインスタンス。</param>
    ''' <param name="expression">キャッシュから取得するプロパティを格納しているオブジェクトを識別する式。</param>
    ''' <remarks></remarks>
    Public Sub GetModelPropertyCache(Of TModel As QyPageViewModelBase, TProperty)(model As TModel, expression As Expression(Of Func(Of TModel, TProperty)))

        Dim modelName As String = model.GetType().Name
        Dim propertyName As String = DirectCast(expression.Body.GetType().GetProperty("Member").GetValue(expression.Body, Nothing), PropertyInfo).Name
        Dim key As String = String.Format("{0}.{1}", modelName, propertyName)
        Dim value As Object = Nothing

        Me.cacheManager.GetCache(QyCacheManager.QyCacheTypeEnum.ModelProperty, key, value)

        If value IsNot Nothing Then model.GetType().GetProperty(propertyName).SetValue(model, value, Nothing)

    End Sub

    ''' <summary>
    ''' インプットモデルをキャッシュへ追加します。
    ''' キャッシュ内の既存の値は上書きされます。
    ''' </summary>
    ''' <typeparam name="TModel">インプットモデルの型。</typeparam>
    ''' <param name="model">プロパティ値が設定されたインプットモデルのインスタンス。</param>
    ''' <remarks></remarks>
    Public Sub SetInputModelCache(Of TModel As QyPageViewModelBase)(model As TModel)

        Dim key As String = model.GetType().Name

        Me.cacheManager.RemoveCache(QyCacheManager.QyCacheTypeEnum.InputModel, key)
        Me.cacheManager.SetCache(QyCacheManager.QyCacheTypeEnum.InputModel, key, model)

    End Sub

    ''' <summary>
    ''' インプット モデルをキャッシュから取得します。
    ''' </summary>
    ''' <typeparam name="TModel">インプット モデルの型。</typeparam>
    ''' <returns>
    ''' 成功ならインプット モデルのインスタンス、
    ''' 失敗なら Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetInputModelCache(Of TModel As QyPageViewModelBase)() As TModel

        Dim result As TModel = Nothing
        Dim key As String = GetType(TModel).Name

        Me.cacheManager.GetCache(QyCacheManager.QyCacheTypeEnum.InputModel, key, result)

        Return result

    End Function

    ''' <summary>
    ''' インプット モデルをキャッシュから削除します。
    ''' </summary>
    ''' <typeparam name="TModel">インプット モデルの型。</typeparam>
    ''' <remarks></remarks>
    Public Sub RemoveInputModelCache(Of TModel As QyPageViewModelBase)()

        Dim key As String = GetType(TModel).Name

        Me.cacheManager.RemoveCache(QyCacheManager.QyCacheTypeEnum.InputModel, key)

    End Sub

    ' ''' <summary>
    ' ''' 添付ファイルのサムネイル画像をキャッシュから取得します。
    ' ''' </summary>
    ' ''' <param name="fileKey">ファイル キー。</param>
    ' ''' <returns>
    ' ''' キャッシュされていれば Web API 戻り値クラス、
    ' ''' キャッシュされていなければ Nothing。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Function GetAttachedFileThumbnail(fileKey As Guid) As QhFileStorageReadApiResults

    '    Dim result As QhFileStorageReadApiResults = Nothing

    '    Me.cacheManager.GetCache(QyCacheManager.QyCacheTypeEnum.Thumbnail, fileKey, result)

    '    Return result

    'End Function

    ' ''' <summary>
    ' ''' 添付ファイルのサムネイル画像をキャッシュから取得します。
    ' ''' </summary>
    ' ''' <param name="fileKey">ファイル キー。</param>
    ' ''' <returns>
    ' ''' キャッシュされていれば Web API 戻り値クラス、
    ' ''' キャッシュされていなければ Nothing。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Function GetAttachedFileThumbnail(fileKey As Guid) As QhBlobStorageReadApiResults

    '    Dim result As QhBlobStorageReadApiResults = Nothing

    '    Me.cacheManager.GetCache(QyCacheManager.QyCacheTypeEnum.Thumbnail, fileKey, result)

    '    Return result

    'End Function

    ''' <summary>
    ''' 添付 ファイル の サムネイル 画像を キャッシュ から取得します。
    ''' </summary>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <returns>
    ''' キャッシュ されていれば Web API 戻り値 クラス、
    ''' キャッシュ されていなければ Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetAttachedFileThumbnail(fileKey As Guid) As QhStorageReadApiResults

        Dim result As QhStorageReadApiResults = Nothing

        Me.cacheManager.GetCache(QyCacheManager.QyCacheTypeEnum.Thumbnail, fileKey, result)

        Return result

    End Function

    ' ''' <summary>
    ' ''' 添付ファイルのサムネイル画像をキャッシュへ設定します。
    ' ''' </summary>
    ' ''' <param name="fileKey">ファイル キー。</param>
    ' ''' <param name="apiResults">キャッシュする Web API 戻り値クラス。</param>
    ' ''' <returns>
    ' ''' 成功ならTrue、
    ' ''' 失敗ならFalse。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Function SetAttachedFileThumbnail(fileKey As Guid, apiResults As QhFileStorageReadApiResults) As Boolean

    '    Return Me.cacheManager.SetCache(QyCacheManager.QyCacheTypeEnum.Thumbnail, fileKey, apiResults)

    'End Function

    ' ''' <summary>
    ' ''' 添付ファイルのサムネイル画像をキャッシュへ設定します。
    ' ''' </summary>
    ' ''' <param name="fileKey">ファイル キー。</param>
    ' ''' <param name="apiResults">キャッシュする Web API 戻り値クラス。</param>
    ' ''' <returns>
    ' ''' 成功ならTrue、
    ' ''' 失敗ならFalse。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Function SetAttachedFileThumbnail(fileKey As Guid, apiResults As QhBlobStorageReadApiResults) As Boolean

    '    Return Me.cacheManager.SetCache(QyCacheManager.QyCacheTypeEnum.Thumbnail, fileKey, apiResults)

    'End Function

    ''' <summary>
    ''' 添付 ファイル の サムネイル 画像を キャッシュ へ設定します。
    ''' </summary>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="apiResults">キャッシュ する Web API 戻り値 クラス。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function SetAttachedFileThumbnail(fileKey As Guid, apiResults As QhStorageReadApiResults) As Boolean

        Return Me.cacheManager.SetCache(QyCacheManager.QyCacheTypeEnum.Thumbnail, fileKey, apiResults)

    End Function

    ''' <summary>
    ''' 添付ファイルのサムネイル画像を全てキャッシュから削除します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearAttachedFileThumbnail()

        Me.cacheManager.ClearCache(QyCacheManager.QyCacheTypeEnum.Thumbnail)

    End Sub

    ''' <summary>
    ''' 分割アップロードされたファイルをキャッシュから取得します。
    ''' </summary>
    ''' <returns>
    ''' キャッシュされていれば分割アップロード ファイル クラス、
    ''' キャッシュされていなければ Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetChunkUploadFile() As ChunkUploadFile

        Return Me._uploadedFile

    End Function

    ''' <summary>
    ''' 分割アップロードされたファイルをキャッシュへ設定します。
    ''' </summary>
    ''' <param name="uploadedFile">分割アップロードされたファイル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function SetChunkUploadFile(uploadedFile As ChunkUploadFile) As Boolean

        Me._uploadedFile = uploadedFile

        Return Me._uploadedFile IsNot Nothing

    End Function

    ''' <summary>
    ''' 分割アップロードされたファイルをキャッシュから削除します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearChunkUploadFile()

        Me._uploadedFile = Nothing

    End Sub

    ''' <summary>
    ''' 仮アップロードされたファイルをキャッシュから取得します。
    ''' </summary>
    ''' <param name="tempFileKey">仮アップロード時のファイル キー。</param>
    ''' <returns>
    ''' キャッシュされていれば仮アップロード ファイル クラス、
    ''' キャッシュされていなければ Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetPostedFile(tempFileKey As String) As PostedFileItem

        Dim result As PostedFileItem = Nothing

        Me.cacheManager.GetCache(QyCacheManager.QyCacheTypeEnum.PostedFile, tempFileKey, result)

        Return result

    End Function

    ''' <summary>
    ''' 仮アップロードされたファイルをキャッシュへ設定します。
    ''' </summary>
    ''' <param name="tempFileKey">仮アップロード時のファイル キー。</param>
    ''' <param name="item">キャッシュする仮アップロード ファイル クラス。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function SetPostedFile(tempFileKey As String, item As PostedFileItem) As Boolean

        Return Me.cacheManager.SetCache(QyCacheManager.QyCacheTypeEnum.PostedFile, tempFileKey, item)

    End Function

    ''' <summary>
    ''' 仮アップロードされたファイルをキャッシュから全て削除します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearPostedFile()

        Me.cacheManager.ClearCache(QyCacheManager.QyCacheTypeEnum.PostedFile)

    End Sub

    ' TODO:
    Public Sub IncrementPageViewCount(pageNo As QyPageNoTypeEnum)

        If pageNo <> QyPageNoTypeEnum.None Then
            If Me._pageViewCounts.ContainsKey(pageNo) Then
                Me._pageViewCounts(pageNo) += 1
            Else
                Me._pageViewCounts.Add(pageNo, 1)
            End If
        End If

    End Sub

    ' TODO:
    Public Function GetPageViewCount(pageNo As QyPageNoTypeEnum) As Integer

        If pageNo <> QyPageNoTypeEnum.None AndAlso Me._pageViewCounts.ContainsKey(pageNo) Then
            Return Me._pageViewCounts(pageNo)
        Else
            Return 0
        End If

    End Function

#End Region

End Class

''' <summary>
''' 分割アップロードされたファイル情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ChunkUploadFile

#Region "Constant"

    ''' <summary>
    ''' ファイルの種類が不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_EXTENSION_ERROR_MESSAGE As String = "pdf、bmp、jpg、jpeg、pngファイルを選択してください。"

    ''' <summary>
    ''' ファイル名の長さが不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_NAME_LENGTH_ERROR_MESSAGE As String = "ファイル名が100文字以下のファイルを選択してください。"

    ''' <summary>
    ''' ファイルの最大サイズが不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_MAX_SIZE_ERROR_MESSAGE As String = "4MB以下のファイルを選択してください。"

    ''' <summary>
    ''' 画像ファイルの種類が不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const IMAGE_FILE_EXTENSION_ERROR_MESSAGE As String = "bmp、jpg、jpeg、pngファイルを選択してください。"

    ''' <summary>
    ''' ファイルの分割サイズを表します（512 KB）。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly fileChunkSize As Integer = 512 * 1024

    ''' <summary>
    ''' 許可するファイルの拡張子と MIME タイプのディクショナリを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly fileContentTypes As New Dictionary(Of String, String)() From {
        {"pdf", "application/pdf"},
        {"bmp", "image/bmp"},
        {"jpg", "image/jpeg"},
        {"jpeg", "image/jpeg"},
        {"png", "image/png"}
    }

    ''' <summary>
    ''' 許可するファイルの最大サイズを表します（4 MB）。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly fileMaxSize As Integer = 1024 * 1024 * 4

    ''' <summary>
    ''' 許可するファイル名の長さを表します（拡張子込みで 100 文字）。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly fileNameLength As Integer = 100

#End Region

#Region "Variable"

    ''' <summary>
    ''' SyncLock 用オブジェクトを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private lockThis As New Object

    ''' <summary>
    ''' ファイル キーを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _key As String = String.Empty

    ''' <summary>
    ''' ファイル名を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _name As String = String.Empty

    ''' <summary>
    ''' ファイル サイズを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _size As Integer = Integer.MinValue

    ''' <summary>
    ''' MIME タイプを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _contentType As String = String.Empty

    ''' <summary>
    ''' ファイルの内容を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _contents() As Byte = Nothing

    ''' <summary>
    ''' ファイルの分割位置と分割サイズのディクショナリを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _slices As New Dictionary(Of Integer, Integer)()

    ''' <summary>
    ''' アップロードが完了したかを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _completed As New HashSet(Of Integer)()

    ''' <summary>
    ''' エラー メッセージを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _errorMessage As String = String.Empty

#End Region

#Region "Public Property"

    ''' <summary>
    ''' ファイル キーを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Key As String

        Get
            Return Me._key
        End Get

    End Property

    ''' <summary>
    ''' ファイル名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Name As String

        Get
            Return Me._name
        End Get

    End Property

    ''' <summary>
    ''' ファイル サイズを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Size As Integer

        Get
            Return Me._size
        End Get

    End Property

    ''' <summary>
    ''' MIME タイプを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ContentType As String

        Get
            Return Me._contentType
        End Get

    End Property

    ''' <summary>
    ''' ファイルの内容を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Contents As Byte()

        Get
            If Me.IsCompleted Then
                Return Me._contents
            Else
                Return Nothing
            End If
        End Get

    End Property

    ''' <summary>
    ''' ファイルの分割位置と分割サイズのディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Slices As Dictionary(Of Integer, Integer)

        Get
            Return Me._slices
        End Get

    End Property

    ''' <summary>
    ''' アップロードが可能かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsReady As Boolean

        Get
            Return Me._contents IsNot Nothing AndAlso Me._contents.Count() = Me._size
        End Get

    End Property

    ''' <summary>
    ''' アップロードの進捗を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Progress As Double

        Get
            Return If(Me.IsReady, Me._completed.Count / Me._slices.Count, 0)
        End Get

    End Property

    ''' <summary>
    ''' アップロードが完了したかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsCompleted As Boolean

        Get
            Return Me.IsReady AndAlso Me._completed.Count = Me._slices.Count
        End Get

    End Property

    ''' <summary>
    ''' エラー メッセージを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ErrorMessage As String

        Get
            Return Me._errorMessage
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="ChunkUploadFile" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="name">ファイル名。</param>
    ''' <param name="size">ファイル サイズ。</param>
    ''' <param name="contentType">MIME タイプ。</param>
    ''' <param name="imageOnly">画像ファイルのみを許可するかのフラグ。</param>
    ''' <remarks></remarks>
    Public Sub New(name As String, size As Integer, contentType As String, imageOnly As Boolean)

        If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentNullException()
        If size <= 0 Then Throw New ArgumentOutOfRangeException()
        If String.IsNullOrWhiteSpace(contentType) Then Throw New ArgumentNullException()

        Me._key = Date.Now.ToString("yyyyMMddHHmmssfffffff")
        Me._name = name
        Me._size = size
        Me._contentType = contentType

        If Me.CheckParams(imageOnly) Then
            Me._contents = New Byte(size - 1) {}

            Dim count As Integer = size \ ChunkUploadFile.fileChunkSize + If(size Mod ChunkUploadFile.fileChunkSize > 0, 1, 0)

            For a As Integer = 0 To count - 1
                Dim key As Integer = a * ChunkUploadFile.fileChunkSize
                Dim value As Integer = If(key + ChunkUploadFile.fileChunkSize <= Me._size, ChunkUploadFile.fileChunkSize, Me._size - key)

                Me._slices.Add(key, value)
            Next
        End If

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' ファイルの拡張子と MIME タイプの組み合わせを検証します。
    ''' </summary>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckExtensionAndContentType() As Boolean

        Dim result As Boolean = False
        Dim ext As String = IO.Path.GetExtension(Me._name).Replace(".", String.Empty).ToLower()
        Dim contentType As String = Me._contentType.Replace(" ", String.Empty).ToLower()

        ' 拡張子をチェック
        If ChunkUploadFile.fileContentTypes.ContainsKey(ext) Then
            ' MIME タイプをチェック
            If contentType.CompareTo(ChunkUploadFile.fileContentTypes(ext)) = 0 Then
                result = True
            ElseIf contentType.StartsWith("image/x-") And contentType.EndsWith("-bmp") Then
                ' 一部 Bitmap 用の回避処理
                result = True
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' ファイルを検証します。
    ''' </summary>
    ''' <param name="imageOnly">画像ファイルのみを許可するかのフラグ。</param>
    ''' <returns>
    ''' ファイルが有効なら True、
    ''' 無効なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckParams(imageOnly As Boolean) As Boolean

        Dim result As Boolean = False
        'Dim isImage As Boolean = False

        ' 最大ファイル サイズを検証
        If Me._size > ChunkUploadFile.fileMaxSize Then
            ' 検証エラー
            Me._errorMessage = ChunkUploadFile.FILE_MAX_SIZE_ERROR_MESSAGE
            result = False
        Else
            result = True
        End If

        ' ファイル名の長さを検証
        If result Then
            If Me._name.Length <= 0 OrElse Me._name.Length > ChunkUploadFile.fileNameLength Then
                ' 検証エラー
                Me._errorMessage = ChunkUploadFile.FILE_NAME_LENGTH_ERROR_MESSAGE
                result = False
            End If
        End If

        ' 拡張子と MIME タイプの組み合わせを検証
        If result Then
            If Not Me.CheckExtensionAndContentType() Then
                ' 検証エラー
                Me._errorMessage = If(imageOnly, ChunkUploadFile.IMAGE_FILE_EXTENSION_ERROR_MESSAGE, ChunkUploadFile.FILE_EXTENSION_ERROR_MESSAGE)
                result = False
            End If
        End If

        ' 画像のみ許可の場合の検証
        If result Then
            If imageOnly And Not Me.ContentType.ToLower.StartsWith("image/") Then
                ' 検証エラー
                Me._errorMessage = ChunkUploadFile.IMAGE_FILE_EXTENSION_ERROR_MESSAGE
                result = False
            End If
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 分割アップロードされたファイルの内容を結合します。
    ''' </summary>
    ''' <param name="key">ファイル キー。</param>
    ''' <param name="position">分割位置。</param>
    ''' <param name="size">分割サイズ。</param>
    ''' <param name="chunk">分割アップロードされたファイルの内容。</param>
    ''' <param name="refProgress">アップロードの進捗が格納される変数。</param>
    ''' <param name="refIsCompleted">アップロードが完了したかが格納される変数。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function MergeChunk(key As String, position As Integer, size As Integer, chunk() As Byte, ByRef refProgress As Double, ByRef refIsCompleted As Boolean) As Boolean

        Dim result As Boolean = False

        refProgress = Me.Progress
        refIsCompleted = Me.IsCompleted

        SyncLock Me.lockThis
            If Me.IsReady _
                AndAlso Me._key.CompareTo(key) = 0 _
                AndAlso Not Me._completed.Contains(position) _
                AndAlso size > 0 _
                AndAlso size <= ChunkUploadFile.fileChunkSize _
                AndAlso position + size <= Me._size Then

                Try
                    Buffer.BlockCopy(chunk, 0, Me._contents, position, size)
                    Me._completed.Add(position)

                    refProgress = Me.Progress
                    refIsCompleted = Me.IsCompleted

                    result = True
                Catch
                End Try
            End If
        End SyncLock

        Return result

    End Function

#End Region

End Class

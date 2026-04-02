Imports System.Security.Cryptography
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

<Serializable()>
Public MustInherit Class QyAccountItemBase

#Region "Public Property"

    ''' <summary>
    ''' アカウント キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AccountKey As Guid = Guid.Empty

    ''' <summary>
    ''' アカウント キー ハッシュ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AccountKeyHash As String = String.Empty

    ''' <summary>
    ''' 漢字姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FamilyName As String = String.Empty

    ''' <summary>
    ''' 漢字 ミドル ネーム を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MiddleName As String = String.Empty

    ''' <summary>
    ''' 漢字名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GivenName As String = String.Empty

    ''' <summary>
    ''' カナ 姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FamilyKanaName As String = String.Empty

    ''' <summary>
    ''' カナ ミドル ネーム を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MiddleKanaName As String = String.Empty

    ''' <summary>
    ''' カナ 名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GivenKanaName As String = String.Empty

    ''' <summary>
    ''' ローマ 字姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FamilyRomanName As String = String.Empty

    ''' <summary>
    ''' ローマ 字 ミドル ネーム を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MiddleRomanName As String = String.Empty

    ''' <summary>
    ''' ローマ 字名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GivenRomanName As String = String.Empty

    ''' <summary>
    ''' 性別の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SexType As QySexTypeEnum = QySexTypeEnum.None

    ''' <summary>
    ''' 生年月日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Birthday As Date = Date.MinValue

    ''' <summary>
    ''' 利用規約同意 フラグ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AcceptFlag As Boolean = False

    ' ''' <summary>
    ' ''' 顔写真 キー を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property PhotoKey As Guid = Guid.Empty

    ''' <summary>
    ''' バイタル 情報の種別および バイタル 標準値の種別を キー、
    ''' API 用の バイタル 目標値情報を値とする、
    ''' バイタル 標準値情報の ディクショナリ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StandardValues As New Dictionary(Of Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum), QhApiTargetValueItem)()

    ''' <summary>
    ''' バイタル 情報の種別および バイタル 標準値の種別を キー、
    ''' API 用の バイタル 目標値情報を値とする、
    ''' バイタル 目標値情報の ディクショナリ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Public Property TargetValues As New Dictionary(Of Tuple(Of QyVitalTypeEnum, QyStandardValueTypeEnum), QhApiTargetValueItem)()

    ''' <summary>
    ''' 身長を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Height As Decimal = Decimal.MinValue

    ''' <summary>
    ''' ビュー 内に展開する暗号化された アカウント キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EncryptedAccountKey As String = String.Empty

    ''' <summary>
    ''' ビュー 内に展開する暗号化された顔写真情報への参照 パラメータ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EncryptedPhotoReference As String = String.Empty

    ''' <summary>
    ''' ビュー 内に展開する暗号化された顔写真 サムネイル 情報への参照 パラメータ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EncryptedThumbnailPhotoReference As String = String.Empty

    ''' <summary>
    ''' 漢字姓名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Name As String

        Get
            Return Me.CreateName()
        End Get

    End Property

    ''' <summary>
    ''' カナ 姓名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property KanaName As String

        Get
            Return Me.CreateKanaName()
        End Get

    End Property

    ''' <summary>
    ''' ローマ 字姓名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RomanName As String

        Get
            Return Me.CreateRomanName()
        End Get

    End Property

    ''' <summary>
    ''' 年齢を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Age As Integer

        Get
            Return Me.ToAge()
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyAccountItemBase" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 漢字姓名を生成します。
    ''' </summary>
    ''' <returns>
    ''' 漢字姓名。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CreateName() As String

        Dim entries As List(Of String) = New List(Of String)({Me.FamilyName, Me.MiddleName, Me.GivenName}).Where(Function(i) Not String.IsNullOrWhiteSpace(i)).ToList()

        If entries.Any() Then
            Return String.Join(" ", entries)
        Else
            Return String.Empty
        End If

    End Function

    ''' <summary>
    ''' カナ 姓名を生成します。
    ''' </summary>
    ''' <returns>
    ''' カナ 姓名。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CreateKanaName() As String

        Dim entries As List(Of String) = New List(Of String)({Me.FamilyKanaName, Me.MiddleKanaName, Me.GivenKanaName}).Where(Function(i) Not String.IsNullOrWhiteSpace(i)).ToList()

        If entries.Any() Then
            Return String.Join(" ", entries)
        Else
            Return String.Empty
        End If

    End Function

    ''' <summary>
    ''' ローマ 字姓名を生成します。
    ''' </summary>
    ''' <returns>
    ''' ローマ 字姓名。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CreateRomanName() As String

        Dim entries As List(Of String) = New List(Of String)({Me.FamilyRomanName, Me.MiddleRomanName, Me.GivenRomanName}).Where(Function(i) Not String.IsNullOrWhiteSpace(i)).ToList()

        If entries.Any() Then
            Return String.Join(" ", entries)
        Else
            Return String.Empty
        End If

    End Function

    ''' <summary>
    ''' 生年月日を年齢へ変換します。
    ''' </summary>
    ''' <returns>
    ''' 成功なら 0 以上の年齢、
    ''' 失敗なら Integer.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function ToAge() As Integer

        Dim result As Integer = Integer.MinValue

        If Me.Birthday <> Date.MinValue Then
            Dim now As Date = Date.Now
            Dim age As Integer = Integer.MinValue

            age = ((now.Year * 10000 + now.Month * 100 + now.Day) - (Birthday.Year * 10000 + Birthday.Month * 100 + Birthday.Day)) \ 10000

            If age >= Byte.MinValue AndAlso age <= Byte.MaxValue Then result = age
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' アカウント キー ハッシュ を生成します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <returns>
    ''' アカウント キー ハッシュ。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateAccountKeyHashString(accountKey As Guid) As String

        Dim result As New StringBuilder()

        Using algorithm As New SHA256CryptoServiceProvider()
            algorithm.ComputeHash(accountKey.ToByteArray()).ToList().ForEach(Sub(i) result.Append(i.ToString("x2")))
        End Using

        Return result.ToString()

    End Function

    ''' <summary>
    ''' ビュー 内に展開する暗号化された アカウント キー を生成します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <returns>
    ''' 暗号化された アカウント キー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function EncryptAccountKey(accountKey As Guid) As String

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
            Return crypt.EncryptString(accountKey.ToApiGuidString())
        End Using

    End Function

    ''' <summary>
    ''' ビュー 内に展開する暗号化された顔写真情報への参照 パラメータ を生成します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <param name="photoKey">顔写真 キー。</param>
    ''' <param name="fileType">ファイル の種別。</param>
    ''' <returns>
    ''' 暗号化された顔写真情報への参照 パラメータ。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function EncryptPhotoReference(accountKey As Guid, photoKey As Guid, fileType As QyFileTypeEnum) As String

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
            Return crypt.EncryptString(
                New FileStorageReferenceJsonParameter() With {
                    .AccountKey = accountKey,
                    .FileKey = photoKey,
                    .TempFileKey = String.Empty,
                    .FileType = fileType
                }.ToJsonString()
            )
        End Using

    End Function

    ''' <summary>
    ''' 暗号化された アカウント キー を復号化します。
    ''' </summary>
    ''' <param name="value">暗号化された アカウント キー。</param>
    ''' <returns>
    ''' 成功なら Guid.Empty 以外、
    ''' 失敗なら Guid.Empty。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function DecryptAccountKey(value As String) As Guid

        Dim result As Guid = Guid.Empty

        If Not String.IsNullOrWhiteSpace(value) Then
            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    result = Guid.Parse(crypt.DecryptString(value))
                End Using
            Catch
            End Try
        End If

        Return result

    End Function

    ' ''' <summary>
    ' ''' 暗号化された顔写真情報への参照パラメータを復号化します。
    ' ''' </summary>
    ' ''' <param name="value">暗号化された顔写真情報への参照パラメータ。</param>
    ' ''' <returns>
    ' ''' 成功なら <see cref="FileStorageReferenceJsonParameter" /> クラス、
    ' ''' 失敗なら Nothing。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Shared Function DecryptPhotoReference(value As String) As FileStorageReferenceJsonParameter

    '    Dim result As FileStorageReferenceJsonParameter = Nothing

    '    If Not String.IsNullOrWhiteSpace(value) Then
    '        Try
    '            Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
    '                result = QhJsonParameterBase.FromJsonString(Of FileStorageReferenceJsonParameter)(crypt.DecryptString(value))
    '            End Using
    '        Catch
    '        End Try
    '    End If

    '    Return result

    'End Function

    ' ''' <summary>
    ' ''' 顔写真画像の URI を生成します。
    ' ''' </summary>
    ' ''' <param name="photoKey">顔写真キー。</param>
    ' ''' <param name="sexType">性別の種別。</param>
    ' ''' <param name="controllerName">コントローラー名。</param>
    ' ''' <param name="actionName">
    ' ''' このアクションメソッドは HTTP GET で処理され、
    ' ''' 引数名 reference（String 型）を受け取る前提です。
    ' ''' </param>
    ' ''' <param name="reference">暗号化された顔写真情報への参照パラメータ。</param>
    ' ''' <returns>
    ' ''' 顔写真画像の URI。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Shared Function CreatePhotoUri(photoKey As Guid, sexType As QhSexTypeEnum, controllerName As String, actionName As String, reference As String) As String

    '    Dim result As String = String.Empty

    '    Select Case True
    '        Case photoKey <> Guid.Empty
    '            ' 顔写真
    '            result = String.Format("../{0}/{1}?reference={2}", controllerName, actionName, reference)

    '        Case photoKey = Guid.Empty And sexType = QhSexTypeEnum.Male
    '            ' 男性
    '            result = "../dist/img/tmpl/ico_male.png"

    '        Case photoKey = Guid.Empty And sexType = QhSexTypeEnum.Female
    '            ' 女性
    '            result = "../dist/img/tmpl/ico_female.png"

    '        Case Else
    '            ' TODO: 性別に依存しない画像
    '            result = "../dist/img/tmpl/ico_male.png"

    '    End Select

    '    Return result

    'End Function

    ''' <summary>
    ''' 顔写真 サムネイル 画像の URI を生成します。
    ''' </summary>
    ''' <param name="photoKey">顔写真 キー。</param>
    ''' <param name="sexType">性別の種別。</param>
    ''' <param name="controllerName">コントローラー 名。</param>
    ''' <param name="actionName">
    ''' この アクション メソッド は HTTP GET で処理され、
    ''' 引数名 reference（String 型）を受け取る前提です。
    ''' </param>
    ''' <param name="reference">暗号化された顔写真 サムネイル 情報への参照 パラメータ。</param>
    ''' <returns>
    ''' 顔写真 サムネイル 画像の URI。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function CreateThumbnailPhotoUri(photoKey As Guid, sexType As QySexTypeEnum, controllerName As String, actionName As String, reference As String) As String

        Dim result As String = String.Empty

        Select Case True
            Case photoKey <> Guid.Empty
                ' 顔写真
                result = String.Format("../{0}/{1}?reference={2}", controllerName, actionName, reference)

            Case photoKey = Guid.Empty And sexType = QySexTypeEnum.Male
                ' 男性
                result = "../dist/img/tmpl/ico_male.png"

            Case photoKey = Guid.Empty And sexType = QySexTypeEnum.Female
                ' 女性
                result = "../dist/img/tmpl/ico_female.png"

            Case Else
                ' TODO: 性別に依存しない画像
                result = "../dist/img/tmpl/ico_male.png"

        End Select

        Return result

    End Function

#End Region

End Class

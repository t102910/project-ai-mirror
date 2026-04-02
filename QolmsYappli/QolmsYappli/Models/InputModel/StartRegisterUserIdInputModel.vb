Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Attribute

Public NotInheritable Class StartRegisterUserIdInputModel
    Implements  _
        IQyModelUpdater(Of StartRegisterUserIdInputModel), 
        IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' ダミーのセッションIDを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared SESSION_ID As String = New String("Z"c, 100)

    ''' <summary>
    ''' ダミーのAPI認証キーを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    ''' <summary>
    ''' 電話番号の最小長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_10 As Integer = 10

    ''' <summary>
    ''' 電話番号の最大長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_11 As Integer = 11

    ''' <summary>
    ''' 漢字姓、
    ''' 漢字名、
    ''' カナ姓、
    ''' カナ名、
    ''' の入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_25 As Integer = 25

    ''' <summary>
    ''' ユーザーIDの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_100 As Integer = 100

    ''' <summary>
    ''' メールアドレスの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_256 As Integer = 256

    ''' <summary>
    ''' パスワードの最小長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_8 As Integer = 8

    ''' <summary>
    ''' パスワードの最大長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_32 As Integer = 32

    ''' <summary>
    ''' ユーザーIDとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const QOLMS_PASSWORD_PATTERN As String = "^[A-Za-z0-9!-/:-@≠\[-`{-~]*$"

    ''' <summary>
    ''' パスワードとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const PASSWORD_PATTERN As String = "^((?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])|(?=.*[a-z])(?=.*[A-Z])(?=.*[!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;])|(?=.*[A-Z])(?=.*[0-9])(?=.*[!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;])|(?=.*[a-z])(?=.*[0-9])(?=.*[!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;]))([a-zA-Z0-9!#$%&()*+,\-./<=>?@\[\\\]\^_{|}~:;]){8,32}$"

    ''' <summary>
    ''' メールアドレスの正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_PATTERN As String = "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z"

#End Region

#Region "Public Property"

    ''' <summary>
    ''' ダミーのセッションIDを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SessionId As String

        Get
            Return StartRegisterUserIdInputModel.SESSION_ID
        End Get

    End Property

    ''' <summary>
    ''' ダミーのAPI認証キーを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ApiAuthorizeKey As Guid

        Get
            Return StartRegisterUserIdInputModel.API_AUTHORIZE_KEY
        End Get

    End Property

    ''' <summary>
    ''' アカウントキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Accountkey As Guid = Guid.Empty

    ''' <summary>
    ''' 漢字姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("姓")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_25, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property FamilyName As String = String.Empty

    ''' <summary>
    ''' 漢字名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("名")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_25, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property GivenName As String = String.Empty

    ''' <summary>
    ''' カナ姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("姓フリガナ")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <RegularExpression("^[\p{IsKatakana}\s]*$", ErrorMessage:="{0}は全角カナ文字で入力してください。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_25, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property FamilyKanaName As String = String.Empty

    ''' <summary>
    ''' カナ名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("名フリガナ")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <RegularExpression("^[\p{IsKatakana}\s]*$", ErrorMessage:="{0}は全角カナ文字で入力してください。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_25, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property GivenKanaName As String = String.Empty

    ''' <summary>
    ''' 性別（M|F）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("性別")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <Range(QySexTypeEnum.Male, QySexTypeEnum.Female, ErrorMessage:="{0}を選択してください。")>
    Public Property Sex As QySexTypeEnum = QySexTypeEnum.None

    ''' <summary>
    ''' 生年月日の年を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <RegularExpression("^[0-9]*$", ErrorMessage:="{0}は半角数字で入力してください。")>
    <StringLength(4, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property BirthYear As String = String.Empty

    ''' <summary>
    ''' 生年月日の月を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <RegularExpression("^[0-9]*$", ErrorMessage:="{0}は半角数字で入力してください。")>
    <StringLength(2, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property BirthMonth As String = String.Empty

    ''' <summary>
    ''' 生年月日の日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <RegularExpression("^[0-9]*$", ErrorMessage:="{0}は半角数字で入力してください。")>
    <StringLength(2, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    Public Property BirthDay As String = String.Empty

    ''' <summary>
    ''' ユーザーIDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("ユーザーID")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_100, MinimumLength:=StartRegisterUserIdInputModel.LENGTH_8, ErrorMessage:="{0}は{2}文字以上、{1}文字以下で入力してください。")>
    <RegularExpression(StartRegisterUserIdInputModel.QOLMS_PASSWORD_PATTERN, ErrorMessage:="{0}に使用できない文字が含まれています。")>
    Public Property UserId As String = String.Empty

    ''' <summary>
    ''' パスワードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("パスワード")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_32, MinimumLength:=StartRegisterUserIdInputModel.LENGTH_8, ErrorMessage:="{0}は{2}文字以上、{1}文字以下で入力してください。")>
    <RegularExpression(StartRegisterUserIdInputModel.PASSWORD_PATTERN, ErrorMessage:="{0}に使用できない文字が含まれています。")>
    Public Property Password As String = String.Empty

    ''' <summary>
    ''' パスワード（確認）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("パスワード（確認）")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_32, MinimumLength:=StartRegisterUserIdInputModel.LENGTH_8, ErrorMessage:="{0}は{2}文字以上、{1}文字以下で入力してください。")>
    <RegularExpression(StartRegisterUserIdInputModel.PASSWORD_PATTERN, ErrorMessage:="{0}に使用できない文字が含まれています。")>
    Public Property Password2 As String = String.Empty

    ''' <summary>
    ''' メールアドレスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("メールアドレス")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    <StringLength(StartRegisterUserIdInputModel.LENGTH_256, ErrorMessage:="{0}は{1}文字以下で入力してください。")>
    <EmailAddress(ErrorMessage:="メールアドレスの形式が不正です。")>
    Public Property MailAddress As String = String.Empty

    ' ''' <summary>
    ' ''' 申請完了フラグを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property SendFlag As Boolean = False

    'Public Property OpenId As String = String.Empty
    'Public Property OpenIdType As Byte = Byte.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="StartRegisterIdInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' インプットモデルの内容を現在のインスタンスに反映します。
    ''' </summary>
    ''' <param name="inputModel">インプットモデル。</param>
    ''' <remarks></remarks>
    Public Sub UpdateByInput(inputModel As StartRegisterUserIdInputModel) Implements IQyModelUpdater(Of StartRegisterUserIdInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.FamilyName = If(String.IsNullOrWhiteSpace(inputModel.FamilyName), String.Empty, .FamilyName.Trim())
                Me.GivenName = If(String.IsNullOrWhiteSpace(inputModel.GivenName), String.Empty, .GivenName.Trim())
                Me.FamilyKanaName = If(String.IsNullOrWhiteSpace(inputModel.FamilyKanaName), String.Empty, .FamilyKanaName.Trim())
                Me.GivenKanaName = If(String.IsNullOrWhiteSpace(inputModel.GivenKanaName), String.Empty, .GivenKanaName.Trim())
                Me.Sex = .Sex
                Me.BirthYear = If(String.IsNullOrWhiteSpace(inputModel.BirthYear), String.Empty, .BirthYear.Trim())
                Me.BirthMonth = If(String.IsNullOrWhiteSpace(inputModel.BirthMonth), String.Empty, .BirthMonth.Trim())
                Me.BirthDay = If(String.IsNullOrWhiteSpace(inputModel.BirthDay), String.Empty, .BirthDay.Trim())
                Me.MailAddress = If(String.IsNullOrWhiteSpace(inputModel.MailAddress), String.Empty, .MailAddress.Trim())
                Me.UserId = If(String.IsNullOrWhiteSpace(inputModel.UserId), String.Empty, .UserId.Trim())
                Me.Password = If(String.IsNullOrWhiteSpace(inputModel.Password), String.Empty, .Password.Trim())
                Me.Password2 = If(String.IsNullOrWhiteSpace(inputModel.Password2), String.Empty, .Password.Trim())
                'Me.SendFlag = inputModel.SendFlag
            End With
        End If

    End Sub

#End Region

#Region "IValidatableObject Support"

    ''' <summary>
    ''' 指定されたオブジェクトが有効かどうかを判断します。
    ''' </summary>
    ''' <param name="validationContext">検証コンテキスト。</param>
    ''' <returns>
    ''' 失敗した検証の情報を保持するコレクション。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()

        ' カナ姓が有効か検証
        If Not String.IsNullOrWhiteSpace(Me.FamilyKanaName) Then
            Dim family As String = StrConv(Me.FamilyKanaName, VbStrConv.Narrow)

            If family.Length > LENGTH_25 Then
                ' 半角カナへ変換後、25文字を超える
                result.Add(New ValidationResult("姓（フリガナ）は25文字以下で入力してください。（※濁点、半濁点は1文字として扱います）", {"FamilyKanaName"}))
            ElseIf family.Length <> Encoding.GetEncoding("shift_jis").GetByteCount(family) Then
                ' 半角カナに変換できない文字が含まれる（ヱ、ヰ、ヮ、ヵなど）
                result.Add(New ValidationResult("姓（フリガナ）に使用出来ない文字が含まれています。", {"FamilyKanaName"}))
            End If
        End If

        ' カナ名が有効か検証
        If Not String.IsNullOrWhiteSpace(Me.GivenKanaName) Then
            Dim given As String = StrConv(Me.GivenKanaName, VbStrConv.Narrow)

            If given.Length > LENGTH_25 Then
                ' 半角カナへ変換後、25文字を超える
                result.Add(New ValidationResult("名（フリガナ）は25文字以下で入力してください。（※濁点、半濁点は1文字として扱います）", {"GivenKanaName"}))
            ElseIf given.Length <> Encoding.GetEncoding("shift_jis").GetByteCount(given) Then
                ' 半角カナに変換できない文字が含まれる（ヱ、ヰ、ヮ、ヵなど）
                result.Add(New ValidationResult("名（フリガナ）に使用出来ない文字が含まれています。", {"GivenKanaName"}))
            End If
        End If

        ' 生年月日を検証
        Dim birthday As Date = Date.MinValue
        Dim BornYear As Integer = Integer.MinValue
        Dim BornMonth As Integer = Integer.MinValue
        Dim BornDay As Integer = Integer.MinValue

        If Not Integer.TryParse(Me.BirthYear, BornYear) _
            OrElse Not Integer.TryParse(Me.BirthMonth, BornMonth) _
            OrElse Not Integer.TryParse(Me.BirthDay, BornDay) _
            OrElse BornYear < 0 _
            OrElse BornMonth < 1 _
            OrElse BornDay < 1 _
            OrElse Not Date.TryParseExact(BornYear.ToString("d4") + BornMonth.ToString("d2") + BornDay.ToString("d2"), "yyyyMMdd", Nothing, Globalization.DateTimeStyles.None, birthday) _
            OrElse birthday = Date.MinValue _
            OrElse birthday > Date.Now.Date Then

            result.Add(New ValidationResult("生年月日が不正です。", {"BirthYear"}))
        End If

        'パスワードの入力検証
        If Not String.IsNullOrWhiteSpace(Me.Password) Then
            If Not Regex.IsMatch(Me.Password, PASSWORD_PATTERN) Then result.Add(New ValidationResult("パスワードの形式が不正です。。", {"Password"}))
            If Me.Password.Length < LENGTH_8 OrElse Me.Password.Length > LENGTH_32 Then result.Add(New ValidationResult("パスワードは8文字以上、32文字以下で入力してください。", {"Password"}))
        Else
            result.Add(New ValidationResult("パスワードに使用できない文字が含まれています。", {"Password"}))
        End If

        If Not String.IsNullOrWhiteSpace(Me.Password) AndAlso Not String.IsNullOrWhiteSpace(Me.Password2) Then
            If Me.Password.Trim().CompareTo(Me.Password2.Trim()) <> 0 Then
                result.Add(New ValidationResult("パスワードと、パスワード（確認）が異なります。", {"Password"}))
            End If
        Else
            result.Add(New ValidationResult("パスワードの形式が不正です。", {"Password"}))
        End If

        ' メールアドレスを検証
        If Not String.IsNullOrWhiteSpace(Me.MailAddress) Then
            Dim eM As New EmailAddressAttribute()
            If Not eM.IsValid(Me.MailAddress) OrElse Not Regex.IsMatch(Me.MailAddress, MAILADDRESS_PATTERN, RegexOptions.IgnoreCase) Then
                result.Add(New ValidationResult("メールアドレスの形式が不正です。", {"MailAddress"}))
            End If
        End If

        Return result

    End Function

    ' ''' <summary>
    ' ''' 性別を表す文字列へ変換します。
    ' ''' </summary>
    ' ''' <returns>
    ' ''' 成功なら性別名、
    ' ''' 失敗ならString.Empty。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Public Function ToSexString() As String

    '    If QhDictionary.SexType.ContainsKey(Me.Sex) Then
    '        Return QhDictionary.SexType()
    '    Else
    '        Return String.Empty
    '    End If

    'End Function

    ''' <summary>
    ''' 生年月日を日時書式形式へ変換します。
    ''' </summary>
    ''' <param name="format">日時書式指定文字列。</param>
    ''' <returns>
    ''' 成功なら生年月日の日時書式形式、
    ''' 失敗なら"0000/01/01"。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ToBirthdayString(format As String) As String

        Dim result As String = "0000/01/01"
        Dim dateValue As Date = Date.MinValue

        If Date.TryParseExact(String.Format("{0}{1}{2}", Me.BirthYear.PadLeft(4, "0"c), Me.BirthMonth.PadLeft(2, "0"c), Me.BirthDay.PadLeft(2, "0"c)), "yyyyMMdd", Nothing, Globalization.DateTimeStyles.None, dateValue) _
            AndAlso Not String.IsNullOrWhiteSpace(format) Then

            result = dateValue.ToString(format)
        End If

        Return result

    End Function
#End Region

End Class
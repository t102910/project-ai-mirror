Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Attribute
Imports System.Text.RegularExpressions

<Serializable()>
Public NotInheritable Class PortalHospitalConnectionRequestInputModel
    Inherits QyHealthPageViewModelBase
    Implements IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' 診察券番号の入力文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_8 As Integer = 8

    ''' <summary>
    ''' 診察券番号の入力文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_12 As Integer = 12

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

    ' ''' <summary>
    ' ''' パスワードの最小長を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const LENGTH_8 As Integer = 8

    ' ''' <summary>
    ' ''' パスワードの最大長を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const LENGTH_32 As Integer = 32

    ''' <summary>
    ''' IDとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const ID_PATTERN As String = "^[A-Za-z0-9]*$"

    ''' <summary>
    ''' カナとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const KANA_PATTERN As String = "^[\p{IsKatakana}\s]*$"

    ''' <summary>
    ''' メールアドレスとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_PATTERN As String = "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z"
    'Private Const MAILADDRESS_PATTERN As String = "^[A-Za-z0-9]*$"

    ''' <summary>
    ''' 必須項目のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERRORMESSAGE As String = "{0}は必須項目です。"


    ''' <summary>
    ''' 文字列長のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const ID_LENGTH_ERRORMESSAGE As String = "{0}は{1}文字で入力してください。{1}文字未満の場合は先頭を0で埋めて入力してください。"

    ''' <summary>
    ''' 文字列長のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_ERRORMESSAGE As String = "{0}は{1}文字以下で入力してください。"

    ''' <summary>
    ''' 正規表現のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REGULAREXPRESSION_ERRORMESSAGE As String = "{0}は{1}で入力してください。"

    ''' <summary>
    ''' その他のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const OTHERS_ERRORMESSAGE As String = "{0}は{1}で入力してください。"


    ''' <summary>
    ''' メールアドレスの形式エラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_ERRORMESSAGE As String = "有効なメールアドレス形式で入力してください。"




#End Region

#Region "Public Property"

    ''' <summary>
    ''' 遷移元の画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None

    ''' <summary>
    ''' 病院のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HospitalList As New List(Of KeyValuePair(Of Integer, String))

    ''' <summary>
    ''' 病院を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("連携病院")>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 診察券番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("診察券番号")>
    Public Property LinkageSystemId As String = String.Empty

    ''' <summary>
    ''' 氏名　姓　を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("姓")>
    Public Property FamilyName As String = String.Empty

    ''' <summary>
    ''' 氏名　名　を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("名")>
    Public Property GivenName As String = String.Empty

    ''' <summary>
    ''' 氏名　カナ姓　を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("カナ姓")>
    Public Property FamilyKanaName As String = String.Empty

    ''' <summary>
    ''' 氏名　カナ名　を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("カナ名")>
    Public Property GivenKanaName As String = String.Empty

    ''' <summary>
    ''' 性別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("性別")>
    Public Property SexType As QySexTypeEnum = QySexTypeEnum.None

    ''' <summary>
    ''' 生年月日の年を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    Public Property BirthYear As String = String.Empty

    ''' <summary>
    ''' 生年月日の月を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    Public Property BirthMonth As String = String.Empty

    ''' <summary>
    ''' 生年月日の日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("生年月日")>
    Public Property BirthDay As String = String.Empty

    ''' <summary>
    ''' 生年月日の日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("メールアドレス")>
    Public Property MailAddress As String = String.Empty

    ''' <summary>
    ''' 個人情報の更新を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IdentityUpdateFlag As Boolean = False

    ''' <summary>
    ''' 連携種別フラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("開示許可")>
    Public Property RelationContentFlags As QyRelationContentTypeEnum = QyRelationContentTypeEnum.None

    ''' <summary>
    ''' 連携があるかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StatusType As Byte = Byte.MinValue


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHospitalConnectionRequestInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region "Public Method"

    ' ''' <summary>
    ' ''' インプットモデルの内容を現在のインスタンスに反映します。
    ' ''' </summary>
    ' ''' <param name="inputModel">インプットモデル。</param>
    ' ''' <remarks></remarks>
    'Public Sub UpdateByInput(inputModel As PortalHospitalConnectionRequestInputModel) Implements IQyModelUpdater(Of PortalHospitalConnectionRequestInputModel).UpdateByInput

    '    If inputModel IsNot Nothing Then
    '        With inputModel
    '            Me.LinkageSystemNo = LinkageSystemNo
    '            Me.LinkageSystemId = If(String.IsNullOrWhiteSpace(inputModel.LinkageSystemId), String.Empty, .LinkageSystemId.Trim())
    '            Me.FamilyName = If(String.IsNullOrWhiteSpace(inputModel.FamilyName), String.Empty, .FamilyName.Trim())
    '            Me.GivenName = If(String.IsNullOrWhiteSpace(inputModel.GivenName), String.Empty, .GivenName.Trim())
    '            Me.FamilyKanaName = If(String.IsNullOrWhiteSpace(inputModel.FamilyKanaName), String.Empty, .FamilyKanaName.Trim())
    '            Me.GivenKanaName = If(String.IsNullOrWhiteSpace(inputModel.GivenKanaName), String.Empty, .GivenKanaName.Trim())
    '            Me.SexType = .SexType
    '            Me.BirthYear = If(String.IsNullOrWhiteSpace(inputModel.BirthYear), String.Empty, .BirthYear.Trim())
    '            Me.BirthMonth = If(String.IsNullOrWhiteSpace(inputModel.BirthMonth), String.Empty, .BirthMonth.Trim())
    '            Me.BirthDay = If(String.IsNullOrWhiteSpace(inputModel.BirthDay), String.Empty, .BirthDay.Trim())
    '            Me.MailAddress = If(String.IsNullOrWhiteSpace(inputModel.MailAddress), String.Empty, .MailAddress.Trim())
    '        End With
    '    End If

    'End Sub

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

        '半角英数に直す
        Dim rxId As New Regex(ID_PATTERN, RegexOptions.Compiled)
        Dim rxKana As New Regex(KANA_PATTERN, RegexOptions.Compiled)
        'Dim result As Boolean = rx.IsMatch("{検査対象文字列}")

        '病院の連携番号
        If LinkageSystemNo <= 0 Then result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "連携病院"), {"model.LinkageSystemNo"}))

        '診察券番号
        If String.IsNullOrWhiteSpace(Me.LinkageSystemId) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "診察券番号"), {"model.LinkageSystemId"}))
        Else

            If Not rxId.IsMatch(Me.LinkageSystemId) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "診察券番号", "半角英数"), {"model.LinkageSystemId"}))

            '中部地区医師会、北部地区医師会病院の場合12桁
            If Me.LinkageSystemNo = 47106 OrElse Me.LinkageSystemNo = 47000016 Then
                If Me.LinkageSystemId.Length <> LENGTH_12 Then result.Add(New ValidationResult(String.Format(ID_LENGTH_ERRORMESSAGE, "診察券番号", LENGTH_12), {"model.LinkageSystemId"}))
            Else
                If Me.LinkageSystemId.Length <> LENGTH_8 Then result.Add(New ValidationResult(String.Format(ID_LENGTH_ERRORMESSAGE, "診察券番号", LENGTH_8), {"model.LinkageSystemId"}))
            End If

        End If

        '名前 姓
        If String.IsNullOrWhiteSpace(Me.FamilyName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "姓"), {"model.FamilyName"}))
        Else

            If Me.FamilyName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "姓", LENGTH_25), {"model.FamilyName"}))

        End If

        '名前 名
        If String.IsNullOrWhiteSpace(Me.GivenName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "名"), {"model.FamilyName"}))
        Else

            If Me.GivenName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "名", LENGTH_25), {"model.FamilyName"}))

        End If

        '名前　カナ姓
        If String.IsNullOrWhiteSpace(Me.FamilyKanaName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "カナ姓"), {"model.FamilyKanaName"}))
        Else

            If Not rxKana.IsMatch(Me.FamilyKanaName) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "カナ姓", "全角カナ文字"), {"model.FamilyKanaName"}))
            If Me.FamilyKanaName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "カナ姓", LENGTH_25), {"model.FamilyKanaName"}))

        End If

        '名前 カナ名
        If String.IsNullOrWhiteSpace(Me.GivenKanaName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "カナ名"), {"model.FamilyKanaName"}))
        Else

            If Not rxKana.IsMatch(Me.GivenKanaName) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "カナ名", "全角カナ文字"), {"model.FamilyKanaName"}))
            If Me.GivenKanaName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "カナ名", LENGTH_25), {"model.FamilyKanaName"}))

        End If

        '性別
        If Me.SexType = QySexTypeEnum.None Or Me.SexType = QySexTypeEnum.Other Then result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "性別"), {"model.SexType"}))

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

            result.Add(New ValidationResult("生年月日が不正です。", {"model.BirthYear"}))
        End If

        'メールアドレス
        If String.IsNullOrWhiteSpace(Me.MailAddress) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "メールアドレス"), {"model.MailAddress"}))
        Else
            Dim mail As New EmailAddressAttribute()

            If Not mail.IsValid(Me.MailAddress) OrElse Not Regex.IsMatch(Me.MailAddress, MAILADDRESS_PATTERN, RegexOptions.IgnoreCase) Then
                result.Add(New ValidationResult(String.Format(MAILADDRESS_ERRORMESSAGE), {"model.MailAddress"}))
            End If

            If Me.MailAddress.Length > LENGTH_256 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "メールアドレス", LENGTH_256), {"model.MailAddress"}))
        End If

        Return result

    End Function

#End Region

End Class
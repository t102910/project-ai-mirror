Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Attribute
Imports System.Text.RegularExpressions

<Serializable()>
Public NotInheritable Class PortalMedicineConnectionRequestInputModel
    Inherits QyHealthPageViewModelBase
    Implements IValidatableObject

#Region "Constant"

    ''' <summary>
    ''' 診察券番号の入力文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_8 As Integer = 8

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

    ' ''' <summary>
    ' ''' メールアドレスの入力最大文字数を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const LENGTH_256 As Integer = 256

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

    ' ''' <summary>
    ' ''' メールアドレスとして使用可能な文字を表す正規表現パターンを表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
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
    Private Const ID_LENGTH_ERRORMESSAGE As String = "{0}は{1}文字で入力してください。{1}文字以下の場合は先頭を0で埋めて入力してください。"

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


    ' ''' <summary>
    ' ''' メールアドレスの形式エラー文字列を表します
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const MAILADDRESS_ERRORMESSAGE As String = "有効なメールアドレス形式で入力してください。"




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
    ''' 連携システム番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 施設キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FacilityKey As Guid = Guid.Empty

    ''' <summary>
    ''' ID を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PharmacyId As Integer = Integer.MinValue

    ''' <summary>
    ''' 施設名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("連携先薬局")>
    Public Property FacilityName As String = String.Empty

    ''' <summary>
    ''' 薬局診察券番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("薬局診察券番号")>
    Public Property PatientCardNo As String = String.Empty

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

    ' ''' <summary>
    ' ''' 氏名　カナ姓　を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("カナ姓")>
    'Public Property FamilyKanaName As String = String.Empty

    ' ''' <summary>
    ' ''' 氏名　カナ名　を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("カナ名")>
    'Public Property GivenKanaName As String = String.Empty

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

    ' ''' <summary>
    ' ''' 生年月日の日を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<DisplayName("メールアドレス")>
    'Public Property MailAddress As String = String.Empty

    ''' <summary>
    ''' 個人情報の更新を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IdentityUpdateFlag As Boolean = False



    ' ''' <summary>
    ' ''' 連携があるかを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property CompanyConnectedFlag As Boolean = False


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalMedicineConnectionRequestInputModel" /> クラスの新しいインスタンスを初期化します。
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
    'Public Sub UpdateByInput(inputModel As PortalMedicineConnectionRequestInputModel) Implements IQyModelUpdater(Of PortalMedicineConnectionRequestInputModel).UpdateByInput

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
        If Me.LinkageSystemNo <= 0 Then result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "連携先施設"), {"model.LinkageSystemNo"}))
        If Me.FacilityKey = Guid.Empty Then result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "連携先施設"), {"model.LinkageSystemNo"}))

        '診察券番号
        If String.IsNullOrWhiteSpace(Me.PatientCardNo) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "患者番号"), {"model.PatientCardNo"}))
        Else

            'If Not rxId.IsMatch(Me.PatientCardNo) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "診察券番号", "半角英数"), {"model.PatientCardNo"}))
            'If Me.PatientCardNo.Length <> LENGTH_8 Then result.Add(New ValidationResult(String.Format(ID_LENGTH_ERRORMESSAGE, "診察券番号", LENGTH_8), {"model.LinkageSystemId"}))

        End If

        '名前 姓
        If String.IsNullOrWhiteSpace(Me.FamilyName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "姓"), {"model.FamilyName"}))
        Else

            If Me.FamilyName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "姓", LENGTH_25), {"model.FamilyName"}))

        End If

        '名前 名
        If String.IsNullOrWhiteSpace(Me.GivenName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "名"), {"model.GivenName"}))
        Else

            If Me.GivenName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "名", LENGTH_25), {"model.GivenName"}))

        End If

        ''名前　カナ姓
        'If String.IsNullOrWhiteSpace(Me.FamilyKanaName) Then
        '    result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "カナ姓"), {"model.FamilyKanaName"}))
        'Else

        '    If Not rxKana.IsMatch(Me.FamilyKanaName) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "カナ姓", "全角カナ文字"), {"model.FamilyKanaName"}))
        '    If Me.FamilyKanaName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "カナ姓", LENGTH_25), {"model.FamilyKanaName"}))

        'End If

        ''名前 カナ名
        'If String.IsNullOrWhiteSpace(Me.GivenKanaName) Then
        '    result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "カナ名"), {"model.GivenKanaName"}))
        'Else

        '    If Not rxKana.IsMatch(Me.GivenKanaName) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "カナ名", "全角カナ文字"), {"model.GivenKanaName"}))
        '    If Me.GivenKanaName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "カナ名", LENGTH_25), {"model.GivenKanaName"}))

        'End If

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

        ''メールアドレス
        'If String.IsNullOrWhiteSpace(Me.MailAddress) Then
        '    result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "メールアドレス"), {"model.MailAddress"}))
        'Else
        '    Dim mail As New EmailAddressAttribute()

        '    If Not mail.IsValid(Me.MailAddress) Then result.Add(New ValidationResult(String.Format(MAILADDRESS_ERRORMESSAGE), {"model.MailAddress"}))
        '    If Me.MailAddress.Length > LENGTH_256 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "メールアドレス", LENGTH_256), {"model.MailAddress"}))
        'End If

        Return result

    End Function

#End Region

End Class
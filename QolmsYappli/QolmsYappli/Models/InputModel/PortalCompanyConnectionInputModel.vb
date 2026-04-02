Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Attribute
Imports System.Text.RegularExpressions

<Serializable()>
Public NotInheritable Class PortalCompanyConnectionRequestInputModel
    Inherits QyPortalPageViewModelBase
    Implements  _
        IQyModelUpdater(Of PortalCompanyConnectionRequestInputModel), 
        IValidatableObject

#Region "Constant"

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
    ''' 社員番号の最大長を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_40 As Integer = 40

    ' ''' <summary>
    ' ''' ユーザーIDの入力最大文字数を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const LENGTH_100 As Integer = 100

    ' ''' <summary>
    ' ''' メールアドレスの入力最大文字数を表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const LENGTH_256 As Integer = 256

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

    ' ''' <summary>
    ' ''' パスワードとして使用可能な文字を表す正規表現パターンを表します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const PASSWORD_PATTERN As String = "^[A-Za-z0-9!-/:-@≠\[-`{-~]*$"

    ''' <summary>
    ''' IDとして使用可能な文字を表す正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const NUMBER_PATTERN As String = "^[0-9]*$"

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
    ''' 必須項目のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERRORMESSAGE As String = "{0}は必須項目です。"

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
    ''' 企業番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("組織ID")>
    Public Property FacilityId As String = String.Empty

    '<Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    ''' <summary>
    ''' 社員番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("社員番号")>
    Public Property EmployeeNo As String = String.Empty

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
    Public Property CompanyConnectedFlag As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalCompanyConnectionInputModel" /> クラスの新しいインスタンスを初期化します。
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
    Public Sub UpdateByInput(inputModel As PortalCompanyConnectionRequestInputModel) Implements IQyModelUpdater(Of PortalCompanyConnectionRequestInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            'With inputModel
            '    Me.ID = If(String.IsNullOrWhiteSpace(inputModel.ID), String.Empty, .ID.Trim())
            '    Me.Password = If(String.IsNullOrWhiteSpace(inputModel.Password), String.Empty, .Password.Trim())
            '    'Me.DataFalgs = inputModel.DataFalgs
            'End With
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

        Dim rxNumber As New Regex(NUMBER_PATTERN, RegexOptions.Compiled)
        Dim rxId As New Regex(ID_PATTERN, RegexOptions.Compiled)
        Dim rxKana As New Regex(KANA_PATTERN, RegexOptions.Compiled)

        '企業コード
        If String.IsNullOrWhiteSpace(Me.FacilityId) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "企業コード"), {"model.CompanyCode"}))
        Else
            If Me.FacilityId.Length >= 10 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "企業コード", "10"), {"model.CompanyCode"}))
        End If

        '社員番号
        If String.IsNullOrWhiteSpace(Me.EmployeeNo) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "社員番号"), {"model.Companyid"}))
        Else
            If Not rxId.IsMatch(Me.EmployeeNo) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "社員番号", "半角英数"), {"model.Companyid"}))
            If Me.EmployeeNo.Length >= LENGTH_40 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "社員番号", LENGTH_40), {"model.Companyid"}))

        End If

        '名前

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

        '名前　カナ姓
        If String.IsNullOrWhiteSpace(Me.FamilyKanaName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "カナ姓"), {"model.FamilyKanaName"}))
        Else

            If Not rxKana.IsMatch(Me.FamilyKanaName) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "カナ姓", "全角カナ文字"), {"model.FamilyKanaName"}))
            If Me.FamilyKanaName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "カナ姓", LENGTH_25), {"model.FamilyKanaName"}))

        End If

        '名前 カナ名
        If String.IsNullOrWhiteSpace(Me.GivenKanaName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERRORMESSAGE, "カナ名"), {"model.GivenKanaName"}))
        Else

            If Not rxKana.IsMatch(Me.GivenKanaName) Then result.Add(New ValidationResult(String.Format(REGULAREXPRESSION_ERRORMESSAGE, "カナ名", "全角カナ文字"), {"model.GivenKanaName"}))
            If Me.GivenKanaName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "カナ名", LENGTH_25), {"model.GivenKanaName"}))

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


        Return result

    End Function

#End Region

End Class
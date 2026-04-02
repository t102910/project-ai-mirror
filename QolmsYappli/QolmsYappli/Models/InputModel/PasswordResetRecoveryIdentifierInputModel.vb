Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports MGF.QOLMS.QolmsApiCoreV1

<Serializable()>
Public NotInheritable Class PasswordResetRecoveryIdentifierInputModel
    Inherits QyPasswordResetPageViewModelBase
    Implements IValidatableObject

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
    ''' メールアドレスの入力最大文字数を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_256 As Integer = 256

    ''' <summary>
    ''' 入力値が必須であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERROR_MESSAGE As String = "{0}を入力してください。"
        
    ''' <summary>
    ''' 文字列長のエラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LENGTH_ERRORMESSAGE As String = "{0}は{1}文字以下で入力してください。"

    ''' <summary>
    ''' メールアドレスの形式エラー文字列を表します
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_ERRORMESSAGE As String = "有効なメールアドレス形式で入力してください。"

    ''' <summary>
    ''' メールアドレスの正規表現パターンを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAILADDRESS_PATTERN As String = "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z"

#End Region

#Region "Public Property"


    Public Property PasswordResetKey As Guid = Guid.Empty

    ''' <summary>
    ''' JOTO IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("JOTO ID")>
    Public Property JotoId As String = String.Empty

    ''' <summary>
    ''' 姓を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("姓")>
    Public Property FamilyName As String = String.Empty
            
    ''' <summary>
    ''' 名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("名")>
    Public Property GivenName As String = String.Empty

    ''' <summary>
    ''' 性別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("性別")>
    Public Property Sex As String = String.Empty
    ''' <summary>
    ''' 生年を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BirthYear As String = String.Empty

    ''' <summary>
    ''' 生月を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BirthMonth As String = String.Empty
    
    ''' <summary>
    ''' 生日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BirthDay As String = String.Empty

    ''' <summary>
    ''' 登録メールアドレスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("登録メールアドレス")>
    Public Property MailAddress As String = String.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteExerciseInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 入力検証エラー メッセージを作成します。
    ''' </summary>
    ''' <param name="format">複合書式指定文字列。</param>
    ''' <param name="propertyName">プロパティ名。</param>
    ''' <param name="displayName">表示名。</param>
    ''' <param name="arg1">書式指定する第 1 オブジェクト。</param>
    ''' <param name="arg2">書式指定する第 2 オブジェクト（オプショナル、デフォルト = Nothing）。</param>
    ''' <returns>
    ''' 入力検証エラー メッセージ。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CreateErrorMessage(format As String, propertyName As String, displayName As String, Optional arg1 As Object = Nothing, Optional arg2 As Object = Nothing) As String

        Return String.Format(format, If(String.IsNullOrWhiteSpace(displayName), propertyName, displayName), arg1, arg2)

    End Function


#End Region

#Region "Public Method"


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
        '// JOTOID
        If String.IsNullOrWhiteSpace(Me.JotoId) Then
            'とりあえず必須チェックだけ
            result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "JOTO ID"), {"model.JotoId"}))
        Else 
            'あとは半角くらいはチェックする？
            '<StringLength(StartRegisterUserIdInputModel.LENGTH_100, MinimumLength:=StartRegisterUserIdInputModel.LENGTH_8, ErrorMessage:="{0}は{2}文字以上、{1}文字以下で入力してください。")>
            '<RegularExpression(StartRegisterUserIdInputModel.QOLMS_PASSWORD_PATTERN, ErrorMessage:="{0}に使用できない文字が含まれて

        End If

        '// 姓名
        If String.IsNullOrWhiteSpace(Me.FamilyName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "姓"), {"model.FamilyName"}))
           Else
            If Me.FamilyName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "姓", LENGTH_25), {"model.FamilyName"}))
        End If
                
        If String.IsNullOrWhiteSpace(Me.GivenName) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "名"), {"model.FamilyName"}))
        Else
            If Me.GivenName.Length > LENGTH_25 Then result.Add(New ValidationResult(String.Format(LENGTH_ERRORMESSAGE, "名", LENGTH_25), {"model.FamilyName"}))
        End If

        '// 性別（男女のみ）
        If  String.IsNullOrWhiteSpace(Me.Sex) Then
            
            result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "性別"), {"model.Sex"}))
        Else

            Dim sextype As QySexTypeEnum = Me.Sex.TryToValueType(QySexTypeEnum.None)
            If sextype = QySexTypeEnum.None OrElse sextype = QySexTypeEnum.Other Then
                result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "性別"), {"model.Sex"}))
            End If
        End If


        '// 生年月日
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

        '// メールアドレス
        If String.IsNullOrWhiteSpace(Me.MailAddress) Then
            result.Add(New ValidationResult(String.Format(REQUIRED_ERROR_MESSAGE, "メールアドレス"), {"model.MailAddress"}))
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
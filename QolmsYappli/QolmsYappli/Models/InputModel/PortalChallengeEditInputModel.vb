Imports System.ComponentModel.DataAnnotations

''' <summary>
''' 「チャレンジエントリー」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalChallengeEditInputModel
    Inherits QyPortalPageViewModelBase
    Implements IValidatableObject


#Region "Public Property"

    ''' <summary>
    ''' チャレンジの情報を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeItem As ChallengeItem

    ''' <summary>
    ''' チャレンジ情報を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeInputItem As New ChallengeInputItem

    ''' <summary>
    ''' 連携種別フラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RelationContentFlags As QyRelationContentTypeEnum = QyRelationContentTypeEnum.None

    ''' <summary>
    ''' 連携IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property linkageSystemId As String = String.Empty

    ''' <summary>
    ''' 連携IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property linkageStatus As Byte = Byte.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeEditInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalChallengeEditInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalChallenge)

    End Sub


#End Region

#Region "IValidatableObject Support"


    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()

        Return result

    End Function
#End Region

#Region "Public Method"
    ''' <summary>
    ''' 画面から入力された項目でUpdateします
    ''' 
    ''' </summary>
    ''' <param name="values"></param>
    ''' <remarks></remarks>
    Public Sub UpdateByInput(values As Dictionary(Of String, String), checked As Byte)

        'Me.Pass = pass
        Me.RelationContentFlags = DirectCast([Enum].ToObject(GetType(QyRelationContentTypeEnum), checked), QyRelationContentTypeEnum)

        For Each item As ChallengeInputValueItem In Me.ChallengeInputItem.RequiredN

            Select Case item.Key

                Case "Name", "KanaName", "Birthday"

                Case Else
                    Dim updateItem As KeyValuePair(Of String, String) = values.Where(Function(i) i.Key = item.Key).First()

                    item.Value = updateItem.Value

            End Select
        Next

        For Each item As ChallengeInputValueItem In Me.ChallengeInputItem.OptionalN

            Select Case item.Key

                Case "Name", "KanaName", "Birthday"

                Case Else
                    Dim updateItem As KeyValuePair(Of String, String) = values.Where(Function(i) i.Key = item.Key).FirstOrDefault()

                    If Not String.IsNullOrWhiteSpace(updateItem.Key) Then
                        item.Value = updateItem.Value
                    End If

            End Select
        Next

    End Sub

    Public Function IsValid(ByRef errorMessage As Dictionary(Of String, String)) As Boolean

        '' Passが正しく入力されているか
        'If Me.PassPartialViewModel.PassCodeVisible Then

        '    If Me.PassPartialViewModel.PassCodes.IndexOf(Pass) < 0 Then
        '        AddErrorMessage(errorMessage, "Pass", "エントリーコードが間違っています。")

        '    End If
        'End If

        '入力項目

        For Each item As ChallengeInputValueItem In Me.ChallengeInputItem.RequiredN

            Select Case item.Key

                Case "PhoneNumber"
                    '数値＋桁数（10か11）
                    If Not Regex.IsMatch(item.Value, "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Key, "数値のみで入力してください。")
                    If item.Value.Length < 10 OrElse item.Value.Length > 11 Then AddErrorMessage(errorMessage, item.Key, "市外局番を含む10桁または11桁で入力してください。")

                Case "Name"

                    If Not String.IsNullOrWhiteSpace(item.Value) Then

                    Else
                        AddErrorMessage(errorMessage, item.Key, "氏名は必須です。")
                    End If

                Case "KanaName"

                    ' カナ姓が有効か検証
                    If Not String.IsNullOrWhiteSpace(item.Value) Then
                        Dim kana As String = StrConv(item.Value, VbStrConv.Narrow)
                        If Not Regex.IsMatch(item.Value, "^[\p{IsKatakana}\s]*$") Then AddErrorMessage(errorMessage, item.Key, "全角カナのみで入力してください。")

                        ' 半角カナに変換できない文字が含まれる（ヱ、ヰ、ヮ、ヵなど）
                        If kana.Length <> Encoding.GetEncoding("shift_jis").GetByteCount(kana) Then AddErrorMessage(errorMessage, item.Key, "カナに使用出来ない文字が含まれています。")

                    Else
                        AddErrorMessage(errorMessage, item.Key, "カナ名は必須です。")
                    End If

                Case "Mail"

                    If Not String.IsNullOrWhiteSpace(item.Value) Then

                        If New EmailAddressAttribute().IsValid(item.Value) AndAlso
                            Regex.IsMatch(item.Value, "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z", RegexOptions.IgnoreCase) Then
                            Try
                                Dim a As New System.Net.Mail.MailAddress(item.Value)
                            Catch ex As FormatException
                                'FormatExceptionがスローされた時は、正しくない
                                AddErrorMessage(errorMessage, item.Key, "メールアドレスの形式が不正です。")
                            End Try
                        Else
                            'メールアドレスが無効
                            AddErrorMessage(errorMessage, item.Key, "メールアドレスの形式が不正です。")
                        End If

                    Else

                        AddErrorMessage(errorMessage, item.Key, "メールアドレスは必須です。")
                    End If

                Case "Family"

                    If Not String.IsNullOrWhiteSpace(item.Value) Then

                    Else
                        AddErrorMessage(errorMessage, item.Key, "氏名は必須です。")
                    End If

                Case "FamilyRelationship"

                    If Not String.IsNullOrWhiteSpace(item.Value) Then

                    Else
                        AddErrorMessage(errorMessage, item.Key, "続柄は必須です。")
                    End If

                Case "FamilyPhoneNumber"

                    '数値＋桁数（10か11）
                    If Not Regex.IsMatch(item.Value, "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Key, "数値のみで入力してください。")
                    If item.Value.Length < 10 OrElse item.Value.Length > 11 Then AddErrorMessage(errorMessage, item.Key, "市外局番を含む10桁または11桁で入力してください。")
                Case "Birthday"

                    If Not String.IsNullOrWhiteSpace(item.Value) Then

                        Dim arr() As String = item.Value.Split("/"c)

                        If arr.Length = 3 Then

                            ' 生年月日を検証
                            Dim birthday As Date = Date.MinValue
                            Dim BornYear As Integer = Integer.MinValue
                            Dim BornMonth As Integer = Integer.MinValue
                            Dim BornDay As Integer = Integer.MinValue

                            If Not Integer.TryParse(arr(0), BornYear) _
                                OrElse Not Integer.TryParse(arr(1), BornMonth) _
                                OrElse Not Integer.TryParse(arr(2), BornDay) _
                                OrElse BornYear < 0 _
                                OrElse BornMonth < 1 _
                                OrElse BornDay < 1 _
                                OrElse Not Date.TryParseExact(BornYear.ToString("d4") + BornMonth.ToString("d2") + BornDay.ToString("d2"), "yyyyMMdd", Nothing, Globalization.DateTimeStyles.None, birthday) _
                                OrElse birthday = Date.MinValue _
                                OrElse birthday > Date.Now.Date Then

                                AddErrorMessage(errorMessage, item.Key, "生年月日が不正です。")
                            End If
                        Else
                            AddErrorMessage(errorMessage, item.Key, "生年月日が不正です。西暦""/"" 区切りで入力してください。")

                        End If

                    Else
                        AddErrorMessage(errorMessage, item.Key, "生年月日は必須です。")

                    End If

                Case "PostalCode"

                    If Not String.IsNullOrWhiteSpace(item.Value) Then

                        If Not Regex.IsMatch(item.Value, "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Value, "数値のみで入力してください。")
                        If item.Value.Length <> 7 Then AddErrorMessage(errorMessage, item.Key, "郵便番号は7桁で入力してください。")
                    Else
                        AddErrorMessage(errorMessage, item.Key, "郵便番号は必須です。")
                    End If

                Case "Address"
                    If Not String.IsNullOrWhiteSpace(item.Value) Then

                    Else
                        AddErrorMessage(errorMessage, item.Key, "住所は必須です。")
                    End If
            End Select
        Next

        For Each item As ChallengeInputValueItem In Me.ChallengeInputItem.OptionalN

            If Not String.IsNullOrWhiteSpace(item.Value) Then

                Select Case item.Key

                    Case "InsuredNumber"

                        ''数値＋桁数（10か11）
                        If Not Regex.IsMatch(item.Value, "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Key, "数値のみで入力してください。")
                        'If item.Value.Length < 10 OrElse item.Value.Length > 11 Then result.Add(New ValidationResult("市外局番を含む10桁または11桁で入力してください。", {item.Key}))

                End Select

            End If

        Next

        Return errorMessage.Count = 0

    End Function

#End Region

    Private Sub AddErrorMessage(ByRef message As Dictionary(Of String, String), addKey As String, addValue As String)

        If message.ContainsKey(addKey) Then

            Dim bild As New StringBuilder()
            bild.AppendLine(message(addKey))
            bild.AppendLine(addValue)

            message(addKey) = bild.ToString()
        Else
            message.Add(addKey, addValue)
        End If


    End Sub

End Class
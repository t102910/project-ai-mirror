Imports System.ComponentModel.DataAnnotations

''' <summary>
''' 「チャレンジエントリー」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalChallengeEntryInputModel
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
    ''' 同意チェック済みかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AgreeChecked As Boolean = False

    ''' <summary>
    ''' エントリーコードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Pass As String = String.Empty

    ''' <summary>
    ''' 必須項目を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RequiredN As New Dictionary(Of String, String)()

    ''' <summary>
    ''' 任意項目を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OptionalN As New Dictionary(Of String, String)()

    ''' <summary>
    ''' 連携種別フラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RelationContentFlags As QyRelationContentTypeEnum = QyRelationContentTypeEnum.None


    'Premiumかどうか
    'ステータス進行度


    'PartialViewModel

    ''' <summary>
    ''' 規約同意画面のパーシャルビューモデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AgreePartialViewModel As PortalChallengeEntryAgreeResultPartialViewModel

    ''' <summary>
    ''' 規約同意画面のパーシャルビューモデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PassPartialViewModel As PortalChallengeEntryPassResultPartialViewModel


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeEntryInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalChallengeEntryInputModel" /> クラスの新しいインスタンスを初期化します。
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

        ' Passが正しく入力されているか
        If Me.PassPartialViewModel.PassCodeVisible Then

            If Me.PassPartialViewModel.PassCodes.IndexOf(Pass) < 0 Then
                result.Add(New ValidationResult("エントリーコードが間違っています。", {"Pass"}))

            End If
        End If

        '入力項目

        For Each item As KeyValuePair(Of String, String) In Me.PassPartialViewModel.RequiredN

            If Me.RequiredN.ContainsKey(item.Key) Then

                Select Case item.Key

                    Case "PhoneNumber"
                        '数値＋桁数（10か11）
                        If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[0-9]+$") Then result.Add(New ValidationResult("数値のみで入力してください。", {item.Key}))
                        If Me.RequiredN(item.Key).Length < 10 OrElse Me.RequiredN(item.Key).Length > 11 Then result.Add(New ValidationResult("市外局番を含む10桁または11桁で入力してください。", {item.Key}))

                    Case "Name"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                        Else
                            result.Add(New ValidationResult("氏名は必須です。", {item.Key}))
                        End If

                    Case "KanaName"

                        ' カナ姓が有効か検証
                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then
                            Dim kana As String = StrConv(Me.RequiredN(item.Key), VbStrConv.Narrow)

                            If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[\p{IsKatakana}\s]*$") Then result.Add(New ValidationResult("全角カナのみで入力してください。", {item.Key}))

                            If kana.Length <> Encoding.GetEncoding("shift_jis").GetByteCount(kana) Then
                                ' 半角カナに変換できない文字が含まれる（ヱ、ヰ、ヮ、ヵなど）
                                result.Add(New ValidationResult("カナに使用出来ない文字が含まれています。", {item.Key}))
                            End If
                        Else
                            result.Add(New ValidationResult("カナ名は必須です。", {item.Key}))
                        End If

                    Case "Mail"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                            If New EmailAddressAttribute().IsValid(Me.RequiredN(item.Key)) AndAlso
                                Regex.IsMatch(Me.RequiredN(item.Key), "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z", RegexOptions.IgnoreCase) Then
                                Try
                                    Dim a As New System.Net.Mail.MailAddress(Me.RequiredN(item.Key))
                                Catch ex As FormatException
                                    'FormatExceptionがスローされた時は、正しくない
                                    result.Add(New ValidationResult("メールアドレスの形式が不正です。", {item.Key}))
                                End Try
                            Else
                                'メールアドレスが無効
                                result.Add(New ValidationResult("メールアドレスの形式が不正です。", {item.Key}))
                            End If

                        Else
                            result.Add(New ValidationResult("メールアドレスは必須です。", {item.Key}))
                        End If

                    Case "Family"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                        Else
                            result.Add(New ValidationResult("氏名は必須です。", {item.Key}))
                        End If

                    Case "FamilyRelationship"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                        Else
                            result.Add(New ValidationResult("続柄は必須です。", {item.Key}))
                        End If

                    Case "FamilyPhoneNumber"

                        '数値＋桁数（10か11）
                        If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[0-9]+$") Then result.Add(New ValidationResult("数値のみで入力してください。", {item.Key}))
                        If Me.RequiredN(item.Key).Length < 10 OrElse Me.RequiredN(item.Key).Length > 11 Then result.Add(New ValidationResult("市外局番を含む10桁または11桁で入力してください。", {item.Key}))

                    Case "Birthday"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                            Dim arr() As String = Me.RequiredN(item.Key).Split("/"c)

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

                                    result.Add(New ValidationResult("生年月日が不正です。", {item.Key}))
                                End If
                            Else
                                result.Add(New ValidationResult("生年月日が不正です。", {item.Key}))

                            End If

                        Else
                            result.Add(New ValidationResult("生年月日は必須です。", {item.Key}))

                        End If

                End Select

            ElseIf Me.OptionalN.ContainsKey(item.Key) Then

                Select item.Key

                    Case "InsuredNumber"

                        ''数値＋桁数（10か11）
                        If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[0-9]+$") Then result.Add(New ValidationResult("数値のみで入力してください。", {item.Key}))
                        'If Me.RequiredN(item.Key).Length < 10 OrElse Me.RequiredN(item.Key).Length > 11 Then result.Add(New ValidationResult("市外局番を含む10桁または11桁で入力してください。", {item.Key}))

                End Select

            Else
                result.Add(New ValidationResult("{0}は必須です。", {item.Key}))

            End If
        Next

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
    Public Sub UpdateByInput(pass As String, values As Dictionary(Of String, String), checked As Byte)

        Me.Pass = Pass
        Me.RelationContentFlags = DirectCast([Enum].ToObject(GetType(QyRelationContentTypeEnum), checked), QyRelationContentTypeEnum)

        For Each item As KeyValuePair(Of String, String) In values

            If Me.PassPartialViewModel.RequiredN.ContainsKey(item.Key) Then

                If Me.RequiredN.ContainsKey(item.Key) Then
                    Me.RequiredN(item.Key) = values(item.Key)
                Else
                    Me.RequiredN.Add(item.Key, values(item.Key))
                End If

            ElseIf Me.PassPartialViewModel.OptionalN.ContainsKey(item.Key) Then

                If Me.OptionalN.ContainsKey(item.Key) Then
                    Me.OptionalN(item.Key) = values(item.Key)
                Else
                    Me.OptionalN.Add(item.Key, values(item.Key))
                End If

            End If

        Next

    End Sub

    Public Function IsValid(ByRef errorMessage As Dictionary(Of String, String)) As Boolean

        ' Passが正しく入力されているか
        If Me.PassPartialViewModel.PassCodeVisible Then

            If Me.PassPartialViewModel.PassCodes.IndexOf(Pass) < 0 Then
                AddErrorMessage(errorMessage, "Pass", "エントリーコードが間違っています。")

            End If
        End If

        '入力項目

        For Each item As KeyValuePair(Of String, String) In Me.PassPartialViewModel.RequiredN

            If Me.RequiredN.ContainsKey(item.Key) Then

                Select Case item.Key

                    Case "PhoneNumber"
                        '数値＋桁数（10か11）
                        If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Key, "数値のみで入力してください。")
                        If Me.RequiredN(item.Key).Length < 10 OrElse Me.RequiredN(item.Key).Length > 11 Then AddErrorMessage(errorMessage, item.Key, "市外局番を含む10桁または11桁で入力してください。")

                    Case "Name"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                        Else
                            AddErrorMessage(errorMessage, item.Key, "氏名は必須です。")
                        End If

                    Case "KanaName"

                        ' カナ姓が有効か検証
                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then
                            Dim kana As String = StrConv(Me.RequiredN(item.Key), VbStrConv.Narrow)
                            If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[\p{IsKatakana}\s]*$") Then AddErrorMessage(errorMessage, item.Key, "全角カナのみで入力してください。")

                            ' 半角カナに変換できない文字が含まれる（ヱ、ヰ、ヮ、ヵなど）
                            If kana.Length <> Encoding.GetEncoding("shift_jis").GetByteCount(kana) Then AddErrorMessage(errorMessage, item.Key, "カナに使用出来ない文字が含まれています。")

                        Else
                            AddErrorMessage(errorMessage, item.Key, "カナ名は必須です。")
                        End If

                    Case "Mail"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                            If New EmailAddressAttribute().IsValid(Me.RequiredN(item.Key)) Then
                                Try
                                    If Not Regex.IsMatch(Me.RequiredN(item.Key), "\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z", RegexOptions.IgnoreCase) Then
                                        AddErrorMessage(errorMessage, item.Key, "メールアドレスの形式が不正です。")
                                    Else
                                        Dim a As New System.Net.Mail.MailAddress(Me.RequiredN(item.Key))
                                    End If
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

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                        Else
                            AddErrorMessage(errorMessage, item.Key, "氏名は必須です。")
                        End If

                    Case "FamilyRelationship"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                        Else
                            AddErrorMessage(errorMessage, item.Key, "続柄は必須です。")
                        End If

                    Case "FamilyPhoneNumber"

                        '数値＋桁数（10か11）
                        If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Key, "数値のみで入力してください。")
                        If Me.RequiredN(item.Key).Length < 10 OrElse Me.RequiredN(item.Key).Length > 11 Then AddErrorMessage(errorMessage, item.Key, "市外局番を含む10桁または11桁で入力してください。")
                    Case "Birthday"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                            Dim arr() As String = Me.RequiredN(item.Key).Split("/"c)

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

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                            If Me.RequiredN(item.Key).Length <> 7 Then AddErrorMessage(errorMessage, item.Key, "郵便番号は7桁で入力してください。")
                            If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Key, "数値のみで入力してください。")
                        Else
                            AddErrorMessage(errorMessage, item.Key, "郵便番号は必須です。")
                        End If

                    Case "Address"

                        If Not String.IsNullOrWhiteSpace(Me.RequiredN(item.Key)) Then

                        Else
                            AddErrorMessage(errorMessage, item.Key, "住所は必須です。")
                        End If

                End Select

            ElseIf Me.OptionalN.ContainsKey(item.Key) Then

                Select Case item.Key

                    Case "InsuredNumber"

                        ''数値＋桁数（10か11）
                        If Not Regex.IsMatch(Me.RequiredN(item.Key), "^[0-9]+$") Then AddErrorMessage(errorMessage, item.Key, "数値のみで入力してください。")
                        'If Me.RequiredN(item.Key).Length < 10 OrElse Me.RequiredN(item.Key).Length > 11 Then result.Add(New ValidationResult("市外局番を含む10桁または11桁で入力してください。", {item.Key}))

                End Select

            Else
                AddErrorMessage(errorMessage, item.Key, "{0}は必須です。")

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
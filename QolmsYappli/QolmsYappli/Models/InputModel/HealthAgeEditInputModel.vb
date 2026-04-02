Imports System.ComponentModel.DataAnnotations
Imports System.Reflection

<Serializable()>
Public NotInheritable Class HealthAgeEditInputModel
    Inherits QyHealthPageViewModelBase
    Implements IQyModelUpdater(Of HealthAgeEditInputModel), 
    IValidatableObject

#Region "Constant"

    ' ''' <summary>
    ' ''' 入力値が不正であることを表す標準のエラー メッセージです。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Const DEFAULT_ERROR_MESSAGE As String = "{0}が不正です。"

    ''' <summary>
    ''' 入力値が必須であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REQUIRED_ERROR_MESSAGE As String = "{0}を入力してください。"

    ''' <summary>
    ''' 入力値の範囲が不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const RANGE_ERROR_MESSAGE As String = "{0}は{1}～{2}の範囲で入力してください。"

    ''' <summary>
    ''' 入力値が重複していることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const OVERLAP_ERROR_MESSAGE As String = "{0}が重複しています。"

    ''' <summary>
    ''' 入力値が逆転していることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REVERSE_ERROR_MESSAGE As String = "{0}が逆転しています。"

    ''' <summary>
    ''' 入力値に小数部が含まれていることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DECIMAL_PART_ERROR_MESSAGE As String = "{0}は整数で入力して下さい。"

    ''' <summary>
    ''' 入力値の小数部桁数が不正であることを表すエラー メッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const DECIMAL_PART_DIGIT_ERROR_MESSAGE As String = "{0}は小数点以下{1}桁以内で入力してください。"

    ''' <summary>
    ''' 入力可能最小値を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MIN_VALUE As Decimal = 1D

    ''' <summary>
    ''' 入力可能最大値（歩数以外）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const MAX_VALUE As Decimal = 999999.9999D

    ''' <summary>
    ''' 項目名をキー、
    ''' 入力値の範囲のタプルを値とするディクショナリを表します。
    ''' タプルは、入力許容下限（以上）、入力許容上限（以下）、入力許容小数部桁数の 3 組で構成されます。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly valueRanges As New Dictionary(Of QyHealthAgeValueTypeEnum, Tuple(Of Decimal, Decimal, Integer))() From {
        {QyHealthAgeValueTypeEnum.BMI, New Tuple(Of Decimal, Decimal, Integer)(10D, 100D, 1)},
        {QyHealthAgeValueTypeEnum.Ch014, New Tuple(Of Decimal, Decimal, Integer)(60D, 300D, 0)},
        {QyHealthAgeValueTypeEnum.Ch016, New Tuple(Of Decimal, Decimal, Integer)(30D, 150D, 0)},
        {QyHealthAgeValueTypeEnum.Ch019, New Tuple(Of Decimal, Decimal, Integer)(10D, 2000D, 0)},
        {QyHealthAgeValueTypeEnum.Ch021, New Tuple(Of Decimal, Decimal, Integer)(10D, 500D, 1)},
        {QyHealthAgeValueTypeEnum.Ch023, New Tuple(Of Decimal, Decimal, Integer)(20D, 1000D, 1)},
        {QyHealthAgeValueTypeEnum.Ch025, New Tuple(Of Decimal, Decimal, Integer)(1D, 1000D, 0)},
        {QyHealthAgeValueTypeEnum.Ch027, New Tuple(Of Decimal, Decimal, Integer)(1D, 1000D, 0)},
        {QyHealthAgeValueTypeEnum.Ch029, New Tuple(Of Decimal, Decimal, Integer)(1D, 1000D, 0)},
        {QyHealthAgeValueTypeEnum.Ch035, New Tuple(Of Decimal, Decimal, Integer)(3D, 20D, 1)},
        {QyHealthAgeValueTypeEnum.Ch035FBG, New Tuple(Of Decimal, Decimal, Integer)(20D, 600D, 0)},
        {QyHealthAgeValueTypeEnum.Ch037, New Tuple(Of Decimal, Decimal, Integer)(1D, 5D, 0)},
        {QyHealthAgeValueTypeEnum.Ch039, New Tuple(Of Decimal, Decimal, Integer)(1D, 5D, 0)}
    } ' TODO:

#End Region

#Region "Variable"

    ' TODO:
    Private _latestDateN As New Dictionary(Of QyHealthAgeValueTypeEnum, Date)()

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 遷移元の画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None

    ' TODO:
    Public Property IsMaintenance As Boolean = False

    ' TODO:
    Public Property MaintenanceMessage As String = String.Empty

    ''' <summary>
    ''' 健診受診日（登録日）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' BMI を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BMI As String = String.Empty

    ''' <summary>
    ''' 収縮期血圧を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch014 As String = String.Empty

    ''' <summary>
    ''' 拡張期血圧を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch016 As String = String.Empty

    ''' <summary>
    ''' 中性脂肪を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch019 As String = String.Empty

    ''' <summary>
    ''' HDL コレステロールを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch021 As String = String.Empty

    ''' <summary>
    ''' LDL コレステロールを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch023 As String = String.Empty

    ''' <summary>
    ''' GOT（AST）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch025 As String = String.Empty

    ''' <summary>
    ''' GPT（ALT）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch027 As String = String.Empty

    ''' <summary>
    ''' γ-GT（γ-GTP）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch029 As String = String.Empty

    ''' <summary>
    ''' HbA1c（NGSP）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch035 As String = String.Empty

    ''' <summary>
    ''' 空腹時血糖を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch035FBG As String = String.Empty

    ''' <summary>
    ''' 尿糖を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch037 As String = String.Empty

    ''' <summary>
    ''' 尿蛋白（定性）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ch039 As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteVitalEditInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    Public Sub New(mainModel As QolmsYappliModel, valueN As Dictionary(Of QyHealthAgeValueTypeEnum, Tuple(Of Date, Decimal)))

        MyBase.New(mainModel, QyPageNoTypeEnum.HealthAgeEdit)

        Me.RecordDate = Date.Now

        If valueN IsNot Nothing AndAlso valueN.Any() Then
            valueN.ToList().ForEach(
                Sub(i)
                    Dim key As QyHealthAgeValueTypeEnum = i.Key

                    If key <> QyHealthAgeValueTypeEnum.None Then
                        Dim pi As PropertyInfo = Me.GetType.GetProperty(key.ToString())

                        If pi IsNot Nothing AndAlso Not Me._latestDateN.ContainsKey(key) Then
                            Me._latestDateN.Add(key, i.Value.Item1)
                            pi.SetValue(Me, If(i.Value.Item2 > Decimal.Zero, i.Value.Item2.ToString("0.####"), String.Empty))
                        End If
                    End If
                End Sub
            )
        End If

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

    ''' <summary>
    ''' 10 進数の小数部桁数を取得します。
    ''' </summary>
    ''' <param name="value">取得対象の 10 進数。</param>
    ''' <returns>
    ''' 小数部の桁数。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetDecimalPartScale(value As Decimal) As Integer

        Return Decimal.GetBits(value)(3) >> 16 And &HFF

    End Function

    ''' <summary>
    ''' 入力された 10 進数を検証します。
    ''' </summary>
    ''' <param name="value">入力値。</param>
    ''' <param name="lowerLimit">入力値許容下限（オプショナル、デフォルト = 1D）。</param>
    ''' <param name="upperLimit">入力値許容上限（オプショナル、デフォルト = 999999.9999D）。</param>
    ''' <returns>
    ''' 検証に成功なら Decimal.MinValue 以外、
    ''' 失敗なら Decimal.MinValue。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalValue(value As String, Optional lowerLimit As Decimal = HealthAgeEditInputModel.MIN_VALUE, Optional upperLimit As Decimal = HealthAgeEditInputModel.MAX_VALUE) As Decimal

        Dim result As Decimal = Decimal.MinValue

        If Not String.IsNullOrWhiteSpace(value) Then
            Dim decimalValue As Decimal = Decimal.MinValue

            If Decimal.TryParse(value, decimalValue) _
                AndAlso decimalValue >= lowerLimit _
                AndAlso decimalValue <= upperLimit Then

                result = decimalValue
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 入力された 10 進数の小数部桁数を検証します。
    ''' </summary>
    ''' <param name="value">入力値。</param>
    ''' <param name="scaleLimit">許容する小数部桁数。</param>
    ''' <returns>
    ''' 検証に成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckDecimalPartScale(value As Decimal, scaleLimit As Integer) As Boolean

        Return Me.GetDecimalPartScale(value) <= scaleLimit

    End Function

    ' TODO: 血圧値のチェック
    Private Function CheckPressureValue(value1 As String, value2 As String) As List(Of Tuple(Of QyHealthAgeValueTypeEnum, String))

        Dim result As New List(Of Tuple(Of QyHealthAgeValueTypeEnum, String))()
        Dim decimalValue1 As Decimal = Decimal.MinValue
        Dim decimalValue2 As Decimal = Decimal.MinValue

        ' 血圧（上）のチェック
        If Not String.IsNullOrWhiteSpace(value1) Then
            decimalValue1 = Me.CheckDecimalValue(
                value1,
                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch014).Item1,
                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch014).Item2
            )

            If decimalValue1 = Decimal.MinValue Then
                ' 範囲検証エラー
                result.Add(
                    New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                        QyHealthAgeValueTypeEnum.Ch014,
                        Me.CreateErrorMessage(
                            HealthAgeEditInputModel.RANGE_ERROR_MESSAGE,
                            String.Empty,
                            "血圧（上）",
                            HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch014).Item1,
                            HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch014).Item2
                        )
                    )
                )
            ElseIf Not Me.CheckDecimalPartScale(
                decimalValue1,
                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch014).Item3
            ) Then

                ' 小数部桁数エラー
                If HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch014).Item3 = 0 Then
                    result.Add(
                        New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                            QyHealthAgeValueTypeEnum.Ch014,
                            Me.CreateErrorMessage(
                                HealthAgeEditInputModel.DECIMAL_PART_ERROR_MESSAGE,
                                String.Empty,
                                "血圧（上）"
                            )
                        )
                    )
                Else
                    result.Add(
                        New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                            QyHealthAgeValueTypeEnum.Ch014,
                            Me.CreateErrorMessage(
                                HealthAgeEditInputModel.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                String.Empty,
                                "血圧（上）",
                                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch014).Item3
                            )
                        )
                    )
                End If
            End If
        Else
            ' 必須検証エラー
            result.Add(
                New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                    QyHealthAgeValueTypeEnum.Ch014,
                    Me.CreateErrorMessage(
                        HealthAgeEditInputModel.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        "血圧（上）"
                    )
                )
            )
        End If

        ' 血圧（下）のチェック
        If Not String.IsNullOrWhiteSpace(value2) Then
            decimalValue2 = Me.CheckDecimalValue(
                value2,
                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch016).Item1,
                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch016).Item2
            )

            If decimalValue2 = Decimal.MinValue Then
                ' 範囲検証エラー
                result.Add(
                    New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                        QyHealthAgeValueTypeEnum.Ch016,
                        Me.CreateErrorMessage(
                            HealthAgeEditInputModel.RANGE_ERROR_MESSAGE,
                            String.Empty,
                            "血圧（下）",
                            HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch016).Item1,
                            HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch016).Item2
                        )
                    )
                )
            ElseIf Not Me.CheckDecimalPartScale(
                decimalValue2,
                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch016).Item3
            ) Then

                ' 小数部桁数エラー
                If HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch016).Item3 = 0 Then
                    result.Add(
                        New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                            QyHealthAgeValueTypeEnum.Ch016,
                            Me.CreateErrorMessage(
                                HealthAgeEditInputModel.DECIMAL_PART_ERROR_MESSAGE,
                                String.Empty,
                                "血圧（下）"
                            )
                        )
                    )
                Else
                    result.Add(
                        New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                            QyHealthAgeValueTypeEnum.Ch016,
                            Me.CreateErrorMessage(
                                HealthAgeEditInputModel.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                String.Empty,
                                "血圧（下）",
                                HealthAgeEditInputModel.valueRanges(QyHealthAgeValueTypeEnum.Ch016).Item3
                            )
                        )
                    )
                End If
            End If
        Else
            ' 必須検証エラー
            result.Add(
                New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                    QyHealthAgeValueTypeEnum.Ch016,
                    Me.CreateErrorMessage(
                        HealthAgeEditInputModel.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        "血圧（下）"
                    )
                )
            )
        End If

        If Not String.IsNullOrWhiteSpace(value1) _
            AndAlso Not String.IsNullOrWhiteSpace(value2) _
            AndAlso decimalValue1 <> Decimal.MinValue _
            AndAlso decimalValue2 <> Decimal.MinValue Then

            If decimalValue1 = decimalValue2 Then
                ' 重複検証エラー
                result.Add(
                    New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                        QyHealthAgeValueTypeEnum.Ch014,
                        Me.CreateErrorMessage(
                            HealthAgeEditInputModel.OVERLAP_ERROR_MESSAGE,
                            String.Empty,
                            "血圧の上下"
                        )
                    )
                )
            ElseIf decimalValue1 < decimalValue2 Then
                ' 逆転検証エラー
                result.Add(
                    New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                        QyHealthAgeValueTypeEnum.Ch014,
                        Me.CreateErrorMessage(
                            HealthAgeEditInputModel.REVERSE_ERROR_MESSAGE,
                            String.Empty,
                            "血圧の上下"
                        )
                    )
                )
            End If
        End If

        Return result

    End Function

    ' TODO: 尿糖値・尿蛋白値のチェック
    Private Function CheckUrineValue(valueType As QyHealthAgeValueTypeEnum, value As String, name As String) As List(Of Tuple(Of QyHealthAgeValueTypeEnum, String))

        Select Case valueType
            Case QyHealthAgeValueTypeEnum.Ch037,
                QyHealthAgeValueTypeEnum.Ch039

                ' OK
            Case Else
                ' エラー
                Throw New ArgumentOutOfRangeException()

        End Select

        If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentNullException()

        Dim result As New List(Of Tuple(Of QyHealthAgeValueTypeEnum, String))()
        Dim decimalValue As Decimal = Decimal.MinValue
        Dim isError As Boolean = False

        ' 値のチェック
        If Not String.IsNullOrWhiteSpace(value) Then
            decimalValue = Me.CheckDecimalValue(
                value,
                HealthAgeEditInputModel.valueRanges(valueType).Item1,
                HealthAgeEditInputModel.valueRanges(valueType).Item2
            )

            If decimalValue = Decimal.MinValue Then
                ' 範囲検証エラー
                isError = True

            ElseIf Not Me.CheckDecimalPartScale(
                decimalValue,
                HealthAgeEditInputModel.valueRanges(valueType).Item3
            ) Then

                ' 小数部桁数エラー
                isError = True
            End If
        Else
            ' 必須検証エラー
            isError = True
        End If

        If isError Then
            result.Add(
                New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                    valueType,
                    Me.CreateErrorMessage(
                        "{0}を選択してください。",
                        String.Empty,
                        name
                    )
                )
            )
        End If

        Return result

    End Function

    ' TODO: 他の値のチェック
    Private Function CheckOtherValue(valueType As QyHealthAgeValueTypeEnum, value As String, name As String) As List(Of Tuple(Of QyHealthAgeValueTypeEnum, String))

        Select Case valueType
            Case QyHealthAgeValueTypeEnum.BMI,
                QyHealthAgeValueTypeEnum.Ch019,
                QyHealthAgeValueTypeEnum.Ch021,
                QyHealthAgeValueTypeEnum.Ch023,
                QyHealthAgeValueTypeEnum.Ch025,
                QyHealthAgeValueTypeEnum.Ch027,
                QyHealthAgeValueTypeEnum.Ch029,
                QyHealthAgeValueTypeEnum.Ch035,
                QyHealthAgeValueTypeEnum.Ch035FBG

                ' OK
            Case Else
                ' エラー
                Throw New ArgumentOutOfRangeException()

        End Select

        If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentNullException()

        Dim result As New List(Of Tuple(Of QyHealthAgeValueTypeEnum, String))()
        Dim decimalValue As Decimal = Decimal.MinValue

        ' 値のチェック
        If Not String.IsNullOrWhiteSpace(value) Then
            decimalValue = Me.CheckDecimalValue(
                value,
                HealthAgeEditInputModel.valueRanges(valueType).Item1,
                HealthAgeEditInputModel.valueRanges(valueType).Item2
            )

            If decimalValue = Decimal.MinValue Then
                ' 範囲検証エラー
                result.Add(
                    New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                        valueType,
                        Me.CreateErrorMessage(
                            HealthAgeEditInputModel.RANGE_ERROR_MESSAGE,
                            String.Empty,
                            name,
                            HealthAgeEditInputModel.valueRanges(valueType).Item1,
                            HealthAgeEditInputModel.valueRanges(valueType).Item2
                        )
                    )
                )
            ElseIf Not Me.CheckDecimalPartScale(
                decimalValue,
                HealthAgeEditInputModel.valueRanges(valueType).Item3
            ) Then

                ' 小数部桁数エラー
                If HealthAgeEditInputModel.valueRanges(valueType).Item3 = 0 Then
                    result.Add(
                        New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                            valueType,
                            Me.CreateErrorMessage(
                                HealthAgeEditInputModel.DECIMAL_PART_ERROR_MESSAGE,
                                String.Empty,
                                name
                            )
                        )
                    )
                Else
                    result.Add(
                        New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                            valueType,
                            Me.CreateErrorMessage(
                                HealthAgeEditInputModel.DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                String.Empty,
                                name,
                                HealthAgeEditInputModel.valueRanges(valueType).Item3
                            )
                        )
                    )
                End If
            End If
        Else
            ' 必須検証エラー
            result.Add(
                New Tuple(Of QyHealthAgeValueTypeEnum, String)(
                    valueType,
                    Me.CreateErrorMessage(
                        HealthAgeEditInputModel.REQUIRED_ERROR_MESSAGE,
                        String.Empty,
                        name
                    )
                )
            )
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    Public Sub UpdateByInput(inputModel As HealthAgeEditInputModel) Implements IQyModelUpdater(Of HealthAgeEditInputModel).UpdateByInput

        If inputModel IsNot Nothing Then
            With inputModel
                Me.RecordDate = inputModel.RecordDate
                Me.BMI = If(String.IsNullOrWhiteSpace(inputModel.BMI), String.Empty, inputModel.BMI.Trim())
                Me.Ch014 = If(String.IsNullOrWhiteSpace(inputModel.Ch014), String.Empty, inputModel.Ch014.Trim())
                Me.Ch016 = If(String.IsNullOrWhiteSpace(inputModel.Ch016), String.Empty, inputModel.Ch016.Trim())
                Me.Ch019 = If(String.IsNullOrWhiteSpace(inputModel.Ch019), String.Empty, inputModel.Ch019.Trim())
                Me.Ch021 = If(String.IsNullOrWhiteSpace(inputModel.Ch021), String.Empty, inputModel.Ch021.Trim())
                Me.Ch023 = If(String.IsNullOrWhiteSpace(inputModel.Ch023), String.Empty, inputModel.Ch023.Trim())
                Me.Ch025 = If(String.IsNullOrWhiteSpace(inputModel.Ch025), String.Empty, inputModel.Ch025.Trim())
                Me.Ch027 = If(String.IsNullOrWhiteSpace(inputModel.Ch027), String.Empty, inputModel.Ch027.Trim())
                Me.Ch029 = If(String.IsNullOrWhiteSpace(inputModel.Ch029), String.Empty, inputModel.Ch029.Trim())
                Me.Ch035 = If(String.IsNullOrWhiteSpace(inputModel.Ch035), String.Empty, inputModel.Ch035.Trim())
                Me.Ch035FBG = If(String.IsNullOrWhiteSpace(inputModel.Ch035FBG), String.Empty, inputModel.Ch035FBG.Trim())
                Me.Ch037 = If(String.IsNullOrWhiteSpace(inputModel.Ch037), String.Empty, inputModel.Ch037.Trim())
                Me.Ch039 = If(String.IsNullOrWhiteSpace(inputModel.Ch039), String.Empty, inputModel.Ch039.Trim())
            End With
        End If

    End Sub

    Public Function GetLatestDateString(valueType As QyHealthAgeValueTypeEnum, Optional format As String = "yyyy年M月d日更新") As String

        If Me._latestDateN.ContainsKey(valueType) AndAlso Me._latestDateN(valueType) <> Date.MinValue AndAlso Not String.IsNullOrWhiteSpace(format) Then
            Return _latestDateN(valueType).ToString(format)
        Else
            Return String.Empty
        End If

    End Function

    <Obsolete("健康年齢（ベイジアン ネットワーク）算出用")>
    Public Sub SetValues(recordDate As Date, valueN As Dictionary(Of QyHealthAgeValueTypeEnum, Tuple(Of Date, Decimal)))

        Me.RecordDate = recordDate

        If valueN IsNot Nothing AndAlso valueN.Any() Then
            valueN.ToList().ForEach(
                Sub(i)
                    Dim key As QyHealthAgeValueTypeEnum = i.Key

                    If key <> QyHealthAgeValueTypeEnum.None Then
                        Dim pi As PropertyInfo = Me.GetType.GetProperty(key.ToString())

                        If pi IsNot Nothing AndAlso Not Me._latestDateN.ContainsKey(key) Then
                            Me._latestDateN.Add(key, i.Value.Item1)
                            pi.SetValue(Me, If(i.Value.Item2 > Decimal.Zero, i.Value.Item2.ToString("0.####"), String.Empty))
                        End If
                    End If
                End Sub
            )
        End If

    End Sub

#End Region

#Region "IValidatableObject Support"

    ''' <summary>
    ''' 指定された オブジェクト が有効かどうかを判断します。
    ''' </summary>
    ''' <param name="validationContext">検証 コンテキスト。</param>
    ''' <returns>
    ''' 失敗した検証の情報を保持する コレクション。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Validate(validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate

        Dim result As New List(Of ValidationResult)()
        Dim dicN As New Dictionary(Of String, List(Of String))()
        Dim messageN As New List(Of Tuple(Of QyHealthAgeValueTypeEnum, String))()

        If Me.RecordDate = Date.MinValue Then
            dicN.Add("RecordDate", {"健診受診日が不正です。"}.ToList())
        ElseIf Me.RecordDate.Date > Date.Now.Date Then
            dicN.Add("RecordDate", {"健診受診日が未来です。"}.ToList())
        End If

        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.BMI, Me.BMI, "BMI"))
        messageN.AddRange(Me.CheckPressureValue(Me.Ch014, Me.Ch016))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch019, Me.Ch019, "中性脂肪"))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch021, Me.Ch021, "HDLコレステロール"))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch023, Me.Ch023, "LDLコレステロール"))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch025, Me.Ch025, "AST（GOT）"))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch027, Me.Ch027, "ALT（GPT）"))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch029, Me.Ch029, "γ-GT（γ-GTP）"))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch035, Me.Ch035, "HbA1c（NGSP）"))
        messageN.AddRange(Me.CheckOtherValue(QyHealthAgeValueTypeEnum.Ch035FBG, Me.Ch035FBG, "空腹時血糖"))
        messageN.AddRange(Me.CheckUrineValue(QyHealthAgeValueTypeEnum.Ch037, Me.Ch037, "尿糖"))
        messageN.AddRange(Me.CheckUrineValue(QyHealthAgeValueTypeEnum.Ch039, Me.Ch039, "尿蛋白（定性）"))

        messageN.ForEach(
            Sub(i)
                Dim key As String = i.Item1.ToString()

                If Not dicN.ContainsKey(key) Then dicN.Add(key, New List(Of String)())

                dicN(key).Add(i.Item2)
            End Sub
        )

        If dicN.Any() Then
            dicN.ToList().ForEach(
                Sub(i)
                    result.Add(New ValidationResult(String.Join(Environment.NewLine, i.Value), {i.Key}))
                End Sub
            )
        End If

        Return result

    End Function

#End Region

End Class

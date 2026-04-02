''' <summary>
''' QOLMSポイント付与情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class QolmsPointGrantItem
    '付与ポイントの配列です。サイト側から固定で付与ポイントが決まっている場合はここに追加してください。
    '（通常会員のポイントを記載。減算は0。プレミアム２倍は_pointDoubleItemにQyPointItemTypeEnumのアイテムを追加してください。）
    Public Shared ReadOnly PointValues As Integer() = {0, 500, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 0, 0, 100, 0, 0, 0, 0, 1000, 1}

    Private ReadOnly Property _pointDoubleItem As List(Of QyPointItemTypeEnum)
        Get
            '付与ポイント２倍のポイント種別を追加した場合はここも追加してください。
            Return New List(Of QyPointItemTypeEnum)() From {
                QyPointItemTypeEnum.Login,
                QyPointItemTypeEnum.Walk5k,
                QyPointItemTypeEnum.Walk6k,
                QyPointItemTypeEnum.Walk7k,
                QyPointItemTypeEnum.Walk8k,
                QyPointItemTypeEnum.Walk9k,
                QyPointItemTypeEnum.Walk10k,
                QyPointItemTypeEnum.Exercise,
                QyPointItemTypeEnum.Breakfast,
                QyPointItemTypeEnum.Lunch,
                QyPointItemTypeEnum.Dinner,
                QyPointItemTypeEnum.Snack,
                QyPointItemTypeEnum.Vital,
                QyPointItemTypeEnum.Examination,
                QyPointItemTypeEnum.TanitaConnection,
                QyPointItemTypeEnum.Meal
            }
        End Get

    End Property


#Region "Public Property"

    ''' <summary>
    ''' 付与日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ActionDate As Date = Date.MinValue

    ''' <summary>
    ''' シリアル番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>呼び出しシステム内で一意になる40文字以内の文字列</remarks>
    Public Property SerialCode As String = String.Empty

    ''' <summary>
    ''' ポイント項目番号（ポイント項目マスタに登録されているもの）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PointItemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 付与ポイント数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Point As Integer = Integer.MinValue

    ''' <summary>
    ''' ポイント対象日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PointTargetDate As Date = Date.MinValue

    ''' <summary>
    ''' ポイント有効期限を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Date型にしていますが時間は無視されます</remarks>
    Public Property PointExpirationDate As Date = Date.MinValue

    ''' <summary>
    ''' 付与理由を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Reason As String = String.Empty
#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QolmsPointGrantItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    Public Sub New(memberShipLevel As QyMemberShipTypeEnum, actionDate As Date, serialCode As String, pointItemType As QyPointItemTypeEnum, pointExpirationDate As Date,
                    Optional reason As String = "")
        Me.New(memberShipLevel, actionDate, serialCode, pointItemType, pointExpirationDate,
                    actionDate, reason)
    End Sub

    Public Sub New(memberShipLevel As QyMemberShipTypeEnum, actionDate As Date, serialCode As String, pointItemType As QyPointItemTypeEnum, pointExpirationDate As Date,
                    pointTargetDate As Date, Optional reason As String = "")
        Me.ActionDate = actionDate
        Me.SerialCode = serialCode
        Me.PointItemNo = pointItemType
        Select Case memberShipLevel
            Case QyMemberShipTypeEnum.LimitedTime, QyMemberShipTypeEnum.Premium, QyMemberShipTypeEnum.Business
                'プレミアム会員であれば500ポイントの以外は2倍ポイント
                If _pointDoubleItem.IndexOf(pointItemType) >= 0 Then
                    Me.Point = PointValues(PointItemNo) * 2
                Else
                    Me.Point = PointValues(PointItemNo)
                End If
            Case Else
                Me.Point = PointValues(PointItemNo)
        End Select
        Me.PointExpirationDate = pointExpirationDate
        Me.PointTargetDate = pointTargetDate
        Me.Reason = reason
    End Sub
#End Region

End Class

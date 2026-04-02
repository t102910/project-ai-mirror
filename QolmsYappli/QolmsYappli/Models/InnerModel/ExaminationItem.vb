Imports System.Runtime.Serialization
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 結果項目を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ExaminationItem

#Region "Variable"

    ''' <summary>
    ''' 一意のキーを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _keyGuid As Guid = Guid.NewGuid

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 一意のキーを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property KeyGuid As Guid

        Get
            Return _keyGuid
        End Get

    End Property

    ''' <summary>
    ''' 検査結果項目の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemType As QyExaminationItemTypeEnum = QyExaminationItemTypeEnum.None

    ''' <summary>
    ''' 検査項目コード（JLAC10）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Code As String = String.Empty

    ''' <summary>
    ''' 検査項目コード（ローカルコード）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LocalCode As String = String.Empty

    ''' <summary>
    ''' 検査項目コード表示名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String = String.Empty

    ''' <summary>
    ''' 検査結果データ型を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ValueType As QyExaminationItemValueTypeEnum = QyExaminationItemValueTypeEnum.None

    ''' <summary>
    ''' 結果を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value As String = String.Empty

    ''' <summary>
    ''' 単位コードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Unit As String = String.Empty

    ''' <summary>
    ''' 結果解釈コード（H | L | N）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Interpretation As String = String.Empty

    ''' <summary>
    ''' 基準値下限閾値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Low As String = String.Empty

    ''' <summary>
    ''' 基準値上限閾値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property High As String = String.Empty

    ''' <summary>
    ''' 基準値を表す文字列パターンを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReferenceDisplayName As String = String.Empty

    ''' <summary>
    ''' 検査結果情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChildN As New List(Of ExaminationItem)()

    ''' <summary>
    ''' 検査結果に対するコメントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    Public Property Comment As String = String.Empty

    ''' <summary>
    ''' 基準とする単位系と異なるかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsDifferentUnit As Boolean = False

    ''' <summary>
    ''' 結果を保持しているかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsEmpty As Boolean

        Get
            Return String.IsNullOrWhiteSpace(Me.Value)
        End Get

    End Property

    ''' <summary>
    ''' 結果データ型が物理量かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsPhysicalQuantity As Boolean

        Get
            Return Me.ValueType = QyExaminationItemValueTypeEnum.PQ
        End Get

    End Property

    ''' <summary>
    ''' 結果が下限閾値を下回っているかを取得します。
    ''' この値は結果データ型が物理量の場合のみ有効です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsLower As Boolean

        Get
            Return Me.IsPhysicalQuantity AndAlso String.Compare(Me.Interpretation, "L", True) = 0
        End Get

    End Property

    ''' <summary>
    ''' 結果が上限閾値を上回っているかを取得します。
    ''' この値は結果データ型が物理量の場合のみ有効です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsHigher As Boolean

        Get
            Return Me.IsPhysicalQuantity AndAlso String.Compare(Me.Interpretation, "H", True) = 0
        End Get

    End Property

    ''' <summary>
    ''' 結果が基準値範囲外かを取得します。
    ''' この値は結果データ型が物理量の場合のみ有効です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsAbnormalValue As Boolean

        Get
            Return Me.IsLower OrElse Me.IsHigher
        End Get

    End Property

    <Obsolete("検討中")>
    Public Property GroupNo As String = String.Empty

    <Obsolete("検討中")>
    Public Property DispOrder As String = String.Empty

#End Region

#Region "Public Method"

    <Obsolete("検討中")>
    Public Function GetKey() As Tuple(Of String, String)

        Return New Tuple(Of String, String)(Me.Code, Me.LocalCode)

    End Function

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ExaminationItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 検査結果項目を指定して、
    ''' <see cref="CheckupElement" />クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="entry">検査結果項目。</param>
    ''' <remarks></remarks>
    Public Sub New(entry As QhApiExaminationItem)

        Me.InitializeBy(entry)

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 検査結果項目を指定して、
    ''' <see cref="CheckupElement" />クラスのインスタンスを初期化します。
    ''' </summary>
    ''' <param name="entry">検査結果項目。</param>
    ''' <remarks></remarks>
    Private Sub InitializeBy(entry As QhApiExaminationItem)

        If entry IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(entry.Value) Then
            With entry
                Dim enumItemType As QyExaminationItemTypeEnum = QyExaminationItemTypeEnum.None
                Dim enumValueType As QyExaminationItemValueTypeEnum = QyExaminationItemValueTypeEnum.None

                [Enum].TryParse(.ItemType, enumItemType)
                [Enum].TryParse(.ValueType, enumValueType)

                If (enumItemType = QyExaminationItemTypeEnum.Value OrElse enumItemType = QyExaminationItemTypeEnum.TotalJudgment) _
                    AndAlso enumValueType <> QyExaminationItemValueTypeEnum.None Then

                    Me.ItemType = enumItemType
                    Me.Code = .Code
                    Me.Name = .Name
                    Me.ValueType = enumValueType
                    Me.Value = .Value

                    If Me.ValueType = QyExaminationItemValueTypeEnum.PQ Then
                        Me.Unit = .Unit
                        Me.Interpretation = .Interpretation
                        Me.Low = .Low
                        Me.High = .High
                        Me.ReferenceDisplayName = .ReferenceDisplayName
                    End If

                    '検査項目を入れ子構造で持つかどうかは要検討
                    'Me.Children = .CheckupEntryN.Where(
                    '    Function(i)
                    '        [Enum].TryParse(i.EntryType, enumEntryType)
                    '        [Enum].TryParse(i.ValueType, enumValueType)

                    '        Return enumEntryType = KmApiCheckupEntryTypeEnum.Judgment _
                    '            AndAlso enumValueType = KmApiCheckupEntryValueTypeEnum.ST _
                    '            AndAlso Not String.IsNullOrWhiteSpace(i.Value)
                    '    End Function
                    ').OrderBy(
                    '    Function(i) i.Code
                    ').Select(
                    '    Function(i) i.ToCheckupElement()
                    ').ToList()


                End If
            End With
        End If

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' インスタンスのディープコピーを作成します。
    ''' </summary>
    ''' <returns>
    ''' このインスタンスをディープコピーしたオブジェクト。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Copy() As ExaminationItem

        Dim result As ExaminationItem = Nothing

        Using stream As New MemoryStream()
            With New BinaryFormatter()
                .Serialize(stream, Me)

                stream.Position = 0

                result = DirectCast(.Deserialize(stream), ExaminationItem)

            End With
        End Using

        Return result

    End Function

#End Region

End Class
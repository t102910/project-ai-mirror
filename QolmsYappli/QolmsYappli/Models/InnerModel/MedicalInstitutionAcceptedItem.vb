''' <summary>
''' 医療機関の診療時間情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class MedicalInstitutionAcceptedItem

#Region "Public Property"

    ''' <summary>
    ''' 診療可能かどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AccepteDays As New List(Of Boolean)

    ''' <summary>
    ''' 診療開始時間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AcceptedStart As String = String.Empty

    ''' <summary>
    ''' 診療終了時間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AcceptedEnd As String = String.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="MedicalInstitutionItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class

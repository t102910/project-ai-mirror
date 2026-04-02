Imports System.Collections.ObjectModel

<Serializable()>
Public NotInheritable Class HealthAgeAdviceAreaPartialViewModel
    Inherits QyPartialViewModelBase(Of HealthAgeViewModel)

#Region "Variable"

    Private _hasData As Boolean = False

    Private _adviceData As New List(Of HealthAgeAdviceItem)()

#End Region

#Region "Public Property"

    Public ReadOnly Property HasData As Boolean

        Get
            Return Me._hasData
        End Get

    End Property

    Public ReadOnly Property AdviceData As ReadOnlyCollection(Of HealthAgeAdviceItem)

        Get
            Return Me._adviceData.ToList().AsReadOnly()
        End Get

    End Property

#End Region

#Region "Private Method"

    Private Sub InitializeBy(items As IEnumerable(Of HealthAgeAdviceItem))

        Me._hasData = False
        Me._adviceData = New List(Of HealthAgeAdviceItem)()

        If items IsNot Nothing AndAlso items.Any() Then
            Me._hasData = True
            Me._adviceData = items.ToList()
        End If

    End Sub

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeAdviceAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="HealthAgeAdviceAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As HealthAgeViewModel)

        MyBase.New(model)

        Me.InitializeBy(model.HealthAgeAdviceN)

    End Sub

#End Region

End Class

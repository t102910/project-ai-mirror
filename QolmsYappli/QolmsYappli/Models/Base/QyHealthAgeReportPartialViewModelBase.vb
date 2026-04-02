Imports System.Collections.ObjectModel

<Serializable()>
Public MustInherit Class QyHealthAgeReportPartialViewModelBase
    Inherits QyPartialViewModelBase(Of QyPageViewModelBase)

#Region "Variable"

    ''' <summary>
    ''' ビューに展開する展開する健康年齢レポート情報を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected _reportItem As New HealthAgeReportItem() ' TODO:

    'Protected _graphIemN As New List(Of HealthAgeReportGraphItem)() ' TODO 

#End Region

#Region "Public Property"

    Public ReadOnly Property ReportItem As HealthAgeReportItem

        Get
            Return Me._reportItem
        End Get

    End Property

    'Public ReadOnly Property GraphIemN As ReadOnlyCollection(Of HealthAgeReportGraphItem)

    '    Get
    '        Return Me._graphIemN.AsReadOnly()
    '    End Get

    'End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyHealthAgeReportPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

    End Sub

    Protected Sub New(model As HealthAgeViewModel, reportItem As HealthAgeReportItem)

        MyBase.New(model)

        Me._reportItem = reportItem

        'Me.InitializeBy(Me._reportItem)

    End Sub

#End Region

#Region "Protected Method"

    Protected MustOverride Sub InitializeBy(reportItem As HealthAgeReportItem)

#End Region

End Class

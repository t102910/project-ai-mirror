Imports System.Collections.ObjectModel

''' <summary>
''' 「お薬手帳」画面の、
''' お薬テーブル パーシャルビューモデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteMedicineTablePartialViewModel
    Inherits QyPartialViewModelBase(Of NoteMedicineViewModel)

#Region "Variable"

    ''' <summary>
    ''' 日付ごとの調剤薬、市販薬情報のリストを保持します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property _medicineSetN As New List(Of MedicineItem)()

    ''' <summary>
    ''' 調剤薬情報のリストをページごとに全て保持します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property _prescriptionAllSetN As New List(Of List(Of PrescriptionItem))

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 日付ごとの調剤薬、市販薬情報のリストを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicineSetN As List(Of MedicineItem)

        Get
            Return Me._medicineSetN
        End Get
        Set(value As List(Of MedicineItem))
            Me._medicineSetN = value
        End Set

    End Property

    ''' <summary>
    ''' 調剤薬情報のリストを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property PrescriptionAllSetN As ReadOnlyCollection(Of List(Of PrescriptionItem))

        Get
            Return Me._prescriptionAllSetN.AsReadOnly()
        End Get

    End Property


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMedicineTablePartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteMedicineTablePartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">メインモデル。</param>
    ''' <param name="medicineSetN">日付ごとの調剤薬、市販薬情報のコレクション。</param>
    ''' <remarks></remarks>
    Public Sub New(model As NoteMedicineViewModel, medicineSetN As IEnumerable(Of MedicineItem))

        MyBase.New(model)

        Me._medicineSetN = If(medicineSetN IsNot Nothing AndAlso medicineSetN.Any(), medicineSetN.ToList(), New List(Of MedicineItem))

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteMedicineTablePartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">メインモデル。</param>
    ''' <param name="prescriptionAllSetN">調剤薬情報のコレクション。</param>
    ''' <remarks></remarks>
    Public Sub New(model As NoteMedicineViewModel, prescriptionAllSetN As IEnumerable(Of List(Of PrescriptionItem)))

        MyBase.New(model)

        Me._prescriptionAllSetN = If(prescriptionAllSetN IsNot Nothing AndAlso prescriptionAllSetN.Any(), prescriptionAllSetN.ToList(), New List(Of List(Of PrescriptionItem)))

    End Sub

#End Region

End Class

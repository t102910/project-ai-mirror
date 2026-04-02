Imports System.Runtime.Serialization

''' <summary>
''' 品目情報を格納するエンティティ クラスを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class FoodItem

#Region "Public Property"

    ''' <summary>
    ''' 品目名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property label As String = String.Empty

    ''' <summary>
    ''' 品目である度合を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property probability As String = String.Empty

    ''' <summary>
    ''' カロリー（kcal）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property calorie As String = String.Empty

    ''' <summary>
    ''' タンパク質（g）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property protein As String = String.Empty

    ''' <summary>
    ''' 脂質（g）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property lipid As String = String.Empty

    ''' <summary>
    ''' 炭水化物（g）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property carbohydrate As String = String.Empty

    ''' <summary>
    ''' 食塩相当量（g）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property salt_amount As String = String.Empty

    ''' <summary>
    ''' 糖質（g）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property available_carbohydrate As String = String.Empty

    ''' <summary>
    ''' 食物繊維（g）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property fiber As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="FoodItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
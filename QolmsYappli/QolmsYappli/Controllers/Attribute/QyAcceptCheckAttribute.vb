''' <summary>
''' 「コルムス ヤプリ サイト」で使用する アクション メソッド の実行時に、
''' 利用規約同意 フラグ を チェック するかを指定する属性を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Friend NotInheritable Class QyAcceptCheckAttribute
    Inherits Attribute

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyAcceptCheckAttribute" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class

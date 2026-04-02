''' <summary>
''' 「JOTO ホーム ドクター」で使用する <see cref="QyJsonResultBase" /> クラス の プロパティ が、
''' サニタイズ の対象かを指定する属性を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False)>
Friend NotInheritable Class QyForceSanitizing
    Inherits Attribute

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyForceSanitizing" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class

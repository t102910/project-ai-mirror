''' <summary>
''' 「コルムス ヤプリ サイト」で使用するアクション　メソッドの実行時に、
''' アクセス ログを出力するかを指定する属性を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Friend NotInheritable Class QyLoggingAttribute
    Inherits Attribute

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyLoggingAttribute" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class

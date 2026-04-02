''' <summary>
''' 「コルムス ヤプリ サイト」で使用する アクション メソッド の実行時に、
''' QolmsJotoApi 用 API 認証 キー を取得するかを指定する属性を表します。
''' この クラス は継承できません。
''' </summary>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Friend NotInheritable Class QjApiAuthorizeAttribute
    Inherits Attribute

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QjApiAuthorizeAttribute" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class

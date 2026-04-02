''' <summary>
''' 「コルムス ヤプリ サイト」で使用する アクション メソッド の実行時に、
''' 現在の セッション 内での画面表示回数を保持するかを指定する属性を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Friend NotInheritable Class QyViewCountAttribute
    Inherits Attribute

#Region "Public Property"

    ''' <summary>
    ''' 画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PageNo As QyPageNoTypeEnum = QyPageNoTypeEnum.None

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyViewCountAttribute" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' 画面番号を指定して、
    ''' <see cref="QyViewCountAttribute" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <param name="pageNo">画面番号の種別。</param>
    ''' <remarks></remarks>
    Public Sub New(pageNo As QyPageNoTypeEnum)

        MyBase.New()

        Me.PageNo = pageNo

    End Sub

#End Region

End Class

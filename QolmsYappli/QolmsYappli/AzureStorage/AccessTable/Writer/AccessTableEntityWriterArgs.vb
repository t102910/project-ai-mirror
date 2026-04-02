Imports MGF.QOLMS.QolmsAzureStorageCoreV1

''' <summary>
''' アクセス ログ テーブル ストレージ へ値を登録するための情報を格納する引数 クラス を表します。
''' この クラス は継承できません。
''' </summary>
''' <typeparam name="TTableEntity">アクセス ログ テーブル ストレージ エンティティ の型。</typeparam>
''' <remarks></remarks>
Friend NotInheritable Class AccessTableEntityWriterArgs(Of TTableEntity As QsAccessTableEntityBase)
    Inherits QsAzureTableStorageWriterArgsBase(Of TTableEntity)

#Region "Public Property"

    ''' <summary>
    ''' アカウント キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AccountKey As Guid = Guid.Empty

    ''' <summary>
    ''' アクセス 日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AccessDate As Date = Date.MinValue

    ''' <summary>
    ''' アクセス ログ の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AccessType As Byte = Byte.MinValue

    ''' <summary>
    ''' アクセス URI を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AccessUri As String = String.Empty

    ''' <summary>
    ''' 補足 コメント を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Comment As String = String.Empty

    ''' <summary>
    ''' ユーザー ホスト アドレス を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserHostAddress As String = String.Empty

    ''' <summary>
    ''' ユーザー ホスト 名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserHostName As String = String.Empty

    ''' <summary>
    ''' ユーザー エージェント を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserAgent As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AccessTableEntityWriterArgs(Of TTableEntity)" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class

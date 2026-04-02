Imports MGF.QOLMS.QolmsAzureStorageCoreV1

''' <summary>
''' アクセス ログ テーブル ストレージ へ値を登録するための機能を提供します。
''' この クラス は継承できません。
''' </summary>
''' <typeparam name="TTableEntity">アクセス ログ テーブル ストレージ エンティティ の型。</typeparam>
''' <remarks></remarks>
Friend NotInheritable Class AccessTableEntityWriter(Of TTableEntity As {QsAccessTableEntityBase, New})
    Inherits QsAzureTableStorageWriterBase(Of TTableEntity)
    Implements IQsAzureTableStorageWriter(Of TTableEntity, AccessTableEntityWriterArgs(Of TTableEntity), AccessTableEntityWriterResults)

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AccessTableEntityWriter(Of TTableEntity)" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New(True)

    End Sub

#End Region

#Region "IQsAzureTableStorageWriter Support"

    ''' <summary>
    ''' Azure テーブル ストレージ へ値を設定します。
    ''' </summary>
    ''' <param name="args">引数 クラス。</param>
    ''' <returns>
    ''' 戻り値 クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Execute(args As AccessTableEntityWriterArgs(Of TTableEntity)) As AccessTableEntityWriterResults Implements IQsAzureTableStorageWriter(Of TTableEntity, AccessTableEntityWriterArgs(Of TTableEntity), AccessTableEntityWriterResults).Execute

        Dim result As New AccessTableEntityWriterResults() With {.IsSuccess = False}
        Dim entity As New TTableEntity()

        With entity
            .SetPartitionKey(args.AccessDate)
            .SetRowKey(args.AccessDate, args.AccountKey)
            .AccessType = args.AccessType
            .AccessUri = args.AccessUri
            .Comment = args.Comment
            .UserHostAddress = args.UserHostAddress
            .UserHostName = args.UserHostName
            .UserAgent = args.UserAgent
        End With

        result.IsSuccess = Me.InsertOrUpdateEntities(New List(Of TTableEntity)() From {entity}).Count = 1
        result.Result = If(result.IsSuccess, 1, 0)

        Return result

    End Function

#End Region

End Class

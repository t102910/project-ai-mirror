Imports MGF.QOLMS.QolmsAzureStorageCoreV1

''' <summary>
''' アクセス ログ テーブル ストレージ へ値を登録した結果を格納する戻り値 クラス を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class AccessTableEntityWriterResults
    Inherits QsAzureTableStorageWriterResultsBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AccessTableEntityWriterResults" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class

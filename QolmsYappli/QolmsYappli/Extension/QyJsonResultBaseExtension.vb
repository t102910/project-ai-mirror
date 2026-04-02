Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' 「JOTO ホーム ドクター」で使用する、
''' <see cref="QyJsonResultBase"/> クラス の拡張機能を提供します。
''' </summary>
''' <remarks></remarks>
Friend Module QyJsonResultBaseExtension

    ''' <summary>
    ''' インスタンス の ディープ コピー を作成します。
    ''' </summary>
    ''' <typeparam name="TEntity">コピー する インスタンス の型。</typeparam>
    ''' <param name="target">コピー 元 インスタンス。</param>
    ''' <returns>
    ''' ディープ コピー された新しい インスタンス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function Copy(Of TEntity As QyJsonResultBase)(target As TEntity) As TEntity

        Using ms As New MemoryStream()
            With New BinaryFormatter()
                .Serialize(ms, target)
                ms.Position = 0

                Return DirectCast(.Deserialize(ms), TEntity)
            End With
        End Using

    End Function

End Module

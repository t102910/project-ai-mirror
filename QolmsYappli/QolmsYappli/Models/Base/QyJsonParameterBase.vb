Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' 画面ビュー内に展開するするための、
''' JSON 形式のパラメータを保持するための基本クラスを表します。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public MustInherit Class QyJsonParameterBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyJsonParameterBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' パラメータ クラスを、
    ''' JSON 形式の文字列へシリアル化します。
    ''' </summary>
    ''' <typeparam name="T">シリアル化するパラメータ クラスの型。</typeparam>
    ''' <param name="value">シリアル化するパラメータ クラス。</param>
    ''' <returns>
    ''' JSON形式の文字列。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function ToJsonString(Of T As QyJsonParameterBase)(value As T) As String

        Using ms As New IO.MemoryStream()
            With New DataContractJsonSerializer(value.GetType)
                .WriteObject(ms, value)

                Return Encoding.UTF8.GetString(ms.ToArray())
            End With
        End Using

    End Function

    ''' <summary>
    ''' JSON 形式の文字列を、
    ''' パラメータ クラスへ逆シリアル化します。
    ''' </summary>
    ''' <typeparam name="T">逆シリアル化するパラメータ クラスの型。</typeparam>
    ''' <param name="value">逆シリアル化する JSON 形式の文字列。</param>
    ''' <returns>
    ''' パラメータ クラスのインスタンス。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function FromJsonString(Of T As QyJsonParameterBase)(value As String) As T

        Using ms As New IO.MemoryStream()
            ms.Write(Encoding.UTF8.GetBytes(value), 0, Encoding.UTF8.GetByteCount(value))
            ms.Position = 0

            With New DataContractJsonSerializer(GetType(T))
                Return DirectCast(.ReadObject(ms), T)
            End With
        End Using

    End Function

    ''' <summary>
    ''' JSON 形式の文字列へシリアル化します。
    ''' </summary>
    ''' <returns>
    ''' JSON 形式の文字列。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ToJsonString() As String

        Return QyJsonParameterBase.ToJsonString(Me)

    End Function

#End Region

End Class

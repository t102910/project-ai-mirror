Imports System.Reflection
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' キャッシュ機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class QyCacheManager

#Region "Enum"

    ''' <summary>
    ''' キャッシュ対象の種別を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum QyCacheTypeEnum As Byte

        ''' <summary>
        ''' 未指定です。
        ''' </summary>
        ''' <remarks></remarks>
        None = 0

        ''' <summary>
        ''' ビューモデルのプロパティです。
        ''' </summary>
        ''' <remarks></remarks>
        ModelProperty = 1

        ''' <summary>
        ''' インプットモデルです。
        ''' </summary>
        ''' <remarks></remarks>
        InputModel = 2

        ''' <summary>
        ''' 仮アップロードされたファイルです。
        ''' </summary>
        ''' <remarks></remarks>
        PostedFile = 3

        ''' <summary>
        ''' サムネイル画像です。
        ''' </summary>
        ''' <remarks></remarks>
        Thumbnail = 4

    End Enum

#End Region

#Region "Constant"

    ''' <summary>
    ''' キャッシュを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private ReadOnly caches As New Dictionary(Of QyCacheTypeEnum, Dictionary(Of Object, Object))() From {
        {QyCacheTypeEnum.ModelProperty, New Dictionary(Of Object, Object)},
        {QyCacheTypeEnum.InputModel, New Dictionary(Of Object, Object)},
        {QyCacheTypeEnum.PostedFile, New Dictionary(Of Object, Object)},
        {QyCacheTypeEnum.Thumbnail, New Dictionary(Of Object, Object)}
    }

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyCacheManager" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' オブジェクトのディープ コピーを作成します。
    ''' </summary>
    ''' <typeparam name="T">コピーするオブジェクトの型。</typeparam>
    ''' <param name="target">コピー対象のオブジェクト。</param>
    ''' <param name="refValue">コピーされたオブジェクト。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function Copy(Of T)(target As Object, ByRef refValue As T) As Boolean

        Dim result As Boolean = False

        If target.GetType().IsValueType Then
            ' 値型
            Try
                refValue = DirectCast(target, T)
                result = True
            Catch
            End Try
        Else
            ' 参照型
            If target.GetType().IsSerializable() Then
                ' シリアル化可能
                Try
                    Using stream As New IO.MemoryStream()
                        With New BinaryFormatter()
                            .Serialize(stream, target)

                            stream.Position = 0

                            refValue = DirectCast(.Deserialize(stream), T)
                            result = True
                        End With
                    End Using
                Catch
                End Try
            Else
                ' シリアル化不可能
                Throw New InvalidOperationException("オブジェクトはシリアル化できません。")
            End If
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' キャッシュから指定した種別の値を取得します。
    ''' </summary>
    ''' <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
    ''' <typeparam name="TValue">値の型。</typeparam>
    ''' <param name="cacheType">キャッシュの種別。</param>
    ''' <param name="key">キー。</param>
    ''' <param name="refValue">取得した値が格納される変数。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetCache(Of TKey, TValue)(cacheType As QyCacheTypeEnum, key As TKey, ByRef refValue As TValue) As Boolean

        Dim result As Boolean = False
        Dim keyType As Type = key.GetType()

        If (keyType.IsValueType OrElse keyType Is GetType(String)) _
            AndAlso Me.caches.ContainsKey(cacheType) _
            AndAlso Me.caches(cacheType).ContainsKey(key) Then

            ' 複製した値を返却する
            result = Me.Copy(Of TValue)(Me.caches(cacheType)(key), refValue)
        End If

        Return result

    End Function

    ''' <summary>
    ''' キャッシュから指定した種別の全てのキーを取得します。
    ''' </summary>
    ''' <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
    ''' <param name="cacheType">キャッシュの種別。</param>
    ''' <returns>
    ''' キーのリスト。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetCacheKeys(Of TKey)(cacheType As QyCacheTypeEnum) As List(Of TKey)

        Dim result As New List(Of TKey)()

        If Me.caches.ContainsKey(cacheType) AndAlso Me.caches(cacheType).Count > 0 Then
            ' 複製したキーのリストを返却する
            Me.caches(cacheType).Keys.ToList().ForEach(
                Sub(i)
                    Dim returnKey As TKey = Nothing

                    If Me.Copy(Of TKey)(i, returnKey) Then result.Add(returnKey)
                End Sub
            )
        End If

        Return result

    End Function

    ''' <summary>
    ''' キャッシュへ指定した種別の値を追加します。
    ''' </summary>
    ''' <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
    ''' <typeparam name="TValue">値の型。</typeparam>
    ''' <param name="cacheType">キャッシュの種別。</param>
    ''' <param name="key">キー。</param>
    ''' <param name="value">値。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function SetCache(Of TKey, TValue)(cacheType As QyCacheTypeEnum, key As TKey, value As TValue) As Boolean

        Dim result As Boolean = False
        Dim keyType As Type = key.GetType()

        If (keyType.IsValueType OrElse keyType Is GetType(String)) _
            AndAlso Me.caches.ContainsKey(cacheType) _
            AndAlso Not Me.caches(cacheType).ContainsKey(key) _
            AndAlso value IsNot Nothing Then

            ' 複製したキーと値を格納する
            Dim copyKey As TKey = Nothing
            Dim copyValue As TValue = Nothing

            If Me.Copy(Of TKey)(key, copyKey) AndAlso Me.Copy(Of TValue)(value, copyValue) Then
                Me.caches(cacheType).Add(copyKey, copyValue)

                result = True
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' キャッシュから指定した種別の値を削除します。
    ''' </summary>
    ''' <typeparam name="TKey">キーの型（値型もしくは String 型）。</typeparam>
    ''' <param name="cacheType">キャッシュの種別。</param>
    ''' <param name="key">キー。</param>
    ''' <remarks></remarks>
    Public Sub RemoveCache(Of TKey)(cacheType As QyCacheTypeEnum, key As TKey)

        Dim keyType As Type = key.GetType()

        If (keyType.IsValueType OrElse keyType Is GetType(String)) _
            AndAlso Me.caches.ContainsKey(cacheType) _
            AndAlso Me.caches(cacheType).ContainsKey(key) Then

            Me.caches(cacheType).Remove(key)
        End If

    End Sub

    ''' <summary>
    ''' キャッシュから指定した種別の全ての値を削除します。
    ''' </summary>
    ''' <param name="cacheType">キャッシュの種別。</param>
    ''' <remarks></remarks>
    Public Sub ClearCache(cacheType As QyCacheTypeEnum)

        If Me.caches.ContainsKey(cacheType) Then Me.caches(cacheType).Clear()

    End Sub

#End Region

End Class

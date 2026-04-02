''' <summary>
''' インプットモデルの内容を現在のインスタンスに反映するための機能を実装します。
''' </summary>
''' <typeparam name="TModel">インプットモデルの型。</typeparam>
''' <remarks></remarks>
Public Interface IQyModelUpdater(Of TModel As Class)

    ''' <summary>
    ''' インプットモデルの内容を現在のインスタンスに反映します。
    ''' </summary>
    ''' <param name="inputModel">インプットモデル。</param>
    ''' <remarks></remarks>
    Sub UpdateByInput(inputModel As TModel)

End Interface
Imports MGF.QOLMS.QolmsCryptV1

Public NotInheritable Class StorageController
    Inherits QyMvcControllerBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="StorageController" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' 暗号化された JSON 形式のパラメータを復号化します。
    ''' </summary>
    ''' <typeparam name="T">復号化するパラメータ クラスの型。</typeparam>
    ''' <param name="reference">暗号化された JSON 形式のパラメータ。</param>
    ''' <returns>
    ''' 復号化されたパラメータ クラスのインスタンス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Function DecryptReference(Of T As QyJsonParameterBase)(reference As String) As T

        Dim result As T = Nothing

        Try
            Dim jsonString As String = String.Empty

            Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                jsonString = crypt.DecryptString(reference)
            End Using

            result = QyJsonParameterBase.FromJsonString(Of T)(jsonString)
        Catch
        End Try

        Return result

    End Function

#End Region

#Region "非ページ ビュー アクション"

#Region "分割アップロード"

    ''' <summary>
    ''' 添付ファイルのアップロード開始要求を処理します。
    ''' </summary>
    ''' <param name="name">ファイル名。</param>
    ''' <param name="size">ファイルサイズ。</param>
    ''' <param name="contentType">MIME タイプ。</param>
    ''' <returns>
    ''' JSON 形式のコンテンツ。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Start")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyLogging()>
    Public Function ChunkUploadResult(name As String, size As Nullable(Of Integer), contentType As String) As JsonResult

        Dim result As New ChunkUploadStartJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .Message = HttpUtility.HtmlEncode("ファイルのアップロードに失敗しました。")
        }
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 既存の分割アップロード済みのファイルがあれば削除
        mainModel.ClearChunkUploadFile()

        If Not String.IsNullOrWhiteSpace(name) _
            AndAlso size.HasValue _
            AndAlso size > 0 AndAlso _
            Not String.IsNullOrWhiteSpace(contentType) Then

            Dim uploaded As New ChunkUploadFile(name, size.Value, contentType, True)

            With result
                If uploaded.IsReady Then
                    ' アップロード準備完了
                    .Key = uploaded.Key
                    .SliceN = uploaded.Slices.ToList().ConvertAll(
                        Function(i) New ChunkUploadSliceJsonResult() With {
                            .Position = i.Key.ToString(),
                            .Size = i.Value.ToString()
                        }
                    )
                    .Message = String.Empty
                    .IsSuccess = Boolean.TrueString

                    ' 新規に分割アップロードするファイルを設定
                    mainModel.SetChunkUploadFile(uploaded)
                Else
                    ' アップロード準備失敗
                    .Message = HttpUtility.HtmlEncode(uploaded.ErrorMessage)
                    .IsSuccess = Boolean.FalseString
                End If
            End With
        End If

        ' JSON を返却
        Return result.ToJsonResult()

    End Function

    ''' <summary>
    ''' 添付ファイルの分割アップロード要求を処理します。
    ''' </summary>
    ''' <param name="key">ファイル キー。</param>
    ''' <param name="position">分割位置。</param>
    ''' <param name="size">分割サイズ。</param>
    ''' <param name="chunk">分割アップロードされたファイルの内容。</param>
    ''' <returns>
    ''' JSON 形式のコンテンツ。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Chunk")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyLogging()>
    Public Function ChunkUploadResult(key As String, position As Nullable(Of Integer), size As Nullable(Of Integer), chunk As HttpPostedFileBase) As JsonResult

        Dim result As New ChunkUploadJsonResult() With {
            .Key = String.Empty,
            .Progress = "0",
            .IsCompleted = Boolean.FalseString,
            .IsSuccess = Boolean.FalseString
        }
        Dim uploaded As ChunkUploadFile = Me.GetQolmsYappliModel().GetChunkUploadFile() ' 分割アップロード中のファイルを取得

        If uploaded IsNot Nothing _
            AndAlso position.HasValue _
            AndAlso size.HasValue _
            AndAlso chunk IsNot Nothing Then

            Try
                Dim buf() As Byte = New Byte(size.Value - 1) {}
                Dim progress As Double = 0
                Dim isCompleted As Boolean = False

                chunk.InputStream.Read(buf, 0, size.Value)

                ' 分割アップロードされたファイルの内容を結合
                If uploaded.MergeChunk(key, position.Value, size.Value, buf, progress, isCompleted) Then
                    ' 結合成功
                    result.Key = key
                    result.Progress = progress.ToString()
                    result.IsCompleted = isCompleted.ToString() ' 全ての断片が結合されると True になる
                    result.IsSuccess = Boolean.TrueString
                End If
            Catch
            End Try
        End If

        ' JSON を返却
        Return result.ToJsonResult()

    End Function

#End Region

#Region "「食事」画面"

    ''' <summary>
    ''' 「食事」画面の添付ファイルの取得要求を処理します。
    ''' </summary>
    ''' <param name="reference">
    ''' 取得対象のファイル情報を表す、
    ''' 暗号化された JSON 文字列（<see cref="FileStorageReferenceJsonParameter" /> クラス）。
    ''' </param>
    ''' <returns>
    ''' JPG 形式のファイルコンテンツ。
    ''' ファイルが存在しなければ EmptyResult。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyAuthorize(True)>
    <OutputCache(CacheProfile:="NoteMealFileCacheProfile")>
    <QyLogging()>
    <QyApiAuthorize()>
    Public Function MealFile(reference As String) As ActionResult

        ' ファイル情報を復号化
        Dim jsonObject As FileStorageReferenceJsonParameter = Me.DecryptReference(Of FileStorageReferenceJsonParameter)(reference)

        Try
            If jsonObject IsNot Nothing Then
                Dim originalName As String = String.Empty
                Dim contentType As String = String.Empty

                ' ファイルを返却
                If jsonObject.FileKey <> Guid.Empty Then
                    ' 登録済みファイル
                    Return New FileContentResult(NoteMealWorker4.GetFile(Me.GetQolmsYappliModel(), jsonObject.FileKey, jsonObject.FileType, originalName, contentType), contentType)
                Else
                    ' 仮アップロード中ファイル
                    Return New FileContentResult(NoteMealWorker4.GetTempFile(Me.GetQolmsYappliModel(), jsonObject.TempFileKey, jsonObject.FileType, originalName, contentType), contentType)
                End If
            Else
                Return New EmptyResult()
            End If
        Catch
            Return New EmptyResult()
        End Try

    End Function

    ''' <summary>
    ''' 「食事」画面の添付ファイルの追加（分割アップロード完了）要求を処理します。
    ''' </summary>
    ''' <param name="key">ファイル キー。</param>
    ''' <param name="dummy">ダミー引数（メソッドのオーバーロード用）。</param>
    ''' <returns>
    ''' JSON 形式のコンテンツ。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Add")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyLogging()>
    Public Function MealFileResult(key As String, dummy As String) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim tempFileKey As String = String.Empty
        Dim result As AttachedFileAddJsonResult = AttachedFileWorker.GetPostedFile(mainModel, mainModel.GetChunkUploadFile(), False, tempFileKey) ' 分割アップロード済みのファイルから添付ファイルを取得

        If result.IsSuccess.CompareTo(Boolean.TrueString) = 0 Then

            ' 表示対象のモデルをキャッシュから取得
            Dim viewmodel As NoteMealViewModel = mainModel.GetInputModelCache(Of NoteMealViewModel)()
            ' 編集対象のモデルをキャッシュから取得
            Dim inputModel As NoteMealInputModel = viewmodel.InputModel
            inputModel.AttachedFileN.Clear()

            ' モデルへ添付ファイルを追加
            If inputModel.AddAttachedFile(
                New AttachedFileItem() With {
                    .FileKey = Guid.Empty,
                    .TempFileKey = tempFileKey,
                    .OriginalName = IO.Path.GetFileNameWithoutExtension(result.OriginalName) + ".jpg"
                }
            ) Then

                ' キャッシュへ再格納
                viewmodel.InputModel = inputModel
                mainModel.SetInputModelCache(viewmodel)

                ' 分割アップロード済みのファイルを削除
                mainModel.ClearChunkUploadFile()
            Else
                result.Message = HttpUtility.HtmlEncode("ファイルのアップロードに失敗しました。")
                result.IsSuccess = Boolean.FalseString
            End If
        Else
            result.Message = HttpUtility.HtmlEncode(result.Message)
        End If

        ' JSON を返却
        Return result.ToJsonResult()

    End Function

    ''' <summary>
    ''' 「食費」画面の添付ファイルの削除要求を処理します。
    ''' </summary>
    ''' <param name="reference">
    ''' 削除対象のファイル情報を表す、
    ''' 暗号化された JSON 文字列（<see cref="FileStorageReferenceJsonParameter" /> クラス）。
    ''' </param>
    ''' <returns>
    ''' JSON 形式のコンテンツ。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize(True)>
    <QyActionMethodSelector("Remove")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <QyLogging()>
    <Obsolete("未使用です。")>
    Public Function MealFileResult(reference As String) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim result As New AttachedFileRemoveJsonResult() With {.IsSuccess = Boolean.FalseString}

        ' ファイル情報を復号化
        Dim jsonObject As FileStorageReferenceJsonParameter = Me.DecryptReference(Of FileStorageReferenceJsonParameter)(reference)

        Try
            If jsonObject IsNot Nothing Then
                ' 編集対象のモデルをキャッシュから取得
                Dim inputModel As NoteMealInputModel = mainModel.GetInputModelCache(Of NoteMealInputModel)()

                ' モデルから添付ファイルを削除
                result.IsSuccess = inputModel.RemoveAttachedFile(
                    New AttachedFileItem() With {
                        .FileKey = jsonObject.FileKey,
                        .TempFileKey = jsonObject.TempFileKey,
                        .OriginalName = String.Empty
                    }
                ).ToString()

                ' キャッシュへ再格納
                mainModel.SetInputModelCache(inputModel)
            End If
        Catch
        End Try

        ' JSON を返却
        Return result.ToJsonResult()

    End Function

#End Region

#End Region

End Class

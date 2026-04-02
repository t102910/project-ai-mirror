Imports System.Collections.ObjectModel
Imports System.Drawing
Imports System.Drawing.Imaging
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' 添付 ファイル に関する機能を提供します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class AttachedFileWorker

#Region "Enum"

    ''' <summary>
    ''' 画像の回転方向の種別を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum ImageOrientationEnum

        ''' <summary>
        ''' 回転無しです。
        ''' </summary>
        ''' <remarks></remarks>
        Horizontal = 1

        ''' <summary>
        ''' 180 度回転です。
        ''' </summary>
        ''' <remarks></remarks>
        Rotate180 = 3

        ''' <summary>
        ''' 時計回り 90 度回転です。
        ''' </summary>
        ''' <remarks></remarks>
        Rotate90CW = 6

        ''' <summary>
        ''' 時計回り 270 度回転です。
        ''' </summary>
        ''' <remarks></remarks>
        Rotate270CW = 8

    End Enum

#End Region

#Region "Constant"

    ''' <summary>
    ''' Exif 画像方向 タグ ID を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const EXIF_ORIENTATION_TAGID As Integer = &H112

    ''' <summary>
    ''' サムネイル の横幅（ピクセル）を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const THUMBNAIL_WIDTH As Integer = 120

    ''' <summary>
    ''' 許可する ファイル の最小 サイズ を表します（マジック ナンバー 検証のために最低 8 Byte 必要）。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_MIN_SIZE As Integer = 8

    ''' <summary>
    ''' 許可する ファイル の最大 サイズ を表します（10 MB）。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_MAX_SIZE As Integer = 1024 * 1024 * 10

    ''' <summary>
    ''' 許可する ファイル 名の長さを表します（拡張子込み）。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_NAME_LENGTH As Integer = 100

    ''' <summary>
    ''' ファイル の最小 サイズ が不正であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_MIN_SIZE_ERROR_MESSAGE As String = "ファイルサイズが不正です。"

    ''' <summary>
    ''' ファイル の最大 サイズ が不正であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_MAX_SIZE_ERROR_MESSAGE As String = "10MB以下のファイルを選択してください。"

    ''' <summary>
    ''' ファイル 名の長さが不正であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_NAME_LENGTH_ERROR_MESSAGE As String = "ファイル名が100文字以下のファイルを選択してください。"

    ''' <summary>
    ''' ファイル の種類が不正であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const FILE_EXTENSION_ERROR_MESSAGE As String = "pdf、bmp、jpg、jpeg、pngファイルを選択してください。"

    ''' <summary>
    ''' 画像 ファイル の種類が不正であることを表す エラー メッセージ です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const IMAGE_FILE_EXTENSION_ERROR_MESSAGE As String = "bmp、jpg、jpeg、pngファイルを選択してください。"

    ''' <summary>
    ''' 許可する ファイル の拡張子と マジック ナンバー の 読み取り専用 ディクショナリ を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly fileMagicNumbers As New Lazy(Of ReadOnlyDictionary(Of String, Byte()))(
        Function() New ReadOnlyDictionary(Of String, Byte())(
            New Dictionary(Of String, Byte())() From {
                {"pdf", {&H25, &H50, &H44, &H46, &H2D}},
                {"bmp", {&H42, &H4D}},
                {"jpg", {&HFF, &HD8}},
                {"jpeg", {&HFF, &HD8}},
                {"png", {&H89, &H50, &H4E, &H47, &HD, &HA, &H1A, &HA}}
            }
        )
    )

    ''' <summary>
    ''' 許可する ファイル の拡張子と MIME タイプ の 読み取り専用 ディクショナリ を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly fileContentTypes As New Lazy(Of ReadOnlyDictionary(Of String, String))(
        Function() New ReadOnlyDictionary(Of String, String)(
            New Dictionary(Of String, String)() From {
                {"pdf", "application/pdf"},
                {"bmp", "image/bmp"},
                {"jpg", "image/jpeg"},
                {"jpeg", "image/jpeg"},
                {"png", "image/png"}
            }
        )
    )

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタ は使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' ファイル の拡張子と マジック ナンバー の組み合わせを検証します。
    ''' </summary>
    ''' <param name="uploadedFile">アップロード された ファイル。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CheckExtensionAndMagicNumber(uploadedFile As ChunkUploadFile) As Boolean

        Dim result As Boolean = False

        If uploadedFile IsNot Nothing AndAlso uploadedFile.IsCompleted Then
            Dim ext As String = IO.Path.GetExtension(uploadedFile.Name).Replace(".", String.Empty).ToLower()

            ' 拡張子を チェック
            If AttachedFileWorker.fileMagicNumbers.Value.ContainsKey(ext) Then
                ' マジック ナンバー を チェック
                Try
                    Dim length As Integer = AttachedFileWorker.fileMagicNumbers.Value(ext).Length
                    Dim data(length - 1) As Byte

                    Buffer.BlockCopy(uploadedFile.Contents, 0, data, 0, length)
                    result = data.SequenceEqual(AttachedFileWorker.fileMagicNumbers.Value(ext))
                Catch
                End Try
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' ファイル の拡張子と MIME タイプ の組み合わせを検証します。
    ''' </summary>
    ''' <param name="uploadedFile">アップロード された ファイル。</param>
    ''' <param name="refIsImage">
    ''' アップロード された ファイル が、
    ''' 画像かどうかが格納される変数。
    ''' </param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CheckExtensionAndContentType(uploadedFile As ChunkUploadFile, ByRef refIsImage As Boolean) As Boolean

        Dim result As Boolean = False

        refIsImage = False

        If uploadedFile IsNot Nothing AndAlso uploadedFile.IsCompleted Then
            Dim ext As String = IO.Path.GetExtension(uploadedFile.Name).Replace(".", String.Empty).ToLower()
            Dim contentType As String = uploadedFile.ContentType.Replace(" ", String.Empty).ToLower()

            ' 拡張子を チェック
            If AttachedFileWorker.fileContentTypes.Value.ContainsKey(ext) Then
                ' MIME タイプ を チェック
                If contentType.CompareTo(AttachedFileWorker.fileContentTypes.Value(ext)) = 0 Then
                    result = True
                ElseIf contentType.StartsWith("image/x-") And contentType.EndsWith("-bmp") Then
                    ' 一部 Bitmap 用の回避処理
                    result = True
                End If

                refIsImage = result And contentType.StartsWith("image/")
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' Exif タグ より画像の回転方向を取得します。
    ''' </summary>
    ''' <param name="source">取得元 イメージ。</param>
    ''' <returns>
    ''' 取得できれば回転方向を表す <see cref="ImageOrientationEnum" />、
    ''' 取得出来なければ <see cref="ImageOrientationEnum.Horizontal" />。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function GetImageOrientation(source As Image) As ImageOrientationEnum

        Dim result As ImageOrientationEnum = ImageOrientationEnum.Horizontal

        If source IsNot Nothing Then
            Try
                Dim item As PropertyItem = source.PropertyItems.ToList().Find(Function(i) i.Id = AttachedFileWorker.EXIF_ORIENTATION_TAGID AndAlso i.Type = 3 AndAlso i.Len = 2)

                If item IsNot Nothing Then
                    Dim value As Byte = If(item.Value(0) <> 0, item.Value(0), item.Value(1))

                    ' 回転のみ サポート（反転は未 サポート）
                    result = DirectCast([Enum].ToObject(GetType(ImageOrientationEnum), value), ImageOrientationEnum)
                End If
            Catch
            End Try
        End If

        Return result

    End Function

    ''' <summary>
    ''' 画像の バイト 配列から イメージ を作成します。
    ''' イメージ は必要に応じて回転変換されます。
    ''' </summary>
    ''' <param name="data">画像の バイト 配列。</param>
    ''' <returns>
    ''' 成功なら イメージ、
    ''' 失敗なら Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CreateImage(data() As Byte) As Image

        Dim result As Image = Nothing

        If data IsNot Nothing AndAlso data.Length > 0 Then
            Try
                Using stream As New IO.MemoryStream(data)
                    result = Image.FromStream(stream)

                    Select Case AttachedFileWorker.GetImageOrientation(result)
                        Case ImageOrientationEnum.Rotate180
                            ' 180 度回転
                            result.RotateFlip(RotateFlipType.Rotate180FlipNone)

                        Case ImageOrientationEnum.Rotate90CW
                            ' 時計回りに 90 度回転
                            result.RotateFlip(RotateFlipType.Rotate90FlipNone)

                        Case ImageOrientationEnum.Rotate270CW
                            ' 時計回りに 270 度回転
                            result.RotateFlip(RotateFlipType.Rotate270FlipNone)

                    End Select
                End Using
            Catch
            End Try
        End If

        Return result

    End Function

    ''' <summary>
    ''' イメージから JPEG 形式の サムネイル 画像を作成します。
    ''' </summary>
    ''' <param name="source">作成元 イメージ。</param>
    ''' <param name="withinWidth">サムネイル の横幅（ピクセル）。</param>
    ''' <param name="withinHeight">サムネイル の縦幅（ピクセル）。</param>
    ''' <param name="mode">サムネイル の品質。</param>
    ''' <returns>
    ''' 成功なら JPEG 形式の サムネイル 画像の バイト 配列、
    ''' 失敗なら Nothing。
    ''' サムネイル 画像は、
    ''' 作成元 イメージ の縦横比を維持して、
    ''' 指定された横幅・縦幅に収まるように縮小されます。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CreateJpegThumbnailBytes(source As Image, withinWidth As Integer, withinHeight As Integer, mode As Drawing2D.InterpolationMode) As Byte()

        Dim result() As Byte = Nothing
        Dim w As Double
        Dim h As Double

        If source IsNot Nothing AndAlso withinWidth > 0 AndAlso withinHeight > 0 Then
            If source.Width >= source.Height Then
                w = withinWidth
                h = w * source.Height / source.Width

                If h > withinHeight Then
                    w = w * withinHeight / h
                    h = withinHeight
                End If
            Else
                h = withinHeight
                w = h * source.Width / source.Height

                If w > withinWidth Then
                    h = h * withinWidth / w
                    w = withinWidth
                End If
            End If

            Using bmp As New Bitmap(Convert.ToInt32(Math.Floor(w)), Convert.ToInt32(Math.Floor(h)))
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.InterpolationMode = mode
                    g.DrawRectangle(Pens.Transparent, New Rectangle(0, 0, bmp.Width, bmp.Height))
                    g.DrawImage(source, New Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel)

                    Using stream As New IO.MemoryStream()
                        Dim jpegEncoder As ImageCodecInfo = ImageCodecInfo.GetImageEncoders().ToList().Find(Function(i) i.FormatID = ImageFormat.Jpeg.Guid)

                        If jpegEncoder IsNot Nothing Then
                            Dim category As Imaging.Encoder = Imaging.Encoder.Quality
                            Dim params As New EncoderParameters(1)

                            params.Param(0) = New EncoderParameter(category, 100L)
                            bmp.Save(stream, jpegEncoder, params)

                            If stream.Length > 0 Then
                                Dim length As Integer = Convert.ToInt32(stream.Length)
                                Dim bytes(length - 1) As Byte

                                stream.Position = 0
                                stream.Read(bytes, 0, length)

                                result = bytes
                            End If
                        End If
                    End Using
                End Using
            End Using
        End If

        Return result

    End Function

    ' ''' <summary>
    ' ''' ストレージからのファイル取得 API を実行します。
    ' ''' </summary>
    ' ''' <param name="mainModel">メインモデル。</param>
    ' ''' <param name="fileKey">ファイルキー。</param>
    ' ''' <param name="fileType">ファイルの種別。</param>
    ' ''' <param name="fileRelationType">ファイルの連携先の種別。</param>
    ' ''' <param name="photoAccountKey">
    ' ''' 顔写真画像の取得対象となる人物のアカウントキー。
    ' ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ' ''' </param>
    ' ''' <returns>
    ' ''' Web API 戻り値クラス。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Private Shared Function FileFromApi(mainModel As QolmsYappliModel, fileKey As Guid, fileType As QyFileTypeEnum, fileRelationType As QyFileRelationTypeEnum, photoAccountKey As Guid) As QhBlobStorageReadApiResults

    '    Dim apiArgs As New QhBlobStorageReadApiArgs(
    '        QhApiTypeEnum.BlobStorageRead,
    '        QsApiSystemTypeEnum.Qolms,
    '        mainModel.ApiExecutor,
    '        mainModel.ApiExecutorName
    '    ) With {
    '        .ActorKey = If(fileRelationType = QyFileRelationTypeEnum.PersonPhoto, photoAccountKey, mainModel.AuthorAccount.AccountKey).ToApiGuidString(),
    '        .FileKey = fileKey.ToApiGuidString(),
    '        .FileType = fileType.ToString(),
    '        .FileRelationType = fileRelationType.ToString()
    '    }
    '    Dim apiResults As QhBlobStorageReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhBlobStorageReadApiResults)(
    '        apiArgs,
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) Then
    '            Return apiResults
    '        Else
    '            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '        End If
    '    End With

    'End Function

    ''' <summary>
    ''' ストレージ からの ファイル 取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="fileType">ファイル の種別。</param>
    ''' <param name="fileRelationType">ファイル の連携先の種別。</param>
    ''' <param name="photoAccountKey">
    ''' 顔写真画像の取得対象となる人物の アカウント キー。
    ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ''' </param>
    ''' <returns>
    ''' Web API 戻り値 クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteStorageReadApi(mainModel As QolmsYappliModel, fileKey As Guid, fileType As QyFileTypeEnum, fileRelationType As QyFileRelationTypeEnum, photoAccountKey As Guid) As QhStorageReadApiResults

        Dim apiArgs As New QhStorageReadApiArgs(
            QhApiTypeEnum.StorageRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = If(fileRelationType = QyFileRelationTypeEnum.PersonPhoto, photoAccountKey, mainModel.AuthorAccount.AccountKey).ToApiGuidString(),
            .FileRelationType = fileRelationType.ToString(),
            .FileType = fileType.ToString(),
            .FileKey = fileKey.ToApiGuidString(),
            .IsJoto = Boolean.TrueString
        }
        Dim apiResults As QhStorageReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhStorageReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    ' ''' <summary>
    ' ''' ストレージからの固定サムネイル画像取得 API を実行します。
    ' ''' </summary>
    ' ''' <param name="mainModel">メインモデル。</param>
    ' ''' <param name="originalName">オリジナルファイル名。</param>
    ' ''' <returns>
    ' ''' サムネイル画像のバイト配列。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Private Shared Function ExecuteBlobStorageThumbnailReadApi(mainModel As QolmsYappliModel, originalName As String) As Byte()

    '    Dim apiArgs As New QhBlobStorageThumbnailReadApiArgs(
    '        QhApiTypeEnum.BlobStorageThumbnailRead,
    '        QsApiSystemTypeEnum.Qolms,
    '        mainModel.ApiExecutor,
    '        mainModel.ApiExecutorName
    '    ) With {
    '        .OriginalName = originalName
    '    }
    '    Dim apiResults As QhBlobStorageThumbnailReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhBlobStorageThumbnailReadApiResults)(
    '        apiArgs,
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) AndAlso Not String.IsNullOrWhiteSpace(.Data) Then
    '            Return Convert.FromBase64String(.Data)
    '        Else
    '            Return {}
    '        End If
    '    End With

    'End Function

    ''' <summary>
    ''' ストレージ からの固定 サムネイル 画像取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="originalName">オリジナル ファイル 名。</param>
    ''' <returns>
    ''' サムネイル 画像の バイト 配列。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteStorageFixedThumbnailReadApi(mainModel As QolmsYappliModel, originalName As String) As Byte()

        Dim apiArgs As New QhStorageFixedThumbnailReadApiArgs(
            QhApiTypeEnum.StorageFixedThumbnailRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .OriginalName = originalName,
            .IsJoto = Boolean.TrueString
        }
        Dim apiResults As QhStorageFixedThumbnailReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhStorageFixedThumbnailReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) AndAlso Not String.IsNullOrWhiteSpace(.Data) Then
                Return Convert.FromBase64String(.Data)
            Else
                Return {}
            End If
        End With

    End Function

    ' ''' <summary>
    ' ''' ストレージ への ファイル 登録 API を実行します。
    ' ''' </summary>
    ' ''' <param name="mainModel">メイン モデル。</param>
    ' ''' <param name="originalName">オリジナル ファイル 名。</param>
    ' ''' <param name="contentType">ファイル の MIME タイプ。</param>
    ' ''' <param name="data">ファイル の バイト 配列。</param>
    ' ''' <param name="fileRelationType">ファイル の連携先の種別。</param>
    ' ''' <param name="photoAccountKey">
    ' ''' 顔写真画像の取得対象となる人物の アカウント キー。
    ' ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ' ''' </param>
    ' ''' <returns>
    ' ''' Web API 戻り値 クラス。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Private Shared Function ExecuteBlobStorageWriteApi(mainModel As QolmsYappliModel, originalName As String, contentType As String, data() As Byte, fileRelationType As QyFileRelationTypeEnum, photoAccountKey As Guid) As QhBlobStorageWriteApiResults

    '    Dim apiArgs As New QhBlobStorageWriteApiArgs(
    '        QhApiTypeEnum.BlobStorageWrite,
    '        QsApiSystemTypeEnum.Qolms,
    '        mainModel.ApiExecutor,
    '        mainModel.ApiExecutorName
    '    ) With {
    '        .ActorKey = If(fileRelationType = QyFileRelationTypeEnum.PersonPhoto, photoAccountKey, mainModel.AuthorAccount.AccountKey).ToApiGuidString(),
    '        .OriginalName = originalName,
    '        .ContentType = contentType,
    '        .Data = Convert.ToBase64String(data),
    '        .FileRelationType = fileRelationType.ToString()
    '    }
    '    Dim apiResults As QhBlobStorageWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhBlobStorageWriteApiResults)(
    '        apiArgs,
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) Then
    '            Return apiResults
    '        Else
    '            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '        End If
    '    End With

    'End Function

    ''' <summary>
    ''' ストレージ への ファイル 登録 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="originalName">オリジナル ファイル 名。</param>
    ''' <param name="contentType">ファイル の MIME タイプ。</param>
    ''' <param name="data">ファイル の バイト 配列。</param>
    ''' <param name="fileRelationType">ファイル の連携先の種別。</param>
    ''' <param name="photoAccountKey">
    ''' 顔写真画像の取得対象となる人物の アカウント キー。
    ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ''' </param>
    ''' <returns>
    ''' Web API 戻り値 クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteStorageWriteApi(mainModel As QolmsYappliModel, originalName As String, contentType As String, data() As Byte, fileRelationType As QyFileRelationTypeEnum, photoAccountKey As Guid) As QhStorageWriteApiResults

        Dim apiArgs As New QhStorageWriteApiArgs(
            QhApiTypeEnum.StorageWrite,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = If(fileRelationType = QyFileRelationTypeEnum.PersonPhoto, photoAccountKey, mainModel.AuthorAccount.AccountKey).ToApiGuidString(),
            .OriginalName = originalName,
            .ContentType = contentType,
            .Data = Convert.ToBase64String(data),
            .FileRelationType = fileRelationType.ToString()
        }
        Dim apiResults As QhStorageWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhStorageWriteApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function

    ' ''' <summary>
    ' ''' ストレージからのファイル取得 API を実行します。
    ' ''' サムネイルの場合はキャッシュされているものを優先して返却します。
    ' ''' </summary>
    ' ''' <param name="mainModel">メインモデル。</param>
    ' ''' <param name="fileKey">ファイルキー。</param>
    ' ''' <param name="fileType">ファイルの種別。</param>
    ' ''' <param name="fileRelationType">ファイルの連携先の種別。</param>
    ' ''' <param name="photoAccountKey">
    ' ''' 顔写真画像の取得対象となる人物のアカウントキー。
    ' ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ' ''' </param>
    ' ''' <param name="refOriginalName">取得したファイルのオリジナルファイル名が格納される変数。</param>
    ' ''' <param name="refContentType">取得したファイルのMIMEタイプが格納される変数。</param>
    ' ''' <returns>
    ' ''' ファイルのバイト配列。
    ' ''' </returns>
    ' ''' <remarks></remarks>
    'Private Shared Function ExecuteBlobStorageReadApi(mainModel As QolmsYappliModel, fileKey As Guid, fileType As QyFileTypeEnum, fileRelationType As QyFileRelationTypeEnum, photoAccountKey As Guid, ByRef refOriginalName As String, ByRef refContentType As String) As Byte()

    '    Dim result() As Byte = Nothing
    '    Dim apiResults As QhBlobStorageReadApiResults = Nothing

    '    refOriginalName = String.Empty
    '    refContentType = String.Empty

    '    If fileType = QyFileTypeEnum.Thumbnail Then
    '        ' サムネイルはキャッシュされているものを優先して使う
    '        apiResults = mainModel.GetAttachedFileThumbnail(fileKey)

    '        ' キャッシュされていなければAPIを実行
    '        If apiResults Is Nothing Then
    '            apiResults = AttachedFileWorker.FileFromApi(mainModel, fileKey, fileType, fileRelationType, photoAccountKey)

    '            ' キャッシュ
    '            mainModel.SetAttachedFileThumbnail(fileKey, apiResults)
    '        End If
    '    Else
    '        ' サムネイル以外
    '        apiResults = AttachedFileWorker.FileFromApi(mainModel, fileKey, fileType, fileRelationType, photoAccountKey)
    '    End If

    '    If apiResults IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(apiResults.Data) Then
    '        With apiResults
    '            refOriginalName = .OriginalName
    '            refContentType = .ContentType

    '            Return Convert.FromBase64String(.Data)
    '        End With
    '    End If

    '    Return result

    'End Function

    ''' <summary>
    ''' ストレージ からの ファイル 取得 API を実行します。
    ''' サムネイル の場合は キャッシュ されているものを優先して返却します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="fileType">ファイル の種別。</param>
    ''' <param name="fileRelationType">ファイル の連携先の種別。</param>
    ''' <param name="photoAccountKey">
    ''' 顔写真画像の取得対象となる人物の アカウント キー。
    ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ''' </param>
    ''' <param name="refOriginalName">取得した ファイル の オリジナル ファイル 名が格納される変数。</param>
    ''' <param name="refContentType">取得した ファイル の MIME タイプ が格納される変数。</param>
    ''' <returns>
    ''' ファイル の バイト 配列。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function GetFileFromCacheOrApi(mainModel As QolmsYappliModel, fileKey As Guid, fileType As QyFileTypeEnum, fileRelationType As QyFileRelationTypeEnum, photoAccountKey As Guid, ByRef refOriginalName As String, ByRef refContentType As String) As Byte()

        refOriginalName = String.Empty
        refContentType = String.Empty

        'Dim result() As Byte = Nothing
        Dim apiResults As QhStorageReadApiResults = Nothing

        If fileType = QyFileTypeEnum.Thumbnail Then
            ' サムネイル は キャッシュ されているものを優先して使う
            apiResults = mainModel.GetAttachedFileThumbnail(fileKey)

            ' キャッシュ されていなければ API を実行
            If apiResults Is Nothing Then
                apiResults = AttachedFileWorker.ExecuteStorageReadApi(mainModel, fileKey, fileType, fileRelationType, photoAccountKey)

                ' キャッシュ
                mainModel.SetAttachedFileThumbnail(fileKey, apiResults)
            End If
        Else
            ' サムネイル 以外
            apiResults = AttachedFileWorker.ExecuteStorageReadApi(mainModel, fileKey, fileType, fileRelationType, photoAccountKey)
        End If

        If apiResults IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(apiResults.Data) Then
            With apiResults
                refOriginalName = .OriginalName
                refContentType = .ContentType

                Return Convert.FromBase64String(.Data)
            End With
        Else
            Return Nothing
        End If

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' クライアント によって アップロード された添付 ファイル を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="uploadedFile">クライアント によって アップロード された ファイル。</param>
    ''' <param name="imageOnly">画像 ファイル のみを許可するかの フラグ。</param>
    ''' <param name="refTempFileKye">
    ''' アップロード された ファイル に対する ファイル キー が格納される変数。
    ''' アップロード に成功なら "yyyyMMddHHmmssffff" 形式の文字列、
    ''' 失敗なら String.Empty。
    ''' </param>
    ''' <returns>
    ''' 添付 ファイル を追加した結果を保持する、
    ''' JSON 形式の コンテンツ。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetPostedFile(mainModel As QolmsYappliModel, uploadedFile As ChunkUploadFile, imageOnly As Boolean, ByRef refTempFileKye As String) As AttachedFileAddJsonResult

        Dim result As New AttachedFileAddJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .Message = "ファイルのアップロードに失敗しました。"
        }
        Dim isValid As Boolean = False

        refTempFileKye = String.Empty

        If uploadedFile IsNot Nothing AndAlso uploadedFile.IsCompleted Then
            Dim contentLength As Integer = uploadedFile.Size
            Dim nameLength As Integer = uploadedFile.Name.Length
            Dim isImage As Boolean = False

            ' 最小 ファイル サイズ を検証
            If contentLength < AttachedFileWorker.FILE_MIN_SIZE Then
                ' 検証 エラー
                result.Message = AttachedFileWorker.FILE_MIN_SIZE_ERROR_MESSAGE
                isValid = False
            Else
                isValid = True
            End If

            ' 最大 ファイル サイズ を検証
            If isValid Then
                If contentLength > AttachedFileWorker.FILE_MAX_SIZE Then
                    ' 検証 エラー
                    result.Message = AttachedFileWorker.FILE_MAX_SIZE_ERROR_MESSAGE
                    isValid = False
                End If
            End If

            ' ファイル 名の長さを検証
            If isValid Then
                If nameLength <= 0 OrElse nameLength > AttachedFileWorker.FILE_NAME_LENGTH Then
                    ' 検証 エラー
                    result.Message = AttachedFileWorker.FILE_NAME_LENGTH_ERROR_MESSAGE
                    isValid = False
                End If
            End If

            ' 拡張子と マジック ナンバー の組み合わせを検証
            If isValid Then
                If Not AttachedFileWorker.CheckExtensionAndMagicNumber(uploadedFile) Then
                    ' 検証 エラー
                    result.Message = AttachedFileWorker.FILE_EXTENSION_ERROR_MESSAGE
                    isValid = False
                End If
            End If

            ' 拡張子と MIME タイプ の組み合わせを検証
            If isValid Then
                If Not AttachedFileWorker.CheckExtensionAndContentType(uploadedFile, isImage) Then
                    ' 検証 エラー
                    result.Message = If(imageOnly, AttachedFileWorker.IMAGE_FILE_EXTENSION_ERROR_MESSAGE, AttachedFileWorker.FILE_EXTENSION_ERROR_MESSAGE)
                    isValid = False
                End If
            End If

            ' 画像のみ許可の場合の検証
            If isValid Then
                If imageOnly AndAlso Not isImage Then
                    ' 検証 エラー
                    result.Message = AttachedFileWorker.IMAGE_FILE_EXTENSION_ERROR_MESSAGE
                    isValid = False
                End If
            End If

            ' ファイル を読み込み キャッシュ
            If isValid Then
                Dim fileItem As New PostedFileItem() With {
                    .TempFileKey = Date.Now.ToString("yyyyMMddHHmmssfffffff"),
                    .FileName = uploadedFile.Name,
                    .ContentType = uploadedFile.ContentType
                }
                Dim data(contentLength - 1) As Byte

                Try
                    Dim readLenth As Integer = 0

                    Using ms As New IO.MemoryStream(uploadedFile.Contents)
                        readLenth = ms.Read(data, 0, contentLength)
                    End Using

                    If readLenth = contentLength Then
                        fileItem.Data = data

                        If isImage Then
                            ' 画像の場合は サムネイル を作成
                            fileItem.Thumbnail = AttachedFileWorker.CreateJpegThumbnailBytes(AttachedFileWorker.CreateImage(data), AttachedFileWorker.THUMBNAIL_WIDTH, Integer.MaxValue, Drawing2D.InterpolationMode.HighQualityBicubic)
                        Else
                            ' 画像以外の場合は固定の サムネイル
                            'fileItem.Thumbnail = AttachedFileWorker.ExecuteBlobStorageThumbnailReadApi(mainModel, fileItem.FileName) ' API を実行
                            fileItem.Thumbnail = AttachedFileWorker.ExecuteStorageFixedThumbnailReadApi(mainModel, fileItem.FileName) ' API を実行
                        End If
                    End If

                    ' キャッシュ へ格納
                    If fileItem.Thumbnail IsNot Nothing AndAlso mainModel.SetPostedFile(fileItem.TempFileKey, fileItem) Then
                        ' 追加結果
                        result.ThumbnailReference = New AttachedFileItem() With {
                            .FileKey = Guid.Empty,
                            .TempFileKey = fileItem.TempFileKey,
                            .OriginalName = String.Empty
                        }.ToEncryptedFileStorageReference(QyFileTypeEnum.Thumbnail)

                        result.DataReference = New AttachedFileItem() With {
                            .FileKey = Guid.Empty,
                            .TempFileKey = fileItem.TempFileKey,
                            .OriginalName = String.Empty
                        }.ToEncryptedFileStorageReference(QyFileTypeEnum.None)

                        result.OriginalName = fileItem.FileName
                        result.Message = String.Empty
                        result.IsSuccess = Boolean.TrueString

                        ' ファイル キー
                        refTempFileKye = fileItem.TempFileKey
                    Else
                        ' 検証 エラー
                        result.Message = "サムネイルの作成に失敗しました。"
                    End If
                Catch
                End Try
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' 添付 ファイル を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="fileType">ファイル の種別。</param>
    ''' <param name="fileRelationType">ファイル の連携先の種別。</param>
    ''' <param name="refOriginalName">取得した ファイル の オリジナル ファイル 名が格納される変数。</param>
    ''' <param name="refContentType">取得した ファイル の MIME タイプ が格納される変数。</param>
    ''' <param name="photoAccountKey">
    ''' 顔写真画像の取得対象となる人物の アカウント キー。
    ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ''' </param>
    ''' <returns>
    ''' ファイル の バイト 配列。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetFile(mainModel As QolmsYappliModel, fileKey As Guid, fileType As QyFileTypeEnum, fileRelationType As QyFileRelationTypeEnum, ByRef refOriginalName As String, ByRef refContentType As String, Optional photoAccountKey As Guid = Nothing) As Byte()

        ' API を実行
        'Return AttachedFileWorker.ExecuteBlobStorageReadApi(mainModel, fileKey, fileType, fileRelationType, photoAccountKey, refOriginalName, refContentType)
        Return AttachedFileWorker.GetFileFromCacheOrApi(mainModel, fileKey, fileType, fileRelationType, photoAccountKey, refOriginalName, refContentType)

    End Function

    ''' <summary>
    ''' 仮 アップロード された添付 ファイル を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="tempFileKey">仮 アップロード 時の ファイル キー。</param>
    ''' <param name="fileType">ファイル の種別。</param>
    ''' <param name="refOriginalName">取得した ファイル の オリジナル ファイル 名が格納される変数。</param>
    ''' <param name="refContentType">取得した ファイル の MIME タイプ が格納される変数。</param>
    ''' <returns>
    ''' ファイル の バイト 配列。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetTempFile(mainModel As QolmsYappliModel, tempFileKey As String, fileType As QyFileTypeEnum, ByRef refOriginalName As String, ByRef refContentType As String) As Byte()

        Dim result As Byte() = Nothing
        Dim cache As PostedFileItem = mainModel.GetPostedFile(tempFileKey) ' キャッシュ から取得

        refOriginalName = String.Empty
        refContentType = String.Empty

        If cache IsNot Nothing Then
            Select Case fileType
                Case QyFileTypeEnum.Thumbnail
                    ' サムネイル 画像
                    refOriginalName = cache.FileName
                    refContentType = "image/jpeg"
                    result = cache.Thumbnail

                Case QyFileTypeEnum.Original, QyFileTypeEnum.Edited
                    ' オリジナル ファイル
                    refOriginalName = cache.FileName
                    refContentType = cache.ContentType
                    result = cache.Data

            End Select
        End If

        Return result

    End Function

    ''' <summary>
    ''' 仮 アップロード された添付 ファイル を登録します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="tempFileKey">仮 アップロード 時の ファイル キー。</param>
    ''' <param name="fileRelationType">ファイル の連携先の種別。</param>
    ''' <param name="refFileKey">登録した ファイル を指す ファイル キー が格納される変数。</param>
    ''' <param name="photoAccountKey">
    ''' 顔写真画像の登録対象となる人物の アカウント キー。
    ''' <paramref name="fileRelationType" /> = <see cref="QyFileRelationTypeEnum.PersonPhoto" /> の場合に使用します。
    ''' </param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function SetTempFile(mainModel As QolmsYappliModel, tempFileKey As String, fileRelationType As QyFileRelationTypeEnum, ByRef refFileKey As Guid, Optional photoAccountKey As Guid = Nothing) As Boolean

        Dim result As Boolean = False
        Dim cache As PostedFileItem = mainModel.GetPostedFile(tempFileKey) ' キャッシュ から取得、メイン モデル 内の メソッド 名を変える

        refFileKey = Guid.Empty

        If cache IsNot Nothing Then
            ' API を実行
            'With AttachedFileWorker.ExecuteBlobStorageWriteApi(mainModel, cache.FileName, cache.ContentType, cache.Data, fileRelationType, photoAccountKey)
            '    refFileKey = .FileKey.TryToValueType(Guid.Empty)
            '    result = (refFileKey <> Guid.Empty)
            'End With
            With AttachedFileWorker.ExecuteStorageWriteApi(mainModel, cache.FileName, cache.ContentType, cache.Data, fileRelationType, photoAccountKey)
                refFileKey = .FileKey.TryToValueType(Guid.Empty)
                result = (refFileKey <> Guid.Empty)
            End With
        End If

        Return result

    End Function

    ''' <summary>
    ''' 食事の写真画像を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="fileType">ファイル の種別。</param>
    ''' <param name="refOriginalName">取得した ファイル の オリジナル ファイル 名が格納される変数。</param>
    ''' <returns>
    ''' 成功なら ファイル の バイト 配列、
    ''' 失敗なら Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetMealFile(mainModel As QolmsYappliModel, fileKey As Guid, fileType As QyFileTypeEnum, ByRef refOriginalName As String) As Byte()

        'Dim result As Byte() = Nothing
        refOriginalName = String.Empty
        Dim refContentType As String = String.Empty

        If fileKey <> Guid.Empty Then
            '' APIを実行
            'result = AttachedFileWorker.ExecuteBlobStorageReadApi(mainModel, fileKey, QyFileTypeEnum.Original, QyFileRelationTypeEnum.MealPhoto, Nothing, refOriginalName, refContentType)
            Return AttachedFileWorker.GetFile(mainModel, fileKey, QyFileTypeEnum.Original, QyFileRelationTypeEnum.MealPhoto, refOriginalName, refContentType, Nothing)
        Else
            Return Nothing
        End If

        'Return result

    End Function

    ''' <summary>
    ''' 食事の写真画像を登録します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="photoName">オリジナル ファイル 名。</param>
    ''' <param name="photoData">ファイル の バイト 配列。</param>
    ''' <param name="refFileKey">登録した ファイル を指す ファイル キー が格納される変数。</param>
    ''' <returns>
    ''' 成功なら True、
    ''' 失敗なら False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function SetMealFile(mainModel As QolmsYappliModel, photoName As String, photoData() As Byte, ByRef refFileKey As Guid) As Boolean

        refFileKey = Guid.Empty

        'Dim result As Boolean = False

        If Not String.IsNullOrWhiteSpace(photoName) _
            AndAlso photoData IsNot Nothing _
            AndAlso photoData.Any() Then

            ' API を実行
            'With AttachedFileWorker.ExecuteBlobStorageWriteApi(mainModel, photoName, "image/jpeg", photoData, QyFileRelationTypeEnum.MealPhoto, Nothing)
            '    refFileKey = .FileKey.TryToValueType(Guid.Empty)
            '    result = (refFileKey <> Guid.Empty)
            'End With
            With AttachedFileWorker.ExecuteStorageWriteApi(mainModel, photoName, "image/jpeg", photoData, QyFileRelationTypeEnum.MealPhoto, Nothing)
                refFileKey = .FileKey.TryToValueType(Guid.Empty)

                Return refFileKey <> Guid.Empty
            End With
        Else
            Return False
        End If

        'Return result

    End Function

#End Region

End Class

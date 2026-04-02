Imports MGF.QOLMS.QolmsCryptV1

''' <summary>
''' 添付ファイル情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class AttachedFileItem

#Region "Public Property"

    ''' <summary>
    ''' ファイル キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FileKey As Guid = Guid.Empty

    ''' <summary>
    ''' 仮アップロード時のファイル キーを取得または設定します。
    ''' この値は "yyyyMMddHHmmssfffffff" 形式の文字列です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TempFileKey As String = String.Empty

    ''' <summary>
    ''' オリジナル ファイル名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OriginalName As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AttachedFileItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' ビュー内に展開する暗号化されたファイル情報への参照パラメータを生成します。
    ''' </summary>
    ''' <param name="fileType">ファイルの種別。</param>
    ''' <returns>
    ''' 暗号化されたファイル情報への参照パラメータ。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ToEncryptedFileStorageReference(fileType As QyFileTypeEnum) As String

        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
            Return crypt.EncryptString(
                New FileStorageReferenceJsonParameter() With {
                    .AccountKey = Guid.Empty,
                    .FileKey = Me.FileKey,
                    .TempFileKey = Me.TempFileKey,
                    .FileType = fileType
                }.ToJsonString()
            )
        End Using

    End Function

#End Region

End Class

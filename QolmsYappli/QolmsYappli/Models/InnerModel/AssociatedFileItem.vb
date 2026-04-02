Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 健診結果付随 ファイル 情報を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class AssociatedFileItem

#Region "Public Property"

    ''' <summary>
    ''' 連携 システム 番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 連携 システム ID を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemId As String = String.Empty

    ''' <summary>
    ''' 健診日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 施設 キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FacilityKey As Guid = Guid.Empty

    ''' <summary>
    ''' 記録 タイプ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataType As QyExaminationDataTypeEnum = QyExaminationDataTypeEnum.None

    ''' <summary>
    ''' データ キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataKey As Guid = Guid.Empty

    ''' <summary>
    ''' URL 連携の場合に使用する キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("未使用です。")>
    Public Property AdditionalKey As String = String.Empty

    ''' <summary>
    ''' ファイル取得用の承認情報を含むJsonの文字列を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FileStorageReferenceJson As String = String.Empty



#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AssociatedFileItem" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' ビュー 内に展開する暗号化された ファイル 情報への参照 パラメータ を生成します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <param name="loginAt">ログイン 日時。</param>
    ''' <param name="cryptor">
    ''' 暗号化および復号化の機能。
    ''' 暗号化の種別は <see cref="QsCryptTypeEnum.QolmsWeb" /> を使用してください。
    ''' </param>
    ''' <returns>
    ''' 暗号化された ファイル 情報への参照 パラメータ。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ToEncryptedAssociatedFileStorageReference(accountKey As Guid, loginAt As Date, cryptor As QsCrypt) As String

        If cryptor Is Nothing Then Throw New ArgumentNullException("cryptor", "暗号化および復号化の機能が null 参照です。")

        Return cryptor.EncryptString(
            New AssociatedFileStorageReferenceJsonParameter() With {
                .Accountkey = accountKey.ToApiGuidString(),
                .LoginAt = loginAt.ToApiDateString(),
                .LinkageSystemNo = Me.LinkageSystemNo.ToString(),
                .LinkageSystemId = Me.LinkageSystemId,
                .RecordDate = Me.RecordDate.ToApiDateString(),
                .FacilityKey = Me.FacilityKey.ToApiGuidString(),
                .DataKey = Me.DataKey.ToApiGuidString(),
                .AdditionalKey = Me.AdditionalKey
            }.ToJsonString()
        )

    End Function

    ''' <summary>
    ''' ビュー 内に展開する暗号化された ファイル 情報への参照 パラメータ を生成します。
    ''' </summary>
    ''' <param name="accountKey">アカウント キー。</param>
    ''' <param name="loginAt">ログイン 日時。</param>
    ''' <returns>
    ''' 暗号化された ファイル 情報への参照 パラメータ。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ToEncryptedAssociatedFileStorageReference(accountKey As Guid, loginAt As Date) As String

        Using cryptor As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
            Return ToEncryptedAssociatedFileStorageReference(accountKey, loginAt, cryptor)
        End Using

    End Function

#End Region

End Class

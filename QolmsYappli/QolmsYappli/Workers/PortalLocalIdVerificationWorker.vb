Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 「市民確認 同意」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class PortalLocalIdVerificationWorker

#Region "Constant"

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 市民確認にすでにエントリーしているかどうかを返却します。
    ''' </summary>
    ''' <param name="mainModel"></param>
    ''' <returns></returns>
    Friend Shared Function IsEntered(mainModel As QolmsYappliModel) As Boolean

        Dim result As Boolean = False
        '共通化は未定。

        '宜野湾市民確認用のLinkageSystemNo
        Dim LINKAGE_NO As Integer = 47900021
        Dim linkageItem As LinkageItem = LinkageWorker.GetLinkageItem(mainModel, LINKAGE_NO)

        result = linkageItem.LinkageSystemNo = LINKAGE_NO

        '宜野湾市エントリー用のLinkageSystemNo
        Dim GINOWAN_LINKAGE_NO As Integer = 47900022
        Dim ginowanItem As LinkageItem = LinkageWorker.GetLinkageItem(mainModel, GINOWAN_LINKAGE_NO)

        result = result AndAlso ginowanItem.LinkageSystemNo = GINOWAN_LINKAGE_NO

        Return result

    End Function

#End Region

End Class

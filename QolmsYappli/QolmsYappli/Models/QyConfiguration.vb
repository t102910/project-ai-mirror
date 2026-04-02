Imports System.Configuration
Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 沖縄セルラー Yappli の設定を取得する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class QyConfiguration

#Region "Constant"

    ''' <summary>
    ''' 沖縄セルラー Yappli のサイト名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly _QolmsYappliSiteName As String = QyConfiguration.GetQolmsYappliSiteName()

    ''' <summary>
    ''' 沖縄セルラー Yappli のサイト URI を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly _QolmsYappliSiteUri As String = QyConfiguration.GetQolmsYappliSiteUri()

    ''' <summary>
    ''' 沖縄セルラー Yappli のお問合せページ URI を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly _QolmsYappliSiteContactUri As String = QyConfiguration.GetQolmsYappliSiteContactUri()

    ''' <summary>
    ''' 沖縄セルラー Yappli の運営組織名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly _QolmsYappliSiteOwnerName As String = QyConfiguration.GetQolmsYappliSiteOwnerName()

    ''' <summary>
    ''' 沖縄セルラー Yappli の運営組織 URI を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly _QolmsYappliSiteOwnerUri As String = QyConfiguration.GetQolmsYappliSiteOwnerUri()

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 沖縄セルラー Yappli のサイト名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property QolmsYappliSiteName As String

        Get
            Return QyConfiguration._QolmsYappliSiteName
        End Get

    End Property

    ''' <summary>
    ''' 沖縄セルラー Yappli のサイト URI を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property QolmsYappliSiteUri As String

        Get
            Return QyConfiguration._QolmsYappliSiteUri
        End Get

    End Property

    ''' <summary>
    ''' 沖縄セルラー Yappli のお問合せページ URI を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property QolmsYappliSiteContactUri As String

        Get
            Return QyConfiguration._QolmsYappliSiteContactUri
        End Get

    End Property

    ''' <summary>
    ''' 沖縄セルラー Yappli の運営組織名を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property QolmsYappliSiteOwnerName As String

        Get
            Return QyConfiguration._QolmsYappliSiteOwnerName
        End Get

    End Property

    ''' <summary>
    ''' 沖縄セルラー Yappli の運営組織 URI を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property QolmsYappliSiteOwnerUri As String

        Get
            Return QyConfiguration._QolmsYappliSiteOwnerUri
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' アプリケーション構成ファイルから、
    ''' アプリケーション設定を取得します。
    ''' </summary>
    ''' <param name="key">エントリのキー。</param>
    ''' <returns>
    ''' エントリの値。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function GetConfiguration(key As String) As String

        Dim result As String = String.Empty

        Try
            result = ConfigurationManager.AppSettings(key)

            If String.IsNullOrWhiteSpace(result) Then Throw New ConfigurationErrorsException(String.Format("{0} に対する値が存在しません。", key))
        Catch
            Throw
        End Try

        Return result

    End Function

    ''' <summary>
    ''' 初期値を指定して、
    ''' アプリケーション構成ファイルから、
    ''' アプリケーション設定を取得します。
    ''' </summary>
    ''' <typeparam name="T">エントリの値の型。</typeparam>
    ''' <param name="key">エントリのキー。</param>
    ''' <param name="initialValue">変換に失敗した場合に返却される初期値を指定。</param>
    ''' <returns>
    ''' 成功なら指定した型へ変換されたエントリの値、
    ''' 失敗なら指定した初期値。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function TryGetConfiguration(Of T As Structure)(key As String, initialValue As T) As T

        Dim value As String = String.Empty

        Try
            value = QyConfiguration.GetConfiguration(key)
        Catch
        End Try

        Return If(String.IsNullOrWhiteSpace(value), initialValue, TryToValueType(value, initialValue))

    End Function

    ''' <summary>
    ''' 初期値を指定して、
    ''' アプリケーション構成ファイルから、
    ''' アプリケーション設定を取得します。
    ''' </summary>
    ''' <param name="key">エントリのキー。</param>
    ''' <param name="initialValue">変換に失敗した場合に返却される初期値を指定。</param>
    ''' <returns>
    ''' 成功ならエントリの値、
    ''' 失敗なら指定した初期値。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function TryGetConfiguration(key As String, initialValue As String) As String

        Dim value As String = String.Empty

        Try
            value = QyConfiguration.GetConfiguration(key)
        Catch
        End Try

        Return If(String.IsNullOrWhiteSpace(value), initialValue, value)

    End Function

    ''' <summary>
    ''' アプリケーション構成ファイルから、
    ''' 沖縄セルラー Yappli のサイト名を取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetQolmsYappliSiteName() As String

        Return QyConfiguration.TryGetConfiguration("QolmsYappliSiteName", "QolmsYappli")

    End Function

    ''' <summary>
    ''' アプリケーション構成ファイルから、
    ''' 沖縄セルラー Yappli のサイト URI を取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetQolmsYappliSiteUri() As String

        Return QyConfiguration.TryGetConfiguration("QolmsYappliSiteUri", "https://app.QolmsYappli.com/")

    End Function

    ''' <summary>
    ''' アプリケーション構成ファイルから、
    ''' 沖縄セルラー Yappli のお問合せページ URI を取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetQolmsYappliSiteContactUri() As String

        Return QyConfiguration.TryGetConfiguration("QolmsYappliSiteContactUri", "https://www.QolmsYappli.com/contact/")

    End Function

    ''' <summary>
    ''' アプリケーション構成ファイルから、
    ''' 沖縄セルラー Yappli の運営組織名を取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetQolmsYappliSiteOwnerName() As String

        Return QyConfiguration.TryGetConfiguration("QolmsYappliSiteOwnerName", "エムジーファクトリー株式会社")

    End Function

    ''' <summary>
    ''' アプリケーション構成ファイルから、
    ''' 沖縄セルラー Yappli の運営組織 URI を取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetQolmsYappliSiteOwnerUri() As String

        Return QyConfiguration.TryGetConfiguration("QolmsYappliYappliSiteOwnerUri", "http://www.mgfactory.co.jp/")

    End Function

#End Region

End Class
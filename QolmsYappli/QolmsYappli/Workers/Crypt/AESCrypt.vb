

Imports System.IO
Imports System.Security.Cryptography

Friend NotInheritable Class AESCrypt


#Region "Public Prop"

    Private Property _key As String = String.Empty
    Private Property _iv As String = String.Empty

#End Region


#Region "Constant"


#End Region


#Region "Constructor"

    ''' <summary>
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(key As String, iv As String)

        If String.IsNullOrWhiteSpace(key) Then Throw New ArgumentNullException("key は 必須です。")
        If String.IsNullOrWhiteSpace(iv) Then Throw New ArgumentNullException("iv は 必須です。")

        Me._key = key
        Me._iv = iv

    End Sub

#End Region

#Region "Private Method"

    'param = "{patientID: '0000001', patientName: '砂川 太郎', patientNameKana: 'スナガワ タロウ', gender: 1, birthday: '2000-01-01'}"
    Private Function AesEncrypt(param As String) As String

        Dim encodedParam As String = String.Empty
        Using aes As Aes = Aes.Create()

            '// キーと IV のサイズを設定
            aes.Key = Encoding.UTF8.GetBytes(_key)
            aes.IV = Encoding.UTF8.GetBytes(_iv)

            '// 暗号化用のトランスフォームを作成
            Dim encryptor As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)

            Using ms As MemoryStream = New MemoryStream()
                Using cs As CryptoStream = New CryptoStream(ms, encryptor, CryptoStreamMode.Write)
                    Using sw As StreamWriter = New StreamWriter(cs)
                        sw.Write(param)
                    End Using

                End Using
                '// 暗号化されたデータを Base64 に変換
                encodedParam = Convert.ToBase64String(ms.ToArray())
            End Using
        End Using

        Return encodedParam

    End Function

#End Region

#Region "Private Method"

    Public Function Encrypt(prm As String) As String

        If String.IsNullOrWhiteSpace(prm) Then Throw New ArgumentNullException("暗号化対象の文字列がありません。")

        Return AesEncrypt(prm)

    End Function

#End Region


End Class

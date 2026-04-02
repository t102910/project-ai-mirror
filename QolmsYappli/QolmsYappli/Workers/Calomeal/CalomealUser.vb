Imports System.Runtime.Serialization

    <DataContract()>
    <Serializable()>
Public NotInheritable Class CalomealUser
    Inherits QyJsonParameterBase

        <DataMember()>
        Public Property aud As String
        <DataMember()>
       Public Property jti As String
        <DataMember()>
       Public Property iat As integer
        <DataMember()>
       Public Property nbf As integer
        <DataMember()>
      Public  Property exp As integer
        <DataMember()>
       Public Property [sub] As String
        <DataMember()>
      Public  Property scopes As List(Of string)

        Public Sub New()

        End Sub
    End Class

            ''{"aud":"test_client1",
            '"jti":"0c5c0d362af8fee8e77f24fce8c52c248a6c11e74cbee9c9d386c5a8221a2df3fda400da4b8b64c9",
            '"iat":1688369315,
            '"nbf":1688369315,
            '"exp":1688455715,
            '"sub":"3200",
            '"scopes":["all"]}


Namespace QolmsYappli.OwlRequest


    '''<remarks/>
    <System.SerializableAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kddi.com/cocoa"), _
     System.Xml.Serialization.XmlRootAttribute([Namespace]:="http://www.kddi.com/cocoa", IsNullable:=False)> _
    Partial Public Class biscuitif

        Private sidField As String

        Private fidField As String

        Private utypeField As String

        Private uidtfField As String

        '''<remarks/>
        Public Property sid() As String
            Get
                Return Me.sidField
            End Get
            Set(value As String)
                Me.sidField = value
            End Set
        End Property

        '''<remarks/>
        Public Property fid() As String
            Get
                Return Me.fidField
            End Get
            Set(value As String)
                Me.fidField = value
            End Set
        End Property

        '''<remarks/>
        Public Property utype() As String
            Get
                Return Me.utypeField
            End Get
            Set(value As String)
                Me.utypeField = value
            End Set
        End Property

        '''<remarks/>
        Public Property uidtf() As String
            Get
                Return Me.uidtfField
            End Get
            Set(value As String)
                Me.uidtfField = value
            End Set
        End Property
    End Class


End Namespace


''' <summary>
''' 「健診結果」画面の検査結果表パーシャルビューモデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class NoteExaminationResultPartialViewModel
    Inherits QyPartialViewModelBase(Of NoteExaminationViewModel)

#Region "Constructor"

    ''' <summary>
    ''' 親ビューモデルを指定して、
    ''' <see cref="NoteExaminationResultPartialViewModel" />クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="model">親ビューモデル。</param>
    ''' <remarks></remarks>
    Public Sub New(model As NoteExaminationViewModel)

        MyBase.New(model)

    End Sub

#End Region

End Class
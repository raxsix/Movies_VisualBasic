Public Interface IMovieInterface

    Function getJsonString() As String

    Function parseJsonString(ByVal jsonString) As List(Of Movie)

End Interface

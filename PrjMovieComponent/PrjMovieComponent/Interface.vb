
Public Interface IMovieInterface

    Function getJsonString(ByVal valik As Integer) As String

    Function parseJsonString(ByVal jsonString) As List(Of Movie)

End Interface

Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports System.Text
Imports PrjMovieComponent

Public Class CMovieComponent

    Implements IMovieInterface

    'Funktsioon teeb päringu themoviedb.org vastavasse endpointi ja tagastab json formaadis filmidega seotud andmed
    Private Function getJsonString() As String Implements IMovieInterface.getJsonString

        Dim responseString As String = ""

        Try
            Dim request As HttpWebRequest = HttpWebRequest.Create("http://api.themoviedb.org/3/movie/upcoming?api_key=6a77b11f7edadaabb2c8f2b339e947a3")
            request.Proxy = Nothing 'teeb päringu kiiremaks 

            'Create the response and reader
            Dim response As HttpWebResponse = request.GetResponse
            Dim responseStream As System.IO.Stream = response.GetResponseStream

            'Create a new stream reader
            Dim streamReader As New System.IO.StreamReader(responseStream)
            responseString = streamReader.ReadToEnd
            streamReader.Close()

        Catch ex As Exception

        End Try

        Return responseString

    End Function


    'Võtab selle jsonString vastuse, teeb info juppideks ja paneb Movie objekti
    Private Function parseJsonString(jsonString As Object) As List(Of Movie) Implements IMovieInterface.parseJsonString

        'Teeme sellest jsonStringist -> Json objekti
        Dim jsonObject As JObject = JObject.Parse(jsonString)


        'Seda on vaja selleks et teada saada mitme filmi andmed server meile tagastas, seda muutujat on vaja loopimisel
        Dim jsonArray As JArray = jsonObject.GetValue("results")

        'Copy see link browserisse http://api.themoviedb.org/3/movie/upcoming?api_key=6a77b11f7edadaabb2c8f2b339e947a3 siis on paremini aru saada
        'Tee list kõigist elementidest mis selle jsonObjecti sees on
        Dim data As List(Of JToken) = jsonObject.Children().ToList

        'Teeme uue listi tüübiga Movie kuhu paneme iga filmi ja sellega seotud andmed
        Dim list As New List(Of Movie)(jsonArray.ToArray.Length)

        'Teeme readeri, mis hakkab jsonObjecti järjest läbi lugema, item on elemendi nimi
        For Each item As JProperty In data
            item.CreateReader()
            Select Case item.Name

                'Kuna vajalik info filmide kohta asub "results" arrays siis meil on just seda vaja 
                Case "results"

                    'Nüüd liikusime "results" array sisse ja hakkame iga filmi kohta eraldi andmeid välja võtma 
                    For Each movieDetail As JObject In item.Values

                        'Loome uue Movie objekti
                        Dim movie As New Movie

                        'Siin võtame filmi kohta käiva info ja paneme movie objekti
                        Dim id As String = movieDetail("id")
                        movie.id = id

                        Dim title As String = movieDetail("original_title")
                        movie.title = title

                        Dim overview As String = movieDetail("overview")
                        movie.overview = overview

                        Dim releaseDate As String = movieDetail("release_date")
                        movie.releaseDate = releaseDate

                        Dim posterPath As String = movieDetail("poster_path")
                        movie.posterPath = posterPath

                        'Lisame movie objekti movie listi
                        list.Add(movie)

                    Next 'Võtame järgmise filmi andmed ja kordame sama asja

            End Select

        Next

        Return list  'tagasta list, kus on kõikide filmide andmed sees

    End Function
End Class

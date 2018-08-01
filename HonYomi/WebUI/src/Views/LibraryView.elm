module LibraryView exposing (..)

import Array exposing (Array)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)
import Maybe exposing (withDefault)
import Messages exposing (..)
import Models exposing (..)
import ServerBook exposing (ServerBook, getCurrentTrack)


bookRow : Int -> ServerBook -> Html Msg
bookRow index book =
    let
        rowClass =
            if index % 2 == 0 then
                class "tableRow"
            else
                class "tableRowAlt"
    in
        tr [ class "book-row", rowClass ]
            [ div [ onClick <| Playback (SetTrack <| getCurrentTrack book) ] [ i [ class "fas fa-play" ] [] ]
            , div [] [ text <| withDefault book.guid book.title ]
            , div [] [ text <| book.guid ]
            , div [] [ text <| toString <| Array.length book.fileProgresses ]
            ]


bookTable : Array ServerBook -> Html Msg
bookTable books =
    div [ class "book-grid" ]
        [ div [ class "book-grid-header" ]
            [ div [ id "book-play-col-header" ] []
            , div [ id "book-title-col-header" ] [ text "Title" ]
            , div [ id "book-id-col-header" ] [ text "Id" ]
            , div [ id "book-tracks-col-header" ] [ text "Tracks" ]
            ]
        , div [ class "book-rows" ] (Array.toList <| Array.indexedMap bookRow books)
        ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    div [ id "library-books" ]
        [ libraryModel.books
            |> bookTable
        ]

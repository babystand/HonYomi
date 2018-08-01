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
        [ td [ onClick <| Playback (SetTrack <| getCurrentTrack book) ] [ i [ class "fas fa-play" ] [] ]
        , td [] [ text <| withDefault book.guid book.title ]
        , td [] [ text <| book.guid ]
        , td [] [ text <| toString <| Array.length book.fileProgresses ]
        ]


bookTable : Array ServerBook -> Html Msg
bookTable books =
    table [ class "book-table" ]
        [ thead [ class "fixed-table-header" ]
            [ tr []
                [ th [] []
                , th [] [ text "Title" ]
                , th [] [ text "Id" ]
                , th [] [ text "Tracks" ]
                ]
            ]
        , tbody [ class "scrollable-table" ] (Array.toList <| Array.indexedMap bookRow books)
        ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    div [ id "library-books" ]
        [ libraryModel.books
            |> bookTable
        ]

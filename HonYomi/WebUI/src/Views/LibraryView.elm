module LibraryView exposing (..)

import Array exposing (Array)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)
import Maybe exposing (withDefault)
import Messages exposing (..)
import Models exposing (..)
import ServerBook exposing (ServerBook, getCurrentTrack)
import ServerFile exposing (ServerFile)


setBookOnClick : ServerBook -> Html.Attribute Msg
setBookOnClick book =
    onClick <| (Library <| SetSelectedBook book)


fileRowView : ServerFile -> Html Msg
fileRowView file =
    div [] [ text "row" ]


selectedBookView : ServerBook -> Html Msg
selectedBookView book =
    div [] (Array.toList <| Array.map (\f -> fileRowView f) book.fileProgresses)


bookRow : Maybe ServerBook -> Int -> ServerBook -> Html Msg
bookRow selected index book =
    let
        rowClass =
            if index % 2 == 0 then
                class "tableRow"

            else
                class "tableRowAlt"
    in
    div [ class "book-row", rowClass ]
        [ div [ class "book-play-col", onClick <| Playback (SetTrack <| getCurrentTrack book) ] [ i [ class "fas fa-play" ] [] ]
        , div [ class "book-title-col", setBookOnClick book ] [ text <| withDefault book.guid book.title ]
        , div [ class "book-id-col", setBookOnClick book ] [ text <| book.guid ]
        , div [ class "book-tracks-col", setBookOnClick book ] [ text <| toString <| Array.length book.fileProgresses ]
        , case selected of
            Just s ->
                if s == book then
                    selectedBookView book

                else
                    text ""

            _ ->
                text ""
        ]


bookTable : Maybe ServerBook -> Array ServerBook -> Html Msg
bookTable selected books =
    div [ class "book-collection" ]
        [ div [ class "book-grid-header" ]
            [ div [ id "book-play-col-header" ] []
            , div [ id "book-title-col-header" ] [ text "Title" ]
            , div [ id "book-id-col-header" ] [ text "Id" ]
            , div [ id "book-tracks-col-header" ] [ text "Tracks" ]
            ]
        , div [ class "book-grid" ] (Array.toList <| Array.indexedMap (bookRow selected) books)
        ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    div [ id "library-books" ]
        [ libraryModel.books
            |> bookTable libraryModel.selectedBook
        ]

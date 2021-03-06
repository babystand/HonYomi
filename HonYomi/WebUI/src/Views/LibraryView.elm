module LibraryView exposing (..)

import Array exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)
import Maybe exposing (withDefault)
import Messages exposing (..)
import Models exposing (..)
import ServerBook exposing (ServerBook, getCurrentTrack)
import ServerFile exposing (ServerFile)
import Util exposing (tryHead)


setBookOnClick : ServerBook -> Html.Attribute Msg
setBookOnClick book =
    onClick <| (Library <| SetSelectedBook book)


unsetBookOnClick : Html.Attribute Msg
unsetBookOnClick =
    onClick <| Library UnsetBook


bookOnClick : String -> ServerBook -> Html.Attribute Msg
bookOnClick selectid book =
    if selectid == book.guid then
        unsetBookOnClick

    else
        setBookOnClick book


fileRowView : Int -> ServerFile -> Html Msg
fileRowView index file =
    let
        rowClass =
            if index % 2 == 0 then
                class "file-row"

            else
                class "file-row file-row-alt"
    in
    div [ rowClass ]
        [ div [ class "file-gap-col" ] []
        , div [ class "file-play-col", onClick <| Playback (SetTrackReload <| Just file) ] [ i [ class "fas fa-play" ] [] ]
        , div [ class "file-title-col" ] [ text <| withDefault "" file.title ]
        , div [ class "file-index-col" ] [ text <| toString file.trackIndex ]
        , div [ class "file-progress-col" ] [ text <| (toString <| round <| file.progressSeconds / file.duration * 100) ++ "%" ]
        ]


bookRow : String -> Int -> ServerBook -> Html Msg
bookRow selectid index book =
    let
        rowClass =
            if index % 2 == 0 then
                class "book-row"

            else
                class "book-row book-row-alt"
    in
    div
        [ rowClass
        , class <|
            if book.guid == selectid then
                "selected-book-row"

            else
                ""
        ]
        [ div [ class "book-play-col", onClick <| Playback (SetTrackReload <| getCurrentTrack book) ] [ i [ class "fas fa-play" ] [] ]
        , div [ class "book-title-col", bookOnClick selectid book ] [ text <| book.title ]
        , div [ class "book-author-col", bookOnClick selectid book ] [ text <| withDefault "" book.author ]
        , div [ class "book-tracks-col", bookOnClick selectid book ] [ text <| toString <| Array.length book.fileProgresses ]
        ]


bookTable : String -> Array ServerBook -> Html Msg
bookTable selectid books =
    div [ class "book-collection" ]
        [ div [ class "book-grid-header" ]
            [ div [ id "book-play-col-header" ] []
            , div [ id "book-title-col-header" ] [ text "Title" ]
            , div [ id "book-author-col-header" ] [ text "Author" ]
            , div [ id "book-tracks-col-header" ] [ text "Tracks" ]
            ]
        , div [ class "book-grid" ] (Array.toList <| Array.indexedMap (bookRow selectid) books)
        ]


selectedBookView : Maybe ServerBook -> Html Msg
selectedBookView mbook =
    case mbook of
        Nothing ->
            text ""

        Just book ->
            div [ id "selected-panel" ]
                [ h2 [ class "selected-title" ] [ text book.title ]
                ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    let
        book =
            getbook libraryModel.selectedBookId libraryModel.books
    in
    div []
        [ div [ id "library-books" ]
            [ libraryModel.books
                |> bookTable libraryModel.selectedBookId
            ]
        , selectedBookView book
        ]


getbook : String -> Array ServerBook -> Maybe ServerBook
getbook guid books =
    Array.filter (\x -> x.guid == guid) books
        |> tryHead

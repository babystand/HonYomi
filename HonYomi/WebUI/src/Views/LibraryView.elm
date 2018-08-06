module LibraryView exposing (..)

import Array exposing (..)
import Exts.Maybe exposing (isJust)
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
        , div [ class "file-play-col" ] [ i [ class "fas fa-play" ] [] ]
        , div [ class "file-title-col" ] [ text <| withDefault "" file.title ]
        , div [ class "file-id-col" ] [ text <| file.guid ]
        , div [ class "file-progress-col" ] [ text <| toString file.progressSeconds ]
        ]


selectedBookView : ServerBook -> Html Msg
selectedBookView book =
    div [ class "file-rows" ] <|
        [ div [ class "file-row file-rows-header" ]
            [ div [ class "file-gap-col" ] []
            , div [ class "file-play-col" ] []
            , div [ class "file-title-col" ] [ text "Title" ]
            , div [ class "file-id-col" ] [ text "Id" ]
            , div [ class "file-progress-col" ] [ text "Progress" ]
            ]
        ]
            ++ (Array.toList <|
                    Array.indexedMap fileRowView book.fileProgresses
               )


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
        , div [ class "book-title-col", bookOnClick selectid book ] [ text <| withDefault book.guid book.title ]
        , div [ class "book-id-col", bookOnClick selectid book ] [ text <| book.guid ]
        , div [ class "book-tracks-col", bookOnClick selectid book ] [ text <| toString <| Array.length book.fileProgresses ]
        , div [ class "file-rows" ] <|
            [ div [ class "file-row file-rows-header" ]
                [ div [ class "file-gap-col" ] []
                , div [ class "file-play-col" ] []
                , div [ class "file-title-col" ] [ text "Title" ]
                , div [ class "file-id-col" ] [ text "Id" ]
                , div [ class "file-progress-col" ] [ text "Progress" ]
                ]
            ]
                ++ (Array.toList <|
                        Array.indexedMap fileRowView book.fileProgresses
                   )
        ]


bookTable : String -> Array ServerBook -> Html Msg
bookTable selectid books =
    div [ class "book-collection" ]
        [ div [ class "book-grid-header" ]
            [ div [ id "book-play-col-header" ] []
            , div [ id "book-title-col-header" ] [ text "Title" ]
            , div [ id "book-id-col-header" ] [ text "Id" ]
            , div [ id "book-tracks-col-header" ] [ text "Tracks" ]
            ]
        , div [ class "book-grid" ] (Array.toList <| Array.indexedMap (bookRow selectid) books)
        ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    -- let
    --     a =
    --         Debug.log "selectedBook" libraryModel.selectedBook
    -- in
    div [ id "library-books" ]
        [ libraryModel.books
            |> bookTable libraryModel.selectedBookId
        ]

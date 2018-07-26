module LibraryView exposing (..)

import Array exposing (Array)
import Exts.Html.Events exposing (onEnter)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onInput)
import Maybe exposing (withDefault)
import Messages exposing (..)
import Models exposing (..)
import ServerBook exposing (ServerBook)
import ServerConfig exposing (WatchDir)


bookRow : ServerBook -> Html Msg
bookRow book =
    tr [ class "book-row" ]
        [ td [] [ text <| withDefault book.guid book.title ]
        , td [] [ text <| book.guid ]
        , td [] [ text <| toString <| Array.length book.fileProgresses ]
        ]


bookTable : Array ServerBook -> Html Msg
bookTable books =
    table [ class "book-table" ]
        [ thead [ class "fixed-table-header" ]
            [ tr []
                [ th [] [ text "Title" ]
                , th [] [ text "Id" ]
                , th [] [ text "Tracks" ]
                ]
            ]
        , tbody [ class "scrollable-table" ] (Array.toList <| Array.map bookRow books)
        ]


bookView : ServerBook -> Html Msg
bookView serverBook =
    div [ class "book", id serverBook.guid ]
        [ div [ class "book-row" ]
            [ span [ class "book-title" ] [ text <| withDefault serverBook.guid serverBook.title ]
            , span [ class "separator" ] [ text " | " ]
            , span [ class "book-file-count" ] [ text <| (\x -> "Tracks: " ++ x) <| toString <| Array.length serverBook.fileProgresses ]
            ]
        ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    div [ id "library-books" ]
        [ libraryModel.books
            |> bookTable
        ]

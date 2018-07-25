module LibraryView exposing (..)

import Array exposing (Array)
import Exts.Html.Events exposing (onEnter)
import Html exposing (..)
import Html.Attributes exposing (checked, for, hidden, id, placeholder, type_)
import Html.Events exposing (onClick, onInput)
import Messages exposing (..)
import Models exposing (..)
import ServerBook exposing (ServerBook)
import ServerConfig exposing (WatchDir)


bookView : ServerBook -> Html Msg
bookView serverBook =
    li [ id serverBook.guid ]
        [ div [] [ text serverBook.guid ]
        ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    div []
        [ libraryModel.books
            |> Array.map bookView
            |> Array.toList
            |> ul [ id "library-section" ]
        ]

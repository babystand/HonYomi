module ConfigView exposing (..)

import Array exposing (Array)
import Exts.Html.Events exposing (onEnter)
import Html exposing (..)
import Html.Attributes exposing (checked, for, hidden, id, placeholder, type_)
import Html.Events exposing (onClick, onInput)
import Messages exposing (..)
import Models exposing (..)
import ServerConfig exposing (WatchDir)


watchDirView : WatchDir -> Html Msg
watchDirView watchDir =
    div []
        [ span [ hidden True ] [ text watchDir.guid ]
        , span [] [ text <| "path: " ++ watchDir.path ]
        ]


configPageView : ConfigModel -> Html Msg
configPageView configModel =
    div []
        [ div [] [ text <| "scan interval: " ++ toString configModel.scanInterval ]
        , div [] [ text <| "server port: " ++ toString configModel.serverPort ]
        , label [ for "watchForChanges" ] [ text "watch for changes: " ]
        , input [ id "watchForChanges", type_ "checkbox", checked configModel.watchForChanges ] []
        , div [] (Array.toList <| Array.map watchDirView configModel.watchDirectories)
        ]

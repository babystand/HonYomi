module ConfigView exposing (..)

import Array exposing (Array)
import Exts.Html.Events exposing (onEnter)
import Html exposing (..)
import Html.Attributes exposing (..)
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
    div [ id "config-section" ]
        [ div [ class "config-values" ]
            [ table []
                [ tr [] [ td [] [ text "Scan Interval (minutes)" ], td [] [ input [ value <| toString configModel.config.scanInterval ] [] ] ]
                , tr [] [ td [] [ text "Server Port (requires restart)" ], td [] [ input [ value <| toString configModel.config.serverPort ] [] ] ]
                , tr [] [ td [] [ text "Watch for Changes" ], td [] [ input [ type_ "checkbox", checked configModel.config.watchForChanges ] [] ] ]
                ]
            ]
        , div [ class "config-dirs" ] [ div [] (Array.toList <| Array.map watchDirView configModel.config.watchDirectories) ]
        ]

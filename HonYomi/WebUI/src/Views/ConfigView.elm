module ConfigView exposing (..)

import Array exposing (Array)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onCheck, onClick, onInput)
import Messages exposing (..)
import Models exposing (..)
import ServerConfig exposing (WatchDir)


watchDirView : Int -> WatchDir -> Html Msg
watchDirView index watchDir =
    div []
        [ span [ class "config-remove", onClick <| Config (RemoveDir index) ] [ text "-" ]
        , input [ class "config-path", value watchDir.path, onInput <| \s -> Config (ModifyDir index s) ] []
        ]


configPageView : ConfigModel -> Html Msg
configPageView configModel =
    div [ id "config-section" ]
        [ div [ class "config-values" ]
            [ table []
                [ tr [] [ td [] [ text "Scan Interval (minutes)" ], td [] [ input [ onInput <| \s -> Config (SetScanInterval (String.toInt s |> Result.toMaybe |> Maybe.withDefault configModel.config.scanInterval)), value <| toString configModel.config.scanInterval ] [] ] ]
                , tr [] [ td [] [ text "Server Port (requires restart)" ], td [] [ input [ onInput <| \s -> Config (SetServerPort (String.toInt s |> Result.toMaybe |> Maybe.withDefault configModel.config.serverPort)), value <| toString configModel.config.serverPort ] [] ] ]
                , tr [] [ td [] [ text "Watch for Changes" ], td [] [ input [ type_ "checkbox", checked configModel.config.watchForChanges, onCheck <| \b -> Config (SetWatchForChanges b) ] [] ] ]
                ]
            ]
        , div [ class "username-change-section" ]
            [ h5 [] [ text "Change Username" ]
            , br [] []
            , span [] [ text "New Username" ]
            , input [ placeholder "New Username" ] []
            , button [ type_ "submit" ] [ text "Change" ]
            ]
        , div [ class "password-change-section" ]
            [ h5 [] [ text "Change Password" ]
            , br [] []
            , span [] [ text "Old Password" ]
            , input [ placeholder "Old Password" ] []
            , br [] []
            , span [] [ text "New Password" ]
            , input [ placeholder "New Password" ] []
            , button [ type_ "submit" ] [ text "Change" ]
            ]
        , div [ class "config-dirs" ]
            [ div [] [ span [ class "config-add", onClick <| Config AddDir ] [ text "+" ] ]
            , div [] (Array.toList <| Array.indexedMap watchDirView configModel.config.watchDirectories)
            ]
        , div [ class "config-buttons" ]
            [ button [ type_ "submit", class "config-submit", onClick <| Config ConfigPostRequest ] [ text "Save Changes" ]
            , button [ type_ "reset", class "config-reset", onClick <| Config ConfigGetRequest ] [ text "Undo Changes" ]
            ]
        ]

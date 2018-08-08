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
    div [ class "watch-dir" ]
        [ div [ class "config-remove", onClick <| Config (RemoveDir index) ] [ text "-" ]
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
        , div [ class "config-dirs-section" ]
            [ div [] [ span [ class "config-add", onClick <| Config AddDir ] [ text "+" ] ]
            , div [ id "config-dirs" ] (Array.toList <| Array.indexedMap watchDirView configModel.config.watchDirectories)
            ]
        , div [ class "config-buttons" ]
            [ button [ type_ "submit", class "config-submit", onClick <| Config ConfigPostRequest ] [ text "Save Changes" ]
            , button [ type_ "reset", class "config-reset", onClick <| Config ConfigGetRequest ] [ text "Undo Changes" ]
            ]
        , div [ class "change-section" ]
            [ h3 [] [ text "Change Username" ]
            , br [] []
            , input [ placeholder "New Username", onInput <| Config << SetConfigUsernameField ] []
            , br [] []
            , button [ type_ "submit", onClick <| Config ChangeUsernameRequest, disabled (configModel.username == "") ] [ text "Change" ]
            ]
        , div [ class "change-section" ]
            [ h3 [] [ text "Change Password" ]
            , br [] []
            , input [ placeholder "Old Password", type_ "password", onInput <| Config << SetConfigPasswordField ] []
            , br [] []
            , input [ placeholder "New Password", type_ "password", onInput <| Config << SetConfigNewPasswordField ] []
            , br [] []
            , button [ type_ "submit", onClick <| Config ChangePasswordRequest, disabled (configModel.password == "" || configModel.newPassword == "" || configModel.password /= configModel.newPassword) ] [ text "Change" ]
            ]
        ]

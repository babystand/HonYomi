module LoginView exposing (..)

import Exts.Html.Events exposing (onEnter)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onInput)
import Messages exposing (..)
import Models exposing (..)


loginPageView : LoginModel -> Html Msg
loginPageView loginModel =
    div [ class "page-wrapper" ]
        [ div [ class "page-body" ]
            [ div [ id "login-section" ]
                [ label [ for "username" ] [ text "Username:" ]
                , input [ id "username", placeholder "Username", onEnter (Auth LoginRequest), onInput (\x -> Auth (SetUsernameField x)) ] []
                , br [] []
                , label [ for "password" ] [ text "Password:" ]
                , input [ id "password", placeholder "Password", onEnter (Auth LoginRequest), type_ "password", onInput (\x -> Auth (SetPasswordField x)) ] []
                , br [] []
                , button [ type_ "submit", onClick (Auth LoginRequest) ] [ text "Submit" ]
                ]
            ]
        ]

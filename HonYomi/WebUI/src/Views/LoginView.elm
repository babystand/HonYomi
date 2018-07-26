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
                [ h3 [] [ text "Login" ]
                , div [ id "login-inner" ]
                    [ input [ id "username", placeholder "Username", onEnter (Auth LoginRequest), onInput (\x -> Auth (SetUsernameField x)) ] []
                    , br [] []
                    , input [ id "password", placeholder "Password", onEnter (Auth LoginRequest), type_ "password", onInput (\x -> Auth (SetPasswordField x)) ] []
                    , br [] []
                    , button [ type_ "submit", onClick (Auth LoginRequest) ] [ text "Submit" ]
                    ]
                ]
            ]
        ]

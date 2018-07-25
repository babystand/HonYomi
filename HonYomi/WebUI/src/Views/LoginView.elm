module LoginView exposing (..)

import Exts.Html.Events exposing (onEnter)
import Html exposing (..)
import Html.Attributes exposing (checked, for, hidden, id, placeholder, type_)
import Html.Events exposing (onClick, onInput)
import Messages exposing (..)
import Models exposing (..)


loginPageView : LoginModel -> Html Msg
loginPageView loginModel =
    div [ id "login-section" ]
        [ input [ id "username", placeholder "Username", onEnter (Auth LoginRequest), onInput (\x -> Auth (SetUsernameField x)) ] []
        , input [ id "password", placeholder "Password", onEnter (Auth LoginRequest), type_ "password", onInput (\x -> Auth (SetPasswordField x)) ] []
        , button [ type_ "submit", onClick (Auth LoginRequest) ] [ text "Submit" ]
        ]

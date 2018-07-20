module Main exposing (main)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import LoginPage exposing (..)


type Page
    = PageLogin LoginPage.Model
    | Home


type Model
    = Unauthenticated Page
    | Authenticated Page String


init : ( Model, Cmd Msg )
init =
    ( Unauthenticated (PageLogin LoginPage.init), Cmd.none )


type Msg
    = LoginMsg LoginPage.Msg


update : Msg -> Model -> ( Model, Cmd Msg )
update message model =
    case message of
        LoginMsg x ->
            case x of
                LoggedIn tok ->
                    Authenticated Home tok

                _ ->
                    case model of
                        Unauthenticated p ->
                            Unauthenticated ()


view : Model -> Html Msg
view model =
    case model of
        PageLogin x ->
            LoginPage.view x


main : Program Never Model Msg
main =
    Html.program
        { init = init
        , update = update
        , view = view
        , subscriptions = always Sub.none
        }

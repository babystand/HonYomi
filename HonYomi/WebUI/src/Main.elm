module Main exposing (main)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


type alias Page =
    Int


type Model
    = Unauthenticated Page
    | Authenticated Page String


init : ( Model, Cmd Msg )
init =
    ( Unauthenticated 1, Cmd.none )


type Msg
    = Start
    | Stop


update : Msg -> Model -> ( Model, Cmd Msg )
update message model =
    ( model, Cmd.none )


view : Model -> Html Msg
view model =
    div [] []


main : Program Never Model Msg
main =
    Html.program
        { init = init
        , update = update
        , view = view
        , subscriptions = always Sub.none
        }

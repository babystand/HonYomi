module LoginPage exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Json.Decode
import Http
import UserCreds


type alias Model =
    { user : UserCreds.UserCreds }


type Msg
    = ChangeUsername String
    | ChangePassword String
    | Submit
    | LoginResponse (Result Http.Error String)
    | LoggedIn String
    | LoginFailed


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ChangeUsername x ->
            let
                oldUser =
                    model.user

                newUser =
                    { oldUser | username = x }
            in
                ( { model | user = newUser }, Cmd.none )

        ChangePassword x ->
            let
                oldUser =
                    model.user

                newUser =
                    { oldUser | password = x }
            in
                ( { model | user = newUser }, Cmd.none )

        LoginResponse (Ok tok) ->
            update (LoggedIn tok) model

        LoginResponse (Err _) ->
            update LoginFailed model

        LoggedIn tok ->
            ( model, Cmd.none )

        LoginFailed ->
            ( model, Cmd.none )

        Submit ->
            ( model, submit model )


submit : Model -> Cmd Msg
submit model =
    let
        url =
            "/api/account/login"

        body =
            Http.jsonBody (UserCreds.encodeUserCreds model.user)
    in
        Http.send LoginResponse <| Http.post url body Json.Decode.string


viewForm : Html Msg
viewForm =
    Html.form [ onSubmit Submit ]
        [ div []
            [ Html.input [ type_ "text", placeholder "Username" ] []
            , Html.input [ type_ "password", placeholder "Password" ] []
            , Html.button [ class "btn", type_ "submit" ] [ text "Sign In" ]
            ]
        ]

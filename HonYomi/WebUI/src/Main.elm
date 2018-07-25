module Main exposing (main)

import ConfigView exposing (configPageView)
import Html exposing (..)
import LayoutView exposing (..)
import LibraryView exposing (libraryPageView)
import LoginView exposing (loginPageView)
import Messages exposing (..)
import Models exposing (..)
import Requests exposing (authRequest, libraryRequest, mapAuthRequest, refreshRequest)


type alias Page =
    Int


init : ( Model, Cmd Msg )
init =
    ( initMainModel, Cmd.none )


replaceToken : Model -> Token -> Model
replaceToken model token =
    case model of
        Unauthorized page ->
            model

        Authorized oldToken page ->
            Authorized token page


update : Msg -> Model -> ( Model, Cmd Msg )
update message model =
    let
        page =
            getPage model
    in
    case ( message, page ) of
        ( NoOp, _ ) ->
            ( model, Cmd.none )

        ( TriggerRefresh, _ ) ->
            ( model, refreshRequest <| getToken model )

        ( Refresh tok, _ ) ->
            ( replaceToken model tok, Cmd.none )

        ( Auth authmsg, LoginPage lpage ) ->
            case authmsg of
                SetUsernameField s ->
                    let
                        newPage =
                            { lpage | username = s }
                    in
                    ( Unauthorized <| LoginPage newPage, Cmd.none )

                Messages.SetPasswordField s ->
                    let
                        newPage =
                            { lpage | password = s }
                    in
                    ( Unauthorized <| LoginPage newPage, Cmd.none )

                Messages.LoginRequest ->
                    let
                        authResponse =
                            authRequest lpage
                    in
                    ( model, authResponse )

                Messages.LoginSuccess tok ->
                    update (Library BooksRequest) (Authorized tok <| LibraryPage initLibraryModel)

                Messages.LoginFailure _ ->
                    ( model, Cmd.none )

        ( Auth authmsg, _ ) ->
            ( model, Cmd.none )

        ( Library libmsg, LibraryPage lpage ) ->
            case libmsg of
                BooksRequest ->
                    let
                        booksResponse =
                            libraryRequest <| getToken model
                    in
                    ( model, booksResponse )

                Messages.BooksSuccess bs ->
                    let
                        newPage =
                            { lpage | books = bs }
                    in
                    ( Authorized (getToken model) <| LibraryPage newPage, Cmd.none )

                Messages.BooksError _ ->
                    ( model, Cmd.none )

        ( Library libmsg, _ ) ->
            ( model, Cmd.none )

        ( Config config, ConfigPage cpage ) ->
            ( model, Cmd.none )

        ( Config config, _ ) ->
            ( model, Cmd.none )

        ( Route route, _ ) ->
            case route of
                RouteToLibrary ->
                    update (Library BooksRequest) (Authorized (getToken model) <| LibraryPage initLibraryModel)

                RouteToConfig ->
                    update (Config ConfigGetRequest) (Authorized (getToken model) <| ConfigPage initConfigModel)


getPageView : Model -> Html Msg
getPageView model =
    case model of
        Unauthorized (LoginPage pmod) ->
            loginPageView pmod

        Authorized _ (LibraryPage pmod) ->
            libraryPageView pmod

        Authorized _ (ConfigPage cmod) ->
            configPageView cmod

        _ ->
            div [] [ text "The only way to get here is to be on a page without a view. I'm impressed " ]


view : Model -> Html Msg
view model =
    let
        page =
            getPage model
    in
    applyLayout model <| getPageView model


main : Program Never Model Msg
main =
    Html.program
        { init = init
        , update = update
        , view = view
        , subscriptions = always Sub.none
        }

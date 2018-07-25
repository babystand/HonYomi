module Main exposing (main)

import Html exposing (..)
import Html.Attributes exposing (checked, for, hidden, id, placeholder, type_)
import Html.Events exposing (onClick, onInput)
import List exposing (map)
import Messages exposing (..)
import Models exposing (..)
import Requests exposing (authRequest, libraryRequest, mapAuthRequest, refreshRequest)
import ServerBook exposing (ServerBook)
import ServerConfig exposing (WatchDir)


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
                    ( Authorized (getToken model) <| LibraryPage lpage, Cmd.none )

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


loginPageView : LoginModel -> Html Msg
loginPageView loginModel =
    div [ id "login-section" ]
        [ input [ id "username", placeholder "Username", onInput (\x -> Auth (SetUsernameField x)) ] []
        , input [ id "password", placeholder "Password", type_ "password", onInput (\x -> Auth (SetPasswordField x)) ] []
        , button [ type_ "submit", onClick (Auth LoginRequest) ] [ text "Submit" ]
        ]


bookView : ServerBook -> Html Msg
bookView serverBook =
    li [ id serverBook.guid ]
        [ div [] [ text serverBook.guid ]
        ]


libraryPageView : LibraryModel -> Html Msg
libraryPageView libraryModel =
    div []
        [ libraryModel.books
            |> List.map bookView
            |> ul [ id "library-section" ]
        ]


watchDirView : WatchDir -> Html Msg
watchDirView watchDir =
    div []
        [ span [ hidden True ] [ text watchDir.guid ]
        , span [] [ text <| "path: " ++ watchDir.path ]
        ]


configPageView : ConfigModel -> Html Msg
configPageView configModel =
    div []
        [ div [] [ text <| "scan interval: " ++ toString configModel.scanInterval ]
        , div [] [ text <| "server port: " ++ toString configModel.serverPort ]
        , label [ for "watchForChanges" ] [ text "watch for changes: " ]
        , input [ id "watchForChanges", type_ "checkbox", checked configModel.watchForChanges ] []
        , div [] (List.map watchDirView configModel.watchDirectories)
        ]


navLayoutView : String -> Html Msg -> Html Msg
navLayoutView pagename child =
    div []
        [ div []
            [ a [ onClick <| Route RouteToLibrary ] [ text " >>go to library<< " ]
            , a [ onClick <| Route RouteToConfig ] [ text " >>go to config<< " ]
            ]
        , br [] []
        , h2 [] [ text pagename ]
        , br [] []
        , div [] [ child ]
        ]


view : Model -> Html Msg
view model =
    let
        page =
            getPage model
    in
    case page of
        LoginPage pmod ->
            loginPageView pmod

        LibraryPage pmod ->
            navLayoutView "library" <| libraryPageView pmod

        ConfigPage cmod ->
            navLayoutView "config" <| configPageView cmod


main : Program Never Model Msg
main =
    Html.program
        { init = init
        , update = update
        , view = view
        , subscriptions = always Sub.none
        }

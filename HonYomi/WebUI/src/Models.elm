{- Page Models -}


module Models exposing (..)

import ServerBook
import ServerConfig exposing (ServerConfig)
import UserCreds exposing (UserCreds)


type alias Token =
    String


type alias LoginModel =
    UserCreds


type alias LibraryModel =
    { books : ServerBook.Library
    , selectedBook : Maybe ServerBook.ServerBook
    }


type alias ConfigModel =
    ServerConfig


type Page
    = LoginPage LoginModel
    | LibraryPage LibraryModel
    | ConfigPage ConfigModel


type Model
    = Unauthorized Page
    | Authorized Token Page


initLoginModel : LoginModel
initLoginModel =
    { username = "", password = "" }


initLibraryModel : LibraryModel
initLibraryModel =
    { books = [], selectedBook = Nothing }


initConfigModel : ConfigModel
initConfigModel =
    { watchForChanges = True, scanInterval = 59, serverPort = 5000, watchDirectories = [] }


initMainModel : Model
initMainModel =
    Unauthorized <| LoginPage initLoginModel


getPage : Model -> Page
getPage model =
    case model of
        Unauthorized page ->
            page

        Authorized _ page ->
            page


getToken : Model -> Token
getToken model =
    case model of
        Unauthorized _ ->
            ""

        Authorized token _ ->
            token

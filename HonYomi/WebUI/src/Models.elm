{- Page Models -}


module Models exposing (..)

import ServerBook
import UserCreds exposing (UserCreds)


type alias Token =
    String


type alias LoginModel =
    UserCreds


type alias LibraryModel =
    { books : ServerBook.Library
    , selectedBook : Maybe ServerBook.ServerBook
    }


type Page
    = LoginPage LoginModel
    | LibraryPage LibraryModel


type Model
    = Unauthorized Page
    | Authorized Token Page


initLoginModel : LoginModel
initLoginModel =
    { username = "", password = "" }


initLibraryModel : LibraryModel
initLibraryModel =
    { books = [], selectedBook = Nothing }


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

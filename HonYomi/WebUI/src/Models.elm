{- Page Models -}


module Models exposing (..)

import ServerBook


type alias Token =
    String


type alias LoginModel =
    { username : String
    , password : String
    }


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

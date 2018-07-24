{- Msg types for entire site -}


module Messages exposing (..)

import Models
import ServerBook as Book
import UserCreds as User


type AuthMsg
    = SetUsernameField String
    | SetPasswordField String
    | LoginRequest
    | LoginSuccess Models.Token
    | LoginFailure String


type LibraryMsg
    = BooksRequest
    | BooksSuccess Book.Library
    | BooksError String


type Msg
    = NoOp
    | Auth AuthMsg
    | Library LibraryMsg

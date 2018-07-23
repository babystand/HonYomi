{- Msg types for entire site -}


module Messages exposing (..)

import UserCreds as User
import ServerBook as Book
import Models


type AuthMsg
    = SetUsernameField String
    | SetPasswordField String
    | LoginRequest User.UserCreds
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

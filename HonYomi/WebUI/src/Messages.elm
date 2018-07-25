{- Msg types for entire site -}


module Messages exposing (..)

import Models
import ServerBook as Book
import ServerConfig exposing (ServerConfig)


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


type ConfigMsg
    = ConfigGetRequest
    | ConfigPostRequest
    | ConfigSuccess ServerConfig
    | ConfigError String
    | ToggleWatchForChanges
    | SetScanInterval Int
    | SetServerPort Int


type RouteMsg
    = RouteToLibrary
    | RouteToConfig


type Msg
    = NoOp
    | TriggerRefresh
    | Refresh Models.Token
    | Auth AuthMsg
    | Library LibraryMsg
    | Config ConfigMsg
    | Route RouteMsg

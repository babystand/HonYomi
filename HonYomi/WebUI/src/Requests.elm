{- Http endpoint functions -}


module Requests exposing (..)

import Http
import Json.Decode
import Jwt
import Messages as M
import Models
import Result
import ServerBook
import UserCreds as User


mapAuthRequest : Result Http.Error String -> M.Msg
mapAuthRequest result =
    case result of
        Ok s ->
            M.Auth <| M.LoginSuccess s

        Err err ->
            case err of
                Http.Timeout ->
                    M.Auth <| M.LoginFailure "Network Error"

                Http.NetworkError ->
                    M.Auth <| M.LoginFailure "Network Error"

                Http.BadPayload m r ->
                    M.Auth <| M.LoginFailure m

                Http.BadStatus s ->
                    M.Auth <| M.LoginFailure s.status.message

                Http.BadUrl _ ->
                    M.Auth <| M.LoginFailure "Bad URL"


authRequest : User.UserCreds -> Cmd M.Msg
authRequest user =
    Http.send mapAuthRequest <| Http.getString "/api/auth/login"


mapLibraryRequest : Result Http.Error ServerBook.Library -> M.Msg
mapLibraryRequest result =
    case result of
        Ok lib ->
            M.Library <| M.BooksSuccess lib

        Err err ->
            case err of
                Http.Timeout ->
                    M.Library <| M.BooksError "Network Error"

                Http.NetworkError ->
                    M.Library <| M.BooksError "Network Error"

                Http.BadPayload m r ->
                    M.Library <| M.BooksError m

                Http.BadStatus s ->
                    M.Library <| M.BooksError s.status.message

                Http.BadUrl _ ->
                    M.Library <| M.BooksError "Bad URL"


libraryRequest : Models.Token -> Cmd M.Msg
libraryRequest tok =
    Http.send mapLibraryRequest <|
        Jwt.get tok "/api/books/list" <|
            Json.Decode.list ServerBook.decodeServerBook

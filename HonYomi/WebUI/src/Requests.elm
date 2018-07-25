{- Http endpoint functions -}


module Requests exposing (..)

import Http exposing (jsonBody)
import Json.Decode
import JsonToken exposing (..)
import Jwt
import Messages as M
import Models
import Result
import ServerBook
import ServerConfig
import UserCreds as User exposing (encodeUserCreds)


mapAuthRequest : Result Http.Error JsonToken -> M.Msg
mapAuthRequest result =
    M.Auth <|
        case Debug.log "result" result of
            Ok s ->
                M.LoginSuccess s.token

            Err err ->
                case err of
                    Http.Timeout ->
                        M.LoginFailure "Network Error"

                    Http.NetworkError ->
                        M.LoginFailure "Network Error"

                    Http.BadPayload m r ->
                        M.LoginFailure m

                    Http.BadStatus s ->
                        M.LoginFailure s.status.message

                    Http.BadUrl _ ->
                        M.LoginFailure "Bad URL"


authRequest : User.UserCreds -> Cmd M.Msg
authRequest user =
    Http.send mapAuthRequest <| Http.post "/api/auth/login" (Http.jsonBody <| encodeUserCreds user) decodeJsonToken


mapRefreshRequest : Result Http.Error JsonToken -> M.Msg
mapRefreshRequest result =
    case Debug.log "result" result of
        Ok s ->
            M.Refresh s.token

        Err err ->
            case err of
                Http.Timeout ->
                    M.NoOp

                Http.NetworkError ->
                    M.NoOp

                Http.BadPayload m r ->
                    M.NoOp

                Http.BadStatus s ->
                    M.NoOp

                Http.BadUrl _ ->
                    M.NoOp


refreshRequest : Models.Token -> Cmd M.Msg
refreshRequest tok =
    Http.send mapRefreshRequest <| Jwt.get tok "/api/auth/refresh" decodeJsonToken


mapLibraryRequest : Result Http.Error ServerBook.Library -> M.Msg
mapLibraryRequest result =
    M.Library <|
        case result of
            Ok lib ->
                M.BooksSuccess lib

            Err err ->
                case err of
                    Http.Timeout ->
                        M.BooksError "Network Error"

                    Http.NetworkError ->
                        M.BooksError "Network Error"

                    Http.BadPayload m r ->
                        M.BooksError m

                    Http.BadStatus s ->
                        M.BooksError s.status.message

                    Http.BadUrl _ ->
                        M.BooksError "Bad URL"


libraryRequest : Models.Token -> Cmd M.Msg
libraryRequest tok =
    Http.send mapLibraryRequest <|
        Jwt.get tok "/api/books/list/fake" <|
            Json.Decode.list ServerBook.decodeServerBook


mapConfigRequest : Result Http.Error ServerConfig.ServerConfig -> M.Msg
mapConfigRequest result =
    M.Config <|
        case result of
            Ok config ->
                M.ConfigSuccess config

            Err err ->
                case err of
                    Http.Timeout ->
                        M.ConfigError "Network Error"

                    Http.NetworkError ->
                        M.ConfigError "Network Error"

                    Http.BadPayload m r ->
                        M.ConfigError m

                    Http.BadStatus s ->
                        M.ConfigError s.status.message

                    Http.BadUrl _ ->
                        M.ConfigError "Bad URL"


configGetRequest : Models.Token -> Cmd M.Msg
configGetRequest tok =
    Http.send mapConfigRequest <|
        Jwt.get tok "/api/db/config" <|
            ServerConfig.decodeServerConfig


configPostRequest : Models.Token -> ServerConfig.ServerConfig -> Cmd M.Msg
configPostRequest tok config =
    Http.send mapConfigRequest <|
        Jwt.post tok "/api/db/config" (jsonBody <| ServerConfig.encodeServerConfig config) ServerConfig.decodeServerConfig

{- Http endpoint functions -}


module Requests exposing (..)

import Http exposing (jsonBody)
import Json.Decode
import JsonToken exposing (..)
import Jwt
import Messages as M
import Models exposing (PlaybackModel)
import Result
import ServerBook
import ServerConfig
import ServerFile exposing (..)
import UserCreds as User exposing (encodeUserCreds)


mapAuthRequest : Result Http.Error JsonToken -> M.Msg
mapAuthRequest result =
    M.Auth <|
        case result of
            Ok s ->
                M.LoginSuccess s.token

            Err err ->
                case Debug.log "network error: " err of
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
    case result of
        Ok s ->
            M.Refresh s.token

        Err err ->
            case Debug.log "network error: " err of
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
                case Debug.log "network error: " err of
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
        Jwt.get tok "/api/books/list" <|
            Json.Decode.array ServerBook.decodeServerBook


mapConfigRequest : Result Http.Error ServerConfig.ServerConfig -> M.Msg
mapConfigRequest result =
    M.Config <|
        case result of
            Ok config ->
                M.ConfigSuccess config

            Err err ->
                case Debug.log "network error: " err of
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


mapProgressRequest : Result Http.Error a -> M.Msg
mapProgressRequest result =
    case result of
        Ok _ ->
            M.Playback M.SaveTrackSuccess

        Err _ ->
            M.Playback M.SaveTrackError


setProgressRequest : Models.Token -> PlaybackModel -> Cmd M.Msg
setProgressRequest tok model =
    Http.send mapProgressRequest <|
        Jwt.get tok ("/api/tracks/progress/set/" ++ model.guid ++ "/" ++ toString model.currentTime) <|
            Json.Decode.bool


mapProgressBookRequest : Result Http.Error ServerFile -> M.Msg
mapProgressBookRequest result =
    case result of
        Ok file ->
            M.Playback <| M.ProgressBookSuccess file

        Err _ ->
            M.Playback <| M.ProgressBookError


progressBookRequest : Models.Token -> String -> Cmd M.Msg
progressBookRequest tok trackid =
    Http.send mapProgressBookRequest <|
        Jwt.get tok ("/api/books/settrack/" ++ trackid) <|
            decodeServerFile

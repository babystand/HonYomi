{- Page Models -}


module Models exposing (..)

import Array
import Base64
import Exts.Array
import Maybe exposing (withDefault)
import ServerBook
import ServerConfig exposing (ServerConfig)
import ServerFile exposing (..)
import UserCreds exposing (UserCreds)


type alias Token =
    String


type alias LoginModel =
    UserCreds


type alias PlaybackModel =
    { guid : String
    , title : String
    , bookTitle : String
    , trackIndex : Int
    , url : String
    , mediaType : String
    , currentTime : Float
    , duration : Float
    , ended : Bool
    }


type alias LibraryModel =
    { books : ServerBook.Library
    , selectedBook : Maybe ServerBook.ServerBook
    }


type alias ConfigModel =
    { config : ServerConfig
    }


type Page
    = LoginPage LoginModel
    | LibraryPage LibraryModel
    | ConfigPage ConfigModel


type alias Model =
    { token : Token
    , page : Page
    , playback : Maybe PlaybackModel
    }


initLoginModel : LoginModel
initLoginModel =
    { username = "", password = "" }


initLibraryModel : LibraryModel
initLibraryModel =
    { books = Array.empty, selectedBook = Nothing }


initConfigModel : ConfigModel
initConfigModel =
    { config = { watchForChanges = True, scanInterval = 59, serverPort = 5000, watchDirectories = Array.empty } }


initMainModel : Model
initMainModel =
    { token = "", page = LoginPage initLoginModel, playback = Nothing }


setSelectedBook : LibraryModel -> ServerBook.ServerBook -> LibraryModel
setSelectedBook model book =
    { model | selectedBook = Just book }


unsetSelectedBook : LibraryModel -> LibraryModel
unsetSelectedBook model =
    { model | selectedBook = Nothing }


getPlayback : Model -> PlaybackModel
getPlayback model =
    model.playback |> withDefault { guid = "", title = "", bookTitle = "", trackIndex = 0, url = "", mediaType = "", currentTime = 0.0, duration = 0.0, ended = False }


setPlayback : ServerFile -> Token -> PlaybackModel -> PlaybackModel
setPlayback file tok model =
    { guid = file.guid, title = withDefault "" file.title, bookTitle = withDefault "" file.bookTitle, trackIndex = file.trackIndex, url = "/api/tracks/stream/" ++ encodeTrackUrl tok file.guid, mediaType = file.mediaType, currentTime = file.progressSeconds, duration = 0.0, ended = False }


removeWatchDirectory : Int -> ConfigModel -> ConfigModel
removeWatchDirectory index model =
    let
        conf =
            model.config

        newDirs =
            Exts.Array.delete index model.config.watchDirectories

        newconf =
            { conf | watchDirectories = newDirs }
    in
    { config = newconf }


addWatchDirectory : ConfigModel -> ConfigModel
addWatchDirectory model =
    let
        conf =
            model.config

        newDirs =
            Array.push { guid = "", path = "" } model.config.watchDirectories

        newconf =
            { conf | watchDirectories = newDirs }
    in
    { config = newconf }


modifyWatchDirectory : Int -> String -> ConfigModel -> ConfigModel
modifyWatchDirectory index newPath model =
    let
        old =
            Array.get index model.config.watchDirectories |> withDefault { guid = "", path = "" }

        new =
            { old | path = newPath }

        conf =
            model.config

        newconf =
            { conf | watchDirectories = model.config.watchDirectories |> Array.set index new }
    in
    { model | config = newconf }


encodeTrackUrl : Token -> String -> String
encodeTrackUrl tok guid =
    guid
        ++ ":"
        ++ tok
        |> Base64.encode

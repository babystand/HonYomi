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
    , bookId : String
    , trackIndex : Int
    , url : String
    , mediaType : String
    , savedTime : Float
    , currentTime : Float
    , duration : Float
    , ended : Bool
    , isPlaying : Bool
    , next : String
    }


type alias LibraryModel =
    { books : ServerBook.Library
    , selectedBookId : String
    }


type alias ConfigModel =
    { config : ServerConfig
    , username : String
    , password : String
    , newPassword : String
    , newPassword2 : String
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
    { username = "", password = "", newPassword = "" }


initLibraryModel : LibraryModel
initLibraryModel =
    { books = Array.empty, selectedBookId = "" }


initConfigModel : ConfigModel
initConfigModel =
    { config = { watchForChanges = True, scanInterval = 59, serverPort = 5000, watchDirectories = Array.empty }, username = "", password = "", newPassword = "", newPassword2 = "" }


initMainModel : Model
initMainModel =
    { token = "", page = LoginPage initLoginModel, playback = Nothing }


setSelectedBook : LibraryModel -> ServerBook.ServerBook -> LibraryModel
setSelectedBook model book =
    { model | selectedBookId = book.guid }


unsetSelectedBook : LibraryModel -> LibraryModel
unsetSelectedBook model =
    { model | selectedBookId = "" }


getPlayback : Model -> PlaybackModel
getPlayback model =
    model.playback |> withDefault { guid = "", title = "", bookTitle = "", bookId = "", trackIndex = 0, url = "", mediaType = "", savedTime = 0, currentTime = 0.0, duration = 0.0, ended = False, isPlaying = False, next = "" }


setPlayback : ServerFile -> Token -> PlaybackModel -> PlaybackModel
setPlayback file tok model =
    Debug.log "setp"
        { guid = file.guid, title = withDefault "" file.title, bookTitle = withDefault "" file.bookTitle, bookId = file.bookGuid, trackIndex = file.trackIndex, url = "/api/tracks/stream/" ++ encodeTrackUrl tok file.guid, mediaType = file.mediaType, savedTime = file.progressSeconds, currentTime = file.progressSeconds, duration = file.duration, ended = False, isPlaying = False, next = file.nextFile }


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
    { model | config = newconf }


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
    { model | config = newconf }


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

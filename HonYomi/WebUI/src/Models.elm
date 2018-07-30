{- Page Models -}


module Models exposing (..)

import Array
import Exts.Array
import Maybe exposing (withDefault)
import ServerBook
import ServerConfig exposing (ServerConfig)
import ServerFile exposing (ServerFile)
import UserCreds exposing (UserCreds)


type alias Token =
    String


type alias LoginModel =
    UserCreds


type alias PlaybackModel =
    { reservation : Maybe ( ServerFile, ServerFile.FileReservation )
    , position : Float
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
    , playback : PlaybackModel
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


initPlaybackModel : PlaybackModel
initPlaybackModel =
    { reservation = Nothing, ended = False, position = 0 }


initMainModel : Model
initMainModel =
    { token = "", page = LoginPage initLoginModel, playback = initPlaybackModel }


loadTrack : Model -> ( ServerFile, ServerFile.FileReservation ) -> Model
loadTrack model serverFile =
    let
        oldplayback =
            model.playback

        newplayback =
            { oldplayback | reservation = Just serverFile }
    in
        { model | playback = newplayback }


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

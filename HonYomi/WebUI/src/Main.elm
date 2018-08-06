module Main exposing (main)

import ConfigView exposing (configPageView)
import Html exposing (..)
import LayoutView exposing (..)
import LibraryView exposing (libraryPageView)
import LoginView exposing (loginPageView)
import Maybe exposing (withDefault)
import Messages exposing (..)
import Models exposing (..)
import Ports exposing (..)
import Requests exposing (..)


type alias Page =
    Int


init : ( Model, Cmd Msg )
init =
    ( initMainModel, Cmd.none )


replaceToken : Model -> Token -> Model
replaceToken model token =
    { model | token = token }


updateLoginPage : Model -> AuthMsg -> LoginModel -> ( Model, Cmd Msg )
updateLoginPage model authmsg lpage =
    case authmsg of
        SetUsernameField s ->
            let
                newPage =
                    { lpage | username = s }
            in
            ( { model | page = LoginPage newPage }, Cmd.none )

        Messages.SetPasswordField s ->
            let
                newPage =
                    { lpage | password = s }
            in
            ( { model | page = LoginPage newPage }, Cmd.none )

        Messages.LoginRequest ->
            let
                authResponse =
                    authRequest lpage
            in
            ( model, authResponse )

        Messages.LoginSuccess tok ->
            update (Library BooksRequest) { model | token = tok, page = LibraryPage initLibraryModel }

        Messages.LoginFailure _ ->
            ( model, Cmd.none )


updateLibraryPage : Model -> LibraryMsg -> LibraryModel -> ( Model, Cmd Msg )
updateLibraryPage model libmsg lpage =
    case libmsg of
        BooksRequest ->
            let
                booksResponse =
                    libraryRequest model.token
            in
            ( model, booksResponse )

        Messages.BooksSuccess bs ->
            let
                newPage =
                    { lpage | books = bs }

                newModel =
                    { model | page = LibraryPage newPage }
            in
            case lpage.selectedBook of
                Just book ->
                    update (Library <| SetSelectedBook book) newModel

                Nothing ->
                    ( newModel, Cmd.none )

        BooksError _ ->
            ( model, Cmd.none )

        SetSelectedBook book ->
            ( { model | page = LibraryPage <| setSelectedBook lpage book }, Cmd.none )

        UnsetBook ->
            ( { model | page = LibraryPage <| unsetSelectedBook lpage }, Cmd.none )


updateConfigPage : Model -> ConfigMsg -> ConfigModel -> ( Model, Cmd Msg )
updateConfigPage model config cpage =
    case config of
        ConfigGetRequest ->
            ( model, configGetRequest <| model.token )

        ConfigPostRequest ->
            ( model, configPostRequest model.token cpage.config )

        ConfigSuccess conf ->
            let
                newPage =
                    ConfigPage { cpage | config = conf }
            in
            ( { model | page = newPage }, Cmd.none )

        ConfigError _ ->
            ( model, Cmd.none )

        SetWatchForChanges b ->
            let
                oldconf =
                    cpage.config

                conf =
                    { oldconf | watchForChanges = b }

                newPage =
                    ConfigPage { cpage | config = conf }
            in
            ( { model | page = newPage }, Cmd.none )

        SetScanInterval i ->
            let
                oldconf =
                    cpage.config

                conf =
                    { oldconf | scanInterval = i }

                newPage =
                    ConfigPage { cpage | config = conf }
            in
            ( { model | page = newPage }, Cmd.none )

        SetServerPort p ->
            let
                oldconf =
                    cpage.config

                conf =
                    { oldconf | serverPort = p }

                newPage =
                    ConfigPage { cpage | config = conf }
            in
            ( { model | page = newPage }, Cmd.none )

        AddDir ->
            let
                newPage =
                    addWatchDirectory cpage
            in
            ( { model | page = ConfigPage newPage }, Cmd.none )

        RemoveDir i ->
            let
                newPage =
                    removeWatchDirectory i cpage
            in
            ( { model | page = ConfigPage newPage }, Cmd.none )

        ModifyDir i s ->
            let
                newPage =
                    modifyWatchDirectory i s cpage
            in
            ( { model | page = ConfigPage newPage }, Cmd.none )


updatePlayback : Model -> PlaybackMsg -> ( Model, Cmd Msg )
updatePlayback model msg =
    let
        pmod =
            getPlayback model
    in
    case msg of
        SetTrackReload mfile ->
            case mfile of
                Just file ->
                    let
                        newPlayback =
                            setPlayback file model.token pmod
                    in
                    updatePlayback { model | playback = Just <| newPlayback } (LoadTrack (Debug.log "urlis" newPlayback.url))

                Nothing ->
                    ( model, Cmd.none )

        SetTrackNoReload mfile ->
            case mfile of
                Just file ->
                    update (Library BooksRequest) { model | playback = Just <| setPlayback file model.token pmod }

                Nothing ->
                    ( model, Cmd.none )

        LoadTrack url ->
            ( model, setAudioSource (Debug.log "urlisis" url) )

        AudioLoaded ->
            let
                newTime =
                    case model.playback of
                        Just p ->
                            p.savedTime

                        Nothing ->
                            0
            in
            ( model, setCurrentTime newTime )

        ProgressChanged progress ->
            let
                newPlayback =
                    Just <| { pmod | currentTime = progress }

                newModel =
                    { model | playback = newPlayback }
            in
            if abs (progress - pmod.currentTime) < 1 && abs (progress - pmod.savedTime) > 5 then
                updatePlayback newModel SaveTrackPosition

            else
                ( newModel, Cmd.none )

        DurationChanged dur ->
            let
                newPlayback =
                    Just <| { pmod | duration = dur }
            in
            ( { model | playback = newPlayback }, Cmd.none )

        Ended ->
            let
                ( m, c ) =
                    updatePlayback model Pause

                newPlayback =
                    Just <| { pmod | ended = True }
            in
            updatePlayback { m | playback = newPlayback } NextTrackRequest

        Play ->
            ( model, playAudio () )

        Pause ->
            ( model, pauseAudio () )

        Played ->
            let
                newPlayback =
                    Just <| { pmod | isPlaying = True }
            in
            ( { model | playback = newPlayback }, Cmd.none )

        Paused ->
            let
                newPlayback =
                    Just <| { pmod | isPlaying = False }
            in
            ( { model | playback = newPlayback }, Cmd.none )

        ScrubTo f ->
            updatePlayback model (SetCurrentTime <| f * pmod.duration)

        SetCurrentTime f ->
            ( model, setCurrentTime f )

        SaveTrackPosition ->
            ( model, setProgressRequest model.token pmod )

        SaveTrackError ->
            ( model, Cmd.none )

        SaveTrackSuccess ->
            update (Library BooksRequest) model

        SetBookProgress trackid ->
            ( model, progressBookRequest model.token trackid )

        ProgressBookSuccess file ->
            updatePlayback model (SetTrackNoReload <| Just file)

        ProgressBookError ->
            ( model, Cmd.none )

        NextTrackRequest ->
            ( model, nextTrackRequest model.token pmod.next )

        NextTrackSuccess file ->
            updatePlayback model (SetTrackReload (Just file))

        NextTrackError ->
            ( model, Cmd.none )


update : Msg -> Model -> ( Model, Cmd Msg )
update message model =
    case ( message, model.page ) of
        ( NoOp, _ ) ->
            ( model, Cmd.none )

        ( TriggerRefresh, _ ) ->
            ( model, refreshRequest model.token )

        ( Refresh tok, _ ) ->
            ( replaceToken model tok, Cmd.none )

        ( Auth authmsg, LoginPage lpage ) ->
            updateLoginPage model authmsg lpage

        ( Auth authmsg, _ ) ->
            ( model, Cmd.none )

        ( Library libmsg, LibraryPage lpage ) ->
            updateLibraryPage model libmsg lpage

        ( Library libmsg, _ ) ->
            ( model, Cmd.none )

        ( Config config, ConfigPage cpage ) ->
            updateConfigPage model config cpage

        ( Config config, _ ) ->
            ( model, Cmd.none )

        ( Playback _, LoginPage _ ) ->
            ( model, Cmd.none )

        ( Playback pmsg, _ ) ->
            updatePlayback model pmsg

        ( Route route, _ ) ->
            case route of
                RouteToLibrary ->
                    update (Library BooksRequest) { model | page = LibraryPage initLibraryModel }

                RouteToConfig ->
                    update (Config ConfigGetRequest) { model | page = ConfigPage initConfigModel }


getPageView : Model -> Html Msg
getPageView model =
    case model.page of
        LoginPage pmod ->
            loginPageView pmod

        LibraryPage pmod ->
            libraryPageView pmod

        ConfigPage cmod ->
            configPageView cmod


view : Model -> Html Msg
view model =
    applyLayout model <| getPageView model


subscriptions : Model -> Sub Msg
subscriptions model =
    Sub.batch
        [ audioProgress (Playback << ProgressChanged)
        , audioLoaded (\_ -> Playback AudioLoaded)
        , durationChange (Playback << DurationChanged)
        , onEnded (\_ -> Playback Ended)
        , onPlayed (\_ -> Playback Played)
        , onPaused (\_ -> Playback Paused)
        , onScrub (Playback << ScrubTo)

        -- , Time.every Time.second <| \_ -> Playback UpdatePostion
        ]


main : Program Never Model Msg
main =
    Html.program
        { init = init
        , update = update
        , view = view
        , subscriptions = subscriptions
        }

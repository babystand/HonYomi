port module Ports exposing (..)


port setAudioSource : String -> Cmd msg


port audioLoaded : (() -> msg) -> Sub msg


port audioProgress : (Float -> msg) -> Sub msg


port durationChange : (Float -> msg) -> Sub msg


port onScrub : (Float -> msg) -> Sub msg


port onEnded : (() -> msg) -> Sub msg


port onPlayed : (() -> msg) -> Sub msg


port onPaused : (() -> msg) -> Sub msg


port playAudio : () -> Cmd msg


port pauseAudio : () -> Cmd msg


port setCurrentTime : Float -> Cmd msg

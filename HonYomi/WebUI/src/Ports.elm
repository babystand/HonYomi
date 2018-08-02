port module Ports exposing (..)


port loadAudioSource : () -> Cmd msg


port audioProgress : (Float -> msg) -> Sub msg


port durationChange : (Float -> msg) -> Sub msg


port onEnded : (() -> msg) -> Sub msg


port onPlayed : (() -> msg) -> Sub msg


port onPaused : (() -> msg) -> Sub msg


port playAudio : () -> Cmd msg


port pauseAudio : () -> Cmd msg

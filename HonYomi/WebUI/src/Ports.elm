port module Ports exposing (..)


port loadAudioSource : () -> Cmd msg


port audioProgress : (Float -> msg) -> Sub msg


port onEnded : (() -> msg) -> Sub msg

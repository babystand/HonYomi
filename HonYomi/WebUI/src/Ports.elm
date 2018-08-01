port module Ports exposing (..)


port loadAudioSource : () -> Cmd msg


port getAudioProgress : () -> Cmd msg


port audioProgress : (Float -> msg) -> Sub msg

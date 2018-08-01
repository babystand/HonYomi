module PlaybackView exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Messages exposing (..)
import Models exposing (..)


playbackView : PlaybackModel -> Html Msg
playbackView model =
    div [ id "playback-section" ]
        [ div [ id "player" ]
            [ audio [ id "audio", controls True ]
                [ source [ id "audio-source", src model.url ] []
                , text "This browser cannot play audio"
                ]
            ]
        ]

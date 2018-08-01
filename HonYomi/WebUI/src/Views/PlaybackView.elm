module PlaybackView exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Messages exposing (..)
import Models exposing (..)


playbackView : PlaybackModel -> Html Msg
playbackView model =
    div [ id "playback-section" ]
        [ div [ id "playback-info" ]
            [ span [ id "playback-book" ] [ text model.bookTitle ]
            , span [ class "playback-separator" ] [ text " - " ]
            , span [ id "playback-track" ] [ text <| "Track " ++ toString model.trackIndex ]
            , span [ class "playback-separator" ] [ text " - " ]
            , span [ id "playback-track-title" ] [ text model.title ]
            ]
        , div [ id "player" ]
            [ audio [ id "audio", controls True ]
                [ source [ id "audio-source", src model.url ] []
                , text "This browser cannot play audio"
                ]
            ]
        ]

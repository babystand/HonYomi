module PlaybackView exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)
import Messages exposing (..)
import Models exposing (..)


toggleMsg : Bool -> Msg
toggleMsg bool =
    case bool of
        True ->
            Playback Pause

        False ->
            Playback Play


toggleIconClass : Bool -> String
toggleIconClass bool =
    case bool of
        True ->
            "fas fa-pause"

        False ->
            "fas fa-play"


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
            [ div [ id "player-controls" ]
                [ span [ onClick <| toggleMsg model.isPlaying ] [ i [ class <| toggleIconClass model.isPlaying ] [] ]
                ]
            , div [ class "player-progress" ]
                [ div [ id "progress-bar" ]
                    [ div [ id "progress-value", style [ ( "width", (toString <| 100 * model.currentTime / model.duration) ++ "%" ) ] ] []
                    ]
                ]
            , audio [ id "audio" ]
                [ source [ id "audio-source", src model.url ] []
                , text "This browser cannot play audio"
                ]
            ]
        ]

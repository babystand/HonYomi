module ServerBook exposing (..)

import Array exposing (Array)
import Json.Decode
import Json.Decode.Pipeline
import ServerFile


type alias Library =
    Array ServerBook


type alias ServerBook =
    { guid : String
    , currentTrackGuid : String
    , title : Maybe String
    , fileProgresses : Array ServerFile.ServerFile
    }


decodeServerBook : Json.Decode.Decoder ServerBook
decodeServerBook =
    Json.Decode.Pipeline.decode ServerBook
        |> Json.Decode.Pipeline.required "guid" Json.Decode.string
        |> Json.Decode.Pipeline.required "currentTrackGuid" Json.Decode.string
        |> Json.Decode.Pipeline.required "title" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "fileProgresses" (Json.Decode.array ServerFile.decodeServerFile)

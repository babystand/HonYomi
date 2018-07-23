module ServerBook exposing (..)

import Json.Decode
import ServerFile
import Json.Decode.Pipeline


type alias Library =
    List ServerBook


type alias ServerBook =
    { guid : String
    , currentTrackGuid : String
    , title : Maybe String
    , fileProgresses : List ServerFile.ServerFile
    }


decodeServerBook : Json.Decode.Decoder ServerBook
decodeServerBook =
    Json.Decode.Pipeline.decode ServerBook
        |> Json.Decode.Pipeline.required "guid" (Json.Decode.string)
        |> Json.Decode.Pipeline.required "currentTrackGuid" (Json.Decode.string)
        |> Json.Decode.Pipeline.required "title" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "fileProgresses" (Json.Decode.list ServerFile.decodeServerFile)

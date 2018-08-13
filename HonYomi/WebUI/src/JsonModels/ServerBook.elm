module ServerBook exposing (..)

import Array exposing (Array)
import Json.Decode
import Json.Decode.Pipeline
import ServerFile exposing (..)


type alias Library =
    Array ServerBook


type alias ServerBook =
    { guid : String
    , currentTrackGuid : String
    , title : String
    , author : Maybe String
    , isbn : Maybe String
    , fileProgresses : Array ServerFile.ServerFile
    }


getCurrentTrack : ServerBook -> Maybe ServerFile
getCurrentTrack book =
    Array.filter (\f -> f.guid == book.currentTrackGuid) book.fileProgresses
        |> Array.get 0


decodeServerBook : Json.Decode.Decoder ServerBook
decodeServerBook =
    Json.Decode.Pipeline.decode ServerBook
        |> Json.Decode.Pipeline.required "guid" Json.Decode.string
        |> Json.Decode.Pipeline.required "currentTrackGuid" Json.Decode.string
        |> Json.Decode.Pipeline.required "title" Json.Decode.string
        |> Json.Decode.Pipeline.required "author" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "isbn" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "fileProgresses" (Json.Decode.array ServerFile.decodeServerFile)

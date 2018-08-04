module ServerFile exposing (..)

import Json.Decode
import Json.Decode.Pipeline


type alias ServerFile =
    { guid : String
    , title : Maybe String
    , bookGuid : String
    , bookTitle : Maybe String
    , trackIndex : Int
    , progressSeconds : Float
    , mediaType : String
    , nextFile : String
    }


defaultServerFile : ServerFile
defaultServerFile =
    { guid = "", title = Just "", bookGuid = "", bookTitle = Just "", trackIndex = 0, progressSeconds = 0, mediaType = "", nextFile = "" }


decodeServerFile : Json.Decode.Decoder ServerFile
decodeServerFile =
    Json.Decode.Pipeline.decode ServerFile
        |> Json.Decode.Pipeline.required "guid" Json.Decode.string
        |> Json.Decode.Pipeline.required "title" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "bookGuid" Json.Decode.string
        |> Json.Decode.Pipeline.required "bookTitle" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "trackIndex" Json.Decode.int
        |> Json.Decode.Pipeline.required "progressSeconds" Json.Decode.float
        |> Json.Decode.Pipeline.required "mediaType" Json.Decode.string
        |> Json.Decode.Pipeline.required "nextFile" Json.Decode.string

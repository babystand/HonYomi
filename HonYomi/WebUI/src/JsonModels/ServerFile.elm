module ServerFile exposing (..)

import Json.Decode
import Json.Decode.Pipeline


type alias ServerFile =
    { guid : String
    , title : Maybe String
    , bookGuid : String
    , bookTitle : Maybe String
    , progressSeconds : Float
    , mediaType : String
    }


defaultServerFile : ServerFile
defaultServerFile =
    { guid = "", title = Just "", bookGuid = "", bookTitle = Just "", progressSeconds = 0, mediaType = "" }


decodeServerFile : Json.Decode.Decoder ServerFile
decodeServerFile =
    Json.Decode.Pipeline.decode ServerFile
        |> Json.Decode.Pipeline.required "guid" Json.Decode.string
        |> Json.Decode.Pipeline.required "title" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "bookGuid" Json.Decode.string
        |> Json.Decode.Pipeline.required "bookTitle" (Json.Decode.maybe Json.Decode.string)
        |> Json.Decode.Pipeline.required "progressSeconds" Json.Decode.float
        |> Json.Decode.Pipeline.required "mediaType" Json.Decode.string

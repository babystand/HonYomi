module ServerConfig exposing (..)

-- elm-package install -- yes noredink/elm-decode-pipeline

import Array exposing (Array)
import Json.Decode
import Json.Decode.Pipeline
import Json.Encode


type alias WatchDir =
    { path : String, guid : String }


type alias ServerConfig =
    { watchForChanges : Bool
    , scanInterval : Int
    , serverPort : Int
    , watchDirectories : Array WatchDir
    }


decodeWatchDir : Json.Decode.Decoder WatchDir
decodeWatchDir =
    Json.Decode.Pipeline.decode WatchDir
        |> Json.Decode.Pipeline.required "path" Json.Decode.string
        |> Json.Decode.Pipeline.required "guid" Json.Decode.string


encodeWatchDir : WatchDir -> Json.Encode.Value
encodeWatchDir record =
    Json.Encode.object
        [ ( "path", Json.Encode.string <| record.path )
        , ( "guid", Json.Encode.string <| record.guid )
        ]


decodeServerConfig : Json.Decode.Decoder ServerConfig
decodeServerConfig =
    Json.Decode.Pipeline.decode ServerConfig
        |> Json.Decode.Pipeline.required "watchForChanges" Json.Decode.bool
        |> Json.Decode.Pipeline.required "scanInterval" Json.Decode.int
        |> Json.Decode.Pipeline.required "serverPort" Json.Decode.int
        |> Json.Decode.Pipeline.required "watchDirectories" (Json.Decode.array decodeWatchDir)


encodeServerConfig : ServerConfig -> Json.Encode.Value
encodeServerConfig record =
    Json.Encode.object
        [ ( "watchForChanges", Json.Encode.bool <| record.watchForChanges )
        , ( "scanInterval", Json.Encode.int <| record.scanInterval )
        , ( "serverPort", Json.Encode.int <| record.serverPort )
        , ( "watchDirectories", Json.Encode.array <| Array.map encodeWatchDir <| record.watchDirectories )
        ]

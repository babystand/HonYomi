module ServerConfig exposing (..)

-- elm-package install -- yes noredink/elm-decode-pipeline

import Json.Decode
import Json.Decode.Pipeline
import Json.Encode


type alias WatchDir =
    { guid : String, path : String }


type alias ServerConfig =
    { watchForChanges : Bool
    , scanInterval : Int
    , serverPort : Int
    , watchDirectories : List WatchDir
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
        |> Json.Decode.Pipeline.required "watchDirectories" (Json.Decode.list decodeWatchDir)


encodeServerConfig : ServerConfig -> Json.Encode.Value
encodeServerConfig record =
    Json.Encode.object
        [ ( "watchForChanges", Json.Encode.bool <| record.watchForChanges )
        , ( "scanInterval", Json.Encode.int <| record.scanInterval )
        , ( "serverPort", Json.Encode.int <| record.serverPort )
        , ( "watchDirectories", Json.Encode.list <| List.map encodeWatchDir <| record.watchDirectories )
        ]

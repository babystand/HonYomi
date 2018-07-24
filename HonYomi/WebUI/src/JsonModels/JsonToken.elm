module JsonToken exposing (..)

import Json.Decode exposing (Decoder, float, int, string)
import Json.Decode.Pipeline exposing (decode, hardcoded, optional, required)
import Json.Encode exposing (object)


type alias JsonToken =
    { token : String
    }


decodeJsonToken : Decoder JsonToken
decodeJsonToken =
    decode JsonToken
        |> required "token" string


encodeJsonToken : JsonToken -> Json.Encode.Value
encodeJsonToken record =
    object
        [ ( "token", Json.Encode.string <| record.token )
        ]

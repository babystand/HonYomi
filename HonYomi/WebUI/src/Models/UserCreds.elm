module UserCreds exposing (UserCreds, encodeUserCreds)

import Json.Encode exposing (object)
import Json.Decode exposing (int, string, float, Decoder)
import Json.Decode.Pipeline exposing (decode, required, optional, hardcoded)


type alias UserCreds =
    { username : String
    , password : String
    }


decodeUserCreds : Decoder UserCreds
decodeUserCreds =
    decode UserCreds
        |> required "username" (string)
        |> required "password" (string)


encodeUserCreds : UserCreds -> Json.Encode.Value
encodeUserCreds record =
    object
        [ ( "username", Json.Encode.string <| record.username )
        , ( "password", Json.Encode.string <| record.password )
        ]

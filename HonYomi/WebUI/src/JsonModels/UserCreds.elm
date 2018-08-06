module UserCreds exposing (UserCreds, encodeUserCreds)

import Json.Decode exposing (Decoder, float, int, string)
import Json.Decode.Pipeline exposing (decode, hardcoded, optional, required)
import Json.Encode exposing (object)


type alias UserCreds =
    { username : String
    , password : String
    , newPassword : String
    }


decodeUserCreds : Decoder UserCreds
decodeUserCreds =
    decode UserCreds
        |> required "username" string
        |> required "password" string
        |> required "newPassword" string


encodeUserCreds : UserCreds -> Json.Encode.Value
encodeUserCreds record =
    object
        [ ( "username", Json.Encode.string <| record.username )
        , ( "password", Json.Encode.string <| record.password )
        , ( "newPassword", Json.Encode.string <| record.newPassword )
        ]

module Util exposing (..)

import Array


tryHead : Array.Array a -> Maybe a
tryHead arr =
    Array.get 0 arr

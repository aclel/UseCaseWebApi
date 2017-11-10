module CommonLibrary

open Microsoft.FSharpLu.Json

/////// ROP //////

// convert a single value into a two-track result
let succeed x = 
    Ok x

// convert a single value into a two-track result
let fail x = 
    Error x

// apply either a success function or failure function
let either successFunc failureFunc twoTrackInput =
    match twoTrackInput with
    | Ok s -> successFunc s
    | Error f -> failureFunc f

// convert a switch function into a two-track function
let bind f = 
    either f fail

// pipe a two-track value into a switch function 
let (>>=) x f = 
    bind f x

// convert a one-track function into a switch
let switch f = 
    f >> succeed

// convert a one-track function into a two-track function
let map f = 
    either (f >> succeed) fail

// convert a dead-end function into a one-track function
let tee f x = 
    f x; x 

// convert a one-track function into a switch with exception handling
let tryCatch f exnHandler x =
    try
        f x |> succeed
    with
    | ex -> exnHandler ex |> fail

// convert two one-track functions into a two-track function
let doubleMap successFunc failureFunc =
    either (successFunc >> succeed) (failureFunc >> fail)

// add two switches in parallel
let plus addSuccess addFailure switch1 switch2 x = 
    match (switch1 x),(switch2 x) with
    | Ok s1,Ok s2 -> Ok (addSuccess s1 s2)
    | Error f1,Ok _  -> Error f1
    | Ok _ ,Error f2 -> Error f2
    | Error f1,Error f2 -> Error (addFailure f1 f2)

let (&&&) v1 v2 = 
    let addSuccess r1 r2 = r1 // return first
    let addFailure s1 s2 = s1 + "; " + s2  // concat
    plus addSuccess addFailure v1 v2

///// OTHER STUFF ////////

let serialise input =
    Compact.serialize input

let parseRequest json =
    Compact.deserialize json
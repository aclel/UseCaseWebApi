module BasinUseCases

open CommonLibrary
open FSharp.Data
open Microsoft.FSharpLu.Json

[<Literal>]
let connectionString = 
    @""


type BasinId = int
type Code = string
type Name = string

let validateCodeRequired (code : Code) =
    if code = "" then Error "Code must not be blank"
    else Ok ()

let validateCodeLength (code : Code) =
    let minLength = 3
    let maxLength = 6
    if code.Length < minLength || code.Length > maxLength
        then Error (sprintf "Code must be between %d and %d characters long" minLength maxLength)
    else Ok ()

let validateNameRequired (name : Name) =
    if name = "" then Error "Name must not be blank" else Ok()
        

module GetBasinsUseCase =

    type GetBasinResponse = {
        BasinId: BasinId
        Code: Code
        Name: Name
    }

    let getBasins' =
        use cmd = new SqlCommandProvider<"
            select Id, Name, Code from BASIN order by Id
        ", connectionString>(connectionString)


        cmd.Execute()
        |> Seq.map (fun x ->
            { BasinId = x.Id; Code = x.Code; Name = x.Name }
        )

    let serialiseGetBasinResponse = serialise

    let getBasins'' query serialise =
        query
        |> serialise
        |> fun x -> 200, x

    let getBasins = getBasins'' getBasins' serialiseGetBasinResponse


module CreateBasinUseCase =

    type CreateBasinRequest = {
        BasinId: BasinId
        Code: Code
        Name: Name
    }

    let createBasin' ((basinId : BasinId), (name : Name), (code : Code)) =
        use cmd = new SqlCommandProvider<"
            insert into BASIN values (@Id, @Code, @Name)
        ", connectionString>(connectionString)

        cmd.Execute(Id = basinId, Code = code, Name = name) |> ignore

    let validateCodeRequired (input : CreateBasinRequest) =
        match validateCodeRequired input.Code with
        | Ok _ -> Ok input
        | Error x -> Error x

    let validateCodeLength (input : CreateBasinRequest) =
        match validateCodeLength input.Code with
        | Ok _ -> Ok input
        | Error x -> Error x

    let validateNameRequired (input : CreateBasinRequest) =
        match validateNameRequired input.Name with
        | Ok _ -> Ok input
        | Error x -> Error x
        
    // Validate in parallel
    let validateCreateBasinRequest =
        validateCodeRequired
        &&& validateCodeLength
        &&& validateNameRequired

    let parseCreateBasinRequest json =
        Compact.deserialize<CreateBasinRequest> json

    let createBasin'' (request : CreateBasinRequest) = createBasin'(request.BasinId, request.Code, request.Name)
    
    let createBasin''' parse validate create json =
        json
        |> parse
        |> validate
        |> fun a ->
            match a with
            | Ok x -> create x |> fun _ -> 200, ""
            | Error x -> 422, x

    let createBasin = createBasin''' parseCreateBasinRequest validateCreateBasinRequest createBasin''


module UpdateBasinUseCase =

    type UpdateBasinRequest = {
        BasinId: BasinId
        Code: Code
        Name: Name
    }

    let updateBasin' ((basinId : BasinId), (name : Name), (code : Code)) =
        use cmd = new SqlCommandProvider<"
            update BASIN set Code = @Code, Name = @Name where Id = @BasinId
        ", connectionString>(connectionString)

        cmd.Execute(BasinId = basinId, Code = code, Name = name) |> ignore

    let validateCodeRequired (input : UpdateBasinRequest) =
        match validateCodeRequired input.Code with
        | Ok _ -> Ok input
        | Error x -> Error x

    let validateCodeLength (input : UpdateBasinRequest) =
        match validateCodeLength input.Code with
        | Ok _ -> Ok input
        | Error x -> Error x

    let validateNameRequired (input : UpdateBasinRequest) =
        match validateNameRequired input.Name with
        | Ok _ -> Ok input
        | Error x -> Error x
        
    // Validate in parallel
    let validateUpdateBasinRequest =
        validateCodeRequired
        &&& validateCodeLength
        &&& validateNameRequired

    let parseUpdateBasinRequest json =
        Compact.deserialize<UpdateBasinRequest> json

    let updateBasin'' (request : UpdateBasinRequest) = updateBasin'(request.BasinId, request.Code, request.Name)

    let updateBasin''' parse validate update json =
        json
        |> parse
        |> validate
        |> fun a ->
            match a with
            | Ok x -> update x |> fun _ -> 200, ""
            | Error x -> 422, x

    let updateBasin = updateBasin''' parseUpdateBasinRequest validateUpdateBasinRequest updateBasin''
module BasinHandlers

open Giraffe
open Giraffe.HttpHandlers
open Giraffe.HttpContextExtensions

open Microsoft.AspNetCore.Http

open BasinUseCases


let handleGetBasins =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let response =
                GetBasinsUseCase.getBasins
                |> fun (statusCode, message) ->
                    setStatusCode statusCode >=> json message
           return! response next ctx
        }


let handleCreateBasin =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! body = ctx.ReadBodyFromRequest() 
            let response =
                body
                |> CreateBasinUseCase.createBasin
                |> fun (statusCode, message) ->
                    setStatusCode statusCode >=> json message
            return! response next ctx
        }

let handleUpdateBasin =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! body = ctx.ReadBodyFromRequest() 
            let response =
                body
                |> UpdateBasinUseCase.updateBasin
                |> fun (statusCode, message) ->
                    setStatusCode statusCode >=> json message
            return! response next ctx
        }
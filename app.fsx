#r "packages/Suave/lib/net40/Suave.dll"
#load "static.fsx"

open Suave
open Suave.Web
open Suave.Http
open Suave.Types
open Suave.Http.Successful
open Suave.Http.Redirection
open Suave.Http.Files
open Suave.Http.RequestErrors
open Suave.Http.Applicatives
open Static

let mutable id = 0
let mutable todos = []

let getTodos () =
    todos
    |> List.map (fun t -> sprintf "{ \"id\": %d, \"text\": \"%s\" }" (fst t) (snd t))
    |> String.concat ","
    |> sprintf "{ \"todos\": [ %s ] }" 

let add (text : Choice<string, string>) =
    match text with
    | Choice1Of2 t -> 
        let next = id + 1
        id    <- next
        todos <- (id, t) :: todos
        ()
    | Choice2Of2 t -> 
        ()
    
let remove id = 
    let removed =
        todos
        |> List.filter (fun t -> (fst t) <> id)
    todos <- removed
    ()

let app : WebPart =
    choose 
        [ GET >>= choose
            [ path "/static/app.js" >>= Writers.setMimeType "application/javascript" >>= OK script
              path "/static/style.css" >>= Writers.setMimeType "text/css" >>= OK style
              path "/" >>= OK html 
              //pathScan "/static/%s" (fun (filename) -> file (sprintf "./static/%s" filename)) 
              path "/todos" >>= request (fun req -> OK (getTodos ())) ]   
          POST >>= choose
            [ path "/todos" >>= request (fun req -> add (req.formData "text") ; OK "") ]
          DELETE >>= choose
            [ pathScan "/todos/%d" (fun (id) -> remove id ; OK "") ]       
        ]
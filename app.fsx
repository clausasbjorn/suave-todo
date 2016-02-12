#r "packages/Suave/lib/net40/Suave.dll"
#r "System.Data.dll"
#r "System.Data.Linq.dll"
#r "packages/FSharp.Data.TypeProviders/lib/net40/FSharp.Data.TypeProviders.dll"
#load "static.fsx"

open System
open System.Linq
open System.Data
open System.Data.Linq
open Suave
open Suave.Web
open Suave.Http
open Suave.Successful
open Suave.Redirection
open Suave.Files
open Suave.Filters
open Suave.RequestErrors

open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open Static

type Sql = 
    SqlDataConnection<"YOUR CONNECTION STRING">

let getDb () =
    Sql.GetDataContext()

let getTodos () =
    let db = getDb () 
    query {
        for t in db.TodoItems do
        select (t.TodoItemId, t.Todo)
    } 
    |> Seq.toList
    |> List.map (fun t -> sprintf "{ \"id\": %d, \"text\": \"%s\" }" (fst t) (snd t))
    |> String.concat ","
    |> sprintf "{ \"todos\": [ %s ] }" 

let add (text : Choice<string, string>) =
    match text with
    | Choice1Of2 t -> 
        let db = getDb ()
        let record = new Sql.ServiceTypes.TodoItems(Todo = t)
        db.TodoItems.InsertOnSubmit(record)
        db.DataContext.SubmitChanges()
        ()
    | Choice2Of2 t -> 
        ()
    
let remove id = 
    let db = getDb () 
    let deleteRowsFrom (table:Table<_>) rows = table.DeleteAllOnSubmit(rows)
    query {
        for t in db.TodoItems do
        where (t.TodoItemId.Equals(id))
        select t
    } |> deleteRowsFrom db.TodoItems
    db.DataContext.SubmitChanges()
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
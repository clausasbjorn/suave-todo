#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/FAKE/tools/FakeLib.dll"
#load "app.fsx"
open App
open Fake
open System
open System.IO
open Suave
open Suave.Http
open Suave.Web
open Suave.Types

let serverConfig =
    let port = int (getBuildParam "port")
    { defaultConfig with
        homeFolder = Some __SOURCE_DIRECTORY__
        logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Warn
        bindings = [ Types.HttpBinding.mk' Types.HTTP "127.0.0.1" port ] }

Target "run" (fun _ ->
    startWebServer serverConfig app
)

RunTargetOrDefault "run"
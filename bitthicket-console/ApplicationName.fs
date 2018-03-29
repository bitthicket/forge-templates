module <%= namespace %>

open System
open System.IO
open Config
open Logging

[<EntryPoint>]
let main argv =
    let (arguConfig, config) = parseAllConfig argv
    let cliLogLevel = arguConfig.TryGetResult (<@ LogLevel @>)
    let logLevel = Option.defaultValue (parseLogLevel config.["Serilog:MinimumLevel"]) cliLogLevel

    let logger = configureLogger config cliLogLevel |> getLogger
    logger.Information("Starting {programName} at LogLevel {logLevel}", 
        Environment.GetCommandLineArgs().[0] |> Path.GetFileName, 
        logLevel)
    logger.Verbose("testing verbose")

    // do stuff

    0 // return an integer exit code

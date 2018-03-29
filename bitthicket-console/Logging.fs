module Logging

open Serilog
open Serilog.Events

let getLogger (loggerConfig:LoggerConfiguration) = loggerConfig.CreateLogger()

let configureLogger config level =
    let mutable loggerConfiguration = LoggerConfiguration()

    loggerConfiguration <- loggerConfiguration.ReadFrom.Configuration(config)

    loggerConfiguration <- 
        match level with
        | Some LogEventLevel.Verbose -> loggerConfiguration.MinimumLevel.Verbose()
        | Some LogEventLevel.Debug -> loggerConfiguration.MinimumLevel.Debug()
        | Some LogEventLevel.Information -> loggerConfiguration.MinimumLevel.Information()
        | Some LogEventLevel.Warning -> loggerConfiguration.MinimumLevel.Warning()
        | Some LogEventLevel.Error -> loggerConfiguration.MinimumLevel.Error()
        | Some LogEventLevel.Fatal -> loggerConfiguration.MinimumLevel.Fatal()
        | _ -> loggerConfiguration

    loggerConfiguration

let parseLogLevel levelString =
    LogEventLevel.Parse levelString
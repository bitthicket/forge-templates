module internal Config

open System
open System.IO

open Microsoft.Extensions.Configuration
open Argu
open Serilog
open Serilog.Events

type Arguments =
    | [<CustomCommandLine("--log-level", "-l")>]
      LogLevel of LogEventLevel
    | [<CustomCommandLine("--config", "-c")>] 
      ConfigFilePath of string
    with
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | LogLevel _ -> sprintf "control verbosity of output; default [%A]" LogEventLevel.Debug
                | ConfigFilePath _ -> "config file path; defaults to current directory."

type private AspNetConfigurationReader(config:IConfiguration) =
    interface IConfigurationReader with
        member __.Name = "AspNetConfigurationReader"
        member __.GetValue(key:string) = config.[key]

let loadConfigurationFromFile path =
    let addFile path builder =
        YamlConfigurationExtensions.AddYamlFile(builder, path)

    let fullPath = Path.GetFullPath(path)
    let builder = ConfigurationBuilder()
                    |> addFile fullPath
    builder.Build()

let parseAllConfig args =
    let errorHandler = ProcessExiter(colorizer = function 
                                                 | ErrorCode.HelpText -> None
                                                 | _ -> Some ConsoleColor.Red)
    let programName = Path.GetFileName(Environment.GetCommandLineArgs().[0])
    
    let parser = ArgumentParser.Create<Arguments>(programName = programName, errorHandler = errorHandler)
    let cliResult = parser.ParseCommandLine(inputs = args, raiseOnUsage = false)
    let configPath = cliResult.GetResult (<@ ConfigFilePath @>, "./config.yml")
    let config = loadConfigurationFromFile configPath
    
    parser.Parse(args, AspNetConfigurationReader(config)), config
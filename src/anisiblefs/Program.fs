open System.IO
open Chiron
open Chiron.Operators

type ModuleArgs = {
    Name : string
}
with
    static member create name =
        {
            Name = name
        }
    static member FromJson(_:ModuleArgs) =
        ModuleArgs.create
        <!> Json.readOrDefault "name" "World"

type Response = {
    Message : string
    Changed : bool
    Failed : bool
}
with
    static member ToJson (r:Response) =
           Json.write "msg" r.Message
        *> Json.write "changed" r.Changed
        *> Json.write "failed" r.Failed

module Result =
    let ofException fn =
        try
            let r = fn
            r |> Result.Ok
        with
            | ex -> ex |> Result.Error

[<EntryPoint>]
let main argv =
    match argv with
    | [|argsFilePath|] ->
        match File.ReadAllText argsFilePath |> Result.ofException with
        | Result.Ok(json) ->
            let (args:ModuleArgs) = json |> Json.parse |> Json.deserialize
            let result = { Message = (System.String.Concat ("Hello, ", args.Name, " from F# module.")); Changed = true; Failed = false }
            System.Console.WriteLine(result |> Json.serialize |> Json.format)
        | Result.Error(ex) ->
            let result = { Message = (System.String.Concat ("Error reading arguments file: ", ex)); Changed = true; Failed = true }
            System.Console.WriteLine(result |> Json.serialize |> Json.format)
    | _ ->
        let result =
            {
                Message = "Module expects a single argument (args file path)."
                Changed = false
                Failed = true
            }
        System.Console.WriteLine(result |> Json.serialize |> Json.format)
    
    0
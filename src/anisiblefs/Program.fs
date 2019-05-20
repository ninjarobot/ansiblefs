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

/// Response that is returned from running the task.
/// https://docs.ansible.com/ansible/latest/reference_appendices/common_return_values.html
type Response = {
    Message : string
    Changed : bool
    Failed : bool
    Exception : string option
    Facts: Map<string, Json>
}
with
    static member ToJson (r:Response) =
           Json.write "msg" r.Message
        *> Json.write "changed" r.Changed
        *> Json.write "failed" r.Failed
        *> Json.writeUnlessDefault "exception" None r.Exception
        *> Json.writeUnlessDefault "ansible_facts" Map.empty r.Facts

let inline parseDeserialize json =
    match json |> Json.tryParse with
    | Choice1Of2 parsed ->
        match parsed |> Json.tryDeserialize with
        | Choice1Of2 o -> Result.Ok o
        | Choice2Of2 err -> Result.Error err
    | Choice2Of2 err -> Result.Error err

module Result =
    let ofException fn =
        try
            let r = fn
            r |> Result.Ok
        with
            | ex -> ex |> Result.Error

[<EntryPoint>]
let main argv =

    // +11 MB
    let http = new System.Net.Http.HttpClient()

    match argv with
    | [|argsFilePath|] ->
        match File.ReadAllText argsFilePath |> Result.ofException with
        | Result.Error (ex) ->
            {
                Message = (System.String.Concat ("Error reading arguments file: ", ex.Message))
                Changed = false
                Failed = true
                Exception = Some (string ex)
                Facts = Map.empty
            }
        | Result.Ok json ->
            match json |> parseDeserialize with
            | Result.Error err ->
                {
                    Message = (System.String.Concat ("Error parsing arguments JSON: ", err))
                    Changed = false
                    Failed = true
                    Exception = Some (err)
                    Facts = Map.empty
                }
            | Result.Ok (args:ModuleArgs) ->
                let nicFacts =
                    System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                    |> Array.mapi(fun i iface -> (System.String.Format ("nic-{0}", i)), Json.String(iface.Name))
                {
                    Message = (System.String.Concat ("Hello, ", args.Name, " from F# module."))
                    Changed = true
                    Failed = false
                    Exception = None
                    Facts = nicFacts |> Map.ofArray
                }
    | _ ->
        {
            Message = "Module expects a single argument (args file path)."
            Changed = false
            Failed = true
            Exception = None
            Facts = Map.empty
        }
    |> Json.serialize
    |> Json.format
    |> System.Console.WriteLine
    
    0
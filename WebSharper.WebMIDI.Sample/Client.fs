namespace WebSharper.WebMIDI.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.WebMIDI

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let statusMessage = Var.Create "Waiting for MIDI connection..."
    let midiMessages = Var.Create ""

    let handleMIDIMessage(event: MIDIMessageEvent) = 
        let data = event.Data

        printfn($"MIDI Message: {data}")
        midiMessages := $"{midiMessages}\n{data}" 

    // Function to connect to MIDI devices
    let connectMIDI () =
        promise {
            try 
                let! midiAccess = As<Navigator>(JS.Window.Navigator).RequestMIDIAccess()
                statusMessage := "MIDI Access Granted!"

                midiAccess.Inputs.ForEach(fun (input: MIDIInput) ->
                    input.AddEventListener("midimessage", fun (evt: Dom.Event) ->
                        let event = evt |> As<MIDIMessageEvent>
                        handleMIDIMessage(event)
                    )
                )
                //midiAccess.Inputs.Get()
            with error ->
                statusMessage := "MIDI Access Denied!"
                printfn($"MIDI Connection Error: {error.Message}")
        }

    [<SPAEntryPoint>]
    let Main () =
        let newName = Var.Create ""

        IndexTemplate.Main()
            .connectMIDI(fun _ ->
                async {
                    do! connectMIDI().AsAsync()
                }
                |> Async.Start
            )
            .status(statusMessage.V)
            .midiMessages(midiMessages.V)
            .Doc()
        |> Doc.RunById "main"

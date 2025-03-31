# WebSharper WebMIDI API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [WebMIDI API](https://developer.mozilla.org/en-US/docs/Web/API/WebMIDI_API), enabling seamless communication with MIDI devices in WebSharper applications.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the WebMIDI API.

2. **Sample Project**:
   - Demonstrates how to use the WebMIDI API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/WebMIDI/).

## Installation

To use this package in your WebSharper project, add the NuGet package:

```bash
   dotnet add package WebSharper.WebMIDI
```

## Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/WebMIDI.git
   cd WebMIDI
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.WebMIDI/WebSharper.WebMIDI.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.WebMIDI.Sample
   dotnet build
   dotnet run
   ```

4. Open the hosted demo to see the Sample project in action:
   [https://dotnet-websharper.github.io/WebMIDI/](https://dotnet-websharper.github.io/WebMIDI/)

## Example Usage

Below is an example of how to use the WebMIDI API in a WebSharper project:

```fsharp
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

    // Variable to display the MIDI connection status.
    let statusMessage = Var.Create "Waiting for MIDI connection..."

    // Variable to store received MIDI messages.
    let midiMessages = Var.Create ""

    // Function to handle incoming MIDI messages.
    let handleMIDIMessage(event: MIDIMessageEvent) =
        let data = event.Data

        printfn($"MIDI Message: {data}")
        midiMessages := $"{midiMessages}\n{data}"

    // Function to connect to MIDI devices
    let connectMIDI () =
        promise {
            try
                let! midiAccess = JS.Window.Navigator.RequestMIDIAccess()
                statusMessage := "MIDI Access Granted!"

                midiAccess.Inputs.ForEach(fun (input: MIDIInput) ->
                    input.AddEventListener("midimessage", fun (evt: Dom.Event) ->
                        let event = evt |> As<MIDIMessageEvent>
                        handleMIDIMessage(event)
                    )
                )
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
                    // Start MIDI connection asynchronously.
                    do! connectMIDI().AsAsync()
                }
                |> Async.Start
            )
            // Bind the status message to the UI.
            .status(statusMessage.V)
            // Display received MIDI messages in the UI.
            .midiMessages(midiMessages.V)
            .Doc()
        |> Doc.RunById "main"
```

This example demonstrates how to request access to MIDI devices, listen for incoming MIDI messages, and handle MIDI input in a WebSharper project.

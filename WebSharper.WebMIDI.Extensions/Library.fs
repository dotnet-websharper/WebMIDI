namespace WebSharper.WebMIDI

open WebSharper
open WebSharper.JavaScript

[<JavaScript; AutoOpen>]
module Extensions =

    type Navigator with

        [<Inline "$this.requestMIDIAccess($MIDIOptions)">]
        member this.RequestMIDIAccess(MIDIOptions: MIDIOptions) : Promise<MIDIAccess> =
            X<Promise<MIDIAccess>>

        [<Inline "$this.requestMIDIAccess()">]
        member this.RequestMIDIAccess() : Promise<MIDIAccess> =
            X<Promise<MIDIAccess>>

namespace WebSharper.WebMIDI

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    module Enum = 
        let MIDIPortType = 
            Pattern.EnumStrings "MIDIPortType" [
                "input" 
                "output"
            ]

        let MIDIState = 
            Pattern.EnumStrings "MIDIState" [
                "connected" 
                "disconnected"
            ]

        let MIDIConnection =
            Pattern.EnumStrings "MIDIConnection" [
                "open"
                "closed"
                "pending"
            ]

    let MIDIPort =
        Class "MIDIPort"
        |=> Inherits T<Dom.EventTarget>
        |+> Instance [
            "id" =? T<string>  
            "manufacturer" =? T<string> 
            "name" =? T<string> 
            "type" =? Enum.MIDIPortType 
            "version" =? T<string> 
            "state" =? Enum.MIDIState 
            "connection" =? Enum.MIDIConnection 

            "open" => T<unit> ^-> T<Promise<unit>> 
            "close" => T<unit> ^-> T<Promise<unit>> 
        ]

    let MIDIMessageEventInit =
        Pattern.Config "MIDIMessageEventInit" {
            Required = [
                "data", T<Uint8Array> 
            ]
            Optional = []
        }
            
    let MIDIMessageEvent =
        Class "MIDIMessageEvent"
        |=> Inherits T<Dom.Event>
        |+> Static [
            Constructor (T<string>?``type`` * !?MIDIMessageEventInit?options) 
        ]
        |+> Instance [
            "data" =? T<Uint8Array> 
        ]
    
    let MIDIInput =
        Class "MIDIInput"
        |=> Inherits MIDIPort
        |+> Instance [
            "onmidimessage" =@ MIDIMessageEvent ^-> T<unit>
        ]
        
    let MIDIInputMap = 
        let forEachCallbackfn = MIDIInput?value * !?T<string>?key * !?TSelf?parent ^-> T<unit>

        Class "MIDIInputMap"
        |=> Inherits T<Map<string, obj>>  
        |+> Instance [
            "size" =? T<int>

            "entries" => T<unit> ^-> T<obj>
            "get" => T<string>?key ^-> MIDIInput
            "has" => T<string>?key ^-> T<bool>
            "keys" => T<unit> ^-> T<obj>
            "values" => T<unit> ^-> T<obj>
            "forEach" => (forEachCallbackfn?callbackfn * !?T<obj>?thisArg ^-> T<unit>)
        ]

    let MIDIOutput =
        Class "MIDIOutput"
        |=> Inherits MIDIPort
        |+> Instance [
            "send" => (T<Uint8Array>?data * !?T<float>?timestamp) ^-> T<unit> 
            "clear" => T<unit> ^-> T<unit> 
        ]
    
    let MIDIOutputMap = 
        let forEachCallbackfn = MIDIInput?value * !?T<string>?key * !?TSelf?parent ^-> T<unit>

        Class "MIDIOutputMap"
        |=> Inherits T<Map<string, obj>> 
        |+> Instance [
            "size" =? T<int>

            "entries" => T<unit> ^-> T<obj>
            "get" => T<string>?key ^-> MIDIOutput
            "has" => T<string>?key ^-> T<bool>
            "keys" => T<unit> ^-> T<obj>
            "values" => T<unit> ^-> T<obj>
            "forEach" => (forEachCallbackfn?callbackfn * !?T<obj>?thisArg ^-> T<unit>)
        ]
    
    let MIDIAccess =
        Class "MIDIAccess"
        |=> Inherits T<Dom.EventTarget>
        |+> Instance [
            "inputs" =? MIDIInputMap.Type  
            "outputs" =? MIDIOutputMap.Type 
            "sysexEnabled" =? T<bool>  
        ]

    let MIDIConnectionEventInit =
        Pattern.Config "MIDIConnectionEventInit" {
            Required = [
                "port", MIDIPort.Type 
            ]
            Optional = [
                "bubbles", T<bool> 
                "cancelable", T<bool> 
                "composed", T<bool> 
            ]
        }

    let MIDIConnectionEvent =
        Class "MIDIConnectionEvent"
        |=> Inherits T<Dom.Event>
        |+> Static [
            Constructor (T<string>?``type`` * !?MIDIConnectionEventInit?options)
        ] 
        |+> Instance [
            "port" =? MIDIPort 
        ]

    let MIDIOptions = 
        Pattern.Config "MIDIOptions" {
            Required = [
                "sysex", T<bool> 
                "software", T<bool>
            ]
            Optional = []
        }

    let Assembly =
        Assembly [
            Namespace "WebSharper.WebMIDI" [
                MIDIOptions
                MIDIMessageEvent
                MIDIMessageEventInit
                MIDIConnectionEvent
                MIDIConnectionEventInit
                MIDIAccess
                MIDIOutputMap
                MIDIOutput
                MIDIInputMap
                MIDIInput
                MIDIPort
                Enum.MIDIConnection
                Enum.MIDIState
                Enum.MIDIPortType
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()

namespace Tasky.Core

open System

[<Sealed>]
type internal Task(id: int, name: string, notes: string, ddone: bool) = class
    member val Id = id with get
    member val Name = name with get, set
    member val Notes = notes with get, set
    member val Done = ddone with get, set

    new() = Task(0, "", "", false)
end
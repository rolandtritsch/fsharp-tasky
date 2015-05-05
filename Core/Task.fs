namespace Tasky.Core

open System

type Task(id: int, name: string, notes: string, ddone: bool) = class
    member this.Id = id
    member val Name = name with get, set
    member val Notes = notes with get, set
    member val Done = ddone with get, set

    new() = Task(0, "", "", false)
end
namespace Tasky.Core

open System

type Task(id: int, name: string, notes: string, ddone: bool) = class
    member this.Id = id
    member this.Name = name
    member this.Notes = notes
    member this.Done = ddone

    new() = Task(0, "", "", false)
end
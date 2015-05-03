namespace Tasky.Core

open System

type Task(id: int, name: string, note: string, ddone: bool) = class
    member this.Id = id
    member this.Name = name
    member this.Note = note
    member this.Done = ddone
end
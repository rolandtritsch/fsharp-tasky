namespace Tasky.Core

open System

open SQLite

[<Sealed>]
[<Table("Items")>]
type Task(id: int, name: string, notes: string, ddone: bool) = class
    static member val IdKey = "TaskId" with get 

    [<PrimaryKey; AutoIncrement; Column("_id")>]
    member val Id = id with get
    member val Name = name with get, set
    member val Notes = notes with get, set
    member val Done = ddone with get, set

    new() = Task(0, "", "", false)
end
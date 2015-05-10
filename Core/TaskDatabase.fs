namespace Tasky.Core

open System
open System.IO
open System.Data

open SQLite

[<Sealed>]
[<AbstractClass>]
type TaskDatabase private() = class
    static let mutable db: SQLiteConnection = null

    static let dbPath(dbName) = begin
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dbName)
    end

    static let init() = begin
        if(db = null) then begin
            db <- new SQLiteConnection(TaskDatabase.DbName)
            db.CreateTable<Task>() |> ignore
        end
    end

    static member val DbName = dbPath("TaskDatabase.db3") with get

    static member GetTasks(): List<Task> = begin
        init()
        let all = [
            for t in db.Table<Task>() do yield t
            ]
        all
    end

    static member GetTask(id: int): Task = begin
        init()
        db.Get<Task>(id)
    end
        
    static member SaveTask(t: Task): int = begin
        init()
        if(t.Id >= 0) then db.Update(t) else db.Insert(t)
    end
         
    static member DeleteTask(id: int): int = begin
        init()
        db.Delete<Task>(id)
    end

    static member Reset() = begin
        db <- null
    end
end
namespace Tasky.Core

open System
open System.IO
open System.Data

open Mono.Data.Sqlite

type TaskDatabase(dbPath: string) = class
    let monitor = new Object()
    let dbPath = dbPath 
    let mutable connection = null
    do 
        connection <- new SqliteConnection("Data Source=" + dbPath)
        connection.Open()

        // create the table (and ignore the exception, if the table already exists)
        let c = connection.CreateCommand()
        c.CommandText <- "CREATE TABLE [Items] (_id INTEGER PRIMARY KEY ASC, Name NTEXT, Notes NTEXT, Done INTEGER);"
        try    
            c.ExecuteNonQuery() |> ignore
        with
            | _ -> () 

    interface System.IDisposable with
        member this.Dispose() = connection.Close()
    end

    static member DatabaseFilePath(sqliteFilename: string): string = begin
        #if NETFX_CORE
        let path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
        #else

        #if SILVERLIGHT
        // Windows Phone expects a local path, not absolute
        let path = sqliteFilename
        #else

        #if __ANDROID__
        // Just use whatever directory SpecialFolder.Personal returns
        let libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        #else
        // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
        // (they don't want non-user-generated data in Documents)
        let documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) // Documents folder
        let libraryPath = Path.Combine(documentsPath, "..", "Library") // Library folder
        #endif

        let path = Path.Combine(libraryPath, sqliteFilename)
        #endif

        #endif
        path
    end

    static member private FromReader(r: SqliteDataReader): Task = begin
        Task(
            Convert.ToInt32(r.["_id"]),
            r.["Name"].ToString(),
            r.["Notes"].ToString(),
            if(Convert.ToInt32(r.["Done"]) = 1) then true else false
        )
    end

    member this.GetTasks(): List<Task> = begin
        lock monitor (
            using (connection.CreateCommand()) (fun c ->
                c.CommandText <- "SELECT [_id], [Name], [Notes], [Done] from [Items]"
                let r = c.ExecuteReader()
                let tasks = [
                    while r.Read() do yield TaskDatabase.FromReader(r)
                ]
                (fun unit -> tasks)
            )
        )
    end

    member this.GetTask(id: int): Task = begin
        lock monitor (
            using (connection.CreateCommand()) (fun c -> 
                c.CommandText <- "SELECT [_id], [Name], [Notes], [Done] from [Items] WHERE [_id] = ?"
                c.Parameters.Add(SqliteParameter(DbType.Int32, id)) |> ignore
                let r = c.ExecuteReader()
                r.Read() |> ignore
                let t = TaskDatabase.FromReader(r)
                (fun unit -> t)
            ) 
        )
    end
        
    member this.SaveTask(t: Task): int = begin
        lock monitor (
            if (t.Id <> 0) then
                using (connection.CreateCommand()) (fun c ->
                    c.CommandText <- "UPDATE [Items] SET [Name] = ?, [Notes] = ?, [Done] = ? WHERE [_id] = ?"
                    c.Parameters.Add(SqliteParameter(DbType.String, t.Name)) |> ignore
                    c.Parameters.Add(SqliteParameter(DbType.String, t.Notes)) |> ignore
                    c.Parameters.Add(SqliteParameter(DbType.Int32, t.Done)) |> ignore
                    c.Parameters.Add(SqliteParameter(DbType.Int32, t.Id)) |> ignore
                    let r = c.ExecuteNonQuery()
                    (fun unit -> r)
                )
            else
                using (connection.CreateCommand()) (fun c -> 
                    c.CommandText <- "INSERT INTO [Items] ([Name], [Notes], [Done]) VALUES (? ,?, ?)"
                    c.Parameters.Add(SqliteParameter(DbType.String, t.Name)) |> ignore
                    c.Parameters.Add(SqliteParameter(DbType.String, t.Notes)) |> ignore
                    c.Parameters.Add(SqliteParameter(DbType.Int32, t.Done)) |> ignore
                    let r = c.ExecuteNonQuery()
                    (fun unit -> r)
                )
        )
    end
         
    member this.DeleteTask(id: int): int = begin
        lock monitor (
            using (connection.CreateCommand()) (fun c ->
                c.CommandText <- "DELETE FROM [Items] WHERE [_id] = ?"
                c.Parameters.Add(SqliteParameter(DbType.Int32, id)) |> ignore
                let r = c.ExecuteNonQuery()
                (fun unit -> r)
            )
        )
    end
end
namespace Tasky.Core

open System
open System.IO
open System.Data

open Mono.Data.Sqlite

[<Sealed>]
[<AbstractClass>]
type TaskDatabase private() = class
    static let monitor = new Object()

    static let dbPath(dbName) = begin
        #if NETFX_CORE
        let path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName);
        #else

        #if SILVERLIGHT
        // Windows Phone expects a local path, not absolute
        let path = dbName
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

        let path = Path.Combine(libraryPath, dbName)
        #endif

        #endif
        path
    end

    static let fromReader(r: SqliteDataReader): Task = begin
        Task(
            Convert.ToInt32(r.["_id"]),
            r.["Name"].ToString(),
            r.["Notes"].ToString(),
            if(Convert.ToInt32(r.["Done"]) = 1) then true else false
        )
    end

    do begin
        let connection = new SqliteConnection("Data Source=" + TaskDatabase.DbName)
        connection.Open()

        // create the table (and ignore the exception, if the table already exists)
        let c = connection.CreateCommand()
        c.CommandText <- "CREATE TABLE [Items] (_id INTEGER PRIMARY KEY ASC, Name NTEXT, Notes NTEXT, Done INTEGER)"
        try    
            c.ExecuteNonQuery() |> ignore
        with
            | _ -> () 
        
        connection.Close()
    end

    static member val DbName = dbPath("TaskDatabase.db3") with get

    static member GetTasks(): List<Task> = begin
        let connection = new SqliteConnection("Data Source=" + TaskDatabase.DbName)
        connection.Open()

        let tasks = lock monitor (
            using (connection.CreateCommand()) (fun c ->
                c.CommandText <- "SELECT [_id], [Name], [Notes], [Done] from [Items]"
                let r = c.ExecuteReader()
                let tasks = [
                    while r.Read() do yield fromReader(r)
                ]
                (fun unit -> tasks)
            )
        )
        connection.Close()
        tasks
    end

    static member GetTask(id: int): Task = begin
        let connection = new SqliteConnection("Data Source=" + TaskDatabase.DbName)
        connection.Open()

        let task = lock monitor (
            using (connection.CreateCommand()) (fun c -> 
                c.CommandText <- "SELECT [_id], [Name], [Notes], [Done] from [Items] WHERE [_id] = ?"
                c.Parameters.Add(SqliteParameter(DbType.Int32, id)) |> ignore
                let r = c.ExecuteReader()
                r.Read() |> ignore
                let t = fromReader(r)
                (fun unit -> t)
            ) 
        )

        connection.Close()
        task
    end
        
    static member SaveTask(t: Task): int = begin
        let connection = new SqliteConnection("Data Source=" + TaskDatabase.DbName)
        connection.Open()

        let r = lock monitor (
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
        connection.Close()
        r
    end
         
    static member DeleteTask(id: int): int = begin
        let connection = new SqliteConnection("Data Source=" + TaskDatabase.DbName)
        connection.Open()

        let r = lock monitor (
            using (connection.CreateCommand()) (fun c -> 
                c.CommandText <- "DELETE FROM [Items] WHERE [_id] = ?"
                c.Parameters.Add(SqliteParameter(DbType.Int32, id)) |> ignore
                let r = c.ExecuteNonQuery()
                (fun unit -> r)
            )
        )
        connection.Close()
        r
    end
end
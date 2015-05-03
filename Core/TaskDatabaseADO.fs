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
        connection <- new SqliteConnection("Data Source=" + this.dbPath)
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
            Convert.ToInt32(r ["_id"]),
			r["Name"].ToString(),
			r["Notes"].ToString(),
            Convert.ToInt32(if (r["Done"]) = 1 then true else false)
        )
    end

    member this.GetTasks(): List<Task> = begin
        let tl = List<Task>()
        lock monitor (
            using (connection.CreateCommand()) (fun c ->
                c.CommandText <- "SELECT [_id], [Name], [Notes], [Done] from [Items]"
                let r = c.ExecuteReader()
                while (r.Read()) do
                    tl.Add (FromReader(r))
                done
			)
         )
         tl
    end

    member this.GetTask(id: int): Task = begin
        let mutable t: Task = null
        lock monitor (
            using (connection.CreateCommand()) (fun c ->
                c.CommandText <- "SELECT [_id], [Name], [Notes], [Done] from [Items] WHERE [_id] = ?"
                c.Parameters.Add(SqliteParameter(DbType.Int32), {Value = id})
                let r = c.ExecuteReader()
                r.Read; t <- FromReader(r)
            )
        )
        t
    end
        
    member SaveTask(t: Task): int = begin
        let mutable r = 0
        lock monitor (
            if (t.Id != 0) then
                using (connection.CreateCommand()) (fun c ->
                    c.CommandText <- "UPDATE [Items] SET [Name] = ?, [Notes] = ?, [Done] = ? WHERE [_id] = ?"
                    c.Parameters.Add(new SqliteParameter(DbType.String), {Value = t.Name})
                    c.Parameters.Add(new SqliteParameter(DbType.String), {Value = t.Notes})
                    c.Parameters.Add(new SqliteParameter(DbType.Int32), {Value = t.Done})
                    c.Parameters.Add(new SqliteParameter(DbType.Int32), {Value = t.Id})
                    r <- c.ExecuteNonQuery()
                )
            else
                using (connection.CreateCommand()) (fun c -> 
                    c.CommandText <- "INSERT INTO [Items] ([Name], [Notes], [Done]) VALUES (? ,?, ?)"
                    c.Parameters.Add(new SqliteParameter(DbType.String), {Value = t.Name})
                    c.Parameters.Add(new SqliteParameter(DbType.String), {Value = t.Notes})
                    c.Parameters.Add(new SqliteParameter(DbType.Int32), {Value = t.Done})
                    r <- command.ExecuteNonQuery()
                )
        )
        r
    end
         
    member DeleteTask(id: int): int = begin
        let mutable r = 0
        lock monitor (
            using (connection.CreateCommand()) (fun c ->
                c.CommandText <- "DELETE FROM [Items] WHERE [_id] = ?"
                c.Parameters.Add(new SqliteParameter(DbType.Int32), {Value = id})
                r <- c.ExecuteNonQuery()
            )
        )
    end
end
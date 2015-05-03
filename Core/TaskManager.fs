namespace Tasky.Core

open System

type TaskManager = class
    static member theDb = new TaskDatabase(TaskDatabase.DatabaseFilePath("TaskDatabase.db3"))
end
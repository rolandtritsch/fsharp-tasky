namespace Tasky.Android.Test

open System
open NUnit.Framework

open Tasky.Core

[<TestFixture>]
type TestCore() = class
    let theTask = Task(999, "Name", "Notes", false)

    [<SetUp>]
    member x.Setup() = begin
        // MainActivity.GetAppCtx().DeleteDatabase(TaskDatabase.DbName)
    end
    
    [<TearDown>]
    member x.Tear() = ()

    [<Test>]
    member this.TestTask() = begin
        let task = Task()
        Assert.That(task.Id = -1)

        let name = "Roland"
        task.Name <- name
        Assert.That(task.Name = name)

        let notes = "Tritsch"
        task.Notes <- notes
        Assert.That(task.Notes = notes)

        let ddone = true
        task.Done <- ddone
        Assert.That(task.Done = ddone)
    end

    [<Test>]
    member this.TestTaskDatabase() = begin
        let numberOfTasks = 10
        let tasks = [
            for i in 1 .. numberOfTasks do yield Task(-1, "Task_" + i.ToString(), "Note_" + i.ToString(), false)
        ]

        List.iter (fun t -> TaskDatabase.SaveTask(t) |> ignore) tasks

        let allTasks = TaskDatabase.GetTasks()
        Assert.That(allTasks.Length = numberOfTasks)

        let secondTaskId = 2
        let secondTask = TaskDatabase.GetTask(secondTaskId)
        Assert.That(secondTask.Id = secondTaskId)

        TaskDatabase.SaveTask(Task(-1, "Task_999", "Notes_999", false)) |> ignore
        let allTasksPlusOne = TaskDatabase.GetTasks()
        Assert.That(allTasksPlusOne.Length = numberOfTasks + 1)
        Assert.True(List.exists (fun (t: Task) -> t.Name = "Task_999") allTasksPlusOne)
    end
end
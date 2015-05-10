namespace Tasky.Android

open Android.App
open Android.OS
open Android.Widget

open Tasky.Core

[<Sealed>]
[<Activity (Label = "Tasky.Android.TaskDetailsScreen")>]
type public TaskDetailsScreen() = class
    inherit Activity()

    let mutable task = new Task()
    let mutable cancelDeleteButton: Button = null
    let mutable notesTextEdit: EditText = null
    let mutable nameTextEdit: EditText = null
    let mutable saveButton: Button = null

    member private this.Save() = begin
        task.Name <- nameTextEdit.Text
        task.Notes <- notesTextEdit.Text
        TaskDatabase.SaveTask(task) |> ignore
        base.Finish()
    end

    member private this.CancelDelete() = begin
        if(task.Id <> 0) then TaskDatabase.DeleteTask(task.Id) |> ignore
        base.Finish()
    end

    override this.OnCreate(savedInstanceState: Bundle) = begin
        base.OnCreate(savedInstanceState)

        // set our layout to be the task details screen
        base.SetContentView(Resource_Layout.TaskDetails)
        nameTextEdit <- base.FindViewById<EditText>(Resource_Id.NameText)
        notesTextEdit <- base.FindViewById<EditText>(Resource_Id.NotesText)
        saveButton <- base.FindViewById<Button>(Resource_Id.SaveButton)

        // find all our controls
        cancelDeleteButton <- base.FindViewById<Button>(Resource_Id.CancelDeleteButton)

        // set the cancel delete based on whether or not it's an existing task
        cancelDeleteButton.Text <- if(task.Id = 0) then "Cancel" else "Delete"

        // wire up the handlers
        cancelDeleteButton.Click.Add(fun e -> this.CancelDelete())
        saveButton.Click.Add(fun e -> this.Save())

        let taskId = base.Intent.GetIntExtra(Task.IdKey, 0)
        if(taskId > 0) then task <- TaskDatabase.GetTask(taskId) else task <- new Task()
        nameTextEdit.Text <- task.Name
        notesTextEdit.Text <- task.Notes
    end
end
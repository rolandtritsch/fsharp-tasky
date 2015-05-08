namespace Tasky.Android

open Android.App
open Android.OS
open Android.Widget

open Tasky.Core

[<Activity (Label = "Tasky.Android.TaskDetailsScreen")>]
type TaskDetailsScreen() = class
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

        let taskID = base.Intent.GetIntExtra("TaskID", 0)
        if(taskID > 0) then task <- TaskDatabase.GetTask(taskID)

        // set our layout to be the home screen
        base.SetContentView(Resource_Layout.TaskDetails)
        nameTextEdit <- base.FindViewById<EditText>(Resource_Id.NameText)
        notesTextEdit <- base.FindViewById<EditText>(Resource_Id.NotesText)
        saveButton <- base.FindViewById<Button>(Resource_Id.SaveButton)

        // find all our controls
        cancelDeleteButton <- base.FindViewById<Button>(Resource_Id.CancelDeleteButton)

        // set the cancel delete based on whether or not it's an existing task
        cancelDeleteButton.Text <- if (task.Id = 0) then "Cancel" else "Delete"

        nameTextEdit.Text <- task.Name
        notesTextEdit.Text <- task.Notes

        // button clicks 
        cancelDeleteButton.Click.Add (fun (e: System.EventArgs) -> this.CancelDelete())
        saveButton.Click.Add (fun (e: System.EventArgs) -> this.Save())
    end
end
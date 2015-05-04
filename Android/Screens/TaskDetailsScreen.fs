namespace Tasky.Screens

open Android.App
open Android.OS
open Android.Widget

open Tasky.Core

[Activity (Label = "TaskDetailsScreen")]
type TaskDetailsScreen() = class
    inherit Activity()

    let task = new Task()
    let mutable cancelDeleteButton: Button = null
    let mutable notesTextEdit: EditText = null
    let mutable nameTextEdit: EditText = null
    let mutable saveButton: Button = null 

    override this.OnCreate(savedInstanceState: Bundle) = begin
        base.OnCreate(savedInstanceState)

        let taskID = Intent.GetIntExtra("TaskID", 0)
        if(taskID > 0) then task <- TaskManager.theDb.GetTask(taskID)

        // set our layout to be the home screen
        SetContentView(global::Android.Resource.Layout.TaskDetails)
        nameTextEdit <- FindViewById<EditText>(Resource_Id.NameText)
        notesTextEdit <- FindViewById<EditText>(Resource_Id.NotesText)
        saveButton <- FindViewById<Button>(Resource_Id.SaveButton)

        // find all our controls
        cancelDeleteButton <- FindViewById<Button>(Resource_Id.CancelDeleteButton)

        // set the cancel delete based on whether or not it's an existing task
        cancelDeleteButton.Text <- if (task.ID = 0) then "Cancel" else "Delete"

        nameTextEdit.Text <- task.Name
        notesTextEdit.Text <- task.Notes

        // button clicks 
        cancelDeleteButton.Click.Add (fun (sender, e) -> CancelDelete())
        saveButton.Click.Add (fun (sender, e) -> Save())
    end
    
    let Save() = begin
        task.Name <- nameTextEdit.Text
        task.Notes <- notesTextEdit.Text
        TaskManager.theDb.SaveTask(task)
        Finish()
    end

    let CancelDelete() = begin
        if(task.ID != 0) then TaskManager.theDb.DeleteTask(task.ID)
        Finish()
    end
end
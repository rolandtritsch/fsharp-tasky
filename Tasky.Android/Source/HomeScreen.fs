namespace Tasky.Android

open System

open Android.App
open Android.Content
open Android.OS
open Android.Widget

open Tasky.Core

[<Sealed>]
[<Activity (Label = "Tasky.Android", MainLauncher = true)>]
type HomeScreen() = class
    inherit Activity()

    let mutable taskList = new TaskListAdapter(null, List.Empty)
    let mutable tasks = List<Task>.Empty
    let mutable deleteDbButton: Button = null
    let mutable addTaskButton: Button = null
    let mutable taskListView: ListView = null
        
    override this.OnCreate(savedInstanceState: Bundle) = begin
        base.OnCreate(savedInstanceState)

        // set our layout to be the home screen
        base.SetContentView(Resource_Layout.HomeScreen)

        // find our controls
        deleteDbButton <- base.FindViewById<Button>(Resource_Id.DeleteDbButton)
        addTaskButton <- base.FindViewById<Button>(Resource_Id.AddButton)
        taskListView <- base.FindViewById<ListView>(Resource_Id.TaskList)

        // wire up handlers
        assert (deleteDbButton <> null)
        deleteDbButton.Click.Add (fun e ->
            this.ApplicationContext.DeleteDatabase(TaskDatabase.DbName) |> ignore
            TaskDatabase.Reset()

            tasks <- TaskDatabase.GetTasks()
            taskList <- new TaskListAdapter(this, tasks)
            taskListView.Adapter <- taskList
        )           

        assert (addTaskButton <> null)
        addTaskButton.Click.Add (fun e ->
            let clazz: System.Type = typeof<TaskDetailsScreen>
            this.StartActivity(clazz)
        )           

        assert (taskListView <> null)
        taskListView.ItemClick.Add (fun (e: AdapterView.ItemClickEventArgs) -> 
            let ctx: Context = this.ApplicationContext
            let clazz: System.Type = typeof<TaskDetailsScreen>
            let taskDetails = new Intent(ctx, clazz)
            let pos = e.Position
            let task = tasks.Item(pos)
            taskDetails.PutExtra(Task.IdKey, task.Id) |> ignore
            this.StartActivity(taskDetails)
        )
    end
        
    override this.OnResume() = begin
        base.OnResume()

        tasks <- TaskDatabase.GetTasks()
        taskList <- new TaskListAdapter(this, tasks)
        taskListView.Adapter <- taskList
    end
end
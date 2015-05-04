namespace Tasky.Screens

open Android.App
open Android.Content
open Android.OS
open Android.Widget

open Tasky
open Tasky.Core
open Tasky.Adapters

[Activity (Label = "Tasky", MainLauncher = true, Icon="@drawable/icon")]
type HomeScreen() = class
    inherit Activity()
    let mutable taskList: TaskListAdapter = null
    let mutable tasks: List<Task> = null
    let mutable addTaskButton: Button = null
    let mutable taskListView: ListView = null
        
    override this.OnCreate(savedInstanceState: Bundle) = begin
        base.OnCreate(savedInstanceState)

        // set our layout to be the home screen
        base.SetContentView(Resource_Layout.HomeScreen)

        // find our controls
        taskListView <- base.FindViewById<ListView>(Resource_Id.TaskList)
        addTaskButton <- base.FindViewById<Button>(Resource_Id.AddButton)

        // wire up add task button handler
        assert (addTaskButton != null)
        addTaskButton.Click.Add (fun sender e -> begin
            StartActivity(typeof(TaskDetailsScreen))
        end)           

        // wire up task click handler
        assert (taskListView != null)
        taskListView.ItemClick.Add (fun (sender: object, e: AdapterView.ItemClickEventArgs e) -> begin
            let taskDetails = new Intent (this, typeof (TaskDetailsScreen))
            taskDetails.PutExtra("TaskID", tasks[e.Position].ID)
            StartActivity(taskDetails)
        end)
    end
        
    override this.OnResume() = begin
        base.OnResume()

        tasks <- TaskManager.theDb.GetTasks()

        // create our adapter
        taskList <- new Adapters.TaskListAdapter(this, tasks)

        // hook up our adapter to our ListView
        taskListView.Adapter <- taskList
    end
end
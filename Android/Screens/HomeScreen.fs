namespace Tasky.Android.Screens

open Android.App
open Android.Content
open Android.OS
open Android.Widget

open Tasky.Core
open Tasky.Android.Adapters

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
        SetContentView(global::Android.Resource.Layout.HomeScreen);

        // find our controls
        taskListView <- FindViewById<ListView>(global::Android.Resource.Id.TaskList);
        addTaskButton <- FindViewById<Button>(global::Android.Resource.Id.AddButton);

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
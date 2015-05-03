using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using Tasky.Core;
using Tasky.Android.Adapters;

namespace Tasky.Android.Screens {
	/// <summary>
	/// Main ListView screen displays a list of tasks, plus an [Add] button
	/// </summary>
	[Activity (Label = "Tasky", MainLauncher = true, Icon="@drawable/icon")]			
	public class HomeScreen : Activity {
		TaskListAdapter taskList;
		IList<Task> tasks;
		Button addTaskButton;
		ListView taskListView;
		
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			// set our layout to be the home screen
			SetContentView(global::Android.Resource.Layout.HomeScreen);

			//Find our controls
			taskListView = FindViewById<ListView>(global::Android.Resource.Id.TaskList);
			addTaskButton = FindViewById<Button>(global::Android.Resource.Id.AddButton);

			// wire up add task button handler
			if(addTaskButton != null) {
				addTaskButton.Click += (sender, e) => {
					StartActivity(typeof(TaskDetailsScreen));
				};
			}
			
			// wire up task click handler
			if(taskListView != null) {
				taskListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					var taskDetails = new Intent (this, typeof (TaskDetailsScreen));
					taskDetails.PutExtra ("TaskID", tasks[e.Position].ID);
					StartActivity (taskDetails);
				};
			}
		}
		
		protected override void OnResume() {
			base.OnResume();

			tasks = TaskManager.theDb.GetTasks();
			
			// create our adapter
			taskList = new Adapters.TaskListAdapter(this, tasks);

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}
	}
}
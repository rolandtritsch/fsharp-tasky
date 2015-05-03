using Android.App;
using Android.OS;
using Android.Widget;
using Tasky.Core;
using Tasky.Android;

namespace Tasky.Android.Screens {
	/// <summary>
	/// View/edit a Task
	/// </summary>
	[Activity (Label = "TaskDetailsScreen")]			
	public class TaskDetailsScreen : Activity {
		Task task = new Task();
		Button cancelDeleteButton;
		EditText notesTextEdit;
		EditText nameTextEdit;
		Button saveButton;

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			
			int taskID = Intent.GetIntExtra("TaskID", 0);
			if(taskID > 0) {
				task = TaskManager.theDb.GetTask(taskID);
			}
			
			// set our layout to be the home screen
			SetContentView(global::Android.Resource.Layout.TaskDetails);
			nameTextEdit = FindViewById<EditText>(global::Android.Resource.Id.NameText);
			notesTextEdit = FindViewById<EditText>(global::Android.Resource.Id.NotesText);
			saveButton = FindViewById<Button>(global::Android.Resource.Id.SaveButton);
			
			// find all our controls
			cancelDeleteButton = FindViewById<Button>(global::Android.Resource.Id.CancelDeleteButton);
			
			// set the cancel delete based on whether or not it's an existing task
			cancelDeleteButton.Text = (task.ID == 0 ? "Cancel" : "Delete");
			
			nameTextEdit.Text = task.Name; 
			notesTextEdit.Text = task.Notes;

			// button clicks 
			cancelDeleteButton.Click += (sender, e) => { CancelDelete(); };
			saveButton.Click += (sender, e) => { Save(); };
		}

		void Save() {
			task.Name = nameTextEdit.Text;
			task.Notes = notesTextEdit.Text;
			TaskManager.theDb.SaveTask(task);
			Finish();
		}
		
		void CancelDelete() {
			if(task.ID != 0) {
				TaskManager.theDb.DeleteTask(task.ID);
			}
			Finish();
		}
	}
}
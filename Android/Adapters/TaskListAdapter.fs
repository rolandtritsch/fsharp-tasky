using System.Collections.Generic;

using Android.App;
using Android.Widget;
using Android.Views;

using Tasky.Core;

namespace Tasky.Android.Adapters {
	/// <summary>
	/// Adapter that presents Tasks in a row-view
	/// </summary>
	public class TaskListAdapter : BaseAdapter<Task> {
		Activity context = null;
		IList<Task> tasks = new List<Task>();
		
		public TaskListAdapter(Activity context, IList<Task> tasks) : base () {
			this.context = context;
			this.tasks = tasks;
		}
		
		public override Task this[int index] {
			get { return tasks[index]; }
		}
		
		public override long GetItemId(int position) {
			return position;
		}
		
		public override int Count {
			get { return tasks.Count; }
		}
		
		public override View GetView(int position, View convertView, ViewGroup parent) {
			// Get our object for position
			var item = tasks[position];			

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// gives us some performance gains by not always inflating a new view
			// will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = (convertView ?? context.LayoutInflater.Inflate(
					global::Android.Resource.Layout.TaskListItem, 
					parent, 
					false)) as LinearLayout;

			// Find references to each subview in the list item's view
			var txtName = view.FindViewById<TextView>(global::Android.Resource.Id.NameText);
			var txtDescription = view.FindViewById<TextView>(global::Android.Resource.Id.NotesText);

			//Assign item's values to the various subviews
			txtName.SetText(item.Name, TextView.BufferType.Normal);
			txtDescription.SetText(item.Notes, TextView.BufferType.Normal);

			//Finally return the view
			return view;
		}
	}
}
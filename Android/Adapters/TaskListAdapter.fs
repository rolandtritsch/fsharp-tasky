namespace Tasky.Android.Adapters

open System.Collections.Generic

open Android.App
open Android.Widget
open Android.Views

open Tasky.Core

type TaskListAdapter(context: Activity, tasks: List<Task>) = class
    inherit BaseAdapter<Task>()

    override this.GetItemId(position: int): int64 = begin
        (int64) position
    end

    override this.Count: int = begin
        tasks.Count
    end

    override this.GetView(position: int, convertView: View, parent: ViewGroup): View = begin 
        let item = tasks.Item(position)
        let view = if (convertView != null) then
                       convertView
                   else
                       context.LayoutInflater.Inflate(
                           global::Android.Resource.Layout.TaskListItem, 
                           parent, 
                           false
                       )
                   |?> LinearLayout

        // Find references to each subview in the list item's view
        let txtName = view.FindViewById<TextView>(global::Android.Resource.Id.NameText)
        let txtDescription = view.FindViewById<TextView>(global::Android.Resource.Id.NotesText)

        // Assign item's values to the various subviews
        txtName.SetText(item.Name, TextView.BufferType.Normal)
        txtDescription.SetText(item.Note, TextView.BufferType.Normal)

        //Finally return the view
        view
    end
end
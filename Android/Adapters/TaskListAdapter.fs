namespace Tasky.Adapters

open System.Collections.Generic

open Android.App
open Android.Widget
open Android.Views

open Tasky
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
                           Resource_Layout.TaskListItem, 
                           parent, 
                           false
                       ) :?> LinearLayout

        // Find references to each subview in the list item's view
        let txtName = view.FindViewById<TextView>(Resource_Id.NameText)
        let txtDescription = view.FindViewById<TextView>(Resource_Id.NotesText)

        // Assign item's values to the various subviews
        txtName.SetText(item.Name, TextView.BufferType.Normal)
        txtDescription.SetText(item.Notes, TextView.BufferType.Normal)

        //Finally return the view
        view
    end
end
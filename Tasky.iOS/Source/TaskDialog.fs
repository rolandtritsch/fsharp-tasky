namespace Tasky.iOS

open UIKit

open MonoTouch.Dialog

open Tasky.Core

type TaskDialog(task: Task) = class
    [<Entry("task name")>]
    member val Name = task.Name with get, set

    [<Entry("other task info")>]
    member val Notes = task.Notes with get, set

    [<Section ("")>]
    [<OnTap ("SaveTask")>]
    [<Alignment (UITextAlignment.Center)>]
    member val Save = "" with get, set

    [<Section ("")>]
    [<OnTap ("DeleteTask")>]
    [<Alignment (UITextAlignment.Center)>]
    member val Delete = "" with get, set
end
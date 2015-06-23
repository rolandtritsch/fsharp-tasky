namespace Tasky.iOS

open System

open UIKit
open Foundation

open MonoTouch.Dialog

open Tasky.Core

[<Register ("ViewController")>]
type HomeScreen() as this = class
    inherit DialogViewController(UITableViewStyle.Plain, null)

    let mutable tasks = List<Task>.Empty
    let mutable context: BindingContext = null
    let mutable taskDialog: TaskDialog = TaskDialog(Task())
    let mutable currentTask: Task = Task()
    let mutable detailsScreen: DialogViewController = null

    do begin
        this.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Add), false)
        this.NavigationItem.RightBarButtonItem.Clicked.Add(fun e -> this.ShowTaskDetails(new Task()))
    end

    member this.ShowTaskDetails(task: Task) = begin
        currentTask <- task
        taskDialog <- new TaskDialog(task)
        context <- new BindingContext(this, taskDialog, "Task Details")
        detailsScreen <- new DialogViewController(context.Root, true)
        this.ActivateController(detailsScreen)
    end

    member this.SaveTask() = begin
        assert(context <> null
        context.Fetch()
        currentTask.Name <- taskDialog.Name
        currentTask.Notes <- taskDialog.Notes
        TaskDatabase.SaveTask(currentTask) |> ignore
        this.NavigationController.PopViewController(true)
    end

    member this.DeleteTask() = begin
        if(currentTask.Id >= 0) then TaskDatabase.DeleteTask(currentTask.Id) |> ignore
        this.NavigationController.PopViewController(true)
    end

    override this.ViewWillAppear(animated) = begin
        base.ViewWillAppear(animated)

        tasks <- TaskDatabase.GetTasks()
        let rows = [
                for t in tasks do yield new Element(t.Name + ": " + t.Notes)
            ]
        let s = new Section()
        s.AddAll(rows) |> ignore
        this.Root <- new RootElement("Tasky")
        this.Root <- new RootElement("Tasky") {s}
    end
    
    override this.Selected(indexPath: NSIndexPath) = begin
        let task = tasks.Item(indexPath.Row)
        this.ShowTaskDetails(task)
    end

    override this.CreateSizingSource(unevenRows) = begin
        let source = new EditingSource(this) 
        source :> DialogViewController.Source
    end

    member this.DeleteTaskRow(rowId: int) = begin
        TaskDatabase.DeleteTask(tasks.Item(rowId).Id)
    end
end

and EditingSource(dvc: DialogViewController) = class
    inherit DialogViewController.Source(dvc)

    override this.CanEditRow(tableView: UITableView, indexPath: NSIndexPath) = begin
        true
    end

    override this.EditingStyleForRow(tableView: UITableView, indexPath: NSIndexPath) = begin
        UITableViewCellEditingStyle.Delete
    end

    override this.CommitEditingStyle(tableView: UITableView, editingStyle: UITableViewCellEditingStyle, indexPath: NSIndexPath) = begin
        let section = this.Container.Root.Item(indexPath.Section)
        let element = if(section.Item(indexPath.Row) :? StringElement) then section.Item(indexPath.Row) :?> StringElement else null
        section.Remove(element)

        let dvc = if(this.Container :? HomeScreen) then this.Container :?> HomeScreen else new HomeScreen()
        dvc.DeleteTaskRow(indexPath.Row) |> ignore
    end
end
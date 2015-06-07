namespace Tasky.iOS

open UIKit
open Foundation

open MonoTouch.Dialog

type EditingSource(dvc: DialogViewController) = class
    inherit DialogViewController.Source(dvc)

    override this.CanEditRow(tableView: UITableView, indexPath: NSIndexPath) = begin
        true
    end

    override this.EditingStyleForRow(tableView: UITableView, indexPath: NSIndexPath) = begin
        UITableViewCellEditingStyle.Delete
    end

    override this.CommitEditingStyle(tableView: UITableView, editingStyle: UITableViewCellEditingStyle, indexPath: NSIndexPath) = begin
        let section = Container.Root[indexPath.Section]
        let element = if(section[indexPath.Row] :? StringElement) then section[indexPath.Row] :?> StringElement else null
        section.Remove(element)

        let dvc = if(Container :? HomeScreen) then Container :?> HomeScreen else null
        dvc.DeleteTaskRow(indexPath.Row)
    end
end
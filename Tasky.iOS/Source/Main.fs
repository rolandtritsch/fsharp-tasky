namespace Tasky.iOS

open UIKit

module Application = begin
    [<EntryPoint>]
    let main args = begin
        UIApplication.Main(args, null, "AppDelegate")
        0
    end
end
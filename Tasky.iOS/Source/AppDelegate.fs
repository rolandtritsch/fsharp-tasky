namespace Tasky.iOS

open System

open UIKit
open Foundation

[<Register ("AppDelegate")>]
type AppDelegate() = class
    inherit UIApplicationDelegate ()

    override val Window = null with get, set

    // This method is invoked when the application is ready to run.
    override this.FinishedLaunching (app, options) = begin
        true
    end
end
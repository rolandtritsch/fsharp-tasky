namespace Android.Test

open System

open Android.App
open Android.Content
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

[<Activity (Label = "Android.Test", MainLauncher = true)>]
type MainActivity () = class
    inherit Activity ()

    let mutable count: int = 1

    override this.OnCreate (bundle) = begin
        base.OnCreate (bundle)

        // Set our view from the "main" layout resource
        this.SetContentView (Resource_Layout.Main)

        // Get our button from the layout resource, and attach an event to it
        let button = this.FindViewById<Button>(Resource_Id.myButton)
        button.Click.Add (fun args -> 
            button.Text <- sprintf "%d clicks!" count
            count <- count + 1
        )
    end
end
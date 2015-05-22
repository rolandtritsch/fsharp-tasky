namespace Tasky.Android.Test

open System.Reflection

open Android.App
open Android.OS
open Android.Content

open Xamarin.Android.NUnitLite

[<Activity(Label = "Tasky.Android.Test", MainLauncher = true)>]
type MainActivity() = class
    inherit TestSuiteActivity()

    static let mutable globalAppCtx: Context = null

    static member val GetAppCtx = globalAppCtx with get

    override this.OnCreate(bundle) = begin 
        // tests can be inside the main assembly
        this.AddTest(Assembly.GetExecutingAssembly())
        // or in any reference assemblies
        // AddTest (typeof (Your.Library.TestClass).Assembly);
        // Once you called base.OnCreate(), you cannot add more assemblies.
        base.OnCreate(bundle)

        globalAppCtx <- this.ApplicationContext
    end
end
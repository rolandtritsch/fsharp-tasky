namespace Tasky.Android.Test

open System
open NUnit.Framework

[<TestFixture>]
type TestsSample = class    
    [<SetUp>]
    member x.Setup() = ()
    
    [<TearDown>]
    member x.Tear() = ()
    
    [<Test>]
    member x.Pass() = begin
        Console.WriteLine("test1")
        Assert.True(true)
    end
    
    [<Test>]
    member x.Fail() = begin
        Assert.False(true)
    end

    [<Test>]
    [<Ignore("another time")>]
    member x.Ignore() = begin
        Assert.True(false)
    end

    [<Test>]
    member x.Inconclusive() = begin
        Assert.Inconclusive("Inconclusive")
    end
end
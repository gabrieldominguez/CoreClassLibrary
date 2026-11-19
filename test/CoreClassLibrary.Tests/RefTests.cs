
using CoreClassLibrary.Objects.Refs;
using Xunit;

namespace CoreClassLibrary.Tests;

/// <summary>
/// RefTests
/// </summary>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/performance/#pass-and-return-by-reference"/>
public class RefTests
    {
    [Fact]
    public void ExpirableRef()
        { 
        ExpirableRef<DateTimeStamped<string>> obj = new ExpirableRef<DateTimeStamped<string>>(t => t.DateTimeStamp < DateTime.Now);

        Assert.NotNull(obj);
        Assert.Null(obj.Value);

        obj.Value = new DateTimeStamped<string>("expired", DateTime.Today);
        Assert.Null(obj.Value);

        obj.Value = new DateTimeStamped<string>("ok", DateTime.Today.AddDays(1));
        Assert.NotNull(obj.Value);
        Assert.Equal("ok", obj.Value.Value);
        Assert.Equal(DateTime.Today.AddDays(1), obj.Value.DateTimeStamp);
        }

    [Fact]
    public void Ref()
        { 
        ReadWrite<int> obj = new ReadWrite<int>(0);

        Assert.NotNull(obj);
        Assert.True(obj is not null);

        Assert.Equal(0, obj.Value);

        obj.Value = 1;
        Assert.Equal(1, obj.Value);
        }    

    [Fact]
    public void ValueAssignment()
        {
        int anInteger = 42;

        int location = anInteger;
        
        Assert.Equal(42, location);

        #pragma warning disable IDE0059 // Unnecessary assignment of a value
        int sameLocation = location;
        #pragma warning restore IDE0059 // Unnecessary assignment of a value
        sameLocation = 19;

        Assert.Equal(42, anInteger);
        Assert.Equal(42, location);
        Assert.Equal(19, sameLocation);
        }

    [Fact]
    public void RefAssignment()
        { 
        int anInteger = 42;

        ref int location = ref anInteger;
        
        Assert.Equal(42, location);

        ref int sameLocation = ref location;
        sameLocation = 19;

        Assert.Equal(19, anInteger);
        Assert.Equal(19, location);
        Assert.Equal(19, sameLocation);
        }

    [Fact]
    public void ClassAssignment()
        {
        ReadWrite<int> anInteger = new ReadWrite<int>(42);

        ReadWrite<int> location = anInteger;
        
        Assert.Equal(42, location.Value);

        ReadWrite<int> sameLocation = location;
        sameLocation.Value = 19;

        Assert.Equal(19, anInteger.Value);
        Assert.Equal(19, location.Value);
        Assert.Equal(19, sameLocation.Value);
        }
    }

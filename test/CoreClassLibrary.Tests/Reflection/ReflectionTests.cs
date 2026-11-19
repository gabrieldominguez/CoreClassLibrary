#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Objects;
using System.Reflection;
using Xunit;

namespace CoreClassLibrary.Tests.Reflection;

public class ReflectionTests(ITestOutputHelper output)
    {
    [Fact]
    // https://stackoverflow.com/questions/4482557/what-interfaces-do-all-arrays-implement-in-c
    // https://learn.microsoft.com/en-us/dotnet/api/system.array
    public void ArrayInterfaces()
        {
        foreach (Type type in typeof(int[]).GetInterfaces())
            {
            output.WriteLine(type.FullName ?? "");
            }

        int[] array = [];

        Assert.Equal(typeof(int[]).GetInterfaces(), array.GetType().GetInterfaces());

        Assert.True(array is IEnumerable<int>);
        Assert.True(array is IReadOnlyList<int>);
        Assert.True(array is IReadOnlyCollection<int>);
        Assert.True(array is IList<int>);
        Assert.True(array is ICollection<int>);

        Assert.Empty(array);

        Assert.Equal(0, array.Length);
        Assert.Equal(0, ((IList<int>)array).Count);
        }

    [Fact]
    public void PocoGetPropertiesTest()
        => Assert.Equal(["StaticProperty", "Property" ], typeof(Poco).GetProperties().Select(prop => prop.Name).ToArray());

    [Fact]
    public void PocoGetStaticPropertiesTest()
        => Assert.Equal(["StaticProperty"], typeof(Poco).GetProperties().Where(prop => prop.IsStatic()).Select(prop => prop.Name).ToArray());

    [Fact]
    public void MessageEntryGetPropertiesTest()
        => Assert.Equal(["DateTime", "Message"], typeof(MessageEntry).GetProperties().Select(prop => prop.Name).ToArray());

    [Fact]
    public void LogEntryGetPropertiesTest()
        => Assert.Equal("DateTime,Level,EventId,Name,State,Message,Exception".Split(','), typeof(LogEntry).GetProperties().Select(prop => prop.Name).ToArray());

    [Fact]
    public void SampleEntity()
        {
        SampleEntity sampleEntity1 = new SampleEntity(1, 2, "");
        SampleEntity sampleEntity2 = new SampleEntity(1, 2, "");

        Assert.Equal(sampleEntity1.GetHashCode(), sampleEntity2.GetHashCode());
        }
    //public static IEnumerable<Assembly> GetAllLocalReferencedAssemblies(this Assembly rootAssembly)

    [Fact]
    public void GetAllLocalReferencedAssemblies()
        {
        Assembly[] assemblies = Assembly.GetExecutingAssembly().GetAllLocalReferencedAssemblies().ToArray();

        foreach (Assembly item in assemblies)
            {
            output.WriteLine(item.GetDisplayName());
            }

        Assert.Contains(assemblies, ass => ass.GetDisplayName() == "CoreClassLibrary");
        }

    [Fact]
    public void NullConvert() => Assert.Null(Convert.ChangeType(null, typeof(SampleEntity)));

    [Fact]
    public void GetTypeDisplayName()
        {
        Assert.Equal("SampleEntity", TypeNameHelper.GetTypeDisplayName(typeof(SampleEntity), fullName: false));
        Assert.Equal("SampleEntity", typeof(SampleEntity).Name);

        Assert.Equal("Entity<SampleEntity, long>", TypeNameHelper.GetTypeDisplayName(typeof(Entity<SampleEntity, long>), fullName: false));
        Assert.Equal("Entity`2", typeof(Entity<SampleEntity, long>).Name);
        }
  
    [Fact]
    public void GetTypeDisplayFullName()
        {
        Assert.Equal("CoreClassLibrary.Objects.SampleEntity", TypeNameHelper.GetTypeDisplayName(typeof(SampleEntity)));
        Assert.Equal("CoreClassLibrary.Objects.SampleEntity", typeof(SampleEntity).FullName);

        Assert.Equal("CoreClassLibrary.Objects.Entity<CoreClassLibrary.Objects.SampleEntity, long>", TypeNameHelper.GetTypeDisplayName(typeof(Entity<SampleEntity, long>)));
        Assert.StartsWith("CoreClassLibrary.Objects.Entity`2[[CoreClassLibrary.Objects.SampleEntity, CoreClassLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[System.Int64, System.Private.CoreLib, Version=", typeof(Entity<SampleEntity, long>).FullName);
        }
    }

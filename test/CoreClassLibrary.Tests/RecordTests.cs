using Xunit;
//using Xunit;

namespace CoreClassLibrary.Tests;

public record Record (int Id)
    {
    public string? Desc { get; init; }
    public string? Desc2 { get; set; }
    }

/// <summary>
/// Record Equality Tests
/// </summary>
/// <remarks>
/// For records, value equality means that two variables of a record type are equal if the types match and all property and field values match. For other reference types such as classes, equality means reference equality. That is, two variables of a class type are equal if they refer to the same object. Methods and operators that determine equality of two record instances use value equality.
/// Not all data models work well with value equality.For example, Entity Framework Core depends on reference equality to ensure that it uses only one instance of an entity type for what is conceptually one entity.For this reason, record types aren't appropriate for use as entity types in Entity Framework Core.
/// </remarks>
/// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records"/>
/// <seealso cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#value-equality"/>
public class RecordTests(ITestOutputHelper output)
    {
    [Fact]
    public void TestToString()
        {
        Record r = new Record(1) { Desc = "A" };
        output.WriteLine(r.ToString());
        }

    [Theory]
    [InlineData("A")]
    [InlineData("")]
    [InlineData(null)]
    public void TestToStringClass(string? name)
        {
        string logEntryClass = new SampleClass.LogEntry { Id = 1, DateTime = new DateTime(2021, 9, 16), Level = 2, LevelDouble = 3.3, LevelDecimal = 4.4M, Name = name ?? "" }.ToString();
        string logEntryRecord = new SampleRecord.LogEntry(1, new DateTime(2021, 9, 16), 2, 3.3, 4.4M, name ?? "").ToString();

        output.WriteLine(logEntryClass);

        Assert.Equal(logEntryClass, logEntryRecord);
        }

    [Fact]
    public void TestToStringEmptyClass()
        {
        string logEntryClass = new SampleClass.LogEntry { Id = 1, DateTime = new DateTime(2021, 9, 16), Level = 2, LevelDouble = 3.3, LevelDecimal = 4.4M, Name = "" }.ToString();
        string logEntryRecord = new SampleRecord.LogEntry(1, new DateTime(2021, 9, 16), 2, 3.3, 4.4M, "").ToString();

        output.WriteLine(logEntryClass);
        Assert.Equal(logEntryClass, logEntryRecord);
        }

    [Fact]
    public void TestEqual()
        {
        Record r = new Record(1) { Desc = "A" };

        Assert.Equal(r, new Record(1) { Desc = "A" });

        Assert.NotEqual(r, new Record(1) { Desc = "B" });
        Assert.NotEqual(r, new Record(2) { Desc = "A" });
        }

    [Fact]
    public void TestEqual2()
        {
        Record r = new Record(1) { Desc = "A", Desc2 = "X" };

        Assert.NotEqual(r, new Record(1) { Desc = "A" });

        //Assert.NotEqual(r, new Record(1) { Desc = "B" });
        //Assert.NotEqual(r, new Record(2) { Desc = "A" });
        }

    // https://stackoverflow.com/questions/67662645/ignore-private-member-variables-when-doing-equality-checks-with-records-in-c-sha
    public record TestingRecord
        {
        // I believe this is causing it to fail, can it be ignored somehow?
        private readonly List<string> list = [];

        public string Variable { get; init; }

        public TestingRecord(string someValue)
            {
            Variable = someValue;
            list.Add(someValue);
            }
        }

    [Fact]
    public void TestRecord()
        {
        TestingRecord test1 = new TestingRecord("hello");
        TestingRecord test2 = new TestingRecord("hello");

        //Assert.Equal(test1, test2); // Fails
        Assert.NotEqual(test1, test2);
        }

    [Fact]
    public void TestClassEqual()
        {
        SampleClass.LogEntry logEntryClass = new SampleClass.LogEntry { Id = 1, DateTime = new DateTime(2021, 9, 16), Level = 2, LevelDouble = 3.3, LevelDecimal = 4.4M, Name = "" };

        Assert.Equal(logEntryClass, new SampleClass.LogEntry { Id = 1 });
        Assert.Equal(logEntryClass, new SampleClass.LogEntry { Id = 1, DateTime = new DateTime(2000, 1, 1) });

        Assert.NotEqual(logEntryClass, new SampleClass.LogEntry { Id = 2 });
        Assert.NotEqual(logEntryClass, new SampleClass.LogEntry { Id = 2, DateTime = new DateTime(2021, 9, 16) });
        }
    }

#pragma warning disable IDE0305 // Simplify collection initialization

using Microsoft.Extensions.Primitives;
using CoreClassLibrary.Collections;
using CoreClassLibrary.Extensions;
using Xunit;
using Xunit.Sdk;

namespace CoreClassLibrary.Tests;

public class EnumerableTests(ITestOutputHelper output)
	{
	public class TestChar : Test<char>;
	public class TestString : Test<string>;
	public class Test<T> : IEquatable<Test<T>>, IComparable<Test<T>>
		{
		public T[] Arr { get;  set; } = [];

		public int CompareTo(Test<T>? other)
			=> other == null ? 1 : this.Arr.CompareTo(other.Arr);

		public bool Equals(Test<T>? other)
			=> other != null && this.Arr.SequenceEqual(other.Arr);

		public override bool Equals(object? obj)
			=> this.Equals(obj as Test<T>);

		public override int GetHashCode()
			=> this.Arr.GetHashCode();
		}

	public class Fail : IEquatable<Fail>, IComparable<Fail>
		{
		public int CompareTo(Fail? other)
			=> throw new Exception("IComparable.CompareTo");

		public bool Equals(Fail? other)
			=> throw new Exception("IEquatable.Equals");

		public override bool Equals(object? obj)
			=> throw new Exception("Object.Equals");

		public override int GetHashCode()
			=> throw new Exception("Object.GetHashCode");
		}

	[Fact]
	public void NotNull()
		{
		object?[] objects = ["a", null, 2];
		foreach(object instance in objects.WhereNotNull())
			{
			output.WriteLine(instance.ToString() ?? "");
			Assert.NotNull(instance);
			}
		}
	
	[Fact]
	public void All()
		{
		const byte par = 3;
		Assert.True(new byte[] { 0, 1, 2 }.All(par, (i, p) => i < p));
		Assert.False(new byte[] { 0, 1, 2 }.All(par, (i, p) => i > p));
		}

	[Fact]
	public void SequenceEqual()
		{
		Assert.True(new byte[] { 0, 1, 2 }.SequenceEqual(new byte[] { 0, 1, 2 }));
		Assert.False(new byte[] { 0, 1, 2 }.SequenceEqual(new byte[] { 0, 1 }));
		Assert.False(new byte[] { 0, 1 }.SequenceEqual(new byte[] { 0, 1, 2 }));
		}

	[Fact]
	public void FirstTuple()
		{
		IEnumerable<int> nums = [ 1, 2, 3 ];

		Assert.Equal((1, 2), nums.FirstTuple());
		}

	[Fact]
	public void SequenceCompareTo()
		{
		IEnumerable<int> sec1 = [ 1, 2, 3 ];
		IEnumerable<int> sec2 = [ 1, 3, 2 ];

		Assert.True(sec1.CompareTo(sec2) < 0);
		}

	[Fact]
	public void SequenceCompareToEqual()
		{
		IEnumerable<int> sec1 = [ 1, 2, 3 ];
		IEnumerable<int> sec2 = [ 1, 2, 3 ];

		Assert.True(sec1.SequenceEqual(sec2));
		Assert.Equal(0, sec1.CompareTo(sec2));
		}

	[Fact]
	public void SequenceCompareToDiffLenght()
		{
		IEnumerable<string> sec1 = [ "A", "B" ];
		IEnumerable<string> sec2 = [ "A", "C" ];
		IEnumerable<string> sec3 = [ "A", "C", "D" ];

		Assert.True(sec1.CompareTo(sec2) < 0);
		Assert.True(sec2.CompareTo(sec3) < 0);
		}

	[Fact]
	public void EnumerateInTuples()
		{
		IEnumerable<string> sec1 = [ "A", "B" ];
		IEnumerable<string> sec2 = [ "A", "C", "D" ];

		(string, string)[] tuples = EnumerableExtensions.EnumerateInTuples(sec1, sec2).ToArray();

		Assert.Equal(2, tuples.Length);
		Assert.Equal(("A", "A"), tuples[0]);
		Assert.Equal(("B", "C"), tuples[1]);
		}

	[Fact]
	public void EnumerateInTuplesWithDefault()
		{
		IEnumerable<string> sec1 = [ "A", "B" ];
		IEnumerable<string> sec2 = [ "A", "C", "D" ];

		(string?, string?)[] tuples = EnumerableExtensions.EnumerateInTuplesWithDefault(sec1, sec2).ToArray();

		Assert.Equal(3, tuples.Length);
		Assert.Equal(("A", "A"), tuples[0]);
		Assert.Equal(("B", "C"), tuples[1]);
		Assert.Equal((null, "D"), tuples[2]);
		}

	[Fact]
	public void EqualsFail()
		{
		Assert.Throws<Exception>(() => Assert.True(new Fail().Equals(new Fail())));
		Assert.Throws<EqualException>(() => Assert.Equal(new Fail(), new Fail()));
		}

	[Fact]
	public void CompareFail()
		=> Assert.Throws<Exception>(() => Assert.Equal(0, new Fail().CompareTo(new Fail())));

	[Fact]
	public void EqualsEntity()
		{
		Assert.True(new TestString() { Arr = ["A", "B"] }.Equals(new TestString() { Arr = ["A", "B"] }));
		Assert.Equal(new TestString() { Arr = ["A", "B"] }, new TestString() { Arr = ["A", "B"] });
		Assert.Equal(new TestString() { Arr = [] }, new TestString() { Arr = [] });        
		Assert.False(new TestString() { Arr = ["A", "B"] }.Equals(null));


		Assert.NotEqual(new TestString() { Arr = ["A", "B"] }, new TestString() { Arr = ["A", "b"] });
		Assert.NotNull(new TestString() { Arr = ["A", "B"] });
		Assert.NotEqual(new TestString() { Arr = ["A", "B"] }, new TestString() { Arr = ["A", "C"] });
		Assert.NotEqual(new TestString() { Arr = ["A", "B"] }, new TestString() { Arr = ["A", "B", "C"] });
		}

	[Fact]
	public void CompareEntity()
		{
		Assert.Equal(0, new TestString() { Arr = ["A", "B"] }.CompareTo(new TestString() { Arr = ["A", "B"] }));
		Assert.Equal(0, new TestString() { Arr = [] }.CompareTo(new TestString() { Arr = [] }));

		Assert.True(new TestString() { Arr = ["A", "B"] }.CompareTo(new TestString() { Arr = ["A", "b"] }) > 0);
		Assert.True(new TestString() { Arr = ["A", "B"] }.CompareTo(null) > 0);
		Assert.True(new TestString() { Arr = ["A", "B"] }.CompareTo(new TestString() { Arr = ["A", "C"] }) < 0);
		Assert.True(new TestString() { Arr = ["A", "C"] }.CompareTo(new TestString() { Arr = ["A", "B"] }) > 0);
		Assert.True(new TestString() { Arr = ["A", "B"] }.CompareTo(new TestString() { Arr = ["A", "B", "C"] }) < 0);
		}

	[Fact]
	public void SequenceOfSequenceOrder()
		{
		TestChar[] secTestString =
			[
			new TestChar() { Arr = ['A', 'B'] },
			new TestChar() { Arr = ['A', 'C', 'D'] },
			new TestChar() { Arr = ['A', 'C', 'B'] },
			new TestChar() { Arr = ['A', 'B'] }
			];

		TestChar[] secOrdered = [.. secTestString.Order()];

		foreach (TestChar testString in secOrdered)
			{
			output.WriteLine(new String(testString.Arr));
			}

		Assert.Equal('B', secOrdered[1].Arr[1]);
		Assert.Equal('B', secOrdered[2].Arr[2]);
		Assert.Equal('D', secOrdered[3].Arr[2]);

		Assert.True(secOrdered.IsOrdered());
		}

	[Fact]
	public void FirstOrDefaultTuple()
		=> Assert.Equal((1, 0), 1.AsEnumerable().FirstOrDefaultTuple());

	[Fact]
	public void FirstOrDefaultTuple2()
		=> Assert.Equal((1, 2), EnumerableExtensions.AsEnumerable(1, 2, 3).FirstOrDefaultTuple());

	[Fact]
	public void CircularEnumerable()
		{
		int[] source = [1, 2, 3];
		int[] expected = [1, 2, 3, 1, 2, 3, 1];

		int[] actualEnumerable = EnumerableExtensions.AsEnumerable(1, 2, 3).ToCircularEnumerable(cancellationToken: TestContext.Current.CancellationToken).Take(expected.Length).ToArray();
		int[] actualEnumerator = source.ToCircularEnumerable(cancellationToken: TestContext.Current.CancellationToken).GetEnumerator().Take(expected.Length).ToArray();

		Assert.Equal(expected, actualEnumerable);
		Assert.Equal(expected, actualEnumerator);
		}


	[Fact]
	public void Join()
		=> Assert.Equal("A-B-C", ((string[]) ["A", "B", "C"]).Interleave("-").Join());

	[Fact]
	public void Join1()
		=> Assert.Equal("ABC", ((string[]) ["A", "B", "C"]).Join());

	[Fact]
	public void Join2()
		=> Assert.Equal("", ((string[]) []).Interleave("-").Join());

	[Fact]
	public void Join22()
		=> Assert.Equal("", ((string[]) [""]).Interleave("-").Join());

	[Fact]
	public void Join3()
		=> Assert.Equal("-", ((string[]) ["", ""]).Interleave("-").Join());

	[Fact]
	public void JoinLinesWithoutFinal()
		=> Assert.Equal("A\r\nB\r\nC", ((string[]) ["A", "B", "C"]).Interleave(Environment.NewLine).Join());

	[Fact]
	public void JoinLines()
		=> Assert.Equal("A\r\nB\r\nC\r\n", ((string[]) ["A", "B", "C"]).JoinLines());

	[Fact]
	public void Split0()
		=> Assert.Equal((string[]) [], "A-B-C".Split("-", 0));

	[Fact]
	public void Split00()
		=> Assert.Equal((string[]) [""], "".Split("-"));

	[Fact]
	public void Split()
		=> Assert.Equal((string[]) ["A", "B", "C"], "A-B-C".Split("-"));

	[Fact]
	public void Split1()
		=> Assert.Equal((string[]) ["A-B-C"], "A-B-C".Split("-", 1));

	[Fact]
	public void Split2()
		=> Assert.Equal((string[]) ["A", "B-C"], "A-B-C".Split("-", 2));

	[Fact]
	public void Split3()
		=> Assert.Equal((string[]) ["A", "B", "C"], "A-B-C".Split("-", 3));

	[Fact]
	public void Split4()
		=> Assert.Equal((string[]) ["A", "B", "C"], "A-B-C".Split("-", 4));

	[Fact]
	public void SortedListOrder()
		{
		SortedList<int, char> sortedList = new SortedList<int, char>
			{
				{ 3, 'C' },
				{ 1, 'A' },
				{ 2, 'B' }
			};

		Assert.True(sortedList.Values.SequenceEqual(['A', 'B', 'C']));
		}

	[Fact]
	public void DistinctBy()
		{
		(int i, char c)[] arr = [ (3, 'B'), (1, 'A'), (2, 'B') ];

		Assert.True(arr.DistinctBy(t => t.i).SequenceEqual([(3, 'B'), (1, 'A'), (2, 'B')]));
		Assert.True(arr.DistinctBy(t => t.c).SequenceEqual([(3, 'B'), (1, 'A')]));
		}

	[Fact]
	public void BoundedQueue()
		{
		BoundedQueue<int> queue = new BoundedQueue<int>(5);

		queue.Enqueue(1);
		queue.Enqueue(2);
		queue.Enqueue(3);

		Assert.Equal((int[])[1, 2, 3], queue);
		Assert.Equal((int[])[1, 2, 3], queue);

		int first = queue.Dequeue();

		Assert.Equal(1, first);

		queue.Enqueue(4);
		queue.Enqueue(5);
		queue.Enqueue(6);

		Assert.Equal((int[])[2, 3, 4, 5, 6], queue);

		queue.Enqueue(7);

		Assert.Equal((int[])[3, 4, 5, 6, 7], queue);

		first = queue.Dequeue();

		Assert.Equal(3, first);

		first = queue.Dequeue();

		Assert.Equal(4, first);

		Assert.Equal((int[])[5, 6, 7], queue);
		}

	[Fact]
	public void BoundedStack()
		{
		BoundedStack<int> queue = new BoundedStack<int>(5);

		queue.Push(1);
		queue.Push(2);
		queue.Push(3);

		Assert.Equal((int[])[1, 2, 3], queue);
		Assert.Equal((int[])[1, 2, 3], queue);

		int last = queue.Pop();

		Assert.Equal(3, last);

		queue.Push(4);
		queue.Push(5);
		queue.Push(6);

		Assert.Equal((int[])[1, 2, 4, 5, 6], queue);

		queue.Push(7);

		Assert.Equal((int[])[2, 4, 5, 6, 7], queue);

		last = queue.Pop();

		Assert.Equal(7, last);

		last = queue.Pop();

		Assert.Equal(6, last);

		Assert.Equal((int[])[2, 4, 5], queue);
		}

	[Fact]
	public void UniqueBoundedQueue()
		{
		UniqueBoundedQueue<int> queue = new UniqueBoundedQueue<int>(5);

		queue.Enqueue(1);
		queue.Enqueue(2);
		queue.Enqueue(3);
		queue.Enqueue(1);
		queue.Enqueue(2);
		queue.Enqueue(3);

		Assert.Equal((int[])[1, 2, 3], queue);
		Assert.Equal((int[])[1, 2, 3], queue);

		int first = queue.Dequeue();

		Assert.Equal(1, first);

		queue.Enqueue(4);
		queue.Enqueue(4);
		queue.Enqueue(5);
		queue.Enqueue(5);
		queue.Enqueue(6);
		queue.Enqueue(6);

		Assert.Equal((int[])[2, 3, 4, 5, 6], queue);

		queue.Enqueue(6);
		queue.Enqueue(6);
		queue.Enqueue(7);
		queue.Enqueue(6);
		queue.Enqueue(6);
		queue.Enqueue(7);
		queue.Enqueue(7);

		Assert.Equal((int[])[3, 4, 5, 6, 7], queue);

		first = queue.Dequeue();

		Assert.Equal(3, first);

		first = queue.Dequeue();

		Assert.Equal(4, first);

		Assert.Equal((int[])[5, 6, 7], queue);
		}


	[Fact]
	public void PreviousNextInt()
		{
		int[] nums = Enumerable.Range(1, 5).ToArray();

		Assert.Equal(0, nums.GetPreviousOrDefault(1));
		foreach (int n in Enumerable.Range(2, 3))
			{
			Assert.Equal(n - 1, nums.GetPrevious(n));
			Assert.Equal(n + 1, nums.GetNext(n));
			}
		Assert.Equal(0, nums.GetNextOrDefault(5));
		}

	[Fact]
	public void PreviousNextString()
		{
		string[] nums = Enumerable.Range(1, 5).Select(n => n.ToString()).ToArray();

		Assert.Null(nums.GetPreviousOrDefault("1"));
		foreach (int n in Enumerable.Range(2, 3))
			{
			Assert.Equal((n - 1).ToString(), nums.GetPrevious(n.ToString()));
			Assert.Equal((n + 1).ToString(), nums.GetNext(n.ToString()));
			}
		Assert.Null(nums.GetNextOrDefault("5"));
		}

	[Fact]
	public void MinEmpty()
		{
		IEnumerable<int> nums = [];

		Assert.Throws<InvalidOperationException>(() => nums.Min());
		}

	[Fact]
	public void MinMax()
		{
		IEnumerable<int> nums = [ 2, 1, 2, 4, 3 ];

		Assert.Equal((1, 4), nums.MinMax());
		}

	[Fact]
	public void MinMaxEmpty()
		{
		IEnumerable<int> nums = [];

		Assert.Throws<InvalidOperationException>(() => nums.MinMax());
		}
	
	[Fact]
	public void OrderBy()
		{
		(int, char)[] array = [(3, 'C'),(1, 'A'),(2, 'B')];

		Assert.True(array.Order().SequenceEqual([(1, 'A'), (2, 'B'), (3, 'C')]));
		}

	[Fact]
	public void OrderBy2()
		{
		(int, char)[] array = [ (3, 'C'), (1, 'C'), (1, 'A'), (2, 'B') ];

		Assert.True(array.Order().SequenceEqual([(1, 'A'), (1, 'C'), (2, 'B'), (3, 'C')]));
		}

	[Fact]
	public void RangesTypes()
		{
		IEnumerable<int> list = Enumerable.Range(0, 1);
        output.WriteLine(list.GetType().FullName ?? "");
        Assert.StartsWith("System.Linq.Enumerable+RangeIterator", list.GetType().FullName);

        IEnumerable<int> list2 = [ 0, 1 ];
		output.WriteLine(list2.GetType().FullName ?? "");
        Assert.Contains("ReadOnlyArray", list2.GetType().FullName);

        int[] list3 = [ 0, 1 ];
		IEnumerable<int> list4 = list3[..];        
		Assert.Equal(typeof(int[]), list3.GetType());
		Assert.Equal(typeof(int[]), list4.GetType());

		Memory<int> list5 = list3.AsMemory();
		Assert.Equal(typeof(Memory<int>), list5.GetType());

		//Span<int> list6 = list3.AsSpan()[..];        
		//Assert.Equal(typeof(Span<int>), list6.GetType());
		}

	[Fact]
	public void ArrayEnumerator1()
		{
		string[] array = [ "a", "b", "c" ];

		IEnumerator<string> enumerator = Enumerable.AsEnumerable(array).GetEnumerator();
		while (enumerator.MoveNext())
			{
			output.WriteLine(enumerator.Current);
			}

		// enumerator.Current is undefined here
		// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1.current?view=net-8.0#remarks
		Assert.Throws<InvalidOperationException>(() => enumerator.Current);
		}

	[Fact]
	public void ArrayEnumerator2()
		{
		string[] array = [ "a", "b", "c" ];

		IEnumerator<string> enumerator = ((IEnumerable<string>)array).GetEnumerator();

		while (enumerator.MoveNext())
			{
			output.WriteLine(enumerator.Current);
			}

		// enumerator.Current is undefined here
		// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1.current?view=net-8.0#remarks
		Assert.Throws<InvalidOperationException>(() => enumerator.Current);
		}
	
	[Fact]
	public void ArrayEnumerator3()
		{
		string[] array = [ "a", "b", "c" ];

		IEnumerator<string> enumerator = array.Select(s => s).GetEnumerator();

		while (enumerator.MoveNext())
			{
			output.WriteLine(enumerator.Current);
			}

		// enumerator.Current is undefined here
		// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1.current?view=net-8.0#remarks
		Assert.Null(enumerator.Current);
		}

	[Fact]
	public void ArrayEnumerator4()
		{
		string[] array = [ "a", "b", "c" ];
		StringValues stringValues = new StringValues(array);

		StringValues.Enumerator enumerator = stringValues.GetEnumerator();

		while (enumerator.MoveNext())
			{
			output.WriteLine(enumerator.Current ?? "");
			}

		// enumerator.Current is last item here
		Assert.Equal(array[^1], enumerator.Current);
		}
	}


using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Comparers;
using Xunit;

namespace CoreClassLibrary.Tests;

public class ComparerTests
    {
    [Fact]
    public void ReferenceEqual()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "A");

        Assert.Equal(r1, r1);
        Assert.Equal(r2, r2);
        Assert.NotEqual(r1, r2);
        }

    [Fact]
    public void FuncEqual()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "A");

        FuncEqualityComparer<ActionResponse> comparer = new FuncEqualityComparer<ActionResponse>((r1, r2) => r1?.Message.EqualsInvariantCulture(r2?.Message) ?? false, r => r.Message.GetHashCode());

        Assert.Equal(r1, r1, comparer);
        Assert.Equal(r2, r2, comparer);
        Assert.Equal(r1, r2, comparer);
        }

    [Fact]
    public void FuncEqualCompare()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "A");

        FuncComparer<ActionResponse> comparer = new FuncComparer<ActionResponse>((r1, r2) => r1?.Message.CompareInvariantCulture(r2?.Message) ?? -1);
        FuncEqualityComparer<ActionResponse> equalityComparer = new FuncEqualityComparer<ActionResponse>(comparer, r => r.Message.GetHashCode());

        Assert.Equal(r1, r1, equalityComparer);
        Assert.Equal(r2, r2, equalityComparer);
        Assert.Equal(r1, r2, equalityComparer);
        Assert.Equal(r2, r1, equalityComparer);

        Assert.Equal(0, comparer.Compare(r1, r1));
        Assert.Equal(0, comparer.Compare(r2, r2));
        Assert.Equal(0, comparer.Compare(r1, r2));
        Assert.Equal(0, comparer.Compare(r2, r1));
       
        }

    [Fact]
    public void FuncNotEqualCompare()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "B");

        FuncComparer<ActionResponse> comparer = new FuncComparer<ActionResponse>((r1, r2) => r1?.Message.CompareInvariantCulture(r2?.Message) ?? -1);
        FuncEqualityComparer<ActionResponse> equalityComparer = new FuncEqualityComparer<ActionResponse>(comparer, r => r.Message.GetHashCode());

        Assert.NotEqual(r1, r2, equalityComparer);
        Assert.NotEqual(r2, r1, equalityComparer);

        Assert.NotEqual(0, comparer.Compare(r1, r2));
        Assert.NotEqual(0, comparer.Compare(r2, r1));
        }

    [Fact]
    public void FuncCompare()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "B");

        FuncComparer<ActionResponse> comparer = new FuncComparer<ActionResponse>((r1, r2) => r1?.Message.CompareInvariantCulture(r2?.Message) ?? -1);

        Assert.Equal(-1, comparer.Compare(r1, r2));
        Assert.Equal(1, comparer.Compare(r2, r1));
        }

    [Fact]
    public void SelectEqual()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "A");

        SelectEqualityComparer<ActionResponse, string> comparer = new SelectEqualityComparer<ActionResponse, string>(r => r?.Message);

        Assert.Equal(r1, r1, comparer);
        Assert.Equal(r2, r2, comparer);
        Assert.Equal(r1, r2, comparer);
        }

    [Fact]
    public void SelectEqualCompare()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "A");

        SelectComparer<ActionResponse, string> comparer = new SelectComparer<ActionResponse, string>(r => r?.Message);
        FuncEqualityComparer<ActionResponse> equalityComparer = new FuncEqualityComparer<ActionResponse>(comparer, r => r.Message.GetHashCode());

        Assert.Equal(r1, r1, equalityComparer);
        Assert.Equal(r2, r2, equalityComparer);
        Assert.Equal(r1, r2, equalityComparer);
        Assert.Equal(r2, r1, equalityComparer);

        Assert.Equal(0, comparer.Compare(r1, r1));
        Assert.Equal(0, comparer.Compare(r2, r2));
        Assert.Equal(0, comparer.Compare(r1, r2));
        Assert.Equal(0, comparer.Compare(r2, r1));
       
        }

    [Fact]
    public void SelectNotEqualCompare()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "B");

        SelectComparer<ActionResponse, string> comparer = new SelectComparer<ActionResponse, string>(r => r?.Message);
        FuncEqualityComparer<ActionResponse> equalityComparer = new FuncEqualityComparer<ActionResponse>(comparer, r => r.Message.GetHashCode());

        Assert.NotEqual(r1, r2, equalityComparer);
        Assert.NotEqual(r2, r1, equalityComparer);

        Assert.NotEqual(0, comparer.Compare(r1, r2));
        Assert.NotEqual(0, comparer.Compare(r2, r1));
        }

    [Fact]
    public void SelectCompare()
        {
        ActionResponse r1 = new ActionResponse(true, "A");
        ActionResponse r2 = new ActionResponse(true, "B");

        SelectComparer<ActionResponse, string> comparer = new SelectComparer<ActionResponse, string>(r => r?.Message);

        Assert.Equal(-1, comparer.Compare(r1, r2));
        Assert.Equal(1, comparer.Compare(r2, r1));
        }
    }


using CoreClassLibrary.Objects.Scopes;
using Xunit;

namespace CoreClassLibrary.Tests;

public class ScopesTests
    {
    public record ScopeValue(string Value);

    private const string ScopedValue = "ScopedValue";
    private const string NestedScopedValue = "NestedScopedValue";

    [Fact]
    public void SimpleScope()
        {
        using (new Scope<ScopeValue>(new ScopeValue(ScopesTests.ScopedValue)))
            {
            Assert.Equal(ScopesTests.ScopedValue, Scope<ScopeValue>.Current?.Value);
            }

        Assert.Null(Scope<ScopeValue>.Current);
        }

    [Fact]
    public void NotNestedScope()
        {
        using (new Scope<ScopeValue>(new ScopeValue(ScopesTests.ScopedValue)))
            {
            Assert.Equal(ScopesTests.ScopedValue, Scope<ScopeValue>.Current?.Value);

            try
                {
                using (new Scope<ScopeValue>(new ScopeValue(ScopesTests.NestedScopedValue)))
                    {
                    Assert.Equal(ScopesTests.NestedScopedValue, Scope<ScopeValue>.Current?.Value);
                    }

                Assert.Fail("Expected InvalidOperationException was not thrown.");
                }
            catch (InvalidOperationException) { }
            }
        Assert.Null(Scope<ScopeValue>.Current);
        }

    [Fact]
    public void NestedScope()
        {
        using (new NestedScope<ScopeValue>(new ScopeValue(ScopesTests.ScopedValue)))
            {
            Assert.Equal(ScopesTests.ScopedValue, NestedScope<ScopeValue>.Current?.Value);

            using (new NestedScope<ScopeValue>(new ScopeValue(ScopesTests.NestedScopedValue)))
                {
                Assert.Equal(ScopesTests.NestedScopedValue, NestedScope<ScopeValue>.Current?.Value);
                }

            Assert.Equal(ScopesTests.ScopedValue, NestedScope<ScopeValue>.Current?.Value);
            }

        Assert.Null(Scope<ScopeValue>.Current);
        }
    }


using CoreClassLibrary.Exceptions.Domain;
using CoreClassLibrary.Validation;
using Xunit;

namespace CoreClassLibrary.Tests;

public class ValidationTests(ITestOutputHelper output)
    {
    [Fact]
    public void Valid()
        {
        new ValidatableDTO().Validate();
        }

    [Fact]
    public void ThrowsDomainException()
        {
        Assert.Throws<DomainException>(() => ValidatableDTO.CreateInvalid().Validate());
        }

    [Fact]
    public void ThrowsBool()
        {
        Assert.Throws<DomainException>(() => new ValidatableDTO() { IsValid = false }.Validate());
        }

    [Fact]
    public void ThrowFixStringLength()
        {
        Assert.Throws<DomainException>(() => new ValidatableDTO() { Branch = "0" }.Validate());
        }

    [Fact]
    public void TryCatchException()
        {
        try
            {
            ValidatableDTO.CreateInvalid().Validate();
            Assert.Fail();
            }
        catch (DomainException ex)
            {
            Assert.Equal(6, ex.ValidationResults.Count);
            output.Write(ex);
            }
        }

    [Fact]
    public void TryCatchExceptionTest2()
        {
        try
            {
            new ValidatableDTO() { Number = -1 }.Validate();
            Assert.Fail();
            }
        catch (DomainException ex)
            {
            Assert.Single(ex.ValidationResults);
            output.Write(ex);
            }
        }

    [Fact]
    public void NonRecursiveValidation()
        {
        RecursiveValidatableDTO.CreateInvalid().Validate();
        }

    [Fact]
    public void RecursiveValidation()
        {
        Assert.Throws<DomainException>(() => RecursiveValidatableDTO.CreateInvalid().RecursiveValidate());
        }

    [Fact]
    public void RecursiveTryCatchAllException()
        {
        try
            {
            RecursiveValidatableDTO.CreateInvalid().RecursiveValidate();
            Assert.Fail();
            }
        catch (DomainException ex)
            {
            Assert.Equal(6, ex.ValidationResults.Count);
            output.Write(ex);
            }
        }
    }

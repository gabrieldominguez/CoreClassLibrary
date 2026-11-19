
using CoreClassLibrary.Validation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CoreClassLibrary.Tests;

public class ValidatableDTO
    {
    public static ValidatableDTO CreateInvalid()
        => new ValidatableDTO() { Description = "", Number = -1, IsValid = false, Branch = "0", Multiple = 0 };

    [Required]  // AllowEmptyStrings = false
    public string Description { get; set; } = "A";

    [Minimum(0)]
    public int Number { get; set; } = 0;

    [Equals(true)]
    public bool IsValid { get; set; } = true;

    [FixedLengthString(3)]
    public string Branch { get; set; } = "000";

    [Minimum(3)]
    [Range(2, 5)]
    public int Multiple { get; set; } = 4;
    }

public class RecursiveValidatableDTO
    {
    public static RecursiveValidatableDTO CreateInvalid()
        => new RecursiveValidatableDTO() { ValidatableData = ValidatableDTO.CreateInvalid() };

    public ValidatableDTO ValidatableData { get; set; } = new ValidatableDTO();
    }
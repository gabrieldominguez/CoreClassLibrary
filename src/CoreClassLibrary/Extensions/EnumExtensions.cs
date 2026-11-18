namespace CoreClassLibrary.Extensions;

/// <summary>
///     Permite setear, borrar o chequear un bit de la bandera.
/// </summary>
/// <example>
///         [Flags]
///         public enum ErrorTypes
///             {
///             None = 0,
///             MissingPassword = 1,
///             MissingUsername = 2,
///             PasswordIncorrect = 4
///             }
///
///         [TestMethod]
/// 		public void CanAppendRemoveAndConsultFlagEnum()
/// 		    {
/// 			ErrorTypes error = ErrorTypes.MissingPassword;
/// 			Assert.IsTrue(error.Has(ErrorTypes.MissingPassword));
/// 			error = error.Append(ErrorTypes.MissingUsername);
/// 			Assert.IsTrue(error.Has(ErrorTypes.MissingPassword));
/// 			Assert.IsTrue(error.Has(ErrorTypes.MissingUsername));
/// 			Assert.IsTrue(error.Has(ErrorTypes.MissingPassword.Append(ErrorTypes.MissingUsername)));
/// 			error = error.Remove(ErrorTypes.MissingPassword);
/// 			Assert.IsFalse(error.Has(ErrorTypes.MissingPassword));
/// 			Assert.IsTrue(error.Has(ErrorTypes.MissingUsername));
/// 		    }
/// </example>
public static class EnumExtensions
    {
    #region Flags
    public static TEnum Append<TEnum>(this Enum type, TEnum value) where TEnum : struct
        => (TEnum)(object)((int)(object)type | (int)(object)value);

    public static TEnum Remove<TEnum>(this Enum type, TEnum value) where TEnum : struct
        => (TEnum)(object)((int)(object)type & ~(int)(object)value);

    public static bool Has<TEnum>(this Enum type, TEnum value) where TEnum : struct
        => ((int)(object)type & (int)(object)value) == (int)(object)value;
    #endregion

    #region Parsing
    public static TEnum Parse<TEnum>(string value) where TEnum : struct
        => (TEnum)Enum.Parse(typeof(TEnum), value, true);

    public static TEnum TryParse<TEnum>(string value) where TEnum : struct
        {
        Enum.TryParse(value, out TEnum result);
        return result;
        }
    #endregion
    }

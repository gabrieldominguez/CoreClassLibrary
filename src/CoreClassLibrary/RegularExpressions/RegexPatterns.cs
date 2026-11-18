namespace CoreClassLibrary.RegularExpressions;

/// <summary>
/// C# Regular Expression Recipes—Using Common Patterns
/// </summary>
/// <see cref="http://tutorials.csharp-online.net/CSharp_Regular_Expression_Recipes%E2%80%94Using_Common_Patterns"/>
public static class RegexPatterns
{
    // We allow all Unicode letter characters as well as internal spaces and hypens, as long as these do not occur in sequences.
    public const string PersonName = @"\A\p{L}+([\p{Zs}\-][\p{L}]+)*\z";

    // We allow all Unicode letter and numeric characters as well as internal spaces, as long as these do not occur in sequences.
    public const string PersonAddress = @"\A[\p{L}\p{N}]+([\p{Zs}][\p{L}\p{N}]+)*\z";

    // We allow all Unicode numeric characters and hypens, as long as these do not occur in sequences.
    public const string Number = @"\A\p{N}+([\p{N}\-][\p{N}]+)*\z";

    public const string IpAddress = @"^[0-9]{1,3}\.){3}[0-9]{1,3}$";

    public const string DomainName = @"^[\-\w]+\.)+[a-zA-Z]{2,4}$";

    //  Verify that an email address is in the form name@address where address is not an IP address
    public const string EmailAddress = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

    public const string URL = @"((^http(s)*://(([\-\w]+\.)+[a-zA-Z]{2,4}.*)))$|(^ftp://([\w](:[\w]))*(([\-\w]+\.)+[a-zA-Z]{2,4}[/\w]*))$";

    // Match only a dollar amount with the optional $ and + or - preceding characters (note that any number of decimal places may be added)
    public const string Money = @"^\$?[+-]?[\d,]*(\.\d*)?$";

    // This is similar to the previous regular expression except that only up to two decimal places are allowed
    public const string Money2Dec = @"^\$?[+-]?[\d,]*\.?\d{0,2}$";

    // Match a credit card number to be entered as four sets of four digits separated with a space, -, or no character at all
    public const string CreditCardNumber = @"^((\d{4}[- ]?){3}\d{4})$";

    public const string LocalPhoneNumber = @"^(\(\d+\))?[\d\s\-]+$";

    // Match a full path beginning with the drive letter and optionally match a filename with a three-character extension (note that no .. characters signifying to move up the directory hierarchy are allowed, nor is a directory name with a . followed by an extension):
    public const string FullPath = @"^[a-zA-Z]:[\\/]([_a-zA-Z0-9]+[\\/]?)*([_a-zA-Z0-9]+\.[_a-zA-Z0-9]{0,3})?$";
}

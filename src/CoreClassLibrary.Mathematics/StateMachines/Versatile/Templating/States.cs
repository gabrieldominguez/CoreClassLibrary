
namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Templating;

//public enum States { InText, StartKey, InKey}

public static class States
    {
    public const int InText = 0;
    public const int StartKey = 1;
    public const int InKey = 2;

    public readonly static string[] Labels = ["InText", "StartKey", "InKey"];
    }

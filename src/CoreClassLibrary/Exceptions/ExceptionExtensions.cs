using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Exceptions;

public static class ExceptionExtensions
    {
    public static string JoinInnerMessages(this Exception ex)
        => ex.GetInnerMessages().Interleave(" ---> ").Join();

    public static IEnumerable<string> GetInnerMessages(this Exception ex)
        => ex.Message.AsEnumerable().Concat(ex.InnerException?.GetInnerMessages() ?? []);
    }

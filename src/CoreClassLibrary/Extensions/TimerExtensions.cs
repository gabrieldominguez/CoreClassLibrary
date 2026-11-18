namespace CoreClassLibrary.Extensions;

public static class TimerExtensions
    {
    public static void Start(this ITimer timer, TimeSpan period) => timer.Change(period);

    public static void Stop(this ITimer timer) => timer.Change(Timeout.InfiniteTimeSpan);

    private static void Change(this ITimer timer, TimeSpan time) => timer.Change(time, time);
    }

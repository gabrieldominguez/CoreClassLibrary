namespace CoreClassLibrary.Mathematics.IdGenerators;

public class UtcNowIdGenerator: IIdGenerator<long>
    {
    public long NextId() => DateTime.UtcNow.Ticks;
    }

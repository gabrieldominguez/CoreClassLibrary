namespace CoreClassLibrary.Mathematics.IdGenerators;

public class GuidIdGenerator: IIdGenerator<Guid>
    {
    public Guid NextId() => Guid.NewGuid();
    }

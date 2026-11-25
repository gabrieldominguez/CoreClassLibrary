namespace CoreClassLibrary.Mathematics.IdGenerators;

/// <summary>
/// Constructor
/// </summary>
/// <param name="format">
/// D: 94ad76b2-450d-4878-9946-c5efaf5efd7c
/// M: 94ad76b2450d48789946c5efaf5efd7c
/// B: {94ad76b2-450d-4878-9946-c5efaf5efd7c}
/// P: (94ad76b2-450d-4878-9946-c5efaf5efd7c)
/// X: {0x94ad76b2,0x450d,0x4878,{0x99,0x46,0xc5,0xef,0xaf,0x5e,0xfd,0x7c}}
/// </param>
public class FormattedGuidIdGenerator(IIdGenerator<Guid> guidGenerator, string format = "") : IIdGenerator<string>
    {
    public string NextId()
        => guidGenerator.NextId().ToString(format);
    }

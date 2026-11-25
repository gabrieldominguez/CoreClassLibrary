namespace CoreClassLibrary.Mathematics.IdGenerators;

/// <summary>
/// This algorithm generates secuential GUIDs, ideal for databases cluster keys.
/// </summary>
/// <remarks>
/// Generates sequential <see cref="Guid" /> values optimized for use in Microsoft SQL server clustered
/// keys or indexes, yielding better performance than random values. This is the default generator for
/// SQL Server <see cref="Guid" /> columns which are set to be generated on add.
///     
/// Although this generator achieves the same goals as SQL Server's NEWSEQUENTIALID, the algorithm used
/// to generate the GUIDs is different.
///         
/// The last 8 bytes are based on the parameter counter.
/// </remarks>
/// <see cref="https://github.com/dotnet/efcore/blob/main/src/EFCore/ValueGeneration/SequentialGuidValueGenerator.cs"/>
/// <see href="https://docs.microsoft.com/sql/t-sql/functions/newsequentialid-transact-sql">NEWSEQUENTIALID</see>
/// <see cref="https://blogs.msdn.microsoft.com/oldnewthing/20080627-00/?p=21823"/>
/// <see cref="http://stackoverflow.com/questions/1752004/sequential-guid-generator-c-sharp"/>
/// <seealso cref="http://www.ietf.org/rfc/rfc4122.txt"/>
/// <seealso cref="https://en.wikipedia.org/wiki/Universally_unique_identifier"/>
/// <seealso cref="https://www.uuidgenerator.net/"/>
/// <seealso cref="http://developmenttips.blogspot.com.ar/2008/03/generate-sequential-guids-for-sql.html"/>
/// <seealso cref="https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/guid.cs#L892"/>
/// <seealso cref="https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Guid.cs#L730"/>
public class SequentialGuidIdGenerator(Guid baseGuid, IIdGenerator<long> longGenerator) : IIdGenerator<Guid>
    {
    public SequentialGuidIdGenerator(IIdGenerator<long> longGenerator): this(new Guid(), longGenerator) { }

    public Guid NextId()
        => SequentialGuidIdGenerator.EncodeId(baseGuid, longGenerator.NextId());

    public static Guid NewGuid()
        => SequentialGuidIdGenerator.EncodeId(DateTime.UtcNow.Ticks);

    public static Guid EncodeId(long id)
        => SequentialGuidIdGenerator.EncodeId(Guid.NewGuid(), id);

    public static Guid EncodeId(Guid guid, long id)
        => SequentialGuidIdGenerator.EncodeId(guid, BitConverter.GetBytes(id));

    public static Guid EncodeId(Guid baseGuid, byte[] id)
        {
        if (!BitConverter.IsLittleEndian)
            {
            Array.Reverse(id);
            }

        // Los primeros 8 bytes son los menos significativos, por lo que son aleatorios (se usan los de un guid normal)
        // Los últimos 8 bytes son los más significativos, por los que se usan los del id en el orden especificado
      
        byte[] sequentialGuid = baseGuid.ToByteArray();

        sequentialGuid[9] = id[0];
        sequentialGuid[8] = id[1];
        sequentialGuid[15] = id[2];
        sequentialGuid[14] = id[3];
        sequentialGuid[13] = id[4];
        sequentialGuid[12] = id[5];
        sequentialGuid[11] = id[6];
        sequentialGuid[10] = id[7];

        return new Guid(sequentialGuid);
        }
    }


namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

public class BlockEngine(IBlockEngine blockEngine) : IEngine
    {
    public void Initialize() => blockEngine.Initialize();

    public byte[] Process(byte[] input, int offset, int count)
        {
        byte[] output = new byte[count];

        for (int i = 0; i < count; i++)
            {
            blockEngine.ProcessBlock(input, offset, output, 0);
            }

        return output;
        }
    }

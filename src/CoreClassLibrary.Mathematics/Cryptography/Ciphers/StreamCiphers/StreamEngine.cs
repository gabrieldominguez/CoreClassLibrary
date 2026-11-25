
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;

public class StreamEngine(IStreamEngine streamEngine) : IEngine
    {
    public void Initialize() => streamEngine.Initialize();

    public byte[] Process(byte[] input, int offset, int count)
        {
        byte[] output = new byte[count];

        for (int i = 0; i < count; i++)
            {
            output[i] = streamEngine.ProcessByte(input[offset + i]);
            }

        return output;
        }
    }


using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers;

public class CipherBase(byte[] key)
    {
    protected readonly byte[] key = key.CloneArray();

    public virtual void Initialize() { }
    }

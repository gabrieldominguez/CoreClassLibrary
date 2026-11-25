
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Cryptography.Hashing;
using CoreClassLibrary.Mathematics.Cryptography.Hashing.Algorithms;

namespace CoreClassLibrary.Mathematics.Cryptography;

/// <summary>
/// Namespace and name-based (Versions 3 and 5) guid generator.
/// </summary>
/// <see cref="http://www.ietf.org/rfc/rfc4122.txt"/>
/// <see cref="https://en.wikipedia.org/wiki/Universally_unique_identifier#Versions_3_and_5_(namespace_name-based)"/>
/// <see cref="https://www.famkruithof.net/guid-uuid-namebased.html"/>
/// <see cref="https://github.com/Informatievlaanderen/deterministic-guid-generator/blob/main/src/Be.Vlaanderen.Basisregisters.Generators.Guid.Deterministic/GuidUtility.cs"/>
/// <see cref="https://stackoverflow.com/questions/10867405/generating-v5-uuid-what-is-name-and-namespace"/>
/// <see cref="https://stackoverflow.com/questions/2190890/how-can-i-generate-a-guid-for-a-string"/>
/// <seealso cref="https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/guid.cs#L892"/>
/// <seealso cref="https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Guid.cs#L730"/>
/// <seealso cref="https://stackoverflow.com/questions/417108/why-are-there-dashes-in-a-net-guid/3557678#3557678"/>
public static class NamespaceGuid
    {
    /// <summary>
    /// Predefined namespace to generate deterministic <see cref="Guid"/>.
    /// </summary>
    public static class Namespaces
        {
        /// <summary>
        /// The namespace for fully-qualified domain names (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid Dns = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

        /// <summary>
        /// The namespace for URLs (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid Url = new Guid("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

        /// <summary>
        /// The namespace for ISO OIDs (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid IsoOid = new Guid("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

        /// <summary>
        /// The namespace for X.500 DN (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid X500Dn = new Guid("6ba7b814-9dad-11d1-80b4-00c04fd430c8");
        }

    public static Guid Create(string name, int version = 5)
        => NamespaceGuid.Create(NamespaceGuid.Namespaces.IsoOid, name, version);

    /// <summary>
    /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
    /// </summary>
    /// <param name="namespaceId">The ID of the namespace.</param>
    /// <param name="name">The name (within that namespace).</param>
    /// <param name="version">The version number of the UUID to create; this value must be either 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
    /// <returns>A UUID derived from the namespace and name.</returns>
    public static Guid Create(Guid namespaceId, string name, int version = 5)
        {
        //using (HashAlgorithm algorithm = version == 3 ? (HashAlgorithm)MD5.Create() : SHA1.Create())
        IHashAlgorithm algorithm = version switch
            {
            3 => new MD5Hash(),
            5 => new SHA1Hash(),
            _ => throw new ArgumentOutOfRangeException(nameof(version), "must be either 3 or 5.")
            };

        // convert the name to a sequence of octets (as defined by the standard or conventions of its namespace) (step 3)
        // ASSUME: UTF-8 encoding is always appropriate
        byte[] nameBytes = StringExtensions.UTF8Encoding.GetBytes(name);

        // convert the namespace UUID to network order (step 3)
        byte[] namespaceBytes = namespaceId.ToByteArray();
        NamespaceGuid.SwapByteOrder(namespaceBytes);

        // compute the hash of the name space ID concatenated with the name (step 4)            
        byte[] combinedBytes = new byte[namespaceBytes.Length + nameBytes.Length];
        Buffer.BlockCopy(namespaceBytes, 0, combinedBytes, 0, namespaceBytes.Length);
        Buffer.BlockCopy(nameBytes, 0, combinedBytes, namespaceBytes.Length, nameBytes.Length);

        byte[] hash = algorithm.ComputeHash(combinedBytes);

        // most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
        byte[] newGuid = new byte[16];
        Array.Copy(hash, 0, newGuid, 0, 16);

        // set the four most significant bits (bits 12 through 15) of the time_hi_and_version
        // field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
        newGuid[6] = (byte)((newGuid[6] & 0x0F) | (version << 4));

        // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved
        // to zero and one, respectively (step 10)
        newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

        // convert the resulting UUID to local byte order (step 13)
        NamespaceGuid.SwapByteOrder(newGuid);

        return new Guid(newGuid);
        }

    // Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
    private static void SwapByteOrder(byte[] guid)
        {
        ObjectExtensions.Swap(ref guid[0], ref guid[3]);
        ObjectExtensions.Swap(ref guid[1], ref guid[2]);
        ObjectExtensions.Swap(ref guid[4], ref guid[5]);
        ObjectExtensions.Swap(ref guid[6], ref guid[7]);
        }
    }

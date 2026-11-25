
using CoreClassLibrary.Mathematics.Cryptography.Hashing;
using CoreClassLibrary.Mathematics.Cryptography.Hashing.Algorithms;
using CoreClassLibrary.Extensions;
using System.Security.Cryptography;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

/// <summary>
/// Test cases are from
///     SHA1 y SHA256       http://csrc.nist.gov/publications/fips/fips180-2/fips180-2.pdf Appendix B
///                         https://msdn.microsoft.com/es-es/magazine/mt845650
///
///     SHA384 y SHA512  http://csrc.nist.gov/groups/ST/toolkit/documents/Examples/SHA_All.pdf
///     MD5              https://tools.ietf.org/html/rfc1321 section A.5
///     RIPEMD160        http://homes.esat.kuleuven.be/~bosselae/ripemd160.html.
///     MurMur3          https://github.com/aappleby/smhasher/issues/6
///
///     HMAC             https://en.wikipedia.org/wiki/Hash-based_message_authentication_code
///     HMAC-SHA1 y MD5             https://tools.ietf.org/html/rfc2202
///     HMAC-SHA256, 384, 512       https://tools.ietf.org/html/rfc4231.
///     HMAC-RIPE                   https://tools.ietf.org/html/rfc2286
///
///     RIPEMD160, SHA-256 y doble hash    https://en.bitcoin.it/wiki/Protocol_documentation#Hashes
///
/// </summary>
/// <see cref="http://onlinemd5.com/"/>
/// <see cref="https://github.com/aappleby/smhasher/wiki/SMHasher"/>
public class HashTests(ITestOutputHelper output)
    {

    #region Hashs
    private void HashAlgorithmString(IHashAlgorithm hashAlgorithm, string data, string expectedHash)
        {
        output.WriteLine("string:");
        output.WriteLine(data);
        this.HashAlgorithm(hashAlgorithm, data.Select(c => (byte)c).ToHEXString(), expectedHash);
        }

    private void HashAlgorithm(IHashAlgorithm hashAlgorithm, string data, string expectedHash)
        {
        byte[] expHash = expectedHash.FromHexString();
        byte[] hash = hashAlgorithm.ComputeHash(data.FromHexString());

        output.WriteLine("HashAlgorithm TEST:");
        output.WriteLine("data:");
        output.WriteLine(data);
        output.WriteLine("expected:");
        output.WriteLine(expHash.ToHexString());
        output.WriteLine("");
        output.WriteLine("actual:");
        output.WriteLine(hash.ToHexString());

        Assert.Equal(expHash, hash);

        byte[] hash2 = this.HashAlgorithm(new HashAlgorithmAdapter(hashAlgorithm), data, expectedHash);

        Assert.Equal(hash, hash2);
        }

    private byte[] HashAlgorithm(HashAlgorithm hashAlgorithm, string data, string expectedHash)
        {
        byte[] expHash = expectedHash.FromHexString();
        byte[] hash = hashAlgorithm.ComputeHash(data.FromHexString());

        output.WriteLine("System HashAlgorithm TEST:");
        output.WriteLine("data:");
        output.WriteLine(data);
        output.WriteLine("expected:");
        output.WriteLine(expHash.ToHexString());
        output.WriteLine("");
        output.WriteLine("actual:");
        output.WriteLine(hash.ToHexString());

        Assert.Equal(expHash, hash);

        return hash;
        }

    [Theory]
    [InlineData("616263", "A9993E364706816ABA3E25717850C26C9CD0D89D")]
    [InlineData("6162636462636465636465666465666765666768666768696768696a68696a6b696a6b6c6a6b6c6d6b6c6d6e6c6d6e6f6d6e6f706e6f7071", "84983E441C3BD26EBAAE4AA1F95129E5E54670F1")]
    public void SHA1_Test1(string data, string expectedHash) => this.HashAlgorithm(new SHA1Hash(), data, expectedHash);

    [Fact]
    public void SHA1_Test2()
        {
        this.HashAlgorithm(new SHA1Hash(), StringBuilderExtensions.RepeatAppend("61", 1000000), "34AA973CD4C4DAA4F61EEB2BDBAD27316534016F");
        this.HashAlgorithm(new SHA1Hash(), StringBuilderExtensions.RepeatAppend("3031323334353637", 80), "DEA356A2CDDD90C7A7ECEDC5EBB563934F460452");
        }

    [Fact]
    public void SHA1_Test3()
        {
        SHA1Hash sha1 = new SHA1Hash();
        this.HashAlgorithm(sha1, StringBuilderExtensions.RepeatAppend("61", 1000000), "34AA973CD4C4DAA4F61EEB2BDBAD27316534016F");
        this.HashAlgorithm(sha1, StringBuilderExtensions.RepeatAppend("61", 1000000), "34AA973CD4C4DAA4F61EEB2BDBAD27316534016F");
        this.HashAlgorithm(sha1, StringBuilderExtensions.RepeatAppend("3031323334353637", 80), "DEA356A2CDDD90C7A7ECEDC5EBB563934F460452");
        }

    [Theory]
    [InlineData("616263", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData("6162636462636465636465666465666765666768666768696768696a68696a6b696a6b6c6a6b6c6d6b6c6d6e6c6d6e6f6d6e6f706e6f7071", "248d6a61d20638b8e5c026930c3e6039a33ce45964ff2167f6ecedd419db06c1")]
    [InlineData("2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824", "9595c9df90075148eb06860365df33584b75bff782a510c6cd4883a419833d50")]
    [InlineData("68656C6C6F", "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824")]
    public void SHA256_Test1(string data, string expectedHash) => this.HashAlgorithm(new SHA256Hash(), data, expectedHash);

    [Fact]
    public void SHA256_Test2() => this.HashAlgorithm(new SHA256Hash(), StringBuilderExtensions.RepeatAppend("61", 1000000), "cdc76e5c9914fb9281a1c7e284d73e67f1809a48a497200e046d39ccc7112cd0");

    [Theory]
    [InlineData("62C66A7A5DD70C3146618063C344E531E6D4B59E379808443CE962B3ABD63C5A", "A5FCE7E78734EC317F83F9019C80FFAF2508689B06EFA02191495A7D21FECE9A")]
    [InlineData("08F271887CE94707DA822D5263BAE19D5519CB3614E0DAEDC4C7CE5DAB7473F1", "6F6DCF58526B0D29EE664A708A939B7CDAC124A6A8569FCACE46FEAD38868E2E")]
    [InlineData("2D214CA69B86C255BE416D42CCA977A59B34A7492873105522C35015FAB806F0", "6C5D08BE9FFBBABD24B5F19AFFE6590FD402D347A50B519A59D40E15DCC0A6CB")]
    [InlineData("0892A10ECE1F933EE98F5D554601B28F8437801D1AA1B77799E4035DDCB6950C", "B2307311CC5877D5A581EDC821F3BFD5F99EB4E3B1D1B4009D9545BCF07E2E1A")]
    [InlineData("9D95A2CF0D7180B5089691163B188A7203B0CDE179346B8CFAA8AB6C2C3E6414", "B5410E155022AE6EB22CA21FADEDE65F0F7296DE14CA1D7A720A4937BD23AA5D")]
    [InlineData("767328E7367048FA9DB37354CFA43DBB1691E8330DB54D54F52C1A444CA2E680", "345DD725FEE80F8C5953A66C1495605E4ED01C4CE5AEF6C0A6D238999266A1A6")]
    [InlineData("CCF33BF1C08B74EDE6A7C15C56EEC16269D83967670032ACDA6EE395361B7595", "3E85B3D910BA77F88ECD5E24D1396457C532C73B89C032DED9AD0CBB4D4D9794")]
    [InlineData("m", "62C66A7A5DD70C3146618063C344E531E6D4B59E379808443CE962B3ABD63C5A")]
    [InlineData("M", "08F271887CE94707DA822D5263BAE19D5519CB3614E0DAEDC4C7CE5DAB7473F1")]
    [InlineData("M1", "2D214CA69B86C255BE416D42CCA977A59B34A7492873105522C35015FAB806F0")]
    [InlineData("M2", "0892A10ECE1F933EE98F5D554601B28F8437801D1AA1B77799E4035DDCB6950C")]
    [InlineData("March", "9D95A2CF0D7180B5089691163B188A7203B0CDE179346B8CFAA8AB6C2C3E6414")]
    [InlineData("March 1, 2018", "767328E7367048FA9DB37354CFA43DBB1691E8330DB54D54F52C1A444CA2E680")]
    [InlineData("March 2, 2018", "CCF33BF1C08B74EDE6A7C15C56EEC16269D83967670032ACDA6EE395361B7595")]
    [InlineData("hello", "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824")]
    public void SHA256_String(string str, string expectedHash) => this.HashAlgorithm(new SHA256Hash(), str.Select(c => (byte) c).ToHEXString(), expectedHash);


    [Theory]
    // Erroneos en el articulo https://msdn.microsoft.com/es-es/magazine/mt845650, 
    // ver mi comentario en https://social.msdn.microsoft.com/forums/en-us/33740a5c-c29c-4d4b-9334-272fcea28e1f/blockchain-blockchain-fundamentals?forum=msdnmagazine
    //[InlineData("m", "A5FCE7E78734EC317F83F9019C80FFAF2508689B06EFA02191495A7D21FECE9A")]
    //[InlineData("M", "6F6DCF58526B0D29EE664A708A939B7CDAC124A6A8569FCACE46FEAD38868E2E")]
    //[InlineData("M1", "6C5D08BE9FFBBABD24B5F19AFFE6590FD402D347A50B519A59D40E15DCC0A6CB")]
    //[InlineData("M2", "B2307311CC5877D5A581EDC821F3BFD5F99EB4E3B1D1B4009D9545BCF07E2E1A")]
    //[InlineData("March", "B5410E155022AE6EB22CA21FADEDE65F0F7296DE14CA1D7A720A4937BD23AA5D")]
    //[InlineData("March 1, 2018", "345DD725FEE80F8C5953A66C1495605E4ED01C4CE5AEF6C0A6D238999266A1A6")]
    //[InlineData("March 2, 2018", "3E85B3D910BA77F88ECD5E24D1396457C532C73B89C032DED9AD0CBB4D4D9794")]
    [InlineData("hello", "9595c9df90075148eb06860365df33584b75bff782a510c6cd4883a419833d50")]
    public void DoubleSHA256_String(string str, string expectedHash) => this.HashAlgorithm(
            new DoubleSHA256HashAlgorithm(),
            str.Select(c => (byte) c).ToHEXString(), expectedHash);

    [Theory]
    [InlineData("hello", "b6a9c8c230722b7c748331a8b450f05566dc7d0f")]
    public void DoubleHashing_String(string str, string expectedHash) => this.HashAlgorithm(
            new DoubleHashAlgorithm(
                new SHA256Hash(),
                new RIPEMD160Hash()
                ),
            str.Select(c => (byte) c).ToHEXString(), expectedHash);

    [Theory]
    [InlineData("616263", "CB00753F45A35E8BB5A03D699AC65007272C32AB0EDED1631A8B605A43FF5BED8086072BA1E7CC2358BAECA134C825A7")]
    [InlineData("61626364656667686263646566676869636465666768696a6465666768696a6b65666768696a6b6c666768696a6b6c6d6768696a6b6c6d6e68696a6b6c6d6e6f696a6b6c6d6e6f706a6b6c6d6e6f70716b6c6d6e6f7071726c6d6e6f707172736d6e6f70717273746e6f707172737475", "09330C33F71147E83D192FC782CD1B4753111B173B3B05D22FA08086E3B0F712FCC7C71A557E2DB966C3E9FA91746039")]
    public void SHA384(string data, string expectedHash) => this.HashAlgorithm(new SHA384Hash(), data, expectedHash);

    [Theory]
    [InlineData("616263", "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f")]
    [InlineData("61626364656667686263646566676869636465666768696a6465666768696a6b65666768696a6b6c666768696a6b6c6d6768696a6b6c6d6e68696a6b6c6d6e6f696a6b6c6d6e6f706a6b6c6d6e6f70716b6c6d6e6f7071726c6d6e6f707172736d6e6f70717273746e6f707172737475", "8e959b75dae313da8cf4f72814fc143f8f7779c6eb9f7fa17299aeadb6889018501d289e4900f7e4331b99dec4b5433ac7d329eeb6dd26545e96e55b874be909")]
    public void SHA512_Test1(string data, string expectedHash) => this.HashAlgorithm(new SHA512Hash(), data, expectedHash);

    [Fact]
    public void SHA512_Test2() => this.HashAlgorithm(new SHA512Hash(), StringBuilderExtensions.RepeatAppend("61", 1000000), "e718483d0ce769644e2e42c7bc15b4638e1f98b13b2044285632a803afa973ebde0ff244877ea60a4cb0432ce577c31beb009c5c2c49aa2e4eadb217ad8cc09b");

    [Theory]
    [InlineData("", "d41d8cd98f00b204e9800998ecf8427e")]    // ""
    [InlineData("61", "0cc175b9c0f1b6a831c399e269772661")]  // "a"
    [InlineData("616263", "900150983cd24fb0d6963f7d28e17f72")]  // "abc"
    [InlineData("6d65737361676520646967657374", "f96b697d7cb7938d525a2f31aaf161d0")]  // "message digest"
    [InlineData("6162636465666768696a6b6c6d6e6f707172737475767778797a", "c3fcd3d76192e4007dfb496cca67e13b")]  // "abcdefghijklmnopqrstuvwxyz"
    [InlineData("4142434445464748494a4b4c4d4e4f505152535455565758595a6162636465666768696a6b6c6d6e6f707172737475767778797a30313233343536373839", "d174ab98d277d9f5a5611c2c9f419d9f")]  // "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    [InlineData("3132333435363738393031323334353637383930313233343536373839303132333435363738393031323334353637383930313233343536373839303132333435363738393031323334353637383930", "57edf4a22be3c955ac49da2e2107b67a")] // "1234567890" x 8
    public void MD5(string data, string expectedHash) => this.HashAlgorithm(new MD5Hash(), data, expectedHash);

    [Theory]
    [InlineData("", "9c1185a5c5e9fc54612808977ee8f548b2258d31")]    // ""
    [InlineData("61", "0bdc9d2d256b3ee9daae347be6f4dc835a467ffe")]  // "a"
    [InlineData("616263", "8eb208f7e05d987a9b044a8e98c6b087f15a0bfc")]  // "abc"
    [InlineData("6d65737361676520646967657374", "5d0689ef49d2fae572b881b123a85ffa21595f36")]  // "message digest"
    [InlineData("6162636465666768696a6b6c6d6e6f707172737475767778797a", "f71c27109c692c1b56bbdceb5b9d2865b3708dbc")]  // "abcdefghijklmnopqrstuvwxyz"
    [InlineData("4142434445464748494a4b4c4d4e4f505152535455565758595a6162636465666768696a6b6c6d6e6f707172737475767778797a30313233343536373839", "b0e20b6e3116640286ed3a87a5713079b21f5189")]  // "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    [InlineData("6162636462636465636465666465666765666768666768696768696a68696a6b696a6b6c6a6b6c6d6b6c6d6e6c6d6e6f6d6e6f706e6f7071", "12a053384a9c0c88e405a06c27dcf49ada62eb2b")]  // "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"
    [InlineData("2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824", "b6a9c8c230722b7c748331a8b450f05566dc7d0f")]
    public void RIPEMD160(string data, string expectedHash) => this.HashAlgorithm(new RIPEMD160Hash(), data, expectedHash);

    [Theory]
    [InlineData("", "00000000000000000000000000000000")]
    [InlineData("The quick brown fox jumps over the lazy dog", "6c1b07bc7bbc4be347939ac4a93c437a")]
    public void Murmur3(string str, string expectedHash) => this.HashAlgorithmString(new Murmur3Hash(), str, expectedHash);

    [Theory]
    [InlineData("blake2", "4245af08b46fbb290222ab8a68613621d92ce78577152d712467742417ebc1153668f1c9e1ec1e152a32a9c242dc686d175e087906377f0c483c5be2cb68953e")]
    [InlineData("hello world", "021ced8799296ceca557832ab941a50b4a11f83478cf141f51f933f653ab9fbcc05a037cddbed06e309bf334942c4e58cdf1a46e237911ccd7fcf9787cbc7fd0")]
    [InlineData("verystrongandlongpassword", "1f7d9b7c9a90f7bfc66e52b69f3b6c3befbd6aee11aac860e99347a495526f30c9e51f6b0db01c24825092a09dd1a15740f0ade8def87e60c15da487571bcef7")]
    [InlineData("The quick brown fox jumps over the lazy dog", "a8add4bdddfd93e4877d2746e62817b116364a1fa7bc148d95090bc7333b3673f82401cf7aa2e4cb1ecd90296e3f14cb5413f8ed77be73045b13914cdcd6a918")]
    [InlineData("", "786a02f742015903c6c6fd852552d272912f4740e15847618a86e217f71f5419d25e1031afee585313896444934eb04b903a685b1448b755d56f701afe9be2ce")]
    [InlineData("abc", "ba80a53f981c4d0d6a2797b69f12f6e94c212f14685ac4b74b12bb6fdbffa2d17d87c5392aab792dc252d5de4533cc9518d38aa8dbf1925ab92386edd4009923")]
    public void Blake2b(string str, string expectedHash) => this.HashAlgorithmString(new Blake2bHash(), str, expectedHash);

    [Theory]
    [InlineData("", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f", "10ebb67700b1868efb4417987acf4690ae9d972fb7a590c2f02871799aaa4786b5e996e8f0f4eb981fc214b005f42d2ff4233499391653df7aefcbc13fc51568")]
    [InlineData("00", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f", "961f6dd1e4dd30f63901690c512e78e4b45e4742ed197c3c5e45c549fd25f2e4187b0bc9fe30492b16b0d0bc4ef9b0f34c7003fac09a5ef1532e69430234cebd")]
    [InlineData("0001", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f", "da2cfbe2d8409a0f38026113884f84b50156371ae304c4430173d08a99d9fb1b983164a3770706d537f49e0c916d9f32b95cc37a95b99d857436f0232c88a965")]
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f", "f1aa2b044f8f0c638a3f362e677b5d891d6fd2ab0765f6ee1e4987de057ead357883d9b405b9d609eea1b869d97fb16d9b51017c553f3b93c0a1e0f1296fedcd")]
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f", "c230f0802679cb33822ef8b3b21bf7a9a28942092901d7dac3760300831026cf354c9232df3e084d9903130c601f63c1f4a4a4b8106e468cd443bbe5a734f45f")]
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfe", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f", "142709d62e28fcccd0af97fad0f8465b971e82201dc51070faa0372aa43e92484be1c1e73ba10906d5d1853db6a4106e0a7bf9800d373d6dee2d46d62ef2a461")]
    public void Blake2b_keyed(string str, string key, string expectedHash) => this.HashAlgorithm(new Blake2bHash(key.FromHexString()), str, expectedHash);

    [Theory]
    [InlineData("", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "48a8997da407876b3d79c0d92325ad3b89cbb754d86ab71aee047ad345fd2c49")]
    [InlineData("00", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "40d15fee7c328830166ac3f918650f807e7e01e177258cdc0a39b11f598066f1")]
    [InlineData("0001", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "6bb71300644cd3991b26ccd4d274acd1adeab8b1d7914546c1198bbe9fc9d803")]
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "172ffc67153d12e0ca76a8b6cd5d4731885b39ce0cac93a8972a18006c8b8baf")]
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "4f8ce1e51d2fe7f24043a904d898ebfc91975418753413aa099b795ecb35cedb")]
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfe", "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "3fb735061abc519dfe979e54c1ee5bfad0a9d858b3315bad34bde999efd724dd")]
    public void Blake2s_keyed(string str, string key, string expectedHash) => this.HashAlgorithm(new Blake2sHash(key.FromHexString()), str, expectedHash);

    [Theory]
    [InlineData(288, "", "6753e3380c09e385d0339eb6b050a68f66cfd60a73476e6fd6adeb72f5edd7c6f04a5d01")]
    [InlineData(288, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f67", "0bbe6afae0d7e89054085c1cc47b1689772c89a41796891e197d1ca1b76f288154933ded")]
    [InlineData(288, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f672e", "82558a209b960ddeb531e6dcb281885b2400ca160472462486e79f071e88a3330a8a303d")]
    [InlineData(224, "", "f71837502ba8e10837bdd8d365adb85591895602fc552b48b7390abd")]
    [InlineData(224, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f67", "310aee6b30c47350576ac2873fa89fd190cdc488442f3ef654cf23fe")]
    [InlineData(224, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f672e", "c59d4eaeac728671c635ff645014e2afa935bebffdb5fbd207ffdeab")]
    [InlineData(256, "", "c5d2460186f7233c927e7db2dcc703c0e500b653ca82273b7bfad8045d85a470")]
    [InlineData(256, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f67", "4d741b6f1eb29cb2a9b9911c82f56fa8d73b04959d3d9d222895df6c0b28aa15")]
    [InlineData(256, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f672e", "578951e24efd62a3d63a86f7cd19aaa53c898fe287d2552133220370240b572d")]
    [InlineData(384, "", "2c23146a63a29acf99e73b88f8c24eaa7dc60aa771780ccc006afbfa8fe2479b2dd2b21362337441ac12b515911957ff")]
    [InlineData(384, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f67", "283990fa9d5fb731d786c5bbee94ea4db4910f18c62c03d173fc0a5e494422e8a0b3da7574dae7fa0baf005e504063b3")]
    [InlineData(384, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f672e", "9ad8e17325408eddb6edee6147f13856ad819bb7532668b605a24a2d958f88bd5c169e56dc4b2f89ffd325f6006d820b")]
    [InlineData(512, "", "0eab42de4c3ceb9235fc91acffe746b29c29a8c366b7c60e4e67c466f36a4304c00fa9caf9d87976ba469bcbe06713b435f091ef2769fb160cdab33d3670680e")]
    [InlineData(512, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f67", "d135bb84d0439dbac432247ee573a23ea7d3c9deb2a968eb31d47c4fb45f1ef4422d6c531b5b9bd6f449ebcc449ea94d0a8f05f62130fda612da53c79659f609")]
    [InlineData(512, "54686520717569636b2062726f776e20666f78206a756d7073206f76657220746865206c617a7920646f672e", "ab7192d2b11f51c7dd744e7b3441febf397ca07bf812cceae122ca4ded6387889064f8db9230f173f6d1ab6e24b6e50f065b039f799f5592360a6558eb52d760")]
    public void KeccakHash(int bitLength, string str, string expectedHash) => this.HashAlgorithm(new KeccakHash(bitLength), str, expectedHash);

    [Theory]
    [InlineData("", "cd5101d1ccdf0d1d1f4ada56e888cd724ca1a0838a3521e7131d4fb78d0f5eb6")]
    [InlineData("a", "c51a1d639596fb613d86557314a150c40f8fff3de48bc93a3b03c161f4105ee4")]
    [InlineData("abc", "0bd1b36109f1318411a0517315aa46b8839df06622a278676f5487996c9cfc04")]
    [InlineData("abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmnhijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu", "02621dbb53f2c7001be64d7308ecb80d21ba7797c92e98d1efc240d41e4c414b")]
    public void KupynaHash(string str, string expectedHash) => this.HashAlgorithmString(new KupynaHash(), str, expectedHash);

    [Theory]
    [InlineData("", "3293AC630C13F0245F92BBB1766E16167A4E58492DDE73F3")]
    [InlineData("a", "77BEFBEF2E7EF8AB2EC8F93BF587A7FC613E247F5F247809")]
    [InlineData("abc", "2AAB1484E8C158F2BFB8C5FF41B57A525129131C957B5F93")]
    [InlineData("message digest", "D981F8CB78201A950DCF3048751E441C517FCA1AA55A29F6")]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "1714A472EEE57D30040412BFCC55032A0B11602FF37BEEE9")]
    [InlineData("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", "0F7BF9A19B9C58F2B7610DF7E84F0AC3A71C631E7B53F78E")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "8DCEA680A17583EE502BA38A3C368651890FFBCCDC49A8CC")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvw", "38F41D9D9A710A10C3727AC0DEEAA270727D9F926EC10139")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ=abcdefghijklmnopqrstuvwxyz+0123456789", "48CEEB6308B87D46E95D656112CDF18D97915F9765658957")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-", "C54034E5B43EB8005848A7E0AE6AAC76E4FF590AE715FD25")]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "1C14795529FD9F207A958F84C52F11E887FA0CABDFD91BFD")]
    [InlineData("Tiger", "DD00230799F5009FEC6DEBC838BB6A27DF2B9D6F110C7937")]
    [InlineData("Tiger - A Fast New Hash Function, by Ross Anderson and Eli Biham, proceedings of Fast Software Encryption 3, Cambridge, 1996.", "631ABDD103EB9A3D245B6DFD4D77B257FC7439501D1568DD")]
    public void TigerHash(string str, string expectedHash) => this.HashAlgorithmString(new TigerHash(), str, expectedHash);

    [Theory]
    [InlineData("", "19FA61D75522A4669B44E39C1D2E1726C530232130D407F89AFEE0964997F7A73E83BE698B288FEBCF88E3E03C4F0757EA8964E59B63D93708B138CC42A66EB3")]
    [InlineData("a", "8ACA2602792AEC6F11A67206531FB7D7F0DFF59413145E6973C45001D0087B42D11BC645413AEFF63A42391A39145A591A92200D560195E53B478584FDAE231A")]
    [InlineData("abc", "4E2448A4C6F486BB16B6562C73B4020BF3043E3A731BCE721AE1B303D97E6D4C7181EEBDB6C57E277D0E34957114CBD6C797FC9D95D8B582D225292076D4EEF5")]
    [InlineData("message digest", "378C84A4126E2DC6E56DCC7458377AAC838D00032230F53CE1F5700C0FFB4D3B8421557659EF55C106B4B52AC5A4AAA692ED920052838F3362E86DBD37A8903E")]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "F1D754662636FFE92C82EBB9212A484A8D38631EAD4238F5442EE13B8054E41B08BF2A9251C30B6A0B8AAE86177AB4A6F68F673E7207865D5D9819A3DBA4EB3B")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "DC37E008CF9EE69BF11F00ED9ABA26901DD7C28CDEC066CC6AF42E40F82F3A1E08EBA26629129D8FB7CB57211B9281A65517CC879D7B962142C65F5A7AF01467")]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "466EF18BABB0154D25B9D38A6414F5C08784372BCCB204D6549C4AFADB6014294D5BD8DF2A6C44E538CD047B2681A51A2C60481E88C5A20B2C2A80CF3A9A083B")]
    [InlineData("abcdbcdecdefdefgefghfghighijhijk", "2A987EA40F917061F5D6F0A0E4644F488A7A5A52DEEE656207C562F988E95C6916BDC8031BC5BE1B7B947639FE050B56939BAAA0ADFF9AE6745B7B181C3BE3FD")]
    public void WhirlpoolHash(string str, string expectedHash) => this.HashAlgorithmString(new WhirlpoolHash(), str, expectedHash);
    #endregion

    #region HMAC
    private void HMAC(IHashAlgorithm hashAlgorithm, string key, string data, string expectedHash) => this.HashAlgorithm(new HMACAlgorithm(hashAlgorithm, key.FromHexString()), data, expectedHash);

    [Theory]
    [InlineData("", "", "fbdb1d1b18aa6c08324b7d64b71fb76370690e1d")]
    [InlineData("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "4869205468657265", "b617318655057264e28bc0b6fb378c8ef146be00")] // "Hi There"
    [InlineData("4a656665", "7768617420646f2079612077616e7420666f72206e6f7468696e673f", "effcdf6ae5eb2fa2d27416d5f184df9c259a7c79")] // "Jefe", "what do ya want for nothing?"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "125d7342b9ac11cd91a39af48aa17b4f63f175d3")]
    [InlineData("0102030405060708090a0b0c0d0e0f10111213141516171819", "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd", "4c9007f4026250c6bc8414f9bf50c86c2d7235da")]
    [InlineData("0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c", "546573742057697468205472756e636174696f6e", "4c1a03424b55e07fe7f27be1d58bb9324a9a5a04")] // "Test With Truncation"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b6579202d2048617368204b6579204669727374", "aa4ae5e15272d00e95705637ce8a3b55ed402112")] // "Test Using Larger Than Block-Size Key - Hash Key First"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b657920616e64204c6172676572205468616e204f6e6520426c6f636b2d53697a652044617461", "e8e99d0f45237d786d6bbaa7965c7808bbff1a91")] // "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
    public void HMAC_SHA1(string key, string data, string expectedHash) => this.HMAC(new SHA1Hash(), key, data, expectedHash);

    [Fact]
    public void HMAC_SHA1_Truncate()
        {
        const string expectedHash96String = "4c1a03424b55e07fe7f27be1";
        byte[] expectedHash96 = expectedHash96String.FromHexString();
        this.HashAlgorithm(new HMACAlgorithm(new SHA1Hash(), "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".FromHexString(), expectedHash96.Length * 8), "546573742057697468205472756e636174696f6e", expectedHash96String); // "Test With Truncation"
        }

    [Theory]
    [InlineData("", "", "74e6f7298a9c2d168935f58c001bad88")]
    [InlineData("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "4869205468657265", "9294727a3638bb1c13f48ef8158bfc9d")] // "Hi There"
    [InlineData("4a656665", "7768617420646f2079612077616e7420666f72206e6f7468696e673f", "750c783e6ab0b503eaa86e310a5db738")] // "Jefe", "what do ya want for nothing?"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "56be34521d144c88dbb8c733f0e8b3f6")]
    [InlineData("0102030405060708090a0b0c0d0e0f10111213141516171819", "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd", "697eaf0aca3a3aea3a75164746ffaa79")]
    [InlineData("0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c", "546573742057697468205472756e636174696f6e", "56461ef2342edc00f9bab995690efd4c")] // "Test With Truncation"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b6579202d2048617368204b6579204669727374", "6b1ab7fe4bd7bf8f0b62e6ce61b9d0cd")] // "Test Using Larger Than Block-Size Key - Hash Key First"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b657920616e64204c6172676572205468616e204f6e6520426c6f636b2d53697a652044617461", "6f630fad67cda0ee1fb1f562db3aa53e")] // "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
    public void HMAC_MD5(string key, string data, string expectedHash) => this.HMAC(new MD5Hash(), key, data, expectedHash);

    [Fact]
    public void HMAC_MD5_Truncate()
        {
        const string expectedHash96String = "56461ef2342edc00f9bab995";
        byte[] expectedHash96 = expectedHash96String.FromHexString();
        this.HashAlgorithm(new HMACAlgorithm(new MD5Hash(), "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".FromHexString(), expectedHash96.Length * 8), "546573742057697468205472756e636174696f6e", expectedHash96String); // "Test With Truncation"
        }

    [Theory]
    [InlineData("", "", "b613679a0814d9ec772f95d778c35fc5ff1697c493715653c6c712144292c5ad")]
    [InlineData("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "4869205468657265", "b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7")] // "Hi There"
    [InlineData("4a656665", "7768617420646f2079612077616e7420666f72206e6f7468696e673f", "5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843")] // "Jefe", "what do ya want for nothing?"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "773ea91e36800e46854db8ebd09181a72959098b3ef8c122d9635514ced565fe")]
    [InlineData("0102030405060708090a0b0c0d0e0f10111213141516171819", "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd", "82558a389a443c0ea4cc819899f2083a85f0faa3e578f8077a2e3ff46729665b")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b6579202d2048617368204b6579204669727374", "60e431591ee0b67f0d8a26aacbf5b77f8e0bc6213728c5140546040f0ee37f54")] // "Test Using Larger Than Block-Size Key - Hash Key First"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "5468697320697320612074657374207573696e672061206c6172676572207468616e20626c6f636b2d73697a65206b657920616e642061206c6172676572207468616e20626c6f636b2d73697a6520646174612e20546865206b6579206e6565647320746f20626520686173686564206265666f7265206265696e6720757365642062792074686520484d414320616c676f726974686d2e", "9b09ffa71b942fcb27635fbcd5b0e944bfdc63644f0713938a7f51535c3a35e2")]
    public void HMAC_SHA256(string key, string data, string expectedHash) => this.HMAC(new SHA256Hash(), key, data, expectedHash);

    [Fact]
    public void HMAC_SHA256_Truncate()
        {
        const string expectedHash96String = "a3b6167473100ee06e0c796c2955552b";
        byte[] expectedHash96 = expectedHash96String.FromHexString();
        this.HashAlgorithm(new HMACAlgorithm(new SHA256Hash(), "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".FromHexString(), expectedHash96.Length * 8), "546573742057697468205472756e636174696f6e", expectedHash96String); // "Test With Truncation"
        }

    [Theory]
    [InlineData("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "4869205468657265", "afd03944d84895626b0825f4ab46907f15f9dadbe4101ec682aa034c7cebc59cfaea9ea9076ede7f4af152e8b2fa9cb6")] // "Hi There"
    [InlineData("4a656665", "7768617420646f2079612077616e7420666f72206e6f7468696e673f", "af45d2e376484031617f78d2b58a6b1b9c7ef464f5a01b47e42ec3736322445e8e2240ca5e69e2c78b3239ecfab21649")] // "Jefe", "what do ya want for nothing?"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "88062608d3e6ad8a0aa2ace014c8a86f0aa635d947ac9febe83ef4e55966144b2a5ab39dc13814b94e3ab6e101a34f27")]
    [InlineData("0102030405060708090a0b0c0d0e0f10111213141516171819", "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd", "3e8a69b7783c25851933ab6290af6ca77a9981480850009cc5577c6e1f573b4e6801dd23c4a7d679ccf8a386c674cffb")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b6579202d2048617368204b6579204669727374", "4ece084485813e9088d2c63a041bc5b44f9ef1012a2b588f3cd11f05033ac4c60c2ef6ab4030fe8296248df163f44952")] // "Test Using Larger Than Block-Size Key - Hash Key First"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "5468697320697320612074657374207573696e672061206c6172676572207468616e20626c6f636b2d73697a65206b657920616e642061206c6172676572207468616e20626c6f636b2d73697a6520646174612e20546865206b6579206e6565647320746f20626520686173686564206265666f7265206265696e6720757365642062792074686520484d414320616c676f726974686d2e", "6617178e941f020d351e2f254e8fd32c602420feb0b8fb9adccebb82461e99c5a678cc31e799176d3860e6110c46523e")]
    public void HMAC_SHA384(string key, string data, string expectedHash) => this.HMAC(new SHA384Hash(), key, data, expectedHash);

    [Fact]
    public void HMAC_SHA384_Truncate()
        {
        const string expectedHash96String = "3abf34c3503b2a23a46efc619baef897";
        byte[] expectedHash96 = expectedHash96String.FromHexString();
        this.HashAlgorithm(new HMACAlgorithm(new SHA384Hash(), "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".FromHexString(), expectedHash96.Length * 8), "546573742057697468205472756e636174696f6e", expectedHash96String); // "Test With Truncation"
        }

    [Theory]
    [InlineData("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "4869205468657265", "87aa7cdea5ef619d4ff0b4241a1d6cb02379f4e2ce4ec2787ad0b30545e17cdedaa833b7d6b8a702038b274eaea3f4e4be9d914eeb61f1702e696c203a126854")] // "Hi There"
    [InlineData("4a656665", "7768617420646f2079612077616e7420666f72206e6f7468696e673f", "164b7a7bfcf819e2e395fbe73b56e0a387bd64222e831fd610270cd7ea2505549758bf75c05a994a6d034f65f8f0e6fdcaeab1a34d4a6b4b636e070a38bce737")] // "Jefe", "what do ya want for nothing?"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "fa73b0089d56a284efb0f0756c890be9b1b5dbdd8ee81a3655f83e33b2279d39bf3e848279a722c806b485a47e67c807b946a337bee8942674278859e13292fb")]
    [InlineData("0102030405060708090a0b0c0d0e0f10111213141516171819", "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd", "b0ba465637458c6990e5a8c5f61d4af7e576d97ff94b872de76f8050361ee3dba91ca5c11aa25eb4d679275cc5788063a5f19741120c4f2de2adebeb10a298dd")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b6579202d2048617368204b6579204669727374", "80b24263c7c1a3ebb71493c1dd7be8b49b46d1f41b4aeec1121b013783f8f3526b56d037e05f2598bd0fd2215d6a1e5295e64f73f63f0aec8b915a985d786598")] // "Test Using Larger Than Block-Size Key - Hash Key First"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "5468697320697320612074657374207573696e672061206c6172676572207468616e20626c6f636b2d73697a65206b657920616e642061206c6172676572207468616e20626c6f636b2d73697a6520646174612e20546865206b6579206e6565647320746f20626520686173686564206265666f7265206265696e6720757365642062792074686520484d414320616c676f726974686d2e", "e37b6a775dc87dbaa4dfa9f96e5e3ffddebd71f8867289865df5a32d20cdc944b6022cac3c4982b10d5eeb55c3e4de15134676fb6de0446065c97440fa8c6a58")]
    public void HMAC_SHA512(string key, string data, string expectedHash) => this.HMAC(new SHA512Hash(), key, data, expectedHash);

    [Fact]
    public void HMAC_SHA512_Truncate()
        {
        const string expectedHash96String = "415fad6271580a531d4179bc891d87a6";
        byte[] expectedHash96 = expectedHash96String.FromHexString();
        this.HashAlgorithm(new HMACAlgorithm(new SHA512Hash(), "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".FromHexString(), expectedHash96.Length * 8), "546573742057697468205472756e636174696f6e", expectedHash96String); // "Test With Truncation"
        }

    [Theory]
    [InlineData("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "4869205468657265", "24cb4bd67d20fc1a5d2ed7732dcc39377f0a5668")] // "Hi There"
    [InlineData("4a656665", "7768617420646f2079612077616e7420666f72206e6f7468696e673f", "dda6c0213a485a9e24f4742064a7f033b43c4069")] // "Jefe", "what do ya want for nothing?"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "b0b105360de759960ab4f35298e116e295d8e7c1")]
    [InlineData("0102030405060708090a0b0c0d0e0f10111213141516171819", "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd", "d5ca862f4d21d5e610e18b4cf1beb97a4365ecf4")]
    [InlineData("0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c", "546573742057697468205472756e636174696f6e", "7619693978f91d90539ae786500ff3d8e0518e39")] // "Test With Truncation"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b6579202d2048617368204b6579204669727374", "6466ca07ac5eac29e1bd523e5ada7605b791fd8b")] // "Test Using Larger Than Block-Size Key - Hash Key First"
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b657920616e64204c6172676572205468616e204f6e6520426c6f636b2d53697a652044617461", "69ea60798d71616cce5fd0871e23754cd75d5a0a")] // "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
    public void HMAC_RIPE160(string key, string data, string expectedHash) => this.HMAC(new RIPEMD160Hash(), key, data, expectedHash);

    [Fact]
    public void HMAC_RIPE160_Truncate()
        {
        const string expectedHash96String = "7619693978f91d90539ae786";
        byte[] expectedHash96 = expectedHash96String.FromHexString();
        this.HashAlgorithm(new HMACAlgorithm(new RIPEMD160Hash(), "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".FromHexString(), expectedHash96.Length * 8), "546573742057697468205472756e636174696f6e", expectedHash96String); // "Test With Truncation"
        }

    #region Initialize & ModifyKey
    [Fact]
    public void ComputeHashShouldInitializeHashAlgorithm()
        {
        // Rfc2202_7
        byte[] key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa".FromHexString();
        byte[] data = "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b657920616e64204c6172676572205468616e204f6e6520426c6f636b2d53697a652044617461".FromHexString(); // "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
        byte[] expectedHash = "6f630fad67cda0ee1fb1f562db3aa53e".FromHexString();
        HMACAlgorithm hmac = new HMACAlgorithm(new MD5Hash(), key);

        byte[] actualHash = hmac.ComputeHash(data);
        Assert.Equal(expectedHash, actualHash);

        // Rfc2202_6
        data = "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b6579202d2048617368204b6579204669727374".FromHexString(); // "Test Using Larger Than Block-Size Key - Hash Key First"
        expectedHash = "6b1ab7fe4bd7bf8f0b62e6ce61b9d0cd".FromHexString();

        actualHash = hmac.ComputeHash(data);
        Assert.Equal(expectedHash, actualHash);
        }

    [Fact]
    public void ModifyKeyBeforeCompute()
        {
        // Rfc2202_7
        byte[] key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa".FromHexString();
        byte[] data = "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b657920616e64204c6172676572205468616e204f6e6520426c6f636b2d53697a652044617461".FromHexString(); // "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
        byte[] expectedHash = "6f630fad67cda0ee1fb1f562db3aa53e".FromHexString();

        // first initialize HMAC with a dummy key
        HMACAlgorithm hmac = new HMACAlgorithm(new MD5Hash(), "0101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101".FromHexString())
            {
            Key = key   // and assign key from Rfc2202_7
            };

        byte[] actualHash = hmac.ComputeHash(data);
        Assert.Equal(expectedHash, actualHash);
        }

    [Fact]
    public void ModifyKeyAfterCompute()
        {
        // Rfc2202_7
        byte[] key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa".FromHexString();
        byte[] data = "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b657920616e64204c6172676572205468616e204f6e6520426c6f636b2d53697a652044617461".FromHexString(); // "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
        byte[] expectedHash = "6f630fad67cda0ee1fb1f562db3aa53e".FromHexString();
        HMACAlgorithm hmac = new HMACAlgorithm(new MD5Hash(), key);

        byte[] actualHash = hmac.ComputeHash(data);
        Assert.Equal(expectedHash, actualHash);

        // Rfc2202_2: assign key smaller than block size
        key = "4a656665".FromHexString(); // "Jefe";
        data = "7768617420646f2079612077616e7420666f72206e6f7468696e673f".FromHexString(); // "what do ya want for nothing?"
        expectedHash = "750c783e6ab0b503eaa86e310a5db738".FromHexString();
        hmac.Key = key;

        actualHash = hmac.ComputeHash(data);
        Assert.Equal(expectedHash, actualHash);

        // Rfc2202_7: assign key larger than block size
        key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa".FromHexString();
        data = "54657374205573696e67204c6172676572205468616e20426c6f636b2d53697a65204b657920616e64204c6172676572205468616e204f6e6520426c6f636b2d53697a652044617461".FromHexString(); // "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data"
        expectedHash = "6f630fad67cda0ee1fb1f562db3aa53e".FromHexString();
        hmac.Key = key;

        actualHash = hmac.ComputeHash(data);
        Assert.Equal(expectedHash, actualHash);
        }
    #endregion
    #endregion

    #region FinalBlock

    [Fact]
    public void TransformFinalBlock_InputCountLessThanBlockSize()
        {
        byte[] data1 = [10, 13, 25, 14, 26];
        byte[] expectedHash1 = [21, 29, 28, 205, 37, 37, 222, 131, 237, 85, 37, 15, 19, 41, 248, 160, 238, 30, 84, 188];

        HashAlgorithm hashAlgorithm = new HashAlgorithmAdapter(new SHA1Hash());

        byte[] actual1 = hashAlgorithm.TransformFinalBlock(data1, 1, 2);
        Assert.Equal([13, 25], actual1);
        Assert.Equal(expectedHash1, hashAlgorithm.Hash);
        }

    [Fact]
    public void TransformBlock_TotalBytesLessThanBlockSize()
        {
        byte[] data = [10, 13, 25, 14, 26, 5, 7, 9, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 80, 1, 32, 34, 56, 43, 6, 1, 56, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 5];
        byte[] expectedHash = [12, 42, 118, 169, 170, 59, 51, 191, 192, 157, 90, 187, 16, 54, 239, 42, 84, 200, 76, 50];
        byte[] outputBuffer = new byte[100];

        HashAlgorithm hashAlgorithm = new HashAlgorithmAdapter(new SHA1Hash());

        int actual1 = hashAlgorithm.TransformBlock(data, 0, 50, outputBuffer, 0);
        Assert.Equal(50, actual1);
        int actual2 = hashAlgorithm.TransformBlock(data, 50, 13, outputBuffer, 0);
        Assert.Equal(13, actual2);
        byte[] actual3 = hashAlgorithm.TransformFinalBlock([], 0, 0);
        Assert.Empty(actual3);
        Assert.Equal(expectedHash, hashAlgorithm.Hash);
        }

    [Fact]
    public void TransformBlock_TotalBytesGreaterThanBlockSize()
        {
        byte[] data = [10, 13, 25, 14, 26, 5, 7, 9, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 80, 1, 32, 34, 56, 43, 6, 1, 56, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 5];
        byte[] expectedHash = [128, 163, 23, 117, 156, 106, 169, 171, 211, 31, 95, 85, 66, 31, 232, 41, 115, 58, 148, 22];
        byte[] outputBuffer = new byte[100];

        HashAlgorithm hashAlgorithm = new HashAlgorithmAdapter(new SHA1Hash());

        int actual1 = hashAlgorithm.TransformBlock(data, 0, 75, outputBuffer, 0);
        Assert.Equal(75, actual1);
        int actual2 = hashAlgorithm.TransformBlock(data, 75, 6, outputBuffer, 0);
        Assert.Equal(6, actual2);
        byte[] actual3 = hashAlgorithm.TransformFinalBlock([], 0, 0);
        Assert.Empty(actual3);
        Assert.Equal(expectedHash, hashAlgorithm.Hash);
        }

    [Fact]
    public void TransformBlock_TotalBytesEqualsBlockSize()
        {
        byte[] data = [10, 13, 25, 14, 26, 5, 7, 9, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 80, 1, 32, 34, 56, 43, 6, 1, 56, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 41, 55, 32, 55, 12, 78, 2, 34, 53, 5, 22];
        byte[] expectedHash = [40, 34, 15, 107, 222, 157, 191, 149, 93, 148, 21, 72, 215, 181, 74, 218, 157, 23, 121, 208];
        byte[] outputBuffer = new byte[100];

        HashAlgorithm hashAlgorithm = new HashAlgorithmAdapter(new SHA1Hash());

        int actual1 = hashAlgorithm.TransformBlock(data, 0, 60, outputBuffer, 0);
        Assert.Equal(60, actual1);
        int actual2 = hashAlgorithm.TransformBlock(data, 60, 4, outputBuffer, 0);
        Assert.Equal(4, actual2);
        byte[] actual3 = hashAlgorithm.TransformFinalBlock([], 0, 0);
        Assert.Empty(actual3);
        Assert.Equal(expectedHash, hashAlgorithm.Hash);
        }

    [Fact]
    public void TransformBlockAndTransformFinalBlock()
        {
        byte[] data = [10, 13, 25, 14, 26, 5, 7, 9, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 80, 1, 32, 34, 56, 43, 6, 1, 56, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 5];
        byte[] expectedHash = [128, 163, 23, 117, 156, 106, 169, 171, 211, 31, 95, 85, 66, 31, 232, 41, 115, 58, 148, 22];
        byte[] outputBuffer = new byte[100];

        HashAlgorithm hashAlgorithm = new HashAlgorithmAdapter(new SHA1Hash());

        int actual1 = hashAlgorithm.TransformBlock(data, 0, 70, outputBuffer, 0);
        Assert.Equal(70, actual1);
        Assert.Equal(data[..70], outputBuffer[..70]);
        Assert.Equal(new byte[30], outputBuffer[70..100]);
        int actual2 = hashAlgorithm.TransformBlock(data, 70, 6, outputBuffer, 15);
        Assert.Equal(6, actual2);
        Assert.Equal(data[0..15], outputBuffer[..15]);
        Assert.Equal(data[70..76], outputBuffer[15..21]);
        Assert.Equal(data[21..70], outputBuffer[21..70]);
        Assert.Equal(new byte[30], outputBuffer[70..100]);
        byte[] actual3 = hashAlgorithm.TransformFinalBlock(data, 76, 5);
        Assert.Equal(data[76..81], actual3);
        Assert.Equal(expectedHash, hashAlgorithm.Hash);
        }

    [Fact]
    public void TransformFinalBlockShouldAddBlockWhenThereIsNoRoomForMessageLength()
        {
        byte[] data = [10, 13, 25, 14, 26, 5, 7, 9, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 80, 1, 32, 34, 56, 43, 6, 1, 56, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 5, 14, 26, 5, 7, 9, 1, 12, 11, 54, 13, 54, 15, 17, 87, 16, 92, 20, 21, 25, 65, 78, 65, 43, 12, 53, 79, 80, 1, 32, 34, 56, 43, 6, 1, 56, 1, 12, 11, 54, 13, 54, 15];
        byte[] expectedHash = [130, 147, 240, 106, 140, 1, 186, 57, 153, 120, 4, 32, 80, 250, 92, 93, 235, 206, 199, 62];

        HashAlgorithm hashAlgorithm = new HashAlgorithmAdapter(new SHA1Hash());

        byte[] actual = hashAlgorithm.TransformFinalBlock(data, 0, data.Length);
        Assert.Equal(data, actual);
        Assert.Equal(expectedHash, hashAlgorithm.Hash);
        }
    #endregion 

    }

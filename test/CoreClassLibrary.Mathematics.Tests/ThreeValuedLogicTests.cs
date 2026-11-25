
using CoreClassLibrary.Extensions;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

// https://en.wikipedia.org/wiki/Three-valued_logic
public class ThreeValuedLogicTests(ITestOutputHelper output)
    {
    [Fact]
    public void ThreeValuedLogic()
        { 
        bool? t = true;
        Assert.True(t);
        Assert.NotNull(t);
        Assert.NotNull(!t);

        t = null;
        Assert.Null(t);
        Assert.Null(!t);
        }

    [Fact]
    public void NOT_()
        { 
        bool?[] aaa = [ false, null, true ];
        bool?[] results = [ true, null, false ];

        output.WriteLine("A\t!A");
        output.WriteLine("-\t-");
        foreach (bool? a in aaa)
            {
            output.WriteLine("{0}\t{1}", ThreeValuedLogicTests.ToString(a), ThreeValuedLogicTests.ToString(!a));
            }

        for (int a = 0; a < aaa.Length; a++)
            {
            Assert.Equal(results[a], !aaa[a]);
            }
        }

    [Fact]
    public void AND_()
        { 
        bool?[] aaa = [ false, null, true ];
        bool?[] bbb = [ false, null, true ];
        bool?[][] results = 
            [ 
            [ false, false, false ],
            [ false, null,  null ],
            [ false, null,  true ]
            ];

        output.WriteLine("\tA ^ B");
        output.WriteLine("");
        output.WriteLine(" \tF\tU\tT");
        output.WriteLine(" \t-\t-\t-");
        foreach (bool? a in aaa)
            {
            output.WriteLine(String.Concat(ThreeValuedLogicTests.ToString(a), " |\t", bbb.Select(b => ThreeValuedLogicTests.ToString(a.And(b))).ToDelimitedString("\t")));
            }

        for (int a = 0; a < aaa.Length; a++)
            {
            for (int b = 0; b < bbb.Length; b++)
                {
                Assert.Equal(results[a][b], aaa[a].And(bbb[b]));
                }
            }
    }
    
    [Fact]
    public void OR_()
        { 
        bool?[] aaa = [ false, null, true ];
        bool?[] bbb = [ false, null, true ];
        bool?[][] results = 
            [ 
            [ false, null, true ],
            [ null, null,  true ],
            [ true, true,  true ]
            ];

        output.WriteLine("\tA ^ B");
        output.WriteLine("");
        output.WriteLine(" \tF\tU\tT");
        output.WriteLine(" \t-\t-\t-");
        foreach (bool? a in aaa)
            {
            output.WriteLine(String.Concat(ThreeValuedLogicTests.ToString(a), " |\t", bbb.Select(b => ThreeValuedLogicTests.ToString(a.Or(b))).ToDelimitedString("\t")));
            }

        for (int a = 0; a < aaa.Length; a++)
            {
            for (int b = 0; b < bbb.Length; b++)
                {
                Assert.Equal(results[a][b], aaa[a].Or(bbb[b]));
                }
            }
        }

    [Fact]
    public void XOR_()
        { 
        bool?[] aaa = [ false, null, true ];
        bool?[] bbb = [ false, null, true ];
        bool?[][] results = 
            [ 
            [ false, null, true ],
            [ null,  null, null ],
            [ true,  null, false ]
            ];

        output.WriteLine("\tA ^ B");
        output.WriteLine("");
        output.WriteLine(" \tF\tU\tT");
        output.WriteLine(" \t-\t-\t-");
        foreach (bool? a in aaa)
            {
            output.WriteLine(String.Concat(ThreeValuedLogicTests.ToString(a), " |\t", bbb.Select(b => ThreeValuedLogicTests.ToString(a.Xor(b))).ToDelimitedString("\t")));
            }

        for (int a = 0; a < aaa.Length; a++)
            {
            for (int b = 0; b < bbb.Length; b++)
                {
                Assert.Equal(results[a][b], aaa[a].Xor(bbb[b]));
                }
            }
        }

    private static string ToString(bool? a)
        => a switch
            {
            true => "T",
            false => "F",
            null => "U"     // Unknown
            };
    }

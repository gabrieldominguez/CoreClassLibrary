
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Randomness;

public class CompositeRandom(IEnumerator<IRandom> circularEnumerator) : IRandom
    {
    public CompositeRandom(IReadOnlyCollection<IRandom> randoms): this(randoms.ToCircularEnumerable().GetEnumerator()) { }

    public double Sample()
        {
        circularEnumerator.MoveNext();
        return circularEnumerator.Current.Sample();
        }
    }

public class CompositeDiscreteRandom(IEnumerator<IDiscreteRandom> circularEnumerator) : IDiscreteRandom
    {
    protected CompositeDiscreteRandom(IReadOnlyCollection<IDiscreteRandom> discreteRandoms): this(discreteRandoms.ToCircularEnumerable().GetEnumerator()) { }

    public uint DiscreteSample()
        {
        circularEnumerator.MoveNext();
        return circularEnumerator.Current.DiscreteSample();
        }
    }

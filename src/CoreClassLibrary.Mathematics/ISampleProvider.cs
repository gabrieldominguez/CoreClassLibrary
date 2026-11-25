
namespace CoreClassLibrary.Mathematics;

public interface ISampleProvider<T>
    {
    void Initialize();

    T GetNext();
    }

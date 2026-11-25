using Xunit;

namespace CoreClassLibrary.Tests.Processing;

public class EventTests(ITestOutputHelper output)
    {
    public event EventHandler<string>? DataAvailable;
    protected void OnDataAvailable(string data) => this.DataAvailable?.Invoke(this, data);

    [Fact]
    public void Event()
        {
        this.DataAvailable += (sender, line) => Shared.Test(line, output);

        foreach (string line in Shared.Source)
            {
            this.OnDataAvailable(line);
            }
        }
    }

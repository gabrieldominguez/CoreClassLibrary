using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CoreClassLibrary.Objects;

/// <summary>
/// Application Data
/// </summary>
/// <see cref="https://github.com/aspnet/Announcements/issues/237"/>
public class ApplicationData
    {
    public DateTime StartedDateTime { get; } = DateTime.Now;

    public string Name { get; }
    public string FullName { get; }

    public string EntryAssemblyName { get; } = ApplicationData.GetEntryAssemblyName();
    public string MachineName { get; } = Environment.MachineName;

    public string BaseDirectory { get; }
    public string BaseFileName { get; }
    public List<string> Versions { get; }   // Versions.Add("v2");

    public string UserAgent { get; }

    public string[] Args { get; }
    public Dictionary<string, string> ArgsDictionary { get; }

    private readonly PooledStringBuilder infoBuilder;
    [JsonIgnore]
    public StringBuilder InfoBuilder 
        => this.Info == "" ? 
            this.infoBuilder.StringBuilder: 
            throw new InvalidOperationException("ApplicationData.Info already built.");

    public string Info { get; private set; } = "";

    public ApplicationData() : this([]) { }
    public ApplicationData(string[] args) : this(args, AppContext.BaseDirectory) { }
    public ApplicationData(string[] args, string baseDirectory) : this(args, baseDirectory, ApplicationData.GetBaseName()) { }
    public ApplicationData(string[] args, string baseDirectory, string name) : this(args, baseDirectory, name, ["v1"]) { }
    public ApplicationData(string[] args, string baseDirectory, string name, List<string> versions)
        {
        this.FullName = String.Concat(this.Name = name, "@", this.MachineName);
        this.UserAgent = String.Concat(this.FullName, "/", (this.Versions = versions).LastOrDefault() ?? "v1", " (", Environment.OSVersion.VersionString, ")");

        this.BaseFileName = Path.Combine(this.BaseDirectory = baseDirectory, ApplicationData.GetBaseName());
        this.ArgsDictionary = (this.Args = args).ParseCommandLineArgs();

        (this.infoBuilder = new PooledStringBuilder()).StringBuilder
            .Append(this.Name).Append(" started on ").Append(this.StartedDateTime.ToLocalDateTimeString()).Append(" in ").Append(this.MachineName).Append(" with ").Append(Environment.ProcessorCount).Append(" processors over ").Append(Environment.OSVersion.VersionString).Append(" CLR v").Append(Environment.Version).AppendLine()
            .AppendLine()
            .Append("Started: ").AppendLine(this.StartedDateTime.ToLocalDateTimeString())
            .Append("Name: ").AppendLine(this.Name)
            .Append("EntryAssemblyName: ").AppendLine(this.EntryAssemblyName)
            .Append("MachineName: ").AppendLine(this.MachineName)
            .Append("OSVersion: ").AppendLine(Environment.OSVersion.VersionString)
            .Append("CLRVersion: ").AppendLine(Environment.Version)
            .Append("BaseDirectory: ").AppendLine(this.BaseDirectory)
            .Append("BaseFileName: ").AppendLine(this.BaseFileName)
            .Append("Versions: ").AppendLine(this.Versions.ToDelimitedString())
            .Append("Args: ").AppendLine(String.Join(" ", this.Args))
            .Append("ArgsDictionary: ").AppendLine(this.ArgsDictionary.ToKeyValueString())
            .AppendLine()
            .Append("CommandLine: ").AppendLine(Environment.CommandLine)
            .Append("CurrentDirectory: ").AppendLine(Environment.CurrentDirectory)
            .Append("User: ").Append(Environment.UserDomainName).Append('\\').AppendLine(Environment.UserName)
            .AppendLine();
        }

    public static string GetBaseName()
        => typeof(ApplicationData).FullName!.GetTextBetweenTokens("", ".");

    public static string GetEntryAssemblyName()
        => Assembly.GetEntryAssembly()?.GetName().Name ?? "";

    public void BuildInfo()
        {
        this.Info = this.InfoBuilder.ToString();
        this.infoBuilder.Dispose();
        }   
    }

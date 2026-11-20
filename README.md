# CoreClassLibrary

A collection of reusable .NET infrastructure types and extension methods. 

## Features

| Area | Examples |
| --- | --- |
| Collections | Bounded queues and stacks, unique bounded queues, sorted collections that allow duplicates, undo/redo stacks, and recently used items |
| Extensions | Helpers for collections, dictionaries, enumerables, strings, dates, files, tuples, and paging |
| Objects | Observable and entity base types, comparers, predicates, disposable wrappers, scopes, object pools, weak delegates, messaging, and pipelines |
| Threading | Asynchronous queues, signals, task helpers, and synchronization utilities |
| Networking | TCP and UDP client/server helpers, request/response protocols, and SNTP support |
| Files and streams | Directory traversal, disposable files, stream buffering, pipe streams, and bounded response buffering |
| Text processing | Regular-expression helpers, lexers, rules, and text templating |
| Validation and exceptions | Data-annotation validation, recursive validation, domain exceptions, and exception detail helpers |

## Getting started

Add a project reference to `src/CoreClassLibrary/CoreClassLibrary.csproj`, then import the namespaces you need.

A commonly used namespaces include `CoreClassLibrary.Extensions`, `CoreClassLibrary.Threading.AsyncQueues`, `CoreClassLibrary.Streams`, and `CoreClassLibrary.Validation`.

## Build and test

Install the .NET 11 SDK, then build the solution from the repository root:

```sh
dotnet build CoreClassLibrary.sln
```

The test project uses Microsoft.Testing.Platform. Select that runner when invoking `dotnet test`.

PowerShell:

```powershell
$env:DOTNET_TEST_RUNNER = "Microsoft.Testing.Platform"
dotnet test --project test/CoreClassLibrary.Tests/CoreClassLibrary.Tests.csproj
```

Bash:

```sh
DOTNET_TEST_RUNNER=Microsoft.Testing.Platform dotnet test --project test/CoreClassLibrary.Tests/CoreClassLibrary.Tests.csproj
```

## Repository layout

- `src/CoreClassLibrary` — library source and project file.
- `test/CoreClassLibrary.Tests` — xUnit unit tests.
- `CoreClassLibrary.sln` — solution containing the library and test project.

[![BugSplat Banner Image](https://user-images.githubusercontent.com/20464226/149019306-3186103c-5315-4dad-a499-4fd1df408475.png)](https://bugsplat.com)

# BugSplat
### **Crash and error reporting built for busy developers.**

[![Follow @bugsplatco on Twitter](https://img.shields.io/twitter/follow/bugsplatco?label=Follow%20BugSplat&style=social)](https://twitter.com/bugsplatco)
[![Join BugSplat on Discord](https://img.shields.io/discord/664965194799251487?label=Join%20Discord&logo=Discord&style=social)](https://discord.gg/bugsplat)

## 👋 Introduction

ZipC contains a .NET 6.0 command-line [tool](#🧰-tool) and .NET Standard 2.0 [library](#📚-library) for creating zip files from [glob patterns](https://en.wikipedia.org/wiki/Glob_(programming)).

## 🧰 Tool

The `ZipC` command-line tool can be installed globally via [dotnet](https://dotnet.microsoft.com/).

```sh
dotnet tool install -g zipc
```

Run `dotnet zipc -h` to see usage information.

```sh
C:\www\ZipCreator\ZipC> dotnet zipc -h
Description:
  Create a zip via a manifest file containing glob pattern rules

Usage:
  ZipC <input> <output> [options]

Arguments:
  <input>   Zip file manifest containing glob patterns of file paths to include
  <output>  Path to zip file output

Options:
  --force         Overwrite output file if it exists [default: False]
  --verbose       Show verbose log statements [default: False]
  --version       Show version information
  -?, -h, --help  Show help and usage information
```

For the `input` argument, pass the name of a file containing glob patterns of files to include. 

[input.txt](./ZipC/input.txt)
```txt
path/to/folder/**/*
README.md
LICENSE.md
```

For the `output` argument, pass a path to the location of the output file. If you'd like to overwrite the output file if it exists, add the `--force` option. To increase log verbosity add the `--verbose` flag to your command's arguments.

## 📚 Library

The `ZipCreator` library can be added to your project via NuGet.

```sh
Install-Package ZipCreator
```

Add a using statement for the `ZipC` namespace

```cs
using ZipC;
```

You can create an instance of `ZipCreator` from a text file that contains a list of filters separated by new lines.

[input.txt](./ZipC/input.txt)
```cs
var zipCreator = ZipCreator.CreatFromFile("input.txt");
```

You can also creat an instances of `ZipCreator` with the default constructor and set the `Settings` properties according to your use case.

```cs
var zipCreator = new ZipCreator();
zipCreator.Settings.Filters = new List<string>() { "path/to/folder/**/*" };
zipCreator.Settings.Interceptors.Add((fileInfo) => Console.WriteLine(fileInfo.FullName));
zipCreator.Settings.Overwrite = true;
zipCreator.Settings.ZipOutputFile = new FileInfo(pathToOutputZip);
```

The `Filters` property accepts a list of path globs. You can also exclude patterns by prefixing the pattern with a `!`.

```cs
zipCreator.Settings.Filters = new List<string>()
{
    "path/to/folder/**/*",
    "!path/to/folder/exclude.txt"
};
```

The `Interceptors` property is a `List<Action<FileInfo>>` containting actions that get invoked with each file before it is added to the zip.

```cs
zipCreator.Settings.Interceptors.Add((fileInfo) => SignTool(fileInfo.FullName));
```

The `Overwrite` property controls whether the `ZipOutputFile` file should be overwritten if it exists. If `Overwrite` is false the call to `MakeZips` will return `ZipCreatorResult.OverwriteError` if the output file exists.

## 🐛 About

[BugSplat](https://bugsplat.com) is a software crash and error reporting service with support for [Windows C++](https://docs.bugsplat.com/introduction/getting-started/integrations/desktop/cplusplus), [.NET Framework](https://docs.bugsplat.com/introduction/getting-started/integrations/desktop/windows-dot-net-framework), [dotnet](https://docs.bugsplat.com/introduction/getting-started/integrations/cross-platform/dot-net-standard) and [many more](https://docs.bugsplat.com/introduction/getting-started/integrations). BugSplat automatically captures critical diagnostic data such as stack traces, log files, and other runtime information. BugSplat also provides automated incident notifications, a convenient dashboard for monitoring trends and prioritizing engineering efforts, and integrations with popular development tools to maximize productivity and ship more profitable software.
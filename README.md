[![BugSplat Banner Image](https://user-images.githubusercontent.com/20464226/149019306-3186103c-5315-4dad-a499-4fd1df408475.png)](https://bugsplat.com)

# BugSplat
### **Crash and error reporting built for busy developers.**

[![Follow @bugsplatco on Twitter](https://img.shields.io/twitter/follow/bugsplatco?label=Follow%20BugSplat&style=social)](https://twitter.com/bugsplatco)
[![Join BugSplat on Discord](https://img.shields.io/discord/664965194799251487?label=Join%20Discord&logo=Discord&style=social)](https://discord.gg/bugsplat)

## 👋 Introduction

ZipCreator is a .NET Standard 2.0 [library](#📚-library) for creating zip files from [glob patterns](https://en.wikipedia.org/wiki/Glob_(programming)).

## ⚙️ Installation

The `ZipCreator` library can be added to your project via NuGet.

```sh
Install-Package ZipCreator
```

## 🏗️ Usage

Add a using statement for the `ZipCreator` namespace

```cs
using ZipCreator;
```

Create an instance of `Zip` from a text file that contains a list of filters separated by new lines.

[input.txt](./ZipCreator/input.txt)
```cs
var zip = Zip.CreateFromFile("input.txt");
```

You can also create an instances of `Zip` with the default constructor and set the `Settings` properties according to your use case.

```cs
var zip = new Zip();
zip.Settings.Filters = new List<string>() { "path/to/folder/**/*" };
zip.Settings.Interceptors.Add((fileInfo) => Console.WriteLine(fileInfo.FullName));
zip.Settings.Output = new FileInfo(pathToOutputZip);
zip.Settings.Overwrite = true;
```

The `Filters` property accepts a list of path globs. You can also exclude patterns by prefixing the pattern with a `!`.

```cs
zip.Settings.Filters = new List<string>()
{
    "path/to/folder/**/*",
    "!path/to/folder/exclude.txt"
};
```

The `Interceptors` property is a `List<Action<FileInfo>>` containting actions that get invoked with each file before it is added to the zip.

```cs
zip.Settings.Interceptors.Add((fileInfo) => SignTool(fileInfo.FullName));
```

The `Overwrite` property controls whether the `Output` file should be overwritten if it exists. If `Overwrite` is false the call to `MakeZips` will return `ZipResult.OverwriteError` if the output file exists.

```cs
zip.Settings.Overwrite = true
```

Finally, call `Write` to create a `.zip` file and write it to the location specified in `Settings.Output`.

```cs
var result = zip.Write();
```

## 🐛 About

[BugSplat](https://bugsplat.com) is a software crash and error reporting service with support for [Windows C++](https://docs.bugsplat.com/introduction/getting-started/integrations/desktop/cplusplus), [.NET Framework](https://docs.bugsplat.com/introduction/getting-started/integrations/desktop/windows-dot-net-framework), [dotnet](https://docs.bugsplat.com/introduction/getting-started/integrations/cross-platform/dot-net-standard) and [many more](https://docs.bugsplat.com/introduction/getting-started/integrations). BugSplat automatically captures critical diagnostic data such as stack traces, log files, and other runtime information. BugSplat also provides automated incident notifications, a convenient dashboard for monitoring trends and prioritizing engineering efforts, and integrations with popular development tools to maximize productivity and ship more profitable software.
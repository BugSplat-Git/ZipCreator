[![BugSplat Banner Image](https://user-images.githubusercontent.com/20464226/149019306-3186103c-5315-4dad-a499-4fd1df408475.png)](https://bugsplat.com)

# BugSplat
### **Crash and error reporting built for busy developers.**

[![Follow @bugsplatco on Twitter](https://img.shields.io/twitter/follow/bugsplatco?label=Follow%20BugSplat&style=social)](https://twitter.com/bugsplatco)
[![Join BugSplat on Discord](https://img.shields.io/discord/664965194799251487?label=Join%20Discord&logo=Discord&style=social)](https://discord.gg/bugsplat)

## 👋 Introduction

ZipCreator is .NET Standard 2.0 library for zip file creation with glob support.

## ⚙️ Installation

ZipCreator can be installed via NuGet.

```sh
Install-Package ZipCreator
```

## 🧑‍💻 Usage

Add a using statement for the `Zc` namespace

```cs
using Zc;
```

You can create an instance of `ZipCreator` from a text file that contains a list of filters separated by new lines.

```cs
// manifest.txt
// path/to/folder/**/*
// README.md
// LICENSE.md
var zipCreator = ZipCreator.CreatFromFile("manifest.txt");
```

You can also creat an instances of `ZipCreator` with the default constructor and set the `Settings` properties according to your use case.

```cs
var zipCreator = new ZipCreator();
zipCreator.Settings.Filters = new List<string>() { "path/to/folder/**/*" };
zipCreator.Settings.Interceptor = (fileInfo) => Debug.WriteLine(fileInfo.FullName);
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

The `Interceptor` property is an `Action<FileInfo>` that get's invoked with each file that will be added to the zip and is useful if you'd like to run an action on certain files before adding them to the zip.

```cs
zipCreator.Settings.Interceptor = (fileInfo) => SignTool(fileInfo.FullName);
```

The `Overwrite` property controls whether the `ZipOutputFile` file should be overwritten if it exists. If `Overwrite` is false the call to `MakeZips` will throw if `ZipOutputFile` exists.


## 🐛 About

[BugSplat](https://bugsplat.com) is a software crash and error reporting service with support for [Windows C++](https://docs.bugsplat.com/introduction/getting-started/integrations/desktop/cplusplus), [.NET Framework](https://docs.bugsplat.com/introduction/getting-started/integrations/desktop/windows-dot-net-framework), [dotnet](https://docs.bugsplat.com/introduction/getting-started/integrations/cross-platform/dot-net-standard) and [many more](https://docs.bugsplat.com/introduction/getting-started/integrations). BugSplat automatically captures critical diagnostic data such as stack traces, log files, and other runtime information. BugSplat also provides automated incident notifications, a convenient dashboard for monitoring trends and prioritizing engineering efforts, and integrations with popular development tools to maximize productivity and ship more profitable software.
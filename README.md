## H.Dependencies.DepsJson

[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/H.Dependencies/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/HavenDV/H.Dependencies.svg?label=License&maxAge=86400)](LICENSE.md) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Standard%202.0-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
[![Build Status](https://github.com/HavenDV/H.Dependencies/workflows/.NET%20Core/badge.svg?branch=master)](https://github.com/HavenDV/H.Dependencies/actions?query=workflow%3A%22.NET+Core%22)

Searches full paths of dependencies in .deps.json files.

### Nuget

[![NuGet](https://img.shields.io/nuget/dt/H.Dependencies.DepsJson.svg?style=flat-square&label=H.Dependencies.DepsJson)](https://www.nuget.org/packages/H.Dependencies.DepsJson/)
```
Install-Package H.Dependencies.DepsJson
```

### Usage

```csharp
using H.Dependencies;

var json = File.ReadAllText("path-to-file.deps.json");
var paths = DepsJsonDependenciesSearcher.Search(json);
```
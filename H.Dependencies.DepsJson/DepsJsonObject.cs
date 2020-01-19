using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable

namespace H.Dependencies
{
    internal class DepsJsonObject
    {
        [JsonProperty("runtimeTarget")]
        public RuntimeTarget? RuntimeTarget { get; set; }

        [JsonProperty("compilationOptions")]
        public object? CompilationOptions { get; set; }

        [JsonProperty("targets")]
        public Dictionary<string, Dictionary<string, NugetPackage>>? Targets { get; set; }

        [JsonProperty("libraries")]
        public Dictionary<string, Library>? Libraries { get; set; }
    }

    internal class Library
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("serviceable")]
        public bool? Serviceable { get; set; }

        [JsonProperty("sha512")]
        public string? Sha512 { get; set; }

        [JsonProperty("path")]
        public string? Path { get; set; }

        [JsonProperty("hashPath")]
        public string? HashPath { get; set; }
    }

    internal class RuntimeTarget
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("signature")]
        public string? Signature { get; set; }
    }

    internal class NugetPackage
    {
        [JsonProperty("dependencies")]
        public Dictionary<string, string>? Dependencies { get; set; }

        [JsonProperty("runtime")]
        public Dictionary<string, object>? Runtime { get; set; }
    }
}

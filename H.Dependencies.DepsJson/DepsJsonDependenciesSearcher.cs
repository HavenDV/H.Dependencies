using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace H.Dependencies
{
    public static class DepsJsonDependenciesSearcher
    {
        public static IList<string> Search(string json, string? nugetFolder = null)
        {
            json = json ?? throw new ArgumentNullException(nameof(json));
            nugetFolder ??= Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nuget", "packages");

            var obj = JsonConvert.DeserializeObject<DepsJsonObject>(json);
            if (obj == null)
            {
                throw new ArgumentException("Invalid json: Deserialized object is null");
            }

            var dependencies = obj.Targets
                .Where(i => i.Value != null)
                .SelectMany(i => i.Value)
                .Where(i => i.Value?.Runtime != null)
                .SelectMany(i => i.Value?.Runtime.Select(j =>
                    j.Key.StartsWith("lib")
                        ? Path.Combine(nugetFolder, i.Key, j.Key)
                        : j.Key))
                .ToList();

            return dependencies;
        }
    }
}

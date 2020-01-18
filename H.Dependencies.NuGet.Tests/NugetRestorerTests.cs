using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Dependencies.Tests
{
    [TestClass]
    public class NugetRestorerTests
    {
        [TestMethod]
        public async Task RestoreRecursiveTest()
        {
            var folder = Path.Combine(Path.GetTempPath(), nameof(NugetRestorerTests));

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            await NugetRestorer.RestoreRecursiveAsync(folder, 
                "H.Pipes", 
                ".NETStandard",
                new Version(2, 1),
                ignoredNames: new []{ "NETStandard.Library" });

            Console.WriteLine("New files:");
            foreach (var path in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine($" - {path.Replace(folder, string.Empty)}");
            }
        }

        [TestMethod]
        public async Task GetDependenciesTest()
        {
            var dependencies = await NugetRestorer.GetDependenciesRecursiveAsync(
                "H.Pipes", 
                ".NETStandard", 
                new Version(2, 1),
                ignoredNames: new [] { "NETStandard.Library" });

            Trace.WriteLine("Dependencies:");

            foreach (var dependency in dependencies)
            {
                Trace.WriteLine($" - {dependency.Id}: {dependency.VersionRange}");
            }
        }
    }
}

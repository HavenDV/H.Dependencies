using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Dependencies.Tests
{
    [TestClass]
    public class DependenciesSearcherTests
    {
        [TestMethod]
        public async Task SearchRecursiveTest()
        {
            var dependencies = await DependenciesSearcher.SearchRecursiveAsync(
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

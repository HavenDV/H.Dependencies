using System;
using System.Linq;
using H.Dependencies.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Dependencies.Tests
{
    [TestClass]
    public class DepsJsonDependenciesSearcherTests
    {
        public void BaseTest(string name)
        {
            var json = ResourcesUtilities.ReadFileAsString(name);
            var paths = DepsJsonDependenciesSearcher.Search(json);

            Console.WriteLine("Dependencies:");
            foreach (var path in paths)
            {
                Console.WriteLine($" - {path}");
            }

            Assert.IsNotNull(paths);
            Assert.IsTrue(paths.Any(), "paths.Any()");
        }

        [TestMethod]
        public void SearchTest()
        {
            BaseTest("NAudioRecorder.deps.json");
        }

        [TestMethod]
        public void SearchWithRuntimeTest()
        {
            BaseTest("YandexConverter.deps.json");
        }
    }
}

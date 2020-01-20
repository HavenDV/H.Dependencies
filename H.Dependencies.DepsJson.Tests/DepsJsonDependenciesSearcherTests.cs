using System;
using H.Dependencies.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Dependencies.Tests
{
    [TestClass]
    public class DepsJsonDependenciesSearcherTests
    {
        public void BaseTest(string name, int expectedCount)
        {
            var json = ResourcesUtilities.ReadFileAsString(name);
            var paths = DepsJsonDependenciesSearcher.Search(json);

            Console.WriteLine("Dependencies:");
            foreach (var path in paths)
            {
                Console.WriteLine($" - {path}");
            }

            Assert.IsNotNull(paths);
            Assert.AreEqual(expectedCount, paths.Count, nameof(paths.Count));
        }

        [TestMethod]
        public void SearchTest()
        {
            BaseTest("NAudioRecorder.deps.json", 3);
        }

        [TestMethod]
        public void SearchWithRuntimeTest()
        {
            BaseTest("YandexConverter.deps.json", 11);
        }
    }
}

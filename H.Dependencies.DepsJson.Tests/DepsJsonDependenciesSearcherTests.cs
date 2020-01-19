using H.Dependencies.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Dependencies.Tests
{
    [TestClass]
    public class DepsJsonDependenciesSearcherTests
    {
        [TestMethod]
        public void SearchTest()
        {
            var json = ResourcesUtilities.ReadFileAsString("NAudioRecorder.deps.json");
            var paths = DepsJsonDependenciesSearcher.Search(json);

            Assert.IsNotNull(paths);
        }
    }
}

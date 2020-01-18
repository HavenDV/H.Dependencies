using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Dependencies.Tests
{
    [TestClass]
    public class RestorerTests
    {
        [TestMethod]
        public async Task RestoreRecursiveTest()
        {
            var folder = Path.Combine(Path.GetTempPath(), nameof(RestorerTests));

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            await Restorer.RestoreRecursiveAsync(folder, 
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
    }
}

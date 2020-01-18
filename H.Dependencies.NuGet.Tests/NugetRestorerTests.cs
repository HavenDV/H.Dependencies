using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Dependencies.Tests
{
    [TestClass]
    public class NugetRestorerTests
    {
        [TestMethod]
        public async Task ConnectTest()
        {
            var folder = Path.Combine(Path.GetTempPath(), nameof(NugetRestorerTests));

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            await NugetRestorer.RestoreAsync(folder, "H.Pipes");

            Console.WriteLine("New files:");
            foreach (var path in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine($" - {path.Replace(folder, string.Empty)}");
            }
        }
    }
}

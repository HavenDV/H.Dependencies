using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace H.Dependencies
{
    internal static class NuGetUtilities
    {
        public static async Task<PackageIdentity> GetIdentityAsync(
            SourceRepository sourceRepository,
            string packageName,
            string? version = default,
            CancellationToken cancellationToken = default)
        {
            if (version != null)
            {
                return new PackageIdentity(packageName, NuGetVersion.Parse(version));
            }

            var search = await sourceRepository.GetResourceAsync<PackageSearchResource>(cancellationToken);
            var packages = await search.SearchAsync(packageName, new SearchFilter(true)
            {
                IncludeDelisted = true,
            }, 0, 10, null, cancellationToken);

            return packages.First().Identity;
        }

        public static SourceRepository GetSourceRepository()
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());

            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");

            return new SourceRepository(packageSource, providers);
        }
    }
}

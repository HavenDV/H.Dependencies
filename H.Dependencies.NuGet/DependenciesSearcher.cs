using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace H.Dependencies
{
    public static class DependenciesSearcher
    {
        #region Public methods

        public static async Task<IList<PackageDependency>> SearchAsync(
            SourceRepository sourceRepository,
            string packageName,
            string targetFramework,
            Version targetFrameworkVersion,
            string? version = default,
            CancellationToken cancellationToken = default)
        {
            var identity = await NuGetUtilities.GetIdentityAsync(sourceRepository, packageName, version, cancellationToken);
            var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>(cancellationToken);
            var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                identity, 
                new NuGetFramework(targetFramework, targetFrameworkVersion), 
                new SourceCacheContext(), 
                new NullLogger(), 
                cancellationToken);

            return dependencyInfo
                .Dependencies
                .ToList();
        }

        public static async Task<IList<PackageDependency>> SearchRecursiveAsync(
            string packageName,
            string targetFramework,
            Version targetFrameworkVersion,
            string? version = default,
            int maxDepth = 99,
            IEnumerable<string>? ignoredNames = default,
            CancellationToken cancellationToken = default)
        {
            var dependencies = new List<PackageDependency>();
            var sourceRepository = NuGetUtilities.GetSourceRepository();

            var identity = await NuGetUtilities.GetIdentityAsync(sourceRepository, packageName, version, cancellationToken);
            var currentDependencies = new List<PackageDependency>
            {
                new PackageDependency(identity.Id, new VersionRange(identity.Version))
            };
            for (var i = 0; i < maxDepth && currentDependencies.Any(); i++)
            {
                var tasks = currentDependencies
                    .Select(async dependency => await SearchAsync(
                        sourceRepository,
                        dependency.Id,
                        targetFramework,
                        targetFrameworkVersion, 
                        dependency.VersionRange.MinVersion.OriginalVersion,
                        cancellationToken));

                currentDependencies = (await Task.WhenAll(tasks))
                    .SelectMany(dependency => dependency)
                    .Where(dependency => ignoredNames?.Contains(dependency.Id) != true)
                    .ToList();

                dependencies.AddRange(currentDependencies);
            }

            return dependencies.Distinct(new PackageDependencyComparer()).ToList();
        }

        #endregion
    }
}

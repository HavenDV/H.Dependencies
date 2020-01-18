using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;

namespace H.Dependencies
{
    public static class NugetRestorer
    {
        #region Public methods

        public static async Task<PackageIdentity> RestoreAsync(
            string folder, 
            string packageName, 
            string? version = default, 
            CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(folder);

            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);

            var settings = Settings.LoadDefaultSettings(folder);
            var packageSourceProvider = new PackageSourceProvider(settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);

            var identity = await GetIdentityAsync(sourceRepository, packageName, version, cancellationToken);
            
            var project = new FolderNuGetProject(folder);
            var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, folder)
            {
                PackagesFolderNuGetProject = project
            };
            await packageManager.InstallPackageAsync(
                project,
                identity,
                new ResolutionContext(
                    DependencyBehavior.Ignore,
                    true,
                    true,
                    VersionConstraints.None),
                new EmptyNuGetProjectContext(), 
                sourceRepository,
                new List<SourceRepository>(),
                cancellationToken);

            return identity;
        }

        public static async Task RestoreRecursiveAsync(
            string folder,
            string packageName,
            string targetFramework,
            Version targetFrameworkVersion,
            string? version = default,
            int maxDepth = 99,
            IEnumerable<string>? ignoredNames = default,
            CancellationToken cancellationToken = default)
        {
            var identity = await RestoreAsync(folder, packageName, version, cancellationToken);

            var dependencies = await GetDependenciesRecursiveAsync(
                packageName,
                targetFramework, 
                targetFrameworkVersion,
                identity.Version.OriginalVersion,
                maxDepth,
                ignoredNames, 
                cancellationToken);

            await Task.WhenAll(dependencies
                .Select(async dependency => await RestoreAsync(
                    folder, 
                    dependency.Id, 
                    dependency.VersionRange.MinVersion.OriginalVersion, 
                    cancellationToken)));
        }

        public static async Task<IList<PackageDependency>> GetDependenciesAsync(
            SourceRepository sourceRepository,
            string packageName,
            string targetFramework,
            Version targetFrameworkVersion,
            string? version = default,
            CancellationToken cancellationToken = default)
        {
            var identity = await GetIdentityAsync(sourceRepository, packageName, version, cancellationToken);
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

        public static async Task<IList<PackageDependency>> GetDependenciesRecursiveAsync(
            string packageName,
            string targetFramework,
            Version targetFrameworkVersion,
            string? version = default,
            int maxDepth = 99,
            IEnumerable<string>? ignoredNames = default,
            CancellationToken cancellationToken = default)
        {
            var dependencies = new List<PackageDependency>();
            var sourceRepository = GetSourceRepository();

            var identity = await GetIdentityAsync(sourceRepository, packageName, version, cancellationToken);
            var currentDependencies = new List<PackageDependency>
            {
                new PackageDependency(identity.Id, new VersionRange(identity.Version))
            };
            for (var i = 0; i < maxDepth && currentDependencies.Any(); i++)
            {
                var tasks = currentDependencies
                    .Select(async dependency => await GetDependenciesAsync(
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

        #region Private methods

        private static async Task<PackageIdentity> GetIdentityAsync(
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

        private static SourceRepository GetSourceRepository()
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());

            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");

            return new SourceRepository(packageSource, providers);
        }

        #endregion
    }
}

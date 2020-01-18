using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;

namespace H.Dependencies
{
    public static class Restorer
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

            var identity = await NuGetUtilities.GetIdentityAsync(sourceRepository, packageName, version, cancellationToken);
            
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

            var dependencies = await DependenciesSearcher.SearchRecursiveAsync(
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

        #endregion
    }
}

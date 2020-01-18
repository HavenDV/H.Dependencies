using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;

namespace H.Dependencies
{
    public static class NugetRestorer
    {
        public static async Task RestoreAsync(string folder, string packageName, CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(folder);

            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);

            var settings = Settings.LoadDefaultSettings(folder);
            var packageSourceProvider = new PackageSourceProvider(settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);

            var search = await sourceRepository.GetResourceAsync<PackageSearchResource>(cancellationToken);
            var packages = await search.SearchAsync(packageName, new SearchFilter(true)
            {
                IncludeDelisted = true,
            }, 0, 10, null, cancellationToken);
            
            INuGetProjectContext projectContext = new EmptyNuGetProjectContext();
            var resolutionContext = new ResolutionContext(
                DependencyBehavior.Ignore,
                true,
                true,
                VersionConstraints.None);

            var package = packages.First();

            var project = new FolderNuGetProject(folder);
            var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, folder)
            {
                PackagesFolderNuGetProject = project
            };
            await packageManager.InstallPackageAsync(
                project,
                package.Identity,
                resolutionContext,
                projectContext,
                sourceRepository,
                new List<SourceRepository>(),
                cancellationToken);

            var infos = await packageManager.GetInstalledPackagesDependencyInfo(project, cancellationToken, true);
            var test = await sourceRepository.GetResourceAsync<DependencyInfoResource>(cancellationToken);
            var www = await test.ResolvePackage(package.Identity, NuGetFramework.AnyFramework,
                new SourceCacheContext(), new ProjectContextLogger(projectContext), cancellationToken);

            var tes2t = test;
        }
    }
}

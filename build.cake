DirectoryPath GetErrorCodesRoot([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
{
    return new FilePath(sourceFilePath).GetDirectory();
}

///
/// Private namespace
///
{
    ///
    /// Root path configuration
    ///
    DirectoryPath rootDirectory = GetErrorCodesRoot();
    string rootPath = rootDirectory.FullPath;

    ///
    /// Argument parsing
    ///
    string configuration = Argument("configuration", "Debug");
    string starNugetPath = Argument("starNugetPath", "");
    if (string.IsNullOrEmpty(starNugetPath))
    {
        starNugetPath = EnvironmentVariable("STAR_NUGET") ?? $"{rootPath}/artifacts";
    }

    var errorCodesProj = rootPath + "/src/Starcounter.ErrorCodes/Starcounter.ErrorCodes.csproj";
    var errorCodesTestProj = rootPath + "/test/Starcounter.ErrorCodes.Tests/Starcounter.ErrorCodes.Tests.csproj";

    ///
    /// Dependent targets
    ///
    Task("BuildErrorCodes")
        .IsDependentOn("RestoreErrorCodes")
        .IsDependentOn("BuildErrorCodesI");

    Task("TestErrorCodes")
        .IsDependentOn("RestoreErrorCodes")
        .IsDependentOn("BuildErrorCodesI")
        .IsDependentOn("TestErrorCodesI");

    Task("PackErrorCodes")
        .IsDependentOn("RestoreErrorCodes")
        .IsDependentOn("BuildErrorCodesI")
        .IsDependentOn("TestErrorCodesI")
        .IsDependentOn("PackErrorCodesI");

    ///
    /// Task for restoring ErrorCodes
    ///
    Task("RestoreErrorCodes").Does(() =>
    {
        var settings = new DotNetCoreRestoreSettings
        {
            NoCache = true
        };

        DotNetCoreRestore(errorCodesProj, settings);
        DotNetCoreRestore(errorCodesTestProj, settings);
    });

    ///
    /// Task for building ErrorCodes
    ///
    Task("BuildErrorCodesI").Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings
            {
                MaxCpuCount = 1
            },
            NoRestore = true
        };

        DotNetCoreBuild(errorCodesProj, settings);
        DotNetCoreBuild(errorCodesTestProj, settings);
    });

    ///
    /// Task for testing ErrorCodes
    ///
    Task("TestErrorCodesI").Does(() => 
    {
        var file = new FilePath(errorCodesTestProj);
        var dir = file.GetDirectory();
        var projNameWithoutExtension = file.GetFilenameWithoutExtension();

        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            DiagnosticFile = dir + "/" + projNameWithoutExtension + ".log",
            NoBuild = true,
            NoRestore = true
        };

        DotNetCoreTest(file.FullPath, settings);
    });

    ///
    /// Task for packaging ErrorCodes
    ///
    Task("PackErrorCodesI").Does(() =>
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = starNugetPath,
            NoRestore = true,
            NoBuild = true
        };

        DotNetCorePack(errorCodesProj, settings);
    });

    ///
    /// Run targets if invoked as self-containment script
    ///
    if (!Tasks.Any(t => t.Name.Equals("Bifrost")))
    {
        // Read targets argument
        IEnumerable<string> targetsArg = Argument("targets", "Pack").Split(new Char[]{',', ' '}).Where(s => !string.IsNullOrEmpty(s));

        // Self-containment dependent targets
        Task("Restore").IsDependentOn("RestoreErrorCodes");
        Task("Build").IsDependentOn("BuildErrorCodes");
        Task("Test").IsDependentOn("TestErrorCodes");
        Task("Pack").IsDependentOn("PackErrorCodes");

        // Run target
        foreach (string t in targetsArg)
        {
            RunTarget(t);
        }
    }
}
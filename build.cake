///
/// Private namespace
///
{
    ///
    /// Root path configuration
    ///
    string rootPath;

    if (Tasks.Any(t => t.Name.Equals("Bifrost")))
    {
        // Executed from Bifrost
        rootPath = MakeAbsolute(Directory("../Starcounter.ErrorCodes")).FullPath;
    }
    else
    {
        // Executed as a self-containment script
        rootPath = MakeAbsolute(Directory(".")).FullPath;
    }

    ///
    /// Argument parsing
    ///
    string configuration = Argument("configuration", "Debug");
    string starNugetPath = Argument("starNugetPath", "");
    if (string.IsNullOrEmpty(starNugetPath))
    {
        starNugetPath = rootPath + "/artifacts";
    }

    var errorCodesProj = rootPath + "/src/Starcounter.ErrorCodes/Starcounter.ErrorCodes.csproj";

    ///
    /// Dependent targets
    ///
    Task("BuildErrorCodes")
        .IsDependentOn("RestoreErrorCodes")
        .IsDependentOn("BuildErrorCodesI");

    Task("PackErrorCodes")
        .IsDependentOn("RestoreErrorCodes")
        .IsDependentOn("BuildErrorCodesI")
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
        Task("Pack").IsDependentOn("PackErrorCodes");

        // Run target
        foreach (string t in targetsArg)
        {
            RunTarget(t);
        }
    }
}
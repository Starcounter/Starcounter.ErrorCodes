///
/// Root path configuration
///
string errorCodesRootPath;

if (Tasks.Any(t => t.Name.Equals("Bifrost")))
{
    // Executed from Bifrost
    errorCodesRootPath = MakeAbsolute(Directory("../Starcounter.ErrorCodes")).FullPath;
}
else
{
    // Executed as a self-containment script
    errorCodesRootPath = MakeAbsolute(Directory("..")).FullPath;
}

///
/// Argument parsing 
///
string errorCodesConfiguration = Argument("configuration", "Debug");
string errorCodesStarNugetPath = Argument("starNugetPath", "");
if (string.IsNullOrEmpty(errorCodesStarNugetPath))
{
    errorCodesStarNugetPath = errorCodesRootPath + "/artifacts";
}

var errorCodesProj = errorCodesRootPath + "/src/Starcounter.ErrorCodes/Starcounter.ErrorCodes.csproj";

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
/// Tasks
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
/// Tasks
///
Task("BuildErrorCodesI").Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = errorCodesConfiguration,
        NoRestore = true
    };

    DotNetCoreBuild(errorCodesProj, settings);
});

///
/// Tasks
///
Task("PackErrorCodesI").Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = errorCodesConfiguration,
        OutputDirectory = errorCodesStarNugetPath,
        NoRestore = true,
        NoBuild = true
    };

    DotNetCorePack(errorCodesProj, settings);
});
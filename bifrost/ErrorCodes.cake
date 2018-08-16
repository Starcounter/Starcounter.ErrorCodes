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

///
/// Tasks
///
Task("PackErrorCodes").Does(() =>
{   
    var errorCodesProj = errorCodesRootPath + "/src/Starcounter.ErrorCodes/Starcounter.ErrorCodes.csproj";

    // restore
    DotNetCoreRestore(errorCodesProj);

    // clean
    var dotNetCoreCleanSettings = new DotNetCoreCleanSettings 
    {
        Configuration = errorCodesConfiguration
    };
    DotNetCoreClean(errorCodesProj, dotNetCoreCleanSettings);

    // pack
    var dotNetCorePackSettings = new DotNetCorePackSettings
    {
        Configuration = errorCodesConfiguration,
        OutputDirectory = errorCodesStarNugetPath
    };
    DotNetCorePack(errorCodesProj, dotNetCorePackSettings);
});
///
/// Global configuration
///
string errorCodesRootPath = Environment.GetEnvironmentVariable("CakeErrorCodesPath");
if (string.IsNullOrEmpty(errorCodesRootPath))
{
    errorCodesRootPath = MakeAbsolute(Directory("..")).FullPath;
    Information("ErrorCodes root full path: {0}", errorCodesRootPath);
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
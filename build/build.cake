///
/// Usage:
///     cake.exe
///     cake.exe build.cake
///     cake.exe build.cake --targets="PackErrorCodes"
///     cake.exe build.cake --targets="PackErrorCodes" --verbosity=diagnostic
///
/// All of the commands above does the same thing

///
/// Preprocessor Directives
///
#load "ErrorCodes.cake"

///
/// Arguments
///
IEnumerable<string> errorCodesTargetsArg = Argument("targets", "PackErrorCodes").Split(new Char[]{',', ' '}).Where(s => !string.IsNullOrEmpty(s));

///
/// Run target
///
foreach (string t in errorCodesTargetsArg)
{
    RunTarget(t);
}
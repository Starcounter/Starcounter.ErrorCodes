:: GitCommitBaseline = The number to subtract from commitcount to get the minorversion
:: %1 = Configuration to build and pack (Release/Debug)
:: %2 = Versionstring to be used instead of automatic versioning

@ECHO OFF
SETLOCAL EnableDelayedExpansion

SET GitCommitBaseline=53
SET MajorVersion=0
SET PatchVersion=0

:: TODO! Dont use hardcoded path.
pushd ..\artifacts
IF EXIST *.nupkg DEL *.nupkg
popd

SET Configuration=%1
IF "%Configuration%"=="" (
    SET Configuration=Release
)

IF "%2"=="" (
    FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
        SET GitCommitCount=%%F
    )
    SET /a MinorVersion=!GitCommitCount!-!GitCommitBaseline!
    SET VersionText=!MajorVersion!.!MinorVersion!.!PatchVersion!
) ELSE (
    SET VersionText=%2
)

ECHO Configuration: %Configuration%
ECHO Version: %VersionText%

pushd ..\src\Starcounter.ErrorCodes

dotnet restore 
dotnet clean /p:Configuration=%Configuration%
dotnet pack /p:Version=%VersionText% /p:Configuration=%Configuration%

popd
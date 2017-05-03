:: GitCommitBaseline = The number to subtract from commitcount to get the minorversion
:: %1 = Configuration to build and pack (Release/Debug)
:: %2 = Versionstring to be used instead of automatic versioning

@ECHO OFF
SETLOCAL EnableDelayedExpansion

SET GitCommitBaseline=39

SET Configuration=%1
IF "%Configuration%"=="" (
    SET Configuration=Release
)

IF "%2"=="" (
    IF EXIST "majorversion.txt" (
        SET /p MajorVersion=<majorversion.txt
    ) ELSE (
        SET MajorVersion=0
    )

    FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
        SET GitCommitCount=%%F
    )
    SET /a MinorVersion=!GitCommitCount!-!GitCommitBaseline!
    SET VersionText=!MajorVersion!.!MinorVersion!.0
) ELSE (
    SET VersionText=%2
)

ECHO Configuration: %Configuration%
ECHO Version: %VersionText%

pushd ..\src\Starcounter.ErrorCodes

dotnet clean /p:Configuration=%Configuration%
dotnet pack /p:Version=%VersionText% /p:Configuration=%Configuration%

popd
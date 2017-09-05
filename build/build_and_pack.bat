:: GitCommitBaseline = The number to subtract from commitcount to get the minorversion
:: %1 = Configuration to build and pack (Release/Debug)
:: %2 = Versionstring to be used instead of automatic versioning

@ECHO OFF
SETLOCAL EnableDelayedExpansion

SET GitCommitBaseline=70
SET MajorVersion=0
SET PatchVersion=0

IF NOT EXIST ..\artifacts MKDIR ..\artifacts

pushd ..\artifacts
IF EXIST *.nupkg DEL *.nupkg
popd

IF "%Configuration%"=="" (
    SET Configuration=%1
)

IF "%Configuration%"=="" (
    SET Configuration=Release
)

IF "%Configuration%"=="Release" GOTO GET_VERSION
IF "%Configuration%"=="Debug" GOTO GET_VERSION

ECHO Invalid configuration specified: %Configuration%
GOTO FAILURE

:GET_VERSION

IF "%2"=="" (
    FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
        SET GitCommitCount=%%F
    )

    IF "!GitCommitCount!"=="" (
        ECHO Could not retrieve git commit count from repository.
        GOTO FAILURE
    )

    SET /a MinorVersion=!GitCommitCount!-!GitCommitBaseline!
    SET VersionText=!MajorVersion!.!MinorVersion!.!PatchVersion!
) ELSE (
    SET VersionText=%2
)

:BUILD_AND_PACK

ECHO Configuration: %Configuration%
ECHO Version: %VersionText%

pushd ..\src\Starcounter.ErrorCodes

dotnet restore
IF ERRORLEVEL 1 (
    ECHO Restore failed.
    GOTO FAILURE
)

dotnet clean /p:Configuration=%Configuration%
IF ERRORLEVEL 1 (
    ECHO Clean failed.
    GOTO FAILURE
)

dotnet pack /p:Version=%VersionText% /p:Configuration=%Configuration% /m:1
IF ERRORLEVEL 1 (
    ECHO Build and pack failed.
    GOTO FAILURE
) 

popd

EXIT /b 0

:FAILURE
EXIT /b 1
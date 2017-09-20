@ECHO OFF
SETLOCAL EnableDelayedExpansion

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

IF "%Configuration%"=="Release" GOTO START_BUILD
IF "%Configuration%"=="Debug" GOTO START_BUILD

ECHO Invalid configuration specified: %Configuration%
GOTO FAILURE

:START_BUILD
ECHO Configuration: %Configuration%

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

dotnet pack /p:Configuration=%Configuration%
IF ERRORLEVEL 1 (
    ECHO Build and pack failed.
    GOTO FAILURE
) 

popd

EXIT /b 0

:FAILURE
EXIT /b 1
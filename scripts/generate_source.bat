@ECHO OFF

PUSHD "..\src"
IF NOT EXIST "errorcodes.xml" (
    GOTO MISSING_ERRORCODES
)

IF NOT EXIST generated (
    mkdir generated
)

PUSHD "Starcounter.ErrorCodes.Generator"
dotnet restore
IF ERRORLEVEL 1 GOTO DOTNET_FAIL

dotnet run ..\errorcodes.xml -cs ..\generated\errorcodes.cs
IF ERRORLEVEL 1 GOTO DOTNET_FAIL

POPD
GOTO END

:MISSING_ERRORCODES
ECHO No files with errorcodes found (%cd%src\errorcodes.xml).
GOTO END

:DOTNET_FAIL
POPD

:END
POPD
ECHO Done.
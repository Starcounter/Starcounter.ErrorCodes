@ECHO OFF

PUSHD "..\src"
IF NOT EXIST "errorcodes.xml" (
    GOTO MISSING_ERRORCODES
)

IF NOT EXIST generated (
    mkdir generated
)

PUSHD "scerrcc"
dotnet restore
IF ERRORLEVEL 1 GOTO DOTNET_FAIL

dotnet run ..\errorcodes.xml -mc ..\generated\errorcodes.mc -cs ..\generated\errorcodes.cs -ea ..\generated\errorcodes.ea
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
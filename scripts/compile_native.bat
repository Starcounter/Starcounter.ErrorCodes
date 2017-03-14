@ECHO OFF

SET "OUTDIR=src\scerrres\bin\Debug"
SET "OBJDIR=src\scerrres\obj"
SET "GENDIR=src\generated"
SET "VCPATH=C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC"
SET "WIN32DIR=x86"
SET "WIN64DIR=x64"

PUSHD ".."
IF NOT EXIST %GENDIR% GOTO SRC_NOT_FOUND
IF NOT EXIST %GENDIR%\errorcodes.mc GOTO SRC_NOT_FOUND

IF NOT EXIST %OBJDIR%\%WIN64DIR% (
    mkdir %OBJDIR%\%WIN64DIR%
)

IF NOT EXIST %OBJDIR%\%WIN32DIR% (
    mkdir %OBJDIR%\%WIN32DIR%
)

IF NOT EXIST %OUTDIR% (
    mkdir %OUTDIR%
)

IF NOT EXIST %OUTDIR%\%WIN64DIR% (
    mkdir %OUTDIR%\%WIN64DIR%
)

IF NOT EXIST %OUTDIR%\%WIN32DIR% (
    mkdir %OUTDIR%\%WIN32DIR%
)

CALL "%VCPATH%\vcvarsall.bat" amd64

mc.exe -r "%GENDIR%" -h "%GENDIR%" "%GENDIR%\errorcodes.mc"
IF ERRORLEVEL 1 GOTO MC_FAILED

rc.exe /V "%GENDIR%\errorcodes.rc"
IF ERRORLEVEL 1 GOTO RC_FAILED

cl.exe /c /D FORMATMESSAGE_EXPORTS /Fo%OBJDIR%\%WIN64DIR%\scerrres.obj /DEBUG /Zi /Fd%OUTDIR%\%WIN64DIR%\scerrres.pdb ".\src\scerrres\Format.cpp"
IF ERRORLEVEL 1 GOTO CL_FAILED

link.exe /MACHINE:X64 /DLL /DEBUG /PDB:%OUTDIR%\%WIN64DIR%\scerrres.pdb /DEF:.\src\scerrres\format.def /OUT:%OUTDIR%\%WIN64DIR%\scerrres.dll %GENDIR%\errorcodes.res %OBJDIR%\%WIN64DIR%\scerrres.obj
IF ERRORLEVEL 1 GOTO LINK_FAILED

CALL "%VCPATH%\vcvarsall.bat" x86

cl.exe /c /D FORMATMESSAGE_EXPORTS /Fo%OBJDIR%\%WIN32DIR%\scerrres.obj /DEBUG /Zi /Fd%OUTDIR%\%WIN32DIR%\scerrres.pdb ".\src\scerrres\Format.cpp"
IF ERRORLEVEL 1 GOTO CL_FAILED

link.exe /MACHINE:X86 /DLL /DEBUG /PDB:%OUTDIR%\%WIN32DIR%\scerrres.pdb /DEF:.\src\scerrres\format.def /OUT:%OUTDIR%\%WIN32DIR%\scerrres.dll %GENDIR%\errorcodes.res %OBJDIR%\%WIN32DIR%\scerrres.obj
IF ERRORLEVEL 1 GOTO LINK_FAILED

GOTO END

:SRC_NOT_FOUND
echo No source files found. Make sure they are generated (by scerrcc.exe) before invoking this script.
GOTO END

:MC_FAILED
ECHO Compiling message text file failed (mc.exe).
GOTO END

:RC_FAILED
ECHO Generating resource files failed (rc.exe).
GOTO END

:CL_FAILED
ECHO Compiling scerrres failed (cl.exe).
GOTO END

:LINK_FAILED
ECHO Linking scerrres and resources failed (link.exe)
GOTO END

:END
POPD
ECHO Done.
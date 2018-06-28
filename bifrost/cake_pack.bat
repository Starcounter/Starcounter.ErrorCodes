:: Pack ErrorCodes
::    usage: cake_pack.bat
::           cake_pack.bat [package output directory]
::           cake_pack.bat C:\GitHub\Starcounter\Starcounter.Nova\%STAR_NUGET%

@echo off

set package_output_dir=%1

if "%package_output_dir%" == "" (
    set star_nuget_path_argument=
) else (
    set star_nuget_path_argument=--starNugetPath="%package_output_dir%"
)

call %~dp0/restore_cake.bat || goto error

set executeCommand=%~dp0tools/Cake/Cake.exe %~dp0build.cake --targets="PackErrorCodes" %star_nuget_path_argument% --verbosity=Normal
echo Executing: %executeCommand%
%executeCommand% || goto error

exit /b 0

:error
echo.
echo Exit with code %errorlevel%
echo.
exit /b %errorlevel%
@ECHO OFF

:: SET "NUGET_URL=https://www.myget.org/F/starcounter/api/v2/package"
:: SET "NUGET_SS_URL=https://www.myget.org/F/starcounter/symbols/api/v2/package"

SET "NUGET_URL=\\127.0.0.1\Share"
:: SET "NUGET_SS_URL=\\127.0.0.1\Share"

IF "%StarcounterNuGetKeyFull%"=="" (
	ECHO StarcounterNuGetKeyFull is not set
	EXIT /b 990
)

dotnet nuget push --source %NUGET_URL% --api-key %StarcounterNuGetKeyFull%  ..\artifacts\

IF ERRORLEVEL 1 (
    ECHO Push failed. 
) ELSE (
    ECHO Push succeeded.
)
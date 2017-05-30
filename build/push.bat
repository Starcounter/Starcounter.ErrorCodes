@ECHO OFF

SET "NUGET_URL=https://www.myget.org/F/starcounter/api/v2/package"
:: SET "NUGET_SS_URL=https://www.myget.org/F/starcounter/symbols/api/v2/package"

IF "%MYGET_API_KEY%"=="" (
	ECHO Myget API key not set. Skipping push.
	EXIT /b 0
)

dotnet nuget push --source %NUGET_URL% --api-key %MYGET_API_KEY%  ..\artifacts\

IF ERRORLEVEL 1 (
    ECHO Push failed. 
) ELSE (
    ECHO Push succeeded.
)
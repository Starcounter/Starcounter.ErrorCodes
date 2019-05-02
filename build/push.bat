@ECHO OFF

SET "NUGET_URL=https://api.nuget.org/v3/index.json"

IF "%GIT_ERRORCODES_BRANCH%" NEQ "master" (
    ECHO Not building main branch 'master'. Skipping push.
    EXIT /b 0
)

IF "%NUGET_API_KEY%"=="" (
	ECHO NuGet API key not set. Skipping push
	EXIT /b 0
)

dotnet nuget push --source %NUGET_URL% --api-key %NUGET_API_KEY%  ..\artifacts\

IF ERRORLEVEL 1 (
    ECHO Push failed. 
) ELSE (
    ECHO Push succeeded.
)
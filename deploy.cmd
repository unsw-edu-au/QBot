@if "%SCM_TRACE_LEVEL%" NEQ "4" @echo off

IF "%SITE_ROLE%" == "bot" (
  deploy.bot.cmd
) ELSE (
	IF "%SITE_ROLE%" == "dashboard" (
		deploy.dashboard.cmd
	) ELSE (
		IF "%SITE_ROLE%" == "questions" (
			deploy.questions.cmd
		) ELSE (
			IF "%SITE_ROLE%" == "function" (
				deploy.function.cmd
			) ELSE (
				echo You have to set SITE_ROLE setting to either "bot" or "configuration"
				exit /b 1
			)
		)
	)
)
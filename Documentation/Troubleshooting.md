## Bot is having code issues
After initial setup, you will see the "Sorry, my bot code is having an issue" response when trying to interact with QBot within a channel.
Often this is due to misconfigured QBot API settings. QBot solution uses `System.Diagnostics.Trace` to output errors and debugging messages.

1. Find the **QBot API App Service** resource in the Azure portal
2. Go to **App Service Logs** blade then turn on Application Logging (File System). Set Level to Verbose
3. Open up **Log Stream** blade, and watch for error messages that might be a clue to the error.

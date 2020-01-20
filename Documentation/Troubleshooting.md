## Bot is having code issues
After initial setup, you will see the "Sorry, my bot code is having an issue" response when trying to interact with QBot within a channel.
Often this is due to misconfigured QBot API settings. QBot solution uses `System.Diagnostics.Trace` to output errors and debugging messages.

1. Find the **QBot API App Service** resource in the Azure portal
2. Go to **App Service Logs** blade then turn on Application Logging (File System). Set Level to Verbose
3. Open up **Log Stream** blade, and watch for error messages that might be a clue to the error.

## Configuration and data
If the bot encounters an error when it is tagged, the issue could be due to the bot being unable to find a user in it's database. This can happen if new users are added to the Team, without synchronising the user to the bot database via the Dashboard Tab.

## Teams Bot Debugging
QBot is a bot deployed in Teams. Please refer to the documentation on [how to debug a teams bot](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/debug/locally-with-an-ide)

## Didn't find your problem here?
Please report the issue [here](https://github.com/unsw-edu-au/QBot/issues/new)

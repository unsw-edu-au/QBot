# Troubleshooting

## Generic possible issues

There are certain issues that can arise that are common to many of the app templates. Please check [here](https://github.com/OfficeDev/microsoft-teams-stickers-app/wiki/Troubleshooting) for reference to these.

## Problems deploying to Azure

### 1. Error when attempting to reuse a Microsoft Azure AD application ID for the bot registration

#### Description

Bot is not valid. Creating the resource of type Microsoft.BotService/botServices failed with status "BadRequest"

This happens when the Microsoft Azure application ID entered during the setup of the deployment has already been used and registered for a bot.

#### Fix

Either register a new Microsoft Azure AD application or delete the bot registration that is currently using the attempted Microsoft Azure application ID.

### 2. Bot is unable to create more KBs and store additional questions

#### Description

Bot will reply to the user post with the error message if it finds that it cannot store any additional QnA pair to the Knowledge base

```
Errors: I cannot save this qna pair due to storage space limitations. Please contact your system administrator to provision additional storage space.
```

#### Fix

In case of such a scenario, system administrator or the app installer will need to update the pricing tier accordingly for QnA service in Azure Portal.

### 3. Error while deploying the ARM Template

#### Description

This happens when the resources are already created or due to some conflicts.
```
Errors: The resource operation completed with terminal provisioning state 'Failed'

```
#### Fix

In case of such a scenario, user needs to navigate to deployment center section of failed/conflict resources through the azure portal and check the error logs to get the actual errors and fix it accordingly.

Redeploy it after fixing the issue/conflict.

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

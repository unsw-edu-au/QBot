# Microsoft Teams QBot

## [Overview](#overview)
QBot is a solution designed for classroom teaching scenarios which allow teachers, tutors and students to intelligently answer each other's questions within the Microsoft Teams collaboration platform. It leverages the power of Azure Cognitive Services, in particular QnA Maker to achieve this.

The solution was originally developed, and is currently deployed for the School of Mechanical and Manufacturing Engineering at the University of New South Wales (UNSW) in Sydney, Australia.

## [How It Works](#how-it-works)
Once QBot is deployed to a Microsoft Team channel, a student can ask a question on the channel by @tagging QBot. QBot can be tagged or used contextually: for example, a student may write “I have a @question regarding the units used to measure force?”, or “I don’t know how to differentiate the function for bending moment for @question 2.14 of the tutorial problems”

QBot interprets the message, and checks QnA Maker knowledge base for the answer. If an answer is found, QBot will notify the student and respond to the original conversation thread with with an adaptive card.

Otherwise, QBot will record the question as "unanswered", whilst @tagging a tutor within the course. The tutor visits a custom "Questions" tab deployed to the Teams Channel, and inputs their answer. Note that other students can also collaborate and try to answer this question from their fellow classmates, however only the original poster, tutor or lecturer can "accept" the answer.

Once "accepted", QBot takes the answer and trains the QnA Maker knowledge base for future reference. This aims to progressively build a knowledge database of high quality successfully answered questions.

The QBot solution also deploys additional custom tabs within the Team to help with management of courses, students and questions:

**Questions Tab** - Displays the consolidated list of answered and unanswered questions posted within the channel. Can be accessed by all students and tutors.

**Dashboard Tab** - Allows management and configuration of courses, students, roles and other general QBot cognitive services settings. Can only be accessed by course administrators.
* For “student” role, view all questions they asked or answered, filterable by topic.
* For “demonstrator” role, view all questions for each tutorial group they oversee.
* For “lecturer” role, view all questions for each tutorial group as well as access to QBot configuration.


QBot also supports usage with multiple courses. Each course is run within it's own Microsoft Team.

## [Deployment Architecture](#architecture)

![overview](Documentation\overview.png)

#### [Teams](#teams)
The QBot solution bot manifest file is uploaded to each course run within a Microsoft Team. This enables the bot itself, as well as additional tabs to support QBot functionality.

#### [Question Bot Connector](#bot-connector)
A Microsoft Bot Channels Registration. The messaging endpoint will be point to the [Question Bot App Service](#question-bot-app-service) and must enable connection to a Microsoft Teams channel.


#### [Question Bot App Service](#question-bot-app-service)
This is an .NET Web API hosted within an Azure App Service.
It handles the bot conversation and workflow logic using the Microsoft Bot Framework V3 SDK. In addition, it also provides API method calls to support management of QBot application used by the [Question](#question-tab) and [Dashboard](#dashboard-tab) tabs.

This API also connects to
* Azure SQL Server Database using Entity Framework 6.0
* Graph API


#### [Question Tab](#question-tab)
This is an Angular 6 application deployed to an Azure App Service. Within the QBot solution, it is hosted within a Microsoft Teams tab. It displays a summary of all  questions asked by users within the channel (both answered and unanswered).


#### [Dashboard Tab](#dashboard-tab)
This is an Angular 6 application deployed to an Azure App Service. Within the QBot solution, it is hosted within a Microsoft Teams static tab. It faciliates administrative functions used to manage the QBot solution within the team.

#### [QnA Maker Service](#qnamaker)
The QnA Maker knowledge base drives all answering and learning aspects of the QBot.

#### [SQL Server Database](#sql-server)
Stores the list of users, roles, tutorial groups, user-tutorial mappings, assessments, questions, attachments and QR code references, and the configuration settings.

#### [Graph API](#graph-api)
RESTful Http API calls from the Bot to get the replies within a conversation thread.
Authenticated using service account (username/password) for delegate permissions


## [Setup Instructions](#setup)
Check out the [full instructions](Documentation\setup.md) on how to provision and configure the QBot solution.

## [Data Model](#data)
The QBot solution uses a SQL Server database to :
* Store users and their roles
* Manage courses and Tutorials
* Manage technical configuration for each course (such as End)
* Manage the list of unanswered questions and their originating conversations

The schema is:

![datamodel](Documentation\datamodel.png)


## [Solution Walkthrough](#code)

The solution deploys a single .NET Web API project, and 2x Angular 6 projects. This section gives an overview on all projects included in the solution source code


### Microsoft.Teams.Apps.Qbot

This is a .NET WebAPI project with 2 controllers:

**MessagesController** - Handles interactions and message processing for the Bot itself. Implementation is based on the Microsoft Bot Builder Framework v3.

The main `Post([FromBody]Microsoft.Bot.Connector.Activity activity)` method has the following workflow:
* Processes each activity from Teams based on the 'Microsoft.Bot.Connector.ActivityTypes' type
* If type is `ActivityTypes.Message`, the Controller will query QnA Maker for an answer, or tag a tutor/lecturer if it cannot find one. The logic and creation of the response is focused in the `RootDialog.cs` file.
* If it is an `ActivityTypes.Invoke`, this means the user has selected an answer. The Controller will update the questions list within SQL Server. It will also train QnA Maker by calling `generateAnswer` as well as Patch (to add new Q&A pairs), then re-publishing the knowledge base.

**RequestController** - Handles additional functions to support Qbot administrative functions.
Main methods:



|API Method|Controller Method|Description|
|:-|:-|:-|
|api/Request/GetTeamGroupIdsWithQuestions|`GetTeamGroupIdsWithQuestions(string tenantId, string upn)`|Gets list of Team IDs for which at least one question has been asked|
|api/Request/GetTeamGroupDetail|`GetTeamGroupDetail(string groupId)`|Queries Graph API to get details of a Team based on the ID|
|api/Request/GetUserByUpn|`GetUserByUpn(string upn)`|Retrieves user details including role and tutorial membership|


Key functionality includes methods to:
* Mark question as answered
* Retrieve and maintain list of courses in the system
* Retrieve and maintain list of users and their roles in the system
* Graph API to get the list of Teams

These will be used by the 2x Angular front-end apps - `DashboardTabApp` and `QuestionsTabApp`.


### Microsoft.Teams.Apps.QBot.Data
The Qbot solution uses a SQL Server to store application related data and Entity Framework (EF) as the ORM.
This project contains the EF entities and some "adapter" helper methods to facilitate database communications.
A "Database First" approach to generate entities.


### Microsoft.Teams.Apps.QBot.Models
This project contains data models that faciliate messaging between `Microsoft.Teams.Apps.QBot` WebAPI and the 


### DashboardTabApp
This is an Angular 6 web application that is deployed as a static teams tab (accessed via bot one-to-one chat. It handles general bot configuration and is intended to be used by course administrators only.
[Technical details about how this is created]


### QuestionsTabApp
This is an Angular 6 web application deployed as a configurable teams tab (can be added per channel). It displays the consolidated list of answered and unanswered questions posted within the channel, and is intended to be used by both students and tutors.
[Technical details about how this is created]

CREATE TABLE [dbo].[Course] (
    [Id]                               INT              IDENTITY (1, 1) NOT NULL,
    [Name]                             NVARCHAR (255)   NOT NULL,
    [GroupId]                          UNIQUEIDENTIFIER NOT NULL,
    [PredictiveQnAServiceHost]         NVARCHAR (1024)  NOT NULL,
    [PredictiveQnAKnowledgeBaseId]     NVARCHAR (512)   NOT NULL,
    [PredictiveQnAEndpointKey]         NVARCHAR (512)   NOT NULL,
    [PredictiveQnAHttpEndpoint]        NVARCHAR (1024)  NOT NULL,
    [PredictiveQnAHttpKey]             NVARCHAR (1024)  NOT NULL,
    [PredictiveQnAKnowledgeBaseName]   NVARCHAR (1024)  NOT NULL,
    [PredictiveQnAConfidenceThreshold] NVARCHAR (1024)  NOT NULL,
    [DeployedURL]                      NVARCHAR (1024)  NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED ([Id] ASC)
);


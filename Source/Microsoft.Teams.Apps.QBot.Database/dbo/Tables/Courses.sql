CREATE TABLE [dbo].[Courses] (
    [CourseID]                         INT            IDENTITY (1, 1) NOT NULL,
    [CourseName]                       NVARCHAR (MAX) NOT NULL,
    [PredictiveQnAServiceHost]         NVARCHAR (MAX) NOT NULL,
    [PredictiveQnAKnowledgeBaseId]     NVARCHAR (MAX) NOT NULL,
    [PredictiveQnAEndpointKey]         NVARCHAR (MAX) NOT NULL,
    [PredictiveQnAHttpEndpoint]        NVARCHAR (MAX) NOT NULL,
    [PredictiveQnAHttpKey]             NVARCHAR (MAX) NOT NULL,
    [PredictiveQnAKnowledgeBaseName]   NVARCHAR (MAX) NOT NULL,
    [PredictiveQnAConfidenceThreshold] NVARCHAR (MAX) NOT NULL,
    [DeployedURL]                      NVARCHAR (MAX) NULL,
    [GroupId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED ([CourseID] ASC)
);


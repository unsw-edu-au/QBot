CREATE TABLE [dbo].[TutorialGroups] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Code]     NVARCHAR (50)  NULL,
    [Class]    NVARCHAR (10)  NULL,
    [Location] NVARCHAR (MAX) NULL,
    [CourseID] INT            NULL,
    CONSTRAINT [PK_TutorialGroups] PRIMARY KEY CLUSTERED ([Id] ASC)
);


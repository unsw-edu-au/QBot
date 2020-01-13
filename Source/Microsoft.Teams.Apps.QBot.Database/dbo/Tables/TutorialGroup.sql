CREATE TABLE [dbo].[TutorialGroup] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [CourseId] INT            NOT NULL,
    [Code]     NVARCHAR (50)  NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_TutorialGroup] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TutorialGroup_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_TutorialGroup_CourseId]
    ON [dbo].[TutorialGroup]([CourseId] ASC);


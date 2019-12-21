CREATE TABLE [dbo].[UserTutorialGroups] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [UserId]          INT NOT NULL,
    [TutorialGroupId] INT NOT NULL,
    CONSTRAINT [PK_UserTutorialGroups] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserTutorialGroups_TutorialGroups] FOREIGN KEY ([TutorialGroupId]) REFERENCES [dbo].[TutorialGroups] ([Id]),
    CONSTRAINT [FK_UserTutorialGroups_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);


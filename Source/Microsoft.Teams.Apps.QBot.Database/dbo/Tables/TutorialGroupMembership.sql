CREATE TABLE [dbo].[TutorialGroupMembership] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [TutorialGroupId] INT NOT NULL,
    [UserId]          INT NOT NULL,
    CONSTRAINT [PK_TutorialGroupMembership] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TutorialGroupMembership_TutorialGroupId] FOREIGN KEY ([TutorialGroupId]) REFERENCES [dbo].[TutorialGroup] ([Id]),
    CONSTRAINT [FK_TutorialGroupMembership_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_TutorialGroupMembership_TutorialGroupId]
    ON [dbo].[TutorialGroupMembership]([TutorialGroupId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TutorialGroupMembership_UserId]
    ON [dbo].[TutorialGroupMembership]([UserId] ASC);


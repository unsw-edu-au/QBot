CREATE TABLE [dbo].[UserCourseRoleMapping] (
    [Id]       INT IDENTITY (1, 1) NOT NULL,
    [UserId]   INT NOT NULL,
    [CourseId] INT NOT NULL,
    [RoleId]   INT NOT NULL,
    CONSTRAINT [PK_UserCourseRoleMapping] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserCourseRoleMapping_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]),
    CONSTRAINT [FK_UserCourseRoleMapping_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]),
    CONSTRAINT [FK_UserCourseRoleMapping_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_UserCourseRoleMapping_CourseId]
    ON [dbo].[UserCourseRoleMapping]([CourseId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserCourseRoleMapping_RoleId]
    ON [dbo].[UserCourseRoleMapping]([RoleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserCourseRoleMapping_UserId]
    ON [dbo].[UserCourseRoleMapping]([UserId] ASC);


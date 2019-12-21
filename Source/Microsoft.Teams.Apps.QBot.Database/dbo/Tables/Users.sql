CREATE TABLE [dbo].[Users] (
    [Id]                              INT            IDENTITY (1, 1) NOT NULL,
    [StudentId]                       NVARCHAR (50)  NULL,
    [FirstName]                       NVARCHAR (MAX) NULL,
    [LastName]                        NVARCHAR (MAX) NULL,
    [Username]                        NVARCHAR (MAX) NULL,
    [Email]                           NVARCHAR (MAX) NULL,
    [RoleId]                          INT            NULL,
    [PersonalConversationContactData] NVARCHAR (MAX) NULL,
    [CourseID]                        INT            NULL,
    [Temp_CourseName]                 NVARCHAR (MAX) NULL,
    [Temp_Tutorial]                   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [StudentIDCourseID] UNIQUE NONCLUSTERED ([StudentId] ASC, [CourseID] ASC)
);


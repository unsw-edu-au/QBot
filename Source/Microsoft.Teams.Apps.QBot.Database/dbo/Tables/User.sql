CREATE TABLE [dbo].[User] (
    [Id]                              INT            IDENTITY (1, 1) NOT NULL,
    [UserPrincipalName]               NVARCHAR (255) NULL,
    [StudentId]                       NVARCHAR (255) NULL,
    [FirstName]                       NVARCHAR (255) NULL,
    [LastName]                        NVARCHAR (255) NULL,
    [Email]                           NVARCHAR (255) NULL,
    [PersonalConversationContactData] NVARCHAR (MAX) NULL,
    [IsGlobalAdmin]                   BIT            NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Username] UNIQUE NONCLUSTERED ([UserPrincipalName] ASC)
);


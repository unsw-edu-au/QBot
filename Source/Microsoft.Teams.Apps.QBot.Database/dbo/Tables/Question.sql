CREATE TABLE [dbo].[Question] (
    [Id]                   INT              IDENTITY (1, 1) NOT NULL,
    [CourseId]             INT              NOT NULL,
    [TenantId]             UNIQUEIDENTIFIER NULL,
    [GroupId]              UNIQUEIDENTIFIER NULL,
    [TeamId]               NVARCHAR (2048)  NULL,
    [TeamName]             NVARCHAR (2048)  NULL,
    [ConversationId]       NVARCHAR (2048)  NULL,
    [MessageId]            NVARCHAR (2048)  NULL,
    [Topic]                NVARCHAR (100)   NULL,
    [Status]               NVARCHAR (50)    NULL,
    [QuestionText]         NVARCHAR (MAX)   NULL,
    [OriginalPosterId]     INT              NOT NULL,
    [DateSubmitted]        DATETIME         NOT NULL,
    [Link]                 NVARCHAR (2048)  NULL,
    [AnswerText]           NVARCHAR (MAX)   NULL,
    [AnswerMessageId]      NVARCHAR (2048)  NULL,
    [AnswerPosterId]       INT              NULL,
    [DateAnswered]         DATETIME         NULL,
    [AnswerCardActivityId] NVARCHAR (2048)  NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Question_AnswerPoster] FOREIGN KEY ([AnswerPosterId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Question_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]),
    CONSTRAINT [FK_Question_OriginalPoster] FOREIGN KEY ([OriginalPosterId]) REFERENCES [dbo].[User] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Question_CourseId]
    ON [dbo].[Question]([CourseId] ASC);


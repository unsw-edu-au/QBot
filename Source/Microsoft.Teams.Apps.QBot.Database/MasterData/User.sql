SET IDENTITY_INSERT [dbo].[User] ON 

IF NOT EXISTS(SELECT [Id] FROM [dbo].[User] WHERE [Id] = 1)
BEGIN
	INSERT [dbo].[User] ([Id], [UserPrincipalName], [StudentId], [FirstName], [LastName], [Email], [PersonalConversationContactData], [IsGlobalAdmin]) VALUES (1, N'questionbot@university.edu', N'0', N'Question', N'Bot', N'questionbot@university.edu', NULL, 0)
END

SET IDENTITY_INSERT [dbo].[User] OFF
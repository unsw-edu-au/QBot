SET IDENTITY_INSERT [dbo].[Users] ON 

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Users] WHERE [Id] = 1)
BEGIN
	INSERT [dbo].[Users] ([Id], [StudentId], [FirstName], [LastName], [Username], [Email], [RoleId], [PersonalConversationContactData], [CourseID], [Temp_CourseName], [Temp_Tutorial]) VALUES (1, N'0', N'Question', N'Bot', N'questionbot@university.edu.au', N'questionbot@university.edu.au', 4, NULL, 0, NULL, NULL)
END

SET IDENTITY_INSERT [dbo].[Users] OFF
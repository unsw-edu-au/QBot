SET IDENTITY_INSERT [dbo].[Role] ON 

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Role] WHERE [Id] = 1)
BEGIN
	INSERT [dbo].[Role] ([Id], [Name]) VALUES (1, N'Lecturer')
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Role] WHERE [Id] = 2)
BEGIN
	INSERT [dbo].[Role] ([Id], [Name]) VALUES (2, N'Demonstrator')
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Role] WHERE [Id] = 3)
BEGIN
	INSERT [dbo].[Role] ([Id], [Name]) VALUES (3, N'Student')
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Role] WHERE [Id] = 4)
BEGIN
	INSERT [dbo].[Role] ([Id], [Name]) VALUES (4, N'Bot')
END

SET IDENTITY_INSERT [dbo].[Role] OFF
SET IDENTITY_INSERT [dbo].[Roles] ON 

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Roles] WHERE [Id] = 1)
BEGIN
	INSERT [dbo].[Roles] ([Id], [RoleName]) VALUES (1, N'Lecturer')
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Roles] WHERE [Id] = 2)
BEGIN
	INSERT [dbo].[Roles] ([Id], [RoleName]) VALUES (2, N'Demonstrator')
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Roles] WHERE [Id] = 3)
BEGIN
	INSERT [dbo].[Roles] ([Id], [RoleName]) VALUES (3, N'Student')
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[Roles] WHERE [Id] = 4)
BEGIN
	INSERT [dbo].[Roles] ([Id], [RoleName]) VALUES (4, N'Bot')
END

SET IDENTITY_INSERT [dbo].[Roles] OFF
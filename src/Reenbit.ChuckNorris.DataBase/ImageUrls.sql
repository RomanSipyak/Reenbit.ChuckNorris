﻿CREATE TABLE [dbo].[ImageUrls]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Value] NVARCHAR(2048) NOT NULL,
	[JokeId] INT NOT NULL,
	CONSTRAINT [FK_JokeImageUrls_To_User] FOREIGN KEY ([JokeId]) REFERENCES Jokes([Id]),
)

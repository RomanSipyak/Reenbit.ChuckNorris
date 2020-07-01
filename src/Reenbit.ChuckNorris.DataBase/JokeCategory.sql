CREATE TABLE [dbo].[JokeCategory]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Joke_Id] INT NOT NULL, 
    [Category_Id] INT NOT NULL,
    CONSTRAINT [FK_JokeCategory_To_Joke] FOREIGN KEY ([Joke_Id]) REFERENCES Jokes([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_JokeCategory_To_Category] FOREIGN KEY ([Category_Id]) REFERENCES Categories([Id]) ON DELETE CASCADE,
);

GO

CREATE UNIQUE INDEX JokeId_CategotyId_Index ON [dbo].[JokeCategory] ([Joke_Id], [Category_Id]);

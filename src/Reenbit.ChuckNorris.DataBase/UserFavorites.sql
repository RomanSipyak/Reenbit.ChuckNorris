CREATE TABLE [dbo].[UserFavorites]
(
	[UserId] INT NOT NULL, 
    [JokeId] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [FK_UserFavorites_To_User] FOREIGN KEY ([UserId]) REFERENCES AspNetUsers([Id]),
    CONSTRAINT [FK_UserFavorites_To_Joke] FOREIGN KEY ([JokeId]) REFERENCES Jokes([Id]),
    CONSTRAINT [PK_UserFavorites] PRIMARY KEY ([UserId], [JokeId]), 
)

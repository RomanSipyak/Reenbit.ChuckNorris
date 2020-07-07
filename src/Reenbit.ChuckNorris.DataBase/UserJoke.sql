CREATE TABLE [dbo].[UserJoke]
(
	[UserId] INT NOT NULL,
    [JokeId] INT NOT NULL,
    [Favourite] BIT NULL DEFAULT 0, 
    CONSTRAINT [FK_UserJoke_To_Joke] FOREIGN KEY ([JokeId]) REFERENCES Jokes([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserJoke_To_User] FOREIGN KEY ([UserId]) REFERENCES AspNetUsers([Id]) ON DELETE CASCADE,
    CONSTRAINT [PK_UserJoke] PRIMARY KEY ([JokeId], [UserId]),
)

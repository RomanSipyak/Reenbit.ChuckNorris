﻿CREATE TABLE [dbo].[JokeCategory]
( 
    [JokeId] INT NOT NULL,
    [CategoryId] INT NOT NULL,
    CONSTRAINT [FK_JokeCategory_To_Joke] FOREIGN KEY ([JokeId]) REFERENCES Jokes([Id]),
    CONSTRAINT [FK_JokeCategory_To_Category] FOREIGN KEY ([CategoryId]) REFERENCES Categories([Id]),
    CONSTRAINT [PK_JokeCategory] PRIMARY KEY ([JokeId], [CategoryId]),
);

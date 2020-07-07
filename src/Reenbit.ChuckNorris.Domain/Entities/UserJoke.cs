﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class UserJoke
    {
        public int JokeId { get; set; }

        public Joke Joke { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public bool Favourite { get; set; }
    }
}

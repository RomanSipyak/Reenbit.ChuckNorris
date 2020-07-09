﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class UserFavorite : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int JokeId { get; set; }

        public Joke Joke { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<ICollection<string>> GetAllCategoriesAsync();
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS
{
    public class CreateCategoryDto
    {
        [Required]
        public string Title { get; set; }
    }
}

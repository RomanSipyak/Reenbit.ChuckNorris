using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Services.CustomExceptions
{
    public class CategoryNotFoundException : ApplicationException
    {
        public CategoryNotFoundException()
        {
        }

        public CategoryNotFoundException(string message)
            : base(message)
        {
        }

        public CategoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

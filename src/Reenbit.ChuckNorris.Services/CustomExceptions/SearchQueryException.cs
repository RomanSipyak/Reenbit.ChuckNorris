using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Services.CustomExceptions
{
    public class SearchQueryException : ApplicationException
    {
        public SearchQueryException()
        {
        }

        public SearchQueryException(string message)
            : base(message)
        {
        }

        public SearchQueryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

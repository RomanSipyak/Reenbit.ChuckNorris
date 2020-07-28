using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Emails.EmailDTOs
{
    public class ResetDataDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ResetPasswordUrl { get; set; }
    }
}

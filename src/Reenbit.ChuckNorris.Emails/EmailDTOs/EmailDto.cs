using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Emails.EmailDTOs
{
    public class EmailDto
    {
        public IEnumerable<string> To { get; set; }

        public string Subject { get; set; }

        public dynamic DataModel { get; set; }

        public EmailTemplates Template { get; set; }

        public string HtmlBody { get; set; }
    }
}

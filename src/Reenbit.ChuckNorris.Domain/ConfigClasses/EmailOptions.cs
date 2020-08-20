using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.ConfigClasses
{
    public class EmailOptions
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public string EmailKey { get; set; }
    }
}

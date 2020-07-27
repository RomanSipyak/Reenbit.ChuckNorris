using Reenbit.ChuckNorris.Emails.EmailDTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Emails.Abstractions
{
    public interface IEmailSender
    {
        Task<bool> Send(EmailDto emailToSend);
    }
}

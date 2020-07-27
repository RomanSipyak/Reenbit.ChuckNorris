using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Emails.Abstractions
{
    public interface IEmailService
    {
        Task SendRestorePasswordEmail(User user, string token);
    }
}

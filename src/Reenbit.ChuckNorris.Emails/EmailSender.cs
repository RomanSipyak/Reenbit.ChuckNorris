using Microsoft.Extensions.Options;
using Reenbit.ChuckNorris.Domain.ConfigClasses;
using Reenbit.ChuckNorris.Emails.Abstractions;
using Reenbit.ChuckNorris.Emails.EmailDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Emails
{
    public class EmailSender : IEmailSender
    {
        private readonly Regex mailRegex;

        private readonly IOptions<EmailOptions> emailOptions;

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            this.emailOptions = emailOptions;

            this.mailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))"
                                       + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                                       RegexOptions.IgnoreCase);
        }

        public async Task<bool> Send(EmailDto emailDto)
        {
            var message = new MailMessage
            {
                IsBodyHtml = true
            };

            List<string> addressesList = emailDto.To.ToList();
            if (!addressesList.Any())
            {
                return false;
            }

            List<string> invalidAddresses = this.GetInvalidAddresses(addressesList);
            if (invalidAddresses.Any())
            {
                return false;
            }

            this.GetValidAddresses(addressesList).ForEach(message.To.Add);
            message.Subject = emailDto.Subject;
            message.From = new MailAddress(this.emailOptions.Value.SenderEmail);
            message.Body = emailDto.HtmlBody;

            using (SmtpClient smtp = new SmtpClient(this.emailOptions.Value.MailServer, this.emailOptions.Value.MailPort))
            {
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(this.emailOptions.Value.SenderEmail,
                                                         this.emailOptions.Value.Password);
                await smtp.SendMailAsync(message);
            }

            return true;
        }

        private List<string> GetInvalidAddresses(IEnumerable<string> addresses)
        {
            return addresses.Where(x => !this.mailRegex.Match(x).Success).ToList();
        }

        private List<string> GetValidAddresses(IEnumerable<string> addresses)
        {
            return addresses.Where(x => this.mailRegex.Match(x).Success).ToList();
        }
    }
}

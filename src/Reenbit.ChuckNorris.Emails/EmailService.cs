using Microsoft.AspNetCore.WebUtilities;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Emails.Abstractions;
using Reenbit.ChuckNorris.Emails.EmailDTOs;
using Reenbit.ChuckNorris.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Emails
{
    public class EmailService : IEmailService
    {
        private readonly ITemplateEngine templateEngine;

        private readonly IEmailSender emailSender;

        private readonly IConfigurationManager configurationManager;

        public EmailService(
            ITemplateEngine templateEngine,
            IEmailSender emailSender,
            IConfigurationManager configurationManager)
        {
            this.templateEngine = templateEngine;
            this.emailSender = emailSender;
            this.configurationManager = configurationManager;
        }

        public Task SendRestorePasswordEmail(User user, string token, string ResetPageUrl)
        {
            var emailDto = new EmailDto
            {
                To = new List<string> { user.Email },
                Subject = "Password Reset",
                Template = EmailTemplates.ResetPassword
            };

            var resetPasswordUrl = GenerateRestoreUrl(user.Id, token, ResetPageUrl);
            emailDto.DataModel = new ResetDataDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ResetPasswordUrl = resetPasswordUrl
            };

            return this.SendEmail(emailDto);
        }

        private async Task SendEmail(EmailDto emailToSend)
        {
            emailToSend.HtmlBody = await this.templateEngine.CompileAsync(emailToSend.Template.ToString(), emailToSend.DataModel);

            await this.emailSender.Send(emailToSend);
        }

        private string GenerateRestoreUrl(int userId, string token, string ResetPageUrl)
        {
            return QueryHelpers.AddQueryString(
                ResetPageUrl,
                new Dictionary<string, string>
                    {
                        { "userId", userId.ToString() },
                        { "token", token }
                    });
        }
    }
}

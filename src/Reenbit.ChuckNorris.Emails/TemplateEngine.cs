using Reenbit.ChuckNorris.Emails.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Emails
{
    public class TemplateEngine : ITemplateEngine
    {
        private const string TemplatesFolderName = "Templates";

        private const string MasterTemplateFileName = "Master";

        private const string ContentBodyPlaceholder = "{{ContentBody}}";

        private static readonly Regex tokenRegex = new Regex(@"\{\{\s*(?<token>\w+)\s*\}\}");

        private static readonly string assemblyFolder = Path.GetDirectoryName(typeof(TemplateEngine).Assembly.Location);

        public async Task<string> CompileAsync<T>(string templateName, T model)
        {
            string templateFilePath = GetTemplateFilePath(templateName);
            if (!File.Exists(templateFilePath))
            {
                throw new InvalidOperationException($"Email template wasn't found by path: {templateFilePath}");
            }

            string templateText = await File.ReadAllTextAsync(templateFilePath);
            templateText = ProcessPlaceholders(templateText, model);
            string masterFilePath = GetTemplateFilePath(MasterTemplateFileName);
            string masterText = await File.ReadAllTextAsync(masterFilePath);
            templateText = masterText.Replace(ContentBodyPlaceholder, templateText);

            return templateText;
        }

        private static string ProcessPlaceholders<T>(string text, T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            MatchCollection matches = tokenRegex.Matches(text);
            IEnumerable<string> tokens = matches.Cast<Match>().Select(m => m.Groups["token"].Value).Distinct();
            var publicProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var token in tokens)
            {
                var property = publicProperties.FirstOrDefault(p => string.Equals(p.Name, token, StringComparison.InvariantCultureIgnoreCase));

                if (property != null && property.CanRead)
                {
                    object valueToReplace = property.GetValue(model) ?? string.Empty;
                    Regex replaceRegex = new Regex(@"\{\{\s*" + token + @"\s*\}\}");

                    text = replaceRegex.Replace(text, valueToReplace.ToString());
                }
            }

            return text;
        }

        private static string GetTemplateFilePath(string templateName)
        {
            return Path.Combine(assemblyFolder, TemplatesFolderName, $"{templateName}.html");
        }
    }
}

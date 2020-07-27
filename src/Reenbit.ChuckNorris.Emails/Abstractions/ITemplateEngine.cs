using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Emails.Abstractions
{
    public interface ITemplateEngine
    {
        Task<string> CompileAsync<T>(string templateName, T model);
    }
}

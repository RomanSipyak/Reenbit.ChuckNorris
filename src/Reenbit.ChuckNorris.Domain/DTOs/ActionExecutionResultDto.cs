using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs
{
    public class ActionExecutionResultDto
    {
        public bool Succeeded { get; set; } = true;

        public string Error { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmailServices
{
    public interface ISendEmail
    {
        bool Send(string Body, MemoryStream memory = null);
    }
}

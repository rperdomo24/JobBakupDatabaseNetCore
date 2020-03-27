using System;
using System.Collections.Generic;
using System.Text;

namespace BakupDB
{
    public class DatabaseConfiguration
    {
        public string ServerName { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public bool TipeLogingSecure { get; set; }
    }
}
